// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.DataGridBoolColumnMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridBoolColumnDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class DataGridBoolColumn : DataGridColumnStyle
{
    public DataGridBoolColumn() => throw new PlatformNotSupportedException();

    public DataGridBoolColumn(PropertyDescriptor prop) => throw new PlatformNotSupportedException();

    public DataGridBoolColumn(PropertyDescriptor prop, bool isDefault) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object TrueValue
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler TrueValueChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object FalseValue
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler FalseValueChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object NullValue
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected internal override void ConcedeFocus() => throw new PlatformNotSupportedException();

    protected internal override object GetColumnValueAtRow(CurrencyManager source, int rowNum) => throw new PlatformNotSupportedException();

    protected internal override void SetColumnValueAtRow(CurrencyManager source, int rowNum, object value) => throw new PlatformNotSupportedException();

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

    protected internal override void Paint(Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum,
        Brush backBrush,
        Brush foreBrush,
        bool alignToRight) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool AllowNull
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler AllowNullChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected internal override void EnterNullValue() => throw new PlatformNotSupportedException();

    protected internal override void Paint(Graphics g1,
        Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum) => throw new PlatformNotSupportedException();
}
