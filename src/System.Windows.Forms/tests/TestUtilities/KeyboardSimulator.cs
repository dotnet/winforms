// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;
using Windows.Win32.Foundation;

namespace System;

public static class KeyboardSimulator
{
    public static void KeyDown(Control control, Keys key)
    {
        (nint keyCode, nint lParam) = GetKeyParameters(key);
        PInvoke.SendMessage(control, PInvoke.WM_KEYDOWN, (WPARAM)keyCode, lParam);
    }

    public static void KeyPress(Control control, Keys key)
    {
        (nint keyCode, nint lParam) = GetKeyParameters(key);
        PInvoke.SendMessage(control, PInvoke.WM_KEYDOWN, (WPARAM)keyCode, lParam);
        PInvoke.SendMessage(control, PInvoke.WM_KEYUP, (WPARAM)keyCode, lParam);
    }

    private static (nint keyCode, nint lParam) GetKeyParameters(Keys key)
    {
        nint keyCode = (nint)key;
        int scanCode = (int)key;
        const int repeatCount = 1;
        nint lParam = PARAM.FromLowHighUnsigned(repeatCount, scanCode);
        return (keyCode , lParam);
    }
}
