// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class UiaCore
    {
        [DllImport(Libraries.UiaCore, ExactSpelling = true)]
        public static extern HRESULT UiaHostProviderFromHwnd(IntPtr hwnd, out IRawElementProviderSimple ppProvider);

        public static HRESULT UiaHostProviderFromHwnd(HandleRef hwnd, out IRawElementProviderSimple ppProvider)
        {
            HRESULT result = UiaHostProviderFromHwnd(hwnd.Handle, out ppProvider);
            GC.KeepAlive(hwnd.Wrapper);
            return result;
        }
    }
}
