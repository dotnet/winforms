// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        // The Cursor class has an IntPtr constructor that takes a handle
        // to an existing cursor. The int constructor does the LoadCursorW
        // call. To avoid accidental use of the IntPtr constructor this
        // set of defines should be left as int, even though they ultimately
        // need converted to IntPtr.
        public static class CursorResourceId
        {
            public const int IDC_ARROW = 32512;
            public const int IDC_IBEAM = 32513;
            public const int IDC_WAIT = 32514;
            public const int IDC_CROSS = 32515;
            public const int IDC_SIZEALL = 32646;
            public const int IDC_SIZENWSE = 32642;
            public const int IDC_SIZENESW = 32643;
            public const int IDC_SIZEWE = 32644;
            public const int IDC_SIZENS = 32645;
            public const int IDC_UPARROW = 32516;
            public const int IDC_NO = 32648;
            public const int IDC_HAND = 32649;
            public const int IDC_APPSTARTING = 32650;
            public const int IDC_HELP = 32651;
        }

        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern IntPtr LoadCursorW(IntPtr hInstance, IntPtr lpCursorName);
    }
}
