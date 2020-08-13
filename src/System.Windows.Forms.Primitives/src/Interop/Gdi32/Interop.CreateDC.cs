// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <remarks>
        ///  Use <see cref="DeleteDC(HDC)"/> when finished with the returned DC.
        ///  Calling with ("DISPLAY", null, null, IntPtr.Zero) will retrieve a DC for the entire desktop.
        /// </remarks>
        [DllImport(Libraries.Gdi32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern HDC CreateDC(string lpszDriver, string? lpszDeviceName, string? lpszOutput, IntPtr devMode);
    }
}
