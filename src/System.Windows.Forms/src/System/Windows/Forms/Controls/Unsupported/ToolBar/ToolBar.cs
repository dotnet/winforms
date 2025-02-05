// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

#nullable disable

/// <summary>
///  This type is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code.
/// </summary>
[Obsolete(
    Obsoletions.ToolBarMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultEvent(nameof(ButtonClick))]
[Designer($"System.Windows.Forms.Design.ToolBarDesigner, {Assemblies.SystemDesign}")]
[DefaultProperty(nameof(Buttons))]
public partial class ToolBar : Control
{
    // Suppress creation of the default constructor by the compiler. This class should not be constructed.
    public ToolBar() => throw new PlatformNotSupportedException();

    [DefaultValue(ToolBarAppearance.Normal)]
    [Localizable(true)]
    public ToolBarAppearance Appearance
    {
        get => throw null;
        set { }
    }

    [DefaultValue(true)]
    [Localizable(true)]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override bool AutoSize
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color BackColor
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image BackgroundImage
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

    [DefaultValue(BorderStyle.None)]
    [DispId(-504)]
    public BorderStyle BorderStyle
    {
        get => throw null;
        set { }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [MergableProperty(false)]
    public ToolBarButtonCollection Buttons => throw null;

    [RefreshProperties(RefreshProperties.All)]
    [Localizable(true)]
    public Size ButtonSize
    {
        get => throw null;
        set { }
    }

    [DefaultValue(true)]
    public bool Divider
    {
        get => throw null;
        set { }
    }

    [Localizable(true)]
    [DefaultValue(DockStyle.Top)]
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

    [DefaultValue(false)]
    [Localizable(true)]
    public bool DropDownArrows
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

    [DefaultValue(null)]
    public ImageList ImageList
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Size ImageSize => throw null;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new ImeMode ImeMode { get => throw null; set { } }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override RightToLeft RightToLeft { get => throw null; set { } }

    [DefaultValue(false)]
    [Localizable(true)]
    public bool ShowToolTips
    {
        get => throw null;
        set { }
    }

    [DefaultValue(false)]
    public new bool TabStop
    {
        get => throw null; set { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Bindable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string Text
    {
        get => throw null; set { }
    }

    [DefaultValue(ToolBarTextAlign.Underneath)]
    [Localizable(true)]
    public ToolBarTextAlign TextAlign
    {
        get => throw null;
        set { }
    }

    [DefaultValue(true)]
    [Localizable(true)]
    public bool Wrappable
    {
        get => throw null;
        set { }
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public new event EventHandler AutoSizeChanged
    {
        add { }
        remove { }
    }

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

    public event ToolBarButtonClickEventHandler ButtonClick
    {
        add { }
        remove { }
    }

    public event ToolBarButtonClickEventHandler ButtonDropDown
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

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler RightToLeftChanged
    {
        add { }
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler TextChanged
    {
        add { }
        remove { }
    }

    protected virtual void OnButtonClick(ToolBarButtonClickEventArgs e) { }

    protected virtual void OnButtonDropDown(ToolBarButtonClickEventArgs e) { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override void ScaleCore(float dx, float dy) { }
}
