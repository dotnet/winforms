// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms.Internal
{
    internal static partial class IntUnsafeNativeMethods
    {
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern int GetROP2(HandleRef hdc);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int SetROP2(HandleRef hDC, int nDrawMode);

        // Font.
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFontIndirectW(ref NativeMethods.LOGFONTW lplf);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int GetObjectW(HandleRef hFont, int nSize, ref NativeMethods.LOGFONTW pv);

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

        /// <remarks>
        ///  This method is currently used just for drawing the text background
        ///  (ComponentEditorForm.cs) and not for rendering text.
        ///  Prefer using DrawText over this method if possible, it handles issues on older
        ///  platforms properly. Ideally, we should remove this method but to avoid issues at this
        ///  point I'm leaving it here.
        /// </remarks>
        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = false, CharSet = CharSet.Auto)]
        internal static extern bool ExtTextOut(HandleRef hdc, int x, int y, int options, ref RECT rect, string str, int length, int[] spacing);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "LineTo", CharSet = CharSet.Auto)]
        public static extern bool IntLineTo(HandleRef hdc, int x, int y);

        public static bool LineTo(HandleRef hdc, int x, int y)
        {
            bool retVal = IntLineTo(hdc, x, y);
            DbgUtil.AssertWin32(retVal, "LineTo(hdc=[0x{0:X8}], x=[{1}], y=[{2}] failed.", hdc.Handle, x, y);
            return retVal;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static unsafe extern bool MoveToEx(HandleRef hdc, int x, int y, Point *lppt);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "Rectangle", CharSet = CharSet.Auto)]
        public static extern bool IntRectangle(HandleRef hdc, int left, int top, int right, int bottom);

        public static bool Rectangle(HandleRef hdc, int left, int top, int right, int bottom)
        {
            bool retVal = IntRectangle(hdc, left, top, right, bottom);
            DbgUtil.AssertWin32(retVal, "Rectangle(hdc=[0x{0:X8}], left=[{1}], top=[{2}], right=[{3}], bottom=[{4}] failed.", hdc.Handle, left, top, right, bottom);
            return retVal;
        }

        [DllImport(ExternDll.User32, SetLastError = true, ExactSpelling = true, EntryPoint = "FillRect", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool IntFillRect(HandleRef hdc, [In] ref RECT rect, HandleRef hbrush);

        public static bool FillRect(HandleRef hDC, [In] ref RECT rect, HandleRef hbrush)
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
        public static extern bool GetViewportOrgEx(HandleRef hdc, out Point lpPoint);

        [DllImport(ExternDll.Gdi32, ExactSpelling = true)]
        public static extern bool SetViewportExtEx(HandleRef hDC, int x, int y, ref Size size);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static unsafe extern bool SetViewportOrgEx(HandleRef hDC, int x, int y, Point *point);

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
