// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

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
public class DataGridBoolColumn : DataGridColumnStyle
{
    public DataGridBoolColumn() => throw new PlatformNotSupportedException();

    public DataGridBoolColumn(PropertyDescriptor prop) => throw new PlatformNotSupportedException();

    public DataGridBoolColumn(PropertyDescriptor prop, bool isDefault) => throw new PlatformNotSupportedException();

    [DefaultValue(true)]
    public bool AllowNull
    {
        get => throw null;
        set { }
    }

    public event EventHandler AllowNullChanged
    {
        add { }
        remove { }
    }

    [TypeConverter(typeof(StringConverter))]
    [DefaultValue(true)]
    public object TrueValue
    {
        get => throw null;
        set { }
    }

    public event EventHandler TrueValueChanged
    {
        add { }
        remove { }
    }

    [TypeConverter(typeof(StringConverter))]
    [DefaultValue(false)]
    public object FalseValue
    {
        get => throw null;
        set { }
    }

    public event EventHandler FalseValueChanged
    {
        add { }
        remove { }
    }

    [TypeConverter(typeof(StringConverter))]
    public object NullValue
    {
        get => throw null;
        set { }
    }

    // We need a concrete implementation for an abstract method.
    protected internal override Size GetPreferredSize(Graphics g, object value) => throw null;

    protected internal override int GetMinimumHeight() => throw null;

    protected internal override int GetPreferredHeight(Graphics g, object value) => throw null;

    protected internal override void Abort(int rowNum) { }

    protected internal override bool Commit(CurrencyManager dataSource, int rowNum) => throw null;

    protected internal override void Edit(CurrencyManager source,
        int rowNum,
        Rectangle bounds,
        bool readOnly,
        string displayText,
        bool cellIsVisible)
    { }

    protected internal override void Paint(Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum,
        bool alignToRight)
    { }

    protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
    { }
}
