// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.None)]
public partial class ToolStripOverflowButton : ToolStripDropDownButton
{
    // Cache this as the Parent property gets reset a lot.
    private readonly ToolStrip _parentToolStrip;

    private static bool s_isScalingInitialized;
    private static int s_maxSize;

    internal ToolStripOverflowButton(ToolStrip parentToolStrip)
    {
        const int LogicalMaxSize = 16;

        if (!s_isScalingInitialized)
        {
            s_maxSize = ScaleHelper.ScaleToInitialSystemDpi(LogicalMaxSize);
            s_isScalingInitialized = true;
        }

        SupportsItemClick = false;
        _parentToolStrip = parentToolStrip;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && HasDropDownItems)
        {
            DropDown.Dispose();
        }

        base.Dispose(disposing);
    }

    protected internal override Padding DefaultMargin => Padding.Empty;

    public override bool HasDropDownItems => ParentInternal is not null && ParentInternal.OverflowItems.Count > 0;

    internal override bool OppositeDropDownAlign => true;

    internal ToolStrip ParentToolStrip => _parentToolStrip;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool RightToLeftAutoMirrorImage
    {
        get => base.RightToLeftAutoMirrorImage;
        set => base.RightToLeftAutoMirrorImage = value;
    }

    protected override AccessibleObject CreateAccessibilityInstance()
    {
        return new ToolStripOverflowButtonAccessibleObject(this);
    }

    protected override ToolStripDropDown CreateDefaultDropDown()
    {
        // AutoGenerate a ToolStrip DropDown - set the property so we hook events
        return new ToolStripOverflow(this);
    }

    public override Size GetPreferredSize(Size constrainingSize)
    {
        Size preferredSize = constrainingSize;
        if (ParentInternal is { } parent)
        {
            if (parent.Orientation == Orientation.Horizontal)
            {
                preferredSize.Width = Math.Min(constrainingSize.Width, s_maxSize);
            }
            else
            {
                preferredSize.Height = Math.Min(constrainingSize.Height, s_maxSize);
            }
        }

        return preferredSize + Padding.Size;
    }

    protected internal override void SetBounds(Rectangle bounds)
    {
        // Make sure the Overflow button extends from edge-edge (ignore Padding/Margin).
        if (ParentInternal is not null && ParentInternal.LayoutEngine is ToolStripSplitStackLayout)
        {
            if (ParentInternal.Orientation == Orientation.Horizontal)
            {
                bounds.Height = ParentInternal.Height;
                bounds.Y = 0;
            }
            else
            {
                bounds.Width = ParentInternal.Width;
                bounds.X = 0;
            }
        }

        base.SetBounds(bounds);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (ParentInternal is not null)
        {
            ToolStripRenderer renderer = ParentInternal.Renderer;
            renderer.DrawOverflowButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
        }
    }
}
