// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    public static ICONINFO GetIconInfo<T>(T cursor)
        where T : IHandle<HICON>
    {
        GetIconInfo(cursor.Handle, out ICONINFO info);
        GC.KeepAlive(cursor.Wrapper);
        return info;
    }
}
