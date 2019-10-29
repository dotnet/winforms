// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using static Interop;
using static Interop.Ole32;

namespace System.Windows.Forms
{
    internal static class SafeNativeMethods
    {
        [DllImport(ExternDll.Gdi32)]
        public static extern int GetSystemPaletteEntries(IntPtr hdc, int iStartIndex, int nEntries, byte[] lppe);

        [DllImport(ExternDll.Gdi32)]
        public static extern int GetDIBits(IntPtr hdc, IntPtr hbm, int uStartScan, int cScanLines, byte[] lpvBits, ref NativeMethods.BITMAPINFO_FLAT bmi, int uUsage);

        [DllImport(ExternDll.Gdi32)]
        public static extern int StretchDIBits(IntPtr hdc, int XDest, int YDest, int nDestWidth, int nDestHeight, int XSrc, int YSrc, int nSrcWidth, int nSrcHeight, byte[] lpBits, ref NativeMethods.BITMAPINFO_FLAT lpBitsInfo, int iUsage, int dwRop);

        [DllImport(ExternDll.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool ChooseFont([In, Out] NativeMethods.CHOOSEFONT cf);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetBitmapBits(HandleRef hbmp, int cbBuffer, byte[] lpvBits);

        [DllImport(ExternDll.Comdlg32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int CommDlgExtendedError();

        [DllImport(ExternDll.Hhctrl, CharSet = CharSet.Auto)]
        public static extern int HtmlHelp(HandleRef hwndCaller, [MarshalAs(UnmanagedType.LPTStr)]string pszFile, int uCommand, int dwData);

        [DllImport(ExternDll.Hhctrl, CharSet = CharSet.Auto)]
        public static extern int HtmlHelp(HandleRef hwndCaller, [MarshalAs(UnmanagedType.LPTStr)]string pszFile, int uCommand, string dwData);

        [DllImport(ExternDll.Hhctrl, CharSet = CharSet.Auto)]
        public static extern int HtmlHelp(HandleRef hwndCaller, [MarshalAs(UnmanagedType.LPTStr)]string pszFile, int uCommand, [MarshalAs(UnmanagedType.LPStruct)]NativeMethods.HH_POPUP dwData);

        [DllImport(ExternDll.Hhctrl, CharSet = CharSet.Auto)]
        public static extern int HtmlHelp(HandleRef hwndCaller, [MarshalAs(UnmanagedType.LPTStr)]string pszFile, int uCommand, [MarshalAs(UnmanagedType.LPStruct)]NativeMethods.HH_FTS_QUERY dwData);

        [DllImport(ExternDll.Hhctrl, CharSet = CharSet.Auto)]
        public static extern int HtmlHelp(HandleRef hwndCaller, [MarshalAs(UnmanagedType.LPTStr)]string pszFile, int uCommand, [MarshalAs(UnmanagedType.LPStruct)]NativeMethods.HH_AKLINK dwData);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool PatBlt(HandleRef hdc, int left, int top, int width, int height, int rop);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool TrackPopupMenuEx(HandleRef hmenu, int fuFlags, int x, int y, HandleRef hwnd, NativeMethods.TPMPARAMS tpm);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetKeyboardLayout(int dwLayout);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr ActivateKeyboardLayout(HandleRef hkl, int uFlags);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetKeyboardLayoutList(int size, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] hkls);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out]NativeMethods.MONITORINFOEX info);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool EnumDisplayMonitors(HandleRef hdc, NativeMethods.COMRECT rcClip, NativeMethods.MonitorEnumProc lpfnEnum, IntPtr dwData);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr /*HPALETTE*/ CreateHalftonePalette(HandleRef hdc);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr CreateDIBSection(HandleRef hdc, HandleRef pbmi, int iUsage, byte[] ppvBits, IntPtr hSection, int dwOffset);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr /*HBITMAP*/ CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitsPerPixel, IntPtr lpvBits);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr /*HBITMAP*/ CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitsPerPixel, short[] lpvBits);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr /*HBITMAP*/ CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitsPerPixel, byte[] lpvBits);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool AdjustWindowRectEx(ref Interop.RECT lpRect, int dwStyle, bool bMenu, int dwExStyle);

        // This API is available only starting Windows 10 RS1
        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool AdjustWindowRectExForDpi(ref RECT lpRect, int dwStyle, bool bMenu, int dwExStyle, uint dpi);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool EnableWindow(HandleRef hWnd, bool enable);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool GetClientRect(HandleRef hWnd, ref RECT rect);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool ValidateRect(HandleRef hWnd, ref RECT rect);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool ValidateRect(IntPtr hwnd, IntPtr prect);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern bool LPtoDP(HandleRef hDC, ref RECT lpRect, int nCount);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool IsWindowEnabled(HandleRef hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(HandleRef hWnd);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);

        internal delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool InvalidateRgn(HandleRef hWnd, HandleRef hrgn, bool erase);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool UpdateWindow(HandleRef hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool MessageBeep(int type);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool DrawMenuBar(HandleRef hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public extern static bool IsChild(HandleRef parent, HandleRef child);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetTimer(HandleRef hWnd, int nIDEvent, int uElapse, IntPtr lpTimerFunc);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool KillTimer(HandleRef hwnd, int idEvent);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern int MessageBox(HandleRef hWnd, string text, string caption, int type);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool ScrollWindow(HandleRef hWnd, int nXAmount, int nYAmount, ref RECT rectScrollRegion, ref RECT rectClip);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool IsWindowUnicode(HandleRef hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool DrawEdge(HandleRef hDC, ref RECT rect, int edge, int flags);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool DrawFrameControl(HandleRef hDC, ref RECT rect, int type, int state);

        // Theming/Visual Styles
        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeBackgroundContentRect(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, [In] NativeMethods.COMRECT pBoundingRect, [Out] NativeMethods.COMRECT pContentRect);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeBackgroundExtent(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, [In] NativeMethods.COMRECT pContentRect, [Out] NativeMethods.COMRECT pExtentRect);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeBackgroundRegion(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, [In] NativeMethods.COMRECT pRect, ref IntPtr pRegion);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeBool(HandleRef hTheme, int iPartId, int iStateId, int iPropId, ref bool pfVal);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeColor(HandleRef hTheme, int iPartId, int iStateId, int iPropId, ref int pColor);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeEnumValue(HandleRef hTheme, int iPartId, int iStateId, int iPropId, ref int piVal);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeFilename(HandleRef hTheme, int iPartId, int iStateId, int iPropId, StringBuilder pszThemeFilename, int cchMaxBuffChars);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeInt(HandleRef hTheme, int iPartId, int iStateId, int iPropId, ref int piVal);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemePartSize(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, [In] NativeMethods.COMRECT prc, VisualStyles.ThemeSizeType eSize, out Size psz);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemePosition(HandleRef hTheme, int iPartId, int iStateId, int iPropId, out Point pPoint);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeMargins(HandleRef hTheme, HandleRef hDC, int iPartId, int iStateId, int iPropId, NativeMethods.COMRECT prc, ref NativeMethods.MARGINS margins);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeString(HandleRef hTheme, int iPartId, int iStateId, int iPropId, StringBuilder pszBuff, int cchMaxBuffChars);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeTextExtent(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, [MarshalAs(UnmanagedType.LPWStr)] string pszText, int iCharCount, int dwTextFlags, [In] NativeMethods.COMRECT pBoundingRect, [Out] NativeMethods.COMRECT pExtentRect);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeTextMetrics(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, ref VisualStyles.TextMetrics ptm);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int HitTestThemeBackground(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, int dwOptions, [In] NativeMethods.COMRECT pRect, HandleRef hrgn, Point ptTest, ref ushort pwHitTestCode);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern bool IsThemeBackgroundPartiallyTransparent(HandleRef hTheme, int iPartId, int iStateId);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern bool GetThemeSysBool(HandleRef hTheme, int iBoolId);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeSysInt(HandleRef hTheme, int iIntId, ref int piValue);

        public static class VisualStyleSystemProperty
        {
            public const int SupportsFlatMenus = 1001;
            public const int MinimumColorDepth = 1301;
        }

        // for Windows Windows 7 to Windows 8.
        [DllImport(ExternDll.User32, SetLastError = true)]
        public static extern bool IsProcessDPIAware();

        // for Windows Windows 7 to Windows 8.
        [DllImport(ExternDll.User32, SetLastError = true)]
        public static extern bool SetProcessDPIAware();

        // for Windows 8.1 and above
        [DllImport(ExternDll.ShCore, SetLastError = true)]
        public static extern int SetProcessDpiAwareness(NativeMethods.PROCESS_DPI_AWARENESS awareness);

        // for Windows 8.1 and above
        [DllImport(ExternDll.ShCore, SetLastError = true)]
        public static extern int GetProcessDpiAwareness(IntPtr processHandle, out NativeMethods.PROCESS_DPI_AWARENESS awareness);

        // for Windows 10 version RS2 and above
        [DllImport(ExternDll.User32, SetLastError = true)]
        public static extern bool IsValidDpiAwarenessContext(int dpiFlag);

        // for Windows 10 version RS2 and above
        [DllImport(ExternDll.User32, SetLastError = true)]
        public static extern bool SetProcessDpiAwarenessContext(int dpiFlag);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public extern static int SetWindowTheme(IntPtr hWnd, string subAppName, string subIdList);

        // Color conversion
        public static int RGBToCOLORREF(int rgbValue)
        {
            // clear the A value, swap R & B values
            int bValue = (rgbValue & 0xFF) << 16;

            rgbValue &= 0xFFFF00;
            rgbValue |= ((rgbValue >> 16) & 0xFF);
            rgbValue &= 0x00FFFF;
            rgbValue |= bValue;
            return rgbValue;
        }

        public static Color ColorFromCOLORREF(int colorref)
        {
            int r = colorref & 0xFF;
            int g = (colorref >> 8) & 0xFF;
            int b = (colorref >> 16) & 0xFF;
            return Color.FromArgb(r, g, b);
        }

        public static int ColorToCOLORREF(Color color)
        {
            return (int)color.R | ((int)color.G << 8) | ((int)color.B << 16);
        }
    }
}

