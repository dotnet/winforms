// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

#nullable disable

/// <summary>
///  This type is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code.
/// </summary>
[Obsolete(
    Obsoletions.StatusBarMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultEvent(nameof(PanelClick))]
[DefaultProperty(nameof(Text))]
[Designer($"System.Windows.Forms.Design.StatusBarDesigner, {Assemblies.SystemDesign}")]
public partial class StatusBar : Control
{
    // Adding this constructor to suppress creation of a default one.
    public StatusBar() => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color BackColor
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Drawing.Image BackgroundImage
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get => throw null;
        set { }
    }

    [Localizable(true)]
    [DefaultValue(DockStyle.Bottom)]
    public override DockStyle Dock
    {
        get => throw null;
        set { }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override bool DoubleBuffered
    {
        get => throw null;
        set { }
    }

    [Localizable(true)]
    public override Drawing.Font Font
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new ImeMode ImeMode
    {
        get => throw null;
        set { }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [MergableProperty(false)]
    public StatusBarPanelCollection Panels => throw null;

    [DefaultValue(false)]
    public bool ShowPanels
    {
        get => throw null;
        set { }
    }

    [DefaultValue(true)]
    public bool SizingGrip
    {
        get => throw null;
        set { }
    }

    [DefaultValue(false)]
    public new bool TabStop { get => throw null; set { } }

    [Localizable(true)]
    public override string Text { get => throw null; set { } }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackColorChanged
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackgroundImageChanged
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackgroundImageLayoutChanged
    {
        add { }
        remove { }
    }

    public event StatusBarDrawItemEventHandler DrawItem
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler ForeColorChanged
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler ImeModeChanged
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler Paint
    {
        add { }
        remove { }
    }

    public event StatusBarPanelClickEventHandler PanelClick
    {
        add { }
        remove { }
    }

    protected virtual void OnPanelClick(StatusBarPanelClickEventArgs e) { }

    protected virtual void OnDrawItem(StatusBarDrawItemEventArgs sbdievent) { }
}
