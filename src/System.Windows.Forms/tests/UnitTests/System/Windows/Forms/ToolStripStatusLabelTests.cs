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
    public void ToolStripStatusLabel_ConstructorWithText_SetsTextProperty()
    {
        string sampleText = "Sample Text";
        _toolStripStatusLabel.Text = sampleText;
        _toolStripStatusLabel.Text.Should().Be(sampleText);
    }

    [WinFormsFact]
    public void ToolStripStatusLabel_ConstructorWithImage_SetsImageProperty()
    {
        using Bitmap sampleImage = new(10, 10);
        _toolStripStatusLabel.Image = sampleImage;
        _toolStripStatusLabel.Image.Should().Be(sampleImage);
    }

    [WinFormsFact]
    public void ToolStripStatusLabel_ConstructorWithTextAndImage_SetsTextAndImageProperties()
    {
        string sampleText = "Sample Text";
        using Bitmap sampleImage = new(10, 10);
        _toolStripStatusLabel.Text = sampleText;
        _toolStripStatusLabel.Image = sampleImage;
        _toolStripStatusLabel.Text.Should().Be(sampleText);
        _toolStripStatusLabel.Image.Should().Be(sampleImage);
    }

    [WinFormsFact]
    public void ToolStripStatusLabel_ConstructorWithTextImageAndOnClick_SetsTextImageAndOnClickProperties()
    {
        string sampleText = "Sample Text";
        using Bitmap sampleImage = new(10, 10);
        bool wasClicked = false;
        EventHandler sampleClickHandler = (sender, e) => wasClicked = true;

        _toolStripStatusLabel.Text = sampleText;
        _toolStripStatusLabel.Image = sampleImage;
        _toolStripStatusLabel.Click += sampleClickHandler;

        _toolStripStatusLabel.Text.Should().Be(sampleText);
        _toolStripStatusLabel.Image.Should().Be(sampleImage);

        _toolStripStatusLabel.TestAccessor().Dynamic.OnClick(null);
        wasClicked.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripStatusLabel_ConstructorWithTextImageOnClickAndName_SetsTextImageOnClickAndNameProperties()
    {
        string sampleText = "Sample Text";
        using Bitmap sampleImage = new(10, 10);
        bool wasClicked = false;
        EventHandler sampleClickHandler = (sender, e) => wasClicked = true;
        string sampleName = "SampleName";

        _toolStripStatusLabel.Text = sampleText;
        _toolStripStatusLabel.Image = sampleImage;
        _toolStripStatusLabel.Click += sampleClickHandler;
        _toolStripStatusLabel.Name = sampleName;

        _toolStripStatusLabel.Text.Should().Be(sampleText);
        _toolStripStatusLabel.Image.Should().Be(sampleImage);
        _toolStripStatusLabel.Name.Should().Be(sampleName);

        _toolStripStatusLabel.TestAccessor().Dynamic.OnClick(null);
        wasClicked.Should().BeTrue();
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
