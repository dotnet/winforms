﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class UiaCore
    {
        [DllImport(Libraries.UiaCore, CharSet = CharSet.Unicode)]
        public static extern nint UiaReturnRawElementProvider(HWND hwnd, nint wParam, nint lParam, IRawElementProviderSimple? el);

        public static nint UiaReturnRawElementProvider(IHandle<HWND> hwnd, WPARAM wParam, LPARAM lParam, IRawElementProviderSimple? el)
        {
            nint result = UiaReturnRawElementProvider(hwnd.Handle, (nint)wParam, (nint)lParam, el);
            GC.KeepAlive(hwnd.Wrapper);
            return result;
        }
    }
}
