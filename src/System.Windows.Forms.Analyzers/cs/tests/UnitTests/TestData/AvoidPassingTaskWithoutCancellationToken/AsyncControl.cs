using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    public class AsyncControl : Control
    {
        // BEGIN ASYNC API
        public Task InvokeAsync(
            Action callback,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource();

            // Note: Code is INCORRECT, it's just here to satisfy the compiler!
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                base.BeginInvoke(callback);
            }

            return tcs.Task;
        }

        public Task InvokeAsync<T>(
            Func<T> callback,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<T>();

            // Note: Code is INCORRECT, it's just here to satisfy the compiler!
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                base.BeginInvoke(callback);
            }

            return tcs.Task;
        }

        public Task InvokeAsync(
            Func<CancellationToken, ValueTask> callback,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource();

            // Note: Code is INCORRECT, it's just here to satisfy the compiler!
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                base.BeginInvoke(callback);
            }

            return tcs.Task;
        }

        public Task<T> InvokeAsync<T>(
            Func<CancellationToken, ValueTask<T>> callback,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<T>();
            // Note: Code is INCORRECT, it's just here to satisfy the compiler!
            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                base.BeginInvoke(callback);
            }
            return tcs.Task;
        }
        // END ASYNC API
    }
}
