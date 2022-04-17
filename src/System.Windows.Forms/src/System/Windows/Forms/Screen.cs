// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a display device or multiple display devices on a single system.
    /// </summary>
    public partial class Screen
    {
        private readonly IntPtr _hmonitor;

        /// <summary>
        ///  Bounds of the screen
        /// </summary>
        private readonly Rectangle _bounds;

        /// <summary>
        ///  Available working area on the screen. This excludes taskbars and other
        ///  docked windows.
        /// </summary>
        private Rectangle _workingArea = Rectangle.Empty;

        /// <summary>
        ///  Set to true if this screen is the primary monitor
        /// </summary>
        private readonly bool _primary;

        /// <summary>
        ///  Device name associated with this monitor
        /// </summary>
        private readonly string _deviceName;

        private readonly int _bitDepth;

        private static readonly object s_syncLock = new object(); //used to lock this class before syncing to SystemEvents

        private static int s_desktopChangedCount = -1; //static counter of desktop size changes

        private int _currentDesktopChangedCount = -1; //instance-based counter used to invalidate WorkingArea

        // This identifier is just for us, so that we don't try to call the multimon
        // functions if we just need the primary monitor... this is safer for
        // non-multimon OSes.
        private const int PRIMARY_MONITOR = unchecked((int)0xBAADF00D);

        private static readonly bool s_multiMonitorSupport = (User32.GetSystemMetrics(User32.SystemMetric.SM_CMONITORS) != 0);
        private static Screen[]? s_screens;

        internal Screen(IntPtr monitor) : this(monitor, default)
        {
        }

        internal unsafe Screen(IntPtr monitor, Gdi32.HDC hdc)
        {
            Gdi32.HDC screenDC = hdc;

            if (!s_multiMonitorSupport || monitor == (IntPtr)PRIMARY_MONITOR)
            {
                // Single monitor system
                _bounds = SystemInformation.VirtualScreen;
                _primary = true;
                _deviceName = "DISPLAY";
            }
            else
            {
                // Multiple monitor system
                var info = new User32.MONITORINFOEXW
                {
                    cbSize = (uint)sizeof(User32.MONITORINFOEXW)
                };
                User32.GetMonitorInfoW(monitor, ref info);
                _bounds = info.rcMonitor;
                _primary = ((info.dwFlags & User32.MONITORINFOF.PRIMARY) != 0);

                _deviceName = new string(info.szDevice);

                if (hdc.IsNull)
                {
                    screenDC = Gdi32.CreateDC(_deviceName, null, null, IntPtr.Zero);
                }
            }

            _hmonitor = monitor;

            _bitDepth = Gdi32.GetDeviceCaps(screenDC, Gdi32.DeviceCapability.BITSPIXEL);
            _bitDepth *= Gdi32.GetDeviceCaps(screenDC, Gdi32.DeviceCapability.PLANES);

            if (hdc != screenDC)
            {
                Gdi32.DeleteDC(screenDC);
            }
        }

        /// <summary>
        ///  Gets an array of all of the displays on the system.
        /// </summary>
        public unsafe static Screen[] AllScreens
        {
            get
            {
                if (s_screens is null)
                {
                    if (s_multiMonitorSupport)
                    {
                        MonitorEnumCallback closure = new MonitorEnumCallback();
                        var proc = new User32.MONITORENUMPROC(closure.Callback);
                        User32.EnumDisplayMonitors(IntPtr.Zero, null, proc, IntPtr.Zero);

                        if (closure.screens.Count > 0)
                        {
                            s_screens = closure.screens.ToArray();
                        }
                        else
                        {
                            s_screens = new Screen[] { new Screen((IntPtr)PRIMARY_MONITOR) };
                        }
                    }
                    else
                    {
                        s_screens = new Screen[] { PrimaryScreen! };
                    }

                    // Now that we have our screens, attach a display setting changed
                    // event so that we know when to invalidate them.
                    SystemEvents.DisplaySettingsChanging += new EventHandler(OnDisplaySettingsChanging);
                }

                return s_screens;
            }
        }

        /// <summary>
        ///  Gets Bits per Pixel value.
        /// </summary>
        public int BitsPerPixel
        {
            get
            {
                return _bitDepth;
            }
        }

        /// <summary>
        ///  Gets the bounds of the display.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return _bounds;
            }
        }

        /// <summary>
        ///  Gets the device name associated with a display.
        /// </summary>
        public string DeviceName
        {
            get
            {
                return _deviceName;
            }
        }

        /// <summary>
        ///  Gets a value indicating whether a particular display is
        ///  the primary device.
        /// </summary>
        public bool Primary
        {
            get
            {
                return _primary;
            }
        }

        /// <summary>
        ///  Gets the
        ///  primary display.
        /// </summary>
        public static Screen? PrimaryScreen
        {
            get
            {
                if (s_multiMonitorSupport)
                {
                    Screen[] screens = AllScreens;
                    for (int i = 0; i < screens.Length; i++)
                    {
                        if (screens[i]._primary)
                        {
                            return screens[i];
                        }
                    }

                    return null;
                }
                else
                {
                    return new Screen((IntPtr)PRIMARY_MONITOR, default);
                }
            }
        }

        /// <summary>
        ///  Gets the working area of the screen.
        /// </summary>
        public unsafe Rectangle WorkingArea
        {
            get
            {
                //if the static Screen class has a different desktop change count
                //than this instance then update the count and recalculate our working area
                if (_currentDesktopChangedCount != Screen.DesktopChangedCount)
                {
                    Interlocked.Exchange(ref _currentDesktopChangedCount, Screen.DesktopChangedCount);

                    if (!s_multiMonitorSupport || _hmonitor == (IntPtr)PRIMARY_MONITOR)
                    {
                        // Single monitor system
                        _workingArea = SystemInformation.WorkingArea;
                    }
                    else
                    {
                        // Multiple monitor System
                        var info = new User32.MONITORINFOEXW
                        {
                            cbSize = (uint)sizeof(User32.MONITORINFOEXW)
                        };
                        User32.GetMonitorInfoW(_hmonitor, ref info);
                        _workingArea = info.rcWork;
                    }
                }

                return _workingArea;
            }
        }

        /// <summary>
        ///  Screen instances call this property to determine
        ///  if their WorkingArea cache needs to be invalidated.
        /// </summary>
        private static int DesktopChangedCount
        {
            get
            {
                if (s_desktopChangedCount == -1)
                {
                    lock (s_syncLock)
                    {
                        //now that we have a lock, verify (again) our changecount...
                        if (s_desktopChangedCount == -1)
                        {
                            //sync the UserPreference.Desktop change event.  We'll keep count
                            //of desktop changes so that the WorkingArea property on Screen
                            //instances know when to invalidate their cache.
                            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);

                            s_desktopChangedCount = 0;
                        }
                    }
                }

                return s_desktopChangedCount;
            }
        }

        /// <summary>
        ///  Specifies a value that indicates whether the specified object is equal to
        ///  this one.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is Screen comp)
            {
                if (_hmonitor == comp._hmonitor)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///  Retrieves a <see cref="Screen"/>
        ///  for the monitor that contains the specified point.
        /// </summary>
        public static Screen FromPoint(Point point)
        {
            if (s_multiMonitorSupport)
            {
                return new Screen(User32.MonitorFromPoint(point, User32.MONITOR.DEFAULTTONEAREST));
            }
            else
            {
                return new Screen((IntPtr)PRIMARY_MONITOR);
            }
        }

        /// <summary>
        ///  Retrieves a <see cref="Screen"/>
        ///  for the monitor that contains the
        ///  largest region of the rectangle.
        /// </summary>
        public static Screen FromRectangle(Rectangle rect)
        {
            if (s_multiMonitorSupport)
            {
                RECT rc = rect;
                return new Screen(User32.MonitorFromRect(ref rc, User32.MONITOR.DEFAULTTONEAREST));
            }
            else
            {
                return new Screen((IntPtr)PRIMARY_MONITOR, default);
            }
        }

        /// <summary>
        ///  Retrieves a <see cref="Screen"/> for the monitor that contains
        ///  the largest region of the window of the control.
        /// </summary>
        public static Screen FromControl(Control control)
        {
            ArgumentNullException.ThrowIfNull(control);

            return FromHandle(control.Handle);
        }

        /// <summary>
        ///  Retrieves a <see cref="Screen"/> for the monitor that contains
        ///  the largest region of the window.
        /// </summary>
        public static Screen FromHandle(IntPtr hwnd)
        {
            if (s_multiMonitorSupport)
            {
                return new Screen(User32.MonitorFromWindow(hwnd, User32.MONITOR.DEFAULTTONEAREST));
            }
            else
            {
                return new Screen((IntPtr)PRIMARY_MONITOR, default);
            }
        }

        /// <summary>
        ///  Retrieves the working area for the monitor that is closest to the
        ///  specified point.
        /// </summary>
        public static Rectangle GetWorkingArea(Point pt)
        {
            return FromPoint(pt).WorkingArea;
        }

        /// <summary>
        ///  Retrieves the working area for the monitor that contains the largest region
        ///  of the specified rectangle.
        /// </summary>
        public static Rectangle GetWorkingArea(Rectangle rect)
        {
            return FromRectangle(rect).WorkingArea;
        }

        /// <summary>
        ///  Retrieves the working area for the monitor that contains the largest
        ///  region of the specified control.
        /// </summary>
        public static Rectangle GetWorkingArea(Control ctl)
        {
            return FromControl(ctl).WorkingArea;
        }

        /// <summary>
        ///  Retrieves the bounds of the monitor that is closest to the specified
        ///  point.
        /// </summary>
        public static Rectangle GetBounds(Point pt)
        {
            return FromPoint(pt).Bounds;
        }

        /// <summary>
        ///  Retrieves the bounds of the monitor that contains the largest region of the
        ///  specified rectangle.
        /// </summary>
        public static Rectangle GetBounds(Rectangle rect)
        {
            return FromRectangle(rect).Bounds;
        }

        /// <summary>
        ///  Retrieves the bounds of the monitor
        ///  that contains the largest region of the specified control.
        /// </summary>
        public static Rectangle GetBounds(Control ctl)
        {
            return FromControl(ctl).Bounds;
        }

        /// <summary>
        ///  Computes and retrieves a hash code for an object.
        /// </summary>
        public override int GetHashCode() => PARAM.ToInt(_hmonitor);

        /// <summary>
        ///  Called by the SystemEvents class when our display settings are
        ///  changing.  We cache screen information and at this point we must
        ///  invalidate our cache.
        /// </summary>
        private static void OnDisplaySettingsChanging(object? sender, EventArgs e)
        {
            // Now that we've responded to this event, we don't need it again until
            // someone re-queries. We will re-add the event at that time.
            SystemEvents.DisplaySettingsChanging -= new EventHandler(OnDisplaySettingsChanging);

            // Display settings changed, so the set of screens we have is invalid.
            s_screens = null;
        }

        /// <summary>
        ///  Called by the SystemEvents class when our display settings have
        ///  changed.  Here, we increment a static counter that Screen instances
        ///  can check against to invalidate their cache.
        /// </summary>
        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.Desktop)
            {
                Interlocked.Increment(ref s_desktopChangedCount);
            }
        }

        /// <summary>
        ///  Retrieves a string representing this object.
        /// </summary>
        public override string ToString()
        {
            return GetType().Name + "[Bounds=" + _bounds.ToString() + " WorkingArea=" + WorkingArea.ToString() + " Primary=" + _primary.ToString() + " DeviceName=" + _deviceName;
        }
    }
}
