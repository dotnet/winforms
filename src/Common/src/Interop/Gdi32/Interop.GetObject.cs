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
        private unsafe static extern int GetObjectW(IntPtr hObject, int c, void* pv);

        public unsafe static bool GetObject<T>(IntPtr hObject, out T @object) where T : unmanaged
        {
            @object = default;
            fixed (void* pv = &@object)
            {
                return GetObjectW(hObject, sizeof(T), pv) != 0;
            }
        }
    }
}
