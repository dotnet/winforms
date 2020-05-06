// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Interop.User32;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public static class KeyboardHelper
    {
        public unsafe static void SendKey(Keys key, bool down)
        {
            var input = new INPUT();
            input.type = INPUTENUM.KEYBOARD;
            input.inputUnion.ki.wVk = (ushort)key;
            input.inputUnion.ki.dwFlags = 0;
            input.inputUnion.ki.dwExtraInfo = IntPtr.Zero;
            input.inputUnion.ki.time = 0;
            input.inputUnion.ki.wScan = 0;

            if (!down)
                input.inputUnion.ki.dwFlags |= KEYEVENTF.KEYUP;

            SendInput(1, &input, Marshal.SizeOf(input));
        }
    }
}
