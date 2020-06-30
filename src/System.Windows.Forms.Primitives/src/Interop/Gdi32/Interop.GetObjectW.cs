// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, ExactSpelling = true)]
        private unsafe static extern int GetObjectW(HGDIOBJ h, int c, void* pv);

        public unsafe static bool GetObjectW<T>(HGDIOBJ h, out T @object) where T : unmanaged
        {
            // HGDIOBJ isn't technically correct, but close enough to filter out bigger mistakes (HWND, etc.).

            @object = default;
            fixed (void* pv = &@object)
            {
                return GetObjectW(h, sizeof(T), pv) != 0;
            }
        }

        public static bool GetObjectW<T>(HandleRef h, out T @object) where T : unmanaged
        {
            bool result = GetObjectW((HGDIOBJ)h.Handle, out @object);
            GC.KeepAlive(h.Wrapper);
            return result;
        }
    }
}
