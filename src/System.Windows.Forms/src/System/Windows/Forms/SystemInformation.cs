// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Interop;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.StationsAndDesktops;
using Windows.Win32.UI.Accessibility;
using static Windows.Win32.UI.WindowsAndMessaging.SYSTEM_METRICS_INDEX;
using static Windows.Win32.UI.WindowsAndMessaging.SYSTEM_PARAMETERS_INFO_ACTION;

namespace System.Windows.Forms;

/// <summary>
///  Provides information about the operating system.
/// </summary>
public static class SystemInformation
{
    private static bool s_checkMultiMonitorSupport;
    private static bool s_multiMonitorSupport;

    private static HWINSTA s_processWinStation;
    private static bool s_isUserInteractive;

    private static PowerStatus? s_powerStatus;

    /// <summary>
    ///  Gets a value indicating whether the user has enabled full window drag.
    /// </summary>
    public static bool DragFullWindows
        => PInvokeCore.SystemParametersInfoBool(SPI_GETDRAGFULLWINDOWS);

    /// <summary>
    ///  Gets a value indicating whether the user has selected to run in high contrast.
    /// </summary>
    public static bool HighContrast
    {
        get
        {
            HIGHCONTRASTW data = default;
            return PInvokeCore.SystemParametersInfo(ref data)
                && data.dwFlags.HasFlag(HIGHCONTRASTW_FLAGS.HCF_HIGHCONTRASTON);
        }
    }

    /// <summary>
    ///  Gets the number of lines to scroll when the mouse wheel is rotated.
    /// </summary>
    public static int MouseWheelScrollLines
        => PInvokeCore.SystemParametersInfoInt(SPI_GETWHEELSCROLLLINES);

    /// <summary>
    ///  Gets the dimensions of the primary display monitor in pixels.
    /// </summary>
    public static Size PrimaryMonitorSize => GetSize(SM_CXSCREEN, SM_CYSCREEN);

    /// <summary>
    ///  Gets the width of the vertical scroll bar in pixels.
    /// </summary>
    public static int VerticalScrollBarWidth => PInvokeCore.GetSystemMetrics(SM_CXVSCROLL);

    /// <summary>
    ///  Gets the width of the vertical scroll bar in pixels.
    /// </summary>
    public static int GetVerticalScrollBarWidthForDpi(int dpi)
        => ScaleHelper.IsThreadPerMonitorV2Aware
            ? PInvoke.GetCurrentSystemMetrics(SM_CXVSCROLL, (uint)dpi)
            : PInvokeCore.GetSystemMetrics(SM_CXVSCROLL);

    /// <summary>
    ///  Gets the height of the horizontal scroll bar in pixels.
    /// </summary>
    public static int HorizontalScrollBarHeight => PInvokeCore.GetSystemMetrics(SM_CYHSCROLL);

    /// <summary>
    ///  Gets the height of the horizontal scroll bar in pixels.
    /// </summary>
    public static int GetHorizontalScrollBarHeightForDpi(int dpi)
        => ScaleHelper.IsThreadPerMonitorV2Aware
            ? PInvoke.GetCurrentSystemMetrics(SM_CYHSCROLL, (uint)dpi)
            : PInvokeCore.GetSystemMetrics(SM_CYHSCROLL);

    /// <summary>
    ///  Gets the height of the normal caption area of a window in pixels.
    /// </summary>
    public static int CaptionHeight => PInvokeCore.GetSystemMetrics(SM_CYCAPTION);

    /// <summary>
    ///  Gets the width and height of a window border in pixels.
    /// </summary>
    public static Size BorderSize => GetSize(SM_CXBORDER, SM_CYBORDER);

    /// <summary>
    ///  Gets the width and height of a window border in pixels.
    /// </summary>
    public static Size GetBorderSizeForDpi(int dpi)
    {
        return ScaleHelper.IsThreadPerMonitorV2Aware
            ? new(PInvoke.GetCurrentSystemMetrics(SM_CXBORDER, (uint)dpi),
                PInvoke.GetCurrentSystemMetrics(SM_CYBORDER, (uint)dpi))
            : BorderSize;
    }

