// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public unsafe class CustomLineCap : MarshalByRefObject, ICloneable, IDisposable
{
    internal GpCustomLineCap* _nativeCap;

    private bool _disposed;

    // For subclass creation
    internal CustomLineCap() { }

    public CustomLineCap(GraphicsPath? fillPath, GraphicsPath? strokePath) : this(fillPath, strokePath, LineCap.Flat) { }

    public CustomLineCap(GraphicsPath? fillPath, GraphicsPath? strokePath, LineCap baseCap) : this(fillPath, strokePath, baseCap, 0) { }

    public CustomLineCap(GraphicsPath? fillPath, GraphicsPath? strokePath, LineCap baseCap, float baseInset)
    {
        GpCustomLineCap* lineCap;
        PInvoke.GdipCreateCustomLineCap(
            fillPath.Pointer(),
            strokePath.Pointer(),
            (GdiPlus.LineCap)baseCap,
            baseInset,
            &lineCap).ThrowIfFailed();

        SetNativeLineCap(lineCap);
    }

    internal CustomLineCap(GpCustomLineCap* lineCap) => SetNativeLineCap(lineCap);

    internal static CustomLineCap CreateCustomLineCapObject(GpCustomLineCap* cap)
    {
        GdiPlus.CustomLineCapType capType;
        Status status = PInvoke.GdipGetCustomLineCapType(cap, &capType);
        if (status != Status.Ok)
        {
            PInvoke.GdipDeleteCustomLineCap(cap);
            throw status.GetException();
        }

        switch (capType)
        {
            case GdiPlus.CustomLineCapType.CustomLineCapTypeDefault:
                return new CustomLineCap(cap);
            case GdiPlus.CustomLineCapType.CustomLineCapTypeAdjustableArrow:
                return new AdjustableArrowCap(cap);
        }

        PInvoke.GdipDeleteCustomLineCap(cap);
        throw Status.NotImplemented.GetException();
    }

    internal void SetNativeLineCap(GpCustomLineCap* handle)
    {
        if (handle is null)
            throw new ArgumentNullException(nameof(handle));

        _nativeCap = handle;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (_nativeCap is not null && Gdip.Initialized)
        {
            Status status = PInvoke.GdipDeleteCustomLineCap(_nativeCap);
            _nativeCap = null;
            Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
        }

        _disposed = true;
    }

    ~CustomLineCap() => Dispose(false);

    public object Clone() => CoreClone();

    internal virtual object CoreClone()
    {
        GpCustomLineCap* clonedCap;
        PInvoke.GdipCloneCustomLineCap(_nativeCap, &clonedCap).ThrowIfFailed();
        GC.KeepAlive(this);
        return CreateCustomLineCapObject(clonedCap);
    }

    public void SetStrokeCaps(LineCap startCap, LineCap endCap)
    {
        PInvoke.GdipSetCustomLineCapStrokeCaps(_nativeCap, (GdiPlus.LineCap)startCap, (GdiPlus.LineCap)endCap).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void GetStrokeCaps(out LineCap startCap, out LineCap endCap)
    {
        fixed (LineCap* sc = &startCap)
        fixed (LineCap* ec = &endCap)
        {
            PInvoke.GdipGetCustomLineCapStrokeCaps(_nativeCap, (GdiPlus.LineCap*)sc, (GdiPlus.LineCap*)ec).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public LineJoin StrokeJoin
    {
        get
        {
            LineJoin lineJoin;
            PInvoke.GdipGetCustomLineCapStrokeJoin(_nativeCap, (GdiPlus.LineJoin*)&lineJoin).ThrowIfFailed();
            GC.KeepAlive(this);
            return lineJoin;
        }
        set
        {
            PInvoke.GdipSetCustomLineCapStrokeJoin(_nativeCap, (GdiPlus.LineJoin)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public LineCap BaseCap
    {
        get
        {
            LineCap baseCap;
            PInvoke.GdipGetCustomLineCapBaseCap(_nativeCap, (GdiPlus.LineCap*)&baseCap).ThrowIfFailed();
            GC.KeepAlive(this);
            return baseCap;
        }
        set
        {
            PInvoke.GdipSetCustomLineCapBaseCap(_nativeCap, (GdiPlus.LineCap)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public float BaseInset
    {
        get
        {
            float inset;
            PInvoke.GdipGetCustomLineCapBaseInset(_nativeCap, &inset).ThrowIfFailed();
            GC.KeepAlive(this);
            return inset;
        }
        set
        {
            PInvoke.GdipSetCustomLineCapBaseInset(_nativeCap, value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public float WidthScale
    {
        get
        {
            float widthScale;
            PInvoke.GdipGetCustomLineCapWidthScale(_nativeCap, &widthScale).ThrowIfFailed();
            GC.KeepAlive(this);
            return widthScale;
        }
        set
        {
            PInvoke.GdipSetCustomLineCapWidthScale(_nativeCap, value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }
}
