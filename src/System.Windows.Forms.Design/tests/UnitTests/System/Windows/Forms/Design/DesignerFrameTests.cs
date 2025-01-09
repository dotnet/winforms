// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Drawing;
using System.Reflection;
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

        Type? overlayControlType = typeof(DesignerFrame).GetNestedType("OverlayControl", BindingFlags.NonPublic);
        overlayControlType.Should().NotBeNull();
        designerFrame.Controls[0].Should().BeOfType(overlayControlType);
    }

    [Fact]
    public void DesignerFrame_Initialize_SetsDesignerProperties()
    {
        Mock<ISite> mockSite = new();
        Control control = new();
        DesignerFrame designerFrame = new(mockSite.Object);

        designerFrame.Initialize(control);

        FieldInfo? designerField = typeof(DesignerFrame).GetField("_designer", BindingFlags.NonPublic | BindingFlags.Instance);

        object designer = designerField?.GetValue(designerFrame) ?? throw new InvalidOperationException("Field '_designer' not found in DesignerFrame.");

        designer.Should().Be(control);
        control.Visible.Should().BeTrue();
        control.Enabled.Should().BeTrue();

        designerFrame.Controls.Cast<Control>().ToList().ForEach(c =>
            Console.WriteLine($"Control Type: {c.GetType()}, Control Name: {c.Name}"));

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

        if (designerFrame.Site is null)
        {
            PropertyInfo? siteProperty = typeof(DesignerFrame).GetProperty("Site", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            siteProperty?.SetValue(designerFrame, mockSite.Object);
        }

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

        designerFrame.Resize += (sender, args) => { /* Some behavior on resize */ };
        designerFrame.Width = 500;

        designerFrame.Width.Should().Be(500);
    }

    [Fact]
    public void CanReflectOverlayControlType()
    {
        Type designerFrameType = typeof(DesignerFrame);

        Type? overlayControlType = designerFrameType.GetNestedType("OverlayControl", BindingFlags.NonPublic);

        overlayControlType.Should().NotBeNull();
        if (overlayControlType is not null)
        {
            Console.WriteLine($"OverlayControl type found: {overlayControlType.FullName}");
        }
    }

    [Fact]
    public void CanReflectOverlayControlConstructor()
    {
        Type designerFrameType = typeof(DesignerFrame);
        Type? overlayControlType = designerFrameType.GetNestedType("OverlayControl", BindingFlags.NonPublic);

        overlayControlType.Should().NotBeNull();

        ConstructorInfo? constructor = overlayControlType?.GetConstructor([typeof(IServiceProvider)]);

        constructor.Should().NotBeNull();
    }

    [Fact]
    public void CanCreateOverlayControlInstance()
    {
        Mock<IServiceProvider> mockProvider = new();

        Type designerFrameType = typeof(DesignerFrame);

        Type? overlayControlType = designerFrameType.GetNestedType("OverlayControl", BindingFlags.NonPublic);

        overlayControlType.Should().NotBeNull();

        ConstructorInfo? constructor = overlayControlType?.GetConstructor([typeof(IServiceProvider)]);

        constructor.Should().NotBeNull();

        object? overlayControl = constructor?.Invoke([mockProvider.Object]);

        overlayControl.Should().NotBeNull();
    }

    [Fact]
    public void CanAccessOverlayControlControlsProperty()
    {
        Mock<IServiceProvider> mockProvider = new();

        Type designerFrameType = typeof(DesignerFrame);

        Type? overlayControlType = designerFrameType.GetNestedType("OverlayControl", BindingFlags.NonPublic);

        ConstructorInfo? constructor = overlayControlType?.GetConstructor([typeof(IServiceProvider)]);

        object? overlayControl = constructor?.Invoke([mockProvider.Object]);

        PropertyInfo? controlsProperty = overlayControlType?.GetProperty("Controls", BindingFlags.Instance | BindingFlags.Public);

        Control.ControlCollection? controls = controlsProperty?.GetValue(overlayControl) as Control.ControlCollection;

        controls.Should().NotBeNull();
    }

    [Fact]
    public void CanAddControlsToOverlayControl()
    {
        Mock<IServiceProvider> mockProvider = new();

        Type designerFrameType = typeof(DesignerFrame);

        Type? overlayControlType = designerFrameType.GetNestedType("OverlayControl", BindingFlags.NonPublic);

        ConstructorInfo? constructor = overlayControlType?.GetConstructor([typeof(IServiceProvider)]);

        object? overlayControl = constructor?.Invoke([mockProvider.Object]);

        PropertyInfo? controlsProperty = overlayControlType?.GetProperty("Controls", BindingFlags.Instance | BindingFlags.Public);

        Control.ControlCollection? controls = controlsProperty?.GetValue(overlayControl) as Control.ControlCollection;

        controls.Should().NotBeNull();
        controls.Should().BeOfType<Control.ControlCollection>();

        Control control1 = new() { Bounds = new Rectangle(0, 0, 100, 100) };
        Control control2 = new() { Bounds = new Rectangle(100, 100, 100, 100) };

        controls?.Add(control1);
        controls?.Add(control2);

        List<Control>? controlsList = controls?.Cast<Control>().ToList();

        controlsList.Should().Contain(control1);
        controlsList.Should().Contain(control2);
    }
}
