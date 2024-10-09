// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#nullable disable

[Obsolete(
    Obsoletions.DataGridMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
[ToolboxItem(false)]
[DesignTimeVisible(false)]
[DefaultProperty("Header")]
public abstract partial class DataGridColumnStyle : Component, IDataGridColumnStyleEditingNotificationService
{
    public DataGridColumnStyle() => throw new PlatformNotSupportedException();

    public DataGridColumnStyle(PropertyDescriptor prop) => throw new PlatformNotSupportedException();

    public virtual HorizontalAlignment Alignment
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public event EventHandler AlignmentChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public AccessibleObject HeaderAccessibleObject => throw new PlatformNotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual PropertyDescriptor PropertyDescriptor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler PropertyDescriptorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public virtual DataGridTableStyle DataGridTableStyle => throw new PlatformNotSupportedException();

    protected int FontHeight => throw new PlatformNotSupportedException();

    public event EventHandler FontChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public virtual string HeaderText
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public event EventHandler HeaderTextChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public string MappingName
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public event EventHandler MappingNameChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public virtual string NullText
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public event EventHandler NullTextChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public virtual bool ReadOnly
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public event EventHandler ReadOnlyChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public virtual int Width
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public event EventHandler WidthChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected virtual AccessibleObject CreateHeaderAccessibleObject() => throw new PlatformNotSupportedException();

    protected virtual void SetDataGrid(DataGrid value) => throw new PlatformNotSupportedException();

    protected virtual void SetDataGridInColumn(DataGrid value) => throw new PlatformNotSupportedException();

    protected void BeginUpdate() => throw new PlatformNotSupportedException();

    protected void EndUpdate() => throw new PlatformNotSupportedException();

    protected internal abstract Size GetPreferredSize(Graphics g, object value);

    protected internal abstract int GetMinimumHeight();

    protected internal abstract int GetPreferredHeight(Graphics g, object value);

    protected internal virtual object GetColumnValueAtRow(CurrencyManager source, int rowNum) => throw new PlatformNotSupportedException();

    protected virtual void Invalidate() => throw new PlatformNotSupportedException();

    protected void CheckValidDataSource(CurrencyManager value) => throw new PlatformNotSupportedException();

    protected internal abstract void Abort(int rowNum);

    protected internal abstract bool Commit(CurrencyManager dataSource, int rowNum);

    protected internal virtual void Edit(CurrencyManager source,
        int rowNum,
        Rectangle bounds,
        bool readOnly) => throw new PlatformNotSupportedException();

    protected internal virtual void Edit(CurrencyManager source,
        int rowNum,
        Rectangle bounds,
        bool readOnly,
        string displayText) => throw new PlatformNotSupportedException();

    protected internal abstract void Edit(CurrencyManager source,
        int rowNum,
        Rectangle bounds,
        bool readOnly,
        string displayText,
        bool cellIsVisible);

    protected internal virtual void EnterNullValue() => throw new PlatformNotSupportedException();

    protected internal virtual void ConcedeFocus() => throw new PlatformNotSupportedException();

    protected internal abstract void Paint(Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum,
        bool alignToRight);

    protected internal virtual void Paint(Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum,
        Brush backBrush,
        Brush foreBrush,
        bool alignToRight) => throw new PlatformNotSupportedException();

    protected internal abstract void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum);

    protected internal virtual void ReleaseHostedControl() => throw new PlatformNotSupportedException();

    public void ResetHeaderText() => throw new PlatformNotSupportedException();

    protected internal virtual void SetColumnValueAtRow(CurrencyManager source, int rowNum, object value) => throw new PlatformNotSupportedException();

    void IDataGridColumnStyleEditingNotificationService.ColumnStartedEditing(Control editingControl) => throw new PlatformNotSupportedException();

    protected internal virtual void ColumnStartedEditing(Control editingControl) => throw new PlatformNotSupportedException();

    protected internal virtual void UpdateUI(CurrencyManager source, int rowNum, string displayText) => throw new PlatformNotSupportedException();
}
