// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32)]
        private static extern nint SetCursor(IntPtr hCursor);

        public static IntPtr SetCursor(IHandle? hCursor)
        {
            IntPtr result = SetCursor(hCursor?.Handle ?? IntPtr.Zero);
            GC.KeepAlive(hCursor);
            return result;
        }
    }
}