    /// <summary>
    ///  Gets the thickness in pixels, of the border for a window that has a caption and is not resizable.
    /// </summary>
    public static Size FixedFrameBorderSize => GetSize(SM_CXFIXEDFRAME, SM_CYFIXEDFRAME);

    /// <summary>
    ///  Gets the height of the scroll box in a vertical scroll bar in pixels.
    /// </summary>
    public static int VerticalScrollBarThumbHeight => PInvokeCore.GetSystemMetrics(SM_CYVTHUMB);

    /// <summary>
    ///  Gets the width of the scroll box in a horizontal scroll bar in pixels.
    /// </summary>
    public static int HorizontalScrollBarThumbWidth => PInvokeCore.GetSystemMetrics(SM_CXHTHUMB);

    /// <summary>
    ///  Gets the default dimensions of an icon in pixels.
    /// </summary>
    public static Size IconSize => GetSize(SM_CXICON, SM_CYICON);

    /// <summary>
    ///  Gets the dimensions of a cursor in pixels.
    /// </summary>
    public static Size CursorSize => GetSize(SM_CXCURSOR, SM_CYCURSOR);

    /// <summary>
    ///  Gets the system's font for menus.
    /// </summary>
    public static Font MenuFont => GetMenuFontHelper(0, useDpi: false);

    /// <summary>
    ///  Gets the system's font for menus, scaled accordingly to an arbitrary DPI you provide.
    /// </summary>
    public static Font GetMenuFontForDpi(int dpi)
        => GetMenuFontHelper((uint)dpi, ScaleHelper.IsThreadPerMonitorV2Aware);

    private static unsafe Font GetMenuFontHelper(uint dpi, bool useDpi)
    {
        // We can get the system's menu font through the NONCLIENTMETRICS structure
        // via SystemParametersInfo
        NONCLIENTMETRICSW data = default;

        bool result = useDpi
            ? PInvokeCore.TrySystemParametersInfoForDpi(ref data, dpi)
            : PInvokeCore.SystemParametersInfo(ref data);

        if (result)
        {
            try
            {
                return Font.FromLogFont(Unsafe.AsRef<LOGFONT>((LOGFONT*)&data.lfMenuFont));
            }
            catch (ArgumentException)
            {
                // Font.FromLogFont throws ArgumentException when it finds
                // a font that is not TrueType. Default to standard control font.
            }
        }

        return Control.DefaultFont;
    }

