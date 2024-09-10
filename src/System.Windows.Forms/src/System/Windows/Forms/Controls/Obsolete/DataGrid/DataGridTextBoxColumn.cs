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
    Obsoletions.DataGridTextBoxColumnMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridTextBoxColumnDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class DataGridTextBoxColumn : DataGridColumnStyle
{
    public DataGridTextBoxColumn() => throw new PlatformNotSupportedException();

    public DataGridTextBoxColumn(PropertyDescriptor prop) => throw new PlatformNotSupportedException();

    public DataGridTextBoxColumn(PropertyDescriptor prop, string format) => throw new PlatformNotSupportedException();

    public DataGridTextBoxColumn(PropertyDescriptor prop, string format, bool isDefault) => throw new PlatformNotSupportedException();

    public DataGridTextBoxColumn(PropertyDescriptor prop, bool isDefault) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual TextBox TextBox => throw new PlatformNotSupportedException();

    protected override void SetDataGridInColumn(DataGrid value) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override PropertyDescriptor PropertyDescriptor => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Format
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IFormatProvider FormatInfo
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool ReadOnly
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected internal override void ConcedeFocus() => throw new PlatformNotSupportedException();

    protected void HideEditBox() => throw new PlatformNotSupportedException();

    protected internal override void UpdateUI(CurrencyManager source, int rowNum, string displayText) => throw new PlatformNotSupportedException();

    protected void EndEdit() => throw new PlatformNotSupportedException();

    protected internal override Size GetPreferredSize(Graphics g, object value) => throw new PlatformNotSupportedException();

    protected internal override int GetMinimumHeight() => throw new PlatformNotSupportedException();

    protected internal override int GetPreferredHeight(Graphics g, object value) => throw new PlatformNotSupportedException();

    protected internal override void Abort(int rowNum) => throw new PlatformNotSupportedException();

    protected internal override void EnterNullValue() => throw new PlatformNotSupportedException();

    protected internal override bool Commit(CurrencyManager dataSource, int rowNum) => throw new PlatformNotSupportedException();

    protected internal override void Edit(CurrencyManager source,
        int rowNum,
        Rectangle bounds,
        bool readOnly,
        string displayText,
        bool cellIsVisible) => throw new PlatformNotSupportedException();

    protected internal override void Paint(Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum,
        bool alignToRight) => throw new PlatformNotSupportedException();

    protected internal override void Paint(Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum,
        Brush backBrush,
        Brush foreBrush,
        bool alignToRight) => throw new PlatformNotSupportedException();

    protected void PaintText(Graphics g,
        Rectangle bounds,
        string text,
        bool alignToRight) => throw new PlatformNotSupportedException();

    protected void PaintText(Graphics g,
        Rectangle textBounds,
        string text,
        Brush backBrush,
        Brush foreBrush,
        bool alignToRight) => throw new PlatformNotSupportedException();

    protected internal override void ReleaseHostedControl() => throw new PlatformNotSupportedException();

    protected internal override void Paint(Graphics g1,
        Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum) => throw new NotImplementedException();
}
