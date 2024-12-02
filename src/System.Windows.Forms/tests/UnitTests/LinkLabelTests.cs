// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class LinkLabelTests
{
    [WinFormsFact]
    public void LinkLabel_Constructor()
    {
        using LinkLabel label = new();

        Assert.NotNull(label);
        Assert.True(label.LinkArea.IsEmpty);
        Assert.Equal(0, label.LinkArea.Start);
        Assert.Equal(0, label.LinkArea.Length);
    }

    [WinFormsFact]
    public void LinkLabel_FlatStyle_Get_ReturnsExpected()
    {
        using LinkLabel label = new();
        label.FlatStyle.Should().Be(FlatStyle.Standard);
    }

    [WinFormsFact]
    public void LinkLabel_FlatStyle_Set_GetReturnsExpected()
    {
        using LinkLabel label = new();

        SetAndVerifyFlatStyle(label, FlatStyle.Flat);
        SetAndVerifyFlatStyle(label, FlatStyle.Popup);
        SetAndVerifyFlatStyle(label, FlatStyle.System);
    }

    private static void SetAndVerifyFlatStyle(LinkLabel label, FlatStyle style)
    {
        label.FlatStyle = style;
        label.FlatStyle.Should().Be(style);
    }

    [WinFormsFact]
    public void LinkLabel_LinkArea_Get_ReturnsExpected()
    {
        using LinkLabel label = new();
        label.LinkArea.Should().Be(new LinkArea(0, 0));
    }

    [WinFormsFact]
    public void LinkLabel_LinkArea_Set_GetReturnsExpected()
    {
        using LinkLabel label = new();

        SetAndVerifyLinkArea(label, new LinkArea(1, 2));
        SetAndVerifyLinkArea(label, new LinkArea(3, 4));
    }

    private static void SetAndVerifyLinkArea(LinkLabel label, LinkArea area)
    {
        label.LinkArea = area;
        label.LinkArea.Should().Be(area);
    }

    [WinFormsFact]
    public void LinkLabel_LinkArea_SetNegativeStart_ThrowsArgumentOutOfRangeException()
    {
        using LinkLabel label = new();
        Action act = () => label.LinkArea = new LinkArea(-1, 2);
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*LinkArea*");
    }

    [WinFormsFact]
    public void LinkLabel_LinkArea_SetNegativeLength_ThrowsArgumentOutOfRangeException()
    {
        using LinkLabel label = new();
        Action act = () => label.LinkArea = new LinkArea(1, -2);
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*LinkArea*");
    }

    [WinFormsFact]
    public void LinkLabel_LinkArea_Set_UpdatesSelectability()
    {
        using LinkLabel label = new();
        label.Text = "Text";

        label.LinkArea = new LinkArea(1, 2);
        label.TabStop.Should().BeTrue();

        label.LinkArea = new LinkArea(0, 0);
        label.TabStop.Should().BeFalse();
    }

    [WinFormsFact]
    public void LinkLabel_LinkBehavior_Get_ReturnsExpected()
    {
        using LinkLabel label = new();
        label.LinkBehavior.Should().Be(LinkBehavior.SystemDefault);
    }

    [WinFormsFact]
    public void LinkLabel_LinkBehavior_Set_GetReturnsExpected()
    {
        using LinkLabel label = new();

        SetAndVerifyLinkBehavior(label, LinkBehavior.AlwaysUnderline);
        SetAndVerifyLinkBehavior(label, LinkBehavior.HoverUnderline);
        SetAndVerifyLinkBehavior(label, LinkBehavior.NeverUnderline);
    }

    private static void SetAndVerifyLinkBehavior(LinkLabel label, LinkBehavior behavior)
    {
        label.LinkBehavior = behavior;
        label.LinkBehavior.Should().Be(behavior);
    }

    [WinFormsFact]
    public void LinkLabel_LinkVisited_Get_ReturnsExpected()
    {
        using LinkLabel label = new();
        label.LinkVisited.Should().BeFalse();
    }

    [WinFormsFact]
    public void LinkLabel_LinkVisited_Set_ReturnsExpected()
    {
        using LinkLabel label = new();

        SetAndVerifyLinkVisited(label, true);
        SetAndVerifyLinkVisited(label, false);
    }

    private static void SetAndVerifyLinkVisited(LinkLabel label, bool visited)
    {
        label.LinkVisited = visited;
        label.LinkVisited.Should().Be(visited);
    }

    [WinFormsFact]
    public void LinkLabel_LinkVisited_Set_AddsLinkIfNoneExists()
    {
        using LinkLabel label = new();
        label.LinkVisited = true;
        label.Links.Count.Should().Be(1);
        label.Links[0].Visited.Should().BeTrue();
    }

    [WinFormsFact]
    public void LinkLabel_LinkVisited_Set_UpdatesExistingLink()
    {
        using LinkLabel label = new();
        label.Links.Add(new LinkLabel.Link(label) { Visited = false });

        label.LinkVisited = true;
        label.Links[0].Visited.Should().BeTrue();

        label.LinkVisited = false;
        label.Links[0].Visited.Should().BeFalse();
    }

    [WinFormsFact]
    public void LinkLabel_TabStop_Get_ReturnsExpected()
    {
        using LinkLabel label = new();
        label.TabStop.Should().BeFalse();
    }

    [WinFormsFact]
    public void LinkLabel_TabStop_Set_ReturnsExpected()
    {
        using LinkLabel label = new();

        SetAndVerifyTabStop(label, true);
        SetAndVerifyTabStop(label, false);
    }

    private static void SetAndVerifyTabStop(LinkLabel label, bool tabStop)
    {
        label.TabStop = tabStop;
        label.TabStop.Should().Be(tabStop);
    }

    [WinFormsFact]
    public void LinkLabel_TabStop_Set_RaisesTabStopChangedEvent()
    {
        using LinkLabel label = new();
        bool eventRaised = false;
        label.TabStopChanged += (sender, e) => eventRaised = true;

        label.TabStop = true;
        eventRaised.Should().BeTrue();

        eventRaised = false;
        label.TabStop = false;
        eventRaised.Should().BeTrue();
    }

    [WinFormsFact]
    public void LinkLabel_Padding_Get_ReturnsExpected()
    {
        using LinkLabel label = new();
        label.Padding.Should().Be(new Padding(0));
    }

    [WinFormsFact]
    public void LinkLabel_Padding_Set_ReturnsExpected()
    {
        using LinkLabel label = new();

        SetAndVerifyPadding(label, new Padding(1, 2, 3, 4));
        SetAndVerifyPadding(label, new Padding(5));
    }

    private static void SetAndVerifyPadding(LinkLabel label, Padding padding)
    {
        label.Padding = padding;
        label.Padding.Should().Be(padding);
    }

    [WinFormsFact]
    public void LinkLabel_VisitedLinkColor_Get_ReturnsExpected()
    {
        using LinkLabel label = new();
        label.VisitedLinkColor.Should().Be(LinkUtilities.GetVisitedLinkColor());
    }

    [WinFormsFact]
    public void LinkLabel_VisitedLinkColor_Set_ReturnsExpected()
    {
        using LinkLabel label = new();

        SetAndVerifyVisitedLinkColor(label, Color.Red);
        SetAndVerifyVisitedLinkColor(label, Color.Blue);
    }

    private static void SetAndVerifyVisitedLinkColor(LinkLabel label, Color color)
    {
        label.VisitedLinkColor = color;
        label.VisitedLinkColor.Should().Be(color);
    }

    [WinFormsFact]
    public void LinkLabel_VisitedLinkColor_Set_UpdatesLink()
    {
        using LinkLabel label = new();
        label.Links.Add(new LinkLabel.Link(label) { Visited = true });
        label.VisitedLinkColor = Color.Red;
        label.VisitedLinkColor.Should().Be(Color.Red);
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
        using LinkLabel label = new();
        label.UseCompatibleTextRendering.Should().BeTrue();
    }

    [WinFormsFact]
    public void LinkLabel_UseCompatibleTextRendering_Set_ReturnsExpected()
    {
        using LinkLabel label = new();

        SetAndVerifyUseCompatibleTextRendering(label, true);
        SetAndVerifyUseCompatibleTextRendering(label, false);
    }

    private static void SetAndVerifyUseCompatibleTextRendering(LinkLabel label, bool value)
    {
        label.UseCompatibleTextRendering = value;
        label.UseCompatibleTextRendering.Should().Be(value);
    }

    [WinFormsFact]
    public void LinkLabel_UseCompatibleTextRendering_Set_TriggersLayout()
    {
        using LinkLabel label = new();
        bool layoutCalled = false;
        label.Layout += (sender, e) => layoutCalled = true;

        label.UseCompatibleTextRendering = true;
        label.PerformLayout();

        layoutCalled.Should().BeTrue();
    }

    private class TestLinkLabel : LinkLabel
    {
        public new void OnLinkClicked(LinkLabelLinkClickedEventArgs e) => base.OnLinkClicked(e);
    }
}
