// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using Moq;
using System.Windows.Forms.Design.Tests.Mocks;
using System.ComponentModel;

namespace System.Windows.Forms.Design.Tests;

public class SplitContainerDesignerTests
{
    private Mock<ISite> GetMockSize(SplitContainer splitContainer, SplitContainerDesigner splitContainerDesigner)
    {
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

        return MockSite.CreateMockSiteWithDesignerHost(mockDesignerHost.Object);
    }

    [WinFormsFact]
    public void SplitContainerDesigner_AssociatedComponentsTest()
    {
        using SplitContainer splitContainer = new();
        using SplitContainerDesigner splitContainerDesigner = new();

        var mockSite = GetMockSize(splitContainer, splitContainerDesigner);
        splitContainer.Site = mockSite.Object;

        splitContainerDesigner.Initialize(splitContainer);

        Assert.Empty(splitContainerDesigner.AssociatedComponents);

        using Control control = new();
        control.Parent = splitContainer.Panel1;

        Assert.Single(splitContainerDesigner.AssociatedComponents);
    }

    // Regression test for https://github.com/dotnet/winforms/issues/11793
    [WinFormsFact]
    public void SplitContainerDesigner_ActionListsTest()
    {
        using SplitContainer splitContainer = new();
        SplitContainerDesigner splitContainerDesigner = new();

        var mockSite = GetMockSize(splitContainer, splitContainerDesigner);
        splitContainer.Site = mockSite.Object;

        splitContainerDesigner.Initialize(splitContainer);

        splitContainerDesigner.Dispose();

        splitContainerDesigner.ActionLists.Count.Should().Be(0);
    }
}
