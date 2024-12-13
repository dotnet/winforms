// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public sealed unsafe class HatchBrush : Brush
{
    public HatchBrush(HatchStyle hatchstyle, Color foreColor)
        : this(hatchstyle, foreColor, Color.FromArgb(unchecked((int)0xff000000)))
    {
    }

    public HatchBrush(HatchStyle hatchstyle, Color foreColor, Color backColor)
    {
        if (hatchstyle is < HatchStyle.Min or > HatchStyle.SolidDiamond)
        {
            throw new ArgumentException(SR.Format(SR.InvalidEnumArgument, nameof(hatchstyle), hatchstyle, nameof(HatchStyle)), nameof(hatchstyle));
        }

        GpHatch* nativeBrush;
        PInvokeGdiPlus.GdipCreateHatchBrush((GdiPlus.HatchStyle)hatchstyle, (ARGB)foreColor, (ARGB)backColor, &nativeBrush).ThrowIfFailed();
        SetNativeBrushInternal((GpBrush*)nativeBrush);
    }

    internal HatchBrush(GpHatch* nativeBrush)
    {
        Debug.Assert(nativeBrush is not null, "Initializing native brush with null.");
        SetNativeBrushInternal((GpBrush*)nativeBrush);
    }

    public override object Clone()
    {
        GpBrush* clonedBrush;
        PInvokeGdiPlus.GdipCloneBrush(NativeBrush, &clonedBrush).ThrowIfFailed();
        GC.KeepAlive(this);
        return new HatchBrush((GpHatch*)clonedBrush);
    }

    public HatchStyle HatchStyle
    {
        get
        {
            GdiPlus.HatchStyle hatchStyle;
            PInvokeGdiPlus.GdipGetHatchStyle((GpHatch*)NativeBrush, &hatchStyle).ThrowIfFailed();
            GC.KeepAlive(this);
            return (HatchStyle)hatchStyle;
        }
    }

    public Color ForegroundColor
    {
        get
        {
            ARGB foregroundArgb;
            PInvokeGdiPlus.GdipGetHatchForegroundColor((GpHatch*)NativeBrush, (uint*)&foregroundArgb).ThrowIfFailed();
            GC.KeepAlive(this);
            return foregroundArgb;
        }
    }

    public Color BackgroundColor
    {
        get
        {
            ARGB backgroundArgb;
            PInvokeGdiPlus.GdipGetHatchBackgroundColor((GpHatch*)NativeBrush, (uint*)&backgroundArgb).ThrowIfFailed();
            GC.KeepAlive(this);
            return backgroundArgb;
        }
    }
}
