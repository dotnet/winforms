// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  This event is fired by owner drawn <see cref="Control"/> objects, such as <see cref="ListBox"/> and
///  <see cref="ComboBox"/>. It contains all the information needed for the user to paint the given item,
///  including the item index, the <see cref="Rectangle"/> in which the drawing should be done, and the
///  <see cref="Graphics"/> object with which the drawing should be done.
/// </summary>
public class DrawItemEventArgs : EventArgs, IDisposable, IDeviceContext, IGraphicsHdcProvider, IHdcContext
{
    private readonly DrawingEventArgs _event;

    /// <summary>
    ///  The backColor to paint each menu item with.
    /// </summary>
    private readonly Color _backColor;

    /// <summary>
    ///  The foreColor to paint each menu item with.
    /// </summary>
    private readonly Color _foreColor;

    /// <summary>
    ///  Creates a new DrawItemEventArgs with the given parameters.
    /// </summary>
    public DrawItemEventArgs(Graphics graphics, Font? font, Rectangle rect, int index, DrawItemState state)
        : this(graphics, font, rect, index, state, Application.SystemColors.WindowText, Application.SystemColors.Window)
    { }

    /// <summary>
    ///  Creates a new DrawItemEventArgs with the given parameters, including the foreColor and backColor
    ///  of the control.
    /// </summary>
    public DrawItemEventArgs(
        Graphics graphics,
        Font? font,
        Rectangle rect,
        int index,
        DrawItemState state,
        Color foreColor,
        Color backColor)
    {
        _event = new DrawingEventArgs(graphics, rect, DrawingEventFlags.GraphicsStateUnclean);
        Font = font;
        Index = index;
        State = state;
        _foreColor = foreColor;
        _backColor = backColor;
    }

    internal DrawItemEventArgs(
        HDC hdc,
        Font? font,
        Rectangle rect,
        uint index,
        ODS_FLAGS state)
        : this(hdc, font, rect, index, state, Application.SystemColors.WindowText, Application.SystemColors.Window)
    { }

    internal DrawItemEventArgs(
        HDC hdc,
        Font? font,
        Rectangle rect,
        uint index,
        ODS_FLAGS state,
        Color foreColor,
        Color backColor)
    {
        _event = new DrawingEventArgs(hdc, rect, DrawingEventFlags.CheckState);
        Font = font;
        Index = (int)index;
        State = (DrawItemState)state;
        _foreColor = foreColor;
        _backColor = backColor;
    }

    /// <summary>
    ///  Gets the <see cref="Drawing.Graphics"/> object used to paint.
    /// </summary>
    public Graphics Graphics => _event.Graphics;

    /// <summary>
    ///  A suggested font, usually the parent control's Font property.
    /// </summary>
    public Font? Font { get; }

    /// <summary>
    ///  The rectangle outlining the area in which the painting should be  done.
    /// </summary>
    public Rectangle Bounds => _event.ClipRectangle;

    /// <summary>
    ///  The index of the item that should be painted.
    /// </summary>
    public int Index { get; }

    /// <summary>
    ///  Miscellaneous state information, such as whether the item is
    ///  "selected", "focused", or some other such information.  ComboBoxes
    ///  have one special piece of information which indicates if the item
    ///  being painted is the editable portion of the ComboBox.
    /// </summary>
    public DrawItemState State { get; }

    /// <summary>
    ///  A suggested color drawing: either Application.SystemColors.WindowText or Application.SystemColors.HighlightText,
    ///  depending on whether this item is selected.
    /// </summary>
    public Color ForeColor
        => (State & DrawItemState.Selected) == DrawItemState.Selected ? Application.SystemColors.HighlightText : _foreColor;

    public Color BackColor
        => (State & DrawItemState.Selected) == DrawItemState.Selected ? Application.SystemColors.Highlight : _backColor;

    /// <summary>
    ///  Fills the <see cref="Bounds"/> with the <see cref="BackColor"/>.
    /// </summary>
    public virtual void DrawBackground()
    {
        using var backBrush = BackColor.GetCachedSolidBrushScope();
        GraphicsInternal.FillRectangle(backBrush, Bounds);
    }

    /// <summary>
    ///  Draws a handy focus rect in the given rectangle.
    /// </summary>
    public virtual void DrawFocusRectangle()
    {
        if ((State & DrawItemState.Focus) == DrawItemState.Focus
            && (State & DrawItemState.NoFocusRect) != DrawItemState.NoFocusRect)
        {
            ControlPaint.DrawFocusRectangle(GraphicsInternal, Bounds, ForeColor, BackColor);
        }
    }

    public void Dispose() => Dispose(disposing: true);

    protected virtual void Dispose(bool disposing)
    {
        // We need this because of IDeviceContext, but we historically didn't take ownership of the Graphics
        // object, so there is nothing to do here unless we specifically created the Graphics object.
        _event.Dispose(true);
    }

    /// <summary>
    ///  For internal use to improve performance. DO NOT use this method if you modify the <see cref="Graphics"/>
    ///  <see cref="Graphics.Clip"/> or <see cref="Graphics.Transform"/>.
    /// </summary>
    internal Graphics GraphicsInternal => _event.GetOrCreateGraphicsInternal();

    /// <summary>
    ///  Returns the <see cref="HDC"/> the event was created off of, if any.
    /// </summary>
    internal HDC HDC => _event.HDC;

    IntPtr IDeviceContext.GetHdc() => Graphics?.GetHdc() ?? IntPtr.Zero;
    HDC IHdcContext.GetHdc() => (HDC)((IDeviceContext)this).GetHdc();
    void IDeviceContext.ReleaseHdc() => Graphics?.ReleaseHdc();
    void IHdcContext.ReleaseHdc() => ((IDeviceContext)this).ReleaseHdc();
    IGraphics? IGraphicsHdcProvider.GetGraphics(bool createIfNeeded) => _event.GetGraphics(createIfNeeded);
    HDC IGraphicsHdcProvider.GetHdc() => _event.GetHDC();
    bool IGraphicsHdcProvider.IsGraphicsStateClean => _event.IsStateClean;
}
