// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TVHT : uint
        {
            NOWHERE = 0x0001,
            ONITEMICON = 0x0002,
            ONITEMLABEL = 0x0004,
            ONITEM = ONITEMICON | ONITEMLABEL | ONITEMSTATEICON,
            ONITEMINDENT = 0x0008,
            ONITEMBUTTON = 0x0010,
            ONITEMRIGHT = 0x0020,
            ONITEMSTATEICON = 0x0040,
            ABOVE = 0x0100,
            BELOW = 0x0200,
            TORIGHT = 0x0400,
            TOLEFT = 0x0800
        }
    }
}
