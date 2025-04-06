// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

/// <summary>
///  This type is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code.
/// </summary>
[Obsolete(
    Obsoletions.MenuMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
[ToolboxItem(false)]
[DesignTimeVisible(false)]
[DefaultEvent(nameof(Click))]
[DefaultProperty(nameof(Text))]
public class MenuItem : Menu
{
    public MenuItem() : this(
        mergeType: MenuMerge.Add,
        mergeOrder: 0,
        shortcut: 0,
        text: null,
        onClick: null,
        onPopup: null,
        onSelect: null,
        items: null) => throw new PlatformNotSupportedException();

    public MenuItem(string text) : this(
        mergeType: MenuMerge.Add,
        mergeOrder: 0,
        shortcut: 0,
        text: text,
        onClick: null,
        onPopup: null,
        onSelect: null,
        items: null) => throw new PlatformNotSupportedException();

    public MenuItem(string text, EventHandler onClick) : this(
        mergeType: MenuMerge.Add,
        mergeOrder: 0,
        shortcut: 0,
        text: text,
        onClick: onClick,
        onPopup: null,
        onSelect: null,
        items: null) => throw new PlatformNotSupportedException();

    public MenuItem(string text, EventHandler onClick, Shortcut shortcut) : this(
        mergeType: MenuMerge.Add,
        mergeOrder: 0,
        shortcut: shortcut,
        text: text,
        onClick: onClick,
        onPopup: null,
        onSelect: null,
        items: null) => throw new PlatformNotSupportedException();

    public MenuItem(string text, MenuItem[] items) : this(
        mergeType: MenuMerge.Add,
        mergeOrder: 0,
        shortcut: 0,
        text: text,
        onClick: null,
        onPopup: null,
        onSelect: null,
        items: items) => throw new PlatformNotSupportedException();

    public MenuItem(
        MenuMerge mergeType,
        int mergeOrder,
        Shortcut shortcut,
        string text,
        EventHandler onClick,
        EventHandler onPopup,
        EventHandler onSelect,
        MenuItem[] items) : base(items: items) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [DefaultValue(false)]
    public bool BarBreak
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [DefaultValue(false)]
    public bool Break
    {
        get => throw null;
        set { }
    }

    [DefaultValue(false)]
    public bool Checked
    {
        get => throw null;
        set { }
    }

    [DefaultValue(false)]
    public bool DefaultItem
    {
        get => throw null;
        set { }
    }

    [DefaultValue(false)]
    public bool OwnerDraw
    {
        get => throw null;
        set { }
    }

    [Localizable(true)]
    [DefaultValue(true)]
    public bool Enabled
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    public int Index
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    public override bool IsParent
    {
        get => throw null;
    }

    [DefaultValue(false)]
    public bool MdiList
    {
        get => throw null;
        set { }
    }

    protected int MenuID
    {
        get => throw null;
    }

    [DefaultValue(MenuMerge.Add)]
    public MenuMerge MergeType
    {
        get => throw null;
        set { }
    }

    [DefaultValue(0)]
    public int MergeOrder
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    public char Mnemonic => throw null;

    [Browsable(false)]
    public Menu Parent
    {
        get => throw null;
    }

    [DefaultValue(false)]
    public bool RadioCheck
    {
        get => throw null;
        set { }
    }

    [Localizable(true)]
    public string Text
    {
        get => throw null;
        set { }
    }

    [Localizable(true)]
    [DefaultValue(Shortcut.None)]
    public Shortcut Shortcut
    {
        get => throw null;
        set { }
    }

    [DefaultValue(true)]
    [Localizable(true)]
    public bool ShowShortcut
    {
        get => throw null;
        set { }
    }

    [Localizable(true)]
    [DefaultValue(true)]
    public bool Visible
    {
        get => throw null;
        set { }
    }

    public event EventHandler Click
    {
        add { }
        remove { }
    }

    public event DrawItemEventHandler DrawItem
    {
        add { }
        remove { }
    }

    public event MeasureItemEventHandler MeasureItem
    {
        add { }
        remove { }
    }

    public event EventHandler Popup
    {
        add { }
        remove { }
    }

    public event EventHandler Select
    {
        add { }
        remove { }
    }

    public virtual MenuItem CloneMenu() => throw null;

    protected void CloneMenu(MenuItem itemSrc) { }

    public virtual MenuItem MergeMenu() => throw null;

    public void MergeMenu(MenuItem itemSrc) { }

    protected virtual void OnClick(EventArgs e) { }

    protected virtual void OnDrawItem(DrawItemEventArgs e) { }

    protected virtual void OnInitMenuPopup(EventArgs e) { }

    protected virtual void OnMeasureItem(MeasureItemEventArgs e) { }

    protected virtual void OnPopup(EventArgs e) { }

    protected virtual void OnSelect(EventArgs e) { }

    public void PerformClick() { }

    public virtual void PerformSelect() { }
}
