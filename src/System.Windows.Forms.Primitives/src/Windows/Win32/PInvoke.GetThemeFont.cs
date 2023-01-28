// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static unsafe HRESULT GetThemeFont<T>(T hTheme, HDC hdc, int iPartId, int iStateId, int iPropId, out LOGFONTW pFont)
            where T : IHandle<HTHEME>
        {
            HRESULT result = GetThemeFont(hTheme.Handle, hdc, iPartId, iStateId, iPropId, out pFont);
            GC.KeepAlive(hTheme.Wrapper);
            return result;
        }
    }
}
