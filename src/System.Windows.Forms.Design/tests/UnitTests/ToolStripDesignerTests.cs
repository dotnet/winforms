// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Forms.Design.Tests.Mocks;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripDesignerTests
{
    [WinFormsFact]
    public void ToolStripDesigner_InitialStateTest()
    {
        using ToolStripDesigner toolStripDesigner = new();
        using ToolStrip toolStrip = new();

        Mock<IComponentChangeService> mockIComponentChangeService = new(MockBehavior.Strict);
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(toolStrip);
        mockDesignerHost
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(mockIComponentChangeService.Object);
        mockDesignerHost.Setup(s => s.AddService(typeof(ToolStripKeyboardHandlingService), It.IsAny<object>()));
        mockDesignerHost.Setup(s => s.AddService(typeof(ISupportInSituService), It.IsAny<object>()));

        var mockSite = MockSite.CreateMockSiteWithDesignerHost(mockDesignerHost.Object);
        mockSite.Setup(s => s.GetService(typeof(BehaviorService))).Returns(null);
        mockSite.Setup(s => s.GetService(typeof(ToolStripAdornerWindowService))).Returns(null);

        Mock<IComponentChangeService> mockComponentChangeService = new(MockBehavior.Strict);
        mockSite.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(mockComponentChangeService.Object);

        toolStrip.Site = mockSite.Object;

        toolStripDesigner.Initialize(toolStrip);

        Assert.Empty(toolStripDesigner.AssociatedComponents);

        toolStrip.Items.Add("123");
        toolStrip.Items.Add("abc");

        Assert.Equal(2, toolStripDesigner.AssociatedComponents.Count);
        Assert.Equal(2, toolStripDesigner.ActionLists.Count);
        Assert.False(toolStripDesigner.CacheItems);
        Assert.False(toolStripDesigner.DontCloseOverflow);
        Assert.IsType<Rectangle>(toolStripDesigner.DragBoxFromMouseDown);
        Assert.Equal(Rectangle.Empty, toolStripDesigner.DragBoxFromMouseDown);
        Assert.False(toolStripDesigner.EditingCollection);
        Assert.NotNull(toolStripDesigner.EditManager);
        Assert.Null(toolStripDesigner.Editor);
        Assert.Null(toolStripDesigner.EditorNode);
        Assert.Null(toolStripDesigner.EditorToolStrip);
        Assert.False(toolStripDesigner.FireSyncSelection);
        Assert.IsType<int>(toolStripDesigner.IndexOfItemUnderMouseToDrag);
        Assert.Equal(-1, toolStripDesigner.IndexOfItemUnderMouseToDrag);
        Assert.Equal(-1, toolStripDesigner.IndexOfItemUnderMouseToDrag);
        Assert.Null(toolStripDesigner.InsertTransaction);
        Assert.Empty(toolStripDesigner.Items);
        Assert.Null(toolStripDesigner.NewItemTransaction);
        Assert.NotNull(toolStripDesigner.SelectionService);
        Assert.True(toolStripDesigner.SerializePerformLayout);
        Assert.True(toolStripDesigner.SupportEditing);
        Assert.True(toolStripDesigner.Visible);
    }
}
