// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32.UI.WindowsAndMessaging;

// https://devblogs.microsoft.com/oldnewthing/20101018-00/?p=12513
// https://devblogs.microsoft.com/oldnewthing/20120720-00/?p=7083

[StructLayout(LayoutKind.Sequential)]
internal struct ICONDIRENTRY
{
    // Width and height are 1 - 255 or 0 for 256
    public byte bWidth;
    public byte bHeight;
    public byte bColorCount;
    public byte bReserved;
    public ushort wPlanes;
    public ushort wBitCount;
    public uint dwBytesInRes;
    public uint dwImageOffset;
}
