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
public class DataGridBoolColumn : DataGridColumnStyle
{
    public DataGridBoolColumn() => throw new PlatformNotSupportedException();

    public DataGridBoolColumn(PropertyDescriptor prop) => throw new PlatformNotSupportedException();

    public DataGridBoolColumn(PropertyDescriptor prop, bool isDefault) =>
        throw new PlatformNotSupportedException();

    public bool AllowNull
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public event EventHandler AllowNullChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public object TrueValue
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public event EventHandler TrueValueChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public object FalseValue
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public event EventHandler FalseValueChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public object NullValue
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected internal override Size GetPreferredSize(Graphics g, object value) => throw new PlatformNotSupportedException();

    protected internal override int GetMinimumHeight() => throw new PlatformNotSupportedException();

    protected internal override int GetPreferredHeight(Graphics g, object value) => throw new PlatformNotSupportedException();

    protected internal override void Abort(int rowNum) => throw new PlatformNotSupportedException();

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

    protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
        => throw new PlatformNotSupportedException();
}
