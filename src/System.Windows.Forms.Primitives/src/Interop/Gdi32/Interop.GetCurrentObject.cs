// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [LibraryImport(Libraries.Gdi32)]
        public static partial HGDIOBJ GetCurrentObject(HDC hdc, OBJ type);

        public static HGDIOBJ GetCurrentObject(IHandle hdc, OBJ type)
        {
            HGDIOBJ result = GetCurrentObject((HDC)hdc.Handle, type);
            GC.KeepAlive(hdc);
            return result;
        }
    }
}
