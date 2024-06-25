// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
public partial class ToolStripSeparator : ToolStripItem
{
    private const int SeparatorThickness = 6;
    private const int SeparatorHeight = 23;

    public ToolStripSeparator()
    {
        ForeColor = Application.ApplicationColors.ControlDark;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool AutoToolTip
    {
        get => base.AutoToolTip;
        set => base.AutoToolTip = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override ImageLayout BackgroundImageLayout
    {
        get => base.BackgroundImageLayout;
        set => base.BackgroundImageLayout = value;
    }

    public override bool CanSelect => DesignMode;

    /// <summary>
    ///  Deriving classes can override this to configure a default size for their control.
    ///  This is more efficient than setting the size in the control's constructor.
    /// </summary>
    protected override Size DefaultSize => new(SeparatorThickness, SeparatorThickness);

    protected internal override Padding DefaultMargin => Padding.Empty;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool DoubleClickEnabled
    {
        get => base.DoubleClickEnabled;
        set => base.DoubleClickEnabled = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool Enabled
    {
        get => base.Enabled;
        set => base.Enabled = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? EnabledChanged
    {
        add => base.EnabledChanged += value;
        remove => base.EnabledChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new ToolStripItemDisplayStyle DisplayStyle
    {
        get => base.DisplayStyle;
        set => base.DisplayStyle = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? DisplayStyleChanged
    {
        add => base.DisplayStyleChanged += value;
        remove => base.DisplayStyleChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public override Font Font
    {
        get => base.Font;
        set => base.Font = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new ContentAlignment ImageAlign
    {
        get => base.ImageAlign;
        set => base.ImageAlign = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Image? Image
    {
        get => base.Image;
        set => base.Image = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new int ImageIndex
    {
        get => base.ImageIndex;
        set => base.ImageIndex = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public new string ImageKey
    {
        get => base.ImageKey;
        set => base.ImageKey = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Color ImageTransparentColor
    {
        get => base.ImageTransparentColor;
        set => base.ImageTransparentColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new ToolStripItemImageScaling ImageScaling
    {
        get => base.ImageScaling;
        set => base.ImageScaling = value;
    }

    private bool IsVertical
    {
        get
        {
            ToolStrip? parent = ParentInternal ?? Owner;

            if (parent is null)
            {
                return true;
            }

            if (parent is ToolStripDropDownMenu)
            {
                return false;
            }

            return parent.LayoutStyle switch
            {
                ToolStripLayoutStyle.VerticalStackWithOverflow => false,
                _ => true,
            };
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string? Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TextChanged
    {
        add => base.TextChanged += value;
        remove => base.TextChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new ContentAlignment TextAlign
    {
        get => base.TextAlign;
        set => base.TextAlign = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DefaultValue(ToolStripTextDirection.Horizontal)]
    public override ToolStripTextDirection TextDirection
    {
        get => base.TextDirection;
        set => base.TextDirection = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new TextImageRelation TextImageRelation
    {
        get => base.TextImageRelation;
        set => base.TextImageRelation = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new string? ToolTipText
    {
        get => base.ToolTipText;
        set => base.ToolTipText = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool RightToLeftAutoMirrorImage
    {
        get => base.RightToLeftAutoMirrorImage;
        set => base.RightToLeftAutoMirrorImage = value;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override AccessibleObject CreateAccessibilityInstance()
        => new ToolStripSeparatorAccessibleObject(this);

    public override Size GetPreferredSize(Size constrainingSize)
    {
        ToolStrip? parent = ParentInternal ?? Owner;

        if (parent is null)
        {
            return new Size(SeparatorThickness, SeparatorThickness);
        }

        if (parent is ToolStripDropDownMenu dropDownMenu)
        {
            return new Size(parent.Width - (parent.Padding.Horizontal - dropDownMenu.ImageMargin.Width), SeparatorThickness);
        }

        // This is always true!
        // parent.LayoutStyle != ToolStripLayoutStyle.HorizontalStackWithOverflow || parent.LayoutStyle != ToolStripLayoutStyle.VerticalStackWithOverflow
        {
            // we don't actually know what size to make it, so just keep it a stock size.
            constrainingSize.Width = SeparatorHeight;
            constrainingSize.Height = SeparatorHeight;
        }

        if (IsVertical)
        {
            return new Size(SeparatorThickness, constrainingSize.Height);
        }

        return new Size(constrainingSize.Width, SeparatorThickness);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (Owner is not null && ParentInternal is not null)
        {
            Renderer!.DrawSeparator(new ToolStripSeparatorRenderEventArgs(e.Graphics, this, IsVertical));
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void OnFontChanged(EventArgs e)
    {
        // Perf: don't call base, we don't care if the font changes
        RaiseEvent(s_fontChangedEvent, e);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal override bool ShouldSerializeForeColor() => ForeColor != Application.ApplicationColors.ControlDark;

    protected internal override void SetBounds(Rectangle rect)
    {
        if (Owner is ToolStripDropDownMenu dropDownMenu)
        {
            // Scooch over by the padding amount. The padding is added to the ToolStripDropDownMenu
            // to keep the non-menu item aligned to the text rectangle. When flow layout comes
            // through to set our position via IArrangedElement, ignore it.
            rect.X = 2;
            rect.Width = dropDownMenu.Width - 4;
        }

        base.SetBounds(rect);
    }
}
