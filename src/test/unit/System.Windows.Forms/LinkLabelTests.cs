// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class LinkLabelTests : IDisposable
{
    private readonly LinkLabel _linkLabel = new();

    public void Dispose() => _linkLabel.Dispose();

    [WinFormsFact]
    public void LinkLabel_Constructor()
    {
        _linkLabel.Should().NotBeNull();
        _linkLabel.LinkArea.IsEmpty.Should().BeTrue();
        _linkLabel.LinkArea.Start.Should().Be(0);
        _linkLabel.LinkArea.Length.Should().Be(0);
    }

    [WinFormsFact]
    public void LinkLabel_FlatStyle_Get_ReturnsExpected()
    {
        _linkLabel.FlatStyle.Should().Be(FlatStyle.Standard);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Flat)]
    [InlineData(FlatStyle.Popup)]
    [InlineData(FlatStyle.System)]
    public void LinkLabel_FlatStyle_Set_ReturnsExpected(FlatStyle flatStyle)
    {
        _linkLabel.FlatStyle = flatStyle;

        _linkLabel.FlatStyle.Should().Be(flatStyle);
    }

    [WinFormsFact]
    public void LinkLabel_LinkArea_Get_ReturnsExpected()
    {
        _linkLabel.LinkArea.Should().Be(new LinkArea(0, 0));
    }

    [WinFormsFact]
    public void LinkLabel_LinkArea_Set_ReturnsExpected()
    {
        LinkArea linkArea1 = new LinkArea(1, 2);
        LinkArea linkArea2 = new LinkArea(3, 4);

        _linkLabel.LinkArea = linkArea1;
        _linkLabel.LinkArea.Should().Be(linkArea1);

        _linkLabel.LinkArea = linkArea2;
        _linkLabel.LinkArea.Should().Be(linkArea2);
    }

    [WinFormsTheory]
    [InlineData(-1, 2)]  // Test with negative Start
    [InlineData(1, -2)]  // Test with negative Length
    public void LinkLabel_LinkArea_Set_InvalidValues_ThrowsArgumentOutOfRangeException(int start, int length)
    {
        Action act = () => _linkLabel.LinkArea = new LinkArea(start, length);
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*LinkArea*");
    }

    [WinFormsFact]
    public void LinkLabel_LinkArea_Set_UpdatesSelectability()
    {
        _linkLabel.Text = "Text";
        _linkLabel.LinkArea = new LinkArea(1, 2);
        _linkLabel.TabStop.Should().BeTrue();

        _linkLabel.LinkArea = new LinkArea(0, 0);
        _linkLabel.TabStop.Should().BeFalse();
    }

    [WinFormsFact]
    public void LinkLabel_LinkBehavior_Get_ReturnsExpected()
    {
        _linkLabel.LinkBehavior.Should().Be(LinkBehavior.SystemDefault);
    }

    [WinFormsTheory]
    [InlineData(LinkBehavior.AlwaysUnderline)]
    [InlineData(LinkBehavior.HoverUnderline)]
    [InlineData(LinkBehavior.NeverUnderline)]
    public void LinkLabel_LinkBehavior_Set_ReturnsExpected(LinkBehavior linkBehavior)
    {
        _linkLabel.LinkBehavior = linkBehavior;
        _linkLabel.LinkBehavior.Should().Be(linkBehavior);
    }

    [WinFormsFact]
    public void LinkLabel_LinkVisited_Get_ReturnsExpected()
    {
        _linkLabel.LinkVisited.Should().BeFalse();
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void LinkLabel_LinkVisited_Set_ReturnsExpected(bool expectedVisited)
    {
        _linkLabel.LinkVisited = expectedVisited;
        _linkLabel.LinkVisited.Should().Be(expectedVisited);
    }

    [WinFormsFact]
    public void LinkLabel_LinkVisited_Set_AddsLinkIfNoneExists()
    {
        _linkLabel.LinkVisited = true;
        _linkLabel.Links.Count.Should().Be(1);
        _linkLabel.Links[0].Visited.Should().BeTrue();
    }

    [WinFormsFact]
    public void LinkLabel_LinkVisited_Set_UpdatesExistingLink()
    {
        _linkLabel.Links.Add(new LinkLabel.Link(_linkLabel) { Visited = false });
        _linkLabel.LinkVisited = true;
        _linkLabel.Links[0].Visited.Should().BeTrue();

        _linkLabel.LinkVisited = false;
        _linkLabel.Links[0].Visited.Should().BeFalse();
    }

    [WinFormsFact]
    public void LinkLabel_TabStop_Get_ReturnsExpected()
    {
        _linkLabel.TabStop.Should().BeFalse();
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void LinkLabel_TabStop_Set_ReturnsExpected(bool expectedTabStop)
    {
        _linkLabel.TabStop = expectedTabStop;
        _linkLabel.TabStop.Should().Be(expectedTabStop);
    }

    [WinFormsFact]
    public void LinkLabel_TabStop_Set_RaisesTabStopChangedEvent()
    {
        bool eventRaised = false;
        _linkLabel.TabStopChanged += (sender, e) => eventRaised = true;

        _linkLabel.TabStop = true;
        eventRaised.Should().BeTrue();

        eventRaised = false;
        _linkLabel.TabStop = false;
        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void LinkLabel_Padding_Get_ReturnsExpected()
    {
        _linkLabel.Padding.Should().Be(new Padding(0));
    }

    [WinFormsFact]
    public void LinkLabel_Padding_Set_ReturnsExpected()
    {
        Padding padding1 = new(1, 2, 3, 4);
        Padding padding2 = new(5);

        _linkLabel.Padding = padding1;
        _linkLabel.Padding.Should().Be(padding1);

        _linkLabel.Padding = padding2;
        _linkLabel.Padding.Should().Be(padding2);
    }

    [WinFormsFact]
    public void LinkLabel_VisitedLinkColor_Get_ReturnsExpected()
    {
        _linkLabel.VisitedLinkColor.Should().Be(LinkUtilities.GetVisitedLinkColor());
    }

    [WinFormsFact]
    public void LinkLabel_VisitedLinkColor_Set_ReturnsExpected()
    {
        _linkLabel.VisitedLinkColor = Color.Red;
        _linkLabel.VisitedLinkColor.Should().Be(Color.Red);

        _linkLabel.VisitedLinkColor = Color.Blue;
        _linkLabel.VisitedLinkColor.Should().Be(Color.Blue);
    }

    [WinFormsFact]
    public void LinkLabel_VisitedLinkColor_Set_UpdatesLink()
    {
        _linkLabel.Links.Add(new LinkLabel.Link(_linkLabel) { Visited = true });
        _linkLabel.VisitedLinkColor = Color.Red;
        _linkLabel.VisitedLinkColor.Should().Be(Color.Red);
    }

    [WinFormsFact]
    public void LinkLabel_LinkClicked_RaisesEvent()
    {
        using TestLinkLabel label = new();
        bool eventRaised = false;
        label.LinkClicked += (sender, e) => eventRaised = true;

        var link = new LinkLabel.Link(label);
        label.Links.Add(link);
        label.OnLinkClicked(new LinkLabelLinkClickedEventArgs(link));

        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void LinkLabel_UseCompatibleTextRendering_Get_ReturnsExpected()
    {
        _linkLabel.UseCompatibleTextRendering.Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void LinkLabel_UseCompatibleTextRendering_Set_ReturnsExpected(bool expectedValue)
    {
        _linkLabel.UseCompatibleTextRendering = expectedValue;
        _linkLabel.UseCompatibleTextRendering.Should().Be(expectedValue);
    }

    [WinFormsFact]
    public void LinkLabel_UseCompatibleTextRendering_Set_TriggersLayout()
    {
        bool layoutCalled = false;
        _linkLabel.Layout += (sender, e) => layoutCalled = true;

        _linkLabel.UseCompatibleTextRendering = true;
        _linkLabel.PerformLayout();
        layoutCalled.Should().BeTrue();

        layoutCalled = false;
        _linkLabel.UseCompatibleTextRendering = false;
        _linkLabel.PerformLayout();
        layoutCalled.Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    public void LinkLabel_OnPaint_EmptyText_DoesNotKeepInvalidating(string? text)
    {
        // Regression test for https://github.com/dotnet/winforms/issues/10515: painting a LinkLabel without text
        // used to rebuild the link collection on every paint, and each rebuild invalidated the control again,
        // which resulted in an endless paint loop.
        using TestLinkLabel linkLabel = new() { Size = new Size(100, 20), Text = text };
        linkLabel.CreateControl();

        using Bitmap bitmap = new(linkLabel.Width, linkLabel.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, linkLabel.ClientRectangle);

        // The first paint calculates the text layout and is allowed to invalidate the control.
        linkLabel.OnPaint(e);

        int invalidatedCount = 0;
        linkLabel.Invalidated += (sender, args) => invalidatedCount++;

        linkLabel.OnPaint(e);
        linkLabel.OnPaint(e);

        invalidatedCount.Should().Be(0);
    }

    [WinFormsFact]
    public void LinkLabel_OnPaint_NonEmptyText_DoesNotKeepInvalidating()
    {
        // A non-empty paint must establish a valid text layout so subsequent paints can reuse it.
        using TestLinkLabel linkLabel = new() { Size = new Size(100, 20), Text = "Some text" };
        linkLabel.CreateControl();

        using Bitmap bitmap = new(linkLabel.Width, linkLabel.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, linkLabel.ClientRectangle);

        linkLabel.OnPaint(e);
        ((bool)linkLabel.TestAccessor.Dynamic._textLayoutValid).Should().BeTrue();

        int invalidatedCount = 0;
        linkLabel.Invalidated += (sender, args) => invalidatedCount++;

        linkLabel.OnPaint(e);
        linkLabel.OnPaint(e);

        invalidatedCount.Should().Be(0);
    }

    [WinFormsFact]
    public void LinkLabel_TextChange_InvalidatesLayoutOnce()
    {
        using TestLinkLabel linkLabel = new() { Size = new Size(100, 20), Text = "Initial text" };
        linkLabel.CreateControl();

        using Bitmap bitmap = new(linkLabel.Width, linkLabel.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, linkLabel.ClientRectangle);

        linkLabel.OnPaint(e);

        int invalidatedCount = 0;
        linkLabel.Invalidated += (sender, args) => invalidatedCount++;

        linkLabel.Text = string.Empty;
        int invalidatedCountAfterTextChange = invalidatedCount;
        ((bool)linkLabel.TestAccessor.Dynamic._textLayoutValid).Should().BeFalse();

        linkLabel.OnPaint(e);
        ((bool)linkLabel.TestAccessor.Dynamic._textLayoutValid).Should().BeTrue();
        int invalidatedCountAfterLayout = invalidatedCount;
        linkLabel.OnPaint(e);

        invalidatedCountAfterTextChange.Should().BeGreaterThan(0);
        invalidatedCount.Should().Be(invalidatedCountAfterLayout);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    public void LinkLabel_OnPaint_EmptyText_WithoutHandle_DoesNotThrowOrLoop(string? text)
    {
        // Same scenario as LinkLabel_OnPaint_EmptyText_DoesNotKeepInvalidating, but without creating the control's
        // handle, since layout/paint caching could plausibly follow a different code path before the handle exists.
        using TestLinkLabel linkLabel = new() { Size = new Size(100, 20), Text = text };

        using Bitmap bitmap = new(linkLabel.Width, linkLabel.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, linkLabel.ClientRectangle);

        // The first paint calculates the text layout and establishes a valid cache.
        linkLabel.OnPaint(e);
        ((bool)linkLabel.TestAccessor.Dynamic._textLayoutValid).Should().BeTrue();

        linkLabel.OnPaint(e);
        linkLabel.OnPaint(e);

        ((bool)linkLabel.TestAccessor.Dynamic._textLayoutValid).Should().BeTrue();
        linkLabel.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    public void LinkLabel_OnPaint_EmptyText_ManyRepaints_StaysStable(string? text)
    {
        // Strengthens LinkLabel_OnPaint_EmptyText_DoesNotKeepInvalidating by repainting many times: the original
        // bug (https://github.com/dotnet/winforms/issues/10515) was an endless invalidate/paint loop, so a larger
        // number of repeated paints gives higher confidence that no every-Nth-call regression slips through.
        using TestLinkLabel linkLabel = new() { Size = new Size(100, 20), Text = text };
        linkLabel.CreateControl();

        using Bitmap bitmap = new(linkLabel.Width, linkLabel.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, linkLabel.ClientRectangle);

        // The first paint calculates the text layout and is allowed to invalidate the control.
        linkLabel.OnPaint(e);

        int invalidatedCount = 0;
        linkLabel.Invalidated += (sender, args) => invalidatedCount++;

        for (int i = 0; i < 20; i++)
        {
            linkLabel.OnPaint(e);
        }

        invalidatedCount.Should().Be(0);
    }

    private class TestLinkLabel : LinkLabel
    {
        public new void OnLinkClicked(LinkLabelLinkClickedEventArgs e) => base.OnLinkClicked(e);
        public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);
    }
}
