// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
[DefaultProperty(nameof(Items))]
public partial class ToolStripComboBox : ToolStripControlHost
{
    internal static readonly object s_eventDropDown = new();
    internal static readonly object s_eventDropDownClosed = new();
    internal static readonly object s_eventDropDownStyleChanged = new();
    internal static readonly object s_eventSelectedIndexChanged = new();
    internal static readonly object s_eventSelectionChangeCommitted = new();
    internal static readonly object s_eventTextUpdate = new();

    private static readonly Padding s_dropDownPadding = new(2);
    private static readonly Padding s_padding = new(1, 0, 1, 0);

    public ToolStripComboBox()
        : base(CreateControlInstance())
    {
        ToolStripComboBoxControl combo = (ToolStripComboBoxControl)Control;
        combo.Owner = this;
    }

    public ToolStripComboBox(string? name)
        : this()
    {
        Name = name;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ToolStripComboBox(Control c)
        : base(c)
    {
        throw new NotSupportedException(SR.ToolStripMustSupplyItsOwnComboBox);
    }

    private static ToolStripComboBoxControl CreateControlInstance() => new()
    {
        FlatStyle = FlatStyle.Popup,
        Font = ToolStripManager.DefaultFont
    };

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxAutoCompleteCustomSourceDescr))]
    [Editor($"System.Windows.Forms.Design.ListControlStringCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public AutoCompleteStringCollection AutoCompleteCustomSource
    {
        get { return ComboBox.AutoCompleteCustomSource; }
        set { ComboBox.AutoCompleteCustomSource = value; }
    }

    [DefaultValue(AutoCompleteMode.None)]
    [SRDescription(nameof(SR.ComboBoxAutoCompleteModeDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public AutoCompleteMode AutoCompleteMode
    {
        get { return ComboBox.AutoCompleteMode; }
        set { ComboBox.AutoCompleteMode = value; }
    }

    [DefaultValue(AutoCompleteSource.None)]
    [SRDescription(nameof(SR.ComboBoxAutoCompleteSourceDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public AutoCompleteSource AutoCompleteSource
    {
        get { return ComboBox.AutoCompleteSource; }
        set { ComboBox.AutoCompleteSource = value; }
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

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ComboBox ComboBox => (ComboBox)Control;

    protected override Size DefaultSize => new(100, 22);

    protected internal override Padding DefaultMargin => IsOnDropDown
        ? ScaleHelper.ScaleToDpi(s_dropDownPadding, ScaleHelper.InitialSystemDpi)
        : ScaleHelper.ScaleToDpi(s_padding, ScaleHelper.InitialSystemDpi);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? DoubleClick
    {
        add => base.DoubleClick += value;
        remove => base.DoubleClick -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ComboBoxOnDropDownDescr))]
    public event EventHandler? DropDown
    {
        add => Events.AddHandler(s_eventDropDown, value);
        remove => Events.RemoveHandler(s_eventDropDown, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ComboBoxOnDropDownClosedDescr))]
    public event EventHandler? DropDownClosed
    {
        add => Events.AddHandler(s_eventDropDownClosed, value);
        remove => Events.RemoveHandler(s_eventDropDownClosed, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ComboBoxDropDownStyleChangedDescr))]
    public event EventHandler? DropDownStyleChanged
    {
        add => Events.AddHandler(s_eventDropDownStyleChanged, value);
        remove => Events.RemoveHandler(s_eventDropDownStyleChanged, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ComboBoxDropDownHeightDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue(106)]
    public int DropDownHeight
    {
        get { return ComboBox.DropDownHeight; }
        set { ComboBox.DropDownHeight = value; }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(ComboBoxStyle.DropDown)]
    [SRDescription(nameof(SR.ComboBoxStyleDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public ComboBoxStyle DropDownStyle
    {
        get { return ComboBox.DropDownStyle; }
        set { ComboBox.DropDownStyle = value; }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ComboBoxDropDownWidthDescr))]
    public int DropDownWidth
    {
        get { return ComboBox.DropDownWidth; }
        set { ComboBox.DropDownWidth = value; }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ComboBoxDroppedDownDescr))]
    public bool DroppedDown
    {
        get { return ComboBox.DroppedDown; }
        set { ComboBox.DroppedDown = value; }
    }

    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(FlatStyle.Popup)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxFlatStyleDescr))]
    public FlatStyle FlatStyle
    {
        get { return ComboBox.FlatStyle; }
        set { ComboBox.FlatStyle = value; }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxIntegralHeightDescr))]
    public bool IntegralHeight
    {
        get { return ComboBox.IntegralHeight; }
        set { ComboBox.IntegralHeight = value; }
    }

    /// <summary>
    ///  Collection of the items contained in this ComboBox.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxItemsDescr))]
    [Editor($"System.Windows.Forms.Design.ListControlStringCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    public ComboBox.ObjectCollection Items
    {
        get
        {
            return ComboBox.Items;
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(8)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxMaxDropDownItemsDescr))]
    public int MaxDropDownItems
    {
        get { return ComboBox.MaxDropDownItems; }
        set { ComboBox.MaxDropDownItems = value; }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(0)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxMaxLengthDescr))]
    public int MaxLength
    {
        get { return ComboBox.MaxLength; }
        set { ComboBox.MaxLength = value; }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ComboBoxSelectedIndexDescr))]
    public int SelectedIndex
    {
        get { return ComboBox.SelectedIndex; }
        set { ComboBox.SelectedIndex = value; }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.selectedIndexChangedEventDescr))]
    public event EventHandler? SelectedIndexChanged
    {
        add => Events.AddHandler(s_eventSelectedIndexChanged, value);
        remove => Events.RemoveHandler(s_eventSelectedIndexChanged, value);
    }

    [Browsable(false)]
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ComboBoxSelectedItemDescr))]
    public object? SelectedItem
    {
        get { return ComboBox.SelectedItem; }
        set { ComboBox.SelectedItem = value; }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ComboBoxSelectedTextDescr))]
    public string SelectedText
    {
        get { return ComboBox.SelectedText; }
        set { ComboBox.SelectedText = value; }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ComboBoxSelectionLengthDescr))]
    public int SelectionLength
    {
        get { return ComboBox.SelectionLength; }
        set { ComboBox.SelectionLength = value; }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ComboBoxSelectionStartDescr))]
    public int SelectionStart
    {
        get { return ComboBox.SelectionStart; }
        set { ComboBox.SelectionStart = value; }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ComboBoxSortedDescr))]
    public bool Sorted
    {
        get { return ComboBox.Sorted; }
        set { ComboBox.Sorted = value; }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ComboBoxOnTextUpdateDescr))]
    public event EventHandler? TextUpdate
    {
        add => Events.AddHandler(s_eventTextUpdate, value);
        remove => Events.RemoveHandler(s_eventTextUpdate, value);
    }

    #region WrappedMethods

    public void BeginUpdate() { ComboBox.BeginUpdate(); }
    public void EndUpdate() { ComboBox.EndUpdate(); }
    public int FindString(string? s) { return ComboBox.FindString(s); }
    public int FindString(string? s, int startIndex) { return ComboBox.FindString(s, startIndex); }
    public int FindStringExact(string? s) { return ComboBox.FindStringExact(s); }
    public int FindStringExact(string? s, int startIndex) { return ComboBox.FindStringExact(s, startIndex); }
    public int GetItemHeight(int index) { return ComboBox.GetItemHeight(index); }
    public void Select(int start, int length) { ComboBox.Select(start, length); }
    public void SelectAll() { ComboBox.SelectAll(); }

    #endregion WrappedMethods

    public override Size GetPreferredSize(Size constrainingSize)
    {
        Size preferredSize = base.GetPreferredSize(constrainingSize);
        preferredSize.Width = Math.Max(preferredSize.Width, 75);

        return preferredSize;
    }

    private void HandleDropDown(object? sender, EventArgs e)
    {
        OnDropDown(e);
    }

    private void HandleDropDownClosed(object? sender, EventArgs e)
    {
        OnDropDownClosed(e);
    }

    private void HandleDropDownStyleChanged(object? sender, EventArgs e)
    {
        OnDropDownStyleChanged(e);
    }

    private void HandleSelectedIndexChanged(object? sender, EventArgs e)
    {
        OnSelectedIndexChanged(e);
    }

    private void HandleSelectionChangeCommitted(object? sender, EventArgs e)
    {
        OnSelectionChangeCommitted(e);
    }

    private void HandleTextUpdate(object? sender, EventArgs e)
    {
        OnTextUpdate(e);
    }

    protected virtual void OnDropDown(EventArgs e)
    {
        if (ParentInternal is not null)
        {
            Application.ThreadContext.FromCurrent().RemoveMessageFilter(ParentInternal.RestoreFocusFilter);
            ToolStripManager.ModalMenuFilter.SuspendMenuMode();
        }

        RaiseEvent(s_eventDropDown, e);
    }

    protected virtual void OnDropDownClosed(EventArgs e)
    {
        if (ParentInternal is not null)
        {
            // PERF,

            Application.ThreadContext.FromCurrent().RemoveMessageFilter(ParentInternal.RestoreFocusFilter);
            ToolStripManager.ModalMenuFilter.ResumeMenuMode();
        }

        RaiseEvent(s_eventDropDownClosed, e);
    }

    protected virtual void OnDropDownStyleChanged(EventArgs e)
    {
        RaiseEvent(s_eventDropDownStyleChanged, e);
    }

    protected virtual void OnSelectedIndexChanged(EventArgs e)
    {
        RaiseEvent(s_eventSelectedIndexChanged, e);
    }

    protected virtual void OnSelectionChangeCommitted(EventArgs e)
    {
        RaiseEvent(s_eventSelectionChangeCommitted, e);
    }

    protected virtual void OnTextUpdate(EventArgs e)
    {
        RaiseEvent(s_eventTextUpdate, e);
    }

    protected override void OnSubscribeControlEvents(Control? control)
    {
        if (control is ComboBox comboBox)
        {
            // Please keep this alphabetized and in sync with Unsubscribe.
            comboBox.DropDown += HandleDropDown;
            comboBox.DropDownClosed += HandleDropDownClosed;
            comboBox.DropDownStyleChanged += HandleDropDownStyleChanged;
            comboBox.SelectedIndexChanged += HandleSelectedIndexChanged;
            comboBox.SelectionChangeCommitted += HandleSelectionChangeCommitted;
            comboBox.TextUpdate += HandleTextUpdate;
        }

        base.OnSubscribeControlEvents(control);
    }

    protected override void OnUnsubscribeControlEvents(Control? control)
    {
        if (control is ComboBox comboBox)
        {
            // Please keep this alphabetized and in sync with Unsubscribe.
            comboBox.DropDown -= HandleDropDown;
            comboBox.DropDownClosed -= HandleDropDownClosed;
            comboBox.DropDownStyleChanged -= HandleDropDownStyleChanged;
            comboBox.SelectedIndexChanged -= HandleSelectedIndexChanged;
            comboBox.SelectionChangeCommitted -= HandleSelectionChangeCommitted;
            comboBox.TextUpdate -= HandleTextUpdate;
        }

        base.OnUnsubscribeControlEvents(control);
    }

    private bool ShouldSerializeDropDownWidth()
    {
        return ComboBox.ShouldSerializeDropDownWidth();
    }

    internal override bool ShouldSerializeFont()
    {
        return !Equals(Font, ToolStripManager.DefaultFont);
    }

    public override string ToString()
    {
        return $"{base.ToString()}, Items.Count: {Items.Count}";
    }
}
