// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Kernel32
    {
        [DllImport(Libraries.Kernel32, ExactSpelling = true)]
        public static extern BOOL CloseHandle(IntPtr handle);

        public static BOOL CloseHandle(HandleRef handle)
        {
            BOOL result = CloseHandle(handle.Handle);
            GC.KeepAlive(handle.Wrapper);
            return result;
        }
    }
}
