// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.StatusBarPanelMessage,
    error: false,
    DiagnosticId = Obsoletions.StatusBarPanelDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public class StatusBarPanel : Component, ISupportInitialize
{
    public StatusBarPanel() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public HorizontalAlignment Alignment
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanelAutoSize AutoSize
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanelBorderStyle BorderStyle
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Icon Icon
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int MinWidth
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
    public StatusBar Parent => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanelStyle Style
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
    public int Width
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public void BeginInit() => throw new PlatformNotSupportedException();

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    protected void Dispose(bool disposing) => throw new PlatformNotSupportedException();
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword

    public void EndInit() => throw new PlatformNotSupportedException();

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public string ToString() => throw new PlatformNotSupportedException();
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
}
