// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        public static extern HRESULT RevokeDragDrop(IntPtr hwnd);

        public static HRESULT RevokeDragDrop(HandleRef hwnd)
        {
            HRESULT result = RevokeDragDrop(hwnd.Handle);
            GC.KeepAlive(hwnd.Wrapper);
            return result;
        }
    }
}
