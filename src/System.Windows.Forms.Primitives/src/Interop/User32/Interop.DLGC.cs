// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum DLGC : uint
        {
            WANTARROWS = 0x0001,
            WANTTAB = 0x0002,
            WANTALLKEYS = 0x0004,
            WANTMESSAGE = 0x0004,       // Pass message to control.
            HASSETSEL = 0x0008,         // Understands EM_SETSEL message.
            DEFPUSHBUTTON = 0x0010,
            UNDEFPUSHBUTTON = 0x0020,
            RADIOBUTTON = 0x0040,
            WANTCHARS = 0x0080,
            STATIC = 0x0100,
            BUTTON = 0x2000,
        }
    }
}
