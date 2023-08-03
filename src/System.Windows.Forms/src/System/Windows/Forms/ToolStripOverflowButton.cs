// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

/// <summary>
///  ToolStripOverflowButton
/// </summary>
[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.None)]
public partial class ToolStripOverflowButton : ToolStripDropDownButton
{
    // we need to cache this away as the Parent property gets reset a lot.
    private readonly ToolStrip _parentToolStrip;

    private static bool isScalingInitialized;
    private const int MAX_WIDTH = 16;
    private const int MAX_HEIGHT = 16;
    private static int maxWidth = MAX_WIDTH;
    private static int maxHeight = MAX_HEIGHT;

    internal ToolStripOverflowButton(ToolStrip parentToolStrip)
    {
        if (!isScalingInitialized)
        {
            if (DpiHelper.IsScalingRequired)
            {
                maxWidth = DpiHelper.LogicalToDeviceUnitsX(MAX_WIDTH);
                maxHeight = DpiHelper.LogicalToDeviceUnitsY(MAX_HEIGHT);
            }

            isScalingInitialized = true;
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

    protected internal override Padding DefaultMargin
    {
        get
        {
            return Padding.Empty;
        }
    }

    public override bool HasDropDownItems
    {
        get
        {
            return ParentInternal is not null && ParentInternal.OverflowItems.Count > 0;
        }
    }

    internal override bool OppositeDropDownAlign
    {
        get { return true; }
    }

    internal ToolStrip ParentToolStrip
    {
        get { return _parentToolStrip; }
    }

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
        if (ParentInternal is not null)
        {
            if (ParentInternal.Orientation == Orientation.Horizontal)
            {
                preferredSize.Width = Math.Min(constrainingSize.Width, maxWidth);
            }
            else
            {
                preferredSize.Height = Math.Min(constrainingSize.Height, maxHeight);
            }
        }

        return preferredSize + Padding.Size;
    }

    // make sure the Overflow button extends from edge-edge. (Ignore Padding/Margin).
    protected internal override void SetBounds(Rectangle bounds)
    {
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
