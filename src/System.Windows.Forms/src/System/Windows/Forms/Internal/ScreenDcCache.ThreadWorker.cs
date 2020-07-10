// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;

namespace System.Windows.Forms
{
    internal sealed partial class ScreenDcCache
    {
        /// <summary>
        ///  Worker for queueing actions to a specific thread.
        /// </summary>
        private class ThreadWorker
        {
            private readonly object _lock = new object();
            private readonly ManualResetEventSlim _pending = new ManualResetEventSlim(initialState: true);
            private readonly Queue<Action> _workQueue = new Queue<Action>();
            private readonly CancellationToken _cancellationToken;

            public ThreadWorker(CancellationToken token) => _cancellationToken = token;

            public void Start()
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    // Sit idle until there is work to do.
                    _pending.Wait(_cancellationToken);

                    lock (_lock)
                    {
                        while (_workQueue.TryDequeue(out Action? action))
                        {
                            action.Invoke();
                        }

                        // Keep Set() and Reset() in the lock to avoid resetting after setting without actually
                        // dequeueing the work item.
                        _pending.Reset();
                    }
                }
            }

            public void QueueAndWaitForCompletion(Action action)
            {
                ManualResetEventSlim finished = new ManualResetEventSlim();

                void trackAction()
                {
                    action();
                    finished.Set();
                }

                lock (_lock)
                {
                    _workQueue.Enqueue(trackAction);
                    _pending.Set();
                }

#if DEBUG
                if (!finished.Wait(50))
                {
                    throw new TimeoutException("Failed to get an HDC");
                }
#else
                finished.Wait();
#endif
            }
        }
    }
}
