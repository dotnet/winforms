// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref Point lpPoints, uint cPoints);

        public static int MapWindowPoints(HandleRef hWndFrom, IntPtr hWndTo, ref Point lpPoints, uint cPoints)
        {
            int result = MapWindowPoints(hWndFrom.Handle, hWndTo, ref lpPoints, cPoints);
            GC.KeepAlive(hWndFrom.Wrapper);
            return result;
        }

        public static int MapWindowPoints(IntPtr hWndFrom, HandleRef hWndTo, ref Point lpPoints, uint cPoints)
        {
            int result = MapWindowPoints(hWndFrom, hWndTo.Handle, ref lpPoints, cPoints);
            GC.KeepAlive(hWndTo.Wrapper);
            return result;
        }

        public static int MapWindowPoints(HandleRef hWndFrom, HandleRef hWndTo, ref Point lpPoints, uint cPoints)
        {
            int result = MapWindowPoints(hWndFrom.Handle, hWndTo.Handle, ref lpPoints, cPoints);
            GC.KeepAlive(hWndFrom.Wrapper);
            GC.KeepAlive(hWndTo.Wrapper);
            return result;
        }

        public static int MapWindowPoints(IntPtr hWndFrom, IHandle hWndTo, ref Point lpPoints, uint cPoints)
        {
            int result = MapWindowPoints(hWndFrom, hWndTo.Handle, ref lpPoints, cPoints);
            GC.KeepAlive(hWndTo);
            return result;
        }

        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref RECT lpPoints, uint cPoints);

        public static int MapWindowPoints(HandleRef hWndFrom, IntPtr hWndTo, ref RECT lpPoints, uint cPoints)
        {
            int result = MapWindowPoints(hWndFrom.Handle, hWndTo, ref lpPoints, cPoints);
            GC.KeepAlive(hWndFrom.Wrapper);
            return result;
        }

        public static int MapWindowPoints(IntPtr hWndFrom, HandleRef hWndTo, ref RECT lpPoints, uint cPoints)
        {
            int result = MapWindowPoints(hWndFrom, hWndTo.Handle, ref lpPoints, cPoints);
            GC.KeepAlive(hWndTo.Wrapper);
            return result;
        }

        public static int MapWindowPoints(HandleRef hWndFrom, HandleRef hWndTo, ref RECT lpPoints, uint cPoints)
        {
            int result = MapWindowPoints(hWndFrom.Handle, hWndTo.Handle, ref lpPoints, cPoints);
            GC.KeepAlive(hWndFrom.Wrapper);
            GC.KeepAlive(hWndTo.Wrapper);
            return result;
        }
    }
}
