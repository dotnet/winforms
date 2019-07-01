// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides information about the operating system.
    /// </summary>
    public static class SystemInformation
    {
        // Figure out if all the multimon stuff is supported on the OS
        private static bool s_checkMultiMonitorSupport;
        private static bool s_multiMonitorSupport;
        private static bool s_checkNativeMouseWheelSupport;
        private static bool s_nativeMouseWheelSupport = true;
        private static bool s_highContrast;
        private static bool s_systemEventsAttached;
        private static bool s_systemEventsDirty = true;

        private static IntPtr s_processWinStation = IntPtr.Zero;
        private static bool s_isUserInteractive;

        private static PowerStatus s_powerStatus;

        private const int DefaultMouseWheelScrollLines = 3;

        /// <summary>
        /// Gets a value indicating whether the user has enabled full window drag.
        /// </summary>
        public static bool DragFullWindows
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETDRAGFULLWINDOWS, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user has selected to run in high contrast.
        /// </summary>
        public static bool HighContrast
        {
            get
            {
                EnsureSystemEvents();
                if (s_systemEventsDirty)
                {
                    var data = new NativeMethods.HIGHCONTRAST_I
                    {
                        cbSize = Marshal.SizeOf<NativeMethods.HIGHCONTRAST_I>(),
                        dwFlags = 0,
                        lpszDefaultScheme = IntPtr.Zero
                    };

                    // Force it to false if we fail to get the parameter.
                    if (UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETHIGHCONTRAST, data.cbSize, ref data, 0))
                    {
                        s_highContrast = (data.dwFlags & NativeMethods.HCF_HIGHCONTRASTON) != 0;
                    }
                    else
                    {
                        s_highContrast = false;
                    }

                    s_systemEventsDirty = false;
                }

                return s_highContrast;
            }
        }

        /// <summary>
        /// Gets the number of lines to scroll when the mouse wheel is rotated.
        /// </summary>
        public static int MouseWheelScrollLines
        {
            get
            {
                if (NativeMouseWheelSupport)
                {
                    int data = 0;
                    UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETWHEELSCROLLLINES, 0, ref data, 0);
                    return data;
                }

                // Check for the MouseZ "service". This is a little app that generated the
                // MSH_MOUSEWHEEL messages by monitoring the hardware. If this app isn't
                // found, then there is no support for MouseWheels on the system.
                IntPtr hWndMouseWheel = UnsafeNativeMethods.FindWindow(NativeMethods.MOUSEZ_CLASSNAME, NativeMethods.MOUSEZ_TITLE);
                if (hWndMouseWheel != IntPtr.Zero)
                {
                    // Register the MSH_SCROLL_LINES message...
                    int message = SafeNativeMethods.RegisterWindowMessage(NativeMethods.MSH_SCROLL_LINES);
                    int lines = (int)UnsafeNativeMethods.SendMessage(new HandleRef(null, hWndMouseWheel), message, 0, 0);

                    // This fails under terminal server, so we default to 3, which is the Windows
                    // default. Nobody seems to pay attention to this value anyways.
                    if (lines != 0)
                    {
                        return lines;
                    }
                }

                return DefaultMouseWheelScrollLines;
            }
        }

        /// <summary>
        /// Gets the dimensions of the primary display monitor in pixels.
        /// </summary>
        public static Size PrimaryMonitorSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXSCREEN),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYSCREEN));
        }

        /// <summary>
        /// Gets the width of the vertical scroll bar in pixels.
        /// </summary>
        public static int VerticalScrollBarWidth
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXVSCROLL);
        }

        /// <summary>
        /// Gets the width of the vertical scroll bar in pixels.
        /// </summary>
        public static int GetVerticalScrollBarWidthForDpi(int dpi)
        {
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                return UnsafeNativeMethods.TryGetSystemMetricsForDpi(NativeMethods.SM_CXVSCROLL, (uint)dpi);
            }
            else
            {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXVSCROLL);
            }
        }

        /// <summary>
        /// Gets the height of the horizontal scroll bar in pixels.
        /// </summary>
        public static int HorizontalScrollBarHeight
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYHSCROLL);
        }

        /// <summary>
        /// Gets the height of the horizontal scroll bar in pixels.
        /// </summary>
        public static int GetHorizontalScrollBarHeightForDpi(int dpi)
        {
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                return UnsafeNativeMethods.TryGetSystemMetricsForDpi(NativeMethods.SM_CYHSCROLL, (uint)dpi);
            }
            else
            {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYHSCROLL);
            }
        }

        /// <summary>
        /// Gets the height of the normal caption area of a window in pixels.
        /// </summary>
        public static int CaptionHeight
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYCAPTION);
        }

        /// <summary>
        /// Gets the width and height of a window border in pixels.
        /// </summary>
        public static Size BorderSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXBORDER),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYBORDER));
        }

        /// <summary>
        /// Gets the width andheight of a window border in pixels.
        /// </summary>
        public static Size GetBorderSizeForDpi(int dpi)
        {
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                return new Size(UnsafeNativeMethods.TryGetSystemMetricsForDpi(NativeMethods.SM_CXBORDER, (uint)dpi),
                                UnsafeNativeMethods.TryGetSystemMetricsForDpi(NativeMethods.SM_CYBORDER, (uint)dpi));
            }
            else
            {
                return BorderSize;
            }
        }

        /// <summary>
        /// Gets the thickness in pixels, of the border for a window that has a caption
        /// and is not resizable.
        /// </summary>
        public static Size FixedFrameBorderSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXFIXEDFRAME),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYFIXEDFRAME));
        }

        /// <summary>
        /// Gets the height of the scroll box in a vertical scroll bar in pixels.
        /// </summary>
        public static int VerticalScrollBarThumbHeight
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYVTHUMB);
        }

        /// <summary>
        /// Gets the width of the scroll box in a horizontal scroll bar in pixels.
        /// </summary>
        public static int HorizontalScrollBarThumbWidth
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXHTHUMB);
        }

        /// <summary>
        /// Gets the default dimensions of an icon in pixels.
        /// </summary>
        public static Size IconSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXICON),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYICON));
        }

        /// <summary>
        /// Gets the dimensions of a cursor in pixels.
        /// </summary>
        public static Size CursorSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXCURSOR),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYCURSOR));
        }

        /// <summary>
        /// Gets the system's font for menus.
        /// </summary>
        public static Font MenuFont => GetMenuFontHelper(0, useDpi: false);

        /// <summary>
        /// Gets the system's font for menus, scaled accordingly to an arbitrary DPI you provide.
        /// </summary>
        public static Font GetMenuFontForDpi(int dpi)
        {
            return GetMenuFontHelper((uint)dpi, DpiHelper.IsPerMonitorV2Awareness);
        }

        private static Font GetMenuFontHelper(uint dpi, bool useDpi)
        {
            Font menuFont = null;

            // We can get the system's menu font through the NONCLIENTMETRICS structure
            // via SystemParametersInfo
            var data = new NativeMethods.NONCLIENTMETRICS();
            bool result;
            if (useDpi)
            {
                result = UnsafeNativeMethods.TrySystemParametersInfoForDpi(NativeMethods.SPI_GETNONCLIENTMETRICS, data.cbSize, data, 0, dpi);
            }
            else
            {
                result = UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, data.cbSize, data, 0);
            }

            if (result && data.lfMenuFont != null)
            {
                try
                {
                    menuFont = Font.FromLogFont(data.lfMenuFont);
                }
                catch
                {
                    // Menu font is not true type. Default to standard control font.
                    menuFont = Control.DefaultFont;
                }
            }
            return menuFont;
        }

        /// <summary>
        /// Gets the height of a one line of a menu in pixels.
        /// </summary>
        public static int MenuHeight
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMENU);
        }

        /// <summary>
        /// Returns the current system power status.
        /// </summary>
        public static PowerStatus PowerStatus
        {
            get => s_powerStatus ?? (s_powerStatus = new PowerStatus());
        }

        /// <summary>
        /// Gets the size of the working area in pixels.
        /// </summary>
        public static Rectangle WorkingArea
        {
            get
            {
                var rc = new NativeMethods.RECT();
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETWORKAREA, 0, ref rc, 0);
                return Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom);
            }
        }

        /// <summary>
        /// Gets the height, in pixels, of the Kanji window at the bottom of the screen
        /// for double-byte (DBCS) character set versions of Windows.
        /// </summary>
        public static int KanjiWindowHeight
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYKANJIWINDOW);
        }

        /// <summary>
        /// Gets a value indicating whether the system has a mouse installed.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool MousePresent
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_MOUSEPRESENT) != 0;
        }

        /// <summary>
        /// Gets the height in pixels, of the arrow bitmap on the vertical scroll bar.
        /// </summary>
        public static int VerticalScrollBarArrowHeight
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYVSCROLL);
        }

        /// <summary>
        /// Gets the height of the vertical scroll bar arrow bitmap in pixels.
        /// </summary>
        public static int VerticalScrollBarArrowHeightForDpi(int dpi)
        {
            return UnsafeNativeMethods.TryGetSystemMetricsForDpi(NativeMethods.SM_CYVSCROLL, (uint)dpi);
        }

        /// <summary>
        /// Gets the width, in pixels, of the arrow bitmap on the horizontal scrollbar.
        /// </summary>
        public static int HorizontalScrollBarArrowWidth
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXHSCROLL);
        }

        /// <summary>
        /// Gets the width of the horizontal scroll bar arrow bitmap in pixels.
        /// </summary>
        public static int GetHorizontalScrollBarArrowWidthForDpi(int dpi)
        {
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                return UnsafeNativeMethods.TryGetSystemMetricsForDpi(NativeMethods.SM_CXHSCROLL, (uint)dpi);
            }
            else
            {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXHSCROLL);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is a debug version of the operating system.
        /// </summary>
        public static bool DebugOS
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_DEBUG) != 0;
        }

        /// <summary>
        /// Gets a value indicating whether the functions of the left and right mouse
        /// buttons have been swapped.
        /// </summary>
        public static bool MouseButtonsSwapped
        {
            get => (UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_SWAPBUTTON) != 0);
        }

        /// <summary>
        /// Gets the minimum allowable dimensions of a window in pixels.
        /// </summary>
        public static Size MinimumWindowSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMIN),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMIN));
        }

        /// <summary>
        /// Gets the dimensions in pixels, of a caption bar or title bar button.
        /// </summary>
        public static Size CaptionButtonSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXSIZE),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYSIZE));
        }

        /// <summary>
        /// Gets the thickness in pixels, of the border for a window that can be resized.
        /// </summary>
        public static Size FrameBorderSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXFRAME),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYFRAME));
        }

        /// <summary>
        /// Gets the system's default minimum tracking dimensions of a window in pixels.
        /// </summary>
        public static Size MinWindowTrackSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMINTRACK),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMINTRACK));
        }

        /// <summary>
        /// Gets the dimensions in pixels, of the area that the user must click within
        /// for the system to consider the two clicks a double-click. The rectangle is
        /// centered around the first click.
        /// </summary>
        public static Size DoubleClickSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXDOUBLECLK),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYDOUBLECLK));
        }

        /// <summary>
        /// Gets the maximum number of milliseconds allowed between mouse clicks for a
        /// double-click.
        /// </summary>
        public static int DoubleClickTime => SafeNativeMethods.GetDoubleClickTime();

        /// <summary>
        /// Gets the dimensions in pixels, of the grid used to arrange icons in a large
        /// icon view.
        /// </summary>
        public static Size IconSpacingSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXICONSPACING),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYICONSPACING));
        }

        /// <summary>
        /// Gets a value indicating whether drop down menus should be right-aligned with
        /// the corresponding menu bar item.
        /// </summary>
        public static bool RightAlignedMenus
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_MENUDROPALIGNMENT) != 0;
        }

        /// <summary>
        /// Gets a value indicating whether the Microsoft Windows for Pen computing
        /// extensions are installed.
        /// </summary>
        public static bool PenWindows
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_PENWINDOWS) != 0;
        }

        /// <summary>
        /// Gets a value indicating whether the operating system is capable of handling
        /// double-byte (DBCS) characters.
        /// </summary>
        public static bool DbcsEnabled
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_DBCSENABLED) != 0;
        }

        /// <summary>
        /// Gets the number of buttons on mouse.
        /// </summary>
        public static int MouseButtons
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CMOUSEBUTTONS);
        }

        /// <summary>
        /// Gets a value indicating whether security is present on this operating system.
        /// </summary>
        public static bool Secure
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_SECURE) != 0;
        }

        /// <summary>
        /// Gets the dimensions in pixels, of a 3-D border.
        /// </summary>
        public static Size Border3DSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXEDGE),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYEDGE));
        }

        /// <summary>
        /// Gets the dimensions in pixels, of the grid into which minimized windows will
        /// be placed.
        /// </summary>
        public static Size MinimizedWindowSpacingSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMINSPACING),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMINSPACING));
        }

        /// <summary>
        /// Gets the recommended dimensions of a small icon in pixels.
        /// </summary>
        public static Size SmallIconSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXSMICON),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYSMICON));
        }

        /// <summary>
        /// Gets the height of a small caption in pixels.
        /// </summary>
        public static int ToolWindowCaptionHeight
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYSMCAPTION);
        }

        /// <summary>
        /// Gets the dimensions of small caption buttons in pixels.
        /// </summary>
        public static Size ToolWindowCaptionButtonSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXSMSIZE),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYSMSIZE));
        }

        /// <summary>
        /// Gets the dimensions in pixels, of menu bar buttons.
        /// </summary>
        public static Size MenuButtonSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMENUSIZE),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMENUSIZE));
        }

        /// <summary>
        /// Gets flags specifying how the system arranges minimized windows.
        /// </summary>
        public static ArrangeStartingPosition ArrangeStartingPosition
        {
            get
            {
                ArrangeStartingPosition mask = ArrangeStartingPosition.BottomLeft | ArrangeStartingPosition.BottomRight | ArrangeStartingPosition.Hide | ArrangeStartingPosition.TopLeft | ArrangeStartingPosition.TopRight;
                int compoundValue = UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_ARRANGE);
                return mask & (ArrangeStartingPosition)compoundValue;
            }
        }

        /// <summary>
        /// Gets flags specifying how the system arranges minimized windows.
        /// </summary>
        public static ArrangeDirection ArrangeDirection
        {
            get
            {
                ArrangeDirection mask = ArrangeDirection.Down | ArrangeDirection.Left | ArrangeDirection.Right | ArrangeDirection.Up;
                int compoundValue = UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_ARRANGE);
                return mask & (ArrangeDirection)compoundValue;
            }
        }

        /// <summary>
        /// Gets the dimensions in pixels, of a normal minimized window.
        /// </summary>
        public static Size MinimizedWindowSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMINIMIZED),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMINIMIZED));
        }

        /// <summary>
        /// Gets the default maximum dimensions in pixels, of a window that has a
        /// caption and sizing borders.
        /// </summary>
        public static Size MaxWindowTrackSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMAXTRACK),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMAXTRACK));
        }

        /// <summary>
        /// Gets the default dimensions, in pixels, of a maximized top-left window on the
        /// primary monitor.
        /// </summary>
        public static Size PrimaryMonitorMaximizedWindowSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMAXIMIZED),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMAXIMIZED));
        }

        /// <summary>
        /// Gets a value indicating whether this computer is connected to a network.
        /// </summary>
        public static bool Network
        {
            get => (UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_NETWORK) & 0x00000001) != 0;
        }

        public static bool TerminalServerSession
        {
            get => (UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_REMOTESESSION) & 0x00000001) != 0;
        }

        /// <summary>
        /// Gets a value that specifies how the system was started.
        /// </summary>
        public static BootMode BootMode
        {
            get => (BootMode)UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CLEANBOOT);
        }

        /// <summary>
        /// Gets the dimensions in pixels, of the rectangle that a drag operation must
        /// extend to be considered a drag. The rectangle is centered on a drag point.
        /// </summary>
        public static Size DragSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXDRAG),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYDRAG));
        }

        /// <summary>
        /// Gets a value indicating whether the user requires an application to present
        /// information visually in situations where it would otherwise present the
        /// information in audible form.
        /// </summary>
        public static bool ShowSounds
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_SHOWSOUNDS) != 0;
        }

        /// <summary>
        /// Gets the dimensions of the default size of a menu checkmark in pixels.
        /// </summary>
        public static Size MenuCheckSize
        {
            get => new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMENUCHECK),
                            UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMENUCHECK));
        }

        /// <summary>
        /// Gets a value indicating whether the system is enabled for Hebrew and Arabic
        /// languages.
        /// </summary>
        public static bool MidEastEnabled
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_MIDEASTENABLED) != 0;
        }

        private static bool MultiMonitorSupport
        {
            get
            {
                if (!s_checkMultiMonitorSupport)
                {
                    s_multiMonitorSupport = (UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CMONITORS) != 0);
                    s_checkMultiMonitorSupport = true;
                }

                return s_multiMonitorSupport;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the system natively supports the mouse wheel
        /// in newer mice.
        /// </summary>
        public static bool NativeMouseWheelSupport
        {
            get
            {
                if (!s_checkNativeMouseWheelSupport)
                {
                    s_nativeMouseWheelSupport = (UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_MOUSEWHEELPRESENT) != 0);
                    s_checkNativeMouseWheelSupport = true;
                }

                return s_nativeMouseWheelSupport;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is a mouse with a mouse wheel
        /// installed on this machine.
        /// </summary>
        public static bool MouseWheelPresent
        {
            get
            {
                if (!NativeMouseWheelSupport)
                {
                    // Check for the MouseZ "service". This is a little app that generated the
                    // MSH_MOUSEWHEEL messages by monitoring the hardware. If this app isn't
                    // found, then there is no support for MouseWheels on the system.
                    return UnsafeNativeMethods.FindWindow(NativeMethods.MOUSEZ_CLASSNAME, NativeMethods.MOUSEZ_TITLE) != IntPtr.Zero;
                }

                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_MOUSEWHEELPRESENT) != 0;
            }
        }

        /// <summary>
        /// Gets the bounds of the virtual screen.
        /// </summary>
        public static Rectangle VirtualScreen
        {
            get
            {
                if (MultiMonitorSupport)
                {
                    return new Rectangle(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_XVIRTUALSCREEN),
                                         UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_YVIRTUALSCREEN),
                                         UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXVIRTUALSCREEN),
                                         UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYVIRTUALSCREEN));
                }

                Size size = PrimaryMonitorSize;
                return new Rectangle(0, 0, size.Width, size.Height);
            }
        }

        /// <summary>
        /// Gets the number of display monitors on the desktop.
        /// </summary>
        public static int MonitorCount
        {
            get
            {
                if (MultiMonitorSupport)
                {
                    return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CMONITORS);
                }

                return 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether all the display monitors have the same color format.
        /// </summary>
        public static bool MonitorsSameDisplayFormat
        {
            get
            {
                if (MultiMonitorSupport)
                {
                    return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_SAMEDISPLAYFORMAT) != 0;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the computer name of the current system.
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
        /// Gets the user's domain name.
        /// </summary>
        public static string UserDomainName => Environment.UserDomainName;

        /// <summary>
        /// Gets a value indicating whether the current process is running in user
        /// interactive mode.
        /// </summary>
        public static bool UserInteractive
        {
            get
            {
                IntPtr hwinsta = UnsafeNativeMethods.GetProcessWindowStation();
                if (hwinsta != IntPtr.Zero && s_processWinStation != hwinsta)
                {
                    s_isUserInteractive = true;

                    int lengthNeeded = 0;
                    NativeMethods.USEROBJECTFLAGS flags = new NativeMethods.USEROBJECTFLAGS();

                    if (UnsafeNativeMethods.GetUserObjectInformation(new HandleRef(null, hwinsta), NativeMethods.UOI_FLAGS, ref flags, Marshal.SizeOf<NativeMethods.USEROBJECTFLAGS>(), ref lengthNeeded))
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
        /// Gets the user name for the current thread, that is, the name of the user
        /// currently logged onto the system.
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
        /// Gets a value indicating whether the drop shadow effect in enabled.
        /// Defaults to false downlevel.
        /// </summary>
        public static bool IsDropShadowEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETDROPSHADOW, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the native user menus have a flat menu
        /// appearance. Defaults to false downlevel.
        /// </summary>
        public static bool IsFlatMenuEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETFLATMENU, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Font Smoothing OSFeature.Feature is enabled.
        /// </summary>
        public static bool IsFontSmoothingEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETFONTSMOOTHING, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Returns a contrast value that is ClearType smoothing.
        /// </summary>
        public static int FontSmoothingContrast
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETFONTSMOOTHINGCONTRAST, 0, ref data, 0);
                return data;
            }
        }

        /// <summary>
        /// Returns a type of Font smoothing.
        /// </summary>
        public static int FontSmoothingType
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETFONTSMOOTHINGTYPE, 0, ref data, 0);
                return data;
            }
        }

        /// <summary>
        /// Retrieves the width in pixels of an icon cell.
        /// </summary>
        public static int IconHorizontalSpacing
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_ICONHORIZONTALSPACING, 0, ref data, 0);
                return data;
            }
        }

        /// <summary>
        /// Retrieves the height in pixels of an icon cell.
        /// </summary>
        public static int IconVerticalSpacing
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_ICONVERTICALSPACING, 0, ref data, 0);
                return data;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Icon title wrapping is enabled.
        /// </summary>
        public static bool IsIconTitleWrappingEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETICONTITLEWRAP, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the menu access keys are always underlined.
        /// </summary>
        public static bool MenuAccessKeysUnderlined
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETKEYBOARDCUES, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Retrieves the Keyboard repeat delay setting, which is a value in the range
        /// from 0 through 3. The Actual Delay Associated with each value may vary
        /// depending on the hardware.
        /// </summary>
        public static int KeyboardDelay
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETKEYBOARDDELAY, 0, ref data, 0);
                return data;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user relies on Keyboard instead of mouse and wants
        /// applications to display keyboard interfaces that would be otherwise hidden.
        /// </summary>
        public static bool IsKeyboardPreferred
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETKEYBOARDPREF, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Retrieves the Keyboard repeat speed setting, which is a value in the range
        /// from 0 through 31. The actual rate may vary depending on the hardware.
        /// </summary>
        public static int KeyboardSpeed
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETKEYBOARDSPEED, 0, ref data, 0);
                return data;
            }
        }

        /// <summary>
        /// Gets the Size in pixels of the rectangle within which the mouse pointer has to stay.
        /// </summary>
        public static Size MouseHoverSize
        {
            get
            {
                int height = 0;
                int width = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMOUSEHOVERHEIGHT, 0, ref height, 0);
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMOUSEHOVERWIDTH, 0, ref width, 0);
                return new Size(width, height);
            }
        }

        /// <summary>
        /// Gets the time, in milliseconds, that the mouse pointer has to stay in the hover rectangle.
        /// </summary>
        public static int MouseHoverTime
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMOUSEHOVERTIME, 0, ref data, 0);
                return data;
            }
        }

        /// <summary>
        /// Gets the current mouse speed.
        /// </summary>
        public static int MouseSpeed
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMOUSESPEED, 0, ref data, 0);
                return data;
            }
        }

        /// <summary>
        /// Determines whether the snap-to-default-button feature is enabled.
        /// </summary>
        public static bool IsSnapToDefaultEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETSNAPTODEFBUTTON, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Determines whether the Popup Menus are left Aligned or Right Aligned.
        /// </summary>
        public static LeftRightAlignment PopupMenuAlignment
        {
            get
            {
                bool data = false;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMENUDROPALIGNMENT, 0, ref data, 0);
                return data ? LeftRightAlignment.Left : LeftRightAlignment.Right;
            }
        }

        /// <summary>
        /// Determines whether the maenu fade animation feature is enabled. Defaults to false
        /// downlevel.
        /// </summary>
        public static bool IsMenuFadeEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMENUFADE, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Indicates the time, in milliseconds, that the system waits before displaying
        /// a shortcut menu.
        /// </summary>
        public static int MenuShowDelay
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMENUSHOWDELAY, 0, ref data, 0);
                return data;
            }
        }

        /// <summary>
        /// Indicates whether the slide open effect for combo boxes is enabled.
        /// </summary>
        public static bool IsComboBoxAnimationEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETCOMBOBOXANIMATION, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Indicates whether the gradient effect for windows title bars is enabled.
        /// </summary>
        public static bool IsTitleBarGradientEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETGRADIENTCAPTIONS, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Indicates whether the hot tracking of user interface elements is enabled.
        /// </summary>
        public static bool IsHotTrackingEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETHOTTRACKING, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Indicates whether the smooth scrolling effect for listbox is enabled.
        /// </summary>
        public static bool IsListBoxSmoothScrollingEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETLISTBOXSMOOTHSCROLLING, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Indicates whether the menu animation feature is enabled.
        /// </summary>
        public static bool IsMenuAnimationEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMENUANIMATION, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Indicates whether the selection fade effect is enabled. Defaults to false
        /// downlevel.
        /// </summary>
        public static bool IsSelectionFadeEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETSELECTIONFADE, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Indicates whether the tool tip animation is enabled. Defaults to false
        /// downlevel.
        /// </summary>
        public static bool IsToolTipAnimationEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETTOOLTIPANIMATION, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Indicates whether all the UI Effects are enabled. Defaults to false
        /// downlevel.
        /// </summary>
        public static bool UIEffectsEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETUIEFFECTS, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Indicates whether the active windows tracking (activating the window the mouse in on) is ON or OFF.
        /// </summary>
        public static bool IsActiveWindowTrackingEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETACTIVEWINDOWTRACKING, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Retrieves the active window tracking delay, in milliseconds.
        /// </summary>
        public static int ActiveWindowTrackingDelay
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETACTIVEWNDTRKTIMEOUT, 0, ref data, 0);
                return data;
            }
        }

        /// <summary>
        /// Indicates whether the active windows tracking (activating the window the mouse in on) is ON or OFF.
        /// </summary>
        public static bool IsMinimizeRestoreAnimationEnabled
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETANIMATION, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <summary>
        /// Retrieves the border multiplier factor that determines the width of a windo's sizing border.
        /// </summary>
        public static int BorderMultiplierFactor
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETBORDER, 0, ref data, 0);
                return data;
            }
        }

        /// <summary>
        /// Indicates the caret blink time.
        /// </summary>
        public static int CaretBlinkTime
        {
            get => unchecked((int)SafeNativeMethods.GetCaretBlinkTime());
        }

        /// <summary>
        /// Indicates the caret width in edit controls.
        /// </summary>
        public static int CaretWidth
        {
            get
            {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETCARETWIDTH, 0, ref data, 0);
                return data;
            }
        }

        public static int MouseWheelScrollDelta => NativeMethods.WHEEL_DELTA;

        /// <summary>
        /// The width of the left and right edges of the focus rectangle.
        /// </summary>
        public static int VerticalFocusThickness
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYFOCUSBORDER);
        }

        /// <summary>
        /// The width of the top and bottom edges of the focus rectangle.
        /// </summary>
        public static int HorizontalFocusThickness
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXFOCUSBORDER);
        }

        /// <summary>
        /// The height of the vertical sizing border around the perimeter of the window that can be resized.
        /// </summary>
        public static int VerticalResizeBorderThickness
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYSIZEFRAME);
        }

        /// <summary>
        /// The width of the horizontal sizing border around the perimeter of the window that can be resized.
        /// </summary>
        public static int HorizontalResizeBorderThickness
        {
            get => UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXSIZEFRAME);
        }

        /// <summary>
        /// The orientation of the screen in degrees.
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
        /// Specifies the thikness, in pixels, of the Sizing Border.
        /// </summary>
        public static int SizingBorderWidth
        {
            get
            {
                // We can get the system's menu font through the NONCLIENTMETRICS structure via SystemParametersInfo
                var data = new NativeMethods.NONCLIENTMETRICS();
                bool result = UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, data.cbSize, data, 0);
                return result && data.iBorderWidth > 0 ? data.iBorderWidth : 0;
            }
        }

        /// <summary>
        /// Specified the Size, in pixels, of the small caption buttons.
        /// </summary>
        public static Size SmallCaptionButtonSize
        {
            get
            {
                // We can get the system's menu font through the NONCLIENTMETRICS structure via SystemParametersInfo
                var data = new NativeMethods.NONCLIENTMETRICS();
                bool result = UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, data.cbSize, data, 0);
                if (result && data.iSmCaptionHeight > 0 && data.iSmCaptionWidth > 0)
                {
                    return new Size(data.iSmCaptionWidth, data.iSmCaptionHeight);
                }
                else
                {
                    return Size.Empty;
                }
            }
        }

        /// <summary>
        /// Specified the Size, in pixels, of the menu bar buttons.
        /// </summary>
        public static Size MenuBarButtonSize
        {
            get
            {
                // We can get the system's menu font through the NONCLIENTMETRICS structure via SystemParametersInfo
                var data = new NativeMethods.NONCLIENTMETRICS();
                bool result = UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, data.cbSize, data, 0);
                if (result && data.iMenuHeight > 0 && data.iMenuWidth > 0)
                {
                    return new Size(data.iMenuWidth, data.iMenuHeight);
                }
                else
                {
                    return Size.Empty;
                }
            }
        }

        /// <summary>
        /// Checks whether the current Winforms app is running on a secure desktop under a terminal
        /// server session. This is the case when the TS session has been locked.
        /// This method is useful when calling into GDI+ Graphics methods that modify the object's
        /// state, these methods fail under a locked terminal session.
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
