// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    /// <inheritdoc cref="GetIconInfo(HICON, ICONINFO*)"/>
    public static ICONINFO GetIconInfo<T>(T icon)
        where T : IHandle<HICON>
    {
        GetIconInfo(icon.Handle, out ICONINFO info);
        GC.KeepAlive(icon.Wrapper);
        return info;
    }
}
