// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.DataGridMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
public class DataGridTableStyle : Component, IDataGridEditingService
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool AllowSorting
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler AllowSortingChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color AlternatingBackColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler AlternatingBackColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ResetAlternatingBackColor() => throw new PlatformNotSupportedException();

    protected virtual bool ShouldSerializeAlternatingBackColor() => throw new PlatformNotSupportedException();

    protected bool ShouldSerializeBackColor() => throw new PlatformNotSupportedException();

    protected bool ShouldSerializeForeColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color BackColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler BackColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public void ResetBackColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color ForeColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler ForeColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public void ResetForeColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color GridLineColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler GridLineColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeGridLineColor() => throw new PlatformNotSupportedException();

    public void ResetGridLineColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridLineStyle GridLineStyle
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler GridLineStyleChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color HeaderBackColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler HeaderBackColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeHeaderBackColor() => throw new PlatformNotSupportedException();

    public void ResetHeaderBackColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Font HeaderFont
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler HeaderFontChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public void ResetHeaderFont() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color HeaderForeColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler HeaderForeColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeHeaderForeColor() => throw new PlatformNotSupportedException();

    public void ResetHeaderForeColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color LinkColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler LinkColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeLinkColor() => throw new PlatformNotSupportedException();

    public void ResetLinkColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color LinkHoverColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler LinkHoverColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeLinkHoverColor() => throw new PlatformNotSupportedException();

    public void ResetLinkHoverColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int PreferredColumnWidth
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler PreferredColumnWidthChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int PreferredRowHeight
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler PreferredRowHeightChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected bool ShouldSerializePreferredRowHeight() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ColumnHeadersVisible
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler ColumnHeadersVisibleChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool RowHeadersVisible
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler RowHeadersVisibleChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int RowHeaderWidth
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler RowHeaderWidthChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color SelectionBackColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler SelectionBackColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected bool ShouldSerializeSelectionBackColor() => throw new PlatformNotSupportedException();

    public void ResetSelectionBackColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color SelectionForeColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler SelectionForeColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeSelectionForeColor() => throw new PlatformNotSupportedException();

    public void ResetSelectionForeColor() => throw new PlatformNotSupportedException();

    public static readonly DataGridTableStyle s_defaultTableStyle = new DataGridTableStyle(isDefaultTableStyle: true);

    public DataGridTableStyle(bool isDefaultTableStyle) => throw new PlatformNotSupportedException();

    public DataGridTableStyle() : this(isDefaultTableStyle: false) => throw new PlatformNotSupportedException();

    public DataGridTableStyle(CurrencyManager listManager) : this() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string MappingName
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler MappingNameChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual GridColumnStylesCollection GridColumnStyles => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual DataGrid DataGrid
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool ReadOnly
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler ReadOnlyChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber) => throw new PlatformNotSupportedException();

    public bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort) => throw new PlatformNotSupportedException();

    protected virtual void OnReadOnlyChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnMappingNameChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnAlternatingBackColorChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnForeColorChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnBackColorChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnAllowSortingChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnGridLineColorChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnGridLineStyleChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnHeaderBackColorChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnHeaderFontChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnHeaderForeColorChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnLinkColorChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnLinkHoverColorChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnPreferredRowHeightChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnPreferredColumnWidthChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnColumnHeadersVisibleChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnRowHeadersVisibleChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnRowHeaderWidthChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnSelectionForeColorChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnSelectionBackColorChanged(EventArgs e) => throw new PlatformNotSupportedException();
}
