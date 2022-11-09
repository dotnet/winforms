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
            Span<char> buffer = stackalloc char[MAX_PATH];
            uint pathLength;
            fixed (char* lpFilename = buffer)
            {
                pathLength = GetModuleFileName(hModule, lpFilename, (uint)buffer.Length);
            }

            if (pathLength == 0)
            {
                return string.Empty;
            }

            if (pathLength < buffer.Length - 1)
            {
                return new string(buffer[..(int)pathLength]);
            }

            char[] lbuffer;
            int bufferSize = 4096;
            // Allocate increasingly larger portions of memory until successful or we hit short.maxvalue.
            for (int i = 1; bufferSize <= short.MaxValue; i++, bufferSize = 4096 * i)
            {
                lbuffer = ArrayPool<char>.Shared.Rent(bufferSize);
                fixed (char* lpFilename = lbuffer)
                {
                    pathLength = GetModuleFileName(hModule, lpFilename, (uint)lbuffer.Length);
                }

                if (pathLength == 0)
                {
                    ArrayPool<char>.Shared.Return(lbuffer);
                    return string.Empty;
                }

                // If the length equals the buffer size we need to check to see if we were told the buffer was insufficient (it was trimmed)
                if (pathLength < lbuffer.Length - 1)
                {
                    // Get return value and return buffer to array pool.
                    string returnValue = new string(lbuffer, 0, (int)pathLength);
                    ArrayPool<char>.Shared.Return(lbuffer);
                    return returnValue;
                }

                // buffer was too small, return to array pool.
                ArrayPool<char>.Shared.Return(lbuffer);
            }

            return string.Empty;
        }
    }
}
