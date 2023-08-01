// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct LVFINDINFOW
        {
            public LVFINDINFOW_FLAGS flags;
            public char* psz;
            public nint lParam;
            public Point pt;
            public uint vkDirection;
        }
    }
}
