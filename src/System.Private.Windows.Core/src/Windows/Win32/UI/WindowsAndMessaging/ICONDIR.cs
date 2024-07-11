// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32.UI.WindowsAndMessaging;

// Needs to be packed to 2 to get ICONDIRENTRY to follow immediately after idCount.
[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct ICONDIR
{
    // Must be 0
    public ushort idReserved;
    // Must be 1
    public ushort idType;
    // Count of entries
    public ushort idCount;
    // First entry (anysize array)
    // public ICONDIRENTRY idEntries;
}
