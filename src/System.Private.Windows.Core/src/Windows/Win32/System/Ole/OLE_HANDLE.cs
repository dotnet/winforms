// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.System.Ole;

internal partial struct OLE_HANDLE
{
    // HANDLE objects are nints and need to be sign extended.

    public static explicit operator HICON(OLE_HANDLE handle) => new((int)handle.Value);
    public static explicit operator HBITMAP(OLE_HANDLE handle) => new((int)handle.Value);
    public static explicit operator HPALETTE(OLE_HANDLE handle) => new((int)handle.Value);
    public static explicit operator HMETAFILE(OLE_HANDLE handle) => new((int)handle.Value);
    public static explicit operator HENHMETAFILE(OLE_HANDLE handle) => new((int)handle.Value);
}
