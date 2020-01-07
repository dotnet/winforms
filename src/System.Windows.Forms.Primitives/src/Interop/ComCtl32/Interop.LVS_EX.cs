// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVS_EX : uint
        {
            GRIDLINES = 0x00000001,
            SUBITEMIMAGES = 0x00000002,
            CHECKBOXES = 0x00000004,
            TRACKSELECT = 0x00000008,
            HEADERDRAGDROP = 0x00000010,
            FULLROWSELECT = 0x00000020,
            ONECLICKACTIVATE = 0x00000040,
            TWOCLICKACTIVATE = 0x00000080,
            FLATSB = 0x00000100,
            REGIONAL = 0x00000200,
            INFOTIP = 0x00000400,
            UNDERLINEHOT = 0x00000800,
            UNDERLINECOLD = 0x00001000,
            MULTIWORKAREAS = 0x00002000,
            LABELTIP = 0x00004000,
            BORDERSELECT = 0x00008000,
            DOUBLEBUFFER = 0x00010000,
            HIDELABELS = 0x00020000,
            SINGLEROW = 0x00040000,
            SNAPTOGRID = 0x00080000,
            SIMPLESELECT = 0x00100000,
            JUSTIFYCOLUMNS = 0x00200000,
            TRANSPARENTBKGND = 0x00400000,
            TRANSPARENTSHADOWTEXT = 0x00800000,
            AUTOAUTOARRANGE = 0x01000000,
            HEADERINALLVIEWS = 0x02000000,
            AUTOCHECKSELECT = 0x08000000,
            AUTOSIZECOLUMNS = 0x10000000,
            COLUMNSNAPPOINTS = 0x40000000,
            COLUMNOVERFLOW = 0x80000000
        }
    }
}
