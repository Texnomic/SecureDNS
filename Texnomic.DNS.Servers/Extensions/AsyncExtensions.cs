using System;
using System.Threading;
using System.Threading.Tasks;

namespace Texnomic.DNS.Servers.Extensions
{
    public static class AsyncExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> Action, CancellationToken CancellationToken)
        {
            var TaskCompletionSource = new TaskCompletionSource<bool>();

            await using (CancellationToken.Register(() => TaskCompletionSource.TrySetResult(true)))
            {
                if (Action != await Task.WhenAny(Action, TaskCompletionSource.Task))
                {
                    throw new OperationCanceledException(CancellationToken);
                }
            }

            return Action.Result;
        }
    }
}