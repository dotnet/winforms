// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Struct that helps ensure we build our <see cref="EventArgs"/> that wrap <see cref="DrawingEventArgs"/>
///  the same way.
/// </summary>
/// <remarks>
///  <para>
///   We should consider making this a base class for the event args that use this rather than a nested struct.
///   That would make things a little more robust, but would require API review as the class itself would have to
///   be public. The internal functionality can obviously still be internal.
///  </para>
/// </remarks>
internal partial class DrawingEventArgs
{
    private Graphics? _graphics;

    /// <summary>
    ///  DC (Display context) for obtaining the graphics object. Used to delay getting the graphics object until
    ///  absolutely necessary (for perf reasons)
    /// </summary>
    private readonly HDC _hdc;
    private HPALETTE _oldPalette;

    public DrawingEventArgs(
        Graphics graphics,
        Rectangle clipRect,
        DrawingEventFlags flags)
    {
        _graphics = graphics.OrThrowIfNull();
        _hdc = default;
        _oldPalette = default;
        CheckGraphicsForState(graphics, flags);
        ClipRectangle = clipRect;
        Flags = flags;
    }

    /// <summary>
    ///  Internal version of constructor for performance. We try to avoid getting the graphics object until needed.
    /// </summary>
    public DrawingEventArgs(
        HDC dc,
        Rectangle clipRect,
        DrawingEventFlags flags)
    {
        ArgumentValidation.ThrowIfNull(dc);

#if DEBUG
        OBJ_TYPE type = (OBJ_TYPE)PInvokeCore.GetObjectType(dc);
        Debug.Assert(type is OBJ_TYPE.OBJ_DC
            or OBJ_TYPE.OBJ_ENHMETADC
            or OBJ_TYPE.OBJ_MEMDC
            or OBJ_TYPE.OBJ_METADC);
#endif

        _hdc = dc;
        _graphics = null;
        _oldPalette = default;
        Flags = flags;
        ClipRectangle = clipRect;
    }

    internal DrawingEventFlags Flags { get; private set; }
    internal Rectangle ClipRectangle { get; }

    internal bool IsStateClean => !Flags.HasFlag(DrawingEventFlags.GraphicsStateUnclean);

    /// <summary>
    ///  Gets the HDC this event is connected to. If there is no associated HDC, or the GDI+ Graphics object has
    ///  been externally accessed (where it may have gotten a transform or clip) a null handle is returned.
    /// </summary>
    internal HDC HDC => IsStateClean ? default : _hdc;

    /// <summary>
    ///  Gets the <see cref="Graphics"/> object used to paint.
    /// </summary>
    internal Graphics Graphics
    {
        get
        {
            // If we're giving this out on the public API expect it to get a clip or transform applied.
            Flags |= DrawingEventFlags.GraphicsStateUnclean;
            return GetOrCreateGraphicsInternal();
        }
    }

    /// <summary>
    ///  For internal use to improve performance. DO NOT use this method if you modify the Graphics Clip or Transform.
    /// </summary>
    internal Graphics GetOrCreateGraphicsInternal(Action<Graphics>? creationAction = null)
    {
        if (_graphics is null)
        {
            Debug.Assert(!_hdc.IsNull);

            // We need to manually unset the palette here so this scope shouldn't be disposed
            var paletteScope = _hdc.HalftonePalette(
                forceBackground: false,
                realizePalette: false);

            GC.SuppressFinalize(paletteScope);

            _oldPalette = paletteScope.HPALETTE;

            _graphics = Graphics.FromHdcInternal((IntPtr)_hdc);
            _graphics.PageUnit = GraphicsUnit.Pixel;
            creationAction?.Invoke(_graphics);

            CheckGraphicsForState(_graphics, Flags);
        }

        return _graphics;
    }

    internal HDC GetHDC() => _hdc;

    internal Graphics? GetGraphics(bool create)
    {
        CheckGraphicsForState(_graphics, Flags);
        return create ? GetOrCreateGraphicsInternal() : _graphics;
    }

    internal void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Only dispose the graphics object if we created it via the HDC.
            if (_graphics is not null && !_hdc.IsNull)
            {
                _graphics.Dispose();
            }
        }

        if (!_oldPalette.IsNull && !_hdc.IsNull)
        {
            PInvokeCore.SelectPalette(_hdc, _oldPalette, bForceBkgd: false);
            _oldPalette = default;
        }
    }

    [Conditional("DEBUG")]
    internal static void CheckGraphicsForState(Graphics? graphics, DrawingEventFlags flags)
    {
        if (graphics is null || !flags.HasFlag(DrawingEventFlags.CheckState)
            || flags.HasFlag(DrawingEventFlags.GraphicsStateUnclean))
        {
            return;
        }

        // Check to see if we've actually corrupted the state
        graphics.GetContextInfo(out PointF offset, out Region? clip);

        using (clip)
        {
            bool isInfinite = clip?.IsInfinite(graphics) ?? true;
            Debug.Assert(offset.IsEmpty, "transform has been modified");
            Debug.Assert(isInfinite, "clipping as been applied");
        }
    }
}
