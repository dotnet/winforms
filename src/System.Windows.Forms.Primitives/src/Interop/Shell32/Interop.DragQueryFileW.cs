// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [DllImport(Libraries.Shell32, ExactSpelling = true, EntryPoint = "DragQueryFileW", CharSet = CharSet.Unicode)]
        private static extern unsafe uint DragQueryFileWInternal(IntPtr hDrop, uint iFile, char* lpszFile, uint cch);

        public static unsafe uint DragQueryFileW(IntPtr hDrop, uint iFile, ref string? lpszFile)
        {
            if (lpszFile is null || iFile == 0xFFFFFFFF)
            {
                lpszFile = string.Empty;
                return DragQueryFileWInternal(hDrop, iFile, null, 0);
            }

            uint resultValue = 0;
            uint capacity;

            // passing null for buffer will return actual number of characters in the file name.
            // So, one extra call would be suffice to avoid while loop in case of long path.
            if ((capacity = DragQueryFileWInternal(hDrop, iFile, null, 0)) < Kernel32.MAX_UNICODESTRING_LEN)
            {
                Span<char> charSpan = new char[(int)capacity];
                fixed (char* pCharSpan = charSpan)
                {
                    resultValue = DragQueryFileWInternal(hDrop, iFile, pCharSpan, capacity);
                }

                // Set lpszFile to the buffer's data.
                lpszFile = charSpan.Slice(0, (int)resultValue).SliceAtFirstNull().ToString();
            }

            return resultValue;
        }
    }
}
