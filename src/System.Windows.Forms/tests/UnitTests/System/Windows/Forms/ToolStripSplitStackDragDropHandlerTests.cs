// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripSplitStackDragDropHandlerTests : IDisposable
{
    private readonly ToolStrip _toolStrip;
    private readonly ToolStripSplitStackDragDropHandler _toolStripSplitStackDragDropHandler;
    public ToolStripSplitStackDragDropHandlerTests()
    {
        _toolStrip = new();
        _toolStripSplitStackDragDropHandler = new(_toolStrip);
    }

    public void Dispose() => _toolStrip.Dispose();

    [WinFormsFact]
    public void OnDragEnter_SetsEffectToMove_WhenDataIsToolStripItem()
    {
        using ToolStripButton toolStripItem = new();
        DataObject dataObject = new();
        dataObject.SetData(typeof(ToolStripItem), toolStripItem);
        DragEventArgs dragEventArgs = new(dataObject, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);

        _toolStripSplitStackDragDropHandler.OnDragEnter(dragEventArgs);

        dragEventArgs.Effect.Should().Be(DragDropEffects.Move);
    }

    [WinFormsFact]
    public void OnDragEnter_DoesNotSetEffect_WhenDataIsNotToolStripItem()
    {
        DataObject dataObject = new();
        DragEventArgs dragEventArgs = new(dataObject, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);

        _toolStripSplitStackDragDropHandler.OnDragEnter(dragEventArgs);

        dragEventArgs.Effect.Should().Be(DragDropEffects.None);
    }

    [WinFormsFact]
    public void OnDragDrop_AddsItemToToolStrip_WhenDataIsToolStripItem()
    {
        using ToolStripButton toolStripItem = new();
        DataObject dataObject = new();
        dataObject.SetData(typeof(ToolStripItem), toolStripItem);
        DragEventArgs dragEventArgs = new(dataObject, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);

        _toolStripSplitStackDragDropHandler.OnDragDrop(dragEventArgs);

        _toolStrip.Items[0].Should().Be(toolStripItem);
    }

    [WinFormsFact]
    public void OnDragDrop_DoesNotAddItem_WhenDataIsNotToolStripItem()
    {
        DataObject dataObject = new();
        DragEventArgs dragEventArgs = new(dataObject, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);

        _toolStripSplitStackDragDropHandler.OnDragDrop(dragEventArgs);

        _toolStrip.Items.Count.Should().Be(0);
    }

    [WinFormsFact]
    public void OnDragOver_SetsEffectToMove_WhenDataIsToolStripItem()
    {
        using ToolStripButton toolStripItem = new();
        DataObject dataObject = new();
        dataObject.SetData(typeof(ToolStripItem), toolStripItem);
        DragEventArgs dragEventArgs = new(dataObject, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);

        _toolStripSplitStackDragDropHandler.OnDragOver(dragEventArgs);

        dragEventArgs.Effect.Should().Be(DragDropEffects.Move);
    }

    [WinFormsFact]
    public void OnDragOver_DoesNotSetEffect_WhenDataIsNotToolStripItem()
    {
        DataObject dataObject = new();
        DragEventArgs dragEventArgs = new(dataObject, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);

        _toolStripSplitStackDragDropHandler.OnDragOver(dragEventArgs);

        dragEventArgs.Effect.Should().Be(DragDropEffects.None);
    }

    [WinFormsFact]
    public void OnGiveFeedback_DoesNotThrowException()
    {
        GiveFeedbackEventArgs giveFeedbackEventArgs = new(effect: DragDropEffects.None, useDefaultCursors: false);

        Action action = () => _toolStripSplitStackDragDropHandler.OnGiveFeedback(giveFeedbackEventArgs);
        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void OnQueryContinueDrag_DoesNotThrowException()
    {
        QueryContinueDragEventArgs queryContinueDragEventArgs = new(0, escapePressed: false, DragAction.Continue);

        Action action = () => _toolStripSplitStackDragDropHandler.OnQueryContinueDrag(queryContinueDragEventArgs);

        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void OnDropItem_AddsItemWhenToolStripIsEmpty()
    {
        using ToolStripButton newToolStripItem = new();
        Point point = new(10, 10);

        _toolStripSplitStackDragDropHandler.TestAccessor().Dynamic.OnDropItem(newToolStripItem, point);

        _toolStrip.Items[0].Should().Be(newToolStripItem);
    }

    [WinFormsFact]
    public void ShowItemDropPoint_ReturnsTrue_WhenToolStripIsEmpty()
    {
        Point point = new(10, 10);
        var result = _toolStripSplitStackDragDropHandler.TestAccessor().Dynamic.ShowItemDropPoint(point);

        ((bool)result).Should().BeTrue();
    }
}
