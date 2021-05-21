// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true, SetLastError = true)]
        public static extern int GetGuiResources(IntPtr hProcess, GR uiFlags);

        public enum GR : int
        {
            GDIOBJECTS          = 0,
            USEROBJECTS         = 1,
            GDIOBJECTS_PEAK     = 2,
            USEROBJECTS_PEAK    = 4,
        }
    }
}
