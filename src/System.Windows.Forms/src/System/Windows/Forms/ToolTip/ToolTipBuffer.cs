// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal struct ToolTipBuffer
{
    private char[]? _buffer;

    public unsafe IntPtr Buffer
    {
        get
        {
            fixed (char* c = _buffer)
            {
                return (IntPtr)c;
            }
        }
    }

    public void SetText(string? text)
    {
        text ??= string.Empty;

        if (_buffer is null || _buffer.Length < text.Length + 1)
        {
            _buffer = GC.AllocateUninitializedArray<char>(text.Length + 1, pinned: true);
        }

        // Copy the string to the allocated memory.
        text.AsSpan().CopyTo(_buffer);
        _buffer[text.Length] = '\0';
    }
}
