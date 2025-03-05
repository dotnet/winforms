// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class OleDragDropHandlerTests : IDisposable
{
    private readonly Mock<SelectionUIHandler> _selectionHandlerMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IOleDragClient> _clientMock;
    private readonly OleDragDropHandler _oleDragDropHandler;
    private readonly IComponent _component;

    public OleDragDropHandlerTests()
    {
        _selectionHandlerMock = new() { CallBase = true };
        _serviceProviderMock = new();
        _clientMock = new();
        _component = new Component();
        _oleDragDropHandler = new(_selectionHandlerMock.Object, _serviceProviderMock.Object, _clientMock.Object);
    }

    public void Dispose()
    {
        _component.Dispose();
    }

    [Fact]
    public void DataFormat_ShouldReturnCF_CODE()
    {
        OleDragDropHandler.DataFormat.Should().Be(OleDragDropHandler.CF_CODE);
    }

    [Fact]
    public void ExtraInfoFormat_ShouldReturnCF_COMPONENTTYPES()
    {
        OleDragDropHandler.ExtraInfoFormat.Should().Be(OleDragDropHandler.CF_COMPONENTTYPES);
    }

    [Fact]
    public void NestedToolboxItemFormat_ShouldReturnCF_TOOLBOXITEM()
    {
        OleDragDropHandler.NestedToolboxItemFormat.Should().Be(OleDragDropHandler.CF_TOOLBOXITEM);
    }

    [Fact]
    public void Dragging_ShouldReturnFalseInitially()
    {
        _oleDragDropHandler.Dragging.Should().BeFalse();
    }

    [Fact]
    public void FreezePainting_ShouldReturnFalseInitially()
    {
        OleDragDropHandler.FreezePainting.Should().BeFalse();
    }

    [Fact]
    public void CreateTool_ShouldReturnEmptyArray_WhenCheckoutExceptionIsCanceled()
    {
        ToolboxItem toolboxItem = new();
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IDesignerHost))).Returns((IDesignerHost?)null);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IToolboxService))).Returns((IToolboxService?)null);

        var result = _oleDragDropHandler.CreateTool(toolboxItem, null, 0, 0, 0, 0, true, true);

        result.Should().BeEmpty();
    }

    [Fact]
    public void DoBeginDrag_ShouldReturnTrue_WhenRulesAreNotSizeable()
    {
        object[] components = [_component];
        bool result = _oleDragDropHandler.DoBeginDrag(components, SelectionRules.Moveable, 0, 0);

        result.Should().BeTrue();
    }

    [Fact]
    public void DoBeginDrag_ShouldReturnTrue_WhenRulesAreSizeable()
    {
        object[] components = [_component];
        bool result = _oleDragDropHandler.DoBeginDrag(components, SelectionRules.AllSizeable, 0, 0);

        result.Should().BeTrue();
    }

    [Fact]
    public void DoEndDrag_ShouldSetDraggingToFalse()
    {
        _oleDragDropHandler.DoEndDrag();

        _oleDragDropHandler.Dragging.Should().BeFalse();
    }

    [Fact]
    public void DoOleDragDrop_ShouldSetEffectToNone_WhenSelectionHandlerIsNotNull()
    {
        OleDragDropHandler handler = new(_selectionHandlerMock.Object, _serviceProviderMock.Object, _clientMock.Object);
        DragEventArgs dragEventArgs = new(new DataObject(), 0, 0, 0, DragDropEffects.None, DragDropEffects.None);

        handler.DoOleDragDrop(dragEventArgs);

        dragEventArgs.Effect.Should().Be(DragDropEffects.None);
    }

    [Fact]
    public void DoOleDragEnter_ShouldSetEffectToNone_WhenDraggingIsFalseAndCannotDropDataObject()
    {
        DragEventArgs dragEventArgs = new(new DataObject(), 0, 0, 0, DragDropEffects.None, DragDropEffects.None);

        _oleDragDropHandler.DoOleDragEnter(dragEventArgs);

        dragEventArgs.Effect.Should().Be(DragDropEffects.None);
    }

    [Fact]
    public void DoOleDragEnter_ShouldSetEffectToNone_WhenCannotModifyComponents()
    {
        DragEventArgs dragEventArgs = new(new DataObject(), 0, 0, 0, DragDropEffects.Move, DragDropEffects.None);
        _clientMock.Setup(client => client.CanModifyComponents).Returns(false);

        _oleDragDropHandler.DoOleDragEnter(dragEventArgs);

        dragEventArgs.Effect.Should().Be(DragDropEffects.None);
    }

    [Fact]
    public void DoOleDragLeave_ShouldSetDraggingToFalse()
    {
        _oleDragDropHandler.DoOleDragLeave();

        _oleDragDropHandler.Dragging.Should().BeFalse();
    }

    [Fact]
    public void DoOleDragOver_ShouldSetEffectToNone_WhenDraggingIsFalseAndDragOkIsFalse()
    {
        DragEventArgs dragEventArgs = new(new DataObject(), 0, 0, 0, DragDropEffects.None, DragDropEffects.None);

        _oleDragDropHandler.DoOleDragOver(dragEventArgs);

        dragEventArgs.Effect.Should().Be(DragDropEffects.None);
    }

    [Fact]
    public void DoOleDragOver_ShouldSetEffectToNone_WhenNotDraggingAndAllowedEffectIsNone()
    {
        DragEventArgs dragEventArgs = new(new DataObject(), 0, 0, 0, DragDropEffects.None, DragDropEffects.None);

        _oleDragDropHandler.DoOleDragOver(dragEventArgs);

        dragEventArgs.Effect.Should().Be(DragDropEffects.None);
    }

    [Fact]
    public void DoOleGiveFeedback_ShouldSetUseDefaultCursorsToTrue_WhenEffectIsNotNoneAndSelectionHandlerIsNotNull()
    {
        GiveFeedbackEventArgs giveFeedbackEventArgs = new(DragDropEffects.Move, false);

        _oleDragDropHandler.DoOleGiveFeedback(giveFeedbackEventArgs);

        giveFeedbackEventArgs.UseDefaultCursors.Should().BeTrue();
        _selectionHandlerMock.Verify(handler => handler.SetCursor(), Times.Never);
    }

    [Fact]
    public void GetDraggingObjects_ShouldReturnNull_WhenDataObjectIsNull()
    {
        OleDragDropHandler.GetDraggingObjects((IDataObject?)null).Should().BeNull();
    }

    [Fact]
    public void GetDraggingObjects_ShouldReturnNull_WhenDragEventArgsDataIsNull()
    {
        DragEventArgs dragEventArgs = new(null, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);

        OleDragDropHandler.GetDraggingObjects(dragEventArgs).Should().BeNull();
    }
}
