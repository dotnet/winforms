// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Microsoft.Win32;

namespace System.Windows.Forms;

/// <summary>
///  Represents a display device or multiple display devices on a single system.
/// </summary>
public partial class Screen
{
    private readonly HMONITOR _hmonitor;

    /// <summary>
    ///  Bounds of the screen
    /// </summary>
    private readonly Rectangle _bounds;

    /// <summary>
    ///  Available working area on the screen. This excludes taskbars and other docked windows.
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

    private static readonly Lock s_syncLock = new(); // used to lock this class before syncing to SystemEvents

    private static int s_desktopChangedCount = -1; // static counter of desktop size changes

    private int _currentDesktopChangedCount = -1; // instance-based counter used to invalidate WorkingArea

    // This identifier is just for us, so that we don't try to call the multimon functions if we just need the
    // primary monitor. This is safer for non-multimon OSes.
    private static readonly HMONITOR s_primaryMonitor = (HMONITOR)unchecked((nint)0xBAADF00D);

    private static Screen[]? s_screens;

    internal Screen(HMONITOR monitor) : this(monitor, default)
    {
    }

    internal unsafe Screen(HMONITOR monitor, HDC hdc)
    {
        HDC screenDC = hdc;

        if (!SystemInformation.MultiMonitorSupport || monitor == s_primaryMonitor)
        {
            // Single monitor system
            _bounds = SystemInformation.VirtualScreen;
            _primary = true;
            _deviceName = "DISPLAY";
        }
        else
        {
            // Multiple monitor system
            MONITORINFOEXW info = new()
            {
                monitorInfo = new() { cbSize = (uint)sizeof(MONITORINFOEXW) }
            };

            // API takes EX, determines which one you pass by size.
            PInvokeCore.GetMonitorInfo(monitor, (MONITORINFO*)&info);
            _bounds = info.monitorInfo.rcMonitor;
            _primary = ((info.monitorInfo.dwFlags & PInvokeCore.MONITORINFOF_PRIMARY) != 0);

            _deviceName = new string(info.szDevice.ToString());

            if (hdc.IsNull)
            {
                screenDC = PInvokeCore.CreateDCW(_deviceName, pwszDevice: null, pszPort: null, pdm: null);
            }
        }

        _hmonitor = monitor;

        _bitDepth = PInvokeCore.GetDeviceCaps(screenDC, GET_DEVICE_CAPS_INDEX.BITSPIXEL);
        _bitDepth *= PInvokeCore.GetDeviceCaps(screenDC, GET_DEVICE_CAPS_INDEX.PLANES);

        if (hdc != screenDC)
        {
            PInvokeCore.DeleteDC(screenDC);
        }
    }

    /// <summary>
    ///  Gets an array of all of the displays on the system.
    /// </summary>
    public static unsafe Screen[] AllScreens
    {
        get
        {
            if (s_screens is null)
            {
                if (SystemInformation.MultiMonitorSupport)
                {
                    List<Screen> screens = [];
                    PInvokeCore.EnumDisplayMonitors((HMONITOR hmonitor, HDC hdc) =>
                    {
                        screens.Add(new(hmonitor, hdc));
                        return true;
                    });

                    s_screens = screens.Count > 0 ? [.. screens] : [new(s_primaryMonitor)];
                }
                else
                {
                    s_screens = [PrimaryScreen!];
                }

                // Now that we have our screens, attach a display setting changed
                // event so that we know when to invalidate them.
                SystemEvents.DisplaySettingsChanging += OnDisplaySettingsChanging;
            }

            return s_screens;
        }
    }

    /// <summary>
    ///  Gets Bits per Pixel value.
    /// </summary>
    public int BitsPerPixel => _bitDepth;

    /// <summary>
    ///  Gets the bounds of the display.
    /// </summary>
    public Rectangle Bounds => _bounds;

    /// <summary>
    ///  Gets the device name associated with a display.
    /// </summary>
    public string DeviceName => _deviceName;

    /// <summary>
    ///  Gets a value indicating whether a particular display is the primary device.
    /// </summary>
    public bool Primary => _primary;

    /// <summary>
    ///  Gets the primary display.
    /// </summary>
    public static Screen? PrimaryScreen
    {
        get
        {
            if (SystemInformation.MultiMonitorSupport)
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
                return new Screen(s_primaryMonitor, default);
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
            // if the static Screen class has a different desktop change count
            // than this instance then update the count and recalculate our working area
            if (_currentDesktopChangedCount != DesktopChangedCount)
            {
                Interlocked.Exchange(ref _currentDesktopChangedCount, DesktopChangedCount);

                if (!SystemInformation.MultiMonitorSupport || _hmonitor == s_primaryMonitor)
                {
                    // Single monitor system
                    _workingArea = SystemInformation.WorkingArea;
                }
                else
                {
                    // Multiple monitor System
                    MONITORINFOEXW info = new()
                    {
                        monitorInfo = new() { cbSize = (uint)sizeof(MONITORINFOEXW) }
                    };

                    // API takes EX, determines which one you pass by size.
                    PInvokeCore.GetMonitorInfo(_hmonitor, (MONITORINFO*)&info);
                    _workingArea = info.monitorInfo.rcWork;
                }
            }

            return _workingArea;
        }
    }

