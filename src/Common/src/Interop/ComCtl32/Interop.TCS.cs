// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TCS : uint
        {
            RIGHTJUSTIFY = 0x0000,
            SINGLELINE = 0x0000,
            TABS = 0x0000,
            SCROLLOPPOSITE = 0x0001,
            BOTTOM = 0x0002,
            RIGHT = 0x0002,
            MULTISELECT = 0x0004,
            FLATBUTTONS = 0x0008,
            FORCEICONLEFT = 0x0010,
            FORCELABELLEFT = 0x0020,
            HOTTRACK = 0x0040,
            VERTICAL = 0x0080,
            BUTTONS = 0x0100,
            MULTILINE = 0x0200,
            FIXEDWIDTH = 0x0400,
            RAGGEDRIGHT = 0x0800,
            FOCUSONBUTTONDOWN = 0x1000,
            OWNERDRAWFIXED = 0x2000,
            TOOLTIPS = 0x4000,
            FOCUSNEVER = 0x8000,
        }
    }
}
