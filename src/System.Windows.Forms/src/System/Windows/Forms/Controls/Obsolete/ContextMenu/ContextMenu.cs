// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.ContextMenuMessage,
    error: false,
    DiagnosticId = Obsoletions.ContextMenuDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class ContextMenu : Menu
{
    public ContextMenu() : base(null)
        => throw new PlatformNotSupportedException();

    public ContextMenu(MenuItem[] menuItems) : base(menuItems)
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Control SourceControl
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler Popup
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler Collapse
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual RightToLeft RightToLeft
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public void Show(Control control, Point pos)
        => throw new PlatformNotSupportedException();

    public void Show(Control control, Point pos, LeftRightAlignment alignment)
        => throw new PlatformNotSupportedException();
}
