// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

/// <summary>
///  This type is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code.
/// </summary>
[Obsolete(
    Obsoletions.MainMenuMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
[ToolboxItemFilter("System.Windows.Forms.MainMenu")]
public class MainMenu : Menu
{
    public MainMenu() : base(items: default) => throw new PlatformNotSupportedException();

    public MainMenu(IContainer container) : this() => throw new PlatformNotSupportedException();

    public MainMenu(MenuItem[] items) : base(items: items) => throw new PlatformNotSupportedException();

    public event EventHandler Collapse
    {
        add { }
        remove { }
    }

    [Localizable(true)]
    [AmbientValue(RightToLeft.Inherit)]
    public virtual RightToLeft RightToLeft
    {
        get => throw null;
        set { }
    }

    public virtual MainMenu CloneMenu() => throw null;

    public Form GetForm() => throw null;

    protected internal virtual void OnCollapse(EventArgs e) { }
}
