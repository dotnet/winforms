// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[Designer($"System.Windows.Forms.Design.ToolStripContainerDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.ToolStripContainerDesc))]
public partial class ToolStripContainer : ContainerControl
{
    private readonly ToolStripPanel _topPanel;
    private readonly ToolStripPanel _bottomPanel;
    private readonly ToolStripPanel _leftPanel;
    private readonly ToolStripPanel _rightPanel;
    private readonly ToolStripContentPanel _contentPanel;

    public ToolStripContainer()
    {
        SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

        SuspendLayout();
        try
        {
            // undone - smart demand creation
            _topPanel = new ToolStripPanel(this);
            _bottomPanel = new ToolStripPanel(this);
            _leftPanel = new ToolStripPanel(this);
            _rightPanel = new ToolStripPanel(this);
            _contentPanel = new ToolStripContentPanel
            {
                Dock = DockStyle.Fill
            };
            _topPanel.Dock = DockStyle.Top;
            _bottomPanel.Dock = DockStyle.Bottom;
            _rightPanel.Dock = DockStyle.Right;
            _leftPanel.Dock = DockStyle.Left;

            if (Controls is ToolStripContainerTypedControlCollection controlCollection)
            {
                controlCollection.AddInternal(_contentPanel);
                controlCollection.AddInternal(_leftPanel);
                controlCollection.AddInternal(_rightPanel);
                controlCollection.AddInternal(_topPanel);
                controlCollection.AddInternal(_bottomPanel);
            }

            // else consider throw new exception
        }
        finally
        {
            ResumeLayout(true);
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool AutoScroll
    {
        get => base.AutoScroll;
        set => base.AutoScroll = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Size AutoScrollMargin
    {
        get => base.AutoScrollMargin;
        set => base.AutoScrollMargin = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Size AutoScrollMinSize
    {
        get => base.AutoScrollMinSize;
        set => base.AutoScrollMinSize = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Color BackColor
    {
        get => base.BackColor;
        set => base.BackColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new event EventHandler? BackColorChanged
    {
        add => base.BackColorChanged += value;
        remove => base.BackColorChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new event EventHandler? BackgroundImageChanged
    {
        add => base.BackgroundImageChanged += value;
        remove => base.BackgroundImageChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override ImageLayout BackgroundImageLayout
    {
        get => base.BackgroundImageLayout;
        set => base.BackgroundImageLayout = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new event EventHandler? BackgroundImageLayoutChanged
    {
        add => base.BackgroundImageLayoutChanged += value;
        remove => base.BackgroundImageLayoutChanged += value;
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripContainerBottomToolStripPanelDescr))]
    [Localizable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ToolStripPanel BottomToolStripPanel
    {
        get
        {
            return _bottomPanel;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripContainerBottomToolStripPanelVisibleDescr))]
    [DefaultValue(true)]
    public bool BottomToolStripPanelVisible
    {
        get
        {
            return BottomToolStripPanel.Visible;
        }
        set
        {
            BottomToolStripPanel.Visible = value;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripContainerContentPanelDescr))]
    [Localizable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ToolStripContentPanel ContentPanel
    {
        get
        {
            return _contentPanel;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool CausesValidation
    {
        get => base.CausesValidation;
        set => base.CausesValidation = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? CausesValidationChanged
    {
        add => base.CausesValidationChanged += value;
        remove => base.CausesValidationChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new ContextMenuStrip? ContextMenuStrip
    {
        get => base.ContextMenuStrip;
        set => base.ContextMenuStrip = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ContextMenuStripChanged
    {
        add => base.ContextMenuStripChanged += value;
        remove => base.ContextMenuStripChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public override Cursor Cursor
    {
        get => base.Cursor;
        set => base.Cursor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new event EventHandler? CursorChanged
    {
        add => base.CursorChanged += value;
        remove => base.CursorChanged -= value;
    }

    protected override Size DefaultSize
    {
        get
        {
            return new Size(150, 175);
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Color ForeColor
    {
        get => base.ForeColor;
        set => base.ForeColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new event EventHandler? ForeColorChanged
    {
        add => base.ForeColorChanged += value;
        remove => base.ForeColorChanged -= value;
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripContainerLeftToolStripPanelDescr))]
    [Localizable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ToolStripPanel LeftToolStripPanel
    {
        get
        {
            return _leftPanel;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripContainerLeftToolStripPanelVisibleDescr))]
    [DefaultValue(true)]
    public bool LeftToolStripPanelVisible
    {
        get
        {
            return LeftToolStripPanel.Visible;
        }
        set
        {
            LeftToolStripPanel.Visible = value;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripContainerRightToolStripPanelDescr))]
    [Localizable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ToolStripPanel RightToolStripPanel
    {
        get
        {
            return _rightPanel;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripContainerRightToolStripPanelVisibleDescr))]
    [DefaultValue(true)]
    public bool RightToolStripPanelVisible
    {
        get
        {
            return RightToolStripPanel.Visible;
        }
        set
        {
            RightToolStripPanel.Visible = value;
        }
    }

    internal override bool SupportsUiaProviders => true;

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripContainerTopToolStripPanelDescr))]
    [Localizable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ToolStripPanel TopToolStripPanel
    {
        get
        {
            return _topPanel;
        }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ToolStripContainerTopToolStripPanelVisibleDescr))]
    [DefaultValue(true)]
    public bool TopToolStripPanelVisible
    {
        get
        {
            return TopToolStripPanel.Visible;
        }
        set
        {
            TopToolStripPanel.Visible = value;
        }
    }

    /// <summary>
    ///  Controls Collection...
    ///  This is overridden so that the Controls.Add ( ) is not Code Gen'd...
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new ControlCollection Controls
    {
        get => base.Controls;
    }

    protected override AccessibleObject CreateAccessibilityInstance()
        => new ToolStripContainerAccessibleObject(this);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override ControlCollection CreateControlsInstance()
    {
        return new ToolStripContainerTypedControlCollection(this, /*isReadOnly*/true);
    }

    protected override void OnRightToLeftChanged(EventArgs e)
    {
        base.OnRightToLeftChanged(e);
        RightToLeft rightToLeft = RightToLeft;

        // no need to suspend layout - we're already in a layout transaction.
        if (rightToLeft == RightToLeft.Yes)
        {
            RightToolStripPanel.Dock = DockStyle.Left;
            LeftToolStripPanel.Dock = DockStyle.Right;
        }
        else
        {
            RightToolStripPanel.Dock = DockStyle.Right;
            LeftToolStripPanel.Dock = DockStyle.Left;
        }
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        foreach (Control c in Controls)
        {
            c.SuspendLayout();
        }

        base.OnSizeChanged(e);
        foreach (Control c in Controls)
        {
            c.ResumeLayout();
        }
    }

    internal override void RecreateHandleCore()
    {
        // If ToolStripContainer's Handle is getting created demand create the childControl handle's
        if (IsHandleCreated)
        {
            foreach (Control c in Controls)
            {
                c.CreateControl(true);
            }
        }

        base.RecreateHandleCore();
    }

    internal override bool AllowsKeyboardToolTip()
    {
        return false;
    }
}
