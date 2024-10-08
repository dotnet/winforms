// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#nullable disable

[Obsolete(
    Obsoletions.ContextMenuMessage,
    error: false,
    DiagnosticId = Obsoletions.ContextMenuDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
public class ContextMenu : Menu
{
    public ContextMenu() : base(items: null) => throw new PlatformNotSupportedException();

    public ContextMenu(MenuItem[] menuItems) : base(items: menuItems)
        => throw new PlatformNotSupportedException();

    public Control SourceControl => throw new PlatformNotSupportedException();

    public event EventHandler Popup
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public event EventHandler Collapse
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public virtual RightToLeft RightToLeft
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public void Show(Control control, Point pos) => throw new PlatformNotSupportedException();

    public void Show(Control control, Point pos, LeftRightAlignment alignment) => throw new PlatformNotSupportedException();
}
