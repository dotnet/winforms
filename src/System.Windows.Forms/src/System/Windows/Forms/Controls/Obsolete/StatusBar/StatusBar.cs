// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#nullable disable
[Obsolete(
    Obsoletions.StatusBarMessage,
    error: false,
    DiagnosticId = Obsoletions.StatusBarDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
public partial class StatusBar : Control
{
    public StatusBar() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public Color BackColor
#pragma warning restore CS0114
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public Image BackgroundImage
#pragma warning restore CS0114
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackgroundImageChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public ImageLayout BackgroundImageLayout
#pragma warning restore CS0114
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackgroundImageLayoutChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    protected CreateParams CreateParams => throw new PlatformNotSupportedException();

    protected ImeMode DefaultImeMode => throw new PlatformNotSupportedException();

    protected Size DefaultSize => throw new PlatformNotSupportedException();

    protected bool DoubleBuffered
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DockStyle Dock
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Font Font
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color ForeColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }
#pragma warning restore CS0114

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler ForeColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new ImeMode ImeMode
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler ImeModeChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanelCollection Panels => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public string Text
#pragma warning restore CS0114
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShowPanels
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool SizingGrip
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool TabStop
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event StatusBarDrawItemEventHandler DrawItem
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event StatusBarPanelClickEventHandler PanelClick
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler Paint
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected virtual void OnPanelClick(StatusBarPanelClickEventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnDrawItem(StatusBarDrawItemEventArgs subevent) => throw new PlatformNotSupportedException();

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    protected void CreateHandle() => throw new PlatformNotSupportedException();

    protected void Dispose(bool disposing) => throw new PlatformNotSupportedException();

    protected void OnHandleCreated(EventArgs e) => throw new PlatformNotSupportedException();

    protected void OnHandleDestroyed(EventArgs e) => throw new PlatformNotSupportedException();

    protected void OnMouseDown(MouseEventArgs e) => throw new PlatformNotSupportedException();

    protected void OnLayout(LayoutEventArgs levent) => throw new PlatformNotSupportedException();

    protected void OnResize(EventArgs e) => throw new PlatformNotSupportedException();

    public string ToString() => throw new PlatformNotSupportedException();

    protected void WndProc(ref Message m) => throw new PlatformNotSupportedException();
#pragma warning restore CS0114
}
