// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class ControlDesignerControlDesignerAccessibleObjectTests
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

    private ControlDesigner.ControlDesignerAccessibleObject CreateAccessibleObject(Mock<AccessibleObject>? mockAccessibleObject = null)
    {
        Control control = mockAccessibleObject is not null
            ? new TestControl(mockAccessibleObject.Object)
            : new Control();

        ControlDesigner designer = new();
        return new ControlDesigner.ControlDesignerAccessibleObject(designer, control);
    }

    [Fact]
    public void Bounds_ReturnsExpectedBounds()
    {
        Rectangle expectedBounds = new(10, 10, 100, 100);
        Mock<AccessibleObject> mockAccessibleObject = new();
        mockAccessibleObject.Setup(a => a.Bounds).Returns(expectedBounds);
        ControlDesigner.ControlDesignerAccessibleObject accessibleObject = CreateAccessibleObject(mockAccessibleObject);

        accessibleObject.Bounds.Should().Be(expectedBounds);
    }

    [Theory]
    [InlineData("Test Description", nameof(AccessibleObject.Description))]
    [InlineData("Test Value", nameof(AccessibleObject.Value))]
    [InlineData(AccessibleRole.PushButton, nameof(AccessibleObject.Role))]
    [InlineData("Parent", nameof(AccessibleObject.Parent))]
    public void Properties_ReturnExpectedValues(object expectedValue, string propertyName)
    {
        Mock<AccessibleObject> mockAccessibleObject = new();

        if (propertyName == nameof(AccessibleObject.Description))
        {
            mockAccessibleObject.Setup(a => a.Description).Returns((string)expectedValue);
        }
        else if (propertyName == nameof(AccessibleObject.Value))
        {
            mockAccessibleObject.Setup(a => a.Value).Returns((string)expectedValue);
        }
        else if (propertyName == nameof(AccessibleObject.Role))
        {
            mockAccessibleObject.Setup(a => a.Role).Returns((AccessibleRole)expectedValue);
        }
        else if (propertyName == nameof(AccessibleObject.Parent))
        {
            AccessibleObject parentMock = new Mock<AccessibleObject>().Object;
            mockAccessibleObject.Setup(a => a.Parent).Returns(parentMock);
            expectedValue = parentMock;
        }

        ControlDesigner.ControlDesignerAccessibleObject accessibleObject = CreateAccessibleObject(mockAccessibleObject);

        object? result = typeof(ControlDesigner.ControlDesignerAccessibleObject).GetProperty(propertyName)!.GetValue(accessibleObject, null);

        result.Should().Be(expectedValue);
    }

    [Fact]
    public void DefaultAction_ReturnsEmptyString()
    {
        ControlDesigner.ControlDesignerAccessibleObject accessibleObject = CreateAccessibleObject();
        accessibleObject.DefaultAction.Should().BeEmpty();
    }

    [Fact]
    public void Name_ReturnsControlName()
    {
        Control control = new() { Name = "TestControl" };
        ControlDesigner designer = new();
        ControlDesigner.ControlDesignerAccessibleObject accessibleObject = new(designer, control);

        accessibleObject.Name.Should().Be("TestControl");
    }

    [Theory]
    [InlineData(AccessibleStates.Selected, true, true)]
    [InlineData(AccessibleStates.Focused, true, true)]
    public void State_ReturnsExpectedStates(AccessibleStates state, bool isSelected, bool isPrimarySelection)
    {
        Control control = new();
        ControlDesigner designer = new();
        ControlDesigner.ControlDesignerAccessibleObject accessibleObject = new(designer, control);

        Mock<ISelectionService> selectionServiceMock = new();
        selectionServiceMock.Setup(s => s.GetComponentSelected(control)).Returns(isSelected);
        selectionServiceMock.Setup(s => s.PrimarySelection).Returns(isPrimarySelection ? control : null);

        dynamic accessor = accessibleObject.TestAccessor().Dynamic;
        accessor._selectionService = selectionServiceMock.Object;

        accessibleObject.State.Should().HaveFlag(state);
    }

    [Theory]
    [InlineData(0, nameof(AccessibleObject.GetChild), typeof(AccessibleObject))]
    [InlineData(3, nameof(AccessibleObject.GetChildCount), typeof(int))]
    public void GetChildMethods_ReturnExpectedValues(object expectedValue, string methodName, Type expectedType)
    {
        Mock<AccessibleObject> mockAccessibleObject = new();
        if (methodName == nameof(AccessibleObject.GetChild))
        {
            AccessibleObject child = new Mock<AccessibleObject>().Object;
            mockAccessibleObject.Setup(a => a.GetChild(It.IsAny<int>())).Returns(child);
            expectedValue = child;
        }
        else
        {
            mockAccessibleObject.Setup(a => a.GetChildCount()).Returns((int)expectedValue);
        }

        ControlDesigner.ControlDesignerAccessibleObject accessibleObject = CreateAccessibleObject(mockAccessibleObject);

        object? result = typeof(ControlDesigner.ControlDesignerAccessibleObject)
            .GetMethod(methodName)!.Invoke(accessibleObject, methodName == nameof(AccessibleObject.GetChild) ? new object[] { 0 } : null);

        result.Should().BeAssignableTo(expectedType);
        result.Should().Be(expectedValue);
    }

    public static IEnumerable<object?[]> TestCases =>
    new List<object?[]>
    {
        new object?[] { AccessibleStates.Focused, nameof(ControlDesigner.ControlDesignerAccessibleObject.GetFocused), null },
        new object?[] { AccessibleStates.Selected, nameof(ControlDesigner.ControlDesignerAccessibleObject.GetSelected), null },
        new object?[] { AccessibleStates.None, nameof(ControlDesigner.ControlDesignerAccessibleObject.HitTest), 10, 10 }
    };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Methods_ReturnExpectedResults(AccessibleStates state, string methodName, int x = 0, int y = 0)
    {
        Control control = new();
        ControlDesigner designer = new();
        Mock<AccessibleObject> mockAccessibleObject = new();
        mockAccessibleObject.Setup(a => a.State).Returns(state);

        AccessibleObject? expectedAccessibleObject = null;
        if (methodName == nameof(ControlDesigner.ControlDesignerAccessibleObject.HitTest))
        {
            expectedAccessibleObject = new Mock<AccessibleObject>().Object;
            mockAccessibleObject.Setup(a => a.HitTest(x, y)).Returns(expectedAccessibleObject);
        }

        TestControl testControl = new(mockAccessibleObject.Object);
        ControlDesigner.ControlDesignerAccessibleObject accessibleObject = new(designer, testControl);

        object? result = methodName switch
        {
            nameof(ControlDesigner.ControlDesignerAccessibleObject.GetFocused) => accessibleObject.GetFocused(),
            nameof(ControlDesigner.ControlDesignerAccessibleObject.GetSelected) => accessibleObject.GetSelected(),
            nameof(ControlDesigner.ControlDesignerAccessibleObject.HitTest) => accessibleObject.HitTest(x, y),

            _ => throw new ArgumentException("Invalid method name", nameof(methodName))
        };

        if (methodName == nameof(ControlDesigner.ControlDesignerAccessibleObject.HitTest))
        {
            result.Should().Be(expectedAccessibleObject);
        }
        else
        {
            result.Should().Be(accessibleObject);
        }
    }
}
