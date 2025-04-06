// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

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
[ToolboxItem(false)]
[DesignTimeVisible(false)]
public class DataGridTableStyle : Component, IDataGridEditingService
{
    public DataGridTableStyle() : this(isDefaultTableStyle: false) => throw new PlatformNotSupportedException();

    public DataGridTableStyle(bool isDefaultTableStyle) => throw new PlatformNotSupportedException();

    public DataGridTableStyle(CurrencyManager listManager) : this() => throw new PlatformNotSupportedException();

    [DefaultValue(true)]
    public bool AllowSorting
    {
        get => throw null;
        set { }
    }

    public event EventHandler AllowSortingChanged
    {
        add { }
        remove { }
    }

    public Color AlternatingBackColor
    {
        get => throw null;
        set { }
    }

    public event EventHandler AlternatingBackColorChanged
    {
        add { }
        remove { }
    }

    public void ResetAlternatingBackColor() { }

    protected virtual bool ShouldSerializeAlternatingBackColor() => throw null;

    protected bool ShouldSerializeBackColor() => throw null;

    protected bool ShouldSerializeForeColor() => throw null;

    public Color BackColor
    {
        get => throw null;
        set { }
    }

    public event EventHandler BackColorChanged
    {
        add { }
        remove { }
    }

    public void ResetBackColor() { }

    public Color ForeColor
    {
        get => throw null;
        set { }
    }

    public event EventHandler ForeColorChanged
    {
        add { }
        remove { }
    }

    public void ResetForeColor() { }

    public Color GridLineColor
    {
        get => throw null;
        set { }
    }

    public event EventHandler GridLineColorChanged
    {
        add { }
        remove { }
    }

    protected virtual bool ShouldSerializeGridLineColor() => throw null;

    public void ResetGridLineColor() { }

    [DefaultValue(DataGridLineStyle.Solid)]
    public DataGridLineStyle GridLineStyle
    {
        get => throw null;
        set { }
    }

    public event EventHandler GridLineStyleChanged
    {
        add { }
        remove { }
    }

    public Color HeaderBackColor
    {
        get => throw null;
        set { }
    }

    public event EventHandler HeaderBackColorChanged
    {
        add { }
        remove { }
    }

    protected virtual bool ShouldSerializeHeaderBackColor() => throw null;

    public void ResetHeaderBackColor() { }

    [Localizable(true)]
    [AmbientValue(null)]
    public Font HeaderFont
    {
        get => throw null;
        set { }
    }

    public event EventHandler HeaderFontChanged
    {
        add { }
        remove { }
    }

    public void ResetHeaderFont() { }

    public Color HeaderForeColor
    {
        get => throw null;
        set { }
    }

    public event EventHandler HeaderForeColorChanged
    {
        add { }
        remove { }
    }

    protected virtual bool ShouldSerializeHeaderForeColor() => throw null;

    public void ResetHeaderForeColor() { }

    public Color LinkColor
    {
        get => throw null;
        set { }
    }

    public event EventHandler LinkColorChanged
    {
        add { }
        remove { }
    }

    protected virtual bool ShouldSerializeLinkColor() => throw null;

    public void ResetLinkColor() { }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color LinkHoverColor
    {
        get => throw null;
        set { }
    }

    public event EventHandler LinkHoverColorChanged
    {
        add { }
        remove { }
    }

    protected virtual bool ShouldSerializeLinkHoverColor() => throw null;

    public void ResetLinkHoverColor() { }

    [DefaultValue(75)]
    [Localizable(true)]
    [TypeConverter(typeof(DataGridPreferredColumnWidthTypeConverter))]
    public int PreferredColumnWidth
    {
        get => throw null;
        set { }
    }

    public event EventHandler PreferredColumnWidthChanged
    {
        add { }
        remove { }
    }

    [Localizable(true)]
    public int PreferredRowHeight
    {
        get => throw null;
        set { }
    }

