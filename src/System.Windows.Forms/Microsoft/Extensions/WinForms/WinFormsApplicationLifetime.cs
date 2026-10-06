// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.WinForms;

/// <summary>
///  Provides lifecycle notifications for a Windows Forms application.
/// </summary>
/// <remarks>
///  <para>
///   Notifications are raised at most once and in startup, stopping, stopped
///   order. If startup fails, the started notification is omitted.
///  </para>
///  <para>
///   Runtime coordination raises these events on the application lifecycle
///   thread. Event-handler exceptions are not suppressed.
///  </para>
/// </remarks>
public sealed class WinFormsApplicationLifetime
{
    /// <summary>
    ///  Identifies the lifecycle phase used to suppress duplicate notifications.
    /// </summary>
    private enum LifecycleState
    {
        NotStarted,
        Started,
        Stopping,
        Stopped
    }

    private LifecycleState _state;
    private readonly Lock _stateLock = new();

    internal WinFormsApplicationLifetime()
    {
    }

    /// <summary>
    ///  Occurs after the application has started successfully.
    /// </summary>
    public event EventHandler? ApplicationStarted;

    /// <summary>
    ///  Occurs when application shutdown begins.
    /// </summary>
    public event EventHandler? ApplicationStopping;

    /// <summary>
    ///  Occurs after the application has stopped.
    /// </summary>
    public event EventHandler? ApplicationStopped;

    internal void NotifyApplicationStarted()
    {
        lock (_stateLock)
        {
            if (_state != LifecycleState.NotStarted)
            {
                return;
            }

            _state = LifecycleState.Started;
            ApplicationStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    internal void NotifyApplicationStopping()
    {
        lock (_stateLock)
        {
            if (_state is LifecycleState.Stopping or LifecycleState.Stopped)
            {
                return;
            }

            _state = LifecycleState.Stopping;
            ApplicationStopping?.Invoke(this, EventArgs.Empty);
        }
    }

    internal void NotifyApplicationStopped()
    {
        lock (_stateLock)
        {
            if (_state is not LifecycleState.Stopping and not LifecycleState.Stopped)
            {
                _state = LifecycleState.Stopping;
                ApplicationStopping?.Invoke(this, EventArgs.Empty);
            }

            if (_state == LifecycleState.Stopped)
            {
                return;
            }

            _state = LifecycleState.Stopped;
            ApplicationStopped?.Invoke(this, EventArgs.Empty);
        }
    }
}
