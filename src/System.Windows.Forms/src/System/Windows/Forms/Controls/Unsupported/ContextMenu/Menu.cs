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
[ListBindable(false)]
[ToolboxItemFilter("System.Windows.Forms")]
public abstract partial class Menu : Component
{
    public const int FindHandle = 0;
    public const int FindShortcut = 1;

    protected Menu(MenuItem[] items) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IntPtr Handle => throw null;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual bool IsParent => throw null;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MenuItem MdiListItem => throw null;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Name
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [MergableProperty(false)]
    public MenuItemCollection MenuItems => throw null;

    [Localizable(false)]
    [Bindable(true)]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object Tag
    {
        get => throw null;
        set { }
    }

    protected internal void CloneMenu(Menu menuSrc) { }

    protected virtual IntPtr CreateMenuHandle() => throw null;

    public MenuItem FindMenuItem(int type, IntPtr value) => throw null;

    protected int FindMergePosition(int mergeOrder) => throw null;

    public ContextMenu GetContextMenu() => throw null;

    public MainMenu GetMainMenu() => throw null;

    public virtual void MergeMenu(Menu menuSrc) { }

    protected internal virtual bool ProcessCmdKey(ref Message msg, Keys keyData) => throw null;
}
