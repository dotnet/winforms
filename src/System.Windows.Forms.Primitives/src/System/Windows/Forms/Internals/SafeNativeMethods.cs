// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using static Interop;
using static Interop.UxTheme;

namespace System.Windows.Forms
{
    internal static class SafeNativeMethods
    {
        [DllImport(ExternDll.Gdi32)]
        public static extern int GetSystemPaletteEntries(IntPtr hdc, int iStartIndex, int nEntries, byte[] lppe);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetBitmapBits(HandleRef hbmp, int cbBuffer, byte[] lpvBits);

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

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr /*HBITMAP*/ CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitsPerPixel, IntPtr lpvBits);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr /*HBITMAP*/ CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitsPerPixel, short[] lpvBits);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr /*HBITMAP*/ CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitsPerPixel, byte[] lpvBits);

        // Theming/Visual Styles
        [DllImport(Libraries.UxTheme, CharSet = CharSet.Auto)]
        public static extern HRESULT GetThemeBackgroundContentRect(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, [In] NativeMethods.COMRECT pBoundingRect, [Out] NativeMethods.COMRECT pContentRect);

        [DllImport(Libraries.UxTheme, CharSet = CharSet.Auto)]
        public static extern HRESULT GetThemeBackgroundExtent(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, [In] NativeMethods.COMRECT pContentRect, [Out] NativeMethods.COMRECT pExtentRect);

        [DllImport(Libraries.UxTheme, CharSet = CharSet.Auto)]
        public static extern HRESULT GetThemeBackgroundRegion(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, [In] NativeMethods.COMRECT pRect, ref IntPtr pRegion);

        [DllImport(Libraries.UxTheme, CharSet = CharSet.Auto)]
        public static extern HRESULT GetThemeFilename(HandleRef hTheme, int iPartId, int iStateId, int iPropId, StringBuilder pszThemeFilename, int cchMaxBuffChars);

        [DllImport(Libraries.UxTheme, CharSet = CharSet.Auto)]
        public static extern HRESULT GetThemePartSize(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, [In] NativeMethods.COMRECT prc, VisualStyles.ThemeSizeType eSize, out Size psz);

        [DllImport(Libraries.UxTheme, CharSet = CharSet.Auto)]
        public static extern HRESULT GetThemePosition(HandleRef hTheme, int iPartId, int iStateId, int iPropId, out Point pPoint);

        [DllImport(Libraries.UxTheme, CharSet = CharSet.Auto)]
        public static extern HRESULT GetThemeMargins(HandleRef hTheme, HandleRef hDC, int iPartId, int iStateId, int iPropId, NativeMethods.COMRECT prc, ref MARGINS margins);

        [DllImport(Libraries.UxTheme, CharSet = CharSet.Auto)]
        public static extern HRESULT GetThemeString(HandleRef hTheme, int iPartId, int iStateId, int iPropId, StringBuilder pszBuff, int cchMaxBuffChars);

        [DllImport(Libraries.UxTheme, CharSet = CharSet.Auto)]
        public static extern HRESULT GetThemeTextExtent(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, [MarshalAs(UnmanagedType.LPWStr)] string pszText, int iCharCount, int dwTextFlags, [In] NativeMethods.COMRECT pBoundingRect, [Out] NativeMethods.COMRECT pExtentRect);

        [DllImport(Libraries.UxTheme, CharSet = CharSet.Auto)]
        public static extern HRESULT GetThemeTextMetrics(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, ref VisualStyles.TextMetrics ptm);

        [DllImport(Libraries.UxTheme, CharSet = CharSet.Auto)]
        public static extern HRESULT HitTestThemeBackground(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, int dwOptions, [In] NativeMethods.COMRECT pRect, HandleRef hrgn, Point ptTest, ref ushort pwHitTestCode);
    }
}
