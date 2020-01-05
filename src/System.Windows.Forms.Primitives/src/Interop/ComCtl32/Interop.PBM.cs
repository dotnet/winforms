// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum PBM : uint
        {
            SETRANGE = User32.WM_USER + 1,
            SETPOS = User32.WM_USER + 2,
            DELTAPOS = User32.WM_USER + 3,
            SETSTEP = User32.WM_USER + 4,
            STEPIT = User32.WM_USER + 5,
            SETRANGE32 = User32.WM_USER + 6,
            GETRANGE = User32.WM_USER + 7,
            GETPOS = User32.WM_USER + 8,
            SETBARCOLOR = User32.WM_USER + 9,
            SETMARQUEE = User32.WM_USER + 10,
            GETSTEP = User32.WM_USER + 13,
            GETBKCOLOR = User32.WM_USER + 14,
            GETBARCOLOR = User32.WM_USER + 15,
            SETSTATE = User32.WM_USER + 16,
            GETSTATE = User32.WM_USER + 17,
            SETBKCOLOR = CCM.SETBKCOLOR
        }
    }
}
