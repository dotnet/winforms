// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum RedrawWindowOptions : uint
        {
            RDW_INVALIDATE = 0x0001,
            RDW_INTERNALPAINT = 0x0002,
            RDW_ERASE = 0x0004,
            RDW_VALIDATE = 0x0008,
            RDW_NOINTERNALPAINT = 0x0010,
            RDW_NOERASE = 0x0020,
            RDW_NOCHILDREN = 0x0040,
            RDW_ALLCHILDREN = 0x0080,
            RDW_UPDATENOW = 0x0100,
            RDW_ERASENOW = 0x0200,
            RDW_FRAME = 0x0400,
            RDW_NOFRAME = 0x0800,
        }

        [DllImport(Libraries.User32, ExactSpelling = true)]
        public unsafe static extern BOOL RedrawWindow(IntPtr hWnd, RECT *lprcUpdate, IntPtr hrgnUpdate, RedrawWindowOptions flags);

        public unsafe static BOOL RedrawWindow(HandleRef hWnd, RECT *lprcUpdate, IntPtr hrgnUpdate, RedrawWindowOptions flags)
        {
            BOOL result = RedrawWindow(hWnd.Handle, lprcUpdate, hrgnUpdate, flags);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }

        public unsafe static BOOL RedrawWindow(HandleRef hWnd, RECT *lprcUpdate, HandleRef hrgnUpdate, RedrawWindowOptions flags)
        {
            BOOL result = RedrawWindow(hWnd.Handle, lprcUpdate, hrgnUpdate.Handle, flags);
            GC.KeepAlive(hWnd.Wrapper);
            GC.KeepAlive(hrgnUpdate.Wrapper);
            return result;
        }
    }
}
