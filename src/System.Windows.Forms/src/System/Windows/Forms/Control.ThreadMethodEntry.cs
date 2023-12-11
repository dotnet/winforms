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
        private ManualResetEventPrivate? _resetEvent;
        private readonly object _invokeSyncObject = new();

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
            // We need finalization only if we will create _resetEvent instance.
            GC.SuppressFinalize(this);
        }

        ~ThreadMethodEntry()
        {
            // If something new is added here, it is necessary to reconsider the conditions of finalization.
            // Look GC.SuppressFinalize(this); in constructor and GC.ReRegisterForFinalize(this); in AsyncWaitHandle.
            _resetEvent?.Close();
            Debug.Fail($"{nameof(ThreadMethodEntry)} finalization hit!");
        }

        public object? AsyncState
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
                            _resetEvent = new ManualResetEventPrivate(this);
                            GC.ReRegisterForFinalize(this);
                            if (IsCompleted)
                            {
                                _resetEvent.Set();
                            }
                        }
                    }
                }

                return _resetEvent;
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
                _resetEvent?.Set();
            }
        }

        private sealed class ManualResetEventPrivate : EventWaitHandle
        {
            private readonly object _owner;

            public ManualResetEventPrivate(object owner) : base(false, EventResetMode.ManualReset)
            {
                _owner = owner;
            }

            protected override void Dispose(bool explicitDisposing)
            {
                base.Dispose(explicitDisposing);
                if (explicitDisposing)
                {
                    // The owner (ThreadMethodEntry) need to free only this WH.
                    // But ThreadMethodEntry can't be Disposed directly because it exposed though IAsyncResult interface.
                    // So after freeing this WH we mark owner ThreadMethodEntry to suppress finalization.
                    GC.SuppressFinalize(_owner);
                }
            }
        }
    }
}
