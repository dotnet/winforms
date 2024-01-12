// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32.UI.Shell;

// This can't be imported in CsWin32 as it technically isn't the same on both X86 and X64 due to a packing of 1 byte on X86.
// For our purposes this is fine as the single definition's layout is the same on both platforms.

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal unsafe struct SHSTOCKICONINFO
{
    public uint cbSize;
    public HICON hIcon;
    public int iSysImageIndex;
    public int iIcon;
    public fixed char szPath[260];
}
