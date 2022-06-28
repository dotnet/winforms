// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [LibraryImport(Libraries.User32)]
        public static partial IntPtr GetAncestor(IntPtr hwnd, GA flags);

        public static IntPtr GetAncestor(IHandle hwnd, GA flags)
        {
            IntPtr result = GetAncestor(hwnd.Handle, flags);
            GC.KeepAlive(hwnd);
            return result;
        }
    }
}
