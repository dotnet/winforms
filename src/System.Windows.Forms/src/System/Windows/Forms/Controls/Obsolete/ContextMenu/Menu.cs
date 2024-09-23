// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable
[Obsolete(
    Obsoletions.MenuMessage,
    error: false,
    DiagnosticId = Obsoletions.MenuDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public abstract partial class Menu : Component
{
    public const int FindHandle = 0;
    public const int FindShortcut = 1;

    protected Menu(MenuItem[] items) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IntPtr Handle => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool IsParent => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public MenuItem MdiListItem => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Name
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public MenuItemCollection MenuItems => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object Tag
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public MenuItem FindMenuItem(int type, IntPtr value) => throw new PlatformNotSupportedException();

    public ContextMenu GetContextMenu() => throw new PlatformNotSupportedException();

    public MainMenu GetMainMenu() => throw new PlatformNotSupportedException();

    public virtual void MergeMenu(Menu menuSrc) => throw new PlatformNotSupportedException();

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public string ToString() => throw new PlatformNotSupportedException();
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
}
