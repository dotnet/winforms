// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using static Interop.Kernel32;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        [DllImport(Libraries.Oleaut32, ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        public unsafe static extern void OleCreatePropertyFrame(
            IntPtr hwndOwner,
            int x,
            int y,
            string caption,
            uint cObjects,
            IntPtr* pobjs,
            uint cPages,
            Guid* pClsid,
            LCID lcid,
            uint dwReserved,
            IntPtr pvReserved);

        public unsafe static void OleCreatePropertyFrame(
            HandleRef hwndOwner,
            int x,
            int y,
            string caption,
            uint cObjects,
            IntPtr* pobjs,
            uint cPages,
            Guid* pClsid,
            LCID lcid,
            uint dwReserved,
            IntPtr pvReserved)
        {
            OleCreatePropertyFrame(hwndOwner.Handle, x, y, caption, cObjects, pobjs, cPages, pClsid, lcid, dwReserved, pvReserved);
            GC.KeepAlive(hwndOwner.Wrapper);
        }
    }
}
