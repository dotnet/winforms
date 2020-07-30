// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Threading;

namespace System.Windows.Forms
{
    public partial class Control
    {
        /// <summary>
        ///  Used with BeginInvoke/EndInvoke
        /// </summary>
        private class ThreadMethodEntry : IAsyncResult
        {
            internal Control _caller;
            internal Control _marshaler;
            internal Delegate _method;
            internal object[] _args;
            internal object _retVal;
            internal Exception _exception;
            internal bool _synchronous;
            private ManualResetEvent _resetEvent;
            private readonly object _invokeSyncObject = new object();

            // Store the execution context associated with the caller thread, and
            // information about which thread actually got the stack applied to it.
            internal ExecutionContext _executionContext;

            // Optionally store the synchronization context associated with the callee thread.
            // This overrides the sync context in the execution context of the caller thread.
            internal SynchronizationContext _syncContext;

            internal ThreadMethodEntry(Control caller, Control marshaler, Delegate method, object[] args, bool synchronous, ExecutionContext executionContext)
            {
                _caller = caller;
                _marshaler = marshaler;
                _method = method;
                _args = args;
                _exception = null;
                _retVal = null;
                _synchronous = synchronous;
                IsCompleted = false;
                _resetEvent = null;
                _executionContext = executionContext;
            }

            ~ThreadMethodEntry()
            {
                if (_resetEvent != null)
                {
                    _resetEvent.Close();
                }
            }

            public object AsyncState
            {
                get
                {
                    return null;
                }
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    if (_resetEvent is null)
                    {
                        // Locking 'this' here is ok since this is an internal class.
                        lock (_invokeSyncObject)
                        {
                            // BeginInvoke hangs on Multi-proc system:
                            // taking the lock prevents a race condition between IsCompleted
                            // boolean flag and resetEvent mutex in multiproc scenarios.
                            if (_resetEvent is null)
                            {
                                _resetEvent = new ManualResetEvent(false);
                                if (IsCompleted)
                                {
                                    _resetEvent.Set();
                                }
                            }
                        }
                    }
                    return (WaitHandle)_resetEvent;
                }
            }

            public bool CompletedSynchronously
            {
                get
                {
                    if (IsCompleted && _synchronous)
                    {
                        return true;
                    }

                    return false;
                }
            }

            public bool IsCompleted { get; private set; }

            internal void Complete()
            {
                lock (_invokeSyncObject)
                {
                    IsCompleted = true;
                    if (_resetEvent != null)
                    {
                        _resetEvent.Set();
                    }
                }
            }
        }
    }
}
