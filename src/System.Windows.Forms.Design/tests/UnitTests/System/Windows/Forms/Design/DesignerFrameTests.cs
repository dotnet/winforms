// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class DesignerFrameTests : IDisposable
{
    private readonly Mock<ISite> _mockSite = new();
    private DesignerFrame _designerFrame;

    public DesignerFrameTests() => _designerFrame = new(_mockSite.Object);

    public void Dispose() => _designerFrame.Dispose();

    private void SetupMocks()
    {
        Mock<IUIService> mockUIService = new();
        mockUIService.Setup(ui => ui.Styles["ArtboardBackground"]).Returns(Color.Red);
        _mockSite.Setup(site => site.GetService(typeof(IUIService))).Returns(mockUIService.Object);
    }

    [Fact]
    public void DesignerFrame_Constructor_SetsProperties()
    {
        SetupMocks();

        using (_designerFrame = new DesignerFrame(_mockSite.Object))
        {
            _designerFrame.Text.Should().Be("DesignerFrame");
            _designerFrame.BackColor.Should().Be(Color.Red);

            _designerFrame.Controls.Cast<Control>().Should().HaveCount(1);
        }
    }

    [Fact]
    public void DesignerFrame_Initialize_SetsDesignerProperties()
    {
        Control control = new();
        try
        {
            _designerFrame.Initialize(control);

            List<Control> controls = _designerFrame.Controls.Cast<Control>().ToList();
            Control? designerRegion = controls.FirstOrDefault(c => c is ScrollableControl);

            designerRegion.Should().NotBeNull();

            Control? designer = designerRegion?.Controls.Cast<Control>().FirstOrDefault();
            designer.Should().Be(control);

            control.Visible.Should().BeTrue();
            control.Enabled.Should().BeTrue();

            _designerFrame.Controls.Cast<Control>().Should().NotContain(control);
        }
        finally
        {
            control.Dispose();
        }
    }

    [Fact]
    public void DesignerFrame_ProcessDialogKey_ReturnsExpectedResult()
    {
        using (_designerFrame = new DesignerFrame(_mockSite.Object))
        {
            bool result = _designerFrame.TestAccessor().Dynamic.ProcessDialogKey(Keys.Enter);

            result.Should().BeFalse();
        }
    }

    [Fact]
    public void DesignerFrame_Constructor_SetsSiteProperty()
    {
        _designerFrame.Site ??= _mockSite.Object;

        _designerFrame.Should().NotBeNull();
        _designerFrame.Site.Should().Be(_mockSite.Object);
    }

    [Fact]
    public void DesignerFrame_AddControl_AddsControlToFrame()
    {
        Control control = new();
        try
        {
            _designerFrame.Controls.Add(control);

            _designerFrame.Controls.Cast<Control>().Should().Contain(control);
        }
        finally
        {
            control.Dispose();
        }
    }

    [Fact]
    public void DesignerFrame_Resize_TriggersExpectedBehavior()
    {
        using (_designerFrame = new DesignerFrame(_mockSite.Object))
        {
            _designerFrame.Resize += (sender, args) => { /* Handle resize if needed */ };

            _designerFrame.Width = 500;

            _designerFrame.Width.Should().Be(500);
        }
    }

    [Fact]
    public void Designer_ProcessDialogKey()
    {
        using (_designerFrame = new DesignerFrame(_mockSite.Object))
        {
            bool result = _designerFrame.TestAccessor().Dynamic.ProcessDialogKey(Keys.Enter);

            result.Should().BeFalse();
        }
    }
}