    /// <summary>
    ///  Screen instances call this property to determine if their WorkingArea cache needs to be invalidated.
    /// </summary>
    private static int DesktopChangedCount
    {
        get
        {
            if (s_desktopChangedCount == -1)
            {
                lock (s_syncLock)
                {
                    // Now that we have a lock, verify (again) our changecount.
                    if (s_desktopChangedCount == -1)
                    {
                        // Sync the UserPreference.Desktop change event. We'll keep count of desktop changes so that
                        // the WorkingArea property on Screen instances know when to invalidate their cache.
                        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;

                        s_desktopChangedCount = 0;
                    }
                }
            }

            return s_desktopChangedCount;
        }
    }

    /// <summary>
    ///  Specifies a value that indicates whether the specified object is equal to this one.
    /// </summary>
    public override bool Equals(object? obj) => obj is Screen comp && _hmonitor == comp._hmonitor;

    /// <summary>
    ///  Retrieves a <see cref="Screen"/> for the monitor that contains the specified point.
    /// </summary>
    public static Screen FromPoint(Point point)
        => SystemInformation.MultiMonitorSupport
        ? new Screen(PInvokeCore.MonitorFromPoint(point, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST))
        : new Screen(s_primaryMonitor);

    /// <summary>
    ///  Retrieves a <see cref="Screen"/> for the monitor that contains the largest region of the rectangle.
    /// </summary>
    public static Screen FromRectangle(Rectangle rect)
        => SystemInformation.MultiMonitorSupport
        ? new Screen(PInvokeCore.MonitorFromRect(rect, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST))
        : new Screen(s_primaryMonitor, default);

    /// <summary>
    ///  Retrieves a <see cref="Screen"/> for the monitor that contains the largest region of the window of the control.
    /// </summary>
    public static Screen FromControl(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);

        return FromHandle(control.Handle);
    }

    /// <summary>
    ///  Retrieves a <see cref="Screen"/> for the monitor that contains the largest region of the window.
    /// </summary>
    public static Screen FromHandle(IntPtr hwnd)
        => SystemInformation.MultiMonitorSupport
        ? new Screen(PInvokeCore.MonitorFromWindow((HWND)hwnd, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST))
        : new Screen(s_primaryMonitor, default);

    /// <summary>
    ///  Retrieves the working area for the monitor that is closest to the specified point.
    /// </summary>
    public static Rectangle GetWorkingArea(Point pt) => FromPoint(pt).WorkingArea;

    /// <summary>
    ///  Retrieves the working area for the monitor that contains the largest region of the specified rectangle.
    /// </summary>
    public static Rectangle GetWorkingArea(Rectangle rect) => FromRectangle(rect).WorkingArea;

    /// <summary>
    ///  Retrieves the working area for the monitor that contains the largest region of the specified control.
    /// </summary>
    public static Rectangle GetWorkingArea(Control ctl) => FromControl(ctl).WorkingArea;

    /// <summary>
    ///  Retrieves the bounds of the monitor that is closest to the specified point.
    /// </summary>
    public static Rectangle GetBounds(Point pt) => FromPoint(pt).Bounds;

    /// <summary>
    ///  Retrieves the bounds of the monitor that contains the largest region of the specified rectangle.
    /// </summary>
    public static Rectangle GetBounds(Rectangle rect) => FromRectangle(rect).Bounds;

    /// <summary>
    ///  Retrieves the bounds of the monitor that contains the largest region of the specified control.
    /// </summary>
    public static Rectangle GetBounds(Control ctl) => FromControl(ctl).Bounds;

    /// <summary>
    ///  Computes and retrieves a hash code for an object.
    /// </summary>
    public override int GetHashCode() => PARAM.ToInt(_hmonitor);

    /// <summary>
    ///  Called by the SystemEvents class when our display settings are changing. We cache screen information and
    ///  at this point we must invalidate our cache.
    /// </summary>
    private static void OnDisplaySettingsChanging(object? sender, EventArgs e)
    {
        // Now that we've responded to this event, we don't need it again until
        // someone re-queries. We will re-add the event at that time.
        SystemEvents.DisplaySettingsChanging -= OnDisplaySettingsChanging;

        // Display settings changed, so the set of screens we have is invalid.
        s_screens = null;
    }

    /// <summary>
    ///  Called by the SystemEvents class when our display settings have changed. Here, we increment a static counter
    ///  that Screen instances can check against to invalidate their cache.
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
        => $"{GetType().Name}[Bounds={_bounds} WorkingArea={WorkingArea} Primary={_primary} DeviceName={_deviceName}";
}
