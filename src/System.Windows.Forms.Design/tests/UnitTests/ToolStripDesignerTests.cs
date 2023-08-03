// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Forms.Design.Tests.Mocks;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripDesignerTests
{
    [WinFormsFact]
    public void ToolStripDesigner_AssociatedComponentsTest()
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
        toolStrip.Site = mockSite.Object;

        toolStripDesigner.Initialize(toolStrip);

        Assert.Empty(toolStripDesigner.AssociatedComponents);

        toolStrip.Items.Add("123");
        toolStrip.Items.Add("abc");

        Assert.Equal(2, toolStripDesigner.AssociatedComponents.Count);
    }
}
