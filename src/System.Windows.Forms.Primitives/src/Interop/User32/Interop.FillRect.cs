// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32)]
        public static extern int FillRect(HDC hDC, ref RECT lprc, HBRUSH hbr);

        public static int FillRect(IHandle hDC, ref RECT lprc, HBRUSH hbr)
        {
            int result = FillRect((HDC)hDC.Handle, ref lprc, hbr);
            GC.KeepAlive(hDC);
            return result;
        }
    }
}
