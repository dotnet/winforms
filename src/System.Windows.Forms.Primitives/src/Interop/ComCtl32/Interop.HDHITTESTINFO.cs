// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct HDHITTESTINFO
        {
            public Point pt;
            public HEADER_HITTEST_INFO_FLAGS flags;
            public int iItem;
        }
    }
}
