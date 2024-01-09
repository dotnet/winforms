// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="EndPage(HDC)"/>
    internal static unsafe int EndPage<T>(T hdc) where T : IHandle<HDC>
    {
        int result = EndPage(hdc.Handle);
        GC.KeepAlive(hdc.Wrapper);
        return result;
    }
}
