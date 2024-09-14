// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;

namespace System.Windows.Forms;

/// <summary>
///  Represents a dialog box form that contains a <see cref="Forms.PrintPreviewControl"/>.
/// </summary>
[Designer($"System.ComponentModel.Design.ComponentDesigner, {AssemblyRef.SystemDesign}")]
[DesignTimeVisible(true)]
[DefaultProperty(nameof(Document))]
[ToolboxItemFilter("System.Windows.Forms.Control.TopLevel")]
[ToolboxItem(true)]
[SRDescription(nameof(SR.DescriptionPrintPreviewDialog))]
public partial class PrintPreviewDialog : Form
{
    private readonly PrintPreviewControl _previewControl;
    private ToolStrip _toolStrip1;
    private ToolStripNumericUpDown _pageCounterItem;
    private NumericUpDown _pageCounter;
    private ToolStripButton _printToolStripButton;
    private ToolStripSplitButton _zoomToolStripSplitButton;
    private ToolStripMenuItem _autoToolStripMenuItem;
    private ToolStripMenuItem _toolStripMenuItem1;
    private ToolStripMenuItem _toolStripMenuItem2;
    private ToolStripMenuItem _toolStripMenuItem3;
    private ToolStripMenuItem _toolStripMenuItem4;
    private ToolStripMenuItem _toolStripMenuItem5;
    private ToolStripMenuItem _toolStripMenuItem6;
    private ToolStripMenuItem _toolStripMenuItem7;
    private ToolStripMenuItem _toolStripMenuItem8;
    private ToolStripSeparator _separatorToolStripSeparator;
    private PrintPreviewDialogToolStripButton _onePageToolStripButton;
    private PrintPreviewDialogToolStripButton _twoPagesToolStripButton;
    private PrintPreviewDialogToolStripButton _threePagesToolStripButton;
    private PrintPreviewDialogToolStripButton _fourPagesToolStripButton;
    private PrintPreviewDialogToolStripButton _sixPagesToolStripButton;
    private ToolStripSeparator _separatorToolStripSeparator1;
    private ToolStripButton _closeToolStripButton;
    private ToolStripLabel _pageToolStripLabel;

    private readonly ImageList _imageList;

