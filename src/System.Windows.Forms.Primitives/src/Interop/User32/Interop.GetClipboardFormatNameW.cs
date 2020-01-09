// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        private static unsafe extern int GetClipboardFormatNameW(uint format, char* lpszFormatName, int cchMaxCount);

        public static unsafe string? GetClipboardFormatNameW(uint format)
        {
            // The max length of the name of clipboard formats is equal to the max length
            // of a Win32 Atom of 255 chars. An additional null terminator character is added,
            // giving a required capacity of 256 chars.
            Span<char> formatName = stackalloc char[256];
            fixed (char* pFormatName = formatName)
            {
                int length = GetClipboardFormatNameW(format, pFormatName, 256);
                if (length == 0)
                {
                    return null;
                }

                return formatName.Slice(0, length).ToString();
            }
        }
    }
}
