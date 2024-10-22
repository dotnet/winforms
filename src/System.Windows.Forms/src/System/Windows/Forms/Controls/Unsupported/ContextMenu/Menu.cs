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
[Browsable(false)]
[ListBindable(false)]
[ToolboxItemFilter("System.Windows.Forms")]
public abstract partial class Menu : Component
{
    public const int FindHandle = 0;
    public const int FindShortcut = 1;

    protected Menu(MenuItem[] items) => throw new PlatformNotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IntPtr Handle => throw new PlatformNotSupportedException();

    public virtual bool IsParent => throw new PlatformNotSupportedException();

    public MenuItem MdiListItem => throw new PlatformNotSupportedException();

    public string Name
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public MenuItemCollection MenuItems => throw new PlatformNotSupportedException();

    public object Tag
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    protected internal void CloneMenu(Menu menuSrc) => throw new PlatformNotSupportedException();

    protected virtual IntPtr CreateMenuHandle() => throw new PlatformNotSupportedException();

    public MenuItem FindMenuItem(int type, IntPtr value) => throw new PlatformNotSupportedException();

    protected int FindMergePosition(int mergeOrder) => throw new PlatformNotSupportedException();

    public ContextMenu GetContextMenu() => throw new PlatformNotSupportedException();

    public MainMenu GetMainMenu() => throw new PlatformNotSupportedException();

    public virtual void MergeMenu(Menu menuSrc) => throw new PlatformNotSupportedException();

    protected internal virtual bool ProcessCmdKey(ref Message msg, Keys keyData)
         => throw new PlatformNotSupportedException();
}
