// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class ControlDesignerAccessibleObjectTests : IDisposable
{
    private class TestControl : Control
    {
        private readonly AccessibleObject _accessibilityObject;

        public TestControl(AccessibleObject accessibilityObject)
        {
            _accessibilityObject = accessibilityObject;
        }

        protected override AccessibleObject CreateAccessibilityInstance() => _accessibilityObject;
    }

    private readonly ControlDesigner _designer;

    public ControlDesignerAccessibleObjectTests() => _designer = new();

    public void Dispose() => _designer.Dispose();

    private ControlDesigner.ControlDesignerAccessibleObject CreateAccessibleObject(Action<Mock<AccessibleObject>>? configureMock = null)
    {
        Mock<AccessibleObject> mockAccessibleObject = new();

        configureMock?.Invoke(mockAccessibleObject);

        TestControl control = new(mockAccessibleObject.Object);
        return new ControlDesigner.ControlDesignerAccessibleObject(_designer, control);
    }

    [Fact]
    public void ControlDesignerAccessibleObject_Bounds_ReturnsExpectedValue()
    {
        Rectangle expectedBounds = new(10, 10, 100, 100);
        var accessibleObject = CreateAccessibleObject(mock => mock.Setup(a => a.Bounds).Returns(expectedBounds));
        accessibleObject.Bounds.Should().Be(expectedBounds);
    }

    [Fact]
    public void ControlDesignerAccessibleObject_Description_ReturnsExpectedValue()
    {
        string expectedDescription = "Test Description";
        var accessibleObject = CreateAccessibleObject(mock => mock.Setup(a => a.Description).Returns(expectedDescription));

        accessibleObject.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void ControlDesignerAccessibleObject_Value_ReturnsExpectedValue()
    {
        string expectedValue = "Test Value";
        var accessibleObject = CreateAccessibleObject(mock => mock.Setup(a => a.Value).Returns(expectedValue));
        accessibleObject.Value.Should().Be(expectedValue);
    }

    [Fact]
    public void ControlDesignerAccessibleObject_Role_ReturnsExpectedValue()
    {
        var accessibleObject = CreateAccessibleObject(mock => mock.Setup(a => a.Role).Returns(AccessibleRole.PushButton));
        accessibleObject.Role.Should().Be(AccessibleRole.PushButton);
    }

    [Fact]
    public void ControlDesignerAccessibleObject_Parent_ReturnsExpectedValue()
    {
        Mock<AccessibleObject> mockParent = new();
        var accessibleObject = CreateAccessibleObject(mock => mock.Setup(a => a.Parent).Returns(mockParent.Object));

        accessibleObject.Parent.Should().Be(mockParent.Object);
    }

    [Fact]
    public void ControlDesignerAccessibleObject_DefaultAction_ReturnsEmptyString()
    {
        CreateAccessibleObject().DefaultAction.Should().BeEmpty();
    }

    [Fact]
    public void ControlDesignerAccessibleObject_Name_ReturnsExpectedValue()
    {
        Control control = new() { Name = "TestControl" };
        var accessibleObject = new ControlDesigner.ControlDesignerAccessibleObject(_designer, control);

        accessibleObject.Name.Should().Be("TestControl");
    }

    [Theory]
    [InlineData(AccessibleStates.Selected, true, true)]
    [InlineData(AccessibleStates.Focused, true, true)]
    public void ControlDesignerAccessibleObject_State_ReturnsExpectedValue(AccessibleStates state, bool isSelected, bool isPrimarySelection)
    {
        Control control = new();
        var accessibleObject = new ControlDesigner.ControlDesignerAccessibleObject(_designer, control);

        dynamic accessor = accessibleObject.TestAccessor().Dynamic;
        accessor._selectionService = Mock.Of<ISelectionService>(s =>
            s.GetComponentSelected(control) == isSelected &&
            s.PrimarySelection == (isPrimarySelection ? control : null));

        accessibleObject.State.Should().HaveFlag(state);
    }

    [Fact]
    public void ControlDesignerAccessibleObject_GetChild_ReturnsExpectedValue()
    {
        Mock<AccessibleObject> mockChild = new();
        var accessibleObject = CreateAccessibleObject(mock => mock.Setup(a => a.GetChild(It.IsAny<int>())).Returns(mockChild.Object));

        accessibleObject.GetChild(0).Should().Be(mockChild.Object);
    }

    [Fact]
    public void ControlDesignerAccessibleObject_GetChildCount_ReturnsExpectedValue()
    {
        int expectedChildCount = 3;
        var accessibleObject = CreateAccessibleObject(mock => mock.Setup(a => a.GetChildCount()).Returns(expectedChildCount));
        accessibleObject.GetChildCount().Should().Be(expectedChildCount);
    }

    [Fact]
    public void ControlDesignerAccessibleObject_GetFocused_ReturnsExpectedValue()
    {
        var accessibleObject = CreateAccessibleObject(mock => mock.Setup(a => a.State).Returns(AccessibleStates.Focused));
        accessibleObject.GetFocused().Should().Be(accessibleObject);
    }

    [Fact]
    public void ControlDesignerAccessibleObject_GetSelected_ReturnsExpectedValue()
    {
        var accessibleObject = CreateAccessibleObject(mock => mock.Setup(a => a.State).Returns(AccessibleStates.Selected));
        accessibleObject.GetSelected().Should().Be(accessibleObject);
    }

    [Fact]
    public void ControlDesignerAccessibleObject_HitTest_ReturnsExpectedValue()
    {
        Mock<AccessibleObject> mockAccessibleObject = new();
        var accessibleObject = CreateAccessibleObject(mock => mock.Setup(a => a.HitTest(It.IsAny<int>(), It.IsAny<int>())).Returns(mockAccessibleObject.Object));

        accessibleObject.HitTest(10, 10).Should().Be(mockAccessibleObject.Object);
    }
}
