using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Texnomic.SecureDNS.Areas.Identity
{
    public class RevalidatingIdentityAuthenticationStateProvider<TUser> : RevalidatingServerAuthenticationStateProvider where TUser : class
    {
        private readonly IServiceScopeFactory ScopeFactory;
        private readonly IdentityOptions Options;

        public RevalidatingIdentityAuthenticationStateProvider(ILoggerFactory LoggerFactory, IServiceScopeFactory ScopeFactory, IOptions<IdentityOptions> OptionsAccessor)
            : base(LoggerFactory)
        {
            this.ScopeFactory = ScopeFactory;
            Options = OptionsAccessor.Value;
        }

        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

        protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState AuthenticationState, CancellationToken CancellationToken)
        {
            // Get the user manager from a new scope to ensure it fetches fresh data
            var Scope = ScopeFactory.CreateScope();

            try
            {
                var UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
                return await ValidateSecurityStampAsync(UserManager, AuthenticationState.User);
            }
            finally
            {
                if (Scope is IAsyncDisposable AsyncDisposable)
                {
                    await AsyncDisposable.DisposeAsync();
                }
                else
                {
                    Scope.Dispose();
                }
            }
        }

        private async Task<bool> ValidateSecurityStampAsync(UserManager<TUser> UserManager, ClaimsPrincipal Principal)
        {
            var User = await UserManager.GetUserAsync(Principal);
            if (User == null)
            {
                return false;
            }
            else if (!UserManager.SupportsUserSecurityStamp)
            {
                return true;
            }
            else
            {
                var PrincipalStamp = Principal.FindFirstValue(Options.ClaimsIdentity.SecurityStampClaimType);
                var UserStamp = await UserManager.GetSecurityStampAsync(User);
                return PrincipalStamp == UserStamp;
            }
        }
    }
}
