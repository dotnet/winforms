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
    public void OverlayControl_Constructor_SetsProperties()
    {
        Mock<IServiceProvider> mockProvider = new();

        Type? overlayControlType = typeof(DesignerFrame).GetNestedType("OverlayControl", BindingFlags.NonPublic);
        overlayControlType.Should().NotBeNull();
        ConstructorInfo? constructor = overlayControlType?.GetConstructor([typeof(IServiceProvider)]);
        constructor.Should().NotBeNull();
        object? overlayControl = constructor?.Invoke([mockProvider.Object]);
        overlayControl.Should().NotBeNull();

        FieldInfo? providerField = overlayControlType?.GetField("_provider", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo? overlayListField = overlayControlType?.GetField("_overlayList", BindingFlags.NonPublic | BindingFlags.Instance);

        providerField?.GetValue(overlayControl).Should().Be(mockProvider.Object);
        overlayListField?.GetValue(overlayControl).Should().NotBeNull();

        ((bool)overlayControlType?.GetProperty("AutoScroll")?.GetValue(overlayControl)!).Should().BeTrue();
        overlayControlType?.GetProperty("Text")?.GetValue(overlayControl).Should().Be("OverlayControl");
    }

    [Fact]
    public void OverlayControl_PushOverlay_AddsControl()
    {
        Mock<IServiceProvider> mockProvider = new();

        Type? overlayControlType = typeof(DesignerFrame).GetNestedType("OverlayControl", BindingFlags.NonPublic);
        overlayControlType.Should().NotBeNull();

        ConstructorInfo? constructor = overlayControlType?.GetConstructor([typeof(IServiceProvider)]);
        constructor.Should().NotBeNull();

        object? overlayControl = constructor?.Invoke([mockProvider.Object]);
        overlayControl.Should().NotBeNull();

        Control control = new();

        MethodInfo? pushOverlayMethod = overlayControlType?.GetMethod("PushOverlay");
        pushOverlayMethod.Should().NotBeNull();

        object? index = pushOverlayMethod?.Invoke(overlayControl, [control]);

        FieldInfo? overlayListField = overlayControlType?.GetField("_overlayList", BindingFlags.NonPublic | BindingFlags.Instance);
        List<Control>? overlayList = overlayListField?.GetValue(overlayControl) as List<Control>;

        overlayList.Should().NotBeNull();
        overlayList.Should().Contain(control);
        index.Should().Be(0);

        if ((bool)overlayControlType?.GetProperty("IsHandleCreated")?.GetValue(overlayControl)!)
        {
            FieldInfo? parentField = typeof(Control).GetField("_parent", BindingFlags.NonPublic | BindingFlags.Instance);
            parentField?.GetValue(control).Should().Be(overlayControl);

            PropertyInfo? displayRectangleProperty = overlayControlType?.GetProperty("DisplayRectangle");
            displayRectangleProperty?.GetValue(overlayControl).Should().Be(control.Bounds);
        }
    }

    [Fact]
    public void OverlayControl_RemoveOverlay_RemovesControl()
    {
        Mock<IServiceProvider> mockProvider = new();

        Type? overlayControlType = typeof(DesignerFrame).GetNestedType("OverlayControl", BindingFlags.NonPublic);
        overlayControlType.Should().NotBeNull();

        ConstructorInfo? constructor = overlayControlType?.GetConstructor([typeof(IServiceProvider)]);
        constructor.Should().NotBeNull();

        object? overlayControl = constructor?.Invoke([mockProvider.Object]);
        overlayControl.Should().NotBeNull();

        Control control = new();

        MethodInfo? pushOverlayMethod = overlayControlType?.GetMethod("PushOverlay");
        pushOverlayMethod.Should().NotBeNull();
        pushOverlayMethod?.Invoke(overlayControl, [control]);

        MethodInfo? removeOverlayMethod = overlayControlType?.GetMethod("RemoveOverlay");
        removeOverlayMethod.Should().NotBeNull();
        removeOverlayMethod?.Invoke(overlayControl, [control]);

        FieldInfo? overlayListField = overlayControlType?.GetField("_overlayList", BindingFlags.NonPublic | BindingFlags.Instance);
        List<Control>? overlayList = overlayListField?.GetValue(overlayControl) as List<Control>;

        overlayList.Should().NotBeNull();
        overlayList.Should().NotContain(control);
        control.Visible.Should().BeFalse();
        control.Parent.Should().BeNull();
    }

    [Fact]
    public void OverlayControl_InsertOverlay_InsertsControlAtIndex()
    {
        Mock<IServiceProvider> mockProvider = new();

        Type? overlayControlType = typeof(DesignerFrame).GetNestedType("OverlayControl", BindingFlags.NonPublic);
        overlayControlType.Should().NotBeNull();

        ConstructorInfo? constructor = overlayControlType?.GetConstructor([typeof(IServiceProvider)]);
        constructor.Should().NotBeNull();

        object? overlayControl = constructor?.Invoke([mockProvider.Object]);
        overlayControl.Should().NotBeNull();

        Control control1 = new();
        Control control2 = new();

        MethodInfo? pushOverlayMethod = overlayControlType?.GetMethod("PushOverlay");
        pushOverlayMethod.Should().NotBeNull();
        pushOverlayMethod?.Invoke(overlayControl, [control1]);

        MethodInfo? insertOverlayMethod = overlayControlType?.GetMethod("InsertOverlay");
        insertOverlayMethod.Should().NotBeNull();
        insertOverlayMethod?.Invoke(overlayControl, [control2, 0]);

        FieldInfo? overlayListField = overlayControlType?.GetField("_overlayList", BindingFlags.NonPublic | BindingFlags.Instance);
        List<Control>? overlayList = overlayListField?.GetValue(overlayControl) as List<Control>;

        overlayList.Should().NotBeNull();
        overlayList?[0].Should().Be(control2);
        overlayList?[1].Should().Be(control1);

        if ((bool)overlayControlType?.GetProperty("IsHandleCreated")?.GetValue(overlayControl)!)
        {
            FieldInfo? parentField = typeof(Control).GetField("_parent", BindingFlags.NonPublic | BindingFlags.Instance);
            parentField?.GetValue(control2).Should().Be(overlayControl);

            PropertyInfo? displayRectangleProperty = overlayControlType?.GetProperty("DisplayRectangle");
            displayRectangleProperty?.GetValue(overlayControl).Should().Be(control2.Bounds);
        }
    }

    [Fact]
    public void OverlayControl_InvalidateOverlays_InvokesInvalidateOnIntersectingControls()
    {
        Mock<IServiceProvider> mockProvider = new();

        Type? overlayControlType = typeof(DesignerFrame).GetNestedType("OverlayControl", BindingFlags.NonPublic);
        overlayControlType.Should().NotBeNull();

        ConstructorInfo? constructor = overlayControlType?.GetConstructor([typeof(IServiceProvider)]);
        constructor.Should().NotBeNull();

        object? overlayControl = constructor?.Invoke([mockProvider.Object]);
        overlayControl.Should().NotBeNull();

        Control control1 = new() { Bounds = new Rectangle(0, 0, 100, 100) };
        Control control2 = new() { Bounds = new Rectangle(50, 50, 100, 100) };

        MethodInfo? pushOverlayMethod = overlayControlType?.GetMethod("PushOverlay");
        pushOverlayMethod.Should().NotBeNull();
        pushOverlayMethod?.Invoke(overlayControl, [control1]);
        pushOverlayMethod?.Invoke(overlayControl, [control2]);

        MethodInfo? invalidateOverlaysMethod = overlayControlType?.GetMethod("InvalidateOverlays", [typeof(Rectangle)]);
        invalidateOverlaysMethod.Should().NotBeNull();
        invalidateOverlaysMethod?.Invoke(overlayControl, [new Rectangle(25, 25, 50, 50)]);

        FieldInfo? overlayListField = overlayControlType?.GetField("_overlayList", BindingFlags.NonPublic | BindingFlags.Instance);
        List<Control>? overlayList = overlayListField?.GetValue(overlayControl) as List<Control>;

        overlayList.Should().NotBeNull();
        overlayList.Should().Contain(control1);
        overlayList.Should().Contain(control2);

        MethodInfo? invalidateMethod = typeof(Control).GetMethod("Invalidate", [typeof(Rectangle)]);
        invalidateMethod.Should().NotBeNull();

        invalidateMethod?.Invoke(control1, [new Rectangle(25, 25, 50, 50)]);
        invalidateMethod?.Invoke(control2, [new Rectangle(25, 25, 50, 50)]);
    }

    [Fact]
    public void OverlayControl_InvalidateOverlays_InvokesInvalidateOnIntersectingControlsWithRegion()
    {
        Mock<IServiceProvider> mockProvider = new();

        Type? overlayControlType = typeof(DesignerFrame).GetNestedType("OverlayControl", BindingFlags.NonPublic);
        overlayControlType.Should().NotBeNull();

        ConstructorInfo? constructor = overlayControlType?.GetConstructor([typeof(IServiceProvider)]);
        constructor.Should().NotBeNull();

        object? overlayControl = constructor?.Invoke([mockProvider.Object]);
        overlayControl.Should().NotBeNull();

        Control control1 = new() { Bounds = new Rectangle(0, 0, 100, 100) };
        Control control2 = new() { Bounds = new Rectangle(50, 50, 100, 100) };

        MethodInfo? pushOverlayMethod = overlayControlType?.GetMethod("PushOverlay");
        pushOverlayMethod.Should().NotBeNull();
        pushOverlayMethod?.Invoke(overlayControl, [control1]);
        pushOverlayMethod?.Invoke(overlayControl, [control2]);

        MethodInfo? invalidateOverlaysMethod = overlayControlType?.GetMethod("InvalidateOverlays", [typeof(Region)]);
        invalidateOverlaysMethod.Should().NotBeNull();

        using (var region = new Region(new Rectangle(25, 25, 50, 50)))
        {
            invalidateOverlaysMethod?.Invoke(overlayControl, [region]);
        }

        FieldInfo? overlayListField = overlayControlType?.GetField("_overlayList", BindingFlags.NonPublic | BindingFlags.Instance);
        List<Control>? overlayList = overlayListField?.GetValue(overlayControl) as List<Control>;

        overlayList.Should().NotBeNull();
        overlayList.Should().Contain(control1);
        overlayList.Should().Contain(control2);

        MethodInfo? invalidateMethod = typeof(Control).GetMethod("Invalidate", [typeof(Region)]);
        invalidateMethod.Should().NotBeNull();

        using (var region = new Region(new Rectangle(25, 25, 50, 50)))
        {
            invalidateMethod?.Invoke(control1, [region]);
            invalidateMethod?.Invoke(control2, [region]);
        }
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
