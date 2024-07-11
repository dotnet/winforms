// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class ToolStripTests : Form
{
    public ToolStripTests()
    {
        InitializeComponent();

        toolStrip1.Items.Add(new ToolStripControlHost(new RadioButton() { Text = "RadioButton" })); // RadioButton supports UIA
        toolStrip1.Items.Add(new ToolStripControlHost(new HScrollBar() { Value = 30 })); // HScrollBar doesn't support UIA
        statusStrip1.Items.Add(new ToolStripControlHost(new RadioButton() { Text = "RadioButton" })); // RadioButton supports UIA
        statusStrip1.Items.Add(new ToolStripControlHost(new HScrollBar() { Value = 30 })); // HScrollBar doesn't support UIA

        toolStrip2_Button4.Image = Image.FromFile("Images\\SmallA.bmp");
        toolStrip2_Button4.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
        toolStrip2_Button5.Image = Image.FromFile("Images\\SmallABlue.bmp");
        toolStrip2_Button5.DisplayStyle = ToolStripItemDisplayStyle.Image;
    }
}
