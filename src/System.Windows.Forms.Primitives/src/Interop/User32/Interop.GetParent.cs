// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [LibraryImport(Libraries.User32)]
        public static partial IntPtr GetParent(IntPtr hWnd);

        public static IntPtr GetParent(IHandle hWnd)
        {
            IntPtr result = GetParent(hWnd.Handle);
            GC.KeepAlive(hWnd);
            return result;
        }

        public static IntPtr GetParent(HandleRef hWnd)
        {
            IntPtr result = GetParent(hWnd.Handle);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
