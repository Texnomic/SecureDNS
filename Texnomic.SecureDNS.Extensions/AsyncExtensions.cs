using System;
using System.Threading;
using System.Threading.Tasks;

namespace Texnomic.SecureDNS.Extensions
{
    /// <summary>
    /// Helper class to run async methods within a sync process.
    /// </summary>
    public static class Async
    {
        private static readonly TaskFactory TaskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        /// <summary>
        /// Executes an async Task method which has a void return value synchronously
        /// USAGE: AsyncUtil.RunSync(() => AsyncMethod());
        /// </summary>
        /// <param name="Task">Task method to execute</param>
        public static void RunSync(Func<Task> Task)
            => TaskFactory
                .StartNew(Task)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        /// <summary>
        /// Executes an async Task<T> method which has a T return type synchronously
        /// USAGE: T result = AsyncUtil.RunSync(() => AsyncMethod<T>());
        /// </summary>
        /// <typeparam name="TResult">Return Type</typeparam>
        /// <param name="Task">Task<T> method to execute</param>
        /// <returns></returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> Task)
            => TaskFactory
                .StartNew(Task)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

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

        public static async Task<T> WithTimeout<T>(this Task<T> Action, int Timeout)
        {
            var CancellationTokenSource = new CancellationTokenSource(Timeout);

            var TaskCompletionSource = new TaskCompletionSource<bool>();

            await using (CancellationTokenSource.Token.Register(() => TaskCompletionSource.TrySetResult(true)))
            {
                if (Action != await Task.WhenAny(Action, TaskCompletionSource.Task))
                {
                    throw new TimeoutException();
                }
            }

            return Action.Result;
        }
    }


}