// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static class RegisteredMessage
{
    private static uint s_wmMouseEnterMessage;
    private static uint s_wmUnSubclass;

    public static MessageId WM_MOUSEENTER
    {
        get
        {
            if (s_wmMouseEnterMessage == 0)
            {
                s_wmMouseEnterMessage = PInvoke.RegisterWindowMessage("WinFormsMouseEnter");
            }

            return s_wmMouseEnterMessage;
        }
    }

    public static MessageId WM_UIUNSUBCLASS
    {
        get
        {
            if (s_wmUnSubclass == 0)
            {
                s_wmUnSubclass = PInvoke.RegisterWindowMessage("WinFormsUnSubclass");
            }

            return s_wmUnSubclass;
        }
    }
}
