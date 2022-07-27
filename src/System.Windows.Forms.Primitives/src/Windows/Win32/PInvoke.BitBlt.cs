// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static BOOL BitBlt(
            IHandle hdc,
            int x,
            int y,
            int cx,
            int cy,
            HDC hdcSrc,
            int x1,
            int y1,
            ROP_CODE rop)
        {
            BOOL result = BitBlt(
                (HDC)hdc.Handle,
                x,
                y,
                cx,
                cy,
                hdcSrc,
                x1,
                y1,
                rop);
            GC.KeepAlive(hdc);
            return result;
        }

        public static BOOL BitBlt(
            HDC hdc,
            int x,
            int y,
            int cx,
            int cy,
            IHandle hdcSrc,
            int x1,
            int y1,
            ROP_CODE rop)
        {
            BOOL result = BitBlt(
                hdc,
                x,
                y,
                cx,
                cy,
                (HDC)hdcSrc.Handle,
                x1,
                y1,
                rop);
            GC.KeepAlive(hdcSrc);
            return result;
        }
    }
}
