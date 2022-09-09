// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Microsoft.Win32;
using Windows.Win32.Foundation;
using Xunit;
using static Interop.User32;

namespace System
{
    public static class SystemEventsHelper
    {
        private static IntPtr GetHWnd()
        {
            // Locate the hwnd used by SystemEvents in this domain.
            FieldInfo windowClassNameField =
                typeof(SystemEvents).GetField("s_className", BindingFlags.Static | BindingFlags.NonPublic)   // runtime
                ?? typeof(SystemEvents).GetField("className", BindingFlags.Static | BindingFlags.NonPublic); // desktop

            Assert.NotNull(windowClassNameField);
            string windowClassName = windowClassNameField.GetValue(null) as string;
            Assert.NotNull(windowClassName);

            IntPtr window = FindWindowW(windowClassName, null);
            return window;
        }

        public static void SendMessageOnUserPreferenceChanged(UserPreferenceCategory category)
        {
            HWND window = (HWND)GetHWnd();

            WM msg;
            nint wParam;
            if (category == UserPreferenceCategory.Color)
            {
                msg = WM.SYSCOLORCHANGE;
                wParam = 0;
            }
            else
            {
                msg = WM.SETTINGCHANGE;

                if (category == UserPreferenceCategory.Accessibility)
                {
                    wParam = (nint)SPI.SETHIGHCONTRAST;
                }
                else if (category == UserPreferenceCategory.Desktop)
                {
                    wParam = (nint)SPI.SETDESKWALLPAPER;
                }
                else if (category == UserPreferenceCategory.Icon)
                {
                    wParam = (nint)SPI.ICONHORIZONTALSPACING;
                }
                else if (category == UserPreferenceCategory.Mouse)
                {
                    wParam = (nint)SPI.SETDOUBLECLICKTIME;
                }
                else if (category == UserPreferenceCategory.Keyboard)
                {
                    wParam = (nint)SPI.SETKEYBOARDDELAY;
                }
                else if (category == UserPreferenceCategory.Menu)
                {
                    wParam = (nint)SPI.SETMENUDROPALIGNMENT;
                }
                else if (category == UserPreferenceCategory.Power)
                {
                    wParam = (nint)SPI.SETLOWPOWERACTIVE;
                }
                else if (category == UserPreferenceCategory.Screensaver)
                {
                    wParam = (nint)SPI.SETMENUDROPALIGNMENT;
                }
                else if (category == UserPreferenceCategory.Window)
                {
                    wParam = (nint)SPI.SETMENUDROPALIGNMENT;
                }
                else
                {
                    throw new NotImplementedException($"Not implemented category {category}.");
                }
            }

            // Call with reflect to immediately send the message.
            SendMessageW(window, msg | WM.REFLECT, wParam);
        }
    }
}
