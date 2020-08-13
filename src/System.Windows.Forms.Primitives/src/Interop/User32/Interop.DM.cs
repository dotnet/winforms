// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum DM : uint
        {
            ORIENTATION = 0x00000001,
            PAPERSIZE = 0x00000002,
            PAPERLENGTH = 0x00000004,
            PAPERWIDTH = 0x00000008,
            SCALE = 0x00000010,
            POSITION = 0x00000020,
            NUP = 0x00000040,
            DISPLAYORIENTATION = 0x00000080,
            COPIES = 0x00000100,
            DEFAULTSOURCE = 0x00000200,
            PRINTQUALITY = 0x00000400,
            COLOR = 0x00000800,
            DUPLEX = 0x00001000,
            YRESOLUTION = 0x00002000,
            TTOPTION = 0x00004000,
            COLLATE = 0x00008000,
            FORMNAME = 0x00010000,
            LOGPIXELS = 0x00020000,
            BITSPERPEL = 0x00040000,
            PELSWIDTH = 0x00080000,
            PELSHEIGHT = 0x00100000,
            DISPLAYFLAGS = 0x00200000,
            DISPLAYFREQUENCY = 0x00400000,
            ICMMETHOD = 0x00800000,
            ICMINTENT = 0x01000000,
            MEDIATYPE = 0x02000000,
            DITHERTYPE = 0x04000000,
            PANNINGWIDTH = 0x08000000,
            PANNINGHEIGHT = 0x10000000,
            DISPLAYFIXEDOUTPUT = 0x20000000,
        }
    }
}
