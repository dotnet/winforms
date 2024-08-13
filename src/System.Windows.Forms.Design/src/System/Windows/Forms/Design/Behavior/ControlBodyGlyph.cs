// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  This Glyph is placed on every control sized to the exact bounds of
///  the control.
/// </summary>
[DebuggerDisplay("{GetType().Name, nq}:: Behavior={Behavior.GetType().Name, nq}, {_hitTestCursor}")]
public class ControlBodyGlyph : ComponentGlyph
{
    private Rectangle _bounds;                  // bounds of the related control
    private readonly Cursor? _hitTestCursor;    // cursor used to hit test
    private readonly IComponent? _component;

    /// <summary>
    ///  Standard Constructor.
    /// </summary>
    public ControlBodyGlyph(
        Rectangle bounds,
        Cursor? cursor,
        IComponent? relatedComponent,
        ControlDesigner designer)
        : base(relatedComponent, new ControlDesigner.TransparentBehavior(designer))
    {
        _bounds = bounds;
        _hitTestCursor = cursor;
        _component = relatedComponent;
    }

    public ControlBodyGlyph(Rectangle bounds, Cursor? cursor, IComponent? relatedComponent, Behavior? behavior)
        : base(relatedComponent, behavior)
    {
        _bounds = bounds;
        _hitTestCursor = cursor;
        _component = relatedComponent;
    }

    /// <summary>
    ///  The bounds of this glyph.
    /// </summary>
    public override Rectangle Bounds => _bounds;

    /// <summary>
    ///  Simple hit test rule: if the point is contained within the bounds
    ///  AND the component is Visible (controls on some tab pages may
    ///  not be, for ex) then it is a positive hit test.
    /// </summary>
    public override Cursor? GetHitTest(Point p)
    {
        // non-controls are always visible here
        bool isVisible = _component is not Control control || control.Visible;

        if (isVisible && _bounds.Contains(p))
        {
            return _hitTestCursor;
        }

        return null;
    }
}
