// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using Microsoft.Win32;
using Xunit;
using static Interop.User32;

namespace System
{
    public static class SystemEventsHelper
    {
        private static IntPtr GetHWnd()
        {
            // locate the hwnd used by SystemEvents in this domain
            FieldInfo windowClassNameField = typeof(SystemEvents).GetField("s_className", BindingFlags.Static | BindingFlags.NonPublic) ??  // runtime
                                             typeof(SystemEvents).GetField("className", BindingFlags.Static | BindingFlags.NonPublic);      // desktop
            Assert.NotNull(windowClassNameField);
            string windowClassName = windowClassNameField.GetValue(null) as string;
            Assert.NotNull(windowClassName);

            IntPtr window = FindWindowW(windowClassName, null);
            return window;
        }

        public static void SendMessageOnUserPreferenceChanged(UserPreferenceCategory category)
        {
            IntPtr window = GetHWnd();

            WM msg;
            IntPtr wParam;
            if (category == UserPreferenceCategory.Color)
            {
                msg = WM.SYSCOLORCHANGE;
                wParam = IntPtr.Zero;
            }
            else
            {
                msg = WM.SETTINGCHANGE;

                if (category == UserPreferenceCategory.Accessibility)
                {
                    wParam = (IntPtr)SPI.SETHIGHCONTRAST;
                }
                else if (category == UserPreferenceCategory.Desktop)
                {
                    wParam = (IntPtr)SPI.SETDESKWALLPAPER;
                }
                else if (category == UserPreferenceCategory.Icon)
                {
                    wParam = (IntPtr)SPI.ICONHORIZONTALSPACING;
                }
                else if (category == UserPreferenceCategory.Mouse)
                {
                    wParam = (IntPtr)SPI.SETDOUBLECLICKTIME;
                }
                else if (category == UserPreferenceCategory.Keyboard)
                {
                    wParam = (IntPtr)SPI.SETKEYBOARDDELAY;
                }
                else if (category == UserPreferenceCategory.Menu)
                {
                    wParam = (IntPtr)SPI.SETMENUDROPALIGNMENT;
                }
                else if (category == UserPreferenceCategory.Power)
                {
                    wParam = (IntPtr)SPI.SETLOWPOWERACTIVE;
                }
                else if (category == UserPreferenceCategory.Screensaver)
                {
                    wParam = (IntPtr)SPI.SETMENUDROPALIGNMENT;
                }
                else if (category == UserPreferenceCategory.Window)
                {
                    wParam = (IntPtr)SPI.SETMENUDROPALIGNMENT;
                }
                else
                {
                    throw new NotImplementedException($"Not implemented category {category}.");
                }
            }

            // Call with reflect to immediately send the message.
            SendMessageW(window, msg | WM.REFLECT, wParam, IntPtr.Zero);
        }
    }
}
