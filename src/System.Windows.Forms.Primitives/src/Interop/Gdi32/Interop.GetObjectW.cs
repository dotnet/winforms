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
        private unsafe static extern int GetObjectW(IntPtr h, int c, void* pv);

        public unsafe static bool GetObjectW<T>(IntPtr h, out T @object) where T : unmanaged
        {
            @object = default;
            fixed (void* pv = &@object)
            {
                return GetObjectW(h, sizeof(T), pv) != 0;
            }
        }

        public static bool GetObjectW<T>(HandleRef h, out T @object) where T : unmanaged
        {
            bool result = GetObjectW(h.Handle, out @object);
            GC.KeepAlive(h.Wrapper);
            return result;
        }
    }
}
