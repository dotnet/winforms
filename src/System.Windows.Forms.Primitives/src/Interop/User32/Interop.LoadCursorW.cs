// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public static class CursorResourceId
        {
            public static IntPtr IDC_ARROW = (IntPtr)32512;
            public static IntPtr IDC_IBEAM = (IntPtr)32513;
            public static IntPtr IDC_WAIT = (IntPtr)32514;
            public static IntPtr IDC_CROSS = (IntPtr)32515;
            public static IntPtr IDC_SIZEALL = (IntPtr)32646;
            public static IntPtr IDC_SIZENWSE = (IntPtr)32642;
            public static IntPtr IDC_SIZENESW = (IntPtr)32643;
            public static IntPtr IDC_SIZEWE = (IntPtr)32644;
            public static IntPtr IDC_SIZENS = (IntPtr)32645;
            public static IntPtr IDC_UPARROW = (IntPtr)32516;
            public static IntPtr IDC_NO = (IntPtr)32648;
            public static IntPtr IDC_HAND = (IntPtr)32649;
            public static IntPtr IDC_APPSTARTING = (IntPtr)32650;
            public static IntPtr IDC_HELP = (IntPtr)32651;
        }

        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern IntPtr LoadCursorW(IntPtr hInstance, IntPtr lpCursorName);
    }
}
