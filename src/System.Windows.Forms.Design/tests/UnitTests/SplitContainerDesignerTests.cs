// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using Moq;
using System.Windows.Forms.Design.Tests.Mocks;

namespace System.Windows.Forms.Design.Tests;

public class SplitContainerDesignerTests
{
    [WinFormsFact]
    public void SplitContainerDesigner_AssociatedComponentsTest()
    {
        using SplitContainer splitContainer = new();
        using SplitContainerDesigner splitContainerDesigner = new();

        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(splitContainer);
        mockDesignerHost
            .Setup(s => s.GetDesigner(It.IsAny<Control>()))
            .Returns(splitContainerDesigner);
        Mock<IComponentChangeService> mockComponentChangeService = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);

        var mockSite = MockSite.CreateMockSiteWithDesignerHost(mockDesignerHost.Object);
        splitContainer.Site = mockSite.Object;

        splitContainerDesigner.Initialize(splitContainer);

        Assert.Empty(splitContainerDesigner.AssociatedComponents);

        using Control control = new();
        control.Parent = splitContainer.Panel1;

        Assert.Equal(1, splitContainerDesigner.AssociatedComponents.Count);
    }
}
