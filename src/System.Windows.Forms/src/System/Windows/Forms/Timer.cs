// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
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

        private protected EventHandler _onTimer;

        private GCHandle _timerRoot;

        // Holder for the HWND that handles our Timer messages.
        private TimerNativeWindow _timerWindow;

        private readonly object _syncObj = new object();

        /// <summary>
        ///  Initializes a new instance of the <see cref='Timer'/> class.
        /// </summary>
        public Timer() : base()
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='Timer'/> class with the specified container.
        /// </summary>
        public Timer(IContainer container) : this()
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Add(this);
        }

        [SRCategory(nameof(SR.CatData))]
        [Localizable(false)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ControlTagDescr))]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag { get; set; }

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
                lock (_syncObj)
                {
                    if (_enabled != value)
                    {
                        _enabled = value;

                        // At runtime, enable or disable the corresponding Windows timer
                        if (!DesignMode)
                        {
                            if (value)
                            {
                                // Create the timer window if needed.
                                if (_timerWindow is null)
                                {
                                    _timerWindow = new TimerNativeWindow(this);
                                }

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
                lock (_syncObj)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.TimerInvalidInterval, value, 0));
                    }

                    if (_interval != value)
                    {
                        _interval = value;
                        if (Enabled)
                        {
                            // Change the timer value, don't tear down the timer itself.
                            if (!DesignMode && _timerWindow != null)
                            {
                                _timerWindow.RestartTimer(value);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Raises the <see cref='Tick'/> event.
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

        private class TimerNativeWindow : NativeWindow, IHandle
        {
            // The timer that owns the window
            private readonly Timer _owner;

            // The current id -- this is usally the same as TimerID but we also
            // use it as a flag of when our timer is running.
            private IntPtr _timerID;

            // An arbitrary timer ID.
            private static IntPtr s_timerID = (IntPtr)1;

            // Setting this when we are stopping the timer so someone can't restart it in the process.
            private bool _stoppingTimer;

            internal TimerNativeWindow(Timer owner)
            {
                _owner = owner;
            }

            ~TimerNativeWindow()
            {
                // This call will work form the finalizer thread.
                StopTimer();
            }

            public bool IsTimerRunning => _timerID != IntPtr.Zero && Handle != IntPtr.Zero;

            private bool EnsureHandle()
            {
                if (Handle == IntPtr.Zero)
                {
                    // Create a totally vanilla invisible window just for WM_TIMER messages
                    var cp = new CreateParams
                    {
                        Style = 0,
                        ExStyle = 0,
                        ClassStyle = 0,
                        Caption = GetType().Name,

                        // Message only windows are cheaper and have fewer issues than
                        // full blown invisible windows.
                        Parent = User32.HWND_MESSAGE
                    };

                    CreateHandle(cp);
                }

                Debug.Assert(Handle != IntPtr.Zero, "Could not create timer HWND.");
                return Handle != IntPtr.Zero;
            }

            /// <summary>
            ///  Returns true if we need to marshal across threads to access this timer's HWND.
            /// </summary>
            private bool GetInvokeRequired(IntPtr hWnd)
            {
                if (hWnd != IntPtr.Zero)
                {
                    return User32.GetWindowThreadProcessId(hWnd, out _) != Kernel32.GetCurrentThreadId();
                }

                return false;
            }

            /// <summary>
            ///  Changes the interval of the timer without destroying the HWND.
            /// </summary>
            public void RestartTimer(int newInterval)
            {
                StopTimer(IntPtr.Zero, destroyHwnd: false);
                StartTimer(newInterval);
            }

            public void StartTimer(int interval)
            {
                if (_timerID == IntPtr.Zero && !_stoppingTimer)
                {
                    if (EnsureHandle())
                    {
                        _timerID = User32.SetTimer(this, s_timerID, (uint)interval, IntPtr.Zero);
                        s_timerID = s_timerID + 1;
                    }
                }
            }

            public void StopTimer() => StopTimer(IntPtr.Zero, destroyHwnd: true);

            /// <summary>
            ///  Stop the timer and optionally destroy the HWND.
            /// </summary>
            public void StopTimer(IntPtr hWnd, bool destroyHwnd)
            {
                if (hWnd == IntPtr.Zero)
                {
                    hWnd = Handle;
                }

                // Fire a message across threads to destroy the timer and HWND on the thread that created it.
                if (GetInvokeRequired(hWnd))
                {
                    User32.PostMessageW(new HandleRef(this, hWnd), User32.WM.CLOSE);
                    return;
                }

                // Locking 'this' here is ok since this is an internal class.
                lock (this)
                {
                    if (_stoppingTimer || hWnd == IntPtr.Zero || User32.IsWindow(new HandleRef(this, hWnd)).IsFalse())
                    {
                        return;
                    }

                    if (_timerID != IntPtr.Zero)
                    {
                        try
                        {
                            _stoppingTimer = true;
                            User32.KillTimer(new HandleRef(this, hWnd), _timerID);
                        }
                        finally
                        {
                            _timerID = IntPtr.Zero;
                            _stoppingTimer = false;
                        }
                    }

                    if (destroyHwnd)
                    {
                        base.DestroyHandle();
                    }
                }
            }

            /// <summary>
            ///  Destroy the handle, stopping the timer first.
            /// </summary>
            public override void DestroyHandle()
            {
                // Avoid recursing.
                StopTimer(IntPtr.Zero, destroyHwnd: false);
                Debug.Assert(_timerID == IntPtr.Zero, "Destroying handle with timerID still set.");
                base.DestroyHandle();
            }

            protected override void OnThreadException(Exception e)
            {
                Application.OnThreadException(e);
            }

            public override void ReleaseHandle()
            {
                // Avoid recursing.
                StopTimer(IntPtr.Zero, destroyHwnd: false);
                Debug.Assert(_timerID == IntPtr.Zero, "Destroying handle with timerID still set.");
                base.ReleaseHandle();
            }

            protected override void WndProc(ref Message m)
            {
                Debug.Assert(m.HWnd == Handle && Handle != IntPtr.Zero, "Timer getting messages for other windows?");

                // For timer messages call the timer event.
                if (m.Msg == (int)User32.WM.TIMER)
                {
                    if (m.WParam == _timerID)
                    {
                        _owner.OnTick(EventArgs.Empty);
                        return;
                    }
                }
                else if (m.Msg == (int)User32.WM.CLOSE)
                {
                    // This is a posted method from another thread that tells us we need
                    // to kill the timer. The handle may already be gone, so we specify it here.
                    StopTimer(m.HWnd, destroyHwnd: true);
                    return;
                }

                base.WndProc(ref m);
            }
        }
    }
}