    /// <summary>
    ///  Gets the height of a one line of a menu in pixels.
    /// </summary>
    public static int MenuHeight => PInvokeCore.GetSystemMetrics(SM_CYMENU);

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
            RECT workingArea = default;
            PInvokeCore.SystemParametersInfo(SPI_GETWORKAREA, ref workingArea);
            return workingArea;
        }
    }

    /// <summary>
    ///  Gets the height, in pixels, of the Kanji window at the bottom of the screen
    ///  for double-byte (DBCS) character set versions of Windows.
    /// </summary>
    public static int KanjiWindowHeight => PInvokeCore.GetSystemMetrics(SM_CYKANJIWINDOW);

    /// <summary>
    ///  Gets a value indicating whether the system has a mouse installed.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool MousePresent => PInvokeCore.GetSystemMetrics(SM_MOUSEPRESENT) != 0;

    /// <summary>
    ///  Gets the height in pixels, of the arrow bitmap on the vertical scroll bar.
    /// </summary>
    public static int VerticalScrollBarArrowHeight => PInvokeCore.GetSystemMetrics(SM_CYVSCROLL);

    /// <summary>
    ///  Gets the height of the vertical scroll bar arrow bitmap in pixels.
    /// </summary>
    public static int VerticalScrollBarArrowHeightForDpi(int dpi)
        => PInvoke.GetCurrentSystemMetrics(SM_CYVSCROLL, (uint)dpi);

    /// <summary>
    ///  Gets the width, in pixels, of the arrow bitmap on the horizontal scrollbar.
    /// </summary>
    public static int HorizontalScrollBarArrowWidth => PInvokeCore.GetSystemMetrics(SM_CXHSCROLL);

    /// <summary>
    ///  Gets the width of the horizontal scroll bar arrow bitmap in pixels.
    /// </summary>
    public static int GetHorizontalScrollBarArrowWidthForDpi(int dpi)
        => ScaleHelper.IsThreadPerMonitorV2Aware
            ? PInvoke.GetCurrentSystemMetrics(SM_CXHSCROLL, (uint)dpi)
            : PInvokeCore.GetSystemMetrics(SM_CXHSCROLL);

    /// <summary>
    ///  Gets a value indicating whether this is a debug version of the operating system.
    /// </summary>
    public static bool DebugOS => PInvokeCore.GetSystemMetrics(SM_DEBUG) != 0;

    /// <summary>
    ///  Gets a value indicating whether the functions of the left and right mouse
    ///  buttons have been swapped.
    /// </summary>
    public static bool MouseButtonsSwapped => PInvokeCore.GetSystemMetrics(SM_SWAPBUTTON) != 0;

    /// <summary>
    ///  Gets the minimum allowable dimensions of a window in pixels.
    /// </summary>
    public static Size MinimumWindowSize => GetSize(SM_CXMIN, SM_CYMIN);

    /// <summary>
    ///  Gets the dimensions in pixels, of a caption bar or title bar button.
    /// </summary>
    public static Size CaptionButtonSize => GetSize(SM_CXSIZE, SM_CYSIZE);

    /// <summary>
    ///  Gets the thickness in pixels, of the border for a window that can be resized.
    /// </summary>
    public static Size FrameBorderSize => GetSize(SM_CXFRAME, SM_CYFRAME);

    /// <summary>
    ///  Gets the system's default minimum tracking dimensions of a window in pixels.
    /// </summary>
    public static Size MinWindowTrackSize => GetSize(SM_CXMINTRACK, SM_CYMINTRACK);

    /// <summary>
    ///  Gets the dimensions in pixels, of the area that the user must click within
    ///  for the system to consider the two clicks a double-click. The rectangle is
    ///  centered around the first click.
    /// </summary>
    public static Size DoubleClickSize => GetSize(SM_CXDOUBLECLK, SM_CYDOUBLECLK);

    /// <summary>
    ///  Gets the maximum number of milliseconds allowed between mouse clicks for a double-click.
    /// </summary>
    public static int DoubleClickTime => (int)PInvoke.GetDoubleClickTime();

    /// <summary>
    ///  Gets the dimensions in pixels, of the grid used to arrange icons in a large icon view.
    /// </summary>
    public static Size IconSpacingSize => GetSize(SM_CXICONSPACING, SM_CYICONSPACING);

    /// <summary>
    ///  Gets a value indicating whether drop down menus should be right-aligned with the corresponding menu
    ///  bar item.
    /// </summary>
    public static bool RightAlignedMenus => PInvokeCore.GetSystemMetrics(SM_MENUDROPALIGNMENT) != 0;

    /// <summary>
    ///  Gets a value indicating whether the Microsoft Windows for Pen computing extensions are installed.
    /// </summary>
    public static bool PenWindows => PInvokeCore.GetSystemMetrics(SM_PENWINDOWS) != 0;

    /// <summary>
    ///  Gets a value indicating whether the operating system is capable of handling
    ///  double-byte (DBCS) characters.
    /// </summary>
    public static bool DbcsEnabled => PInvokeCore.GetSystemMetrics(SM_DBCSENABLED) != 0;

    /// <summary>
    ///  Gets the number of buttons on mouse.
    /// </summary>
    public static int MouseButtons => PInvokeCore.GetSystemMetrics(SM_CMOUSEBUTTONS);

    /// <summary>
    ///  Gets a value indicating whether security is present on this operating system.
    /// </summary>
    public static bool Secure => PInvokeCore.GetSystemMetrics(SM_SECURE) != 0;

    /// <summary>
    ///  Gets the dimensions in pixels, of a 3-D border.
    /// </summary>
    public static Size Border3DSize => GetSize(SM_CXEDGE, SM_CYEDGE);

    /// <summary>
    ///  Gets the dimensions in pixels, of the grid into which minimized windows will be placed.
    /// </summary>
    public static Size MinimizedWindowSpacingSize => GetSize(SM_CXMINSPACING, SM_CYMINSPACING);

    /// <summary>
    ///  Gets the recommended dimensions of a small icon in pixels.
    /// </summary>
    public static Size SmallIconSize => GetSize(SM_CXSMICON, SM_CYSMICON);

    /// <summary>
    ///  Gets the height of a small caption in pixels.
    /// </summary>
    public static int ToolWindowCaptionHeight => PInvokeCore.GetSystemMetrics(SM_CYSMCAPTION);

    /// <summary>
    ///  Gets the dimensions of small caption buttons in pixels.
    /// </summary>
    public static Size ToolWindowCaptionButtonSize => GetSize(SM_CXSMSIZE, SM_CYSMSIZE);

    /// <summary>
    ///  Gets the dimensions in pixels, of menu bar buttons.
    /// </summary>
    public static Size MenuButtonSize => GetSize(SM_CXMENUSIZE, SM_CYMENUSIZE);

    /// <summary>
    ///  Gets flags specifying how the system arranges minimized windows.
    /// </summary>
    public static ArrangeStartingPosition ArrangeStartingPosition
    {
        get
        {
            ArrangeStartingPosition mask = ArrangeStartingPosition.BottomLeft
                | ArrangeStartingPosition.BottomRight
                | ArrangeStartingPosition.Hide
                | ArrangeStartingPosition.TopLeft
                | ArrangeStartingPosition.TopRight;
            int compoundValue = PInvokeCore.GetSystemMetrics(SM_ARRANGE);
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
            ArrangeDirection mask = ArrangeDirection.Down
                | ArrangeDirection.Left | ArrangeDirection.Right | ArrangeDirection.Up;
            int compoundValue = PInvokeCore.GetSystemMetrics(SM_ARRANGE);
            return mask & (ArrangeDirection)compoundValue;
        }
    }

    /// <summary>
    ///  Gets the dimensions in pixels, of a normal minimized window.
    /// </summary>
    public static Size MinimizedWindowSize => GetSize(SM_CXMINIMIZED, SM_CYMINIMIZED);

    /// <summary>
    ///  Gets the default maximum dimensions in pixels, of a window that has a
    ///  caption and sizing borders.
    /// </summary>
    public static Size MaxWindowTrackSize => GetSize(SM_CXMAXTRACK, SM_CYMAXTRACK);

    /// <summary>
    ///  Gets the default dimensions, in pixels, of a maximized top-left window on the
    ///  primary monitor.
    /// </summary>
    public static Size PrimaryMonitorMaximizedWindowSize => GetSize(SM_CXMAXIMIZED, SM_CYMAXIMIZED);

    /// <summary>
    ///  Gets a value indicating whether this computer is connected to a network.
    /// </summary>
    public static bool Network => (PInvokeCore.GetSystemMetrics(SM_NETWORK) & 0x00000001) != 0;

    public static bool TerminalServerSession => (PInvokeCore.GetSystemMetrics(SM_REMOTESESSION) & 0x00000001) != 0;

    /// <summary>
    ///  Gets a value that specifies how the system was started.
    /// </summary>
    public static BootMode BootMode => (BootMode)PInvokeCore.GetSystemMetrics(SM_CLEANBOOT);

    /// <summary>
    ///  Gets the dimensions in pixels, of the rectangle that a drag operation must
    ///  extend to be considered a drag. The rectangle is centered on a drag point.
    /// </summary>
    public static Size DragSize => GetSize(SM_CXDRAG, SM_CYDRAG);

    /// <summary>
    ///  Gets a value indicating whether the user requires an application to present
    ///  information visually in situations where it would otherwise present the
    ///  information in audible form.
    /// </summary>
    public static bool ShowSounds => PInvokeCore.GetSystemMetrics(SM_SHOWSOUNDS) != 0;

    /// <summary>
    ///  Gets the dimensions of the default size of a menu checkmark in pixels.
    /// </summary>
    public static Size MenuCheckSize => GetSize(SM_CXMENUCHECK, SM_CYMENUCHECK);

    /// <summary>
    ///  Gets a value indicating whether the system is enabled for Hebrew and Arabic languages.
    /// </summary>
    public static bool MidEastEnabled => PInvokeCore.GetSystemMetrics(SM_MIDEASTENABLED) != 0;

    internal static bool MultiMonitorSupport
    {
        get
        {
            if (!s_checkMultiMonitorSupport)
            {
                s_multiMonitorSupport = PInvokeCore.GetSystemMetrics(SM_CMONITORS) != 0;
                s_checkMultiMonitorSupport = true;
            }

            return s_multiMonitorSupport;
        }
    }

    /// <summary>
    ///  Gets a value indicating whether a mouse with a mouse wheel is installed.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This was never really correct. All versions of Windows NT from 4.0 onward supported the mouse wheel
    ///   directly. This should have been a version check. Rather than change it and risk breaking apps we'll
    ///   keep it equivalent to <see cref="MouseWheelPresent"/>.
    ///  </para>
    /// </remarks>
    public static bool NativeMouseWheelSupport => PInvokeCore.GetSystemMetrics(SM_MOUSEWHEELPRESENT) != 0;

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
                return new(PInvokeCore.GetSystemMetrics(SM_XVIRTUALSCREEN),
                    PInvokeCore.GetSystemMetrics(SM_YVIRTUALSCREEN),
                    PInvokeCore.GetSystemMetrics(SM_CXVIRTUALSCREEN),
                    PInvokeCore.GetSystemMetrics(SM_CYVIRTUALSCREEN));
            }

            Size size = PrimaryMonitorSize;
            return new Rectangle(0, 0, size.Width, size.Height);
        }
    }

    /// <summary>
    ///  Gets the number of display monitors on the desktop.
    /// </summary>
    public static int MonitorCount => MultiMonitorSupport ? PInvokeCore.GetSystemMetrics(SM_CMONITORS) : 1;

    /// <summary>
    ///  Gets a value indicating whether all the display monitors have the same color format.
    /// </summary>
    public static bool MonitorsSameDisplayFormat
        => !MultiMonitorSupport || PInvokeCore.GetSystemMetrics(SM_SAMEDISPLAYFORMAT) != 0;

    /// <summary>
    ///  Gets the computer name of the current system.
    /// </summary>
    public static string ComputerName => Environment.MachineName;

    /// <summary>
    ///  Gets the user's domain name.
    /// </summary>
    public static string UserDomainName => Environment.UserDomainName;

    /// <summary>
    ///  Gets a value indicating whether the current process is running in user interactive mode.
    /// </summary>
    public static unsafe bool UserInteractive
    {
        get
        {
            HWINSTA hwinsta = PInvoke.GetProcessWindowStation();
            if (!hwinsta.IsNull && s_processWinStation != hwinsta)
            {
                s_isUserInteractive = true;

                USEROBJECTFLAGS flags = default;
                if (PInvoke.GetUserObjectInformation(
                    (HANDLE)hwinsta.Value,
                    USER_OBJECT_INFORMATION_INDEX.UOI_FLAGS,
                    &flags,
                    (uint)sizeof(USEROBJECTFLAGS),
                    lpnLengthNeeded: null))
                {
                    if ((flags.dwFlags & PInvoke.WSF_VISIBLE) == 0)
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
    public static string UserName => Environment.UserName;

    /// <summary>
    ///  Gets whether the drop shadow effect in enabled.
    /// </summary>
    public static bool IsDropShadowEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETDROPSHADOW);

    /// <summary>
    ///  Gets whether the native user menus have a flat menu appearance.
    /// </summary>
    public static bool IsFlatMenuEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETFLATMENU);

    /// <summary>
    ///  Gets whether font smoothing is enabled.
    /// </summary>
    public static bool IsFontSmoothingEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETFONTSMOOTHING);

    /// <summary>
    ///  Returns the ClearType smoothing contrast value.
    /// </summary>
    public static int FontSmoothingContrast => PInvokeCore.SystemParametersInfoInt(SPI_GETFONTSMOOTHINGCONTRAST);

    /// <summary>
    ///  Returns a type of Font smoothing.
    /// </summary>
    public static int FontSmoothingType => PInvokeCore.SystemParametersInfoInt(SPI_GETFONTSMOOTHINGTYPE);

    /// <summary>
    ///  Retrieves the width in pixels of an icon cell.
    /// </summary>
    public static int IconHorizontalSpacing => PInvokeCore.SystemParametersInfoInt(SPI_ICONHORIZONTALSPACING);

    /// <summary>
    ///  Retrieves the height in pixels of an icon cell.
    /// </summary>
    public static int IconVerticalSpacing => PInvokeCore.SystemParametersInfoInt(SPI_ICONVERTICALSPACING);

    /// <summary>
    ///  Gets whether icon title wrapping is enabled.
    /// </summary>
    public static bool IsIconTitleWrappingEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETICONTITLEWRAP);

    /// <summary>
    ///  Gets whether menu access keys are underlined.
    /// </summary>
    public static bool MenuAccessKeysUnderlined => PInvokeCore.SystemParametersInfoBool(SPI_GETKEYBOARDCUES);

    /// <summary>
    ///  Retrieves the Keyboard repeat delay setting, which is a value in the range
    ///  from 0 through 3. The actual delay associated with each value may vary
    ///  depending on the hardware.
    /// </summary>
    public static int KeyboardDelay => PInvokeCore.SystemParametersInfoInt(SPI_GETKEYBOARDDELAY);

    /// <summary>
    ///  Gets whether the user relies on keyboard instead of mouse and wants
    ///  applications to display keyboard interfaces that would be otherwise hidden.
    /// </summary>
    public static bool IsKeyboardPreferred => PInvokeCore.SystemParametersInfoBool(SPI_GETKEYBOARDPREF);

    /// <summary>
    ///  Retrieves the Keyboard repeat speed setting, which is a value in the range
    ///  from 0 through 31. The actual rate may vary depending on the hardware.
    /// </summary>
    public static int KeyboardSpeed => PInvokeCore.SystemParametersInfoInt(SPI_GETKEYBOARDSPEED);

    /// <summary>
    ///  Gets the <see cref="Size"/> in pixels of the rectangle within which the mouse
    ///  pointer has to stay to be considered hovering.
    /// </summary>
    public static Size MouseHoverSize
        => new(PInvokeCore.SystemParametersInfoInt(SPI_GETMOUSEHOVERWIDTH),
            PInvokeCore.SystemParametersInfoInt(SPI_GETMOUSEHOVERHEIGHT));

    /// <summary>
    ///  Gets the time, in milliseconds, that the mouse pointer has to stay in the hover
    ///  rectangle to be considered hovering.
    /// </summary>
    public static int MouseHoverTime => PInvokeCore.SystemParametersInfoInt(SPI_GETMOUSEHOVERTIME);

    /// <summary>
    ///  Gets the current mouse speed.
    /// </summary>
    public static int MouseSpeed => PInvokeCore.SystemParametersInfoInt(SPI_GETMOUSESPEED);

    /// <summary>
    ///  Determines whether the snap-to-default-button feature is enabled.
    /// </summary>
    public static bool IsSnapToDefaultEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETSNAPTODEFBUTTON);

    /// <summary>
    ///  Determines whether the popup menus are left aligned or right aligned.
    /// </summary>
    public static LeftRightAlignment PopupMenuAlignment
        => PInvokeCore.SystemParametersInfoBool(SPI_GETMENUDROPALIGNMENT)
            ? LeftRightAlignment.Left : LeftRightAlignment.Right;

    /// <summary>
    ///  Determines whether the menu fade animation feature is enabled.
    /// </summary>
    public static bool IsMenuFadeEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETMENUFADE);

    /// <summary>
    ///  Indicates the time, in milliseconds, that the system waits before displaying
    ///  a shortcut menu.
    /// </summary>
    public static int MenuShowDelay => PInvokeCore.SystemParametersInfoInt(SPI_GETMENUSHOWDELAY);

    /// <summary>
    ///  Indicates whether the slide open effect for combo boxes is enabled.
    /// </summary>
    public static bool IsComboBoxAnimationEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETCOMBOBOXANIMATION);

    /// <summary>
    ///  Indicates whether the gradient effect for windows title bars is enabled.
    /// </summary>
    public static bool IsTitleBarGradientEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETGRADIENTCAPTIONS);

    /// <summary>
    ///  Indicates whether the hot tracking of user interface elements is enabled.
    /// </summary>
    public static bool IsHotTrackingEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETHOTTRACKING);

    /// <summary>
    ///  Indicates whether the smooth scrolling effect for listbox is enabled.
    /// </summary>
    public static bool IsListBoxSmoothScrollingEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETLISTBOXSMOOTHSCROLLING);

    /// <summary>
    ///  Indicates whether the menu animation feature is enabled.
    /// </summary>
    public static bool IsMenuAnimationEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETMENUANIMATION);

    /// <summary>
    ///  Indicates whether the selection fade effect is enabled.
    /// </summary>
    public static bool IsSelectionFadeEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETSELECTIONFADE);

    /// <summary>
    ///  Indicates whether tool tip animation is enabled.
    /// </summary>
    public static bool IsToolTipAnimationEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETTOOLTIPANIMATION);

    /// <summary>
    ///  Indicates whether UI effects are enabled.
    /// </summary>
    public static bool UIEffectsEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETUIEFFECTS);

    /// <summary>
    ///  Indicates whether the windows tracking (activating the window the mouse in on) is ON or OFF.
    /// </summary>
    public static bool IsActiveWindowTrackingEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETACTIVEWINDOWTRACKING);

    /// <summary>
    ///  Retrieves the active window tracking delay in milliseconds.
    /// </summary>
    public static int ActiveWindowTrackingDelay => PInvokeCore.SystemParametersInfoInt(SPI_GETACTIVEWNDTRKTIMEOUT);

    /// <summary>
    ///  Indicates whether windows minimize/restore animation is enabled.
    /// </summary>
    public static bool IsMinimizeRestoreAnimationEnabled => PInvokeCore.SystemParametersInfoBool(SPI_GETANIMATION);

    /// <summary>
    ///  Retrieves the border multiplier factor that determines the width of a window's sizing border.
    /// </summary>
    public static int BorderMultiplierFactor => PInvokeCore.SystemParametersInfoInt(SPI_GETBORDER);

    /// <summary>
    ///  Indicates the caret blink time.
    /// </summary>
    public static int CaretBlinkTime => (int)PInvoke.GetCaretBlinkTime();

    /// <summary>
    ///  Indicates the caret width in edit controls.
    /// </summary>
    public static int CaretWidth => PInvokeCore.SystemParametersInfoInt(SPI_GETCARETWIDTH);

    public static int MouseWheelScrollDelta => (int)PInvoke.WHEEL_DELTA;

    /// <summary>
    ///  The width of the left and right edges of the focus rectangle.
    /// </summary>
    public static int VerticalFocusThickness => PInvokeCore.GetSystemMetrics(SM_CYFOCUSBORDER);

    /// <summary>
    ///  The width of the top and bottom edges of the focus rectangle.
    /// </summary>
    public static int HorizontalFocusThickness => PInvokeCore.GetSystemMetrics(SM_CXFOCUSBORDER);

    /// <summary>
    ///  The height of the vertical sizing border around the perimeter of the window that can be resized.
    /// </summary>
    public static int VerticalResizeBorderThickness => PInvokeCore.GetSystemMetrics(SM_CYSIZEFRAME);

    /// <summary>
    ///  The width of the horizontal sizing border around the perimeter of the window that can be resized.
    /// </summary>
    public static int HorizontalResizeBorderThickness => PInvokeCore.GetSystemMetrics(SM_CXSIZEFRAME);

    /// <summary>
    ///  The orientation of the screen in degrees.
    /// </summary>
    public static unsafe ScreenOrientation ScreenOrientation
    {
        get
        {
            ScreenOrientation so = ScreenOrientation.Angle0;
            DEVMODEW dm = new()
            {
                dmSize = (ushort)sizeof(DEVMODEW),
            };

            PInvoke.EnumDisplaySettings(lpszDeviceName: null, ENUM_DISPLAY_SETTINGS_MODE.ENUM_CURRENT_SETTINGS, ref dm);
            if ((dm.dmFields & DEVMODE_FIELD_FLAGS.DM_DISPLAYORIENTATION) > 0)
            {
                so = (ScreenOrientation)dm.Anonymous1.Anonymous2.dmDisplayOrientation;
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
            NONCLIENTMETRICSW data = default;
            return PInvokeCore.SystemParametersInfo(ref data)
                && data.iBorderWidth > 0 ? data.iBorderWidth : 0;
        }
    }

    /// <summary>
    ///  The <see cref="Size"/>, in pixels, of the small caption buttons.
    /// </summary>
    public static unsafe Size SmallCaptionButtonSize
    {
        get
        {
            NONCLIENTMETRICSW data = default;
            return PInvokeCore.SystemParametersInfo(ref data)
                && data.iSmCaptionHeight > 0 && data.iSmCaptionWidth > 0
                    ? new Size(data.iSmCaptionWidth, data.iSmCaptionHeight)
                    : Size.Empty;
        }
    }

    /// <summary>
    ///  The <see cref="Size"/>, in pixels, of the menu bar buttons.
    /// </summary>
    public static unsafe Size MenuBarButtonSize
    {
        get
        {
            NONCLIENTMETRICSW data = default;
            return PInvokeCore.SystemParametersInfo(ref data)
                && data.iMenuHeight > 0 && data.iMenuWidth > 0
                    ? new Size(data.iMenuWidth, data.iMenuHeight)
                    : Size.Empty;
        }
    }

    /// <summary>
    ///  Checks whether the current WinForms app is running on a secure desktop under a terminal
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
            HDESK desktop = PInvoke.OpenInputDesktop(0, false, DESKTOP_ACCESS_FLAGS.DESKTOP_SWITCHDESKTOP);
            if (desktop.IsNull)
            {
                return Marshal.GetLastWin32Error() == (int)WIN32_ERROR.ERROR_ACCESS_DENIED;
            }

            PInvoke.CloseDesktop(desktop);
        }

        return false;
    }

    private static Size GetSize(SYSTEM_METRICS_INDEX x, SYSTEM_METRICS_INDEX y)
        => new(PInvokeCore.GetSystemMetrics(x), PInvokeCore.GetSystemMetrics(y));
}
