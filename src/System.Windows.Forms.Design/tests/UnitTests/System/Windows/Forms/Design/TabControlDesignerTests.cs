// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;
using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class TabControlDesignerTests
{
    private (TabControlDesigner designer, TabControl tabControl) CreateInitializedDesignerWithServiceProvider(out Mock<IServiceProvider> serviceProvider, out Mock<IDesignerHost> designerHost)
    {
        TabControl tabControl = new();
        TabControlDesigner designer = new();
        serviceProvider = new();
        designerHost = new();
        serviceProvider.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(designerHost.Object);
        designer.Initialize(tabControl);
        return (designer, tabControl);
    }

    private (TabControlDesigner designer, TabControl tabControl, TabPage tabPage) CreateDesignerWithMockedHost()
    {
        TabControlDesigner designer = new();
        TabControl tabControl = new();
        TabPage tabPage = new();
        Mock<IServiceProvider> serviceProvider = new();
        Mock<IDesignerHost> designerHost = new();
        serviceProvider.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(designerHost.Object);
        designerHost.Setup(h => h.CreateComponent(typeof(TabPage))).Returns(tabPage);
        designer.Initialize(tabControl);
        return (designer, tabControl, tabPage);
    }

    [Fact]
    public void InitializeNewComponent_ShouldAddTabPageToTabControl()
    {
        var (designer, tabControl) = CreateInitializedDesignerWithServiceProvider(out _, out var designerHost);
        using (designer)
        using (tabControl)
        {
            using TabPage tabPage = new();
            tabControl.TabPages.Add(tabPage);
            designerHost.Setup(h => h.CreateComponent(typeof(TabPage))).Returns(tabPage);

            designer.InitializeNewComponent(new Hashtable());

            tabControl.TabPages.Count.Should().Be(1);
        }
    }

    [Theory]
    [InlineData(typeof(TabPage), true)]
    [InlineData(typeof(Button), false)]
    public void CanParent_ReturnsExpectedResult(Type controlType, bool expected)
    {
        var (designer, tabControl) = CreateInitializedDesignerWithServiceProvider(out _, out _);
        using (designer)
        using (tabControl)
        {
            designer.Initialize(tabControl);
            Control? control = Activator.CreateInstance(controlType) as Control;

            if (control is TabPage tabPage && !expected)
            {
                tabControl.TabPages.Add(tabPage);
            }

            bool result = designer.CanParent(control);

            result.Should().Be(expected);
        }
    }

    [Fact]
    public void Verbs_ContainsAddAndRemoveVerbs()
    {
        var (designer, tabControl, tabPage) = CreateDesignerWithMockedHost();
        using (designer)
        using (tabControl)
        using (tabPage)
        {
            var verbs = designer.Verbs.Cast<DesignerVerb>();

            verbs.Should().Contain(v => v.Text == SR.TabControlAdd);
            verbs.Should().Contain(v => v.Text == SR.TabControlRemove);
        }
    }

    [Fact]
    public void Verbs_RemoveVerbEnabledWhenTabPagesExist()
    {
        var (designer, tabControl, tabPage) = CreateDesignerWithMockedHost();
        using (designer)
        using (tabControl)
        using (tabPage)
        {
            tabControl.TabPages.Add(tabPage);

            var verbs = designer.Verbs;
            var removeVerb = verbs.Cast<DesignerVerb>().FirstOrDefault(v => v.Text == SR.TabControlRemove);

            removeVerb.Should().NotBeNull();
            removeVerb.Should().BeOfType<DesignerVerb>().Which.Enabled.Should().BeTrue();
        }
    }

    [Fact]
    public void Verbs_RemoveVerbDisabledWhenNoTabPagesExist()
    {
        var (designer, tabControl, tabPage) = CreateDesignerWithMockedHost();
        using (designer)
        using (tabControl)
        using (tabPage)
        {
            var verbs = designer.Verbs;
            var removeVerb = verbs.Cast<DesignerVerb>().FirstOrDefault(v => v.Text == SR.TabControlRemove);

            removeVerb.Should().NotBeNull();
            removeVerb.Should().BeOfType<DesignerVerb>().Which.Enabled.Should().BeFalse();
        }
    }

    [Fact]
    public void ParticipatesWithSnapLines_ReturnsFalseWhenNotForwardOnDrag()
    {
        var (designer, tabControl, tabPage) = CreateDesignerWithMockedHost();
        using (designer)
        using (tabControl)
        using (tabPage)
        {
            bool result = designer.ParticipatesWithSnapLines;

            result.Should().BeFalse();
        }
    }

    [Fact]
    public void ParticipatesWithSnapLines_ReturnsTrueWhenForwardOnDragAndPageDesignerParticipates()
    {
        var (designer, tabControl, tabPage) = CreateDesignerWithMockedHost();
        using (designer)
        using (tabControl)
        using (tabPage)
        {
            tabControl.TabPages.Add(tabPage);

            Mock<TabPageDesigner> pageDesignerMock = new(MockBehavior.Strict);
            pageDesignerMock.Setup(p => p.ParticipatesWithSnapLines).Returns(true);

            designer.TestAccessor().Dynamic._forwardOnDrag = true;

            bool result = designer.ParticipatesWithSnapLines;

            result.Should().BeTrue();
        }
    }

    [Fact]
    public void ParticipatesWithSnapLines_ReturnsTrueWhenForwardOnDragAndPageDesignerDoesNotParticipate()
    {
        var (designer, tabControl, tabPage) = CreateDesignerWithMockedHost();
        using (designer)
        using (tabControl)
        using (tabPage)
        {
            Mock<TabPageDesigner> pageDesignerMock = new(MockBehavior.Strict);
            pageDesignerMock.Setup(p => p.ParticipatesWithSnapLines).Returns(false);

            designer.TestAccessor().Dynamic._forwardOnDrag = true;

            bool result = designer.ParticipatesWithSnapLines;

            result.Should().BeTrue();
        }
    }
}
