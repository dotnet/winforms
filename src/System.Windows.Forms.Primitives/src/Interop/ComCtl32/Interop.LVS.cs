// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVS : uint
        {
            ICON = 0x0000,
            REPORT = 0x0001,
            SMALLICON = 0x0002,
            LIST = 0x0003,
            TYPEMASK = 0x0003,
            SINGLESEL = 0x0004,
            SHOWSELALWAYS = 0x0008,
            SORTASCENDING = 0x0010,
            SORTDESCENDING = 0x0020,
            SHAREIMAGELISTS = 0x0040,
            NOLABELWRAP = 0x0080,
            AUTOARRANGE = 0x0100,
            EDITLABELS = 0x0200,
            OWNERDATA = 0x1000,
            NOSCROLL = 0x2000,
            TYPESTYLEMASK = 0xfc00,
            ALIGNTOP = 0x0000,
            ALIGNLEFT = 0x0800,
            ALIGNMASK = 0x0c00,
            OWNERDRAWFIXED = 0x0400,
            NOCOLUMNHEADER = 0x4000,
            NOSORTHEADER = 0x8000
        }
    }
}
