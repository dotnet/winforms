// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
///  Implements a Windows-based timer that raises an event at user-defined intervals.
///  This timer is optimized for use in Win Forms applications and must be used in a window.
/// </summary>
[DefaultProperty(nameof(Interval))]
[DefaultEvent(nameof(Tick))]
[ToolboxItemFilter("System.Windows.Forms")]
[SRDescription(nameof(SR.DescriptionTimer))]
public class Timer : Component
{
    private int _interval = 100;

    private bool _enabled;

    private protected EventHandler? _onTimer;

    private GCHandle _timerRoot;

    // Holder for the HWND that handles our Timer messages.
    private TimerNativeWindow? _timerWindow;

    private readonly Lock _lock = new();

    /// <summary>
    ///  Initializes a new instance of the <see cref="Timer"/> class.
    /// </summary>
    public Timer() : base()
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="Timer"/> class with the specified container.
    /// </summary>
    public Timer(IContainer container) : this()
    {
        ArgumentNullException.ThrowIfNull(container);

        container.Add(this);
    }

    [SRCategory(nameof(SR.CatData))]
    [Localizable(false)]
    [Bindable(true)]
    [SRDescription(nameof(SR.ControlTagDescr))]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag { get; set; }

    /// <summary>
    ///  Occurs when the specified timer interval has elapsed and the timer is enabled.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TimerTimerDescr))]
    public event EventHandler Tick
    {
        add => _onTimer += value;
        remove => _onTimer -= value;
    }

