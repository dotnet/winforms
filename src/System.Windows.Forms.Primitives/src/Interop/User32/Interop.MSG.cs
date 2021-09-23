// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class User32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public WM message;
            public nint wParam;
            public nint lParam;
            public uint time;
            public Point pt;

            public static implicit operator Message(MSG msg)
                => new Message { HWnd = msg.hwnd, Msg = (int)msg.message, _WParam = msg.wParam, _LParam = msg.lParam };

            public static implicit operator MSG(Message message)
                => new MSG { hwnd = message.HWnd, message = message._Msg, wParam = message._WParam, lParam = message._LParam };
        }
    }
}
