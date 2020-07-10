// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern BOOL DrawIconEx(
            Gdi32.HDC hDC,
            int xLeft,
            int yTop,
            IntPtr hIcon,
            int cxWidth,
            int cyWidth,
            uint istepIfAniCur,
            IntPtr hbrFlickerFreeDraw,
            DI diFlags);

        public static BOOL DrawIconEx(
            Gdi32.HDC hDC,
            int xLeft,
            int yTop,
            IHandle hIcon,
            int cxWidth,
            int cyWidth)
        {
            BOOL result = DrawIconEx(hDC, xLeft, yTop, hIcon.Handle, cxWidth, cyWidth, 0, IntPtr.Zero, DI.NORMAL);
            GC.KeepAlive(hIcon);
            return result;
        }

        public static BOOL DrawIconEx(
            IHandle hDC,
            int xLeft,
            int yTop,
            IHandle hIcon,
            int cxWidth,
            int cyWidth)
        {
            BOOL result = DrawIconEx((Gdi32.HDC)hDC.Handle, xLeft, yTop, hIcon.Handle, cxWidth, cyWidth, 0, IntPtr.Zero, DI.NORMAL);
            GC.KeepAlive(hIcon);
            GC.KeepAlive(hDC);
            return result;
        }

        public enum DI : uint
        {
            MASK        = 0x0001,
            IMAGE       = 0x0002,
            NORMAL      = 0x0003,
            COMPAT      = 0x0004,
            DEFAULTSIZE = 0x0008,
            NOMIRROR    = 0x0010
        }
    }
}
