// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Moq;
using Moq.Protected;

namespace System.Windows.Forms.Design.Tests;

public class SelectionUIHandlerTests : IDisposable
{
    private readonly Mock<SelectionUIHandler> _selectionUIHandlerMock;
    private readonly Control _control;
    private readonly IComponent _component;
    private readonly Mock<IDesignerHost> _designerHostMock;
    private readonly Mock<IComponentChangeService> _changedServiceMock;

    public SelectionUIHandlerTests()
    {
        _selectionUIHandlerMock = new() { CallBase = true };
        _control = new();
        _component = new Mock<IComponent>().Object;
        _designerHostMock = new();
        _changedServiceMock = new();

        _selectionUIHandlerMock.Protected().Setup<IComponent>("GetComponent").Returns(_component);
        _selectionUIHandlerMock.Protected().Setup<Control>("GetControl").Returns(_control);
        _selectionUIHandlerMock.Protected().Setup<Control>("GetControl", ItExpr.IsAny<IComponent>()).Returns(_control);
        _selectionUIHandlerMock.Protected().Setup<Size>("GetCurrentSnapSize").Returns(new Size(10, 10));
        _selectionUIHandlerMock.Protected().Setup<bool>("GetShouldSnapToGrid").Returns(true);
        _selectionUIHandlerMock.Protected().Setup<object>("GetService", typeof(IDesignerHost)).Returns(_designerHostMock.Object);
        _selectionUIHandlerMock.Protected().Setup<object>("GetService", typeof(IComponentChangeService)).Returns(_changedServiceMock.Object);
    }

    public void Dispose()
    {
        _control.Dispose();
        _component.Dispose();
    }

    [Fact]
    public void BeginDrag_ShouldReturnTrueAndInitializeDragState()
    {
        object[] components = [_component];
        bool result = _selectionUIHandlerMock.Object.BeginDrag(components, SelectionRules.Moveable, 0, 0);

        result.Should().BeTrue();

        Control[] dragControls = _selectionUIHandlerMock.Object.TestAccessor().Dynamic._dragControls;

        dragControls.Should().NotBeNull();
        dragControls.Should().HaveCount(1);
        dragControls[0].Should().Be(_control);
    }

    [Fact]
    public void DragMoved_ShouldUpdateDragOffset()
    {
        object[] components = [_component];
        Rectangle offset = new(10, 10, 0, 0);

        // Ensure BeginDrag is called to initialize the drag state
        _selectionUIHandlerMock.Object.BeginDrag(components, SelectionRules.Moveable, 0, 0);

        _selectionUIHandlerMock.Object.DragMoved(components, offset);

        Rectangle dragOffset = _selectionUIHandlerMock.Object.TestAccessor().Dynamic._dragOffset;

        dragOffset.Should().Be(offset);
    }

    [Fact]
    public void EndDrag_ShouldResetDragState()
    {
        object[] components = [_component];

        // Ensure BeginDrag is called to initialize the drag state
        _selectionUIHandlerMock.Object.BeginDrag(components, SelectionRules.Moveable, 0, 0);

        _selectionUIHandlerMock.Object.EndDrag(components, cancel: false);

        Control[]? dragControls = _selectionUIHandlerMock.Object.TestAccessor().Dynamic._dragControls;
        object? originalCoordinates = _selectionUIHandlerMock.Object.TestAccessor().Dynamic._originalCoordinates;
        Rectangle dragOffset = _selectionUIHandlerMock.Object.TestAccessor().Dynamic._dragOffset;

        dragControls.Should().BeNull();
        originalCoordinates.Should().BeNull();
        dragOffset.Should().Be(Rectangle.Empty);
    }

    [Fact]
    public void QueryBeginDrag_ShouldReturnFalse_WhenCheckoutExceptionIsThrown()
    {
        _changedServiceMock.Setup(cs => cs.OnComponentChanging(It.IsAny<object>(), It.IsAny<PropertyDescriptor>())).Throws(CheckoutException.Canceled);

        object[] components = [_component];
        bool result = _selectionUIHandlerMock.Object.QueryBeginDrag(components);

        result.Should().BeFalse();
    }

