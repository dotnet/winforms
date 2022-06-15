// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [LibraryImport(Libraries.Gdi32)]
        public static partial R2 SetROP2(HDC hdc, R2 rop2);

        public static R2 SetROP2(IHandle hdc, R2 rop2)
        {
            R2 result = SetROP2((HDC)hdc.Handle, rop2);
            GC.KeepAlive(hdc);
            return result;
        }
    }
}
