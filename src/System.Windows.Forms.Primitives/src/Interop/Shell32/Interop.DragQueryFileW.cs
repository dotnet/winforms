// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [DllImport(Libraries.Shell32, ExactSpelling = true, EntryPoint = "DragQueryFileW", CharSet = CharSet.Unicode)]
#pragma warning disable CA1838 // Avoid 'StringBuilder' parameters for P/Invokes
        private static extern uint DragQueryFileWInternal(IntPtr hDrop, uint iFile, StringBuilder? lpszFile, uint cch);
#pragma warning restore CA1838 // Avoid 'StringBuilder' parameters for P/Invokes

        public static uint DragQueryFileW(IntPtr hDrop, uint iFile, StringBuilder? lpszFile)
        {
            if (lpszFile is null || lpszFile.Capacity == 0 || iFile == 0xFFFFFFFF)
            {
                return DragQueryFileWInternal(hDrop, iFile, null, 0);
            }

            uint resultValue = 0;

            // iterating by allocating chunk of memory each time we find the length is not sufficient.
            // Performance should not be an issue for current MAX_PATH length due to this
            if ((resultValue = DragQueryFileWInternal(hDrop, iFile, lpszFile, (uint)lpszFile.Capacity)) == lpszFile.Capacity)
            {
                // passing null for buffer will return actual number of charectors in the file name.
                // So, one extra call would be suffice to avoid while loop in case of long path.
                uint capacity = DragQueryFileWInternal(hDrop, iFile, null, 0);
                if (capacity < Kernel32.MAX_UNICODESTRING_LEN)
                {
                    lpszFile.EnsureCapacity((int)capacity);
                    resultValue = DragQueryFileWInternal(hDrop, iFile, lpszFile, (uint)capacity);
                }
                else
                {
                    resultValue = 0;
                }
            }

            lpszFile.Length = (int)resultValue;
            return resultValue;
        }
    }
}
