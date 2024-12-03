// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class LinkLabelTests : IDisposable
{
    private readonly LinkLabel _linkLabel = new();

    public void Dispose() => _linkLabel.Dispose();

    [WinFormsFact]
    public void LinkLabel_Constructor()
    {
        Assert.NotNull(_linkLabel);
        Assert.True(_linkLabel.LinkArea.IsEmpty);
        Assert.Equal(0, _linkLabel.LinkArea.Start);
        Assert.Equal(0, _linkLabel.LinkArea.Length);
    }

    [WinFormsFact]
    public void LinkLabel_FlatStyle_Get_ReturnsExpected()
    {
        _linkLabel.FlatStyle.Should().Be(FlatStyle.Standard);
    }

    [WinFormsFact]
    public void LinkLabel_FlatStyle_Set_ReturnsExpected()
    {
        _linkLabel.FlatStyle = FlatStyle.Flat;
        _linkLabel.FlatStyle.Should().Be(FlatStyle.Flat);

        _linkLabel.FlatStyle = FlatStyle.Popup;
        _linkLabel.FlatStyle.Should().Be(FlatStyle.Popup);

        _linkLabel.FlatStyle = FlatStyle.System;
        _linkLabel.FlatStyle.Should().Be(FlatStyle.System);
    }

    [WinFormsFact]
    public void LinkLabel_LinkArea_Get_ReturnsExpected()
    {
        _linkLabel.LinkArea.Should().Be(new LinkArea(0, 0));
    }

    [WinFormsFact]
    public void LinkLabel_LinkArea_Set_ReturnsExpected()
    {
        _linkLabel.LinkArea = new LinkArea(1, 2);
        _linkLabel.LinkArea.Should().Be(new LinkArea(1, 2));

        _linkLabel.LinkArea = new LinkArea(3, 4);
        _linkLabel.LinkArea.Should().Be(new LinkArea(3, 4));
    }

    [WinFormsFact]
    public void LinkLabel_LinkArea_SetNegativeStart_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => _linkLabel.LinkArea = new LinkArea(-1, 2);
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*LinkArea*");
    }

    [WinFormsFact]
    public void LinkLabel_LinkArea_SetNegativeLength_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => _linkLabel.LinkArea = new LinkArea(1, -2);
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

    [WinFormsFact]
    public void LinkLabel_LinkBehavior_Set_ReturnsExpected()
    {
        _linkLabel.LinkBehavior = LinkBehavior.AlwaysUnderline;
        _linkLabel.LinkBehavior.Should().Be(LinkBehavior.AlwaysUnderline);

        _linkLabel.LinkBehavior = LinkBehavior.HoverUnderline;
        _linkLabel.LinkBehavior.Should().Be(LinkBehavior.HoverUnderline);

        _linkLabel.LinkBehavior = LinkBehavior.NeverUnderline;
        _linkLabel.LinkBehavior.Should().Be(LinkBehavior.NeverUnderline);
    }

    [WinFormsFact]
    public void LinkLabel_LinkVisited_Get_ReturnsExpected()
    {
        _linkLabel.LinkVisited.Should().BeFalse();
    }

    [WinFormsFact]
    public void LinkLabel_LinkVisited_Set_ReturnsExpected()
    {
        _linkLabel.LinkVisited = true;
        _linkLabel.LinkVisited.Should().BeTrue();

        _linkLabel.LinkVisited = false;
        _linkLabel.LinkVisited.Should().BeFalse();
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

    [WinFormsFact]
    public void LinkLabel_TabStop_Set_ReturnsExpected()
    {
        _linkLabel.TabStop = true;
        _linkLabel.TabStop.Should().BeTrue();

        _linkLabel.TabStop = false;
        _linkLabel.TabStop.Should().BeFalse();
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
        _linkLabel.Padding = new Padding(1, 2, 3, 4);
        _linkLabel.Padding.Should().Be(new Padding(1, 2, 3, 4));

        _linkLabel.Padding = new Padding(5);
        _linkLabel.Padding.Should().Be(new Padding(5));
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

    [WinFormsFact]
    public void LinkLabel_UseCompatibleTextRendering_Set_ReturnsExpected()
    {
        _linkLabel.UseCompatibleTextRendering = true;
        _linkLabel.UseCompatibleTextRendering.Should().BeTrue();

        _linkLabel.UseCompatibleTextRendering = false;
        _linkLabel.UseCompatibleTextRendering.Should().BeFalse();
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

    private class TestLinkLabel : LinkLabel
    {
        public new void OnLinkClicked(LinkLabelLinkClickedEventArgs e) => base.OnLinkClicked(e);
    }
}
