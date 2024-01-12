// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    /// <inheritdoc cref="GetObject(HGDIOBJ,int,void*)"/>
    public static unsafe bool GetObject<T>(HGDIOBJ h, out T @object) where T : unmanaged
    {
        // HGDIOBJ isn't technically correct, but close enough to filter out bigger mistakes (HWND, etc.).

        @object = default;
        fixed (void* pv = &@object)
        {
            return GetObject(h, sizeof(T), pv) != 0;
        }
    }
}
