// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Internal
{
    internal static partial class IntUnsafeNativeMethods
    {
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern int SaveDC(HandleRef hDC);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern bool RestoreDC(HandleRef hDC, int nSavedDC);

        [DllImport(ExternDll.User32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr WindowFromDC(HandleRef hDC);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetDeviceCaps(HandleRef hDC, int nIndex);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern bool OffsetViewportOrgEx(HandleRef hDC, int nXOffset, int nYOffset, ref Point point);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "SetGraphicsMode", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int IntSetGraphicsMode(HandleRef hDC, int iMode);

        public static int SetGraphicsMode(HandleRef hDC, int iMode)
        {
            iMode = IntSetGraphicsMode(hDC, iMode);
            DbgUtil.AssertWin32(iMode != 0, "SetGraphicsMode([hdc=0x{0:X8}], [GM_ADVANCED]) failed.", hDC.Handle);
            return iMode;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetGraphicsMode(HandleRef hDC);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern int GetROP2(HandleRef hdc);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int SetROP2(HandleRef hDC, int nDrawMode);

        // Region.
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "CombineRgn", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntNativeMethods.RegionFlags IntCombineRgn(HandleRef hRgnDest, HandleRef hRgnSrc1, HandleRef hRgnSrc2, RegionCombineMode combineMode);

        public static IntNativeMethods.RegionFlags CombineRgn(HandleRef hRgnDest, HandleRef hRgnSrc1, HandleRef hRgnSrc2, RegionCombineMode combineMode)
        {
            Debug.Assert(hRgnDest.Wrapper != null && hRgnDest.Handle != IntPtr.Zero, "Destination region is invalid");
            Debug.Assert(hRgnSrc1.Wrapper != null && hRgnSrc1.Handle != IntPtr.Zero, "Source region 1 is invalid");
            Debug.Assert(hRgnSrc2.Wrapper != null && hRgnSrc2.Handle != IntPtr.Zero, "Source region 2 is invalid");

            if (hRgnDest.Wrapper == null || hRgnSrc1.Wrapper == null || hRgnSrc2.Wrapper == null)
            {
                return IntNativeMethods.RegionFlags.ERROR;
            }

            // Note: CombineRgn can return Error when no regions are combined, this is not an error condition.
            return IntCombineRgn(hRgnDest, hRgnSrc1, hRgnSrc2, combineMode);
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "GetClipRgn", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int IntGetClipRgn(HandleRef hDC, HandleRef hRgn);

        public static int GetClipRgn(HandleRef hDC, HandleRef hRgn)
        {
            int retVal = IntGetClipRgn(hDC, hRgn);
            DbgUtil.AssertWin32(retVal != -1, "IntGetClipRgn([hdc=0x{0:X8}], [hRgn]) failed.", hDC.Handle);
            return retVal;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "SelectClipRgn", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntNativeMethods.RegionFlags IntSelectClipRgn(HandleRef hDC, HandleRef hRgn);

        public static IntNativeMethods.RegionFlags SelectClipRgn(HandleRef hDC, HandleRef hRgn)
        {
            IntNativeMethods.RegionFlags result = IntSelectClipRgn(hDC, hRgn);
            DbgUtil.AssertWin32(result != IntNativeMethods.RegionFlags.ERROR, "SelectClipRgn([hdc=0x{0:X8}], [hRegion=0x{1:X8}]) failed.", hDC.Handle, hRgn.Handle);
            return result;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "GetRgnBox", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntNativeMethods.RegionFlags IntGetRgnBox(HandleRef hRgn, [In, Out] ref IntNativeMethods.RECT clipRect);

        public static IntNativeMethods.RegionFlags GetRgnBox(HandleRef hRgn, [In, Out] ref IntNativeMethods.RECT clipRect)
        {
            IntNativeMethods.RegionFlags result = IntGetRgnBox(hRgn, ref clipRect);
            DbgUtil.AssertWin32(result != IntNativeMethods.RegionFlags.ERROR, "GetRgnBox([hRegion=0x{0:X8}], [out rect]) failed.", hRgn.Handle);
            return result;
        }

        // Font.
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFontIndirectW(ref NativeMethods.LOGFONT lplf);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int GetObjectW(HandleRef hFont, int nSize, ref NativeMethods.LOGFONT pv);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "SelectObject", CharSet = CharSet.Auto)]
        public static extern IntPtr IntSelectObject(HandleRef hdc, HandleRef obj);

        public static IntPtr SelectObject(HandleRef hdc, HandleRef obj)
        {
            IntPtr oldObj = IntSelectObject(hdc, obj);
            DbgUtil.AssertWin32(oldObj != IntPtr.Zero, "SelectObject(hdc=hObj=[0x{0:X8}], hObj=[0x{1:X8}]) failed.", hdc.Handle, obj.Handle);
            return oldObj;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "GetCurrentObject", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr IntGetCurrentObject(HandleRef hDC, int uObjectType);

        public static IntPtr GetCurrentObject(HandleRef hDC, int uObjectType)
        {
            IntPtr hGdiObj = IntGetCurrentObject(hDC, uObjectType);
            // If the selected object is a region the return value is HGI_ERROR on failure.
            DbgUtil.AssertWin32(hGdiObj != IntPtr.Zero, "GetObject(hdc=[0x{0:X8}], type=[{1}]) failed.", hDC, uObjectType);
            return hGdiObj;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "GetStockObject", CharSet = CharSet.Auto)]
        public static extern IntPtr IntGetStockObject(int nIndex);

        public static IntPtr GetStockObject(int nIndex)
        {
            IntPtr hGdiObj = IntGetStockObject(nIndex);
            DbgUtil.AssertWin32(hGdiObj != IntPtr.Zero, "GetStockObject({0}) failed.", nIndex);
            return hGdiObj;
        }

        // Drawing.

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetNearestColor(HandleRef hDC, int color);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int /*COLORREF*/ SetTextColor(HandleRef hDC, int crColor);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetTextAlign(HandleRef hdc);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int /*COLORREF*/ GetTextColor(HandleRef hDC);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int SetBkColor(HandleRef hDC, int clr);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "SetBkMode", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int IntSetBkMode(HandleRef hDC, int nBkMode);

        public static int SetBkMode(HandleRef hDC, int nBkMode)
        {
            int oldMode = IntSetBkMode(hDC, nBkMode);
            DbgUtil.AssertWin32(oldMode != 0, "SetBkMode(hdc=[0x{0:X8}], Mode=[{1}]) failed.", hDC.Handle, nBkMode);
            return oldMode;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "GetBkMode", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int IntGetBkMode(HandleRef hDC);

        public static int GetBkMode(HandleRef hDC)
        {
            int mode = IntGetBkMode(hDC);
            DbgUtil.AssertWin32(mode != 0, "GetBkMode(hdc=[0x{0:X8}]) failed.", hDC.Handle);
            return mode;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetBkColor(HandleRef hDC);

        [DllImport(ExternDll.User32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        public static extern int DrawTextW(HandleRef hDC, string lpszString, int nCount, ref IntNativeMethods.RECT lpRect, int nFormat);

        public static int DrawText(HandleRef hDC, string text, ref IntNativeMethods.RECT lpRect, int nFormat)
        {
            int retVal = DrawTextW(hDC, text, text.Length, ref lpRect, nFormat);

            DbgUtil.AssertWin32(retVal != 0, "DrawText(hdc=[0x{0:X8}], text=[{1}], rect=[{2}], flags=[{3}] failed.", hDC.Handle, text, lpRect, nFormat);
            return retVal;
        }

        [DllImport(ExternDll.User32, SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        public static extern int DrawTextExW(HandleRef hDC, string lpszString, int nCount, ref IntNativeMethods.RECT lpRect, int nFormat, [In, Out] IntNativeMethods.DRAWTEXTPARAMS lpDTParams);

        public static int DrawTextEx(HandleRef hDC, string text, ref IntNativeMethods.RECT lpRect, int nFormat, [In, Out] IntNativeMethods.DRAWTEXTPARAMS lpDTParams)
        {
            int retVal = DrawTextExW(hDC, text, text.Length, ref lpRect, nFormat, lpDTParams);

            DbgUtil.AssertWin32(retVal != 0, "DrawTextEx(hdc=[0x{0:X8}], text=[{1}], rect=[{2}], flags=[{3}] failed.", hDC.Handle, text, lpRect, nFormat);
            return retVal;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int GetTextExtentPoint32W(HandleRef hDC, string text, int len, ref Size size);

        /// <remarks>
        /// This method is currently used just for drawing the text background
        /// (ComponentEditorForm.cs) and not for rendering text.
        /// Prefer using DrawText over this method if possible, it handles issues on older
        /// platforms properly. Ideally, we should remove this method but to avoid issues at this
        /// point I'm leaving it here.
        /// </remarks>
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = false, CharSet = CharSet.Auto)]
        internal static extern bool ExtTextOut(HandleRef hdc, int x, int y, int options, ref IntNativeMethods.RECT rect, string str, int length, int[] spacing);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "LineTo", CharSet = CharSet.Auto)]
        public static extern bool IntLineTo(HandleRef hdc, int x, int y);

        public static bool LineTo(HandleRef hdc, int x, int y)
        {
            bool retVal = IntLineTo(hdc, x, y);
            DbgUtil.AssertWin32(retVal, "LineTo(hdc=[0x{0:X8}], x=[{1}], y=[{2}] failed.", hdc.Handle, x, y);
            return retVal;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern bool MoveToEx(HandleRef hdc, int x, int y, ref Point pt);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "Rectangle", CharSet = CharSet.Auto)]
        public static extern bool IntRectangle(HandleRef hdc, int left, int top, int right, int bottom);

        public static bool Rectangle(HandleRef hdc, int left, int top, int right, int bottom)
        {
            bool retVal = IntRectangle(hdc, left, top, right, bottom);
            DbgUtil.AssertWin32(retVal, "Rectangle(hdc=[0x{0:X8}], left=[{1}], top=[{2}], right=[{3}], bottom=[{4}] failed.", hdc.Handle, left, top, right, bottom);
            return retVal;
        }

        [DllImport(ExternDll.User32, SetLastError = true, ExactSpelling = true, EntryPoint = "FillRect", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool IntFillRect(HandleRef hdc, [In] ref IntNativeMethods.RECT rect, HandleRef hbrush);

        public static bool FillRect(HandleRef hDC, [In] ref IntNativeMethods.RECT rect, HandleRef hbrush)
        {
            bool retVal = IntFillRect(hDC, ref rect, hbrush);
            DbgUtil.AssertWin32(retVal, "FillRect(hdc=[0x{0:X8}], rect=[{1}], hbrush=[{2}]", hDC.Handle, rect, hbrush.Handle);
            return retVal;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "SetMapMode", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int IntSetMapMode(HandleRef hDC, int nMapMode);

        public static int SetMapMode(HandleRef hDC, int nMapMode)
        {
            int oldMapMode = IntSetMapMode(hDC, nMapMode);
            DbgUtil.AssertWin32(oldMapMode != 0, "SetMapMode(hdc=[0x{0:X8}], MapMode=[{1}]", hDC.Handle, nMapMode);
            return oldMapMode;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "GetMapMode", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int IntGetMapMode(HandleRef hDC);

        public static int GetMapMode(HandleRef hDC)
        {
            int mapMode = IntGetMapMode(hDC);
            DbgUtil.AssertWin32(mapMode != 0, "GetMapMode(hdc=[0x{0:X8}]", hDC.Handle);
            return mapMode;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern bool GetViewportExtEx(HandleRef hdc, ref Size lpSize);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern bool GetViewportOrgEx(HandleRef hdc, ref Point lpPoint);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern bool SetViewportExtEx(HandleRef hDC, int x, int y, ref Size size);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern bool SetViewportOrgEx(HandleRef hDC, int x, int y, ref Point point);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int GetTextMetricsW(HandleRef hDC, ref IntNativeMethods.TEXTMETRIC lptm);

        public static int GetTextMetrics(HandleRef hDC, ref IntNativeMethods.TEXTMETRIC lptm)
        {
            int retVal = IntUnsafeNativeMethods.GetTextMetricsW(hDC, ref lptm);

            DbgUtil.AssertWin32(retVal != 0, "GetTextMetrics(hdc=[0x{0:X8}], [out TEXTMETRIC] failed.", hDC.Handle);
            return retVal;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "BeginPath", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool IntBeginPath(HandleRef hDC);

        public static bool BeginPath(HandleRef hDC)
        {
            bool retVal = IntBeginPath(hDC);
            DbgUtil.AssertWin32(retVal, "BeginPath(hdc=[0x{0:X8}]failed.", hDC.Handle);
            return retVal;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "EndPath", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool IntEndPath(HandleRef hDC);

        public static bool EndPath(HandleRef hDC)
        {
            bool retVal = IntEndPath(hDC);
            DbgUtil.AssertWin32(retVal, "EndPath(hdc=[0x{0:X8}]failed.", hDC.Handle);
            return retVal;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "StrokePath", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool IntStrokePath(HandleRef hDC);

        public static bool StrokePath(HandleRef hDC)
        {
            bool retVal = IntStrokePath(hDC);
            DbgUtil.AssertWin32(retVal, "StrokePath(hdc=[0x{0:X8}]failed.", hDC.Handle);
            return retVal;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "AngleArc", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool IntAngleArc(HandleRef hDC, int x, int y, int radius, float startAngle, float endAngle);

        public static bool AngleArc(HandleRef hDC, int x, int y, int radius, float startAngle, float endAngle)
        {
            bool retVal = IntAngleArc(hDC, x, y, radius, startAngle, endAngle);
            DbgUtil.AssertWin32(retVal, "AngleArc(hdc=[0x{0:X8}], ...) failed.", hDC.Handle);
            return retVal;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "Arc", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool IntArc(
            HandleRef hDC,
            int nLeftRect,   // x-coord of rectangle's upper-left corner
            int nTopRect,    // y-coord of rectangle's upper-left corner
            int nRightRect,  // x-coord of rectangle's lower-right corner
            int nBottomRect, // y-coord of rectangle's lower-right corner
            int nXStartArc,  // x-coord of first radial ending point
            int nYStartArc,  // y-coord of first radial ending point
            int nXEndArc,    // x-coord of second radial ending point
            int nYEndArc     // y-coord of second radial ending point
            );
        public static bool Arc(
            HandleRef hDC,
            int nLeftRect,   // x-coord of rectangle's upper-left corner
            int nTopRect,    // y-coord of rectangle's upper-left corner
            int nRightRect,  // x-coord of rectangle's lower-right corner
            int nBottomRect, // y-coord of rectangle's lower-right corner
            int nXStartArc,  // x-coord of first radial ending point
            int nYStartArc,  // y-coord of first radial ending point
            int nXEndArc,    // x-coord of second radial ending point
            int nYEndArc     // y-coord of second radial ending point
            )
        {
            bool retVal = IntArc(hDC, nLeftRect, nTopRect, nRightRect, nBottomRect, nXStartArc, nYStartArc, nXEndArc, nYEndArc);
            DbgUtil.AssertWin32(retVal, "Arc(hdc=[0x{0:X8}], ...) failed.", hDC.Handle);
            return retVal;
        }

        // Misc.
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int SetTextAlign(HandleRef hDC, int nMode);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "Ellipse", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool IntEllipse(HandleRef hDc, int x1, int y1, int x2, int y2);

        public static bool Ellipse(HandleRef hDc, int x1, int y1, int x2, int y2)
        {
            bool retVal = IntEllipse(hDc, x1, y1, x2, y2);
            DbgUtil.AssertWin32(retVal, "Ellipse(hdc=[0x{0:X8}], x1=[{1}], y1=[{2}], x2=[{3}], y2=[{4}]) failed.", hDc.Handle, x1, y1, x2, y2);
            return retVal;
        }
    }
}
