// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;

namespace Windows.Win32.Graphics.Gdi;

/// <summary>
///  Buffer for <see cref="ARGB"/> values. Uses the stack for buffer sizes up to 16. Use in a <see langword="using"/>
///  statement.
/// </summary>
internal unsafe ref struct ArgbBuffer
{
    private const int StackSpace = 16;

    // Can't create a fixed ARGB
    private fixed uint _stackBuffer[StackSpace];

    private BufferScope<ARGB> _bufferScope;

    public ArgbBuffer(int length)
    {
        fixed (void* s = _stackBuffer)
        {
            _bufferScope = new BufferScope<ARGB>(new Span<ARGB>(s, StackSpace), length);
        }
    }

    public ArgbBuffer(Span<Color> colors) : this(colors.Length)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            _bufferScope[i] = colors[i];
        }
    }

    public readonly ref uint GetPinnableReference() =>
        ref Unsafe.As<ARGB, uint>(ref _bufferScope.GetPinnableReference());

    public readonly Color[] ToColorArray(int length) => ARGB.ToColorArray(_bufferScope[..length]);

    public void Dispose() => _bufferScope.Dispose();
}
