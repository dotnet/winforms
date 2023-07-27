// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripDropDownDesignerTest
{
    private static Mock<ISite> CreateMockSiteWithDesignerHost(object designerHost)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
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
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(ToolStripKeyboardHandlingService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(ISupportInSituService)))
            .Returns(null);
        mockSite
            .Setup(s => s.GetService(typeof(INestedContainer)))
            .Returns(null);
        
        Mock<ISelectionService> mockSelectionService = new(MockBehavior.Strict);

        mockSite
            .Setup(s => s.GetService(typeof(ISelectionService)))
            .Returns(mockSelectionService.Object);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.Name)
            .Returns("Site");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.GetService(typeof(UndoEngine)))
            .Returns(null);

        return mockSite;
    }

    [WinFormsFact]
    public void ToolStripDropDownDesignerTest_AssociatedComponentsTest()
    {
        ToolStripDropDownDesigner toolStripDropDownDesigner = new();
        ToolStripDropDown toolStripDropDown = new();

        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(toolStripDropDown);
        mockDesignerHost
            .Setup(h => h.Loading)
            .Returns(true);
        Mock<IComponentChangeService> mockComponentChangeService = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);
        mockDesignerHost.Setup(s => s.AddService(It.IsAny<Type>(), It.IsAny<object>()));

        Mock<ISite> mockSite = CreateMockSiteWithDesignerHost(mockDesignerHost.Object);
        toolStripDropDown.Site = mockSite.Object;

        toolStripDropDownDesigner.Initialize(toolStripDropDown);

        Assert.Empty(toolStripDropDownDesigner.AssociatedComponents);

        toolStripDropDown.Items.Add("123");
        toolStripDropDown.Items.Add("456");

        Assert.Equal(2, toolStripDropDownDesigner.AssociatedComponents.Count);
    }
}
