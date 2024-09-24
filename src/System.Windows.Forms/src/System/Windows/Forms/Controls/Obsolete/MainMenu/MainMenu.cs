// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.MainMenuMessage,
    error: false,
    DiagnosticId = Obsoletions.MainMenuDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public class MainMenu : Menu
{
    public MainMenu() : base(items: null) => throw new PlatformNotSupportedException();

    public MainMenu(IContainer container) : this() => throw new PlatformNotSupportedException();

    public MainMenu(MenuItem[] items) : base(items: items) => throw new PlatformNotSupportedException();

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

    public virtual MainMenu CloneMenu() => throw new PlatformNotSupportedException();

    public Form GetForm() => throw new PlatformNotSupportedException();

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public string ToString() => throw new PlatformNotSupportedException();
#pragma warning restore CS0108
}