    /// <summary>
    ///  Disposes of the resources (other than memory) used by the timer.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timerWindow?.StopTimer();
            Enabled = false;
        }

        _timerWindow = null;
        base.Dispose(disposing);
    }

    /// <summary>
    ///  Indicates whether the timer is running.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.TimerEnabledDescr))]
    public virtual bool Enabled
    {
        get => _timerWindow is null ? _enabled : _timerWindow.IsTimerRunning;
        set
        {
            lock (_lock)
            {
                if (_enabled == value)
                {
                    return;
                }

                _enabled = value;

                // At runtime, enable or disable the corresponding Windows timer
                if (DesignMode)
                {
                    return;
                }

                if (value)
                {
                    // Create the timer window if needed.
                    _timerWindow ??= new TimerNativeWindow(this);

                    _timerRoot = GCHandle.Alloc(this);
                    _timerWindow.StartTimer(_interval);
                }
                else
                {
                    _timerWindow?.StopTimer();
                    if (_timerRoot.IsAllocated)
                    {
                        _timerRoot.Free();
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Indicates the time, in milliseconds, between timer ticks.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(100)]
    [SRDescription(nameof(SR.TimerIntervalDescr))]
    public int Interval
    {
        get => _interval;
        set
        {
            lock (_lock)
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.TimerInvalidInterval, value, 0));
                }

                if (_interval == value)
                {
                    return;
                }

                _interval = value;

                if (Enabled && !DesignMode)
                {
                    // Change the timer value, don't tear down the timer itself.
                    _timerWindow?.RestartTimer(value);
                }
            }
        }
    }

    /// <summary>
    ///  Raises the <see cref="Tick"/> event.
    /// </summary>
    protected virtual void OnTick(EventArgs e) => _onTimer?.Invoke(this, e);

    /// <summary>
    ///  Starts the timer.
    /// </summary>
    public void Start() => Enabled = true;

    /// <summary>
    ///  Stops the timer.
    /// </summary>
    public void Stop() => Enabled = false;

    public override string ToString() => $"{base.ToString()}, Interval: {Interval}";

    private class TimerNativeWindow : NativeWindow
    {
        // The timer that owns the window
        private readonly Timer _owner;

        // The current id -- this is usually the same as TimerID but we also
        // use it as a flag of when our timer is running.
        private nuint _timerID;

        // An arbitrary timer ID.
        private static nuint s_timerID = 1;

        // Setting this when we are stopping the timer so someone can't restart it in the process.
        private bool _stoppingTimer;

        private readonly Lock _lock = new();

        internal TimerNativeWindow(Timer owner)
        {
            _owner = owner;
        }

        ~TimerNativeWindow()
        {
            // This call will work form the finalizer thread.
            StopTimer();
        }

        public bool IsTimerRunning => _timerID != 0 && !HWND.IsNull;

        private bool EnsureHandle()
        {
            if (HWND.IsNull)
            {
                // Create a totally vanilla invisible window just for WM_TIMER messages
                CreateParams cp = new()
                {
                    Style = 0,
                    ExStyle = 0,
                    ClassStyle = 0,
                    Caption = GetType().Name,

                    // Message only windows are cheaper and have fewer issues than
                    // full blown invisible windows.
                    Parent = HWND.HWND_MESSAGE
                };

                CreateHandle(cp);
            }

            Debug.Assert(!HWND.IsNull, "Could not create timer HWND.");
            return !HWND.IsNull;
        }

        /// <summary>
        ///  Returns true if we need to marshal across threads to access this timer's HWND.
        /// </summary>
        private static bool GetInvokeRequired(HWND hwnd)
        {
            if (!hwnd.IsNull)
            {
                return PInvoke.GetWindowThreadProcessId(hwnd, out _) != PInvokeCore.GetCurrentThreadId();
            }

            return false;
        }

        /// <summary>
        ///  Changes the interval of the timer without destroying the HWND.
        /// </summary>
        public void RestartTimer(int newInterval)
        {
            StopTimer(default, destroyHwnd: false);
            StartTimer(newInterval);
        }

        public void StartTimer(int interval)
        {
            if (_timerID == 0 && !_stoppingTimer)
            {
                if (EnsureHandle())
                {
                    _timerID = PInvoke.SetTimer(this, s_timerID, (uint)interval);
                    s_timerID++;
                }
            }
        }

        public void StopTimer() => StopTimer(default, destroyHwnd: true);

        /// <summary>
        ///  Stop the timer and optionally destroy the HWND.
        /// </summary>
        public void StopTimer(HWND hwnd, bool destroyHwnd)
        {
            if (hwnd.IsNull)
            {
                // This is the normal use case. The hwnd only has a value if it comes back from the WndProc.
                hwnd = HWND;
            }

            // Fire a message across threads to destroy the timer and HWND on the thread that created it.
            if (GetInvokeRequired(hwnd))
            {
                PInvokeCore.PostMessage(hwnd, PInvokeCore.WM_CLOSE);
                return;
            }

            lock (_lock)
            {
                if (_stoppingTimer || hwnd.IsNull || !PInvoke.IsWindow(hwnd))
                {
                    return;
                }

                if (_timerID != 0)
                {
                    try
                    {
                        _stoppingTimer = true;
                        PInvoke.KillTimer(hwnd, _timerID);
                    }
                    finally
                    {
                        _timerID = 0;
                        _stoppingTimer = false;
                    }
                }

                if (destroyHwnd)
                {
                    base.DestroyHandle();
                }
            }

            GC.KeepAlive(this);
        }

        /// <summary>
        ///  Destroy the handle, stopping the timer first.
        /// </summary>
        public override void DestroyHandle()
        {
            // Avoid recursing.
            StopTimer(default, destroyHwnd: false);
            Debug.Assert(_timerID == 0, "Destroying handle with timerID still set.");
            base.DestroyHandle();
        }

        protected override void OnThreadException(Exception e)
        {
            Application.OnThreadException(e);
        }

        public override void ReleaseHandle()
        {
            // Avoid recursing.
            StopTimer(default, destroyHwnd: false);
            Debug.Assert(_timerID == 0, "Destroying handle with timerID still set.");
            base.ReleaseHandle();
        }

        protected override void WndProc(ref Message m)
        {
            Debug.Assert(m.HWND == HWND && !HWND.IsNull, "Timer getting messages for other windows?");

            // For timer messages call the timer event.
            if (m.MsgInternal == PInvokeCore.WM_TIMER)
            {
                if (m.WParamInternal == _timerID)
                {
                    _owner.OnTick(EventArgs.Empty);
                    return;
                }
            }
            else if (m.MsgInternal == PInvokeCore.WM_CLOSE)
            {
                // This is a posted method from another thread that tells us we need
                // to kill the timer. The handle may already be gone, so we specify it here.
                StopTimer(m.HWND, destroyHwnd: true);
                return;
            }

            base.WndProc(ref m);
        }
    }
}