    /// <summary>
    ///  Initializes a new instance of the <see cref="PrintPreviewDialog"/> class.
    /// </summary>
    public PrintPreviewDialog()
    {
        base.AutoScaleBaseSize = new Size(5, 13);

        _previewControl = new PrintPreviewControl();
        _imageList = new ImageList();
        _imageList.Images.AddStrip(ScaleHelper.GetIconResourceAsDefaultSizeBitmap(typeof(PrintPreviewDialog), "PrintPreviewStrip"));
        InitForm();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new IButtonControl? AcceptButton
    {
        get => base.AcceptButton;
        set => base.AcceptButton = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool AutoScale
    {
#pragma warning disable CS0618 // Type or member is obsolete
        get => base.AutoScale;
        set => base.AutoScale = value;
#pragma warning restore CS0618
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool AutoScroll
    {
        get => base.AutoScroll;
        set => base.AutoScroll = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool AutoSize
    {
        get => base.AutoSize;
        set => base.AutoSize = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? AutoSizeChanged
    {
        add => base.AutoSizeChanged += value;
        remove => base.AutoSizeChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override AutoValidate AutoValidate
    {
        get => base.AutoValidate;
        set => base.AutoValidate = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? AutoValidateChanged
    {
        add => base.AutoValidateChanged += value;
        remove => base.AutoValidateChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color BackColor
    {
        get => base.BackColor;
        set => base.BackColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackColorChanged
    {
        add => base.BackColorChanged += value;
        remove => base.BackColorChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new IButtonControl? CancelButton
    {
        get => base.CancelButton;
        set => base.CancelButton = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool ControlBox
    {
        get => base.ControlBox;
        set => base.ControlBox = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ContextMenuStrip? ContextMenuStrip
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
    public new FormBorderStyle FormBorderStyle
    {
        get => base.FormBorderStyle;
        set => base.FormBorderStyle = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool HelpButton
    {
        get => base.HelpButton;
        set => base.HelpButton = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Icon? Icon
    {
        get => base.Icon;
        set => base.Icon = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool IsMdiContainer
    {
        get => base.IsMdiContainer;
        set => base.IsMdiContainer = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool KeyPreview
    {
        get => base.KeyPreview;
        set => base.KeyPreview = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Size MaximumSize
    {
        get => base.MaximumSize;
        set => base.MaximumSize = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MaximumSizeChanged
    {
        add => base.MaximumSizeChanged += value;
        remove => base.MaximumSizeChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool MaximizeBox
    {
        get => base.MaximizeBox;
        set => base.MaximizeBox = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Padding Margin
    {
        get => base.Margin;
        set => base.Margin = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MarginChanged
    {
        add => base.MarginChanged += value;
        remove => base.MarginChanged -= value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Size MinimumSize
    {
        get => base.MinimumSize;
        set => base.MinimumSize = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MinimumSizeChanged
    {
        add => base.MinimumSizeChanged += value;
        remove => base.MinimumSizeChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Padding Padding
    {
        get => base.Padding;
        set => base.Padding = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? PaddingChanged
    {
        add => base.PaddingChanged += value;
        remove => base.PaddingChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Size Size
    {
        get => base.Size;
        set => base.Size = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? SizeChanged
    {
        add => base.SizeChanged += value;
        remove => base.SizeChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new FormStartPosition StartPosition
    {
        get => base.StartPosition;
        set => base.StartPosition = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool TopMost
    {
        get => base.TopMost;
        set => base.TopMost = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Color TransparencyKey
    {
        get => base.TransparencyKey;
        set => base.TransparencyKey = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool UseWaitCursor
    {
        get => base.UseWaitCursor;
        set => base.UseWaitCursor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new FormWindowState WindowState
    {
        get => base.WindowState;
        set => base.WindowState = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new AccessibleRole AccessibleRole
    {
        get => base.AccessibleRole;
        set => base.AccessibleRole = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new string? AccessibleDescription
    {
        get => base.AccessibleDescription;
        set => base.AccessibleDescription = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new string? AccessibleName
    {
        get => base.AccessibleName;
        set => base.AccessibleName = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
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
    public new ControlBindingsCollection DataBindings => base.DataBindings;

    protected override Size DefaultMinimumSize => new(375, 250);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool Enabled
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
    public new Point Location
    {
        get => base.Location;
        set => base.Location = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? LocationChanged
    {
        add => base.LocationChanged += value;
        remove => base.LocationChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new object? Tag
    {
        get => base.Tag;
        set => base.Tag = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool AllowDrop
    {
        get => base.AllowDrop;
        set => base.AllowDrop = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AllowNull]
    public override Cursor Cursor
    {
        get => base.Cursor;
        set => base.Cursor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? CursorChanged
    {
        add => base.CursorChanged += value;
        remove => base.CursorChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageChanged
    {
        add => base.BackgroundImageChanged += value;
        remove => base.BackgroundImageChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get => base.BackgroundImageLayout;
        set => base.BackgroundImageLayout = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageLayoutChanged
    {
        add => base.BackgroundImageLayoutChanged += value;
        remove => base.BackgroundImageLayoutChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new ImeMode ImeMode
    {
        get => base.ImeMode;
        set => base.ImeMode = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ImeModeChanged
    {
        add => base.ImeModeChanged += value;
        remove => base.ImeModeChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Size AutoScrollMargin
    {
        get => base.AutoScrollMargin;
        set => base.AutoScrollMargin = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Size AutoScrollMinSize
    {
        get => base.AutoScrollMinSize;
        set => base.AutoScrollMinSize = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override AnchorStyles Anchor
    {
        get => base.Anchor;
        set => base.Anchor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool Visible
    {
        get => base.Visible;
        set => base.Visible = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? VisibleChanged
    {
        add => base.VisibleChanged += value;
        remove => base.VisibleChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get => base.ForeColor;
        set => base.ForeColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ForeColorChanged
    {
        add => base.ForeColorChanged += value;
        remove => base.ForeColorChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override RightToLeft RightToLeft
    {
        get => base.RightToLeft;
        set => base.RightToLeft = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool RightToLeftLayout
    {
        get => base.RightToLeftLayout;
        set => base.RightToLeftLayout = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? RightToLeftChanged
    {
        add => base.RightToLeftChanged += value;
        remove => base.RightToLeftChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? RightToLeftLayoutChanged
    {
        add => base.RightToLeftLayoutChanged += value;
        remove => base.RightToLeftLayoutChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabStopChanged
    {
        add => base.TabStopChanged += value;
        remove => base.TabStopChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AllowNull]
    public override string Text
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
    public override DockStyle Dock
    {
        get => base.Dock;
        set => base.Dock = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? DockChanged
    {
        add => base.DockChanged += value;
        remove => base.DockChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AllowNull]
    public override Font Font
    {
        get => base.Font;
        set => base.Font = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? FontChanged
    {
        add => base.FontChanged += value;
        remove => base.FontChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new DockPaddingEdges DockPadding => base.DockPadding;

    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PrintPreviewAntiAliasDescr))]
    public bool UseAntiAlias
    {
        get => PrintPreviewControl.UseAntiAlias;
        set => PrintPreviewControl.UseAntiAlias = value;
    }

#pragma warning disable 0809
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This property has been deprecated. Use the AutoScaleDimensions property instead.  https://go.microsoft.com/fwlink/?linkid=14202")]
    public override Size AutoScaleBaseSize
    {
        get => base.AutoScaleBaseSize;
        set
        {
            // No-op
        }
    }
#pragma warning restore 0809

    /// <summary>
    ///  Gets or sets the document to preview.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(null)]
    [SRDescription(nameof(SR.PrintPreviewDocumentDescr))]
    public PrintDocument? Document
    {
        get => _previewControl.Document;
        set => _previewControl.Document = value;
    }

    [Browsable(false)]
    [DefaultValue(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool MinimizeBox
    {
        get => base.MinimizeBox;
        set => base.MinimizeBox = value;
    }

    /// <summary>
    ///  Gets the <see cref="Forms.PrintPreviewControl"/> contained in this form.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.PrintPreviewPrintPreviewControlDescr))]
    [Browsable(false)]
    public PrintPreviewControl PrintPreviewControl => _previewControl;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public new double Opacity
    {
        get => base.Opacity;
        set => base.Opacity = value;
    }

    [Browsable(false)]
    [DefaultValue(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool ShowInTaskbar
    {
        get => base.ShowInTaskbar;
        set => base.ShowInTaskbar = value;
    }

    [Browsable(false)]
    [DefaultValue(SizeGripStyle.Hide)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new SizeGripStyle SizeGripStyle
    {
        get => base.SizeGripStyle;
        set => base.SizeGripStyle = value;
    }

    [MemberNotNull(nameof(_toolStrip1))]
    [MemberNotNull(nameof(_printToolStripButton))]
    [MemberNotNull(nameof(_zoomToolStripSplitButton))]
    [MemberNotNull(nameof(_autoToolStripMenuItem))]
    [MemberNotNull(nameof(_toolStripMenuItem1))]
    [MemberNotNull(nameof(_toolStripMenuItem2))]
    [MemberNotNull(nameof(_toolStripMenuItem3))]
    [MemberNotNull(nameof(_toolStripMenuItem4))]
    [MemberNotNull(nameof(_toolStripMenuItem5))]
    [MemberNotNull(nameof(_toolStripMenuItem6))]
    [MemberNotNull(nameof(_toolStripMenuItem7))]
    [MemberNotNull(nameof(_toolStripMenuItem8))]
    [MemberNotNull(nameof(_separatorToolStripSeparator))]
    [MemberNotNull(nameof(_onePageToolStripButton))]
    [MemberNotNull(nameof(_twoPagesToolStripButton))]
    [MemberNotNull(nameof(_threePagesToolStripButton))]
    [MemberNotNull(nameof(_fourPagesToolStripButton))]
    [MemberNotNull(nameof(_sixPagesToolStripButton))]
    [MemberNotNull(nameof(_separatorToolStripSeparator1))]
    [MemberNotNull(nameof(_closeToolStripButton))]
    [MemberNotNull(nameof(_pageCounterItem))]
    [MemberNotNull(nameof(_pageCounter))]
    [MemberNotNull(nameof(_pageToolStripLabel))]
    private void InitForm()
    {
        ComponentResourceManager resources = new(typeof(PrintPreviewDialog));
        _toolStrip1 = new ToolStrip();
        _printToolStripButton = new ToolStripButton();
        _zoomToolStripSplitButton = new ToolStripSplitButton();
        _autoToolStripMenuItem = new ToolStripMenuItem();
        _toolStripMenuItem1 = new ToolStripMenuItem();
        _toolStripMenuItem2 = new ToolStripMenuItem();
        _toolStripMenuItem3 = new ToolStripMenuItem();
        _toolStripMenuItem4 = new ToolStripMenuItem();
        _toolStripMenuItem5 = new ToolStripMenuItem();
        _toolStripMenuItem6 = new ToolStripMenuItem();
        _toolStripMenuItem7 = new ToolStripMenuItem();
        _toolStripMenuItem8 = new ToolStripMenuItem();
        _separatorToolStripSeparator = new ToolStripSeparator();
        _onePageToolStripButton = new PrintPreviewDialogToolStripButton();
        _twoPagesToolStripButton = new PrintPreviewDialogToolStripButton();
        _threePagesToolStripButton = new PrintPreviewDialogToolStripButton();
        _fourPagesToolStripButton = new PrintPreviewDialogToolStripButton();
        _sixPagesToolStripButton = new PrintPreviewDialogToolStripButton();
        _separatorToolStripSeparator1 = new ToolStripSeparator();
        _closeToolStripButton = new ToolStripButton();
        _pageCounterItem = new ToolStripNumericUpDown();
        _pageCounter = _pageCounterItem.NumericUpDownControl;
        _pageToolStripLabel = new ToolStripLabel();
        _toolStrip1.SuspendLayout();
        SuspendLayout();

        resources.ApplyResources(_toolStrip1, "toolStrip1");
        _toolStrip1.Items.AddRange(
            _printToolStripButton,
            _zoomToolStripSplitButton,
            _separatorToolStripSeparator,
            _onePageToolStripButton,
            _twoPagesToolStripButton,
            _threePagesToolStripButton,
            _fourPagesToolStripButton,
            _sixPagesToolStripButton,
            _separatorToolStripSeparator1,
            _closeToolStripButton);

        _toolStrip1.Name = "toolStrip1";

        _toolStrip1.GripStyle = ToolStripGripStyle.Hidden;

        _printToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _printToolStripButton.Name = "printToolStripButton";
        resources.ApplyResources(_printToolStripButton, "printToolStripButton");

        _zoomToolStripSplitButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _zoomToolStripSplitButton.DoubleClickEnabled = true;
        _zoomToolStripSplitButton.DropDownItems.AddRange(
            _autoToolStripMenuItem,
            _toolStripMenuItem1,
            _toolStripMenuItem2,
            _toolStripMenuItem3,
            _toolStripMenuItem4,
            _toolStripMenuItem5,
            _toolStripMenuItem6,
            _toolStripMenuItem7,
            _toolStripMenuItem8);

        _zoomToolStripSplitButton.Name = "zoomToolStripSplitButton";
        _zoomToolStripSplitButton.SplitterWidth = 1;
        resources.ApplyResources(_zoomToolStripSplitButton, "zoomToolStripSplitButton");

        _autoToolStripMenuItem.CheckOnClick = true;
        _autoToolStripMenuItem.DoubleClickEnabled = true;
        _autoToolStripMenuItem.Checked = true;
        _autoToolStripMenuItem.Name = "autoToolStripMenuItem";
        resources.ApplyResources(_autoToolStripMenuItem, "autoToolStripMenuItem");

        _toolStripMenuItem1.CheckOnClick = true;
        _toolStripMenuItem1.DoubleClickEnabled = true;
        _toolStripMenuItem1.Name = "toolStripMenuItem1";
        resources.ApplyResources(_toolStripMenuItem1, "toolStripMenuItem1");

        _toolStripMenuItem2.CheckOnClick = true;
        _toolStripMenuItem2.DoubleClickEnabled = true;
        _toolStripMenuItem2.Name = "toolStripMenuItem2";
        resources.ApplyResources(_toolStripMenuItem2, "toolStripMenuItem2");

        _toolStripMenuItem3.CheckOnClick = true;
        _toolStripMenuItem3.DoubleClickEnabled = true;
        _toolStripMenuItem3.Name = "toolStripMenuItem3";
        resources.ApplyResources(_toolStripMenuItem3, "toolStripMenuItem3");

        _toolStripMenuItem4.CheckOnClick = true;
        _toolStripMenuItem4.DoubleClickEnabled = true;
        _toolStripMenuItem4.Name = "toolStripMenuItem4";
        resources.ApplyResources(_toolStripMenuItem4, "toolStripMenuItem4");

        _toolStripMenuItem5.CheckOnClick = true;
        _toolStripMenuItem5.DoubleClickEnabled = true;
        _toolStripMenuItem5.Name = "toolStripMenuItem5";
        resources.ApplyResources(_toolStripMenuItem5, "toolStripMenuItem5");

        _toolStripMenuItem6.CheckOnClick = true;
        _toolStripMenuItem6.DoubleClickEnabled = true;
        _toolStripMenuItem6.Name = "toolStripMenuItem6";
        resources.ApplyResources(_toolStripMenuItem6, "toolStripMenuItem6");

        _toolStripMenuItem7.CheckOnClick = true;
        _toolStripMenuItem7.DoubleClickEnabled = true;
        _toolStripMenuItem7.Name = "toolStripMenuItem7";
        resources.ApplyResources(_toolStripMenuItem7, "toolStripMenuItem7");

        _toolStripMenuItem8.CheckOnClick = true;
        _toolStripMenuItem8.DoubleClickEnabled = true;
        _toolStripMenuItem8.Name = "toolStripMenuItem8";
        resources.ApplyResources(_toolStripMenuItem8, "toolStripMenuItem8");

        _separatorToolStripSeparator.Name = "separatorToolStripSeparator";

        _onePageToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _onePageToolStripButton.Name = "onepageToolStripButton";
        resources.ApplyResources(_onePageToolStripButton, "onepageToolStripButton");

        _twoPagesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _twoPagesToolStripButton.Name = "twopagesToolStripButton";
        resources.ApplyResources(_twoPagesToolStripButton, "twopagesToolStripButton");

        _threePagesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _threePagesToolStripButton.Name = "threepagesToolStripButton";
        resources.ApplyResources(_threePagesToolStripButton, "threepagesToolStripButton");

        _fourPagesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _fourPagesToolStripButton.Name = "fourpagesToolStripButton";
        resources.ApplyResources(_fourPagesToolStripButton, "fourpagesToolStripButton");

        _sixPagesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _sixPagesToolStripButton.Name = "sixpagesToolStripButton";
        resources.ApplyResources(_sixPagesToolStripButton, "sixpagesToolStripButton");

        _separatorToolStripSeparator1.Name = "separatorToolStripSeparator1";

        _closeToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        _closeToolStripButton.Name = "closeToolStripButton";
        resources.ApplyResources(_closeToolStripButton, "closeToolStripButton");

        resources.ApplyResources(_pageCounter, "pageCounter");
        _pageCounter.Text = "1";
        _pageCounter.TextAlign = HorizontalAlignment.Right;
        _pageCounter.DecimalPlaces = 0;
        _pageCounter.Minimum = new decimal(0d);
        _pageCounter.Maximum = new decimal(1000d);
        _pageCounter.ValueChanged += UpDownMove;
        _pageCounter.Name = "pageCounter";

        _pageToolStripLabel.Alignment = ToolStripItemAlignment.Right;
        _pageToolStripLabel.Name = "pageToolStripLabel";
        resources.ApplyResources(_pageToolStripLabel, "pageToolStripLabel");

        _previewControl.Size = new Size(792, 610);
        _previewControl.Location = new Point(0, 43);
        _previewControl.Dock = DockStyle.Fill;
        _previewControl.StartPageChanged += PreviewControl_StartPageChanged;

        // Events
        _printToolStripButton.Click += OnPrintToolStripButtonClick;
        _autoToolStripMenuItem.Click += ZoomAuto;
        _toolStripMenuItem1.Click += Zoom500;
        _toolStripMenuItem2.Click += Zoom250;
        _toolStripMenuItem3.Click += Zoom150;
        _toolStripMenuItem4.Click += Zoom100;
        _toolStripMenuItem5.Click += Zoom75;
        _toolStripMenuItem6.Click += Zoom50;
        _toolStripMenuItem7.Click += Zoom25;
        _toolStripMenuItem8.Click += Zoom10;
        _onePageToolStripButton.Click += OnOnePageToolStripButtonClick;
        _twoPagesToolStripButton.Click += OnTwoPagesToolStripButtonClick;
        _threePagesToolStripButton.Click += OnThreePagesToolStripButtonClick;
        _fourPagesToolStripButton.Click += OnFourPagesToolStripButtonClick;
        _sixPagesToolStripButton.Click += OnSixPagesToolStripButtonClick;
        _closeToolStripButton.Click += OnCloseToolStripButtonClick;
        _closeToolStripButton.Paint += OnCloseToolStripButtonPaint;

        // Images
        _toolStrip1.ImageList = _imageList;
        _printToolStripButton.ImageIndex = 0;
        _zoomToolStripSplitButton.ImageIndex = 1;
        _onePageToolStripButton.ImageIndex = 2;
        _twoPagesToolStripButton.ImageIndex = 3;
        _threePagesToolStripButton.ImageIndex = 4;
        _fourPagesToolStripButton.ImageIndex = 5;
        _sixPagesToolStripButton.ImageIndex = 6;

        // tabIndex
        _previewControl.TabIndex = 0;
        _toolStrip1.TabIndex = 1;

        // DefaultItem on the Zoom SplitButton
        _zoomToolStripSplitButton.DefaultItem = _autoToolStripMenuItem;

        // ShowCheckMargin
        if (_zoomToolStripSplitButton.DropDown is ToolStripDropDownMenu menu)
        {
            menu.ShowCheckMargin = true;
            menu.ShowImageMargin = false;
            menu.RenderMode = ToolStripRenderMode.System;
        }

        // Create the ToolStripControlHost
        _pageCounterItem.Alignment = ToolStripItemAlignment.Right;

        _toolStrip1.Items.Add(_pageCounterItem);
        _toolStrip1.Items.Add(_pageToolStripLabel);

        resources.ApplyResources(this, "$this");

        Controls.Add(_previewControl);
        Controls.Add(_toolStrip1);

        ClientSize = new Size(400, 300);
        MinimizeBox = false;
        ShowInTaskbar = false;
        SizeGripStyle = SizeGripStyle.Hide;
        _toolStrip1.ResumeLayout(performLayout: false);
        ResumeLayout(performLayout: false);
        PerformLayout();
    }

    /// <summary>
    ///  Forces the preview to be regenerated every time the dialog comes up
    /// </summary>
#pragma warning disable CS0672 // Member overrides obsolete member
    protected override void OnClosing(CancelEventArgs e)
#pragma warning restore CS0672
    {
#pragma warning disable WFDEV004 // Type or member is obsolete
        base.OnClosing(e);
#pragma warning restore WFDEV004
        _previewControl.InvalidatePreview();
    }

    protected override void CreateHandle()
    {
        // We want to check printer settings before we push the modal message loop,
        // so the user has a chance to catch the exception instead of letting go to
        // the windows forms exception dialog.
        if (Document is not null && !Document.PrinterSettings.IsValid)
        {
            throw new InvalidPrinterException(Document.PrinterSettings);
        }

        base.CreateHandle();
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        Keys keyCode = keyData & Keys.KeyCode;
        if ((keyData & (Keys.Alt | Keys.Control)) == Keys.None)
        {
            switch (keyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    return false;
            }
        }
        else if ((keyData & Keys.Control) == Keys.Control)
        {
            return keyCode switch
            {
                Keys.D1 => PerformPageToolStripButtonClick(_onePageToolStripButton),
                Keys.D2 => PerformPageToolStripButtonClick(_twoPagesToolStripButton),
                Keys.D3 => PerformPageToolStripButtonClick(_threePagesToolStripButton),
                Keys.D4 => PerformPageToolStripButtonClick(_fourPagesToolStripButton),
                Keys.D5 => PerformPageToolStripButtonClick(_sixPagesToolStripButton),
                _ => base.ProcessDialogKey(keyData)
            };

            bool PerformPageToolStripButtonClick(PrintPreviewDialogToolStripButton pageToolStripButton)
            {
                pageToolStripButton.PerformClick();
                _toolStrip1.Focus();
                _toolStrip1.ChangeSelection(pageToolStripButton);
                return true;
            }
        }

        return base.ProcessDialogKey(keyData);
    }

    protected override bool ProcessTabKey(bool forward)
    {
        if (ActiveControl == _previewControl)
        {
            _pageCounter.Focus();
            return true;
        }

        return false;
    }

    internal override bool ShouldSerializeAutoScaleBaseSize()
    {
        // This method is called when the dialog is "contained" on another form.
        // We should use our own base size, not the base size of our container.
        return false;
    }

    internal override bool ShouldSerializeText() => !Text.Equals(SR.PrintPreviewDialog_PrintPreview);

    private void OnCloseToolStripButtonClick(object? sender, EventArgs e) => Close();

    private void PreviewControl_StartPageChanged(object? sender, EventArgs e) =>
        _pageCounter.Value = _previewControl.StartPage + 1;

    private void CheckZoomMenu(ToolStripMenuItem? toChecked)
    {
        foreach (ToolStripMenuItem item in _zoomToolStripSplitButton.DropDownItems)
        {
            item.Checked = toChecked == item;
        }
    }

    private void ZoomAuto(object? sender, EventArgs e)
    {
        ToolStripMenuItem? item = sender as ToolStripMenuItem;
        CheckZoomMenu(item);
        _previewControl.AutoZoom = true;
    }

    private void Zoom500(object? sender, EventArgs e)
    {
        ToolStripMenuItem? item = sender as ToolStripMenuItem;
        CheckZoomMenu(item);
        _previewControl.Zoom = 5.00;
    }

    private void Zoom250(object? sender, EventArgs e)
    {
        ToolStripMenuItem? item = sender as ToolStripMenuItem;
        CheckZoomMenu(item);
        _previewControl.Zoom = 2.50;
    }

    private void Zoom150(object? sender, EventArgs e)
    {
        ToolStripMenuItem? item = sender as ToolStripMenuItem;
        CheckZoomMenu(item);
        _previewControl.Zoom = 1.50;
    }

    private void Zoom100(object? sender, EventArgs e)
    {
        ToolStripMenuItem? item = sender as ToolStripMenuItem;
        CheckZoomMenu(item);
        _previewControl.Zoom = 1.00;
    }

    private void Zoom75(object? sender, EventArgs e)
    {
        ToolStripMenuItem? item = sender as ToolStripMenuItem;
        CheckZoomMenu(item);
        _previewControl.Zoom = .75;
    }

    private void Zoom50(object? sender, EventArgs e)
    {
        ToolStripMenuItem? item = sender as ToolStripMenuItem;
        CheckZoomMenu(item);
        _previewControl.Zoom = .50;
    }

    private void Zoom25(object? sender, EventArgs e)
    {
        ToolStripMenuItem? item = sender as ToolStripMenuItem;
        CheckZoomMenu(item);
        _previewControl.Zoom = .25;
    }

    private void Zoom10(object? sender, EventArgs e)
    {
        ToolStripMenuItem? item = sender as ToolStripMenuItem;
        CheckZoomMenu(item);
        _previewControl.Zoom = .10;
    }

    private void OnCloseToolStripButtonPaint(object? sender, PaintEventArgs e)
    {
        if (sender is ToolStripItem item && !item.Selected)
        {
            Rectangle rect = new(0, 0, item.Bounds.Width - 1, item.Bounds.Height - 1);
            e.Graphics.DrawRectangle(SystemPens.ControlDark, rect);
        }
    }

    private void OnPrintToolStripButtonClick(object? sender, EventArgs e) => _previewControl.Document?.Print();

    private void OnZoomToolStripSplitButtonClick(object? sender, EventArgs e) => ZoomAuto(null, EventArgs.Empty);

    private void OnOnePageToolStripButtonClick(object? sender, EventArgs e)
    {
        _previewControl.Rows = 1;
        _previewControl.Columns = 1;
    }

    private void OnTwoPagesToolStripButtonClick(object? sender, EventArgs e)
    {
        _previewControl.Rows = 1;
        _previewControl.Columns = 2;
    }

    private void OnThreePagesToolStripButtonClick(object? sender, EventArgs e)
    {
        _previewControl.Rows = 1;
        _previewControl.Columns = 3;
    }

    private void OnFourPagesToolStripButtonClick(object? sender, EventArgs e)
    {
        _previewControl.Rows = 2;
        _previewControl.Columns = 2;
    }

    private void OnSixPagesToolStripButtonClick(object? sender, EventArgs e)
    {
        _previewControl.Rows = 2;
        _previewControl.Columns = 3;
    }

    private void UpDownMove(object? sender, EventArgs e)
    {
        int pageNum = ((int)_pageCounter.Value) - 1;
        if (pageNum >= 0)
        {
            // -1 because users like to count from one, and programmers from 0
            _previewControl.StartPage = pageNum;

            // And previewControl_PropertyChanged will change it again,
            // ensuring it stays within legal bounds.
        }
        else
        {
            _pageCounter.Value = _previewControl.StartPage + 1;
        }
    }
}
