﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct LVBKIMAGEW
        {
            public LIST_VIEW_BACKGROUND_IMAGE_FLAGS ulFlags;
            public IntPtr hbm;
            public char* pszImage;
            public uint cchImageMax;
            public int xOffsetPercent;
            public int yOffsetPercent;
        }
    }
}