    public event EventHandler PreferredRowHeightChanged
    {
        add { }
        remove { }
    }

    protected bool ShouldSerializePreferredRowHeight() => throw null;

    [DefaultValue(true)]
    public bool ColumnHeadersVisible
    {
        get => throw null;
        set { }
    }

    public event EventHandler ColumnHeadersVisibleChanged
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

    public event EventHandler RowHeadersVisibleChanged
    {
        add { }
        remove { }
    }

    [DefaultValue(35)]
    [Localizable(true)]
    public int RowHeaderWidth
    {
        get => throw null;
        set { }
    }

    public event EventHandler RowHeaderWidthChanged
    {
        add { }
        remove { }
    }

    public Color SelectionBackColor
    {
        get => throw null;
        set { }
    }

    public event EventHandler SelectionBackColorChanged
    {
        add { }
        remove { }
    }

    protected bool ShouldSerializeSelectionBackColor() => throw null;

    public void ResetSelectionBackColor() { }

    [Description("The foreground color for the current data grid row")]
    public Color SelectionForeColor
    {
        get => throw null;
        set { }
    }

    public event EventHandler SelectionForeColorChanged
    {
        add { }
        remove { }
    }

    protected virtual bool ShouldSerializeSelectionForeColor() => throw null;

    public void ResetSelectionForeColor() { }

    public static readonly DataGridTableStyle DefaultTableStyle;

    [Editor($"System.Windows.Forms.Design.DataGridTableStyleMappingNameEditor, {Assemblies.SystemDesign}", typeof(UITypeEditor))]
    [DefaultValue("")]
    public string MappingName
    {
        get => throw null;
        set { }
    }

    public event EventHandler MappingNameChanged
    {
        add { }
        remove { }
    }

    [Localizable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public virtual GridColumnStylesCollection GridColumnStyles => throw null;

    [Browsable(false)]
    public virtual DataGrid DataGrid
    {
        get => throw null;
        set { }
    }

    [DefaultValue(false)]
    public virtual bool ReadOnly
    {
        get => throw null;
        set { }
    }

    public event EventHandler ReadOnlyChanged
    {
        add { }
        remove { }
    }

    public bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber) => throw null;

    public bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort) => throw null;

    protected internal virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop, bool isDefault) => throw null;

    protected internal virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop) => throw null;

    protected virtual void OnReadOnlyChanged(EventArgs e) { }

    protected virtual void OnMappingNameChanged(EventArgs e) { }

    protected virtual void OnAlternatingBackColorChanged(EventArgs e) { }

    protected virtual void OnForeColorChanged(EventArgs e) { }

    protected virtual void OnBackColorChanged(EventArgs e) { }

    protected virtual void OnAllowSortingChanged(EventArgs e) { }

    protected virtual void OnGridLineColorChanged(EventArgs e) { }

    protected virtual void OnGridLineStyleChanged(EventArgs e) { }

    protected virtual void OnHeaderBackColorChanged(EventArgs e) { }

    protected virtual void OnHeaderFontChanged(EventArgs e) { }

    protected virtual void OnHeaderForeColorChanged(EventArgs e) { }

    protected virtual void OnLinkColorChanged(EventArgs e) { }

    protected virtual void OnLinkHoverColorChanged(EventArgs e) { }

    protected virtual void OnPreferredRowHeightChanged(EventArgs e) { }

    protected virtual void OnPreferredColumnWidthChanged(EventArgs e) { }

    protected virtual void OnColumnHeadersVisibleChanged(EventArgs e) { }

    protected virtual void OnRowHeadersVisibleChanged(EventArgs e) { }

    protected virtual void OnRowHeaderWidthChanged(EventArgs e) { }

    protected virtual void OnSelectionForeColorChanged(EventArgs e) { }

    protected virtual void OnSelectionBackColorChanged(EventArgs e) { }
}
