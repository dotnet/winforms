// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

#nullable disable

[Obsolete(
    Obsoletions.ToolBarMessage,
    error: false,
    DiagnosticId = Obsoletions.ToolBarDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultEvent(nameof(ButtonClick))]
[Designer($"System.Windows.Forms.Design.ToolBarDesigner, {AssemblyRef.SystemDesign}")]
[DefaultProperty(nameof(Buttons))]
public partial class ToolBar : Control
{
    public ToolBar() => throw new PlatformNotSupportedException();

    public ToolBarAppearance Appearance
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler AutoSizeChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color BackColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image BackgroundImage
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackgroundImageChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler BackgroundImageLayoutChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public BorderStyle BorderStyle
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public ToolBarButtonCollection Buttons => throw new PlatformNotSupportedException();

    public Size ButtonSize
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public bool Divider
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override bool DoubleBuffered
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public bool DropDownArrows
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler ForeColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public ImageList ImageList
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Size ImageSize => throw new PlatformNotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new ImeMode ImeMode
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler ImeModeChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override RightToLeft RightToLeft
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler RightToLeftChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public bool ShowToolTips
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public new bool TabStop
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string Text
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler TextChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public ToolBarTextAlign TextAlign
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public bool Wrappable
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    public event ToolBarButtonClickEventHandler ButtonClick
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    public event ToolBarButtonClickEventHandler ButtonDropDown
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler Paint
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected virtual void OnButtonClick(ToolBarButtonClickEventArgs e) => throw new PlatformNotSupportedException();

    protected virtual void OnButtonDropDown(ToolBarButtonClickEventArgs e) => throw new PlatformNotSupportedException();
}
