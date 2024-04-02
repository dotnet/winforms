// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#nullable disable
#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.DataGridTextBoxColumnMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridTextBoxColumnDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class DataGridTextBoxColumn : DataGridColumnStyle
{
    public DataGridTextBoxColumn() : this(null, null)
        => throw new PlatformNotSupportedException();

    public DataGridTextBoxColumn(PropertyDescriptor prop)
    : this(prop, null, false)
        => throw new PlatformNotSupportedException();

    public DataGridTextBoxColumn(PropertyDescriptor prop, string format)
        : this(prop, format, false)
        => throw new PlatformNotSupportedException();

    public DataGridTextBoxColumn(PropertyDescriptor prop, string format, bool isDefault)
        : base(prop, isDefault)
        => throw new PlatformNotSupportedException();

    public DataGridTextBoxColumn(PropertyDescriptor prop, bool isDefault)
        : this(prop, null, isDefault)
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual TextBox TextBox
    {
        get => throw new PlatformNotSupportedException();
    }

    protected override void SetDataGridInColumn(DataGrid value)
    {
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override PropertyDescriptor PropertyDescriptor
    {
        set => throw new PlatformNotSupportedException();
    }

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

    protected internal override void ConcedeFocus()
    {
    }

    protected void HideEditBox()
    {
    }

    protected internal override void UpdateUI(CurrencyManager source, int rowNum, string displayText)
    {
    }

    protected void EndEdit()
    {
    }

    protected internal override Size GetPreferredSize(Graphics g, object value)
        => throw new PlatformNotSupportedException();

    protected internal override int GetMinimumHeight()
        => throw new PlatformNotSupportedException();

    protected internal override int GetPreferredHeight(Graphics g, object value)
        => throw new PlatformNotSupportedException();

    protected internal override void Abort(int rowNum)
    {
    }

    protected internal override void EnterNullValue()
    {
    }

    protected internal override bool Commit(CurrencyManager dataSource, int rowNum)
        => throw new PlatformNotSupportedException();

    protected internal override void Edit(CurrencyManager source,
        int rowNum,
        Rectangle bounds,
        bool readOnly,
        string displayText,
        bool cellIsVisible)
    {
    }

    protected internal override void Paint(Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum,
        bool alignToRight)
        => throw new PlatformNotSupportedException();

    protected internal override void Paint(Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum,
        Brush backBrush,
        Brush foreBrush,
        bool alignToRight)
        => throw new PlatformNotSupportedException();

    protected void PaintText(Graphics g,
        Rectangle bounds,
        string text,
        bool alignToRight)
        => throw new PlatformNotSupportedException();

    protected void PaintText(Graphics g,
        Rectangle textBounds,
        string text,
        Brush backBrush,
        Brush foreBrush,
        bool alignToRight)
        => throw new PlatformNotSupportedException();

    protected internal override void ReleaseHostedControl()
        => throw new PlatformNotSupportedException();

    protected internal override void Paint(Graphics g1,
        Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum)
        => throw new NotImplementedException();
}
