// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripLabelTests : IDisposable
{
    private readonly ToolStripLabel _toolStripLabel = new();
    public void Dispose() => _toolStripLabel.Dispose();

    [WinFormsFact]
    public void ToolStripLabel_DefaultConstructor_SetsDefaults()
    {
        using ToolStripLabel toolStripLabel = new();

        toolStripLabel.Text.Should().BeEmpty();
        toolStripLabel.Image.Should().BeNull();
        toolStripLabel.IsLink.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripLabel_ConstructorWithText_SetsText()
    {
        string text = "Test Label";

        using ToolStripLabel toolStripLabel = new(text);

        toolStripLabel.Text.Should().Be(text);
        toolStripLabel.Image.Should().BeNull();
        toolStripLabel.IsLink.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripLabel_ConstructorWithImage_SetsImage()
    {
        using Bitmap image = new(10, 10);

        using ToolStripLabel toolStripLabel = new(image);

        toolStripLabel.Image.Should().Be(image);
        toolStripLabel.Text.Should().BeNull();
        toolStripLabel.IsLink.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripLabel_ConstructorWithTextAndImage_SetsTextAndImage()
    {
        using Bitmap image = new(10, 10);
        string text = "Test Label";

        using ToolStripLabel toolStripLabel = new(text, image);

        toolStripLabel.Text.Should().Be(text);
        toolStripLabel.Image.Should().Be(image);
        toolStripLabel.IsLink.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripLabel_ConstructorWithTextImageAndIsLink_SetsTextImageAndIsLink()
    {
        using Bitmap image = new(10, 10);
        string text = "Test Label";
        bool isLink = true;

        using ToolStripLabel toolStripLabel = new(text, image, isLink);

        toolStripLabel.Text.Should().Be(text);
        toolStripLabel.Image.Should().Be(image);
        toolStripLabel.IsLink.Should().Be(isLink);
    }

    [WinFormsFact]
    public void ToolStripLabel_ConstructorWithTextImageIsLinkAndOnClick_SetsTextImageIsLinkAndOnClick()
    {
        using Bitmap image = new(10, 10);
        string text = "Test Label";
        bool isLink = true;
        bool eventHandlerCalled = false;
        EventHandler onClick = (sender, e) => eventHandlerCalled = true;

        using ToolStripLabel toolStripLabel = new(text, image, isLink, onClick);

        toolStripLabel.Text.Should().Be(text);
        toolStripLabel.Image.Should().Be(image);
        toolStripLabel.IsLink.Should().Be(isLink);

        toolStripLabel.PerformClick();
        eventHandlerCalled.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripLabel_ActiveLinkColor_DefaultValue()
    {
        var defaultColor = _toolStripLabel.TestAccessor().Dynamic.IEActiveLinkColor;

        _toolStripLabel.ActiveLinkColor.Should().Be(defaultColor);
    }

    [WinFormsFact]
    public void ToolStripLabel_ActiveLinkColor_SetAndGet()
    {
        Color expectedColor = Color.Red;
        _toolStripLabel.ActiveLinkColor = expectedColor;

        _toolStripLabel.ActiveLinkColor.Should().Be(expectedColor);
    }

    [WinFormsFact]
    public void ToolStripLabel_LinkBehavior_DefaultValue()
    {
        _toolStripLabel.LinkBehavior.Should().Be(LinkBehavior.SystemDefault);
    }

    [WinFormsTheory]
    [InlineData(LinkBehavior.AlwaysUnderline)]
    [InlineData(LinkBehavior.HoverUnderline)]
    [InlineData(LinkBehavior.NeverUnderline)]
    public void ToolStripLabel_LinkBehavior_SetAndGet(LinkBehavior behavior)
    {
        _toolStripLabel.LinkBehavior = behavior;
        _toolStripLabel.LinkBehavior.Should().Be(behavior);
    }

    [WinFormsFact]
    public void ToolStripLabel_LinkVisited_DefaultValue()
    {
        _toolStripLabel.LinkVisited.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripLabel_LinkVisited_SetAndGet()
    {
        _toolStripLabel.LinkVisited = true;
        _toolStripLabel.LinkVisited.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripLabel_VisitedLinkColor_SetAndGet()
    {
        Color expectedColor = Color.Green;
        _toolStripLabel.VisitedLinkColor = expectedColor;

        _toolStripLabel.VisitedLinkColor.Should().Be(expectedColor);
    }

    [WinFormsFact]
    public void ToolStripLabel_VisitedLinkColor_DefaultValue()
    {
        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        Color defaultColor = accessor.IEVisitedLinkColor;

        _toolStripLabel.VisitedLinkColor.Should().Be(defaultColor);
    }

    [WinFormsFact]
    public void ToolStripLabel_LinkColor_SetAndGet()
    {
        Color expectedColor = Color.Blue;
        _toolStripLabel.LinkColor = expectedColor;

        _toolStripLabel.LinkColor.Should().Be(expectedColor);
    }

    [WinFormsFact]
    public void ToolStripLabel_InvalidateLinkFonts_DisposesFonts()
    {
        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        accessor._linkFont = new Font("Arial", 10);
        accessor._hoverLinkFont = new Font("Arial", 10, FontStyle.Underline);

        accessor.InvalidateLinkFonts();

        ((Font)accessor._linkFont).Should().BeNull();
        ((Font)accessor._hoverLinkFont).Should().BeNull();
    }

    [WinFormsFact]
    public void ToolStripLabel_OnFontChanged_InvokesInvalidateLinkFonts()
    {
        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        accessor._linkFont = new Font("Arial", 10);
        accessor._hoverLinkFont = new Font("Arial", 10, FontStyle.Underline);

        _toolStripLabel.Font = new Font("Times New Roman", 12);

        ((Font)accessor._linkFont).Should().BeNull();
        ((Font)accessor._hoverLinkFont).Should().BeNull();
    }

    [WinFormsFact]
    public void ToolStripLabel_ResetActiveLinkColor_SetsActiveLinkColorToDefault()
    {
        _toolStripLabel.ActiveLinkColor = Color.Red;

        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        accessor.ResetActiveLinkColor();

        Color defaultColor = accessor.IEActiveLinkColor;

        _toolStripLabel.ActiveLinkColor.Should().Be(defaultColor);
    }

    [WinFormsFact]
    public void ToolStripLabel_ResetLinkColor_SetsLinkColorToDefault()
    {
        _toolStripLabel.LinkColor = Color.Blue;

        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        accessor.ResetLinkColor();

        Color defaultColor = accessor.IELinkColor;

        _toolStripLabel.LinkColor.Should().Be(defaultColor);
    }

    public static TheoryData<string, Color, bool> ShouldSerializeColorData =>
    new()
    {
        { nameof(ToolStripLabel.ActiveLinkColor), Color.Red, true },
        { nameof(ToolStripLabel.ActiveLinkColor), Color.Empty, false },
        { nameof(ToolStripLabel.LinkColor), Color.Blue, true },
        { nameof(ToolStripLabel.LinkColor), Color.Empty, false },
        { nameof(ToolStripLabel.VisitedLinkColor), Color.Green, true },
        { nameof(ToolStripLabel.VisitedLinkColor), Color.Empty, false }
    };

    [WinFormsTheory]
    [MemberData(nameof(ShouldSerializeColorData))]
    public void ToolStripLabel_ShouldSerializeColor_ReturnsExpected(string propertyName, Color color, bool expected)
    {
        var property = typeof(ToolStripLabel).GetProperty(propertyName);
        property!.SetValue(_toolStripLabel, color);

        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        bool result = propertyName switch
        {
            nameof(ToolStripLabel.ActiveLinkColor) => accessor.ShouldSerializeActiveLinkColor(),
            nameof(ToolStripLabel.LinkColor) => accessor.ShouldSerializeLinkColor(),
            nameof(ToolStripLabel.VisitedLinkColor) => accessor.ShouldSerializeVisitedLinkColor(),
            _ => throw new ArgumentException("Invalid property name", nameof(propertyName))
        };

        result.Should().Be(expected);
    }
}
