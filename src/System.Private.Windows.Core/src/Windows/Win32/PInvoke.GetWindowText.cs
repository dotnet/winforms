// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.CompilerServices;
#if DEBUG
using System.Runtime.InteropServices;
#endif

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    [SkipLocalsInit]
    public static unsafe string GetWindowText<T>(T hWnd) where T : IHandle<HWND>
    {
        int textLength = 0;

        using BufferScope<char> buffer = new(stackalloc char[128]);

        while (true)
        {
            // GetWindowTextLength returns the length of the text not including the null terminator.
            int newTextLength = GetWindowTextLength(hWnd.Handle);

            if (newTextLength == 0)
            {
                // The window has no text. Return an empty string.
#if DEBUG
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    Debug.Fail($"GetWindowTextLength failed. Error: {new Win32Exception(error).Message}");
                }
#endif
                return string.Empty;
            }

            textLength = Math.Max(textLength, newTextLength);

            // Use a buffer that has room for at least two additional chars (one for the null terminator,
            // and one to detect if the text length has increased).
            buffer.EnsureCapacity(textLength + 2);
            fixed (char* b = buffer)
            {
                int actualTextLength = GetWindowText(hWnd.Handle, b, buffer.Length);

                if (actualTextLength == 0)
                {
                    // Failed to get the text. Return an empty string.

                    Debug.Fail($"GetWindowText failed. Error: {new Win32Exception().Message}");
                    return string.Empty;
                }

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
