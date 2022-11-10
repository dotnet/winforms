// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static unsafe string GetWindowText(HWND hWnd)
        {
            // Old implementation uses MAX_TITLE_LENGTH characters
            const int MAX_TITLE_LENGTH = 512;
            int length;
            Span<char> buffer = stackalloc char[MAX_TITLE_LENGTH];
            fixed (char* lpString = buffer)
            {
                length = GetWindowText(hWnd, (PWSTR)lpString, buffer.Length);
            }

            // If the window has no title bar or text, if the title bar is empty,
            // or if the window or control handle is invalid, the return value is zero
            return length == 0 ? string.Empty : new string(buffer[..length]);
        }
    }
}
