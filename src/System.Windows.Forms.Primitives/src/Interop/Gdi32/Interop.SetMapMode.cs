// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32)]
        public static extern MM SetMapMode(HDC hdc, MM mode);

        public static MM SetMapMode(IHandle hdc, MM mode)
        {
            MM result = SetMapMode((HDC)hdc.Handle, mode);
            GC.KeepAlive(hdc);
            return result;
        }
    }
}
