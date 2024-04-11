// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Windows.Forms.Design.Tests.Mocks;
using Moq;
using Windows.Win32;

namespace System.Windows.Forms.Design.Tests;

public class ControlDesignerTests
{
    [WinFormsFact]
    public void ControlDesigner_Ctor_Default()
    {
        using TestControlDesigner controlDesigner = new();
        Assert.False(controlDesigner.AutoResizeHandles);
        Assert.Throws<InvalidOperationException>(() => controlDesigner.Control);
        Assert.True(controlDesigner.ControlSupportsSnaplines);
        Assert.Throws<InvalidOperationException>(() => controlDesigner.Component);
        Assert.True(controlDesigner.ForceVisible);
        Assert.Throws<InvalidOperationException>(() => controlDesigner.GetParentComponentProperty());
        Assert.False(controlDesigner.SerializePerformLayout);
    }

    [WinFormsFact]
    public void ControlDesigner_PropertiesTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.Empty(controlDesigner.AssociatedComponents);
        Assert.False(controlDesigner.IsRootDesigner);
        Assert.NotNull(controlDesigner.SnapLines);
        Assert.Equal(8, controlDesigner.SnapLines.Count);
        Assert.NotNull(controlDesigner.StandardBehavior);
        Assert.Equal(Cursors.Default, controlDesigner.StandardBehavior.Cursor);
    }

    [Fact]
    public void AccessibleObjectField()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.Null(controlDesigner.GetAccessibleObjectField());
    }

    [Fact]
    public void BehaviorServiceProperty()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.Null(controlDesigner.GetBehaviorServiceProperty());
    }

    [Fact]
    public void AccessibilityObjectField()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.NotNull(controlDesigner.AccessibilityObject);
    }

    [Fact]
    public void EnableDragRectProperty()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.False(controlDesigner.GetEnableDragRectProperty());
    }

    [Fact]
    public void ParticipatesWithSnapLinesProperty()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.True(controlDesigner.ParticipatesWithSnapLines);
    }

    [Fact]
    public void AutoResizeHandlesProperty()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.True(controlDesigner.AutoResizeHandles = true);
        Assert.True(controlDesigner.AutoResizeHandles);
    }

    [Fact]
    public void SelectionRulesProperty()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);

        SelectionRules selectionRules;
        using (new NoAssertContext())
        {
            selectionRules = controlDesigner.SelectionRules;
        }

        Assert.Equal(SelectionRules.Visible | SelectionRules.AllSizeable | SelectionRules.Moveable, selectionRules);
    }

    [Fact]
    public void InheritanceAttributeProperty()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.NotNull(controlDesigner.GetInheritanceAttributeProperty());
    }

    [Fact]
    public void NumberOfInternalControlDesignersTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.Equal(0, controlDesigner.NumberOfInternalControlDesigners());
    }

    [Fact]
    public void BaseWndProcTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Message m = default;
        controlDesigner.BaseWndProcMethod(ref m);
    }

    [Fact]
    public void CanBeParentedToTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        using ParentControlDesigner parentDesigner = new();
        using Button parentButton = new();
        parentDesigner.Initialize(parentButton);
        Assert.True(controlDesigner.CanBeParentedTo(parentDesigner));
    }

    [Theory]
    [BoolData]
    public void EnableDragDropTest(bool val)
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        controlDesigner.EnableDragDropMethod(val);
    }

    [Fact]
    public void GetHitTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        Assert.False(controlDesigner.GetHitTestMethod(new Drawing.Point()));
    }

    [Fact]
    public void HookChildControlsTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        controlDesigner.HookChildControlsMethod(new Control());
    }

    [Fact]
    public void InitializeTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
    }

    [Fact]
    public void UninitializedTest()
    {
        using TestControlDesigner controlDesigner = new();
        Assert.Throws<InvalidOperationException>(() => controlDesigner.Control);
    }

    [Fact]
    public void OnSetComponentDefaultsTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
#pragma warning disable CS0618 // Type or member is obsolete
        controlDesigner.OnSetComponentDefaults();
#pragma warning restore CS0618
    }

    [Fact]
    public void OnContextMenuTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        controlDesigner.OnContextMenuMethod(0, 0);
    }

    [Fact]
    public void OnCreateHandleTest()
    {
        using TestControlDesigner controlDesigner = new();
        using Button button = new();
        controlDesigner.Initialize(button);
        controlDesigner.OnCreateHandleMethod();
    }

    [WinFormsFact]
    public void ControlDesigner_WndProc_InvokePaint_Success()
    {
        using ControlDesigner designer = new();
        using Button button = new();
        designer.Initialize(button);
        Message m = new Message
        {
            Msg = (int)PInvoke.WM_PAINT
        };
        designer.TestAccessor().Dynamic.WndProc(ref m);
    }

    [Fact]
    public void ControlDesigner_AssociatedComponents_NullSite_Test()
    {
        using ControlDesigner controlDesigner = new();
        using Control control = new();

        using Control childControl = new();
        controlDesigner.Initialize(control);

        Assert.Empty(controlDesigner.AssociatedComponents);

        control.Controls.Add(childControl);

        Assert.Empty(controlDesigner.AssociatedComponents);
    }

    [WinFormsFact]
    public void ControlDesigner_AssociatedComponentsTest()
    {
        using Control control = new();
        using ControlDesigner controlDesigner = new();

        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(control);
        mockDesignerHost
            .Setup(s => s.GetDesigner(It.IsAny<Control>()))
            .Returns(() => null);
        var mockSite = MockSite.CreateMockSiteWithDesignerHost(mockDesignerHost.Object);
        control.Site = mockSite.Object;

        controlDesigner.Initialize(control);

        Assert.Empty(controlDesigner.AssociatedComponents);

        using Control childControl = new();
        childControl.Site = mockSite.Object;
        control.Controls.Add(childControl);

        Assert.Equal(1, controlDesigner.AssociatedComponents.Count);
    }
}
