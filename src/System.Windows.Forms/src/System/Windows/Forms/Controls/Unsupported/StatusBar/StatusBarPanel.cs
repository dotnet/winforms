// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

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
[ToolboxItem(false)]
[DesignTimeVisible(false)]
[DefaultProperty(nameof(Text))]
public class StatusBarPanel : Component, ISupportInitialize
{
    // Added public constructor to suppress creation of the default one.
    public StatusBarPanel() => throw new PlatformNotSupportedException();

    [DefaultValue(HorizontalAlignment.Left)]
    [Localizable(true)]
    public HorizontalAlignment Alignment
    {
        get => throw null;
        set { }
    }

    [DefaultValue(StatusBarPanelAutoSize.None)]
    [RefreshProperties(RefreshProperties.All)]
    public StatusBarPanelAutoSize AutoSize
    {
        get => throw null;
        set { }
    }

    [DefaultValue(StatusBarPanelBorderStyle.Sunken)]
    [Runtime.InteropServices.DispId(-504)]
    public StatusBarPanelBorderStyle BorderStyle
    {
        get => throw null;
        set { }
    }

    [DefaultValue(null)]
    [Localizable(true)]
    public Icon Icon
    {
        get => throw null;
        set { }
    }

    [DefaultValue(10)]
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.All)]
    public int MinWidth
    {
        get => throw null;
        set { }
    }

    [Localizable(true)]
    public string Name
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    public StatusBar Parent => throw null;

    [DefaultValue(StatusBarPanelStyle.Text)]
    public StatusBarPanelStyle Style
    {
        get => throw null;
        set { }
    }

    [Localizable(false)]
    [Bindable(true)]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object Tag
    {
        get => throw null;
        set { }
    }

    [Localizable(true)]
    [DefaultValue("")]
    public string Text
    {
        get => throw null;
        set { }
    }

    [Localizable(true)]
    [DefaultValue("")]
    public string ToolTipText
    {
        get => throw null;
        set { }
    }

    [Localizable(true)]
    [DefaultValue(100)]
    public int Width
    {
        get => throw null;
        set { }
    }

    public void BeginInit() { }

    public void EndInit() { }
}
