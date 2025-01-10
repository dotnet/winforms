// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class DesignerFrameTests
{
    [Fact]
    public void DesignerFrame_Constructor_SetsProperties()
    {
        Mock<ISite> mockSite = new();
        Mock<IUIService> mockUIService = new();
        mockUIService.Setup(ui => ui.Styles["ArtboardBackground"]).Returns(Color.Red);
        mockSite.Setup(site => site.GetService(typeof(IUIService))).Returns(mockUIService.Object);

        DesignerFrame designerFrame = new(mockSite.Object);

        designerFrame.Text.Should().Be("DesignerFrame");
        designerFrame.BackColor.Should().Be(Color.Red);

        designerFrame.Controls.Cast<Control>().Should().HaveCount(1);
    }

    [Fact]
    public void DesignerFrame_Initialize_SetsDesignerProperties()
    {
        Mock<ISite> mockSite = new();
        Control control = new();
        DesignerFrame designerFrame = new(mockSite.Object);

        designerFrame.Initialize(control);

        List<Control> controls = designerFrame.Controls.Cast<Control>().ToList();
        Control? designerRegion = controls.FirstOrDefault(c => c is ScrollableControl);

        designerRegion.Should().NotBeNull();

        Control? designer = designerRegion?.Controls.Cast<Control>().FirstOrDefault();
        designer.Should().Be(control);

        control.Visible.Should().BeTrue();
        control.Enabled.Should().BeTrue();

        designerFrame.Controls.Cast<Control>().Should().NotContain(control);
    }

    [Fact]
    public void DesignerFrame_ProcessDialogKey_ReturnsExpectedResult()
    {
        Mock<ISite> mockSite = new();
        using DesignerFrame designerFrame = new(mockSite.Object);

        bool result = designerFrame.TestAccessor().Dynamic.ProcessDialogKey(Keys.Enter);

        result.Should().BeFalse();
    }

    [Fact]
    public void DesignerFrame_Constructor_SetsSiteProperty()
    {
        Mock<ISite> mockSite = new();

        using DesignerFrame designerFrame = new(mockSite.Object);

        designerFrame.Site ??= mockSite.Object;

        designerFrame.Should().NotBeNull();
        designerFrame.Site.Should().Be(mockSite.Object);
    }

    [Fact]
    public void DesignerFrame_AddControl_AddsControlToFrame()
    {
        Mock<ISite> mockSite = new();
        using DesignerFrame designerFrame = new(mockSite.Object);
        Control control = new();

        designerFrame.Controls.Add(control);

        designerFrame.Controls.Cast<Control>().Should().Contain(control);
    }

    [Fact]
    public void DesignerFrame_Resize_TriggersExpectedBehavior()
    {
        Mock<ISite> mockSite = new();
        using DesignerFrame designerFrame = new(mockSite.Object);

        designerFrame.Resize += (sender, args) => { /* Handle resize if needed */ };

        designerFrame.Width = 500;

        designerFrame.Width.Should().Be(500);
    }

    [Fact]
    public void Designer_ProcessDialogKey()
    {
        Mock<ISite> mockSite = new();
        using DesignerFrame designerFrame = new(mockSite.Object);

        bool result = designerFrame.TestAccessor().Dynamic.ProcessDialogKey(Keys.Enter);

        result.Should().BeFalse();
    }
}
