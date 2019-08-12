// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a display device or multiple display devices on a single system.
    /// </summary>
    public class Screen
    {
        readonly IntPtr hmonitor;
        /// <summary>
        ///  Bounds of the screen
        /// </summary>
        readonly Rectangle bounds;
        /// <summary>
        ///  Available working area on the screen. This excludes taskbars and other
        ///  docked windows.
        /// </summary>
        private Rectangle workingArea = Rectangle.Empty;
        /// <summary>
        ///  Set to true if this screen is the primary monitor
        /// </summary>
        readonly bool primary;
        /// <summary>
        ///  Device name associated with this monitor
        /// </summary>
        readonly string deviceName;

        readonly int bitDepth;

        private static readonly object syncLock = new object();//used to lock this class before sync'ing to SystemEvents

        private static int desktopChangedCount = -1;//static counter of desktop size changes

        private int currentDesktopChangedCount = -1;//instance-based counter used to invalidate WorkingArea

        // This identifier is just for us, so that we don't try to call the multimon
        // functions if we just need the primary monitor... this is safer for
        // non-multimon OSes.
        //
        private const int PRIMARY_MONITOR = unchecked((int)0xBAADF00D);

        private const int MONITOR_DEFAULTTONULL = 0x00000000;
        private const int MONITOR_DEFAULTTOPRIMARY = 0x00000001;
        private const int MONITOR_DEFAULTTONEAREST = 0x00000002;
        private const int MONITORINFOF_PRIMARY = 0x00000001;

        private static readonly bool multiMonitorSupport = (User32.GetSystemMetrics(User32.SystemMetric.SM_CMONITORS) != 0);
        private static Screen[] screens;

        internal Screen(IntPtr monitor) : this(monitor, IntPtr.Zero)
        {
        }

        internal Screen(IntPtr monitor, IntPtr hdc)
        {
            IntPtr screenDC = hdc;

            if (!multiMonitorSupport || monitor == (IntPtr)PRIMARY_MONITOR)
            {
                // Single monitor system
                //
                bounds = SystemInformation.VirtualScreen;
                primary = true;
                deviceName = "DISPLAY";
            }
            else
            {
                // Multiple monitor system
                var info = new NativeMethods.MONITORINFOEX();
                SafeNativeMethods.GetMonitorInfo(new HandleRef(null, monitor), info);
                bounds = Rectangle.FromLTRB(info.rcMonitor.left, info.rcMonitor.top, info.rcMonitor.right, info.rcMonitor.bottom);
                primary = ((info.dwFlags & MONITORINFOF_PRIMARY) != 0);

                deviceName = new string(info.szDevice);
                deviceName = deviceName.TrimEnd((char)0);

                if (hdc == IntPtr.Zero)
                {
                    screenDC = Gdi32.CreateDC(deviceName, null, null, IntPtr.Zero);
                }
            }
            hmonitor = monitor;

            bitDepth = Gdi32.GetDeviceCaps(screenDC, Gdi32.DeviceCapability.BITSPIXEL);
            bitDepth *= Gdi32.GetDeviceCaps(screenDC, Gdi32.DeviceCapability.PLANES);

            if (hdc != screenDC)
            {
                Gdi32.DeleteDC(screenDC);
            }
        }

        /// <summary>
        ///  Gets an array of all of the displays on the system.
        /// </summary>
        public static Screen[] AllScreens
        {
            get
            {
                if (screens == null)
                {
                    if (multiMonitorSupport)
                    {
                        MonitorEnumCallback closure = new MonitorEnumCallback();
                        NativeMethods.MonitorEnumProc proc = new NativeMethods.MonitorEnumProc(closure.Callback);
                        SafeNativeMethods.EnumDisplayMonitors(NativeMethods.NullHandleRef, null, proc, IntPtr.Zero);

                        if (closure.screens.Count > 0)
                        {
                            Screen[] temp = new Screen[closure.screens.Count];
                            closure.screens.CopyTo(temp, 0);
                            screens = temp;
                        }
                        else
                        {
                            screens = new Screen[] { new Screen((IntPtr)PRIMARY_MONITOR) };
                        }
                    }
                    else
                    {
                        screens = new Screen[] { PrimaryScreen };
                    }

                    // Now that we have our screens, attach a display setting changed
                    // event so that we know when to invalidate them.
                    //
                    SystemEvents.DisplaySettingsChanging += new EventHandler(OnDisplaySettingsChanging);
                }

                return screens;
            }
        }

        /// <summary>
        ///  Gets Bits per Pixel value.
        /// </summary>
        public int BitsPerPixel
        {
            get
            {
                return bitDepth;
            }
        }

        /// <summary>
        ///  Gets the bounds of the display.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
        }

        /// <summary>
        ///  Gets the device name associated with a display.
        /// </summary>
        public string DeviceName
        {
            get
            {
                return deviceName;
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
                return primary;
            }
        }

        /// <summary>
        ///  Gets the
        ///  primary display.
        /// </summary>
        public static Screen PrimaryScreen
        {
            get
            {
                if (multiMonitorSupport)
                {
                    Screen[] screens = AllScreens;
                    for (int i = 0; i < screens.Length; i++)
                    {
                        if (screens[i].primary)
                        {
                            return screens[i];
                        }
                    }
                    return null;
                }
                else
                {
                    return new Screen((IntPtr)PRIMARY_MONITOR, IntPtr.Zero);
                }
            }
        }

        /// <summary>
        ///  Gets the working area of the screen.
        /// </summary>
        public Rectangle WorkingArea
        {
            get
            {

                //if the static Screen class has a different desktop change count
                //than this instance then update the count and recalculate our working area
                if (currentDesktopChangedCount != Screen.DesktopChangedCount)
                {

                    Interlocked.Exchange(ref currentDesktopChangedCount, Screen.DesktopChangedCount);

                    if (!multiMonitorSupport || hmonitor == (IntPtr)PRIMARY_MONITOR)
                    {
                        // Single monitor system
                        //
                        workingArea = SystemInformation.WorkingArea;
                    }
                    else
                    {
                        // Multiple monitor System
                        var info = new NativeMethods.MONITORINFOEX();
                        SafeNativeMethods.GetMonitorInfo(new HandleRef(null, hmonitor), info);
                        workingArea = Rectangle.FromLTRB(info.rcWork.left, info.rcWork.top, info.rcWork.right, info.rcWork.bottom);
                    }
                }

                return workingArea;
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
                if (desktopChangedCount == -1)
                {

                    lock (syncLock)
                    {

                        //now that we have a lock, verify (again) our changecount...
                        if (desktopChangedCount == -1)
                        {
                            //sync the UserPreference.Desktop change event.  We'll keep count
                            //of desktop changes so that the WorkingArea property on Screen
                            //instances know when to invalidate their cache.
                            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);

                            desktopChangedCount = 0;
                        }
                    }
                }
                return desktopChangedCount;
            }
        }

        /// <summary>
        ///  Specifies a value that indicates whether the specified object is equal to
        ///  this one.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Screen comp)
            {
                if (hmonitor == comp.hmonitor)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///  Retrieves a <see cref='Screen'/>
        ///  for the monitor that contains the specified point.
        /// </summary>
        public static Screen FromPoint(Point point)
        {
            if (multiMonitorSupport)
            {
                return new Screen(SafeNativeMethods.MonitorFromPoint(point, MONITOR_DEFAULTTONEAREST));
            }
            else
            {
                return new Screen((IntPtr)PRIMARY_MONITOR);
            }
        }

        /// <summary>
        ///  Retrieves a <see cref='Screen'/>
        ///  for the monitor that contains the
        ///  largest region of the rectangle.
        /// </summary>
        public static Screen FromRectangle(Rectangle rect)
        {
            if (multiMonitorSupport)
            {
                RECT rc = rect;
                return new Screen(SafeNativeMethods.MonitorFromRect(ref rc, MONITOR_DEFAULTTONEAREST));
            }
            else
            {
                return new Screen((IntPtr)PRIMARY_MONITOR, IntPtr.Zero);
            }
        }

        /// <summary>
        ///  Retrieves a <see cref='Screen'/> for the monitor that contains
        ///  the largest region of the window of the control.
        /// </summary>
        public static Screen FromControl(Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            return FromHandle(control.Handle);
        }

        /// <summary>
        ///  Retrieves a <see cref='Screen'/> for the monitor that contains
        ///  the largest region of the window.
        /// </summary>
        public static Screen FromHandle(IntPtr hwnd)
        {
            if (multiMonitorSupport)
            {
                return new Screen(SafeNativeMethods.MonitorFromWindow(new HandleRef(null, hwnd), MONITOR_DEFAULTTONEAREST));
            }
            else
            {
                return new Screen((IntPtr)PRIMARY_MONITOR, IntPtr.Zero);
            }
        }

        /// <summary>
        ///  Retrieves the working area for the monitor that is closest to the
        ///  specified point.
        /// </summary>
        public static Rectangle GetWorkingArea(Point pt)
        {
            return Screen.FromPoint(pt).WorkingArea;
        }
        /// <summary>
        ///  Retrieves the working area for the monitor that contains the largest region
        ///  of the specified rectangle.
        /// </summary>
        public static Rectangle GetWorkingArea(Rectangle rect)
        {
            return Screen.FromRectangle(rect).WorkingArea;
        }
        /// <summary>
        ///  Retrieves the working area for the monitor that contains the largest
        ///  region of the specified control.
        /// </summary>
        public static Rectangle GetWorkingArea(Control ctl)
        {
            return Screen.FromControl(ctl).WorkingArea;
        }

        /// <summary>
        ///  Retrieves the bounds of the monitor that is closest to the specified
        ///  point.
        /// </summary>
        public static Rectangle GetBounds(Point pt)
        {
            return Screen.FromPoint(pt).Bounds;
        }
        /// <summary>
        ///  Retrieves the bounds of the monitor that contains the largest region of the
        ///  specified rectangle.
        /// </summary>
        public static Rectangle GetBounds(Rectangle rect)
        {
            return Screen.FromRectangle(rect).Bounds;
        }
        /// <summary>
        ///  Retrieves the bounds of the monitor
        ///  that contains the largest region of the specified control.
        /// </summary>
        public static Rectangle GetBounds(Control ctl)
        {
            return Screen.FromControl(ctl).Bounds;
        }

        /// <summary>
        ///  Computes and retrieves a hash code for an object.
        /// </summary>
        public override int GetHashCode() => (int)hmonitor;

        /// <summary>
        ///  Called by the SystemEvents class when our display settings are
        ///  changing.  We cache screen information and at this point we must
        ///  invalidate our cache.
        /// </summary>
        private static void OnDisplaySettingsChanging(object sender, EventArgs e)
        {
            // Now that we've responded to this event, we don't need it again until
            // someone re-queries. We will re-add the event at that time.
            //
            SystemEvents.DisplaySettingsChanging -= new EventHandler(OnDisplaySettingsChanging);

            // Display settings changed, so the set of screens we have is invalid.
            //
            screens = null;
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
                Interlocked.Increment(ref desktopChangedCount);
            }
        }

        /// <summary>
        ///  Retrieves a string representing this object.
        /// </summary>
        public override string ToString()
        {
            return GetType().Name + "[Bounds=" + bounds.ToString() + " WorkingArea=" + WorkingArea.ToString() + " Primary=" + primary.ToString() + " DeviceName=" + deviceName;
        }

        private class MonitorEnumCallback
        {
            public ArrayList screens = new ArrayList();

            public virtual bool Callback(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lparam)
            {
                screens.Add(new Screen(monitor, hdc));
                return true;
            }
        }
    }
}
