// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    internal struct ToolTipBuffer : IDisposable
    {
        private IntPtr _buffer;
        private int _bufferSize;

        public IntPtr Buffer => _buffer;

        public unsafe void SetText(string text)
        {
            if (text is null)
            {
                text = string.Empty;
            }

            int oldTextLength = _bufferSize - 1;
            if (oldTextLength < text.Length)
            {
                Marshal.FreeHGlobal(_buffer);
                _buffer = IntPtr.Zero;
                _bufferSize = text.Length + 1;
            }

            if (_buffer == IntPtr.Zero)
            {
                _buffer = Marshal.AllocHGlobal(_bufferSize * sizeof(char));
            }

            // Copy the string to the allocated memory.
            ReadOnlySpan<char> textSpan = text;
            var destinationSpan = new Span<char>((void*)_buffer, _bufferSize);
            textSpan.CopyTo(destinationSpan);
            destinationSpan[text.Length] = '\0';
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_buffer);
            _buffer = IntPtr.Zero;
        }
    }
}
