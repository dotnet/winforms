// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#nullable disable
#pragma warning disable RS0016
// Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.DataGridMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class DataGrid : Control, ISupportInitialize, IDataGridEditingService
{
    public DataGrid() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool AllowSorting
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color AlternatingBackColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public void ResetAlternatingBackColor() => throw new PlatformNotSupportedException();

    protected virtual bool ShouldSerializeAlternatingBackColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color BackColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public override void ResetBackColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public override void ResetForeColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public BorderStyle BorderStyle
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler BorderStyleChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected override Size DefaultSize => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color CaptionBackColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeCaptionBackColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color CaptionForeColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeCaptionForeColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Font CaptionFont
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string CaptionText
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool CaptionVisible
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler CaptionVisibleChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DataGridCell CurrentCell
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler CurrentCellChanged
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

    protected bool ShouldSerializeSelectionBackColor() => throw new PlatformNotSupportedException();

    public void ResetSelectionBackColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color SelectionForeColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeSelectionForeColor() => throw new PlatformNotSupportedException();

    public void ResetSelectionForeColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object DataSource
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler DataSourceChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string DataMember
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public void SetDataBinding(object dataSource, string dataMember) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int CurrentRowIndex
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public GridTableStylesCollection TableStyles => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color GridLineColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
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
    public DataGridParentRowsLabelStyle ParentRowsLabelStyle
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler ParentRowsLabelStyleChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int FirstVisibleColumn => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool FlatMode
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler FlatModeChanged
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

    protected virtual bool ShouldSerializeHeaderBackColor() => throw new PlatformNotSupportedException();

    public void ResetHeaderBackColor() => throw new PlatformNotSupportedException();

    protected virtual bool ShouldSerializeBackgroundColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color BackgroundColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler BackgroundColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Font HeaderFont
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected bool ShouldSerializeHeaderFont() => throw new PlatformNotSupportedException();

    public void ResetHeaderFont() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color HeaderForeColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeHeaderForeColor() => throw new PlatformNotSupportedException();

    public void ResetHeaderForeColor() => throw new PlatformNotSupportedException();

    protected ScrollBar HorizScrollBar => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color LinkColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public void ResetLinkColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color LinkHoverColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeLinkHoverColor() => throw new PlatformNotSupportedException();

    public void ResetLinkHoverColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool AllowNavigation
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler AllowNavigationChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Cursor Cursor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler CursorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image BackgroundImage
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackgroundImageChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackgroundImageLayoutChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color ParentRowsBackColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeParentRowsBackColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color ParentRowsForeColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected virtual bool ShouldSerializeParentRowsForeColor() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int PreferredColumnWidth
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int PreferredRowHeight
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected bool ShouldSerializePreferredRowHeight() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ReadOnly
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

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ColumnHeadersVisible
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ParentRowsVisible
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler ParentRowsVisibleChanged
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
    public int RowHeaderWidth
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string Text
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler TextChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected ScrollBar VertScrollBar => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int VisibleColumnCount => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int VisibleRowCount => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object this[int rowIndex, int columnIndex]
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object this[DataGridCell cell]
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected virtual void OnBorderStyleChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnCaptionVisibleChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnCurrentCellChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnFlatModeChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnBackgroundColorChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnAllowNavigationChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnParentRowsVisibleChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnParentRowsLabelStyleChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnReadOnlyChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected void OnNavigate(NavigateEventArgs e) => throw new PlatformNotSupportedException();

    protected void OnRowHeaderClick(EventArgs e) => throw new PlatformNotSupportedException();

    protected void OnScroll(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void GridHScrolled(object sender, ScrollEventArgs se) => throw new PlatformNotSupportedException();

    protected virtual void GridVScrolled(object sender, ScrollEventArgs se) => throw new PlatformNotSupportedException();

    protected void OnBackButtonClicked(object sender, EventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnBackColorChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnBindingContextChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnDataSourceChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected void OnShowParentDetailsButtonClicked(object sender, EventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnForeColorChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnFontChanged(EventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnPaintBackground(PaintEventArgs pevent) => throw new PlatformNotSupportedException();

    protected override void OnLayout(LayoutEventArgs levent) => throw new PlatformNotSupportedException();

    protected override void OnHandleCreated(EventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnHandleDestroyed(EventArgs e) => throw new PlatformNotSupportedException();

    protected internal override void OnEnter(EventArgs e) => throw new PlatformNotSupportedException();

    protected internal override void OnLeave(EventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnKeyDown(KeyEventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnKeyPress(KeyPressEventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnMouseDown(MouseEventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnMouseLeave(EventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnMouseMove(MouseEventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnMouseUp(MouseEventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnMouseWheel(MouseEventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnPaint(PaintEventArgs e) => throw new PlatformNotSupportedException();

    protected override void OnResize(EventArgs e) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event NavigateEventHandler Navigate
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected event EventHandler RowHeaderClick
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler Scroll
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ISite Site
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber) => throw new PlatformNotSupportedException();

    public void BeginInit() => throw new PlatformNotSupportedException();

    public void Collapse(int row) => throw new PlatformNotSupportedException();

    protected override AccessibleObject CreateAccessibilityInstance() => throw new PlatformNotSupportedException();

    protected override void Dispose(bool disposing) => throw new PlatformNotSupportedException();

    public bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort) => throw new PlatformNotSupportedException();

    public void Expand(int row) => throw new PlatformNotSupportedException();

    protected virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop, bool isDefault) => throw new PlatformNotSupportedException();

    protected virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop) => throw new PlatformNotSupportedException();

    public void EndInit() => throw new PlatformNotSupportedException();

    public Rectangle GetCurrentCellBounds() => throw new PlatformNotSupportedException();

    public Rectangle GetCellBounds(int row, int col) => throw new PlatformNotSupportedException();

    public Rectangle GetCellBounds(DataGridCell dgc) => throw new PlatformNotSupportedException();

    public HitTestInfo HitTest(int x, int y) => throw new PlatformNotSupportedException();

    public HitTestInfo HitTest(Point position) => throw new PlatformNotSupportedException();

    public bool IsExpanded(int rowNumber) => throw new PlatformNotSupportedException();

    public bool IsSelected(int row) => throw new PlatformNotSupportedException();

    public void NavigateBack() => throw new PlatformNotSupportedException();

    public void NavigateTo(int rowNumber, string relationName) => throw new PlatformNotSupportedException();

    protected override bool ProcessDialogKey(Keys keyData) => throw new PlatformNotSupportedException();

    protected bool ProcessGridKey(KeyEventArgs ke) => throw new PlatformNotSupportedException();

    protected override bool ProcessKeyPreview(ref Message m) => throw new PlatformNotSupportedException();

    protected bool ProcessTabKey(Keys keyData) => throw new PlatformNotSupportedException();

    protected virtual void CancelEditing() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler BackButtonClick
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler ShowParentDetailsButtonClick
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected void ResetSelection() => throw new PlatformNotSupportedException();

    public void Select(int row) => throw new PlatformNotSupportedException();

    public void SubObjectsSiteChange(bool site) => throw new PlatformNotSupportedException();

    public void UnSelect(int row) => throw new PlatformNotSupportedException();

    protected virtual string GetOutputTextDelimiter() => throw new PlatformNotSupportedException();

    [Obsolete(
        Obsoletions.DataGridHitTestInfoMessage,
        error: false,
        DiagnosticId = Obsoletions.DataGridHitTestInfoDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public sealed class HitTestInfo
    {
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Column => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Row => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public HitTestType Type => throw new PlatformNotSupportedException();

        public override bool Equals(object obj) => throw new PlatformNotSupportedException();

        public override int GetHashCode() => throw new PlatformNotSupportedException();

        public override string ToString() => throw new PlatformNotSupportedException();
    }

    [Flags]
    [Obsolete(
        Obsoletions.DataGridHitTestTypeMessage,
        error: false,
        DiagnosticId = Obsoletions.DataGridHitTestTypeDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public enum HitTestType
    {
        None = 0x00000000,
        Cell = 0x00000001,
        ColumnHeader = 0x00000002,
        RowHeader = 0x00000004,
        ColumnResize = 0x00000008,
        RowResize = 0x00000010,
        Caption = 0x00000020,
        ParentRows = 0x00000040
    }
}
