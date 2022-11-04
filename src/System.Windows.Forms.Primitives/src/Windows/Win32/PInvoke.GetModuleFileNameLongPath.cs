// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static unsafe string GetModuleFileNameLongPath(HINSTANCE hModule)
        {
            char[] buffer;
            int bufferSize = 4096;
            // Allocate increasingly larger portions of memory until successful or we hit short.maxvalue
            for (int i = 1; bufferSize <= short.MaxValue; i++, bufferSize = 4096 * i)
            {
                buffer = ArrayPool<char>.Shared.Rent(bufferSize);
                try
                {
                    uint pathLength;
                    fixed (char* lpFilename = buffer)
                    {
                        pathLength = GetModuleFileName(hModule, lpFilename, (uint)buffer.Length);
                    }

                    bool isBufferTooSmall = (WIN32_ERROR)Marshal.GetLastWin32Error() == WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER;
                    if (pathLength > 0 && (pathLength < buffer.Length || Marshal.GetLastWin32Error() != INSUFFICENT_BUFFER))
                    {
                        return new string(buffer, 0, (int)pathLength);
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(buffer);
                }

                // Double check that the buffer is not insanely big
                Debug.Assert(bufferSize <= int.MaxValue / 2, "Buffer size approaching int.MaxValue");
            }

            return string.Empty;
        }
    }
}
