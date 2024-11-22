// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

internal partial class TestControlDesigner
{
    internal readonly Control _control = new();
    internal readonly Mock<IDesignerHost> _mockDesignerHost = new();
    internal readonly Mock<ISite> _mockSite = new();

    public TestControlDesigner(bool isInitialized = true)
    {
        _mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(_control);
        _mockDesignerHost
            .Setup(s => s.GetDesigner(It.IsAny<Control>()))
            .Returns(this);
        Mock<IComponentChangeService> mockComponentChangeService = new();
        _mockDesignerHost
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);
        _mockSite = CreateMockSiteWithDesignerHost(_mockDesignerHost.Object);
        _control.Site = _mockSite.Object;

        if (isInitialized)
        {
            Initialize(_control);
        }
    }

    public new void Dispose()
    {
        _control.Dispose();
    }

    public static Mock<ISite> CreateMockSiteWithDesignerHost(object designerHost)
    {
        Mock<ISite> mockSite = new();
        mockSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(designerHost);
        mockSite
            .Setup(s => s.GetService(typeof(IInheritanceService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(IDictionaryService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(IExtenderListService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(DesignerActionService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(ToolStripKeyboardHandlingService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(ISupportInSituService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(INestedContainer)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(ToolStripMenuItem)))
            .Returns((object?)null);

        Mock<IServiceProvider> mockServiceProvider = new();

        mockSite
            .Setup(s => s.GetService(typeof(IServiceProvider)))
            .Returns(mockServiceProvider.Object);
        mockSite
            .Setup(s => s.GetService(typeof(ToolStripAdornerWindowService)))
            .Returns((object?)null);
        mockSite
            .Setup(s => s.GetService(typeof(DesignerOptionService)))
            .Returns(mockServiceProvider.Object);

        Mock<ISelectionService> mockSelectionService = new();

        mockSite
            .Setup(s => s.GetService(typeof(ISelectionService)))
            .Returns(mockSelectionService.Object);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer?)null);
        mockSite
            .Setup(s => s.Name)
            .Returns("Site");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.GetService(typeof(UndoEngine)))
            .Returns((object?)null);

        return mockSite;
    }
}
