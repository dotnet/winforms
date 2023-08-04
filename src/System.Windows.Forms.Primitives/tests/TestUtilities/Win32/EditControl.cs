// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System;

internal class EditControl : Window
{
    private static readonly EditClass s_editClass = new();

    public EditControl(string windowName = default,
        WINDOW_STYLE style = WINDOW_STYLE.WS_OVERLAPPED,
        WINDOW_EX_STYLE extendedStyle = WINDOW_EX_STYLE.WS_EX_CLIENTEDGE | WINDOW_EX_STYLE.WS_EX_LEFT | WINDOW_EX_STYLE.WS_EX_LTRREADING,
        bool isMainWindow = false,
        Window parentWindow = default,
        nint parameters = default,
        HMENU menuHandle = default)
        : base(s_editClass, new Rectangle(0, 0, 100, 50), windowName, style, extendedStyle, isMainWindow, parentWindow, parameters, menuHandle)
    {
    }
}
