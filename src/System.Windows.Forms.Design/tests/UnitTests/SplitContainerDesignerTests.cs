// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class SplitContainerDesignerTests
{
    private static Mock<ISite> CreateMockSiteWithDesignerHost(object designerHost)
    {
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
            .SetupGet(s => s.Container)
            .Returns((IContainer)null);

        return mockSite;
    }

    [WinFormsFact]
    public void SplitContainerDesigner_AssociatedComponentsTest()
    {
        using SplitContainer splitContainer = new();
        using SplitContainerDesigner splitContainerDesigner = new();

        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Loose);
        mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(splitContainer);
        mockDesignerHost
            .Setup(s => s.GetDesigner(splitContainer))
            .Returns(splitContainerDesigner);

        Mock<IComponentChangeService> mockComponentChangeService = new(MockBehavior.Loose);
        mockDesignerHost
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);

        var mockSite = CreateMockSiteWithDesignerHost(mockDesignerHost.Object);
        splitContainer.Site = mockSite.Object;

        splitContainerDesigner.Initialize(splitContainer);

        Assert.Empty(splitContainerDesigner.AssociatedComponents);

        using Control control = new();
        control.Parent = splitContainer.Panel1;

        Assert.Equal(1, splitContainerDesigner.AssociatedComponents.Count);
    }
}
