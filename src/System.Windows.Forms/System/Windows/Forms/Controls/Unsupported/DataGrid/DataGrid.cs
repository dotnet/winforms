// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

#nullable disable

/// <summary>
///  This type is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code.
/// </summary>
[Obsolete(
    Obsoletions.DataGridMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[Designer($"System.Windows.Forms.Design.DataGridDesigner, {Assemblies.SystemDesign}")]
[DefaultProperty(nameof(DataSource))]
[DefaultEvent(nameof(Navigate))]
[ComplexBindingProperties(nameof(DataSource), nameof(DataMember))]
public partial class DataGrid : Control, ISupportInitialize, IDataGridEditingService
{
    // Implement the default constructor explicitly to ensure that class can't be constructed.
    public DataGrid() => throw new PlatformNotSupportedException();

    [DefaultValue(true)]
    public bool AllowNavigation
    {
        get => throw null;
        set { }
    }

    [DefaultValue(true)]
    public bool AllowSorting
    {
        get => throw null;
        set { }
    }

    public Color AlternatingBackColor
    {
        get => throw null;
        set { }
    }

    public Color BackgroundColor
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image BackgroundImage
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get => throw null;
        set { }
    }

    [DefaultValue(BorderStyle.Fixed3D)]
    [DispId(-504)]
    public BorderStyle BorderStyle
    {
        get => throw null;
        set { }
    }

    public event EventHandler BorderStyleChanged
    {
        add { }
        remove { }
    }

    public Color CaptionBackColor
    {
        get => throw null;
        set { }
    }

    public Color CaptionForeColor
    {
        get => throw null;
        set { }
    }

    [Localizable(true)]
    [AmbientValue(null)]
    public Font CaptionFont
    {
        get => throw null;
        set { }
    }

    [DefaultValue("")]
    [Localizable(true)]
    public string CaptionText
    {
        get => throw null;
        set { }
    }

    [DefaultValue(true)]
    public bool CaptionVisible
    {
        get => throw null;
        set { }
    }

    public event EventHandler CaptionVisibleChanged
    {
        add { }
        remove { }
    }

    [DefaultValue(true)]
    public bool ColumnHeadersVisible
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataGridCell CurrentCell
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CurrentRowIndex
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Cursor Cursor
    {
        get => throw null;
        set { }
    }

    [DefaultValue(null)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [AttributeProvider(typeof(IListSource))]
    public object DataSource
    {
        get => throw null;
        set { }
    }

    public event EventHandler DataSourceChanged
    {
        add { }
        remove { }
    }

    [DefaultValue(null)]
    [Editor($"System.Windows.Forms.Design.DataMemberListEditor, {Assemblies.SystemDesign}", typeof(UITypeEditor))]
    public string DataMember
    {
        get => throw null;
        set { }
    }

    public event EventHandler CurrentCellChanged
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler CursorChanged
    {
        add { }
        remove { }
    }

    public Color SelectionBackColor
    {
        get => throw null;
        set { }
    }

    public Color SelectionForeColor
    {
        get => throw null;
        set { }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    public GridTableStylesCollection TableStyles => throw null;

    public Color GridLineColor
    {
        get => throw null;
        set { }
    }

    [DefaultValue(DataGridLineStyle.Solid)]
    public DataGridLineStyle GridLineStyle
    {
        get => throw null;
        set { }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DefaultValue(DataGridParentRowsLabelStyle.Both)]
    public DataGridParentRowsLabelStyle ParentRowsLabelStyle
    {
        get => throw null;
        set { }
    }

    public event EventHandler ParentRowsLabelStyleChanged
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    public int FirstVisibleColumn => throw null;

    [DefaultValue(false)]
    public bool FlatMode
    {
        get => throw null;
        set { }
    }

    public event EventHandler FlatModeChanged
    {
        add { }
        remove { }
    }

    public Color HeaderBackColor
    {
        get => throw null;
        set { }
    }

    public Font HeaderFont
    {
        get => throw null;
        set { }
    }

    public Color HeaderForeColor
    {
        get => throw null;
        set { }
    }

    public event EventHandler BackgroundColorChanged
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackgroundImageChanged
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackgroundImageLayoutChanged
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal CurrencyManager ListManager
    {
        get => throw null;
        set { }
    }

    public void ResetAlternatingBackColor() { }

    public void ResetGridLineColor() { }

    public void ResetHeaderBackColor() { }

    public void ResetHeaderFont() { }

    public void ResetSelectionBackColor() { }

    public void ResetSelectionForeColor() { }

    protected bool ShouldSerializeSelectionBackColor() => throw null;

    protected virtual bool ShouldSerializeSelectionForeColor() => throw null;

    public void SetDataBinding(object dataSource, string dataMember) { }

    protected virtual bool ShouldSerializeGridLineColor() => throw null;

    protected virtual bool ShouldSerializeCaptionBackColor() => throw null;

    protected virtual bool ShouldSerializeAlternatingBackColor() => throw null;

    protected virtual bool ShouldSerializeCaptionForeColor() => throw null;

    protected virtual bool ShouldSerializeHeaderBackColor() => throw null;

    protected virtual bool ShouldSerializeBackgroundColor() => throw null;

    protected bool ShouldSerializeHeaderFont() => throw null;

    protected virtual bool ShouldSerializeHeaderForeColor() => throw null;

    public void ResetHeaderForeColor() { }

    protected ScrollBar HorizScrollBar => throw null;

    public Color LinkColor
    {
        get => throw null;
        set { }
    }

    public void ResetLinkColor() { }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color LinkHoverColor
    {
        get => throw null;
        set { }
    }

    protected virtual bool ShouldSerializeLinkHoverColor() => throw null;

    public void ResetLinkHoverColor() { }

    public event EventHandler AllowNavigationChanged
    {
        add { }
        remove { }
    }

    public Color ParentRowsBackColor
    {
        get => throw null;
        set { }
    }

    protected virtual bool ShouldSerializeParentRowsBackColor() => throw null;

    public Color ParentRowsForeColor
    {
        get => throw null;
        set { }
    }

    protected virtual bool ShouldSerializeParentRowsForeColor() => throw null;

    [DefaultValue(75)]
    [TypeConverter(typeof(DataGridPreferredColumnWidthTypeConverter))]
    public int PreferredColumnWidth
    {
        get => throw null;
        set { }
    }

    public int PreferredRowHeight
    {
        get => throw null;
        set { }
    }

    protected bool ShouldSerializePreferredRowHeight() => throw null;

    [DefaultValue(false)]
    public bool ReadOnly
    {
        get => throw null;
        set { }
    }

    public event EventHandler ReadOnlyChanged
    {
        add { }
        remove { }
    }

    [DefaultValue(true)]
    public bool ParentRowsVisible
    {
        get => throw null;
        set { }
    }

    public event EventHandler ParentRowsVisibleChanged
    {
        add { }
        remove { }
    }

    [DefaultValue(true)]
    public bool RowHeadersVisible
    {
        get => throw null;
        set { }
    }

    [DefaultValue(35)]
    public int RowHeaderWidth
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Bindable(false)]
    public override string Text
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected ScrollBar VertScrollBar => throw null;

    [Browsable(false)]
    public int VisibleColumnCount => throw null;

    [Browsable(false)]
    public int VisibleRowCount => throw null;

    public object this[int rowIndex, int columnIndex]
    {
        get => throw null;
        set { }
    }

    public object this[DataGridCell cell]
    {
        get => throw null;
        set { }
    }

    protected virtual void OnBorderStyleChanged(EventArgs e) { }

    protected virtual void OnCaptionVisibleChanged(EventArgs e) { }

    protected virtual void OnCurrentCellChanged(EventArgs e) { }

    protected virtual void OnFlatModeChanged(EventArgs e) { }

    protected virtual void OnBackgroundColorChanged(EventArgs e) { }

    protected virtual void OnAllowNavigationChanged(EventArgs e) { }

    protected virtual void OnParentRowsVisibleChanged(EventArgs e) { }

    protected virtual void OnParentRowsLabelStyleChanged(EventArgs e) { }

    protected virtual void OnReadOnlyChanged(EventArgs e) { }

    protected void OnNavigate(NavigateEventArgs e) { }

    protected void OnRowHeaderClick(EventArgs e) { }

    protected void OnScroll(EventArgs e) { }

    protected virtual void GridHScrolled(object sender, ScrollEventArgs se) { }

    protected virtual void GridVScrolled(object sender, ScrollEventArgs se) { }

    protected void OnBackButtonClicked(object sender, EventArgs e) { }

    protected virtual void OnDataSourceChanged(EventArgs e) { }

    protected void OnShowParentDetailsButtonClicked(object sender, EventArgs e) { }

    public event NavigateEventHandler Navigate
    {
        add { }
        remove { }
    }

    protected event EventHandler RowHeaderClick
    {
        add { }
        remove { }
    }

    public event EventHandler Scroll
    {
        add { }
        remove { }
    }

    public bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber) => throw null;

    public void BeginInit() { }

    public void Collapse(int row) { }

    protected internal virtual void ColumnStartedEditing(Rectangle bounds) { }

    protected internal virtual void ColumnStartedEditing(Control editingControl) { }

    public bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort) => throw null;

    public void Expand(int row) { }

    protected virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop, bool isDefault) => throw null;

    protected virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop) => throw null;

    public void EndInit() { }

    public Rectangle GetCurrentCellBounds() => throw null;

    public Rectangle GetCellBounds(int row, int col) => throw null;

    public Rectangle GetCellBounds(DataGridCell dgc) => throw null;

    public HitTestInfo HitTest(int x, int y) => throw null;

    public HitTestInfo HitTest(Point position) => throw null;

    public bool IsExpanded(int rowNumber) => throw null;

    public bool IsSelected(int row) => throw null;

    public void NavigateBack() { }

    public void NavigateTo(int rowNumber, string relationName) { }

    protected bool ProcessGridKey(KeyEventArgs ke) => throw null;

    protected bool ProcessTabKey(Keys keyData) => throw null;

    protected virtual void CancelEditing() { }

    public event EventHandler BackButtonClick
    {
        add { }
        remove { }
    }

    public event EventHandler ShowParentDetailsButtonClick
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler TextChanged
    {
        add { }
        remove { }
    }

    protected void ResetSelection() { }

    public void Select(int row) { }

    public void SubObjectsSiteChange(bool site) { }

    public void UnSelect(int row) { }

    protected virtual string GetOutputTextDelimiter() => throw null;
}
