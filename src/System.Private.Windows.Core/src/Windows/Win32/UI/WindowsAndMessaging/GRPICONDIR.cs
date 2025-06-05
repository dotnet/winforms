// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32.UI.WindowsAndMessaging;

// https://devblogs.microsoft.com/oldnewthing/?p=7083
// https://devblogs.microsoft.com/oldnewthing/?p=108925
// https://learn.microsoft.com/en-us/windows/win32/menurc/newheader

[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct GRPICONDIR
{
    public ushort Reserved;
    public ushort ResType;
    public ushort ResCount;
}
