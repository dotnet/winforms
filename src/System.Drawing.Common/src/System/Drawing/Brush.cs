// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Drawing;

public abstract unsafe class Brush : MarshalByRefObject, ICloneable, IDisposable
{
    // Handle to native GDI+ brush object to be used on demand.
    private GpBrush* _nativeBrush;

    public abstract object Clone();

    protected internal void SetNativeBrush(IntPtr brush) => SetNativeBrushInternal((GpBrush*)brush);
    internal void SetNativeBrushInternal(GpBrush* brush) => _nativeBrush = brush;

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    internal GpBrush* NativeBrush => _nativeBrush;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_nativeBrush is not null)
        {
            Status status = !Gdip.Initialized ? Status.Ok : PInvokeGdiPlus.GdipDeleteBrush(_nativeBrush);
            _nativeBrush = null;
            Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
        }
    }

    ~Brush() => Dispose(disposing: false);
}
