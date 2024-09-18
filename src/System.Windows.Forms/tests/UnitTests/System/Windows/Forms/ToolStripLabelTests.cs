// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripLabelTests : IDisposable
{
    private readonly ToolStripLabel _toolStripLabel;

    public ToolStripLabelTests()
    {
        _toolStripLabel = new();
    }

    public void Dispose() => _toolStripLabel.Dispose();

    [WinFormsFact]
    public void ToolStripLabel_ConstructorWithImage_SetsImage()
    {
        using Bitmap image = new(10, 10);
        _toolStripLabel.Image = image;
        _toolStripLabel.Text = null;

        _toolStripLabel.Image.Should().Be(image);
        _toolStripLabel.Text.Should().BeNull();
        _toolStripLabel.IsLink.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripLabel_ConstructorWithTextAndImage_SetsTextAndImage()
    {
        using Bitmap image = new(10, 10);
        string text = "Test Label";
        _toolStripLabel.Text = text;
        _toolStripLabel.Image = image;

        _toolStripLabel.Text.Should().Be(text);
        _toolStripLabel.Image.Should().Be(image);
        _toolStripLabel.IsLink.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripLabel_ConstructorWithTextImageAndIsLink_SetsTextImageAndIsLink()
    {
        using Bitmap image = new(10, 10);
        string text = "Test Label";
        bool isLink = true;
        _toolStripLabel.Text = text;
        _toolStripLabel.Image = image;
        _toolStripLabel.IsLink = isLink;

        _toolStripLabel.Text.Should().Be(text);
        _toolStripLabel.Image.Should().Be(image);
        _toolStripLabel.IsLink.Should().Be(isLink);
    }

    [WinFormsFact]
    public void ToolStripLabel_ConstructorWithTextImageIsLinkAndOnClick_SetsTextImageIsLinkAndOnClick()
    {
        using Bitmap image = new(10, 10);
        string text = "Test Label";
        bool isLink = true;
        bool eventHandlerCalled = false;
        EventHandler onClick = (sender, e) => eventHandlerCalled = true;

        _toolStripLabel.Text = text;
        _toolStripLabel.Image = image;
        _toolStripLabel.IsLink = isLink;
        _toolStripLabel.Click += onClick;

        _toolStripLabel.Text.Should().Be(text);
        _toolStripLabel.Image.Should().Be(image);
        _toolStripLabel.IsLink.Should().Be(isLink);

        _toolStripLabel.PerformClick();
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
    public void ToolStripLabel_OnMouseEnter_ChangesCursorToHand_WhenIsLink()
    {
        _toolStripLabel.IsLink = true;
        ToolStrip toolStrip = new();
        toolStrip.Items.Add(_toolStripLabel);
        toolStrip.Cursor = Cursors.Default;

        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        accessor.OnMouseEnter(EventArgs.Empty);

        toolStrip.Cursor.Should().Be(Cursors.Default);
    }

    [WinFormsFact]
    public void ToolStripLabel_OnMouseEnter_DoesNotChangeCursor_WhenNotIsLink()
    {
        _toolStripLabel.IsLink = false;
        ToolStrip toolStrip = new();
        toolStrip.Items.Add(_toolStripLabel);
        toolStrip.Cursor = Cursors.Default;

        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        accessor.OnMouseEnter(EventArgs.Empty);

        toolStrip.Cursor.Should().Be(Cursors.Default);
    }

    [WinFormsFact]
    public void ToolStripLabel_OnMouseLeave_ResetsCursor_WhenIsLink()
    {
        _toolStripLabel.IsLink = true;
        ToolStrip toolStrip = new();
        toolStrip.Items.Add(_toolStripLabel);
        toolStrip.Cursor = Cursors.Hand;
        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        accessor._lastCursor = Cursors.Default;

        accessor.OnMouseLeave(EventArgs.Empty);

        toolStrip.Cursor.Should().Be(Cursors.Hand);
    }

    [WinFormsFact]
    public void ToolStripLabel_OnMouseLeave_DoesNotChangeCursor_WhenNotIsLink()
    {
        _toolStripLabel.IsLink = false;
        ToolStrip toolStrip = new();
        toolStrip.Items.Add(_toolStripLabel);
        toolStrip.Cursor = Cursors.Hand;

        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        accessor.OnMouseLeave(EventArgs.Empty);

        toolStrip.Cursor.Should().Be(Cursors.Hand);
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

    [WinFormsFact]
    public void ToolStripLabel_ShouldSerializeActiveLinkColor_ReturnsTrue_WhenActiveLinkColorIsNotEmpty()
    {
        _toolStripLabel.ActiveLinkColor = Color.Red;

        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        bool result = accessor.ShouldSerializeActiveLinkColor();

        result.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripLabel_ShouldSerializeActiveLinkColor_ReturnsFalse_WhenActiveLinkColorIsEmpty()
    {
        _toolStripLabel.ActiveLinkColor = Color.Empty;

        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        bool result = accessor.ShouldSerializeActiveLinkColor();

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripLabel_ShouldSerializeLinkColor_ReturnsTrue_WhenLinkColorIsNotEmpty()
    {
        _toolStripLabel.LinkColor = Color.Blue;

        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        bool result = accessor.ShouldSerializeLinkColor();

        result.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripLabel_ShouldSerializeLinkColor_ReturnsFalse_WhenLinkColorIsEmpty()
    {
        _toolStripLabel.LinkColor = Color.Empty;

        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        bool result = accessor.ShouldSerializeLinkColor();

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripLabel_ShouldSerializeVisitedLinkColor_ReturnsTrue_WhenVisitedLinkColorIsNotEmpty()
    {
        _toolStripLabel.VisitedLinkColor = Color.Green;

        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        bool result = accessor.ShouldSerializeVisitedLinkColor();

        result.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripLabel_ShouldSerializeVisitedLinkColor_ReturnsFalse_WhenVisitedLinkColorIsEmpty()
    {
        _toolStripLabel.VisitedLinkColor = Color.Empty;

        var accessor = _toolStripLabel.TestAccessor().Dynamic;
        bool result = accessor.ShouldSerializeVisitedLinkColor();

        result.Should().BeFalse();
    } 
}
