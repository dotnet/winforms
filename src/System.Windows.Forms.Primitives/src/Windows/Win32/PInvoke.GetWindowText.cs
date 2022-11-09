// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Buffers;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        //const int MAX_TITLE_LENGTH = 511;

        public static unsafe string GetWindowText(HWND hWnd)
        {
            int length = GetWindowTextLength(hWnd);

            // If the window has no text, the return value is zero.
            if (length == 0)
            {
                return string.Empty;
            }

            // Stackalloc for smaller values
            if (length <= 1024)
            {
                Span<char> buffer = stackalloc char[length + 1];
                fixed (char* lpString = buffer)
                {
                    length = GetWindowText(hWnd, (PWSTR)lpString, buffer.Length);
                }

                // If the window has no title bar or text, if the title bar is empty,
                // or if the window or control handle is invalid, the return value is zero
                if (length == 0)
                {
                    return string.Empty;
                }
            }

            // Use arraypool for larger than 1024 characters.
            char[] lBuffer;
            lBuffer = ArrayPool<char>.Shared.Rent(length + 1);
            int pathLength;
            fixed (char* lpString = lBuffer)
            {
                pathLength = GetWindowText(hWnd, (PWSTR)lpString, lBuffer.Length);
            }

            // If the window has no title bar or text, if the title bar is empty,
            // or if the window or control handle is invalid, the return value is zero
            if (pathLength == 0)
            {
                ArrayPool<char>.Shared.Return(lBuffer);
                return string.Empty;
            }

            // Get return value and return buffer to array pool.
            string returnValue = new string(lBuffer, 0, pathLength);
            ArrayPool<char>.Shared.Return(lBuffer);
            return returnValue;
        }
    }
}
