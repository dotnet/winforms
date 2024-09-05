// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  This internal class is used by the new ToolStripDesigner to add a dummy node to the end.
///  This class inherits from ToolStripControlHost and overrides the CanSelect property so that the dummy Node when
///  shown in the designer doesn't show selection on Mouse movements. The image is set to theDummyNodeImage embedded
///  into the resources.
/// </summary>
internal class DesignerToolStripControlHost : ToolStripControlHost, IComponent
{
    private BehaviorService? _behaviorService;

    public DesignerToolStripControlHost(Control c)
        : base(c)
    {
        // this ToolStripItem should not have defaultPadding.
        Margin = Padding.Empty;
    }

    /// <summary>
    ///  We need to return Default size for Editor ToolStrip (92, 22).
    /// </summary>
    protected override Size DefaultSize
    {
        get => new(92, 22);
    }

    internal GlyphCollection GetGlyphs(ToolStrip parent, GlyphCollection glyphs, Behavior.Behavior standardBehavior)
    {
        if (_behaviorService is null)
        {
            if (parent.Site is not ISite site)
            {
                throw new InvalidOperationException();
            }

            _behaviorService = site.GetRequiredService<BehaviorService>();
        }

        if (Parent is null)
        {
            throw new InvalidOperationException();
        }

        Point loc = _behaviorService.ControlToAdornerWindow(Parent);
        Rectangle r = Bounds;
        r.Offset(loc);
        r.Inflate(-2, -2);
        glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Top, standardBehavior));
        glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Bottom, standardBehavior));
        glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Left, standardBehavior));
        glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Right, standardBehavior));

        return glyphs;
    }

    internal void RefreshSelectionGlyph()
    {
        if (Control is ToolStrip miniToolStrip)
        {
            if (miniToolStrip.Renderer is ToolStripTemplateNode.MiniToolStripRenderer renderer)
            {
                renderer.State = (int)TemplateNodeSelectionState.None;
                miniToolStrip.Invalidate();
            }
        }
    }

    internal void SelectControl()
    {
        if (Control is ToolStrip miniToolStrip)
        {
            if (miniToolStrip.Renderer is ToolStripTemplateNode.MiniToolStripRenderer renderer)
            {
                renderer.State = (int)TemplateNodeSelectionState.TemplateNodeSelected;
                miniToolStrip.Focus();
                miniToolStrip.Invalidate();
            }
        }
    }
}
