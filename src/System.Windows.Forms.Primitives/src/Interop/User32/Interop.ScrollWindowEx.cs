// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32)]
        public static extern unsafe int ScrollWindowEx(
            IntPtr hWnd,
            int dx,
            int dy,
            RECT* prcScroll,
            RECT* prcClip,
            IntPtr hrgnUpdate,
            RECT* prcUpdate,
            ScrollSW flags);

        public static unsafe int ScrollWindowEx(
            IHandle hWnd,
            int dx,
            int dy,
            RECT* prcScroll,
            RECT* prcClip,
            IntPtr hrgnUpdate,
            RECT* prcUpdate,
            ScrollSW flags)
        {
            int result = ScrollWindowEx(hWnd.Handle, dx, dy, prcScroll, prcClip, hrgnUpdate, prcUpdate, flags);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}
