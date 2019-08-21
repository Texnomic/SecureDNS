using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Texnomic.SecureDNS.Data.Identity;

namespace Texnomic.SecureDNS.Areas.Identity
{
    /// <summary>
    /// An <see cref="AuthenticationStateProvider"/> service that revalidates the
    /// authentication state at regular intervals. If a signed-in user's security
    /// stamp changes, this revalidation mechanism will sign the user out.
    /// </summary>
    /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
    public class RevalidatingAuthenticationStateProvider<TUser> : AuthenticationStateProvider, IDisposable where TUser : User
    {
        private static readonly TimeSpan RevalidationInterval = TimeSpan.FromMinutes(30);

        private readonly CancellationTokenSource LoopCancellationTokenSource = new CancellationTokenSource();
        private readonly IServiceScopeFactory ScopeFactory;
        private readonly ILogger Logger;
        private Task<AuthenticationState> CurrentAuthenticationStateTask;

        public RevalidatingAuthenticationStateProvider(IServiceScopeFactory ScopeFactory, SignInManager<TUser> CircuitScopeSignInManager, ILogger<RevalidatingAuthenticationStateProvider<TUser>> Logger)
        {
            var InitialUser = CircuitScopeSignInManager.Context.User;
            CurrentAuthenticationStateTask = Task.FromResult(new AuthenticationState(InitialUser));
            this.ScopeFactory = ScopeFactory;
            this.Logger = Logger;

            if (InitialUser.Identity.IsAuthenticated)
            {
                _ = RevalidationLoop();
            }
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => CurrentAuthenticationStateTask;

        private async Task RevalidationLoop()
        {
            var CancellationToken = LoopCancellationTokenSource.Token;

            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(RevalidationInterval, CancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                var IsValid = await CheckIfAuthenticationStateIsValidAsync();

                if (IsValid) continue;

                // Force sign-out. Also stop the revalidation loop, because the user can
                // only sign back in by starting a new connection.
                var AnonymousUser = new ClaimsPrincipal(new ClaimsIdentity());

                CurrentAuthenticationStateTask = Task.FromResult(new AuthenticationState(AnonymousUser));
                NotifyAuthenticationStateChanged(CurrentAuthenticationStateTask);
                LoopCancellationTokenSource.Cancel();
            }
        }

        private async Task<bool> CheckIfAuthenticationStateIsValidAsync()
        {
            try
            {
                // Get the sign-in manager from a new scope to ensure it fetches fresh data
                using var Scope = ScopeFactory.CreateScope();
                var SignInManager = Scope.ServiceProvider.GetRequiredService<SignInManager<TUser>>();
                var AuthenticationState = await CurrentAuthenticationStateTask;
                var ValidatedUser = await SignInManager.ValidateSecurityStampAsync(AuthenticationState.User);
                return ValidatedUser != null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while revalidating authentication state");
                return false;
            }
        }

        void IDisposable.Dispose() => LoopCancellationTokenSource.Cancel();
    }
}
