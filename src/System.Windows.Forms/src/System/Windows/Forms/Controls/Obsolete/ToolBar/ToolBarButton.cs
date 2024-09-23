// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.ToolBarButtonMessage,
    error: false,
    DiagnosticId = Obsoletions.ToolBarButtonDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public class ToolBarButton : Component
{
    public ToolBarButton() => throw new PlatformNotSupportedException();

    public ToolBarButton(string text) => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Menu DropDownMenu
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
    public int ImageIndex
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string ImageKey
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Name
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ToolBar Parent => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool PartialPush
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Pushed
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Rectangle Rectangle => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ToolBarButtonStyle Style
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object Tag
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
    public string ToolTipText
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Visible
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public string ToString() => throw new PlatformNotSupportedException();
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
}
