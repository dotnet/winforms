// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Memory;

namespace System.Private.Windows.Ole;

/// <summary>
///  Simple scope for writing to HGLOBAL memory.
/// </summary>
internal unsafe ref struct GlobalBuffer
{
    private void* _pointer;
    private Span<byte> _buffer;
    private HGLOBAL _hglobal;

    public GlobalBuffer(HGLOBAL hglobal, uint length)
    {
        if (hglobal.IsNull)
        {
            Status = HRESULT.E_INVALIDARG;
            return;
        }

        _hglobal = PInvokeCore.GlobalReAlloc(
            hglobal,
            length,
            (uint)GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE);

        if (_hglobal.IsNull)
        {
            Status = HRESULT.E_OUTOFMEMORY;
            return;
        }

        _pointer = PInvokeCore.GlobalLock(_hglobal);
        if (_pointer is null)
        {
            Status = HRESULT.E_OUTOFMEMORY;
        }

        _buffer = new((byte*)_pointer, (int)length);
    }

    public HRESULT Status { get; private set; } = HRESULT.S_OK;
    public readonly void* Pointer => _pointer;

    public readonly Span<byte> AsSpan() => _buffer;

    public readonly Span<char> AsCharSpan() => MemoryMarshal.Cast<byte, char>(_buffer);

    public void Dispose()
    {
        if (!_hglobal.IsNull)
        {
            PInvokeCore.GlobalUnlock(_hglobal);
            _buffer = default;
            _pointer = null;
            _hglobal = HGLOBAL.Null;
        }
    }
}
