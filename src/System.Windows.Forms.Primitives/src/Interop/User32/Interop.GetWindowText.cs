// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern int GetWindowTextLengthW(IntPtr hWnd);

        public static int GetWindowTextLengthW(HandleRef hWnd)
        {
            int result = GetWindowTextLengthW(hWnd.Handle);
            GC.KeepAlive(hWnd);
            return result;
        }

        [DllImport(Libraries.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static unsafe extern int GetWindowTextW(IntPtr hWnd, char* lpString, int nMaxCount);

        public static unsafe string GetWindowText(IntPtr hWnd)
        {
            int textLength = 0;

            while (true)
            {
                // GetWindowTextLengthW returns the length of the text not
                // including the null terminator.
                textLength = Math.Max(textLength, GetWindowTextLengthW(hWnd));

                // Use a buffer that has room for at least two additional chars
                // (one for the null terminator, and one to detect if the text length
                // has increased).
                char[] windowTitleBuffer = ArrayPool<char>.Shared.Rent(textLength + 2);
                string windowTitle;
                fixed (char* pWindowTitle = windowTitleBuffer)
                {
                    int actualTextLength = GetWindowTextW(hWnd, pWindowTitle, windowTitleBuffer.Length);

                    // The window text may have changed between calls.
                    // Keep looping until we get a buffer that can fit.
                    if (actualTextLength > windowTitleBuffer.Length - 2)
                    {
                        // We know the text is at least actualTextLength characters
                        // long, so use this as minimum value for the next iteration.
                        textLength = actualTextLength;
                        ArrayPool<char>.Shared.Return(windowTitleBuffer);
                        continue;
                    }

                    windowTitle = new string(pWindowTitle, 0, actualTextLength);
                }

                ArrayPool<char>.Shared.Return(windowTitleBuffer);
                return windowTitle;
            }
        }

        public static string GetWindowText(HandleRef hWnd)
        {
            string result = GetWindowText(hWnd.Handle);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
