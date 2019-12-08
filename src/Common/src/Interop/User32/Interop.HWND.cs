// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        public static class HWND
        {
            public static readonly IntPtr MESSAGE = (IntPtr)(-3);
            public static readonly IntPtr NOTOPMOST = (IntPtr)(-2);
            public static readonly IntPtr TOPMOST = (IntPtr)(-1);
            public static readonly IntPtr DESKTOP = (IntPtr)0;
            public static readonly IntPtr TOP = (IntPtr)0;
            public static readonly IntPtr BOTTOM = (IntPtr)1;
            public static readonly IntPtr BROADCAST = (IntPtr)0xffff;
        }
    }
}
