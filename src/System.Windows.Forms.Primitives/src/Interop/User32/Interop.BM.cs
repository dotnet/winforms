// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum BM : uint
        {
            GETCHECK = 0x00F0,
            SETCHECK = 0x00F1,
            GETSTATE = 0x00F2,
            SETSTATE = 0x00F3,
            SETSTYLE = 0x00F4,
            CLICK = 0x00F5,
            GETIMAGE = 0x00F6,
            SETIMAGE = 0x00F7,
            SETDONTCLICK = 0x00F8,
        }
    }
}
