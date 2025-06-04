// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    ///  Used with BeginInvoke/EndInvoke
    /// </summary>
    private class ThreadMethodEntry : IAsyncResult
    {
        internal Control _caller;
        internal Control _marshaler;
        internal Delegate? _method;
        internal object?[]? _args;
        internal object? _retVal;
        internal Exception? _exception;
        internal bool _synchronous;
        private ManualResetEvent? _resetEvent;
        private readonly Lock _invokeSyncObject = new();

        // Store the execution context associated with the caller thread, and
        // information about which thread actually got the stack applied to it.
        internal ExecutionContext? _executionContext;

        // Optionally store the synchronization context associated with the callee thread.
        // This overrides the sync context in the execution context of the caller thread.
        internal SynchronizationContext? _syncContext;

        internal ThreadMethodEntry(
            Control caller,
            Control marshaler,
            Delegate? method,
            object?[]? args,
            bool synchronous,
            ExecutionContext? executionContext)
        {
            _caller = caller;
            _marshaler = marshaler;
            _method = method;
            _args = args;
            _synchronous = synchronous;
            _executionContext = executionContext;
        }

        public object? AsyncState => null;

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_resetEvent is null)
                {
                    lock (_invokeSyncObject)
                    {
                        _resetEvent ??= new ManualResetEvent(false);

                        if (IsCompleted)
                        {
                            _resetEvent.Set();
                        }
                    }
                }

                return _resetEvent;
            }
        }

        public bool CompletedSynchronously => IsCompleted && _synchronous;

        public bool IsCompleted { get; private set; }

        internal void Complete()
        {
            lock (_invokeSyncObject)
            {
                IsCompleted = true;
                _resetEvent?.Set();
            }
        }
    }
}
