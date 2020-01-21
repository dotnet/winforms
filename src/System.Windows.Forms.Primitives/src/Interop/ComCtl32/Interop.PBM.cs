// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum PBM : uint
        {
            SETRANGE = WM.USER + 1,
            SETPOS = WM.USER + 2,
            DELTAPOS = WM.USER + 3,
            SETSTEP = WM.USER + 4,
            STEPIT = WM.USER + 5,
            SETRANGE32 = WM.USER + 6,
            GETRANGE = WM.USER + 7,
            GETPOS = WM.USER + 8,
            SETBARCOLOR = WM.USER + 9,
            SETMARQUEE = WM.USER + 10,
            GETSTEP = WM.USER + 13,
            GETBKCOLOR = WM.USER + 14,
            GETBARCOLOR = WM.USER + 15,
            SETSTATE = WM.USER + 16,
            GETSTATE = WM.USER + 17,
            SETBKCOLOR = CCM.SETBKCOLOR
        }
    }
}
