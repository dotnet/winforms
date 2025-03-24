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
[DefaultProperty("Header")]
public abstract partial class DataGridColumnStyle : Component, IDataGridColumnStyleEditingNotificationService
{
    public DataGridColumnStyle() => throw new PlatformNotSupportedException();

    public DataGridColumnStyle(PropertyDescriptor prop) => throw new PlatformNotSupportedException();

    [Localizable(true)]
    [DefaultValue(HorizontalAlignment.Left)]
    public virtual HorizontalAlignment Alignment
    {
        get => throw null;
        set { }
    }

    public event EventHandler AlignmentChanged
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    public AccessibleObject HeaderAccessibleObject => throw null;

    [DefaultValue(null)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual PropertyDescriptor PropertyDescriptor
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event EventHandler PropertyDescriptorChanged
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    public virtual DataGridTableStyle DataGridTableStyle => throw null;

    protected int FontHeight => throw null;

    public event EventHandler FontChanged
    {
        add { }
        remove { }
    }

    [Localizable(true)]
    public virtual string HeaderText
    {
        get => throw null;
        set { }
    }

    public event EventHandler HeaderTextChanged
    {
        add { }
        remove { }
    }

    [Editor($"System.Windows.Forms.Design.DataGridColumnStyleMappingNameEditor, {Assemblies.SystemDesign}", typeof(UITypeEditor))]
    [Localizable(true)]
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
    public virtual string NullText
    {
        get => throw null;
        set { }
    }

    public event EventHandler NullTextChanged
    {
        add { }
        remove { }
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

    [Localizable(true)]
    [DefaultValue(100)]
    public virtual int Width
    {
        get => throw null;
        set { }
    }

    public event EventHandler WidthChanged
    {
        add { }
        remove { }
    }

    protected virtual AccessibleObject CreateHeaderAccessibleObject() => throw null;

    protected virtual void SetDataGrid(DataGrid value) { }

    protected virtual void SetDataGridInColumn(DataGrid value) { }

    protected void BeginUpdate() { }

    protected void EndUpdate() { }

    protected internal abstract Size GetPreferredSize(Graphics g, object value);

    protected internal abstract int GetMinimumHeight();

    protected internal abstract int GetPreferredHeight(Graphics g, object value);

    protected internal virtual object GetColumnValueAtRow(CurrencyManager source, int rowNum) => throw null;

    protected virtual void Invalidate() { }

    protected void CheckValidDataSource(CurrencyManager value) { }

    protected internal abstract void Abort(int rowNum);

    protected internal abstract bool Commit(CurrencyManager dataSource, int rowNum);

    protected internal virtual void Edit(
        CurrencyManager source,
        int rowNum,
        Rectangle bounds,
        bool readOnly)
    { }

    protected internal virtual void Edit(
        CurrencyManager source,
        int rowNum,
        Rectangle bounds,
        bool readOnly,
        string displayText)
    { }

    protected internal abstract void Edit(
        CurrencyManager source,
        int rowNum,
        Rectangle bounds,
        bool readOnly,
        string displayText,
        bool cellIsVisible);

    protected internal virtual void EnterNullValue() { }

    protected internal virtual void ConcedeFocus() { }

    protected internal abstract void Paint(
        Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum,
        bool alignToRight);

    protected internal virtual void Paint(
        Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum,
        Brush backBrush,
        Brush foreBrush,
        bool alignToRight)
    { }

    protected internal abstract void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum);

    protected internal virtual void ReleaseHostedControl() { }

    public void ResetHeaderText() { }

    protected internal virtual void SetColumnValueAtRow(CurrencyManager source, int rowNum, object value) { }

    void IDataGridColumnStyleEditingNotificationService.ColumnStartedEditing(Control editingControl) { }

    protected internal virtual void ColumnStartedEditing(Control editingControl) { }

    protected internal virtual void UpdateUI(CurrencyManager source, int rowNum, string displayText) { }
}
