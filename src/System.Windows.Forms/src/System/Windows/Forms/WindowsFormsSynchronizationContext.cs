// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    ///  SynchronizationContext subclass used by the Windows Forms package.
    /// </summary>
    public sealed class WindowsFormsSynchronizationContext : SynchronizationContext, IDisposable
    {
        private Control _controlToSendTo;
        private WeakReference? _destinationThreadRef;

        //ThreadStatics won't get initialized per thread: easiest to just invert the value.
        [ThreadStatic]
        private static bool dontAutoInstall;

        [ThreadStatic]
        private static bool inSyncContextInstallation;

        [ThreadStatic]
        private static SynchronizationContext? previousSyncContext;

        public WindowsFormsSynchronizationContext()
        {
            DestinationThread = Thread.CurrentThread;   //store the current thread to ensure its still alive during an invoke.
            _controlToSendTo = Application.ThreadContext.FromCurrent().MarshalingControl;
            Debug.Assert(_controlToSendTo.IsHandleCreated, "Marshaling control should have created its handle in its ctor.");
        }

        private WindowsFormsSynchronizationContext(Control marshalingControl, Thread? destinationThread)
        {
            _controlToSendTo = marshalingControl;
            DestinationThread = destinationThread;
            Debug.Assert(_controlToSendTo.IsHandleCreated, "Marshaling control should have created its handle in its ctor.");
        }

        // Directly holding onto the Thread can prevent ThreadStatics from finalizing.
        private Thread? DestinationThread
        {
            get => _destinationThreadRef?.IsAlive == true
                ? _destinationThreadRef.Target as Thread
                : null;
            set
            {
                if (value is not null)
                {
                    _destinationThreadRef = new WeakReference(value);
                }
            }
        }

        public void Dispose()
        {
            if (_controlToSendTo is not null)
            {
                if (!_controlToSendTo.IsDisposed)
                {
                    _controlToSendTo.Dispose();
                }

                _controlToSendTo = null!;
            }
        }

        // This is never called because we decide whether to Send or Post and we always post
        public override void Send(SendOrPostCallback d, object? state)
        {
            Thread? destinationThread = DestinationThread;
            if (destinationThread is null || !destinationThread.IsAlive)
            {
                throw new InvalidAsynchronousStateException(SR.ThreadNoLongerValid);
            }

            _controlToSendTo?.Invoke(d, new object?[] { state });
        }

        public override void Post(SendOrPostCallback d, object? state)
        {
            _controlToSendTo?.BeginInvoke(d, new object?[] { state });
        }

        public override SynchronizationContext CreateCopy()
        {
            return new WindowsFormsSynchronizationContext(_controlToSendTo, DestinationThread);
        }

        // Determines whether we install the WindowsFormsSynchronizationContext when we create a control, or
        // when we start a message loop.  Default: true.
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static bool AutoInstall
        {
            get
            {
                return !dontAutoInstall;
            }
            set
            {
                dontAutoInstall = !value;
            }
        }

        // Instantiate and install a WF op sync context, and save off the old one.
        internal static void InstallIfNeeded()
        {
            // Exit if we shouldn't auto-install, if we've already installed and we haven't uninstalled,
            // or if we're being called recursively (creating the WF
            // async op sync context can create a parking window control).
            if (!AutoInstall || inSyncContextInstallation)
            {
                return;
            }

            if (SynchronizationContext.Current is null)
            {
                previousSyncContext = null;
            }

            if (previousSyncContext is not null)
            {
                return;
            }

            inSyncContextInstallation = true;
            try
            {
                SynchronizationContext currentContext = AsyncOperationManager.SynchronizationContext;
                //Make sure we either have no sync context or that we have one of type SynchronizationContext
                if (currentContext is null || currentContext.GetType() == typeof(SynchronizationContext))
                {
                    previousSyncContext = currentContext;

                    AsyncOperationManager.SynchronizationContext = new WindowsFormsSynchronizationContext();
                }
            }
            finally
            {
                inSyncContextInstallation = false;
            }
        }

        public static void Uninstall()
        {
            Uninstall(true);
        }

        internal static void Uninstall(bool turnOffAutoInstall)
        {
            if (AutoInstall)
            {
                if (AsyncOperationManager.SynchronizationContext is WindowsFormsSynchronizationContext winFormsSyncContext)
                {
                    try
                    {
                        if (previousSyncContext is null)
                        {
                            AsyncOperationManager.SynchronizationContext = new SynchronizationContext();
                        }
                        else
                        {
                            AsyncOperationManager.SynchronizationContext = previousSyncContext;
                        }
                    }
                    finally
                    {
                        previousSyncContext = null;
                    }
                }
            }

            if (turnOffAutoInstall)
            {
                AutoInstall = false;
            }
        }
    }
}
