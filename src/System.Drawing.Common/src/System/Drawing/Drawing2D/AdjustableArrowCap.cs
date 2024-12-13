// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public sealed unsafe partial class AdjustableArrowCap : CustomLineCap
{
    internal AdjustableArrowCap(GpCustomLineCap* nativeCap) : base(nativeCap) { }

    public AdjustableArrowCap(float width, float height) : this(width, height, true) { }

    public AdjustableArrowCap(float width, float height, bool isFilled)
    {
        GpAdjustableArrowCap* nativeCap;
        PInvokeGdiPlus.GdipCreateAdjustableArrowCap(height, width, isFilled, &nativeCap).ThrowIfFailed();
        SetNativeLineCap((GpCustomLineCap*)nativeCap);
    }

    private GpAdjustableArrowCap* NativeArrowCap => (GpAdjustableArrowCap*)_nativeCap;

    public float Height
    {
        get
        {
            float height;
            PInvokeGdiPlus.GdipGetAdjustableArrowCapHeight(NativeArrowCap, &height).ThrowIfFailed();
            GC.KeepAlive(this);
            return height;
        }
        set
        {
            PInvokeGdiPlus.GdipSetAdjustableArrowCapHeight(NativeArrowCap, value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public float Width
    {
        get
        {
            float width;
            PInvokeGdiPlus.GdipGetAdjustableArrowCapWidth(NativeArrowCap, &width).ThrowIfFailed();
            GC.KeepAlive(this);
            return width;
        }
        set
        {
            PInvokeGdiPlus.GdipSetAdjustableArrowCapWidth(NativeArrowCap, value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public float MiddleInset
    {
        get
        {
            float middleInset;
            PInvokeGdiPlus.GdipGetAdjustableArrowCapMiddleInset(NativeArrowCap, &middleInset).ThrowIfFailed();
            GC.KeepAlive(this);
            return middleInset;
        }
        set
        {
            PInvokeGdiPlus.GdipSetAdjustableArrowCapMiddleInset(NativeArrowCap, value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public bool Filled
    {
        get
        {
            BOOL isFilled;
            PInvokeGdiPlus.GdipGetAdjustableArrowCapFillState(NativeArrowCap, &isFilled).ThrowIfFailed();
            GC.KeepAlive(this);
            return isFilled;
        }
        set
        {
            PInvokeGdiPlus.GdipSetAdjustableArrowCapFillState(NativeArrowCap, value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }
}
