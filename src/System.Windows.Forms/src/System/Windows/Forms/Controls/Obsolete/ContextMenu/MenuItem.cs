// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable
[Obsolete(
    Obsoletions.MenuMessage,
    error: false,
    DiagnosticId = Obsoletions.MenuDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
public class MenuItem : Menu
{
    public MenuItem() : this(mergeType: MenuMerge.Add,
        mergeOrder: 0,
        shortcut: 0,
        text: null,
        onClick: null,
        onPopup: null,
        onSelect: null,
        items: null)
    {
    }

    public MenuItem(string text) : this(mergeType: MenuMerge.Add,
        mergeOrder: 0,
        shortcut: 0,
        text: text,
        onClick: null,
        onPopup: null,
        onSelect: null,
        items: null)
    {
    }

    public MenuItem(string text, EventHandler onClick) : this(mergeType: MenuMerge.Add,
        mergeOrder: 0,
        shortcut: 0,
        text: text,
        onClick: onClick,
        onPopup: null,
        onSelect: null,
        items: null)
    {
    }

    public MenuItem(string text, EventHandler onClick, Shortcut shortcut) : this(mergeType: MenuMerge.Add,
        mergeOrder: 0,
        shortcut: shortcut,
        text: text,
        onClick: onClick,
        onPopup: null,
        onSelect: null,
        items: null)
    {
    }

    public MenuItem(string text, MenuItem[] items) : this(mergeType: MenuMerge.Add,
        mergeOrder: 0,
        shortcut: 0,
        text: text,
        onClick: null,
        onPopup: null,
        onSelect: null,
        items: items)
    {
    }

    public MenuItem(MenuMerge mergeType,
        int mergeOrder,
        Shortcut shortcut,
        string text,
        EventHandler onClick,
        EventHandler onPopup,
        EventHandler onSelect,
        MenuItem[] items) : base(items: items)
    {
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool BarBreak
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Break
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Checked
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool DefaultItem
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool OwnerDraw
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Enabled
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int Index
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool MdiList
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public MenuMerge MergeType
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int MergeOrder
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public char Mnemonic => '\0';

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Menu Parent
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool RadioCheck
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Text
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Shortcut Shortcut
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShowShortcut
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Visible
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler Click
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event DrawItemEventHandler DrawItem
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event MeasureItemEventHandler MeasureItem
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler Popup
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler Select
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public virtual MenuItem CloneMenu() => throw new PlatformNotSupportedException();

    protected void CloneMenu(MenuItem itemSrc) => throw new PlatformNotSupportedException();

    public virtual MenuItem MergeMenu() => throw new PlatformNotSupportedException();

    public void MergeMenu(MenuItem itemSrc) => throw new PlatformNotSupportedException();

    protected virtual void OnClick(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnDrawItem(DrawItemEventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnInitMenuPopup(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnMeasureItem(MeasureItemEventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnPopup(EventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnSelect(EventArgs e) => throw new PlatformNotSupportedException();

    public void PerformClick() => throw new PlatformNotSupportedException();

    public virtual void PerformSelect() => throw new PlatformNotSupportedException();
}
