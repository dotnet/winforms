// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System;

internal class Window : IDisposable, IHandle<HWND>
{
    private readonly WindowClass _windowClass;

    public HWND Handle { get; }

    public Window(
        WindowClass windowClass,
        Rectangle bounds,
        string windowName = default,
        WINDOW_STYLE style = WINDOW_STYLE.WS_OVERLAPPED,
        WINDOW_EX_STYLE extendedStyle = default,
        bool isMainWindow = false,
        Window parentWindow = default,
        nint parameters = default,
        HMENU menuHandle = default)
    {
        _windowClass = windowClass;
        if (!_windowClass.IsRegistered)
        {
            _windowClass.Register();
        }

        Handle = _windowClass.CreateWindow(
            bounds,
            windowName,
            style,
            extendedStyle,
            isMainWindow,
            parentWindow?.Handle ?? default,
            parameters,
            menuHandle);
    }

    public void Dispose()
    {
        if (!Handle.IsNull)
        {
            PInvoke.DestroyWindow(Handle);
        }

        GC.SuppressFinalize(this);
    }
}
