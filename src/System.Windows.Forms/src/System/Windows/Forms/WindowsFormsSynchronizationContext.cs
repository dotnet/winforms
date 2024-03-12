// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Provides a synchronization context for the Windows Forms application model.
/// </summary>
public sealed class WindowsFormsSynchronizationContext : SynchronizationContext, IDisposable
{
    private Control? _controlToSendTo;
    private WeakReference<Thread>? _destinationThread;

    // ThreadStatics won't get initialized per thread: easiest to just invert the value.
    [ThreadStatic]
    private static bool t_doNotAutoInstall;

    [ThreadStatic]
    private static bool t_inSyncContextInstallation;

    [ThreadStatic]
    private static SynchronizationContext? t_previousSyncContext;

    public WindowsFormsSynchronizationContext()
    {
        // Store the current thread to ensure it stays alive during an invoke.
        DestinationThread = Thread.CurrentThread;
        _controlToSendTo = Application.ThreadContext.FromCurrent().MarshallingControl;
        Debug.Assert(_controlToSendTo.IsHandleCreated, "Marshaling control should have created its handle in its ctor.");
    }

    private WindowsFormsSynchronizationContext(Control? marshalingControl, Thread? destinationThread)
    {
        _controlToSendTo = marshalingControl;
        DestinationThread = destinationThread;
        Debug.Assert(
            _controlToSendTo is null || _controlToSendTo.IsHandleCreated,
            "Marshaling control should have created its handle in its ctor.");
    }

    // Directly holding onto the Thread can prevent ThreadStatics from finalizing.
    private Thread? DestinationThread
    {
        get => _destinationThread?.TryGetTarget(out Thread? target) == true ? target : null;
        set
        {
            if (value is not null)
            {
                if (_destinationThread is null)
                {
                    _destinationThread = new(value);
                }
                else
                {
                    _destinationThread.SetTarget(value);
                }
            }
        }
    }

    public void Dispose()
    {
        if (_controlToSendTo is { } control)
        {
            if (!control.IsDisposed)
            {
                control.Dispose();
            }

            _controlToSendTo = null;
        }
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        // We don't call Send internally, only Post

        Thread? destinationThread = DestinationThread;
        if (destinationThread is null || !destinationThread.IsAlive)
        {
            throw new InvalidAsynchronousStateException(SR.ThreadNoLongerValid);
        }

        _controlToSendTo?.Invoke(d, [state]);
    }

    public override void Post(SendOrPostCallback d, object? state)
        => _controlToSendTo?.BeginInvoke(d, [state]);

    public override SynchronizationContext CreateCopy()
        => new WindowsFormsSynchronizationContext(_controlToSendTo, DestinationThread);

    /// <summary>
    ///  Gets or sets a value indicating whether the <see cref="WindowsFormsSynchronizationContext"/> is installed when
    ///  a control is created.
    /// </summary>
    /// <value>
    ///  <see langword="true"/> if the <see cref="WindowsFormsSynchronizationContext"/> is installed; otherwise,
    ///  <see langword="false"/>. The default is <see langword="true"/>.
    /// </value>
    /// <remarks>
    ///  <para>
    ///   The <see cref="AutoInstall"/> property determines whether the <see cref="WindowsFormsSynchronizationContext"/>
    ///   is installed when a control is created, or when a message loop is started.
    ///  </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static bool AutoInstall
    {
        get => !t_doNotAutoInstall;
        set => t_doNotAutoInstall = !value;
    }

    // Instantiate and install a WinForms op sync context, and save off the old one.
    internal static void InstallIfNeeded()
    {
        // Exit if we shouldn't auto-install, if we've already installed and we haven't uninstalled,
        // or if we're being called recursively (creating the WinForms async op sync context can create
        // a parking window control).
        if (!AutoInstall || t_inSyncContextInstallation)
        {
            return;
        }

        if (Current is null)
        {
            t_previousSyncContext = null;
        }

        if (t_previousSyncContext is not null)
        {
            return;
        }

        t_inSyncContextInstallation = true;
        try
        {
            SynchronizationContext currentContext = AsyncOperationManager.SynchronizationContext;

            // Make sure we either have no sync context or that we have one of type SynchronizationContext
            if (currentContext is null || currentContext.GetType() == typeof(SynchronizationContext))
            {
                t_previousSyncContext = currentContext;

                AsyncOperationManager.SynchronizationContext = new WindowsFormsSynchronizationContext();
            }
        }
        finally
        {
            t_inSyncContextInstallation = false;
        }
    }

    public static void Uninstall() => Uninstall(turnOffAutoInstall: true);

    internal static void Uninstall(bool turnOffAutoInstall)
    {
        if (AutoInstall && AsyncOperationManager.SynchronizationContext is WindowsFormsSynchronizationContext)
        {
            try
            {
                AsyncOperationManager.SynchronizationContext = t_previousSyncContext is null
                    ? new SynchronizationContext()
                    : t_previousSyncContext;
            }
            finally
            {
                t_previousSyncContext = null;
            }
        }

        if (turnOffAutoInstall)
        {
            AutoInstall = false;
        }
    }
}
