// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.Design;
using System.ComponentModel;
using Moq;
using System.Windows.Forms.Design.Tests.Mocks;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripDropDownDesignerTest
{
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

        Mock<ISite> mockSite = MockSite.CreateMockSiteWithDesignerHost(mockDesignerHost.Object);
        toolStripDropDown.Site = mockSite.Object;

        toolStripDropDownDesigner.Initialize(toolStripDropDown);

        Assert.Empty(toolStripDropDownDesigner.AssociatedComponents);

        toolStripDropDown.Items.Add("123");
        toolStripDropDown.Items.Add("456");

        Assert.Equal(2, toolStripDropDownDesigner.AssociatedComponents.Count);
    }
}
