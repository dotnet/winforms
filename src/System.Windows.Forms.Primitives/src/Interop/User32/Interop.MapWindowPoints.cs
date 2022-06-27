// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [LibraryImport(Libraries.User32)]
        public unsafe static partial int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, Point* lpPoints, uint cPoints);

        public unsafe static int MapWindowPoint(IHandle hWndFrom, IHandle hWndTo, ref Point lpPoints)
        {
            fixed (Point* p = &lpPoints)
            {
                int result = MapWindowPoints(
                    hWndFrom?.Handle ?? IntPtr.Zero,
                    hWndTo?.Handle ?? IntPtr.Zero,
                    p,
                    1);

                GC.KeepAlive(hWndFrom);
                GC.KeepAlive(hWndTo);
                return result;
            }
        }

        public unsafe static int MapWindowPoint(IHandle hWndFrom, IntPtr hWndTo, ref Point lpPoints)
        {
            fixed (Point* p = &lpPoints)
            {
                int result = MapWindowPoints(hWndFrom.Handle, hWndTo, p, 1);
                GC.KeepAlive(hWndFrom);
                return result;
            }
        }

        public unsafe static int MapWindowPoint(IntPtr hWndFrom, IHandle hWndTo, ref Point lpPoints)
        {
            fixed (Point* p = &lpPoints)
            {
                int result = MapWindowPoints(hWndFrom, hWndTo.Handle, p, 1);
                GC.KeepAlive(hWndTo);
                return result;
            }
        }

        public unsafe static int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref RECT lpPoints)
        {
            fixed (RECT* r = &lpPoints)
            {
                int result = MapWindowPoints(hWndFrom, hWndTo, (Point*)r, 2);
                return result;
            }
        }

        public unsafe static int MapWindowPoints(IHandle hWndFrom, IntPtr hWndTo, ref RECT lpPoints)
        {
            fixed (RECT* r = &lpPoints)
            {
                int result = MapWindowPoints(hWndFrom.Handle, hWndTo, (Point*)r, 2);
                GC.KeepAlive(hWndFrom);
                return result;
            }
        }

        public unsafe static int MapWindowPoints(IntPtr hWndFrom, IHandle hWndTo, ref RECT lpPoints)
        {
            fixed (RECT* r = &lpPoints)
            {
                int result = MapWindowPoints(hWndFrom, hWndTo.Handle, (Point*)r, 2);
                GC.KeepAlive(hWndTo);
                return result;
            }
        }
    }
}
