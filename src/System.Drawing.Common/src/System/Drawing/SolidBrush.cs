// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Internal;

namespace System.Drawing;

public sealed unsafe class SolidBrush : Brush, ISystemColorTracker
{
    // GDI+ doesn't understand system colors, so we need to cache the value here.
    private Color _color = Color.Empty;
    private bool _immutable;

    public SolidBrush(Color color)
    {
        _color = color;

        GpSolidFill* nativeBrush;
        PInvokeGdiPlus.GdipCreateSolidFill((ARGB)_color, &nativeBrush).ThrowIfFailed();
        SetNativeBrushInternal((GpBrush*)nativeBrush);

        if (_color.IsSystemColor)
        {
            SystemColorTracker.Add(this);
        }
    }

    internal SolidBrush(Color color, bool immutable) : this(color) => _immutable = immutable;

    internal SolidBrush(GpSolidFill* nativeBrush)
    {
        Debug.Assert(nativeBrush is not null, "Initializing native brush with null.");
        SetNativeBrushInternal((GpBrush*)nativeBrush);
    }

    public override object Clone()
    {
        GpBrush* clonedBrush;
        PInvokeGdiPlus.GdipCloneBrush(NativeBrush, &clonedBrush).ThrowIfFailed();
        GC.KeepAlive(this);

        // Clones of immutable brushes are not immutable.
        return new SolidBrush((GpSolidFill*)clonedBrush);
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            _immutable = false;
        }
        else if (_immutable)
        {
            throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Brush"));
        }

        base.Dispose(disposing);
    }

    public Color Color
    {
        get
        {
            if (_color == Color.Empty)
            {
                ARGB color;
                PInvokeGdiPlus.GdipGetSolidFillColor((GpSolidFill*)NativeBrush, (uint*)&color).ThrowIfFailed();
                GC.KeepAlive(this);
                _color = color;
            }

            // GDI+ doesn't understand system colors, so we can't use GdipGetSolidFillColor in the general case.
            return _color;
        }
        set
        {
            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Brush"));
            }

            if (_color != value)
            {
                Color oldColor = _color;
                InternalSetColor(value);

                // NOTE: We never remove brushes from the active list, so if someone is
                // changing their brush colors a lot, this could be a problem.
                if (value.IsSystemColor && !oldColor.IsSystemColor)
                {
                    SystemColorTracker.Add(this);
                }
            }
        }
    }

    // Sets the color even if the brush is considered immutable.
    private void InternalSetColor(Color value)
    {
        PInvokeGdiPlus.GdipSetSolidFillColor((GpSolidFill*)NativeBrush, (ARGB)value).ThrowIfFailed();
        GC.KeepAlive(this);
        _color = value;
    }

    void ISystemColorTracker.OnSystemColorChanged()
    {
        if (NativeBrush is not null)
        {
            InternalSetColor(_color);
        }
    }
}
