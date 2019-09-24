﻿// Licensed to the .NET Foundation under one or more agreements.
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
        [DllImport(ExternDll.User32)]
        public static extern int OemKeyScan(short wAsciiVal);

        [DllImport(ExternDll.Gdi32)]
        public static extern int GetSystemPaletteEntries(IntPtr hdc, int iStartIndex, int nEntries, byte[] lppe);

        [DllImport(ExternDll.Gdi32)]
        public static extern int GetDIBits(IntPtr hdc, IntPtr hbm, int uStartScan, int cScanLines, byte[] lpvBits, ref NativeMethods.BITMAPINFO_FLAT bmi, int uUsage);

        [DllImport(ExternDll.Gdi32)]
        public static extern int StretchDIBits(HandleRef hdc, int XDest, int YDest, int nDestWidth, int nDestHeight, int XSrc, int YSrc, int nSrcWidth, int nSrcHeight, byte[] lpBits, ref NativeMethods.BITMAPINFO_FLAT lpBitsInfo, int iUsage, int dwRop);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr CreateCompatibleBitmap(HandleRef hDC, int width, int height);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool GetScrollInfo(HandleRef hWnd, int fnBar, [In, Out] NativeMethods.SCROLLINFO si);

        [DllImport(ExternDll.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool ChooseFont([In, Out] NativeMethods.CHOOSEFONT cf);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetBitmapBits(HandleRef hbmp, int cbBuffer, byte[] lpvBits);

        [DllImport(ExternDll.Comdlg32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int CommDlgExtendedError();

        [DllImport(ExternDll.Oleaut32, PreserveSig = false)]
        public static extern void OleCreatePropertyFrame(HandleRef hwndOwner, int x, int y, [MarshalAs(UnmanagedType.LPWStr)]string caption, int objects, [MarshalAs(UnmanagedType.Interface)] ref object pobjs, int pages, HandleRef pClsid, int locale, int reserved1, IntPtr reserved2);

        [DllImport(ExternDll.Oleaut32, PreserveSig = false)]
        public static extern void OleCreatePropertyFrame(HandleRef hwndOwner, int x, int y, [MarshalAs(UnmanagedType.LPWStr)]string caption, int objects, [MarshalAs(UnmanagedType.Interface)] ref object pobjs, int pages, Guid[] pClsid, int locale, int reserved1, IntPtr reserved2);

        [DllImport(ExternDll.Oleaut32, PreserveSig = false)]
        public static extern void OleCreatePropertyFrame(HandleRef hwndOwner, int x, int y, [MarshalAs(UnmanagedType.LPWStr)]string caption, int objects, HandleRef lplpobjs, int pages, HandleRef pClsid, int locale, int reserved1, IntPtr reserved2);

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
        public static extern bool LineTo(HandleRef hdc, int x, int y);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static unsafe extern bool MoveToEx(HandleRef hdc, int x, int y, Point *lppt);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool Rectangle(
                                           HandleRef hdc, int left, int top, int right, int bottom);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool PatBlt(HandleRef hdc, int left, int top, int width, int height, int rop);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetMessagePos();

        [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int RegisterClipboardFormat(string format);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern int GetClipboardFormatName(int format, StringBuilder lpString, int cchMax);

        [DllImport(ExternDll.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool ChooseColor([In, Out] NativeMethods.CHOOSECOLOR cc);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static unsafe extern bool SetWindowExtEx(IntPtr hDC, int x, int y, Size *size);

        [DllImport(ExternDll.Comctl32)]
        public static extern void InitCommonControls();

        [DllImport(ExternDll.Comctl32)]
        public static extern bool InitCommonControlsEx(NativeMethods.INITCOMMONCONTROLSEX icc);

#if DEBUG
        private static readonly ArrayList validImageListHandles = ArrayList.Synchronized(new ArrayList());

        [DllImport(ExternDll.Comctl32, EntryPoint = "ImageList_Create")]
        private static extern IntPtr IntImageList_Create(int cx, int cy, int flags, int cInitial, int cGrow);

        public static IntPtr ImageList_Create(int cx, int cy, int flags, int cInitial, int cGrow)
        {
            IntPtr newHandle = IntImageList_Create(cx, cy, flags, cInitial, cGrow);
            validImageListHandles.Add(newHandle);
            return newHandle;
        }
#else
        [DllImport(ExternDll.Comctl32)]
        public static extern IntPtr ImageList_Create(int cx, int cy, int flags, int cInitial, int cGrow);
#endif

#if DEBUG
        [DllImport(ExternDll.Comctl32, EntryPoint = "ImageList_Destroy")]
        private static extern bool IntImageList_Destroy(HandleRef himl);

        public static bool ImageList_Destroy(HandleRef himl)
        {
            System.Diagnostics.Debug.Assert(validImageListHandles.Contains(himl.Handle), "Invalid ImageList handle");
            validImageListHandles.Remove(himl.Handle);
            return IntImageList_Destroy(himl);
        }
#else
        [DllImport(ExternDll.Comctl32)]
        public static extern bool ImageList_Destroy(HandleRef himl);
#endif

        // unfortunately, the neat wrapper to Assert for DEBUG assumes that this was created by
        // our version of ImageList_Create, which is not always the case for the TreeView's internal
        // native state image list. Use separate EntryPoint thunk to skip this check:
        [DllImport(ExternDll.Comctl32, EntryPoint = "ImageList_Destroy")]
        public static extern bool ImageList_Destroy_Native(HandleRef himl);

        [DllImport(ExternDll.Comctl32)]
        public static extern int ImageList_GetImageCount(HandleRef himl);

        [DllImport(ExternDll.Comctl32)]
        public static extern int ImageList_Add(HandleRef himl, IntPtr hbmImage, IntPtr hbmMask);

        [DllImport(ExternDll.Comctl32)]
        public static extern int ImageList_ReplaceIcon(HandleRef himl, int index, HandleRef hicon);

        [DllImport(ExternDll.Comctl32)]
        public static extern int ImageList_SetBkColor(HandleRef himl, int clrBk);

        [DllImport(ExternDll.Comctl32)]
        public static extern bool ImageList_Draw(HandleRef himl, int i, HandleRef hdcDst, int x, int y, int fStyle);

        [DllImport(ExternDll.Comctl32)]
        public static extern bool ImageList_Replace(HandleRef himl, int i, IntPtr hbmImage, IntPtr hbmMask);

        [DllImport(ExternDll.Comctl32)]
        public static extern bool ImageList_DrawEx(HandleRef himl, int i, HandleRef hdcDst, int x, int y, int dx, int dy, int rgbBk, int rgbFg, int fStyle);

        [DllImport(ExternDll.Comctl32)]
        public static extern bool ImageList_GetIconSize(HandleRef himl, out int x, out int y);

#if DEBUG
        [DllImport(ExternDll.Comctl32, EntryPoint = "ImageList_Duplicate")]
        private static extern IntPtr IntImageList_Duplicate(HandleRef himl);

        public static IntPtr ImageList_Duplicate(HandleRef himl)
        {
            IntPtr newHandle = IntImageList_Duplicate(himl);
            validImageListHandles.Add(newHandle);
            return newHandle;
        }
#else
        [DllImport(ExternDll.Comctl32)]
        public static extern IntPtr ImageList_Duplicate(HandleRef himl);
#endif

        [DllImport(ExternDll.Comctl32)]
        public static extern bool ImageList_Remove(HandleRef himl, int i);

        [DllImport(ExternDll.Comctl32)]
        public static extern bool ImageList_GetImageInfo(HandleRef himl, int i, NativeMethods.IMAGEINFO pImageInfo);

#if DEBUG
        [DllImport(ExternDll.Comctl32, EntryPoint = "ImageList_Read")]
        private static extern IntPtr IntImageList_Read(IStream pstm);

        public static IntPtr ImageList_Read(IStream pstm)
        {
            IntPtr newHandle = IntImageList_Read(pstm);
            validImageListHandles.Add(newHandle);
            return newHandle;
        }
#else
        [DllImport(ExternDll.Comctl32)]
        public static extern IntPtr ImageList_Read(IStream pstm);
#endif

        [DllImport(ExternDll.Comctl32)]
        public static extern bool ImageList_Write(HandleRef himl, IStream pstm);

        [DllImport(ExternDll.Comctl32)]
        public static extern int ImageList_WriteEx(HandleRef himl, int dwFlags, IStream pstm);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool TrackPopupMenuEx(HandleRef hmenu, int fuFlags, int x, int y, HandleRef hwnd, NativeMethods.TPMPARAMS tpm);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetKeyboardLayout(int dwLayout);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr ActivateKeyboardLayout(HandleRef hkl, int uFlags);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetKeyboardLayoutList(int size, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] hkls);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref NativeMethods.DEVMODE lpDevMode);

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

        [DllImport(ExternDll.Gdi32, ExactSpelling = true)]
        public static unsafe extern bool SetViewportExtEx(IntPtr hDC, int x, int y, Size *size);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool AdjustWindowRectEx(ref Interop.RECT lpRect, int dwStyle, bool bMenu, int dwExStyle);

        // This API is available only starting Windows 10 RS1
        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool AdjustWindowRectExForDpi(ref RECT lpRect, int dwStyle, bool bMenu, int dwExStyle, uint dpi);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool EnableWindow(HandleRef hWnd, bool enable);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool GetClientRect(HandleRef hWnd, ref RECT rect);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetDoubleClickTime();

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool ValidateRect(HandleRef hWnd, ref RECT rect);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool ValidateRect(IntPtr hwnd, IntPtr prect);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static unsafe extern bool SetViewportOrgEx(IntPtr hdc, int x, int y, Point *lppt);

        public static unsafe bool SetViewportOrgEx(HandleRef hdc, int x, int y, Point *lppt)
        {
            bool result = SetViewportOrgEx(hdc.Handle, x, y, lppt);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern bool LPtoDP(HandleRef hDC, ref RECT lpRect, int nCount);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static unsafe extern bool SetWindowOrgEx(IntPtr hdc, int x, int y, Point *lppt);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool GetViewportOrgEx(HandleRef hdc, out Point lppoint);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int SetMapMode(HandleRef hDC, int nMapMode);

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

        [DllImport(ExternDll.User32, SetLastError = true, ExactSpelling = true)]
        public static extern int ScrollWindowEx(HandleRef hWnd, int nXAmount, int nYAmount, NativeMethods.COMRECT rectScrollRegion, ref RECT rectClip, HandleRef hrgnUpdate, ref RECT prcUpdate, int flags);

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

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool BitBlt(HandleRef hDC, int x, int y, int nWidth, int nHeight,
                                         HandleRef hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport(ExternDll.Gdi32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool ShowCaret(HandleRef hWnd);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool HideCaret(HandleRef hWnd);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern uint GetCaretBlinkTime();

        // Theming/Visual Styles
        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern bool IsAppThemed();

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int GetThemeAppProperties();

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern void SetThemeAppProperties(int Flags);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenThemeData(HandleRef hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszClassList);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int CloseThemeData(HandleRef hTheme);

        [DllImport(ExternDll.Uxtheme, CharSet=CharSet.Auto)]
        public static extern bool IsThemePartDefined(HandleRef hTheme, int iPartId, int iStateId);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int DrawThemeBackground(HandleRef hTheme, HandleRef hdc, int partId, int stateId, [In] NativeMethods.COMRECT pRect, [In] NativeMethods.COMRECT pClipRect);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int DrawThemeEdge(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, [In] NativeMethods.COMRECT pDestRect, int uEdge, int uFlags, [Out] NativeMethods.COMRECT pContentRect);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int DrawThemeParentBackground(HandleRef hwnd, HandleRef hdc, [In] NativeMethods.COMRECT prc);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern int DrawThemeText(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, [MarshalAs(UnmanagedType.LPWStr)] string pszText, int iCharCount, int dwTextFlags, int dwTextFlags2, [In] NativeMethods.COMRECT pRect);

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

        [DllImport(ExternDll.User32)]
        public static extern IntPtr OpenInputDesktop(int dwFlags, [MarshalAs(UnmanagedType.Bool)] bool fInherit, int dwDesiredAccess);

        [DllImport(ExternDll.User32)]
        public static extern bool CloseDesktop(IntPtr hDesktop);

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

        [DllImport(ExternDll.Gdi32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool RoundRect(HandleRef hDC, int left, int top, int right, int bottom, int width, int height);

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

