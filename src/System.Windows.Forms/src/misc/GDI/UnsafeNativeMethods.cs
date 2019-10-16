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
        public static unsafe extern bool MoveToEx(HandleRef hdc, int x, int y, Point *lppt);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, EntryPoint = "Rectangle", CharSet = CharSet.Auto)]
        public static extern bool IntRectangle(HandleRef hdc, int left, int top, int right, int bottom);

        public static bool Rectangle(HandleRef hdc, int left, int top, int right, int bottom)
        {
            bool retVal = IntRectangle(hdc, left, top, right, bottom);
            DbgUtil.AssertWin32(retVal, "Rectangle(hdc=[0x{0:X8}], left=[{1}], top=[{2}], right=[{3}], bottom=[{4}] failed.", hdc.Handle, left, top, right, bottom);
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
    }
}
