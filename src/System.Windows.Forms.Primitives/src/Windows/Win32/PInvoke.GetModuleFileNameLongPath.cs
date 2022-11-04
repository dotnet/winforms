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
            const int INSUFFICIENT_BUFFER = 0x007A;
            char[] buffer;

            // Try increased buffer sizes if on longpath-enabled Windows
            for (int bufferSize = MAX_PATH; bufferSize <= MaxPath; bufferSize *= 2)
            {
                buffer = ArrayPool<char>.Shared.Rent(bufferSize);
                try
                {
                    uint pathLength;
                    fixed (char* lpFilename = buffer)
                    {
                        pathLength = GetModuleFileName(hModule, lpFilename, (uint)bufferSize);
                    }

                    bool isBufferTooSmall = Marshal.GetLastWin32Error() == INSUFFICIENT_BUFFER;
                    if (pathLength != 0 && !isBufferTooSmall && bufferSize <= int.MaxValue)
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
