// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides information about the operating system.
    /// </summary>
    public static class SystemInformation
    {
        private static bool s_checkMultiMonitorSupport;
        private static bool s_multiMonitorSupport;
        private static bool s_highContrast;
        private static bool s_systemEventsAttached;
        private static bool s_systemEventsDirty = true;

        private static IntPtr s_processWinStation = IntPtr.Zero;
        private static bool s_isUserInteractive;

        private static PowerStatus s_powerStatus;

        private const int DefaultMouseWheelScrollLines = 3;

        /// <summary>
        ///  Gets a value indicating whether the user has enabled full window drag.
        /// </summary>
        public static bool DragFullWindows
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETDRAGFULLWINDOWS);

        /// <summary>
        ///  Gets a value indicating whether the user has selected to run in high contrast.
        /// </summary>
        public static bool HighContrast
        {
            get
            {
                EnsureSystemEvents();
                if (s_systemEventsDirty)
                {
                    NativeMethods.HIGHCONTRASTW data = default;

                    s_highContrast = UnsafeNativeMethods.SystemParametersInfoW(ref data)
                        && (data.dwFlags & NativeMethods.HCF_HIGHCONTRASTON) != 0;

                    s_systemEventsDirty = false;
                }

                return s_highContrast;
            }
        }

        /// <summary>
        ///  Gets the number of lines to scroll when the mouse wheel is rotated.
        /// </summary>
        public static int MouseWheelScrollLines
            => UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_GETWHEELSCROLLLINES);

        /// <summary>
        ///  Gets the dimensions of the primary display monitor in pixels.
        /// </summary>
        public static Size PrimaryMonitorSize
            => new Size(GetSystemMetrics(SystemMetric.SM_CXSCREEN),
                        GetSystemMetrics(SystemMetric.SM_CYSCREEN));

        /// <summary>
        ///  Gets the width of the vertical scroll bar in pixels.
        /// </summary>
        public static int VerticalScrollBarWidth
        {
            get => GetSystemMetrics(SystemMetric.SM_CXVSCROLL);
        }

        /// <summary>
        ///  Gets the width of the vertical scroll bar in pixels.
        /// </summary>
        public static int GetVerticalScrollBarWidthForDpi(int dpi)
        {
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                return GetCurrentSystemMetrics(SystemMetric.SM_CXVSCROLL, (uint)dpi);
            }
            
            return GetSystemMetrics(SystemMetric.SM_CXVSCROLL);
        }

        /// <summary>
        ///  Gets the height of the horizontal scroll bar in pixels.
        /// </summary>
        public static int HorizontalScrollBarHeight => GetSystemMetrics(SystemMetric.SM_CYHSCROLL);

        /// <summary>
        ///  Gets the height of the horizontal scroll bar in pixels.
        /// </summary>
        public static int GetHorizontalScrollBarHeightForDpi(int dpi)
        {
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                return GetCurrentSystemMetrics(SystemMetric.SM_CYHSCROLL, (uint)dpi);
            }
            
            return GetSystemMetrics(SystemMetric.SM_CYHSCROLL);
        }

        /// <summary>
        ///  Gets the height of the normal caption area of a window in pixels.
        /// </summary>
        public static int CaptionHeight
        {
            get => GetSystemMetrics(SystemMetric.SM_CYCAPTION);
        }

        /// <summary>
        ///  Gets the width and height of a window border in pixels.
        /// </summary>
        public static Size BorderSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXBORDER),
                            GetSystemMetrics(SystemMetric.SM_CYBORDER));
        }

        /// <summary>
        ///  Gets the width andheight of a window border in pixels.
        /// </summary>
        public static Size GetBorderSizeForDpi(int dpi)
        {
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                return new Size(GetCurrentSystemMetrics(SystemMetric.SM_CXBORDER, (uint)dpi),
                                GetCurrentSystemMetrics(SystemMetric.SM_CYBORDER, (uint)dpi));
            }
            
            return BorderSize;
        }

        /// <summary>
        ///  Gets the thickness in pixels, of the border for a window that has a caption and is not resizable.
        /// </summary>
        public static Size FixedFrameBorderSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXFIXEDFRAME),
                            GetSystemMetrics(SystemMetric.SM_CYFIXEDFRAME));
        }

        /// <summary>
        ///  Gets the height of the scroll box in a vertical scroll bar in pixels.
        /// </summary>
        public static int VerticalScrollBarThumbHeight => GetSystemMetrics(SystemMetric.SM_CYVTHUMB);

        /// <summary>
        ///  Gets the width of the scroll box in a horizontal scroll bar in pixels.
        /// </summary>
        public static int HorizontalScrollBarThumbWidth => GetSystemMetrics(SystemMetric.SM_CXHTHUMB);

        /// <summary>
        ///  Gets the default dimensions of an icon in pixels.
        /// </summary>
        public static Size IconSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXICON),
                            GetSystemMetrics(SystemMetric.SM_CYICON));
        }

        /// <summary>
        ///  Gets the dimensions of a cursor in pixels.
        /// </summary>
        public static Size CursorSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXCURSOR),
                            GetSystemMetrics(SystemMetric.SM_CYCURSOR));
        }

        /// <summary>
        ///  Gets the system's font for menus.
        /// </summary>
        public static Font MenuFont => GetMenuFontHelper(0, useDpi: false);

        /// <summary>
        ///  Gets the system's font for menus, scaled accordingly to an arbitrary DPI you provide.
        /// </summary>
        public static Font GetMenuFontForDpi(int dpi)
        {
            return GetMenuFontHelper((uint)dpi, DpiHelper.IsPerMonitorV2Awareness);
        }

        private unsafe static Font GetMenuFontHelper(uint dpi, bool useDpi)
        {
            // We can get the system's menu font through the NONCLIENTMETRICS structure
            // via SystemParametersInfo
            NativeMethods.NONCLIENTMETRICSW data = default;

            bool result = useDpi
                ? UnsafeNativeMethods.TrySystemParametersInfoForDpi(ref data, dpi)
                : UnsafeNativeMethods.SystemParametersInfoW(ref data);

            if (result)
            {
                try
                {
                    return Font.FromLogFont(data.lfMenuFont);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (ArgumentException)
                {
                    // Font.FromLogFont throws ArgumentException when it finds
                    // a font that is not TrueType. Default to standard control font.
                }
#pragma warning restore CA1031
            }

            return Control.DefaultFont;
        }

        /// <summary>
        ///  Gets the height of a one line of a menu in pixels.
        /// </summary>
        public static int MenuHeight => GetSystemMetrics(SystemMetric.SM_CYMENU);

        /// <summary>
        ///  Returns the current system power status.
        /// </summary>
        public static PowerStatus PowerStatus => s_powerStatus ??= new PowerStatus();

        /// <summary>
        ///  Gets the size of the working area in pixels.
        /// </summary>
        public static Rectangle WorkingArea
        {
            get
            {
                var rect = new RECT();
                UnsafeNativeMethods.SystemParametersInfoW(NativeMethods.SPI_GETWORKAREA, ref rect);
                return rect;
            }
        }

        /// <summary>
        ///  Gets the height, in pixels, of the Kanji window at the bottom of the screen
        ///  for double-byte (DBCS) character set versions of Windows.
        /// </summary>
        public static int KanjiWindowHeight => GetSystemMetrics(SystemMetric.SM_CYKANJIWINDOW);

        /// <summary>
        ///  Gets a value indicating whether the system has a mouse installed.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool MousePresent => GetSystemMetrics(SystemMetric.SM_MOUSEPRESENT) != 0;

        /// <summary>
        ///  Gets the height in pixels, of the arrow bitmap on the vertical scroll bar.
        /// </summary>
        public static int VerticalScrollBarArrowHeight => GetSystemMetrics(SystemMetric.SM_CYVSCROLL);

        /// <summary>
        ///  Gets the height of the vertical scroll bar arrow bitmap in pixels.
        /// </summary>
        public static int VerticalScrollBarArrowHeightForDpi(int dpi)
        {
            return GetCurrentSystemMetrics(SystemMetric.SM_CYVSCROLL, (uint)dpi);
        }

        /// <summary>
        ///  Gets the width, in pixels, of the arrow bitmap on the horizontal scrollbar.
        /// </summary>
        public static int HorizontalScrollBarArrowWidth =>GetSystemMetrics(SystemMetric.SM_CXHSCROLL);

        /// <summary>
        ///  Gets the width of the horizontal scroll bar arrow bitmap in pixels.
        /// </summary>
        public static int GetHorizontalScrollBarArrowWidthForDpi(int dpi)
        {
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                return GetCurrentSystemMetrics(SystemMetric.SM_CXHSCROLL, (uint)dpi);
            }
            
            return GetSystemMetrics(SystemMetric.SM_CXHSCROLL);
        }

        /// <summary>
        ///  Gets a value indicating whether this is a debug version of the operating system.
        /// </summary>
        public static bool DebugOS => GetSystemMetrics(SystemMetric.SM_DEBUG) != 0;

        /// <summary>
        ///  Gets a value indicating whether the functions of the left and right mouse
        ///  buttons have been swapped.
        /// </summary>
        public static bool MouseButtonsSwapped => (GetSystemMetrics(SystemMetric.SM_SWAPBUTTON) != 0);

        /// <summary>
        ///  Gets the minimum allowable dimensions of a window in pixels.
        /// </summary>
        public static Size MinimumWindowSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXMIN),
                            GetSystemMetrics(SystemMetric.SM_CYMIN));
        }

        /// <summary>
        ///  Gets the dimensions in pixels, of a caption bar or title bar button.
        /// </summary>
        public static Size CaptionButtonSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXSIZE),
                            GetSystemMetrics(SystemMetric.SM_CYSIZE));
        }

        /// <summary>
        ///  Gets the thickness in pixels, of the border for a window that can be resized.
        /// </summary>
        public static Size FrameBorderSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXFRAME),
                            GetSystemMetrics(SystemMetric.SM_CYFRAME));
        }

        /// <summary>
        ///  Gets the system's default minimum tracking dimensions of a window in pixels.
        /// </summary>
        public static Size MinWindowTrackSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXMINTRACK),
                            GetSystemMetrics(SystemMetric.SM_CYMINTRACK));
        }

        /// <summary>
        ///  Gets the dimensions in pixels, of the area that the user must click within
        ///  for the system to consider the two clicks a double-click. The rectangle is
        ///  centered around the first click.
        /// </summary>
        public static Size DoubleClickSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXDOUBLECLK),
                            GetSystemMetrics(SystemMetric.SM_CYDOUBLECLK));
        }

        /// <summary>
        ///  Gets the maximum number of milliseconds allowed between mouse clicks for a
        ///  double-click.
        /// </summary>
        public static int DoubleClickTime => SafeNativeMethods.GetDoubleClickTime();

        /// <summary>
        ///  Gets the dimensions in pixels, of the grid used to arrange icons in a large
        ///  icon view.
        /// </summary>
        public static Size IconSpacingSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXICONSPACING),
                            GetSystemMetrics(SystemMetric.SM_CYICONSPACING));
        }

        /// <summary>
        ///  Gets a value indicating whether drop down menus should be right-aligned with the corresponding menu
        ///  bar item.
        /// </summary>
        public static bool RightAlignedMenus => GetSystemMetrics(SystemMetric.SM_MENUDROPALIGNMENT) != 0;

        /// <summary>
        ///  Gets a value indicating whether the Microsoft Windows for Pen computing extensions are installed.
        /// </summary>
        public static bool PenWindows => GetSystemMetrics(SystemMetric.SM_PENWINDOWS) != 0;

        /// <summary>
        ///  Gets a value indicating whether the operating system is capable of handling
        ///  double-byte (DBCS) characters.
        /// </summary>
        public static bool DbcsEnabled => GetSystemMetrics(SystemMetric.SM_DBCSENABLED) != 0;

        /// <summary>
        ///  Gets the number of buttons on mouse.
        /// </summary>
        public static int MouseButtons => GetSystemMetrics(SystemMetric.SM_CMOUSEBUTTONS);

        /// <summary>
        ///  Gets a value indicating whether security is present on this operating system.
        /// </summary>
        public static bool Secure => GetSystemMetrics(SystemMetric.SM_SECURE) != 0;

        /// <summary>
        ///  Gets the dimensions in pixels, of a 3-D border.
        /// </summary>
        public static Size Border3DSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXEDGE),
                            GetSystemMetrics(SystemMetric.SM_CYEDGE));
        }

        /// <summary>
        ///  Gets the dimensions in pixels, of the grid into which minimized windows will
        ///  be placed.
        /// </summary>
        public static Size MinimizedWindowSpacingSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXMINSPACING),
                            GetSystemMetrics(SystemMetric.SM_CYMINSPACING));
        }

        /// <summary>
        ///  Gets the recommended dimensions of a small icon in pixels.
        /// </summary>
        public static Size SmallIconSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXSMICON),
                            GetSystemMetrics(SystemMetric.SM_CYSMICON));
        }

        /// <summary>
        ///  Gets the height of a small caption in pixels.
        /// </summary>
        public static int ToolWindowCaptionHeight => GetSystemMetrics(SystemMetric.SM_CYSMCAPTION);

        /// <summary>
        ///  Gets the dimensions of small caption buttons in pixels.
        /// </summary>
        public static Size ToolWindowCaptionButtonSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXSMSIZE),
                            GetSystemMetrics(SystemMetric.SM_CYSMSIZE));
        }

        /// <summary>
        ///  Gets the dimensions in pixels, of menu bar buttons.
        /// </summary>
        public static Size MenuButtonSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXMENUSIZE),
                            GetSystemMetrics(SystemMetric.SM_CYMENUSIZE));
        }

        /// <summary>
        ///  Gets flags specifying how the system arranges minimized windows.
        /// </summary>
        public static ArrangeStartingPosition ArrangeStartingPosition
        {
            get
            {
                ArrangeStartingPosition mask = ArrangeStartingPosition.BottomLeft | ArrangeStartingPosition.BottomRight | ArrangeStartingPosition.Hide | ArrangeStartingPosition.TopLeft | ArrangeStartingPosition.TopRight;
                int compoundValue = GetSystemMetrics(SystemMetric.SM_ARRANGE);
                return mask & (ArrangeStartingPosition)compoundValue;
            }
        }

        /// <summary>
        ///  Gets flags specifying how the system arranges minimized windows.
        /// </summary>
        public static ArrangeDirection ArrangeDirection
        {
            get
            {
                ArrangeDirection mask = ArrangeDirection.Down | ArrangeDirection.Left | ArrangeDirection.Right | ArrangeDirection.Up;
                int compoundValue = GetSystemMetrics(SystemMetric.SM_ARRANGE);
                return mask & (ArrangeDirection)compoundValue;
            }
        }

        /// <summary>
        ///  Gets the dimensions in pixels, of a normal minimized window.
        /// </summary>
        public static Size MinimizedWindowSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXMINIMIZED),
                            GetSystemMetrics(SystemMetric.SM_CYMINIMIZED));
        }

        /// <summary>
        ///  Gets the default maximum dimensions in pixels, of a window that has a
        ///  caption and sizing borders.
        /// </summary>
        public static Size MaxWindowTrackSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXMAXTRACK),
                            GetSystemMetrics(SystemMetric.SM_CYMAXTRACK));
        }

        /// <summary>
        ///  Gets the default dimensions, in pixels, of a maximized top-left window on the
        ///  primary monitor.
        /// </summary>
        public static Size PrimaryMonitorMaximizedWindowSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXMAXIMIZED),
                            GetSystemMetrics(SystemMetric.SM_CYMAXIMIZED));
        }

        /// <summary>
        ///  Gets a value indicating whether this computer is connected to a network.
        /// </summary>
        public static bool Network => (GetSystemMetrics(SystemMetric.SM_NETWORK) & 0x00000001) != 0;

        public static bool TerminalServerSession => (GetSystemMetrics(SystemMetric.SM_REMOTESESSION) & 0x00000001) != 0;

        /// <summary>
        ///  Gets a value that specifies how the system was started.
        /// </summary>
        public static BootMode BootMode => (BootMode)GetSystemMetrics(SystemMetric.SM_CLEANBOOT);

        /// <summary>
        ///  Gets the dimensions in pixels, of the rectangle that a drag operation must
        ///  extend to be considered a drag. The rectangle is centered on a drag point.
        /// </summary>
        public static Size DragSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXDRAG),
                            GetSystemMetrics(SystemMetric.SM_CYDRAG));
        }

        /// <summary>
        ///  Gets a value indicating whether the user requires an application to present
        ///  information visually in situations where it would otherwise present the
        ///  information in audible form.
        /// </summary>
        public static bool ShowSounds => GetSystemMetrics(SystemMetric.SM_SHOWSOUNDS) != 0;

        /// <summary>
        ///  Gets the dimensions of the default size of a menu checkmark in pixels.
        /// </summary>
        public static Size MenuCheckSize
        {
            get => new Size(GetSystemMetrics(SystemMetric.SM_CXMENUCHECK),
                            GetSystemMetrics(SystemMetric.SM_CYMENUCHECK));
        }

        /// <summary>
        ///  Gets a value indicating whether the system is enabled for Hebrew and Arabic languages.
        /// </summary>
        public static bool MidEastEnabled => GetSystemMetrics(SystemMetric.SM_MIDEASTENABLED) != 0;

        private static bool MultiMonitorSupport
        {
            get
            {
                if (!s_checkMultiMonitorSupport)
                {
                    s_multiMonitorSupport = (GetSystemMetrics(SystemMetric.SM_CMONITORS) != 0);
                    s_checkMultiMonitorSupport = true;
                }

                return s_multiMonitorSupport;
            }
        }

        /// <summary>
        ///  Gets a value indicating whether a mouse with a mouse wheel is installed.
        /// </summary>
        /// <remarks>
        ///  This was never really correct. All versions of Windows NT from 4.0 onward supported the mouse wheel
        ///  directly. This should have been a version check. Rather than change it and risk breaking apps we'll
        ///  keep it equivalent to <see cref="MouseWheelPresent" />
        /// </remarks>
        public static bool NativeMouseWheelSupport => GetSystemMetrics(SystemMetric.SM_MOUSEWHEELPRESENT) != 0;

        /// <summary>
        ///  Gets a value indicating whether a mouse with a mouse wheel is installed.
        /// </summary>
        public static bool MouseWheelPresent => NativeMouseWheelSupport;

        /// <summary>
        ///  Gets the bounds of the virtual screen.
        /// </summary>
        public static Rectangle VirtualScreen
        {
            get
            {
                if (MultiMonitorSupport)
                {
                    return new Rectangle(GetSystemMetrics(SystemMetric.SM_XVIRTUALSCREEN),
                                         GetSystemMetrics(SystemMetric.SM_YVIRTUALSCREEN),
                                         GetSystemMetrics(SystemMetric.SM_CXVIRTUALSCREEN),
                                         GetSystemMetrics(SystemMetric.SM_CYVIRTUALSCREEN));
                }

                Size size = PrimaryMonitorSize;
                return new Rectangle(0, 0, size.Width, size.Height);
            }
        }

        /// <summary>
        ///  Gets the number of display monitors on the desktop.
        /// </summary>
        public static int MonitorCount
        {
            get
            {
                if (MultiMonitorSupport)
                {
                    return GetSystemMetrics(SystemMetric.SM_CMONITORS);
                }

                return 1;
            }
        }

        /// <summary>
        ///  Gets a value indicating whether all the display monitors have the same color format.
        /// </summary>
        public static bool MonitorsSameDisplayFormat
        {
            get
            {
                if (MultiMonitorSupport)
                {
                    return GetSystemMetrics(SystemMetric.SM_SAMEDISPLAYFORMAT) != 0;
                }

                return true;
            }
        }

        /// <summary>
        ///  Gets the computer name of the current system.
        /// </summary>
        public static string ComputerName
        {
            get
            {
                var sb = new StringBuilder(256);
                UnsafeNativeMethods.GetComputerName(sb, new int[] { sb.Capacity });
                return sb.ToString();
            }
        }

        /// <summary>
        ///  Gets the user's domain name.
        /// </summary>
        public static string UserDomainName => Environment.UserDomainName;

        /// <summary>
        ///  Gets a value indicating whether the current process is running in user
        ///  interactive mode.
        /// </summary>
        public unsafe static bool UserInteractive
        {
            get
            {
                IntPtr hwinsta = UnsafeNativeMethods.GetProcessWindowStation();
                if (hwinsta != IntPtr.Zero && s_processWinStation != hwinsta)
                {
                    s_isUserInteractive = true;

                    int lengthNeeded = 0;

                    NativeMethods.USEROBJECTFLAGS flags = new NativeMethods.USEROBJECTFLAGS();
                    if (UnsafeNativeMethods.GetUserObjectInformation(new HandleRef(null, hwinsta), NativeMethods.UOI_FLAGS, ref flags, sizeof(NativeMethods.USEROBJECTFLAGS), ref lengthNeeded))
                    {
                        if ((flags.dwFlags & NativeMethods.WSF_VISIBLE) == 0)
                        {
                            s_isUserInteractive = false;
                        }
                    }

                    s_processWinStation = hwinsta;
                }

                return s_isUserInteractive;
            }
        }

        /// <summary>
        ///  Gets the user name for the current thread, that is, the name of the user currently logged onto
        ///  the system.
        /// </summary>
        public static string UserName
        {
            get
            {
                var sb = new StringBuilder(256);
                UnsafeNativeMethods.GetUserName(sb, new int[] { sb.Capacity });
                return sb.ToString();
            }
        }

        private static void EnsureSystemEvents()
        {
            if (!s_systemEventsAttached)
            {
                SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
                s_systemEventsAttached = true;
            }
        }

        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref)
        {
            s_systemEventsDirty = true;
        }

        /// <summary>
        ///  Gets whether the drop shadow effect in enabled.
        /// </summary>
        public static bool IsDropShadowEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETDROPSHADOW);

        /// <summary>
        ///  Gets whether the native user menus have a flat menu appearance.
        /// </summary>
        public static bool IsFlatMenuEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETFLATMENU);

        /// <summary>
        ///  Gets whether font smoothing is enabled.
        /// </summary>
        public static bool IsFontSmoothingEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETFONTSMOOTHING);

        /// <summary>
        ///  Returns the ClearType smoothing contrast value.
        /// </summary>
        public static int FontSmoothingContrast
            => UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_GETFONTSMOOTHINGCONTRAST);

        /// <summary>
        ///  Returns a type of Font smoothing.
        /// </summary>
        public static int FontSmoothingType
            => UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_GETFONTSMOOTHINGTYPE);

        /// <summary>
        ///  Retrieves the width in pixels of an icon cell.
        /// </summary>
        public static int IconHorizontalSpacing
            => UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_ICONHORIZONTALSPACING);

        /// <summary>
        ///  Retrieves the height in pixels of an icon cell.
        /// </summary>
        public static int IconVerticalSpacing
            => UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_ICONVERTICALSPACING);

        /// <summary>
        ///  Gets whether icon title wrapping is enabled.
        /// </summary>
        public static bool IsIconTitleWrappingEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETICONTITLEWRAP);

        /// <summary>
        ///  Gets whether menu access keys are underlined.
        /// </summary>
        public static bool MenuAccessKeysUnderlined
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETKEYBOARDCUES);

        /// <summary>
        ///  Retrieves the Keyboard repeat delay setting, which is a value in the range
        ///  from 0 through 3. The actual delay associated with each value may vary
        ///  depending on the hardware.
        /// </summary>
        public static int KeyboardDelay
            => UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_GETKEYBOARDDELAY);

        /// <summary>
        ///  Gets whether the user relies on keyboard instead of mouse and wants
        ///  applications to display keyboard interfaces that would be otherwise hidden.
        /// </summary>
        public static bool IsKeyboardPreferred
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETKEYBOARDPREF);

        /// <summary>
        ///  Retrieves the Keyboard repeat speed setting, which is a value in the range
        ///  from 0 through 31. The actual rate may vary depending on the hardware.
        /// </summary>
        public static int KeyboardSpeed
            => UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_GETKEYBOARDSPEED);

        /// <summary>
        ///  Gets the <see cref="Size"/> in pixels of the rectangle within which the mouse
        ///  pointer has to stay to be considered hovering.
        /// </summary>
        public static Size MouseHoverSize
            => new Size(
                UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_GETMOUSEHOVERWIDTH),
                UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_GETMOUSEHOVERHEIGHT));

        /// <summary>
        ///  Gets the time, in milliseconds, that the mouse pointer has to stay in the hover
        ///  rectangle to be considered hovering.
        /// </summary>
        public static int MouseHoverTime
            => UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_GETMOUSEHOVERTIME);

        /// <summary>
        ///  Gets the current mouse speed.
        /// </summary>
        public static int MouseSpeed
            => UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_GETMOUSESPEED);

        /// <summary>
        ///  Determines whether the snap-to-default-button feature is enabled.
        /// </summary>
        public static bool IsSnapToDefaultEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETSNAPTODEFBUTTON);

        /// <summary>
        ///  Determines whether the popup menus are left aligned or right aligned.
        /// </summary>
        public static LeftRightAlignment PopupMenuAlignment
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETMENUDROPALIGNMENT)
                ? LeftRightAlignment.Left : LeftRightAlignment.Right;

        /// <summary>
        ///  Determines whether the menu fade animation feature is enabled.
        /// </summary>
        public static bool IsMenuFadeEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETMENUFADE);

        /// <summary>
        ///  Indicates the time, in milliseconds, that the system waits before displaying
        ///  a shortcut menu.
        /// </summary>
        public static int MenuShowDelay
            => UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_GETMENUSHOWDELAY);

        /// <summary>
        ///  Indicates whether the slide open effect for combo boxes is enabled.
        /// </summary>
        public static bool IsComboBoxAnimationEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETCOMBOBOXANIMATION);

        /// <summary>
        ///  Indicates whether the gradient effect for windows title bars is enabled.
        /// </summary>
        public static bool IsTitleBarGradientEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETGRADIENTCAPTIONS);

        /// <summary>
        ///  Indicates whether the hot tracking of user interface elements is enabled.
        /// </summary>
        public static bool IsHotTrackingEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETHOTTRACKING);

        /// <summary>
        ///  Indicates whether the smooth scrolling effect for listbox is enabled.
        /// </summary>
        public static bool IsListBoxSmoothScrollingEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETLISTBOXSMOOTHSCROLLING);

        /// <summary>
        ///  Indicates whether the menu animation feature is enabled.
        /// </summary>
        public static bool IsMenuAnimationEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETMENUANIMATION);

        /// <summary>
        ///  Indicates whether the selection fade effect is enabled.
        /// </summary>
        public static bool IsSelectionFadeEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETSELECTIONFADE);

        /// <summary>
        ///  Indicates whether tool tip animation is enabled.
        /// </summary>
        public static bool IsToolTipAnimationEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETTOOLTIPANIMATION);

        /// <summary>
        ///  Indicates whether UI effects are enabled.
        /// </summary>
        public static bool UIEffectsEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETUIEFFECTS);

        /// <summary>
        ///  Indicates whether the windows tracking (activating the window the mouse in on) is ON or OFF.
        /// </summary>
        public static bool IsActiveWindowTrackingEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETACTIVEWINDOWTRACKING);

        /// <summary>
        ///  Retrieves the active window tracking delay in milliseconds.
        /// </summary>
        public static int ActiveWindowTrackingDelay
            => UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_GETACTIVEWNDTRKTIMEOUT);

        /// <summary>
        ///  Indicates whether windows minimize/restore animation is enabled.
        /// </summary>
        public static bool IsMinimizeRestoreAnimationEnabled
            => UnsafeNativeMethods.SystemParametersInfoBool(NativeMethods.SPI_GETANIMATION);

        /// <summary>
        ///  Retrieves the border multiplier factor that determines the width of a window's sizing border.
        /// </summary>
        public static int BorderMultiplierFactor
            => UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_GETBORDER);

        /// <summary>
        ///  Indicates the caret blink time.
        /// </summary>
        public static int CaretBlinkTime
        {
            get => unchecked((int)SafeNativeMethods.GetCaretBlinkTime());
        }

        /// <summary>
        ///  Indicates the caret width in edit controls.
        /// </summary>
        public static int CaretWidth
            => UnsafeNativeMethods.SystemParametersInfoInt(NativeMethods.SPI_GETCARETWIDTH);

        public static int MouseWheelScrollDelta => NativeMethods.WHEEL_DELTA;

        /// <summary>
        ///  The width of the left and right edges of the focus rectangle.
        /// </summary>
        public static int VerticalFocusThickness => GetSystemMetrics(SystemMetric.SM_CYFOCUSBORDER);

        /// <summary>
        ///  The width of the top and bottom edges of the focus rectangle.
        /// </summary>
        public static int HorizontalFocusThickness => GetSystemMetrics(SystemMetric.SM_CXFOCUSBORDER);

        /// <summary>
        ///  The height of the vertical sizing border around the perimeter of the window that can be resized.
        /// </summary>
        public static int VerticalResizeBorderThickness => GetSystemMetrics(SystemMetric.SM_CYSIZEFRAME);

        /// <summary>
        ///  The width of the horizontal sizing border around the perimeter of the window that can be resized.
        /// </summary>
        public static int HorizontalResizeBorderThickness => GetSystemMetrics(SystemMetric.SM_CXSIZEFRAME);

        /// <summary>
        ///  The orientation of the screen in degrees.
        /// </summary>
        public static ScreenOrientation ScreenOrientation
        {
            get
            {
                ScreenOrientation so = ScreenOrientation.Angle0;
                NativeMethods.DEVMODE dm = new NativeMethods.DEVMODE
                {
                    dmSize = (short)Marshal.SizeOf<NativeMethods.DEVMODE>(),
                    dmDriverExtra = 0
                };
                try
                {
                    SafeNativeMethods.EnumDisplaySettings(null, -1 /*ENUM_CURRENT_SETTINGS*/, ref dm);
                    if ((dm.dmFields & NativeMethods.DM_DISPLAYORIENTATION) > 0)
                    {
                        so = dm.dmDisplayOrientation;
                    }
                }
                catch
                {
                    // empty catch, we'll just return the default if the call to EnumDisplaySettings fails.
                }

                return so;
            }
        }

        /// <summary>
        ///  The thickness, in pixels, of the sizing border.
        /// </summary>
        public static int SizingBorderWidth
        {
            get
            {
                NativeMethods.NONCLIENTMETRICSW data = default;
                return UnsafeNativeMethods.SystemParametersInfoW(ref data)
                    && data.iBorderWidth > 0 ? data.iBorderWidth : 0;
            }
        }

        /// <summary>
        ///  The <see cref="Size"/>, in pixels, of the small caption buttons.
        /// </summary>
        public unsafe static Size SmallCaptionButtonSize
        {
            get
            {
                NativeMethods.NONCLIENTMETRICSW data = default;
                return UnsafeNativeMethods.SystemParametersInfoW(ref data)
                    && data.iSmCaptionHeight > 0 && data.iSmCaptionWidth > 0
                        ? new Size(data.iSmCaptionWidth, data.iSmCaptionHeight)
                        : Size.Empty;
            }
        }

        /// <summary>
        ///  The <see cref="Size"/>, in pixels, of the menu bar buttons.
        /// </summary>
        public unsafe static Size MenuBarButtonSize
        {
            get
            {
                NativeMethods.NONCLIENTMETRICSW data = default;
                return UnsafeNativeMethods.SystemParametersInfoW(ref data)
                    && data.iMenuHeight > 0 && data.iMenuWidth > 0
                        ? new Size(data.iMenuWidth, data.iMenuHeight)
                        : Size.Empty;
            }
        }

        /// <summary>
        ///  Checks whether the current Winforms app is running on a secure desktop under a terminal
        ///  server session. This is the case when the TS session has been locked.
        ///  This method is useful when calling into GDI+ Graphics methods that modify the object's
        ///  state, these methods fail under a locked terminal session.
        /// </summary>
        internal static bool InLockedTerminalSession()
        {
            if (TerminalServerSession)
            {
                // Try to open the input desktop. If it fails with access denied assume
                // the app is running on a secure desktop.
                IntPtr hDsk = SafeNativeMethods.OpenInputDesktop(0, false, NativeMethods.DESKTOP_SWITCHDESKTOP);
                if (hDsk == IntPtr.Zero)
                {
                    int error = Marshal.GetLastWin32Error();
                    return error == NativeMethods.ERROR_ACCESS_DENIED;
                }

                SafeNativeMethods.CloseDesktop(hDsk);
            }

            return false;
        }
    }
}
