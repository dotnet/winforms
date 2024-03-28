// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Automation;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

/// <summary>
///  A non selectable ToolStrip item
/// </summary>
[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
public partial class ToolStripStatusLabel : ToolStripLabel, IAutomationLiveRegion
{
    private Padding _defaultMargin = ScaleHelper.ScaleToDpi(new Padding(0, 3, 0, 2), ScaleHelper.InitialSystemDpi);

    private Border3DStyle _borderStyle = Border3DStyle.Flat;
    private ToolStripStatusLabelBorderSides _borderSides = ToolStripStatusLabelBorderSides.None;
    private bool _spring;
    private AutomationLiveSetting _liveSetting;

    public ToolStripStatusLabel()
    {
    }

    public ToolStripStatusLabel(string? text)
        : base(text, image: null, isLink: false, onClick: null)
    {
    }

    public ToolStripStatusLabel(Image? image)
        : base(text: null, image, isLink: false, onClick: null)
    {
    }

    public ToolStripStatusLabel(string? text, Image? image)
        : base(text, image, isLink: false, onClick: null)
    {
    }

    public ToolStripStatusLabel(string? text, Image? image, EventHandler? onClick)
        : base(text, image, isLink: false, onClick, name: null)
    {
    }

    public ToolStripStatusLabel(string? text, Image? image, EventHandler? onClick, string? name)
        : base(text, image, isLink: false, onClick, name)
    {
    }

    /// <summary>
    ///  Creates a new AccessibleObject for this ToolStripStatusLabel instance.
    ///  The AccessibleObject instance returned by this method supports UIA Live Region feature.
    /// </summary>
    /// <returns>
    ///  AccessibleObject for this ToolStripStatusLabel instance.
    /// </returns>
    protected override AccessibleObject CreateAccessibilityInstance()
    {
        return new ToolStripStatusLabelAccessibleObject(this);
    }

    /// <summary>
    ///  Creates an instance of the object that defines how image and text
    ///  gets laid out in the ToolStripItem
    /// </summary>
    private protected override ToolStripItemInternalLayout CreateInternalLayout()
    {
        return new ToolStripStatusLabelLayout(this);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new ToolStripItemAlignment Alignment
    {
        get => base.Alignment;
        set => base.Alignment = value;
    }

    [DefaultValue(Border3DStyle.Flat)]
    [SRDescription(nameof(SR.ToolStripStatusLabelBorderStyleDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public Border3DStyle BorderStyle
    {
        get
        {
            return _borderStyle;
        }
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_borderStyle != value)
            {
                _borderStyle = value;
                Invalidate();
            }
        }
    }

    [DefaultValue(ToolStripStatusLabelBorderSides.None)]
    [SRDescription(nameof(SR.ToolStripStatusLabelBorderSidesDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public ToolStripStatusLabelBorderSides BorderSides
    {
        get
        {
            return _borderSides;
        }
        set
        {
            // no Enum.IsDefined as this is a flags enum.
            if (_borderSides != value)
            {
                _borderSides = value;
                LayoutTransaction.DoLayout(Owner, this, PropertyNames.BorderStyle);
                Invalidate();
            }
        }
    }

    protected internal override Padding DefaultMargin => _defaultMargin;

    [DefaultValue(false)]
    [SRDescription(nameof(SR.ToolStripStatusLabelSpringDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public bool Spring
    {
        get { return _spring; }
        set
        {
            if (_spring != value)
            {
                _spring = value;
                if (ParentInternal is not null)
                {
                    LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Spring);
                }
            }
        }
    }

    /// <summary>
    ///  Indicates the "politeness" level that a client should use
    ///  to notify the user of changes to the live region.
    /// </summary>
    [SRCategory(nameof(SR.CatAccessibility))]
    [DefaultValue(AutomationLiveSetting.Off)]
    [SRDescription(nameof(SR.LiveRegionAutomationLiveSettingDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public AutomationLiveSetting LiveSetting
    {
        get
        {
            return _liveSetting;
        }
        set
        {
            SourceGenerated.EnumValidator.Validate(value);
            _liveSetting = value;
        }
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        if (IsParentAccessibilityObjectCreated && LiveSetting != AutomationLiveSetting.Off)
        {
            AccessibilityObject.RaiseLiveRegionChanged();
        }
    }

    public override Size GetPreferredSize(Size constrainingSize)
    {
        if (BorderSides != ToolStripStatusLabelBorderSides.None)
        {
            return base.GetPreferredSize(constrainingSize) + new Size(4, 4);
        }
        else
        {
            return base.GetPreferredSize(constrainingSize);
        }
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        if (Owner is not null)
        {
            ToolStripRenderer renderer = Renderer!;

            renderer.DrawToolStripStatusLabelBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));

            if ((DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image)
            {
                renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(e.Graphics, this, InternalLayout.ImageRectangle));
            }

            PaintText(e.Graphics);
        }
    }
}
