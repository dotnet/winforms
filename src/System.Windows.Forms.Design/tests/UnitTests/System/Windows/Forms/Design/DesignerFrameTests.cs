// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class DesignerFrameTests
{
    private readonly Mock<ISite> _mockSite = new();

    private class TestDesignerFrame : DesignerFrame
    {
        public TestDesignerFrame(ISite site) : base(site) { }

        public void TestSetSite(ISite site)
        {
            Site = site;
        }

        public new bool ProcessDialogKey(Keys keyData)
        {
            return base.ProcessDialogKey(keyData);
        }
    }

    private void SetupMocks()
    {
        Mock<IUIService> mockUIService = new();
        mockUIService.Setup(ui => ui.Styles["ArtboardBackground"]).Returns(Color.Red);
        _mockSite.Setup(site => site.GetService(typeof(IUIService))).Returns(mockUIService.Object);
    }

    [Fact]
    public void DesignerFrame_Constructor_SetsTextBackColorAndControls()
    {
        SetupMocks();

        DesignerFrame designerFrame = new(_mockSite.Object);
        try
        {
            designerFrame.Text.Should().Be("DesignerFrame");
            designerFrame.BackColor.Should().Be(Color.Red);

            designerFrame.Controls.Cast<Control>().Should().HaveCount(1);
        }
        finally
        {
            designerFrame.Dispose();
        }
    }

    [Fact]
    public void DesignerFrame_Initialize_SetsDesignerProperties()
    {
        Control control = new();
        DesignerFrame designerFrame = new(_mockSite.Object);
        try
        {
            designerFrame.Initialize(control);

            List<Control> controls = designerFrame.Controls.Cast<Control>().ToList();
            Control? designerRegion = controls.FirstOrDefault(c => c is ScrollableControl);

            designerRegion.Should().NotBeNull();

            Control? containedControl = designerRegion?.Controls.Cast<Control>().FirstOrDefault();
            containedControl.Should().Be(control);

            control.Visible.Should().BeTrue();
            control.Enabled.Should().BeTrue();

            designerFrame.Controls.Cast<Control>().Should().NotContain(control);
        }
        finally
        {
            control.Dispose();
            designerFrame.Dispose();
        }
    }

    [Fact]
    public void DesignerFrame_ProcessDialogKey_ReturnsExpectedResult()
    {
        TestDesignerFrame designerFrame = new(_mockSite.Object);
        try
        {
            bool result = designerFrame.ProcessDialogKey(Keys.Enter);

            result.Should().BeFalse();
        }
        finally
        {
            designerFrame.Dispose();
        }
    }

    [Fact]
    public void DesignerFrame_Constructor_SetsSitePropertyUsingTestSetSite()
    {
        TestDesignerFrame designerFrame = new(_mockSite.Object);
        try
        {
            designerFrame.TestSetSite(_mockSite.Object);

            designerFrame.Site.Should().Be(_mockSite.Object);
        }
        finally
        {
            designerFrame.Dispose();
        }
    }

    [Fact]
    public void DesignerFrame_AddControl_AddsControlToFrame()
    {
        Control control = new();
        DesignerFrame designerFrame = new(_mockSite.Object);
        try
        {
            designerFrame.Controls.Add(control);

            designerFrame.Controls.Cast<Control>().Should().Contain(control);
        }
        finally
        {
            control.Dispose();
            designerFrame.Dispose();
        }
    }

    [Fact]
    public void DesignerFrame_Resize_TriggersExpectedBehavior()
    {
        DesignerFrame designerFrame = new(_mockSite.Object);
        bool resizeEventTriggered = false;

        try
        {
            designerFrame.Resize += (sender, args) => resizeEventTriggered = true;
            designerFrame.Width = 500;

            resizeEventTriggered.Should().BeTrue();
            designerFrame.Width.Should().Be(500);
        }
        finally
        {
            designerFrame.Dispose();
        }
    }

    [Fact]
    public void Designer_ProcessDialogKey()
    {
        TestDesignerFrame designerFrame = new(_mockSite.Object);
        try
        {
            bool result = designerFrame.ProcessDialogKey(Keys.Enter);

            result.Should().BeFalse();
        }
        finally
        {
            designerFrame.Dispose();
        }
    }
}
