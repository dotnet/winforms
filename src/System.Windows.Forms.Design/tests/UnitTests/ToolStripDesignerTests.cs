// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripDesignerTests
{
    private static Mock<ISite> CreateMockSiteWithDesignerHost(object designerHost)
    {
        Mock<ISelectionService> mockISelectionService = new(MockBehavior.Loose);

        Mock<ISite> mockSite = new(MockBehavior.Loose);
        mockSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(designerHost);
        mockSite
            .Setup(s => s.GetService(typeof(IInheritanceService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IDictionaryService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(IExtenderListService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(DesignerActionService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(INestedContainer)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(ISelectionService)))
            .Returns(mockISelectionService.Object);
        mockSite
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(null);
        mockSite
            .SetupGet(s => s.Container)
            .Returns((IContainer)null);

        return mockSite;
    }

    [WinFormsFact]
    public void ToolStripDesigner_AssociatedComponentsTest()
    {
        using ToolStripDesigner toolStripDesigner = new();
        using ToolStrip toolStrip = new();

        Mock<IComponentChangeService> mockIComponentChangeService = new(MockBehavior.Loose);
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Loose);
        mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(toolStrip);
        mockDesignerHost
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(mockIComponentChangeService.Object);

        var mockSite = CreateMockSiteWithDesignerHost(mockDesignerHost.Object);
        toolStrip.Site = mockSite.Object;

        toolStripDesigner.Initialize(toolStrip);

        Assert.Empty(toolStripDesigner.AssociatedComponents);

        toolStrip.Items.Add("123");
        toolStrip.Items.Add("abc");

        Assert.Equal(2, toolStripDesigner.AssociatedComponents.Count);
    }
}
