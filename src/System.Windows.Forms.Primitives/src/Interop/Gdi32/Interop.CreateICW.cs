// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern unsafe HDC CreateICW(char* lpszDriverName, char* lpszDeviceName, char* lpszOutput, IntPtr /*DEVMODE*/ lpInitData);

        public static unsafe HDC CreateICW(string lpszDriverName, string? lpszDeviceName, string? lpszOutput, IntPtr /*DEVMODE*/ lpInitData)
        {
            fixed (char* driverName = lpszDriverName)
            fixed (char* deviceName = lpszDeviceName)
            fixed (char* output = lpszOutput)
            {
                return CreateICW(driverName, deviceName, output, lpInitData);
            }
        }
    }
}
