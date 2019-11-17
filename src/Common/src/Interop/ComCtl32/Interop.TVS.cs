// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TVS : uint
        {
            HASBUTTONS = 0x0001,
            HASLINES = 0x0002,
            LINESATROOT = 0x0004,
            EDITLABELS = 0x0008,
            DISABLEDRAGDROP = 0x0010,
            SHOWSELALWAYS = 0x0020,
            RTLREADING = 0x0040,
            NOTOOLTIPS = 0x0080,
            CHECKBOXES = 0x0100,
            TRACKSELECT = 0x0200,
            SINGLEEXPAND = 0x0400,
            INFOTIP = 0x0800,
            FULLROWSELECT = 0x1000,
            NOSCROLL = 0x2000,
            NONEVENHEIGHT = 0x4000,
            NOHSCROLL = 0x8000,
        }
    }
}
