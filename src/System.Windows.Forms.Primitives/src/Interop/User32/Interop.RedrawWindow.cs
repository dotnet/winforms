// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        /// <summary>
        ///  <see cref="RedrawWindow(IntPtr, RECT*, IntPtr, RDW)"/> flags.
        /// </summary>
        [Flags]
        public enum RDW : uint
        {
            INVALIDATE = 0x0001,
            INTERNALPAINT = 0x0002,
            ERASE = 0x0004,
            VALIDATE = 0x0008,
            NOINTERNALPAINT = 0x0010,
            NOERASE = 0x0020,
            NOCHILDREN = 0x0040,
            ALLCHILDREN = 0x0080,
            UPDATENOW = 0x0100,
            ERASENOW = 0x0200,
            FRAME = 0x0400,
            NOFRAME = 0x0800,
        }

        [DllImport(Libraries.User32, ExactSpelling = true)]
        public unsafe static extern BOOL RedrawWindow(IntPtr hWnd, RECT *lprcUpdate, IntPtr hrgnUpdate, RDW flags);

        public unsafe static BOOL RedrawWindow(HandleRef hWnd, RECT *lprcUpdate, IntPtr hrgnUpdate, RDW flags)
        {
            BOOL result = RedrawWindow(hWnd.Handle, lprcUpdate, hrgnUpdate, flags);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }

        public unsafe static BOOL RedrawWindow(HandleRef hWnd, RECT* lprcUpdate, Gdi32.HRGN hrgnUpdate, RDW flags)
            => RedrawWindow(hWnd, lprcUpdate, hrgnUpdate.Handle, flags);
    }
}
