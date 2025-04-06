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
public class DataGridTextBoxColumn : DataGridColumnStyle
{
    public DataGridTextBoxColumn() => throw new PlatformNotSupportedException();

    public DataGridTextBoxColumn(PropertyDescriptor prop) => throw new PlatformNotSupportedException();

    public DataGridTextBoxColumn(PropertyDescriptor prop, string format) => throw new PlatformNotSupportedException();

    public DataGridTextBoxColumn(PropertyDescriptor prop, string format, bool isDefault) => throw new PlatformNotSupportedException();

    public DataGridTextBoxColumn(PropertyDescriptor prop, bool isDefault) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    public virtual TextBox TextBox => throw null;

    [DefaultValue(null)]
    [Editor($"System.Windows.Forms.Design.DataGridColumnStyleFormatEditor, {Assemblies.SystemDesign}", typeof(UITypeEditor))]
    public string Format
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IFormatProvider FormatInfo
    {
        get => throw null;
        set { }
    }

    [DefaultValue(null)]
    public override PropertyDescriptor PropertyDescriptor
    {
        set { }
    }

    protected void HideEditBox() { }

    protected void EndEdit() { }

    protected void PaintText(
        Graphics g,
        Rectangle bounds,
        string text,
        bool alignToRight)
    { }

    protected void PaintText(
        Graphics g,
        Rectangle textBounds,
        string text,
        Brush backBrush,
        Brush foreBrush,
        bool alignToRight)
    { }

    protected internal override Size GetPreferredSize(Graphics g, object value) => throw null;

    protected internal override int GetMinimumHeight() => throw null;

    protected internal override int GetPreferredHeight(Graphics g, object value) => throw null;

    protected internal override void Abort(int rowNum) { }

    protected internal override bool Commit(CurrencyManager dataSource, int rowNum) => throw null;

    protected internal override void Edit(
        CurrencyManager source,
        int rowNum,
        Rectangle bounds,
        bool readOnly,
        string displayText,
        bool cellIsVisible)
    { }

    protected internal override void Paint(
        Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum,
        bool alignToRight)
    { }

    protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
    { }
}
