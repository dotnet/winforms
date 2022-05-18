// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class UiaCore
    {
        [DllImport(Libraries.UiaCore, CharSet = CharSet.Unicode)]
        private static extern nint UiaReturnRawElementProvider(IntPtr hwnd, nint wParam, nint lParam, IntPtr el);

        public static nint UiaReturnRawElementProvider(IntPtr hwnd, nint wParam, nint lParam, IRawElementProviderSimple? el)
        {
            var providerPtr = el is null ? IntPtr.Zero : WinFormsComWrappers.Instance.GetComPointer(el, IID.IRawElementProviderSimple);
            return UiaReturnRawElementProvider(hwnd, wParam, lParam, providerPtr);
        }

        public static nint UiaReturnRawElementProvider(IHandle hwnd, nint wParam, nint lParam, IRawElementProviderSimple? el)
        {
            nint result = UiaReturnRawElementProvider(hwnd.Handle, wParam, lParam, el);
            GC.KeepAlive(hwnd);
            return result;
        }
    }
}
