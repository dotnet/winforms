// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum BS : uint
        {
            PUSHBUTTON = 0x00000000,
            DEFPUSHBUTTON = 0x00000001,
            CHECKBOX = 0x00000002,
            AUTOCHECKBOX = 0x00000003,
            RADIOBUTTON = 0x00000004,
            THREE_STATE = 0x00000005,
            AUTO3STATE = 0x00000006,
            GROUPBOX = 0x00000007,
            USERBUTTON = 0x00000008,
            AUTORADIOBUTTON = 0x00000009,
            PUSHBOX = 0x0000000A,
            OWNERDRAW = 0x0000000B,
            TYPEMASK = 0x0000000F,
            LEFTTEXT = 0x00000020,
            TEXT = 0x00000000,
            ICON = 0x00000040,
            BITMAP = 0x00000080,
            LEFT = 0x00000100,
            RIGHT = 0x00000200,
            CENTER = 0x00000300,
            TOP = 0x00000400,
            BOTTOM = 0x00000800,
            VCENTER = 0x00000C00,
            PUSHLIKE = 0x00001000,
            MULTILINE = 0x00002000,
            NOTIFY = 0x00004000,
            FLAT = 0x00008000,
            RIGHTBUTTON = LEFTTEXT
        }
    }
}
