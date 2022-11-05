// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Runtime.InteropServices;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static unsafe string GetModuleFileNameLongPath(HINSTANCE hModule)
        {
            char[] buffer;
            int bufferSize = 4096;
            // Allocate increasingly larger portions of memory until successful or we hit short.maxvalue.
            for (int i = 1; bufferSize <= short.MaxValue; i++, bufferSize = 4096 * i)
            {
                buffer = ArrayPool<char>.Shared.Rent(bufferSize);
                uint pathLength;
                fixed (char* lpFilename = buffer)
                {
                    pathLength = GetModuleFileName(hModule, lpFilename, (uint)buffer.Length);
                }

                if (pathLength == 0)
                {
                    ArrayPool<char>.Shared.Return(buffer);
                    return string.Empty;
                }

                if (pathLength > 0 && (pathLength < buffer.Length || (WIN32_ERROR)Marshal.GetLastWin32Error() != WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER))
                {
                    // Get return value and return buffer to array pool.
                    string returnValue = new string(buffer, 0, (int)pathLength);
                    ArrayPool<char>.Shared.Return(buffer);
                    return returnValue;
                }

                // buffer was too small, return to array pool.
                ArrayPool<char>.Shared.Return(buffer);
            }

            return string.Empty;
        }
    }
}
