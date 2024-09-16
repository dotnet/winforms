// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  The ComponentGlyph class simply contains a pointer back
///  to it's related Component. This can be used to trace
///  Glyphs (during drag operations or otherwise) back to
///  their component.
/// </summary>
public class ComponentGlyph : Glyph
{
    private readonly IComponent? _relatedComponent;

    /// <summary>
    ///  Standard constructor.
    /// </summary>
    public ComponentGlyph(IComponent? relatedComponent, Behavior? behavior)
        : base(behavior)
    {
        _relatedComponent = relatedComponent;
    }

    public ComponentGlyph(IComponent? relatedComponent)
        : base(null)
    {
        _relatedComponent = relatedComponent;
    }

    /// <summary>
    ///  Returns the Component this Glyph is related to.
    /// </summary>
    public IComponent? RelatedComponent => _relatedComponent;

    /// <summary>
    ///  Overrides GetHitTest - this implementation does nothing.
    /// </summary>
    public override Cursor? GetHitTest(Point p) => null;

    /// <summary>
    ///  Overrides Glyph::Paint - this implementation does nothing.
    /// </summary>
    public override void Paint(PaintEventArgs pe)
    {
    }
}
