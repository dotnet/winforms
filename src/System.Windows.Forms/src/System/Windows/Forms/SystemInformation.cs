// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Text;
    using System.Configuration.Assemblies;
    using System.Threading;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System;
    using Microsoft.Win32;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using System.Drawing;
    using System.ComponentModel;
    using System.Runtime.Versioning;

    /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation"]/*' />
    /// <devdoc>
    ///    <para>Provides information about the operating system.</para>
    /// </devdoc>
    public class SystemInformation {

        // private constructor to prevent creation
        //
        private SystemInformation() {
        }

        // Figure out if all the multimon stuff is supported on the OS
        //
        private static bool checkMultiMonitorSupport = false;
        private static bool multiMonitorSupport = false;
        private static bool checkNativeMouseWheelSupport = false;
        private static bool nativeMouseWheelSupport = true;
        private static bool highContrast = false;
        private static bool systemEventsAttached = false;
        private static bool systemEventsDirty = true;

        private static IntPtr processWinStation = IntPtr.Zero;
        private static bool isUserInteractive = false;

        private static PowerStatus powerStatus = null;

        private const int  DefaultMouseWheelScrollLines = 3;
        
        ////////////////////////////////////////////////////////////////////////////
        // SystemParametersInfo
        //

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.DragFullWindows"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the user has enabled full window drag.
        ///    </para>
        /// </devdoc>
        public static bool DragFullWindows {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETDRAGFULLWINDOWS, 0, ref data, 0);
                return data != 0;
            }
        }
        
        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.HighContrast"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the user has selected to run in high contrast
        ///       mode.
        ///    </para>
        /// </devdoc>
        public static bool HighContrast {
            get {
                EnsureSystemEvents();
                if (systemEventsDirty) {
                    NativeMethods.HIGHCONTRAST_I data = new NativeMethods.HIGHCONTRAST_I();
                    data.cbSize = Marshal.SizeOf(data);
                    data.dwFlags = 0;
                    data.lpszDefaultScheme = IntPtr.Zero;
                    
                    bool b = UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETHIGHCONTRAST, data.cbSize, ref data, 0);
    
                    // NT4 does not support this parameter, so we always force
                    // it to false if we fail to get the parameter.
                    //
                    if (b) {
                        highContrast = (data.dwFlags & NativeMethods.HCF_HIGHCONTRASTON) != 0;
                    }
                    else {
                        highContrast = false;
                    }
                    systemEventsDirty = false;
                }
                
                return highContrast;
            }
        }
        
        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MouseWheelScrollLines"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the number of lines to scroll when the mouse wheel is rotated.
        ///    </para>
        /// </devdoc>
        public static int MouseWheelScrollLines {
            get {
                if (NativeMouseWheelSupport) {
                    int data = 0;
                    UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETWHEELSCROLLLINES, 0, ref data, 0);
                    return data;
                }
                else {
                    IntPtr hWndMouseWheel = IntPtr.Zero;

                    // Check for the MouseZ "service". This is a little app that generated the
                    // MSH_MOUSEWHEEL messages by monitoring the hardware. If this app isn't
                    // found, then there is no support for MouseWheels on the system.
                    //
                    hWndMouseWheel = UnsafeNativeMethods.FindWindow(NativeMethods.MOUSEZ_CLASSNAME, NativeMethods.MOUSEZ_TITLE);

                    if (hWndMouseWheel != IntPtr.Zero) {

                        // Register the MSH_SCROLL_LINES message...
                        //
                        int message = SafeNativeMethods.RegisterWindowMessage(NativeMethods.MSH_SCROLL_LINES);

                        
                        int lines = (int)UnsafeNativeMethods.SendMessage(new HandleRef(null, hWndMouseWheel), message, 0, 0);
                        
                        // this fails under terminal server, so we default to 3, which is the windows
                        // default.  Nobody seems to pay attention to this value anyways...
                        if (lines != 0) {
                            return lines;
                        }
                    }
                }

                return DefaultMouseWheelScrollLines;
            }
        }
        
        ////////////////////////////////////////////////////////////////////////////
        // SystemMetrics
        //

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.PrimaryMonitorSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the dimensions of the primary display monitor in pixels.
        ///    </para>
        /// </devdoc>
        public static Size PrimaryMonitorSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXSCREEN),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYSCREEN));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.VerticalScrollBarWidth"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the width of the vertical scroll bar in pixels.
        ///    </para>
        /// </devdoc>
        public static int VerticalScrollBarWidth {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXVSCROLL);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.VerticalScrollBarWidth"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the width of the vertical scroll bar in pixels.
        ///    </para>
        /// </devdoc>
        public static int GetVerticalScrollBarWidthForDpi(int dpi) {
            if (DpiHelper.IsPerMonitorV2Awareness) {
                return UnsafeNativeMethods.TryGetSystemMetricsForDpi(NativeMethods.SM_CXVSCROLL, (uint)dpi);
            }
            else {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXVSCROLL);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.HorizontalScrollBarHeight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the height of the horizontal scroll bar in pixels.
        ///    </para>
        /// </devdoc>
        public static int HorizontalScrollBarHeight {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYHSCROLL);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.HorizontalScrollBarHeight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the height of the horizontal scroll bar in pixels.
        ///    </para>
        /// </devdoc>
        public static int GetHorizontalScrollBarHeightForDpi(int dpi) {
            if (DpiHelper.IsPerMonitorV2Awareness) {
                return UnsafeNativeMethods.TryGetSystemMetricsForDpi(NativeMethods.SM_CYHSCROLL, (uint)dpi);
            }
            else {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYHSCROLL);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.CaptionHeight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the height of the normal caption area of a window in pixels.
        ///    </para>
        /// </devdoc>
        public static int CaptionHeight {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYCAPTION);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.BorderSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the width and
        ///       height of a window border in pixels.
        ///    </para>
        /// </devdoc>
        public static Size BorderSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXBORDER),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYBORDER));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.GetBorderSizeForDpi"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the width and
        ///       height of a window border in pixels.
        ///    </para>
        /// </devdoc>
        public static Size GetBorderSizeForDpi(int dpi) {
            if (DpiHelper.IsPerMonitorV2Awareness) {
                return new Size(UnsafeNativeMethods.TryGetSystemMetricsForDpi(NativeMethods.SM_CXBORDER, (uint)dpi),
                                UnsafeNativeMethods.TryGetSystemMetricsForDpi(NativeMethods.SM_CYBORDER, (uint)dpi));
            }
            else {
                return BorderSize;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.FixedFrameBorderSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the thickness in pixels, of the border for a window that has a caption
        ///       and is not resizable.
        ///    </para>
        /// </devdoc>
        public static Size FixedFrameBorderSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXFIXEDFRAME),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYFIXEDFRAME));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.VerticalScrollBarThumbHeight"]/*' />
        /// <devdoc>
        ///    <para>Gets the height of the scroll box in a vertical scroll bar in pixels.</para>
        /// </devdoc>
        public static int VerticalScrollBarThumbHeight {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYVTHUMB);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.HorizontalScrollBarThumbWidth"]/*' />
        /// <devdoc>
        ///    <para>Gets the width of the scroll box in a horizontal scroll bar in pixels.</para>
        /// </devdoc>
        public static int HorizontalScrollBarThumbWidth {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXHTHUMB);
            }
        }
