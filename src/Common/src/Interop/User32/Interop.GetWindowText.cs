// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    public static partial class User32
    {
        [DllImport(Libraries.User32)]
        public static extern int GetWindowTextLengthW(HandleRef hWnd);

        [DllImport(Libraries.User32, CharSet = CharSet.Unicode)]
        private static unsafe extern int GetWindowTextW(HandleRef hWnd, char *lpString, int nMaxCount);

        public static unsafe string GetWindowText(HandleRef hWnd)
        {
            int textLength = GetWindowTextLengthW(hWnd) + 2;
            char[] windowTitleBuffer = ArrayPool<char>.Shared.Rent(textLength);
            string windowTitle;
            fixed (char *pWindowTitle = windowTitleBuffer)
            {
                int actualTextLength = GetWindowTextW(hWnd, pWindowTitle, textLength);
                windowTitle = new string(pWindowTitle, 0, actualTextLength);
            }
            
            ArrayPool<char>.Shared.Return(windowTitleBuffer);
            return windowTitle;
        }
    }
}
