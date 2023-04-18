// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
#if NET8_0_OR_GREATER
        internal const uint SHGSI_ICON = 0x000000100;
        internal const uint SHGSI_ICONLOCATION = 0x00000000;

        [LibraryImport(Libraries.Shell32)]
        internal static partial HRESULT SHGetStockIconInfo(
            uint siid,
            uint uFlags,
            ref SHSTOCKICONINFO psii);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal unsafe struct SHSTOCKICONINFO
        {
            public uint cbSize;
            public nint hIcon;
            public int iSysImageIndex;
            public int iIcon;
            public fixed char szPath[260];
        }
#endif
    }
}
