// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace Windows.Win32;

internal static partial class PInvoke
{
    [SkipLocalsInit]
    public static unsafe string GetWindowText<T>(T hWnd) where T : IHandle<HWND>
    {
        int textLength = 0;

        using BufferScope<char> buffer = new(stackalloc char[128]);

        while (true)
        {
            // GetWindowTextLength returns the length of the text not including the null terminator.
            textLength = Math.Max(textLength, GetWindowTextLength(hWnd.Handle));

            // Use a buffer that has room for at least two additional chars (one for the null terminator,
            // and one to detect if the text length has increased).
            buffer.EnsureCapacity(textLength + 2);
            fixed (char* b = buffer)
            {
                int actualTextLength = GetWindowText(hWnd.Handle, b, buffer.Length);

                // The window text may have changed between calls. Keep looping until we get a buffer that can fit.
                if (actualTextLength > buffer.Length - 2)
                {
                    // We know the text is at least actualTextLength characters
                    // long, so use this as minimum value for the next iteration.
                    textLength = actualTextLength;
                    continue;
                }

                GC.KeepAlive(hWnd.Wrapper);
                return buffer[..actualTextLength].ToString();
            }
        }
    }
}
