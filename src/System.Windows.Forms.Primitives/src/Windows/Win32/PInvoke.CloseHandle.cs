// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="CloseHandle(HANDLE)"/>
    public static BOOL CloseHandle<T>(T handle) where T : IHandle<HANDLE>
    {
        BOOL result = CloseHandle(handle.Handle);
        GC.KeepAlive(handle.Wrapper);
        return result;
    }
}
