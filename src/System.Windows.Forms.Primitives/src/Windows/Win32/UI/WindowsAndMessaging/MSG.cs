// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace Windows.Win32.UI.WindowsAndMessaging;

internal partial struct MSG
{
    public static implicit operator Message(MSG msg)
        => Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);

    public static implicit operator MSG(Message message)
        => new()
        {
            hwnd = message.HWND,
            message = (uint)message.MsgInternal,
            wParam = message.WParamInternal,
            lParam = message.LParamInternal
        };
}
