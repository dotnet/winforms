// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;

namespace Windows.Win32.Graphics.Gdi;

internal ref struct ArgbBuffer
{
    private BufferScope<ARGB> _bufferScope;

    public ArgbBuffer(int length, Span<ARGB> stackSpace = default) =>
        _bufferScope = new BufferScope<ARGB>(stackSpace, length);

    public ArgbBuffer(Span<Color> colors, Span<ARGB> stackSpace = default) =>
        _bufferScope = ARGB.FromColors(colors, stackSpace);

    public readonly ref uint GetPinnableReference() =>
        ref Unsafe.As<ARGB, uint>(ref _bufferScope.GetPinnableReference());

    public readonly Color[] ToColorArray(int length) => ARGB.ToColorArray(_bufferScope[..length]);

    public void Dispose() => _bufferScope.Dispose();
}
