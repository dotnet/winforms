// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [DllImport(Libraries.Shell32, ExactSpelling = true, EntryPoint = "DragQueryFileW", CharSet = CharSet.Unicode)]
        private static extern unsafe uint DragQueryFileWInternal(IntPtr hDrop, uint iFile, char* lpszFile, uint cch);

        public static unsafe uint DragQueryFileW(IntPtr hDrop, uint iFile, out string lpszFile)
        {
            if (iFile == 0xFFFFFFFF)
            {
                lpszFile = string.Empty;
                return DragQueryFileWInternal(hDrop, iFile, null, 0);
            }

            uint resultValue;

            // Allocating a buffer with stackalloc cannot be resized later, as such we must allocate
            // the buffer with the size of Kernel32.MAX_UNICODESTRING_LEN.
            Span<char> buf = stackalloc char[Kernel32.MAX_UNICODESTRING_LEN];
            fixed (char* valueChars = buf)
            {
                resultValue = DragQueryFileWInternal(hDrop, iFile, valueChars, (uint)buf.Length);
            }

            // Set lpszFile to the buffer's data.
            lpszFile = buf.Slice(0, (int)resultValue).SliceAtFirstNull().ToString();
            return resultValue;
        }
    }
}
