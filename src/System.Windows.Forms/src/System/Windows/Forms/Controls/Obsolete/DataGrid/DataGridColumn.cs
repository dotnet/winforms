﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#nullable disable
#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.DataGridColumnStyleMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridColumnStyleDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public abstract class DataGridColumnStyle : Component, IDataGridColumnStyleEditingNotificationService
{
    public DataGridColumnStyle()
         => throw new PlatformNotSupportedException();

    public DataGridColumnStyle(PropertyDescriptor prop) : this()
         => throw new PlatformNotSupportedException();

    internal DataGridColumnStyle(PropertyDescriptor prop, bool isDefault) : this(prop)
         => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual HorizontalAlignment Alignment
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler AlignmentChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected internal virtual void UpdateUI(CurrencyManager source, int rowNum, string displayText)
    {
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public AccessibleObject HeaderAccessibleObject
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual PropertyDescriptor PropertyDescriptor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler PropertyDescriptorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected virtual AccessibleObject CreateHeaderAccessibleObject()
         => throw new PlatformNotSupportedException();

    protected virtual void SetDataGrid(DataGrid value)
        => throw new PlatformNotSupportedException();

    protected virtual void SetDataGridInColumn(DataGrid value)
    {
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual DataGridTableStyle DataGridTableStyle
        => throw new PlatformNotSupportedException();

    protected int FontHeight
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler FontChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual string HeaderText
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler HeaderTextChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

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

    public void ResetHeaderText()
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual string NullText
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler NullTextChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
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

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual int Width
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler WidthChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected void BeginUpdate()
    {
    }

    protected void EndUpdate()
    {
    }

    protected internal abstract Size GetPreferredSize(Graphics g, object value);

    protected internal abstract int GetMinimumHeight();

    protected internal abstract int GetPreferredHeight(Graphics g, object value);

    protected internal virtual object GetColumnValueAtRow(CurrencyManager source, int rowNum)
        => throw new PlatformNotSupportedException();

    protected virtual void Invalidate()
    {
    }

    protected void CheckValidDataSource(CurrencyManager value)
    {
    }

    protected internal abstract void Abort(int rowNum);

    protected internal abstract bool Commit(CurrencyManager dataSource, int rowNum);

    protected internal virtual void Edit(CurrencyManager source,
        int rowNum,
        Rectangle bounds,
        bool readOnly)
        => throw new PlatformNotSupportedException();

    protected internal virtual void Edit(CurrencyManager source,
        int rowNum,
        Rectangle bounds,
        bool readOnly,
        string displayText)
        => throw new PlatformNotSupportedException();

    protected internal abstract void Edit(CurrencyManager source,
        int rowNum,
        Rectangle bounds,
        bool readOnly,
        string displayText,
        bool cellIsVisible);

    protected internal virtual void EnterNullValue()
    {
    }

    protected internal virtual void ConcedeFocus()
    {
    }

    protected internal abstract void Paint(Graphics g1,
        Graphics g,
        Rectangle bounds,
        CurrencyManager source,
        int rowNum);

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
        bool alignToRight)
            => throw new PlatformNotSupportedException();

    protected internal virtual void SetColumnValueAtRow(CurrencyManager source, int rowNum, object value)
        => throw new PlatformNotSupportedException();

    void IDataGridColumnStyleEditingNotificationService.ColumnStartedEditing(Control editingControl)
    {
    }

    protected internal virtual void ReleaseHostedControl()
    {
    }

    protected class CompModSwitches
    {
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TraceSwitch DGEditColumnEditing
            => throw new PlatformNotSupportedException();
    }

    [Obsolete(
        Obsoletions.DataGridColumnHeaderAccessibleObjectMessage,
        error: false,
        DiagnosticId = Obsoletions.DataGridColumnHeaderAccessibleObjectDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    internal class DataGridColumnHeaderAccessibleObject : AccessibleObject
    {
        public DataGridColumnHeaderAccessibleObject(DataGridColumnStyle owner) : this()
            => throw new PlatformNotSupportedException();

        public DataGridColumnHeaderAccessibleObject() : base()
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Rectangle Bounds
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Name
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected DataGridColumnStyle Owner
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleObject Parent
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleRole Role
            => throw new PlatformNotSupportedException();

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
            => throw new PlatformNotSupportedException();
    }
}
