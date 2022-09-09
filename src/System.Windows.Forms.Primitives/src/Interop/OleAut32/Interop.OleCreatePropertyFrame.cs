// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        [DllImport(Libraries.Oleaut32, CharSet = CharSet.Unicode)]
        public static unsafe extern int OleCreatePropertyFrame(
            IntPtr hwndOwner,
            int x,
            int y,
            string caption,
            uint cObjects,
            IntPtr* pobjs,
            uint cPages,
            Guid* pClsid,
            PInvoke.LCID lcid,
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
            PInvoke.LCID lcid,
            uint dwReserved,
            IntPtr pvReserved)
        {
            OleCreatePropertyFrame(hwndOwner.Handle, x, y, caption, cObjects, pobjs, cPages, pClsid, lcid, dwReserved, pvReserved);
            GC.KeepAlive(hwndOwner.Wrapper);
        }
    }
}
