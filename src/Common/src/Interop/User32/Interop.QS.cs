// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum QS : uint
        {
            KEY = 0x0001,
            MOUSEMOVE = 0x0002,
            MOUSEBUTTON = 0x0004,
            POSTMESSAGE = 0x0008,
            TIMER = 0x0010,
            PAINT = 0x0020,
            SENDMESSAGE = 0x0040,
            HOTKEY = 0x0080,
            ALLPOSTMESSAGE = 0x0100,
            RAWINPUT = 0x0400,
            MOUSE = MOUSEMOVE | MOUSEBUTTON,
            INPUT = MOUSE | KEY | RAWINPUT,
            ALLEVENTS = INPUT | POSTMESSAGE | TIMER | PAINT | HOTKEY,
            ALLINPUT = INPUT | POSTMESSAGE | TIMER | PAINT | HOTKEY | SENDMESSAGE,
        }
    }
}
