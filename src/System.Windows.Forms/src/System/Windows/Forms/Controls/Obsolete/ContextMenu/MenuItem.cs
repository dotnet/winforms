// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable
#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.MenuItemMessage,
    error: false,
    DiagnosticId = Obsoletions.MenuItemDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class MenuItem : Menu
{
    public MenuItem()
        : this(MenuMerge.Add, 0, 0, null, null, null, null, null)
        => throw new PlatformNotSupportedException();

    public MenuItem(string text)
        : this(MenuMerge.Add, 0, 0, text, null, null, null, null)
        => throw new PlatformNotSupportedException();

    public MenuItem(string text, EventHandler onClick)
        : this(MenuMerge.Add, 0, 0, text, onClick, null, null, null)
        => throw new PlatformNotSupportedException();

    public MenuItem(string text, EventHandler onClick, Shortcut shortcut)
        : this(MenuMerge.Add, 0, shortcut, text, onClick, null, null, null)
        => throw new PlatformNotSupportedException();

    public MenuItem(string text, MenuItem[] items)
        : this(MenuMerge.Add, 0, 0, text, null, null, null, items)
        => throw new PlatformNotSupportedException();

    public MenuItem(MenuMerge mergeType,
        int mergeOrder,
        Shortcut shortcut,
        string text,
        EventHandler onClick,
        EventHandler onPopup,
        EventHandler onSelect,
        MenuItem[] items) : base(items)
        => throw new PlatformNotSupportedException();

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
    public override bool IsParent
    {
        get => throw new PlatformNotSupportedException();
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

    public virtual MenuItem CloneMenu()
        => throw new PlatformNotSupportedException();

    public virtual MenuItem MergeMenu()
        => throw new PlatformNotSupportedException();

    public void MergeMenu(MenuItem itemSrc)
        => throw new PlatformNotSupportedException();

    public void PerformClick()
        => throw new PlatformNotSupportedException();

    public virtual void PerformSelect()
        => throw new PlatformNotSupportedException();

    public override string ToString()
        => throw new PlatformNotSupportedException();
}
