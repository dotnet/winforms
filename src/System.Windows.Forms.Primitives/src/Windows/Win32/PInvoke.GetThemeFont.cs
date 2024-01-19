// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Interop;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="GetThemeFont(HTHEME, HDC, int, int, int, LOGFONTW*)"/>
    public static unsafe HRESULT GetThemeFont<T>(T hTheme, HDC hdc, int iPartId, int iStateId, int iPropId, out LOGFONT pFont)
        where T : IHandle<HTHEME>
    {
        fixed (void* p = &pFont)
        {
            HRESULT result = GetThemeFont(hTheme.Handle, hdc, iPartId, iStateId, iPropId, (LOGFONTW*)p);
            GC.KeepAlive(hTheme.Wrapper);
            return result;
        }
    }
}
