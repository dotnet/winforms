// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using static Windows.Win32.UI.WindowsAndMessaging.SYSTEM_PARAMETERS_INFO_ACTION;

namespace System;

public static class SystemEventsHelper
{
    private static HWND GetHWnd()
    {
        // Locate the hwnd used by SystemEvents in this domain.
        FieldInfo windowClassNameField =
            typeof(SystemEvents).GetField("s_className", BindingFlags.Static | BindingFlags.NonPublic)   // runtime
            ?? typeof(SystemEvents).GetField("className", BindingFlags.Static | BindingFlags.NonPublic); // desktop

        Assert.NotNull(windowClassNameField);
        string windowClassName = windowClassNameField.GetValue(null) as string;
        Assert.NotNull(windowClassName);

        HWND window = PInvoke.FindWindow(windowClassName, null);
        return window;
    }

    public static void SendMessageOnUserPreferenceChanged(UserPreferenceCategory category)
    {
        HWND window = GetHWnd();

        MessageId msg;
        WPARAM wParam;
        if (category == UserPreferenceCategory.Color)
        {
            msg = PInvokeCore.WM_SYSCOLORCHANGE;
            wParam = 0;
        }
        else
        {
            msg = PInvokeCore.WM_SETTINGCHANGE;

            if (category == UserPreferenceCategory.Accessibility)
            {
                wParam = (int)SPI_SETHIGHCONTRAST;
            }
            else if (category == UserPreferenceCategory.Desktop)
            {
                wParam = (int)SPI_SETDESKWALLPAPER;
            }
            else if (category == UserPreferenceCategory.Icon)
            {
                wParam = (int)SPI_ICONHORIZONTALSPACING;
            }
            else if (category == UserPreferenceCategory.Mouse)
            {
                wParam = (int)SPI_SETDOUBLECLICKTIME;
            }
            else if (category == UserPreferenceCategory.Keyboard)
            {
                wParam = (int)SPI_SETKEYBOARDDELAY;
            }
            else if (category == UserPreferenceCategory.Menu)
            {
                wParam = (int)SPI_SETMENUDROPALIGNMENT;
            }
            else if (category == UserPreferenceCategory.Power)
            {
                wParam = (int)SPI_SETLOWPOWERACTIVE;
            }
            else if (category == UserPreferenceCategory.Screensaver)
            {
                wParam = (int)SPI_SETMENUDROPALIGNMENT;
            }
            else if (category == UserPreferenceCategory.Window)
            {
                wParam = (int)SPI_SETMENUDROPALIGNMENT;
            }
            else
            {
                throw new NotImplementedException($"Not implemented category {category}.");
            }
        }

        // Call with reflect to immediately send the message.
        PInvokeCore.SendMessage(window, msg | MessageId.WM_REFLECT, wParam);
    }
}
