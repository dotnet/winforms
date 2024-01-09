// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="AbortDoc(HDC)"/>
    internal static unsafe int AbortDoc<T>(T hdc) where T : IHandle<HDC>
    {
        int result = AbortDoc(hdc.Handle);
        GC.KeepAlive(hdc.Wrapper);
        return result;
    }
}
