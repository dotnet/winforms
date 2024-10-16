// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace Windows.Win32;

internal static partial class PInvoke
{
    [SkipLocalsInit]
    public static unsafe string GetModuleFileNameLongPath(HINSTANCE hModule)
    {
        using BufferScope<char> buffer = new(stackalloc char[(int)PInvokeCore.MAX_PATH]);

        // Allocate increasingly larger portions of memory until successful or we hit short.maxvalue.
        while (true)
        {
            fixed (char* lpFilename = buffer)
            {
                int pathLength = (int)GetModuleFileName(hModule, lpFilename, (uint)buffer.Length);

                if (pathLength == 0)
                {
                    return string.Empty;
                }

                // If the length equals the buffer size we need to check to see if we were told the buffer was insufficient (it was trimmed)
                if (pathLength < buffer.Length)
                {
                    // Get return value and return buffer to array pool.
                    return buffer[..pathLength].ToString();
                }

                if (buffer.Length >= short.MaxValue)
                {
                    // Should never happen
                    Debug.Fail($"Module File Name is greater than {short.MaxValue}.");
                    return string.Empty;
                }

                buffer.EnsureCapacity(Math.Min(buffer.Length * 2, short.MaxValue));
            }
        }
    }
}
