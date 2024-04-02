// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.MainMenuMessage,
    error: false,
    DiagnosticId = Obsoletions.MainMenuDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class MainMenu : Menu
{
    public MainMenu() : base(null)
        => throw new PlatformNotSupportedException();

    public MainMenu(IContainer container) : this()
        => throw new PlatformNotSupportedException();

    public MainMenu(MenuItem[] items) : base(items)
        => throw new PlatformNotSupportedException();

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

    public virtual MainMenu CloneMenu()
        => throw new PlatformNotSupportedException();

    public Form GetForm()
        => throw new PlatformNotSupportedException();

    public override string ToString()
        => throw new PlatformNotSupportedException();
}
