// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Windows.Forms.Automation;

namespace System.Windows.Forms.Tests;

public class ToolStripStatusLabelTests : IDisposable
{
    private readonly ToolStripStatusLabel _toolStripStatusLabel;

    public ToolStripStatusLabelTests()
    {
        _toolStripStatusLabel = new();
    }

    public void Dispose()
    {
        _toolStripStatusLabel.Dispose();
    }

    [WinFormsFact]
    public void ToolStripStatusLabel_ConstructorWithText_InitializesCorrectly()
    {
        string sampleText = "Sample Text";
        using ToolStripStatusLabel label = new(sampleText);
        label.Text.Should().Be(sampleText);
    }

    [WinFormsFact]
    public void ToolStripStatusLabel_ConstructorWithImage_InitializesCorrectly()
    {
        using Bitmap sampleImage = new(10, 10);
        using ToolStripStatusLabel label = new(sampleImage);
        label.Text.Should().BeNull();
        label.Image.Should().Be(sampleImage);
    }

    [WinFormsFact]
    public void ToolStripStatusLabel_ConstructorWithTextAndImage_InitializesCorrectly()
    {
        string sampleText = "Sample Text";
        using Bitmap sampleImage = new(10, 10);
        using ToolStripStatusLabel label = new(sampleText, sampleImage);
        label.Text.Should().Be(sampleText);
        label.Image.Should().Be(sampleImage);
    }

    [WinFormsFact]
    public void ToolStripStatusLabel_ConstructorWithTextImageAndOnClick_InitializesCorrectly()
    {
        string sampleText = "Sample Text";
        using Bitmap sampleImage = new(10, 10);
        bool clickInvoked = false;
        EventHandler sampleClickHandler = (sender, e) => clickInvoked = true;

        using ToolStripStatusLabel label = new(sampleText, sampleImage, sampleClickHandler);
        label.Text.Should().Be(sampleText);
        label.Image.Should().Be(sampleImage);

        label.TestAccessor().Dynamic.OnClick(null);
        clickInvoked.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripStatusLabel_ConstructorWithTextImageOnClickAndName_InitializesCorrectly()
    {
        bool clickInvoked = false;
        EventHandler onClick = (sender, e) => clickInvoked = true;

        using Bitmap image = new(10, 10);
        using ToolStripStatusLabel toolStripStatusLabel = new("Sample Text", image, onClick, "SampleName");

        toolStripStatusLabel.Text.Should().Be("Sample Text");
        toolStripStatusLabel.Image.Should().Be(image);
        toolStripStatusLabel.Name.Should().Be("SampleName");

        toolStripStatusLabel.PerformClick();
        clickInvoked.Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(ToolStripItemAlignment.Right)]
    [InlineData(ToolStripItemAlignment.Left)]
    public void ToolStripStatusLabel_AlignmentProperty_SetAndGet_ReturnsExpected(ToolStripItemAlignment alignment)
    {
        _toolStripStatusLabel.Alignment = alignment;
        _toolStripStatusLabel.Alignment.Should().Be(alignment);
    }

    [WinFormsTheory]
    [InlineData(Border3DStyle.Raised)]
    [InlineData(Border3DStyle.Sunken)]
    public void ToolStripStatusLabel_BorderStyleProperty_SetAndGet_ReturnsExpected(Border3DStyle borderStyle)
    {
        _toolStripStatusLabel.BorderStyle = borderStyle;
        _toolStripStatusLabel.BorderStyle.Should().Be(borderStyle);
    }

    [WinFormsTheory]
    [InlineData(ToolStripStatusLabelBorderSides.Left)]
    [InlineData(ToolStripStatusLabelBorderSides.Right)]
    [InlineData(ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Bottom)]
    public void ToolStripStatusLabel_BorderSidesProperty_SetAndGet_ReturnsExpected(ToolStripStatusLabelBorderSides borderSides)
    {
        _toolStripStatusLabel.BorderSides = borderSides;
        _toolStripStatusLabel.BorderSides.Should().Be(borderSides);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripStatusLabel_SpringProperty_SetAndGet_ReturnsExpected(bool spring)
    {
        _toolStripStatusLabel.Spring = spring;
        _toolStripStatusLabel.Spring.Should().Be(spring);
    }

    [WinFormsTheory]
    [InlineData(AutomationLiveSetting.Assertive)]
    [InlineData(AutomationLiveSetting.Polite)]
    [InlineData(AutomationLiveSetting.Off)]
    public void ToolStripStatusLabel_LiveSettingProperty_SetAndGet_ReturnsExpected(AutomationLiveSetting liveSetting)
    {
        _toolStripStatusLabel.LiveSetting = liveSetting;
        _toolStripStatusLabel.LiveSetting.Should().Be(liveSetting);
    }
}
