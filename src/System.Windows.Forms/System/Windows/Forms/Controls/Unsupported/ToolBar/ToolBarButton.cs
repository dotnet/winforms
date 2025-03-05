// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

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
[Designer($"System.Windows.Forms.Design.ToolBarButtonDesigner, {Assemblies.SystemDesign}")]
[DefaultProperty(nameof(Text))]
[ToolboxItem(false)]
[DesignTimeVisible(false)]
public class ToolBarButton : Component
{
    public ToolBarButton() => throw new PlatformNotSupportedException();

    public ToolBarButton(string text) => throw new PlatformNotSupportedException();

    [DefaultValue(null)]
    [TypeConverter(typeof(ReferenceConverter))]
    public Menu DropDownMenu
    {
        get => throw null;
        set { }
    }

    [DefaultValue(true)]
    [Localizable(true)]
    public bool Enabled
    {
        get => throw null;
        set { }
    }

    [TypeConverter(typeof(ImageIndexConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {Assemblies.SystemDesign}", typeof(UITypeEditor))]
    [DefaultValue(-1)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [Localizable(true)]
    public int ImageIndex
    {
        get => throw null;
        set { }
    }

    [TypeConverter(typeof(ImageKeyConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {Assemblies.SystemDesign}", typeof(UITypeEditor))]
    [DefaultValue("")]
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.Repaint)]
    public string ImageKey
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    public string Name
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    public ToolBar Parent => throw null;

    [DefaultValue(false)]
    public bool PartialPush
    {
        get => throw null;
        set { }
    }

    [DefaultValue(false)]
    public bool Pushed
    {
        get => throw null;
        set { }
    }

    public Rectangle Rectangle => throw null;

    [DefaultValue(ToolBarButtonStyle.PushButton)]
    [RefreshProperties(RefreshProperties.Repaint)]
    public ToolBarButtonStyle Style
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

    [DefaultValue(true)]
    [Localizable(true)]
    public bool Visible
    {
        get => throw null;
        set { }
    }
}
