// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    /// <inheritdoc cref="PeekMessage(MSG*, HWND, uint, uint, PEEK_MESSAGE_REMOVE_TYPE)"/>
    public static unsafe BOOL PeekMessage<T>(
        MSG* lpMsg,
        T hWnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax,
        PEEK_MESSAGE_REMOVE_TYPE wRemoveMsg)
        where T : IHandle<HWND>
    {
        BOOL result = PeekMessage(lpMsg, hWnd.Handle, wMsgFilterMin, wMsgFilterMax, wRemoveMsg);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}