/*
        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IconFont"]/*' />
        public static Font IconFont {
            get {
                Font iconFont = IconFontInternal;
                return iconFont != null ? iconFont : Control.DefaultFont;
            }
        }

        // IconFontInternal is the same as IconFont, only it does not fall back to Control.DefaultFont
        // if the icon font can not be retrieved.  It returns null instead.
        internal static Font IconFontInternal {
            get {
                Font iconFont = null;

                NativeMethods.ICONMETRICS data = new NativeMethods.ICONMETRICS();
                bool result = UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETICONMETRICS, data.cbSize, data, 0);

                Debug.Assert(!result || data.iHorzSpacing == IconHorizontalSpacing, "Spacing in ICONMETRICS does not match IconHorizontalSpacing.");
                Debug.Assert(!result || data.iVertSpacing == IconVerticalSpacing, "Spacing in ICONMETRICS does not match IconVerticalSpacing.");

                if (result && data.lfFont != null) {
                    try {
                        iconFont = Font.FromLogFont(data.lfFont);
                    }
                    catch {}
                }
                return iconFont;
            }
        }
*/

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IconSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the default dimensions of an icon in pixels.
        ///    </para>
        /// </devdoc>
        public static Size IconSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXICON),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYICON));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.CursorSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the dimensions of a cursor in pixels.
        ///    </para>
        /// </devdoc>
        public static Size CursorSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXCURSOR),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYCURSOR));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MenuFont"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the system's font for menus.
        ///    </para>
        /// </devdoc>
        public static Font MenuFont {
            [ResourceExposure(ResourceScope.Process)]
            [ResourceConsumption(ResourceScope.Process)]
            get {
                return GetMenuFontHelper(0, false);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.GetMenuFontForDpi"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the system's font for menus, scaled accordingly to an arbitrary DPI you provide.
        ///    </para>
        /// </devdoc>
        public static Font GetMenuFontForDpi(int dpi) {
            return GetMenuFontHelper((uint)dpi, DpiHelper.IsPerMonitorV2Awareness);
        }

        private static Font GetMenuFontHelper(uint dpi, bool useDpi) {
            Font menuFont = null;

            //we can get the system's menu font through the NONCLIENTMETRICS structure via SystemParametersInfo
            //
            NativeMethods.NONCLIENTMETRICS data = new NativeMethods.NONCLIENTMETRICS();
            bool result;
            if (useDpi) {
                result = UnsafeNativeMethods.TrySystemParametersInfoForDpi(NativeMethods.SPI_GETNONCLIENTMETRICS, data.cbSize, data, 0, dpi);
            } else {
                result = UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, data.cbSize, data, 0);
            }

            if (result && data.lfMenuFont != null) {
                // 

                IntSecurity.ObjectFromWin32Handle.Assert();
                try {
                    menuFont = Font.FromLogFont(data.lfMenuFont);
                }
                catch {
                    // menu font is not true type.  Default to standard control font.
                    //
                    menuFont = Control.DefaultFont;
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
            }
            return menuFont;
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MenuHeight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the height of a one line of a menu in pixels.
        ///    </para>
        /// </devdoc>
        public static int MenuHeight {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMENU);
            }
        }

        
        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.PowerStatus"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Returns the current system power status.        
        ///    </para>
        /// </devdoc>
        public static PowerStatus PowerStatus
        {
            get
            {
                if (powerStatus == null) {
                    powerStatus = new PowerStatus();
                }
                return powerStatus;
            }
        }
        
        
        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.WorkingArea"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the size of the working area in pixels.
        ///    </para>
        /// </devdoc>
        public static Rectangle WorkingArea {
            get {
                NativeMethods.RECT rc = new NativeMethods.RECT();
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETWORKAREA, 0, ref rc, 0);
                return Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.KanjiWindowHeight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       the height, in pixels, of the Kanji window at the bottom
        ///       of the screen for double-byte (DBCS) character set versions of Windows.
        ///    </para>
        /// </devdoc>
        public static int KanjiWindowHeight {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYKANJIWINDOW);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MousePresent"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the system has a mouse installed.
        ///    </para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool MousePresent {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_MOUSEPRESENT) != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.VerticalScrollBarArrowHeight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the height in pixels, of the arrow bitmap on the vertical scroll bar.
        ///    </para>
        /// </devdoc>
        public static int VerticalScrollBarArrowHeight {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYVSCROLL);
            }
        }

        /// <summary>
        /// Gets the height of the vertical scroll bar arrow bitmap in pixels.
        /// </summary>
        /// <param name="dpi"></param>
        /// <returns></returns>
        public static int VerticalScrollBarArrowHeightForDpi(int dpi) {
                return UnsafeNativeMethods.TryGetSystemMetricsForDpi(NativeMethods.SM_CXHSCROLL, (uint)dpi);
        }
        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.HorizontalScrollBarArrowWidth"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the width, in pixels, of the arrow bitmap on the horizontal scrollbar.
        ///    </para>
        /// </devdoc>
        public static int HorizontalScrollBarArrowWidth {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXHSCROLL);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.VerticalScrollBarWidth"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the width of the horizontal scroll bar arrow bitmap in pixels.
        ///    </para>
        /// </devdoc>
        public static int GetHorizontalScrollBarArrowWidthForDpi(int dpi) {
            if (DpiHelper.IsPerMonitorV2Awareness) {
                return UnsafeNativeMethods.TryGetSystemMetricsForDpi(NativeMethods.SM_CXHSCROLL, (uint)dpi);
            }
            else {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXHSCROLL);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.DebugOS"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether this is a debug version of the operating
        ///       system.
        ///    </para>
        /// </devdoc>
        public static bool DebugOS {
            get {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "SensitiveSystemInformation Demanded");
                IntSecurity.SensitiveSystemInformation.Demand();
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_DEBUG) != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MouseButtonsSwapped"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the functions of the left and right mouse
        ///       buttons have been swapped.
        ///    </para>
        /// </devdoc>
        public static bool MouseButtonsSwapped {
            get {
                return(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_SWAPBUTTON) != 0);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MinimumWindowSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the minimum allowable dimensions of a window in pixels.
        ///    </para>
        /// </devdoc>
        public static Size MinimumWindowSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMIN),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMIN));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.CaptionButtonSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the dimensions in pixels, of a caption bar or title bar
        ///       button.
        ///    </para>
        /// </devdoc>
        public static Size CaptionButtonSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXSIZE),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYSIZE));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.FrameBorderSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the thickness in pixels, of the border for a window that can be resized.
        ///    </para>
        /// </devdoc>
        public static Size FrameBorderSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXFRAME),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYFRAME));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MinWindowTrackSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the system's default
        ///       minimum tracking dimensions of a window in pixels.
        ///    </para>
        /// </devdoc>
        public static Size MinWindowTrackSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMINTRACK),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMINTRACK));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.DoubleClickSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the dimensions in pixels, of the area that the user must click within
        ///       for the system to consider the two clicks a double-click. The rectangle is
        ///       centered around the first click.
        ///    </para>
        /// </devdoc>
        public static Size DoubleClickSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXDOUBLECLK),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYDOUBLECLK));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.DoubleClickTime"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the maximum number of milliseconds allowed between mouse clicks for a
        ///       double-click.
        ///    </para>
        /// </devdoc>
        public static int DoubleClickTime {
            get {
                return SafeNativeMethods.GetDoubleClickTime();
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IconSpacingSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       the dimensions in pixels, of the grid used
        ///       to arrange icons in a large icon view.
        ///    </para>
        /// </devdoc>
        public static Size IconSpacingSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXICONSPACING),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYICONSPACING));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.RightAlignedMenus"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether drop down menus should be right-aligned with
        ///       the corresponding menu bar item.
        ///    </para>
        /// </devdoc>
        public static bool RightAlignedMenus {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_MENUDROPALIGNMENT) != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.PenWindows"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the Microsoft Windows for Pen computing
        ///       extensions are installed.
        ///    </para>
        /// </devdoc>
        public static bool PenWindows {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_PENWINDOWS) != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.DbcsEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the operating system is capable of handling
        ///       double-byte (DBCS) characters.
        ///    </para>
        /// </devdoc>
        public static bool DbcsEnabled {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_DBCSENABLED) != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MouseButtons"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the number of buttons on mouse.
        ///    </para>
        /// </devdoc>
        public static int MouseButtons {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CMOUSEBUTTONS);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.Secure"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether security is present on this operating system.
        ///    </para>
        /// </devdoc>
        public static bool Secure {
            get {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "SensitiveSystemInformation Demanded");
                IntSecurity.SensitiveSystemInformation.Demand();
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_SECURE) != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.Border3DSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the dimensions in pixels, of a 3-D
        ///       border.
        ///    </para>
        /// </devdoc>
        public static Size Border3DSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXEDGE),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYEDGE));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MinimizedWindowSpacingSize"]/*' />
        /// <devdoc>
        ///    <para>Gets the dimensions
        ///       in pixels, of
        ///       the grid into which minimized windows will be placed.</para>
        /// </devdoc>
        public static Size MinimizedWindowSpacingSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMINSPACING),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMINSPACING));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.SmallIconSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       the recommended dimensions of a small icon in pixels.
        ///    </para>
        /// </devdoc>
        public static Size SmallIconSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXSMICON),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYSMICON));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.ToolWindowCaptionHeight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the height of
        ///       a small caption in pixels.
        ///    </para>
        /// </devdoc>
        public static int ToolWindowCaptionHeight {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYSMCAPTION);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.ToolWindowCaptionButtonSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the
        ///       dimensions of small caption buttons in pixels.
        ///    </para>
        /// </devdoc>
        public static Size ToolWindowCaptionButtonSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXSMSIZE),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYSMSIZE));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MenuButtonSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       the dimensions in pixels, of menu bar buttons.
        ///    </para>
        /// </devdoc>
        public static Size MenuButtonSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMENUSIZE),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMENUSIZE));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.ArrangeStartingPosition"]/*' />
        /// <devdoc>
        ///    <para>Gets flags specifying how the system arranges minimized windows.</para>
        /// </devdoc>
        public static ArrangeStartingPosition ArrangeStartingPosition {
            get {
                ArrangeStartingPosition mask = ArrangeStartingPosition.BottomLeft | ArrangeStartingPosition.BottomRight | ArrangeStartingPosition.Hide | ArrangeStartingPosition.TopLeft | ArrangeStartingPosition.TopRight;
                int compoundValue = UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_ARRANGE);
                return mask & (ArrangeStartingPosition) compoundValue;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.ArrangeDirection"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets flags specifying how the system arranges minimized windows.
        ///    </para>
        /// </devdoc>
        public static ArrangeDirection ArrangeDirection {
            get {
                ArrangeDirection mask = ArrangeDirection.Down | ArrangeDirection.Left | ArrangeDirection.Right | ArrangeDirection.Up;
                int compoundValue = UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_ARRANGE);
                return mask & (ArrangeDirection) compoundValue;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MinimizedWindowSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the dimensions in pixels, of a normal minimized window.
        ///    </para>
        /// </devdoc>
        public static Size MinimizedWindowSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMINIMIZED),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMINIMIZED));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MaxWindowTrackSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the default maximum dimensions in pixels, of a
        ///       window that has a caption and sizing borders.
        ///    </para>
        /// </devdoc>
        public static Size MaxWindowTrackSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMAXTRACK),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMAXTRACK));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.PrimaryMonitorMaximizedWindowSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the default dimensions, in pixels, of a maximized top-left window on the
        ///       primary monitor.
        ///    </para>
        /// </devdoc>
        public static Size PrimaryMonitorMaximizedWindowSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMAXIMIZED),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMAXIMIZED));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.Network"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether this computer is connected to a network.
        ///    </para>
        /// </devdoc>
        public static bool Network {
            get {
                return(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_NETWORK) & 0x00000001) != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.TerminalServerSession"]/*' />
        public static bool TerminalServerSession {
            get {
                return(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_REMOTESESSION) & 0x00000001) != 0;
            }
        }



        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.BootMode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value that specifies how the system was started.
        ///    </para>
        /// </devdoc>
        public static BootMode BootMode {
            get {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "SensitiveSystemInformation Demanded");
                IntSecurity.SensitiveSystemInformation.Demand();
                return(BootMode) UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CLEANBOOT);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.DragSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the dimensions in pixels, of the rectangle that a drag operation
        ///       must extend to be considered a drag. The rectangle is centered on a drag
        ///       point.
        ///    </para>
        /// </devdoc>
        public static Size DragSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXDRAG),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYDRAG));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.ShowSounds"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the user requires an application to present
        ///       information visually in situations where it would otherwise present the
        ///       information in audible form.
        ///    </para>
        /// </devdoc>
        public static bool ShowSounds {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_SHOWSOUNDS) != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MenuCheckSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the
        ///       dimensions of the default size of a menu checkmark in pixels.
        ///    </para>
        /// </devdoc>
        public static Size MenuCheckSize {
            get {
                return new Size(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXMENUCHECK),
                                UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYMENUCHECK));
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MidEastEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value
        ///       indicating whether the system is enabled for Hebrew and Arabic languages.
        ///    </para>
        /// </devdoc>
        public static bool MidEastEnabled {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_MIDEASTENABLED) != 0;
            }
        }

        private static bool MultiMonitorSupport {
            get {
                if (!checkMultiMonitorSupport) {
                    multiMonitorSupport = (UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CMONITORS) != 0);
                    checkMultiMonitorSupport = true;
                }
                return multiMonitorSupport;
            }
        }
        
        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.NativeMouseWheelSupport"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the system natively supports the mouse wheel
        ///       in newer mice.
        ///    </para>
        /// </devdoc>
        public static bool NativeMouseWheelSupport {
            get {
                if (!checkNativeMouseWheelSupport) {
                    nativeMouseWheelSupport = (UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_MOUSEWHEELPRESENT) != 0);
                    checkNativeMouseWheelSupport = true;
                }
                return nativeMouseWheelSupport;
            }
        }


        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MouseWheelPresent"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether there is a mouse with a mouse wheel
        ///       installed on this machine.
        ///    </para>
        /// </devdoc>
        public static bool MouseWheelPresent {
            get {

                bool mouseWheelPresent = false;

                if (!NativeMouseWheelSupport) {
                    IntPtr hWndMouseWheel = IntPtr.Zero;

                    // Check for the MouseZ "service". This is a little app that generated the
                    // MSH_MOUSEWHEEL messages by monitoring the hardware. If this app isn't
                    // found, then there is no support for MouseWheels on the system.
                    //
                    hWndMouseWheel = UnsafeNativeMethods.FindWindow(NativeMethods.MOUSEZ_CLASSNAME, NativeMethods.MOUSEZ_TITLE);

                    if (hWndMouseWheel != IntPtr.Zero) {
                        mouseWheelPresent = true;
                    }
                }
                else {
                    mouseWheelPresent = (UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_MOUSEWHEELPRESENT) != 0);
                }
                return mouseWheelPresent;
            }
        }


        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.VirtualScreen"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the
        ///       bounds of the virtual screen.
        ///    </para>
        /// </devdoc>
        public static Rectangle VirtualScreen {
            get {
                if (MultiMonitorSupport) {
                    return new Rectangle(UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_XVIRTUALSCREEN),
                                         UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_YVIRTUALSCREEN),
                                         UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXVIRTUALSCREEN),
                                         UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYVIRTUALSCREEN));
                }
                else {
                    Size size = PrimaryMonitorSize;
                    return new Rectangle(0, 0, size.Width, size.Height);
                }
            }
        }


        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MonitorCount"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the number of display monitors on the desktop.
        ///    </para>
        /// </devdoc>
        public static int MonitorCount {
            get {
                if (MultiMonitorSupport) {
                    return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CMONITORS);
                }
                else {
                    return 1;
                }
            }
        }


        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MonitorsSameDisplayFormat"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether all the display monitors have the
        ///       same color format.
        ///    </para>
        /// </devdoc>
        public static bool MonitorsSameDisplayFormat {
            get {
                if (MultiMonitorSupport) {
                    return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_SAMEDISPLAYFORMAT) != 0;
                }
                else {
                    return true;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        // Misc
        //


        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.ComputerName"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the computer name of the current system.
        ///    </para>
        /// </devdoc>
        public static string ComputerName {
            get {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "SensitiveSystemInformation Demanded");
                IntSecurity.SensitiveSystemInformation.Demand();

                StringBuilder sb = new StringBuilder(256);
                UnsafeNativeMethods.GetComputerName(sb, new int[] {sb.Capacity});
                return sb.ToString();
            }
        }


        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.UserDomainName"]/*' />
        /// <devdoc>
        ///    Gets the user's domain name.
        /// </devdoc>
        public static string UserDomainName {
            get {
                return Environment.UserDomainName;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.UserInteractive"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the current process is running in user
        ///       interactive mode.
        ///    </para>
        /// </devdoc>
        public static bool UserInteractive {
            get {
                if (Environment.OSVersion.Platform == System.PlatformID.Win32NT) {
                    IntPtr hwinsta = IntPtr.Zero;

                    hwinsta = UnsafeNativeMethods.GetProcessWindowStation();
                    if (hwinsta != IntPtr.Zero && processWinStation != hwinsta) {
                        isUserInteractive = true;

                        int lengthNeeded = 0;
                        NativeMethods.USEROBJECTFLAGS flags = new NativeMethods.USEROBJECTFLAGS();

                        if (UnsafeNativeMethods.GetUserObjectInformation(new HandleRef(null, hwinsta), NativeMethods.UOI_FLAGS, flags, Marshal.SizeOf(flags), ref lengthNeeded)) {
                            if ((flags.dwFlags & NativeMethods.WSF_VISIBLE) == 0) {
                                isUserInteractive = false;
                            }
                        }
                        processWinStation = hwinsta;
                    }
                }
                else {
                    isUserInteractive = true;
                }
                return isUserInteractive;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.UserName"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the user name for the current thread, that is, the name of the
        ///       user currently logged onto the system.
        ///    </para>
        /// </devdoc>
        public static string UserName {
            get {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "SensitiveSystemInformation Demanded");
                IntSecurity.SensitiveSystemInformation.Demand();

                StringBuilder sb = new StringBuilder(256);
                UnsafeNativeMethods.GetUserName(sb, new int[] {sb.Capacity});
                return sb.ToString();
            }
        }
        
        private static void EnsureSystemEvents() {
            if (!systemEventsAttached) {
                SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(SystemInformation.OnUserPreferenceChanged);
                systemEventsAttached = true;
            }
        }
        
        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref) {
            systemEventsDirty = true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // NEW ADDITIONS FOR WHIDBEY                                                                            //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsDropShadowEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the drop shadow effect in enabled. Defaults to false 
        ///       downlevel.
        ///    </para>
        /// </devdoc>
        public static bool IsDropShadowEnabled {
            get {
                if (OSFeature.Feature.OnXp) {
                    int data = 0;
                    UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETDROPSHADOW, 0, ref data, 0);
                    return data != 0;
                }
                return false;
           }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsFlatMenuEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the native user menus have a flat menu appearance. Defaults to false 
        ///       downlevel.
        ///    </para>
        /// </devdoc>
        public static bool IsFlatMenuEnabled {
            get {
                if (OSFeature.Feature.OnXp) {
                    int data = 0;
                    UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETFLATMENU, 0, ref data, 0);
                    return data != 0;
                }
                return false;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsFontSmoothingEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the Font Smoothing OSFeature.Feature is enabled. 
        ///    </para>
        /// </devdoc>
        public static bool IsFontSmoothingEnabled {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETFONTSMOOTHING, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.FontSmoothingContrast"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Returns a contrast value that is ClearType smoothing.
        ///    </para>
        /// </devdoc>
        public static int FontSmoothingContrast {
            get {
                if (OSFeature.Feature.OnXp) {
                    int data = 0;
                    UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETFONTSMOOTHINGCONTRAST, 0, ref data, 0);
                    return data;
                }
                else {
                    throw new NotSupportedException(SR.SystemInformationFeatureNotSupported);
                }
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.FontSmoothingType"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Returns a type of Font smoothing.
        ///    </para>
        /// </devdoc>
        public static int FontSmoothingType {
            get {
                if (OSFeature.Feature.OnXp) {
                    int data = 0;
                    UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETFONTSMOOTHINGTYPE, 0, ref data, 0);
                    return data;
                }
                else {
                    throw new NotSupportedException(SR.SystemInformationFeatureNotSupported);
                }
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IconHorizontalSpacing"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves the width in pixels of an icon cell.
        ///    </para>
        /// </devdoc>
        public static int IconHorizontalSpacing {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_ICONHORIZONTALSPACING, 0, ref data, 0);
                return data;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IconVerticalSpacing"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves the height in pixels of an icon cell.
        ///    </para>
        /// </devdoc>
        public static int IconVerticalSpacing {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_ICONVERTICALSPACING, 0, ref data, 0);
                return data;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsIconTitleWrappingEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Gets a value indicating whether the Icon title wrapping is enabled.
        ///    </para>
        /// </devdoc>
        public static bool IsIconTitleWrappingEnabled {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETICONTITLEWRAP, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MenuAccessKeysUnderlined"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Gets a value indicating whether the menu access keys are always underlined.
        ///    </para>
        /// </devdoc>
        public static bool MenuAccessKeysUnderlined {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETKEYBOARDCUES, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.KeyboardDelay"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves the Keyboard repeat delay setting, which is a value in the 
        ///       range from 0 through 3. The Actual Delay Associated with each value may vary depending on the 
        ///       hardware.
        ///    </para>
        /// </devdoc>
        public static int KeyboardDelay {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETKEYBOARDDELAY, 0, ref data, 0);
                return data;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsKeyboardPreferred"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Gets a value indicating whether the user relies on Keyboard instead of mouse and wants 
        ///      applications to display keyboard interfaces that would be otherwise hidden.
        ///    </para>
        /// </devdoc>
        public static bool IsKeyboardPreferred {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETKEYBOARDPREF, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.KeyboardSpeed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves the Keyboard repeat speed setting, which is a value in the 
        ///       range from 0 through 31. The actual rate may vary depending on the 
        ///       hardware.
        ///    </para>
        /// </devdoc>
        public static int KeyboardSpeed {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETKEYBOARDSPEED, 0, ref data, 0);
                return data;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MouseHoverSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the Size in pixels of the rectangle within which the mouse pointer has to stay.
        ///    </para>
        /// </devdoc>
        public static Size MouseHoverSize {
            get {
                int height = 0;
                int width = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMOUSEHOVERHEIGHT, 0, ref height, 0);
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMOUSEHOVERWIDTH, 0, ref width, 0);
                return new Size(width, height);
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MouseHoverTime"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the time, in milliseconds, that the mouse pointer has to stay in the hover rectangle.
        ///    </para>
        /// </devdoc>
        public static int MouseHoverTime {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMOUSEHOVERTIME, 0, ref data, 0);
                return data;
            }

        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MouseSpeed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the current mouse speed.
        ///    </para>
        /// </devdoc>
        public static int MouseSpeed {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMOUSESPEED, 0, ref data, 0);
                return data;
            }

        }


        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsSnapToDefaultEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Determines whether the snap-to-default-button feature is enabled.
        ///    </para>
        /// </devdoc>
        public static bool IsSnapToDefaultEnabled {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETSNAPTODEFBUTTON, 0, ref data, 0);
                return data != 0;
            }
        }


        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.PopupMenuAlignment"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Determines whether the Popup Menus are left Aligned or Right Aligned.
        ///    </para>
        /// </devdoc>
        public static LeftRightAlignment PopupMenuAlignment {
            get {
                bool data = false;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMENUDROPALIGNMENT, 0, ref data, 0);
                if (data) {
                    return LeftRightAlignment.Left;
                }
                else {
                    return LeftRightAlignment.Right;
                }

            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsMenuFadeEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Determines whether the maenu fade animation feature is enabled. Defaults to false 
        ///       downlevel.
        ///    </para>
        /// </devdoc>
        public static bool IsMenuFadeEnabled {
            get {
                if (OSFeature.Feature.OnXp || OSFeature.Feature.OnWin2k) {
                    int data = 0;
                    UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMENUFADE, 0, ref data, 0);
                    return data != 0;
                }
                return false;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MenuShowDelay"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Indicates the time, in milliseconds, that the system waits before displaying a shortcut menu.
        ///    </para>
        /// </devdoc>
        public static int MenuShowDelay {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMENUSHOWDELAY, 0, ref data, 0);
                return data;
            }

        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsComboBoxAnimationEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Indicates whether the slide open effect for combo boxes is enabled.
        ///    </para>
        /// </devdoc>
        public static bool IsComboBoxAnimationEnabled {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETCOMBOBOXANIMATION, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsTitleBarGradientEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Indicates whether the gradient effect for windows title bars is enabled.
        ///    </para>
        /// </devdoc>
        public static bool IsTitleBarGradientEnabled {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETGRADIENTCAPTIONS, 0, ref data, 0);
                return data != 0;
            }
        }


        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsHotTrackingEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Indicates whether the hot tracking of user interface elements is enabled.
        ///    </para>
        /// </devdoc>
        public static bool IsHotTrackingEnabled {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETHOTTRACKING, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsListBoxSmoothScrollingEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Indicates whether the smooth scrolling effect for listbox is enabled.
        ///    </para>
        /// </devdoc>
        public static bool IsListBoxSmoothScrollingEnabled {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETLISTBOXSMOOTHSCROLLING, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsMenuAnimationEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Indicates whether the menu animation feature is enabled.
        ///    </para>
        /// </devdoc>
        public static bool IsMenuAnimationEnabled {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETMENUANIMATION, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsSelectionFadeEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Indicates whether the selection fade effect is enabled. Defaults to false 
        ///       downlevel.
        ///    </para>
        /// </devdoc>
        public static bool IsSelectionFadeEnabled {
            get {
                if (OSFeature.Feature.OnXp || OSFeature.Feature.OnWin2k) {
                    int data = 0;
                    UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETSELECTIONFADE, 0, ref data, 0);
                    return data != 0;
                }
                return false;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsToolTipAnimationEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Indicates whether the tool tip animation is enabled. Defaults to false 
        ///      downlevel.
        ///    </para>
        /// </devdoc>
        public static bool IsToolTipAnimationEnabled {
            get {
                if (OSFeature.Feature.OnXp || OSFeature.Feature.OnWin2k) {
                    int data = 0;
                    UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETTOOLTIPANIMATION, 0, ref data, 0);
                    return data != 0;
                }
                return false;

            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.UIEffectsEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Indicates whether all the UI Effects are enabled. Defaults to false 
        ///      downlevel.
        ///    </para>
        /// </devdoc>
        public static bool UIEffectsEnabled {
            get {
                if (OSFeature.Feature.OnXp || OSFeature.Feature.OnWin2k) {
                    int data = 0;
                    UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETUIEFFECTS, 0, ref data, 0);
                    return data != 0;
                }
                return false;
            }
        }


        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsActiveWindowTrackingEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Indicates whether the active windows tracking (activating the window the mouse in on) is ON or OFF.
        ///    </para>
        /// </devdoc>
        public static bool IsActiveWindowTrackingEnabled {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETACTIVEWINDOWTRACKING, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.ActiveWindowTrackingDelay"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves the active window tracking delay, in milliseconds.
        ///    </para>
        /// </devdoc>
        public static int ActiveWindowTrackingDelay {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETACTIVEWNDTRKTIMEOUT, 0, ref data, 0);
                return data;
            }

        }


        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.IsMinimizeRestoreAnimationEnabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Indicates whether the active windows tracking (activating the window the mouse in on) is ON or OFF.
        ///    </para>
        /// </devdoc>
        public static bool IsMinimizeRestoreAnimationEnabled {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETANIMATION, 0, ref data, 0);
                return data != 0;
            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.BorderMultiplierFactor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves the border multiplier factor that determines the width of a windo's sizing border.
        ///    </para>
        /// </devdoc>
        public static int BorderMultiplierFactor {
            get {
                int data = 0;
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETBORDER, 0, ref data, 0);
                return data;
            }

        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.CaretBlinkTime"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Indicates the caret blink time.
        ///    </para>
        /// </devdoc>
        public static int CaretBlinkTime {
            get {
                return unchecked((int)SafeNativeMethods.GetCaretBlinkTime());
            }

        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.CaretWidth"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Indicates the caret width in edit controls.
        ///    </para>
        /// </devdoc>
        public static int CaretWidth {
            get {
                if (OSFeature.Feature.OnXp || OSFeature.Feature.OnWin2k) {
                    int data = 0;
                    UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETCARETWIDTH, 0, ref data, 0);
                    return data;
                }
                else {
                    throw new NotSupportedException(SR.SystemInformationFeatureNotSupported);
                }
            }

        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MouseWheelScrollDelta"]/*' />
        /// <devdoc>
        ///    <para>
        ///       None.
        ///    </para>
        /// </devdoc>
        public static int MouseWheelScrollDelta {
            get {
                return NativeMethods.WHEEL_DELTA;
            }

        }


        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.VerticalFocusThickness"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The width of the left and right edges of the focus rectangle.
        ///    </para>
        /// </devdoc>
        public static int VerticalFocusThickness {
            get {
                if (OSFeature.Feature.OnXp) {
                    return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYFOCUSBORDER);
                }
                else {
                    throw new NotSupportedException(SR.SystemInformationFeatureNotSupported);
                }
            }

        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.HorizontalFocusThickness"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The width of the top and bottom edges of the focus rectangle.
        ///    </para>
        /// </devdoc>
        public static int HorizontalFocusThickness {
            get {
                if (OSFeature.Feature.OnXp) {
                    return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXFOCUSBORDER);
                }
                else {
                    throw new NotSupportedException(SR.SystemInformationFeatureNotSupported);
                }
            }

        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.VerticalResizeBorderThickness"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The height of the vertical sizing border around the perimeter of the window that can be resized.
        ///    </para>
        /// </devdoc>
        public static int VerticalResizeBorderThickness {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CYSIZEFRAME);
            }

        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.HorizontalResizeBorderThickness"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The width of the horizontal sizing border around the perimeter of the window that can be resized.
        ///    </para>
        /// </devdoc>
        public static int HorizontalResizeBorderThickness {
            get {
                return UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_CXSIZEFRAME);
            }

        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.ScreenOrientation"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The orientation of the screen in degrees.
        ///    </para>
        /// </devdoc>
        public static ScreenOrientation ScreenOrientation {
            get {
                ScreenOrientation so = ScreenOrientation.Angle0;
                NativeMethods.DEVMODE dm = new NativeMethods.DEVMODE();
                dm.dmSize = (short) Marshal.SizeOf(typeof(NativeMethods.DEVMODE));
                dm.dmDriverExtra = 0;
                try {
                    SafeNativeMethods.EnumDisplaySettings(null, -1 /*ENUM_CURRENT_SETTINGS*/, ref dm);
                    if ( (dm.dmFields & NativeMethods.DM_DISPLAYORIENTATION) > 0) {
                        so = dm.dmDisplayOrientation;
                    }
                }
                catch {
                    // empty catch, we'll just return the default if the call to EnumDisplaySettings fails.
                }

                return so;
            }
        }
        
        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.SizingBorderWidth"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Specifies the thikness, in pixels, of the Sizing Border.
        ///    </para>
        /// </devdoc>
        public static int SizingBorderWidth {
            get {
                //we can get the system's menu font through the NONCLIENTMETRICS structure via SystemParametersInfo
                //
                NativeMethods.NONCLIENTMETRICS data = new NativeMethods.NONCLIENTMETRICS();
                bool result = UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, data.cbSize, data, 0);
                if (result && data.iBorderWidth > 0) {
                    return data.iBorderWidth;
                }
                else {
                    return 0;
                }
            }
        }

        /*
        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.VerticalScrollBarWidth"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Specified the width, in pixels, of a standard vertical scrollbar.
        ///    </para>
        /// </devdoc>
        public static int VerticalScrollBarWidth {
            get {
                
                //we can get the system's menu font through the NONCLIENTMETRICS structure via SystemParametersInfo
                //
                NativeMethods.NONCLIENTMETRICS data = new NativeMethods.NONCLIENTMETRICS();
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, data.cbSize, data, 0);
                if (result && data.iScrollWidth > 0) {
                    return data.iScrollWidth;
                }
                else {
                    return 0;
                }
                

            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.HorizontalScrollBarWidth"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Specified the height, in pixels, of a standard horizontal scrollbar.
        ///    </para>
        /// </devdoc>
        public static int HorizontalScrollBarWidth {
            get {
                
                //we can get the system's menu font through the NONCLIENTMETRICS structure via SystemParametersInfo
                //
                NativeMethods.NONCLIENTMETRICS data = new NativeMethods.NONCLIENTMETRICS();
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, data.cbSize, data, 0);
                if (result && data.iScrollHeight > 0) {
                    return data.iScrollHeight;
                }
                else {
                    return 0;
                }
            }
        }
        

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.CaptionButtonSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Specified the Size, in pixels, of the caption buttons.
        ///    </para>
        /// </devdoc>
        public static Size CaptionButtonSize {
            get {
                
                //we can get the system's menu font through the NONCLIENTMETRICS structure via SystemParametersInfo
                //
                NativeMethods.NONCLIENTMETRICS data = new NativeMethods.NONCLIENTMETRICS();
                UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, data.cbSize, data, 0);
                if (result && data.iCaptionHeight > 0 && data.iCaptionWidth > 0) {
                    return new Size(data.iCaptionWidth, data.iCaptionHeight);
                }
                else {
                    return return new Size(0, 0);
                }
                

            }
        }
        */
        
         /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.SmallCaptionButtonSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Specified the Size, in pixels, of the small caption buttons.
        ///    </para>
        /// </devdoc>
        public static Size SmallCaptionButtonSize {
            get {
                
                //we can get the system's menu font through the NONCLIENTMETRICS structure via SystemParametersInfo
                //
                NativeMethods.NONCLIENTMETRICS data = new NativeMethods.NONCLIENTMETRICS();
                bool result = UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, data.cbSize, data, 0);
                if (result && data.iSmCaptionHeight > 0 && data.iSmCaptionWidth > 0) {
                    return new Size(data.iSmCaptionWidth, data.iSmCaptionHeight);
                }
                else {
                    return Size.Empty;
                }
                

            }
        }

        /// <include file='doc\SystemInformation.uex' path='docs/doc[@for="SystemInformation.MenuBarButtonSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Specified the Size, in pixels, of the menu bar buttons.
        ///    </para>
        /// </devdoc>
        public static Size MenuBarButtonSize {
            get {
                
                //we can get the system's menu font through the NONCLIENTMETRICS structure via SystemParametersInfo
                //
                NativeMethods.NONCLIENTMETRICS data = new NativeMethods.NONCLIENTMETRICS();
                bool result = UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, data.cbSize, data, 0);
                if (result && data.iMenuHeight > 0 && data.iMenuWidth > 0) {
                    return new Size(data.iMenuWidth, data.iMenuHeight);
                }
                else {
                    return Size.Empty;
                }
                

            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // End ADDITIONS FOR WHIDBEY                                                                            //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <devdoc>
        ///     Checks whether the current Winforms app is running on a secure desktop under a terminal
        ///     server session.  This is the case when the TS session has been locked.
        ///     This method is useful when calling into GDI+ Graphics methods that modify the object's
        ///     state, these methods fail under a locked terminal session.
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        internal static bool InLockedTerminalSession() {
            bool retVal = false;

            if (SystemInformation.TerminalServerSession) {
                // Let's try to open the input desktop, it it fails with access denied assume
                // the app is running on a secure desktop.
                IntPtr hDsk = SafeNativeMethods.OpenInputDesktop(0, false, NativeMethods.DESKTOP_SWITCHDESKTOP);

                if (hDsk == IntPtr.Zero) {
                    int error = Marshal.GetLastWin32Error();
                    retVal = error == NativeMethods.ERROR_ACCESS_DENIED;
                }

                if (hDsk != IntPtr.Zero) {
                    SafeNativeMethods.CloseDesktop(hDsk);                
                }
            }

            return retVal;
        }
   }
}

