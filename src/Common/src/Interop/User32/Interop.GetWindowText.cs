// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32)]
        public static extern int GetWindowTextLengthW(HandleRef hWnd);

        [DllImport(Libraries.User32, CharSet = CharSet.Unicode)]
        private static unsafe extern int GetWindowTextW(HandleRef hWnd, char* lpString, int nMaxCount);

        public static unsafe string GetWindowText(HandleRef hWnd)
        {
            while (true)
            {
                // GetWindowTextLengthW returns the length of the text not
                // including the null terminator.
                int textLengthWithNullTerminator = GetWindowTextLengthW(hWnd) + 1;
                char[] windowTitleBuffer = ArrayPool<char>.Shared.Rent(textLengthWithNullTerminator);
                string windowTitle;
                fixed (char* pWindowTitle = windowTitleBuffer)
                {
                    int actualTextLength = GetWindowTextW(hWnd, pWindowTitle, textLengthWithNullTerminator + 1);

                    // The window text may have changed between calls.
                    // Keep looping until we get a buffer that can fit.
                    if (actualTextLength > textLengthWithNullTerminator)
                    {
                        ArrayPool<char>.Shared.Return(windowTitleBuffer);
                        continue;
                    }

                    windowTitle = new string(pWindowTitle, 0, actualTextLength);
                }

                ArrayPool<char>.Shared.Return(windowTitleBuffer);
                return windowTitle;
            }
        }
    }
}
