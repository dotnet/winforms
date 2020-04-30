// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public struct RGNDATAHEADER
        {
            public uint dwSize;
            public uint iType;
            public uint nCount;
            public uint nRgnSize;
            public RECT rcBound;
        }
    }
}