    [Fact]
    public void QueryBeginDrag_ShouldReturnFalse_WhenInvalidOperationExceptionIsThrown()
    {
        _changedServiceMock.Setup(cs => cs.OnComponentChanging(It.IsAny<object>(), It.IsAny<PropertyDescriptor>())).Throws<InvalidOperationException>();

        object[] components = [_component];
        bool result = _selectionUIHandlerMock.Object.QueryBeginDrag(components);

        result.Should().BeFalse();
    }

    [Fact]
    public void QueryBeginDrag_ShouldReturnTrueForValidComponents()
    {
        object[] components = [_component];
        bool result = _selectionUIHandlerMock.Object.QueryBeginDrag(components);

        result.Should().BeTrue();
    }

    [Fact]
    public void QueryBeginDrag_ShouldReturnFalseForInvalidComponents()
    {
        object[] components = Array.Empty<object>();
        bool result = _selectionUIHandlerMock.Object.QueryBeginDrag(components);

        result.Should().BeFalse();
    }

    [Fact]
    public void GetUpdatedRect_ShouldReturnUpdatedRectangle()
    {
        Rectangle originalRect = new(10, 10, 100, 100);
        Rectangle dragRect = new(15, 15, 110, 110);
        bool updateSize = true;

        _selectionUIHandlerMock.Setup(h => h.GetUpdatedRect(originalRect, dragRect, updateSize))
                               .Returns(dragRect);

        Rectangle result = _selectionUIHandlerMock.Object.GetUpdatedRect(originalRect, dragRect, updateSize);

        result.Should().Be(dragRect);
    }

    [Fact]
    public void GetUpdatedRect_ShouldReturnOriginalRectangleWhenNoUpdate()
    {
        Rectangle originalRect = new(10, 10, 100, 100);
        Rectangle dragRect = new(15, 15, 110, 110);
        bool updateSize = false;

        _selectionUIHandlerMock.Setup(h => h.GetUpdatedRect(originalRect, dragRect, updateSize))
                               .Returns(originalRect);

        Rectangle result = _selectionUIHandlerMock.Object.GetUpdatedRect(originalRect, dragRect, updateSize);

        result.Should().Be(originalRect);
    }

    [Fact]
    public void SetCursor_ShouldNotThrowAndSetCorrectCursor()
    {
        Cursor expectedCursor = Cursors.Hand;
        _selectionUIHandlerMock.Setup(h => h.SetCursor()).Callback(() => _control.Cursor = expectedCursor);

        Action act = _selectionUIHandlerMock.Object.SetCursor;

        act.Should().NotThrow();
        _control.Cursor.Should().Be(expectedCursor);
    }

    [Fact]
    public void OleDragEnter_ShouldNotThrow()
    {
        DragEventArgs dragEventArgs = new(new DataObject(), 0, 0, 0, DragDropEffects.None, DragDropEffects.None);
        Action act = () => _selectionUIHandlerMock.Object.OleDragEnter(dragEventArgs);

        act.Should().NotThrow();
    }

    [Fact]
    public void OleDragDrop_ShouldNotThrow()
    {
        DragEventArgs dragEventArgs = new(new DataObject(), 0, 0, 0, DragDropEffects.None, DragDropEffects.None);
        Action act = () => _selectionUIHandlerMock.Object.OleDragDrop(dragEventArgs);

        act.Should().NotThrow();
    }

    [Fact]
    public void OleDragOver_ShouldNotThrow()
    {
        DragEventArgs dragEventArgs = new(new DataObject(), 0, 0, 0, DragDropEffects.None, DragDropEffects.None);
        Action act = () => _selectionUIHandlerMock.Object.OleDragOver(dragEventArgs);

        act.Should().NotThrow();
    }

    [Fact]
    public void OleDragLeave_ShouldNotThrow()
    {
        Action act = _selectionUIHandlerMock.Object.OleDragLeave;

        act.Should().NotThrow();
    }
}
