// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static HRESULT RevokeDragDrop<T>(T hwnd) where T : IHandle<HWND>
        {
            HRESULT result = RevokeDragDrop(hwnd.Handle);
            GC.KeepAlive(hwnd.Wrapper);
            return result;
        }
    }
}
