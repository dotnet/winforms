// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Design;

namespace WinFormsControlsTest.UserControls;

[DesignerCategory("Default")]
internal class UserControlWithFontNameEditor : UserControl
{
    private const string Category = "!Fonts";

    public UserControlWithFontNameEditor()
    {
        AutoScaleMode = AutoScaleMode.Font;
    }

    [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category(Category)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string FontNameBold
    {
        get { return "Arial Black"; }
        set { }
    }

    [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category(Category)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string FontNameBoldItalic
    {
        get { return "Forte"; }
        set { }
    }

    [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category(Category)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string FontNameEmpty
    {
        get { return ""; }
        set { }
    }

    [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category(Category)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FontNameInt
    {
        get { return -1345; }
        set { }
    }

    [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category(Category)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string FontNameInvalid
    {
        get { return "some invalid font family"; }
        set { }
    }

    [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category(Category)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string FontNameNull
    {
        get { return null; }
        set { }
    }

    [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category(Category)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string FontNameItalics
    {
        get { return "Lucida Handwriting"; }
        set { }
    }

    [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category(Category)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string FontNameRegular
    {
        get { return "Arial"; }
        set { }
    }
}
