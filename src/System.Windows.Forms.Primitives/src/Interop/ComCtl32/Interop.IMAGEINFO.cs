﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal partial class ComCtl32
    {
        public struct IMAGEINFO
        {
            public HBITMAP hbmImage;
            public HBITMAP hbmMask;
            public int Unused1;
            public int Unused2;
            public RECT rcImage;
        }
    }
}
