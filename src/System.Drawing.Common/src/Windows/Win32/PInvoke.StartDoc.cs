// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="StartDoc(HDC, DOCINFOW*)"/>
    internal static unsafe int StartDoc<T>(T hdc, in DOCINFOW lpdi) where T : IHandle<HDC>
    {
        fixed (DOCINFOW* di = &lpdi)
        {
            int result = StartDoc(hdc.Handle, di);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
