// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Forms.Design.Tests.Mocks;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class UserControlDocumentDesignerTests
{
    [WinFormsFact]
    public void Constructor_ShouldSetAutoResizeHandlesToTrue()
    {
        using UserControlDocumentDesigner designer = new();
        using UserControl control = new();

        Mock<IDesignerHost> designerHost = new();
        Mock<IComponentChangeService> componentChangeService = new(MockBehavior.Strict);
        designerHost
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(componentChangeService.Object);
        Mock<ISelectionService> selectionService = new(MockBehavior.Strict);
        designerHost
            .Setup(s => s.GetService(typeof(ISelectionService)))
            .Returns(selectionService.Object);
        designerHost
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(designerHost.Object);

        var mockSite = MockSite.CreateMockSiteWithDesignerHost(designerHost.Object);
        mockSite
            .Setup(s => s.GetService(typeof(IExtenderProviderService)))
            .Returns(null!);
        mockSite
            .Setup(s => s.GetService(typeof(IUIService)))
            .Returns(null!);
        mockSite
            .Setup(s => s.GetService(typeof(IOverlayService)))
            .Returns(null!);
        mockSite
            .Setup(s => s.GetService(typeof(IMenuCommandService)))
            .Returns(null!);
        mockSite
            .Setup(s => s.GetService(typeof(INameCreationService)))
            .Returns(null!);
        mockSite
            .Setup(s => s.GetService(typeof(IPropertyValueUIService)))
            .Returns(null!);
        Mock<IEventHandlerService> eventHandlerService = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(IEventHandlerService)))
            .Returns(eventHandlerService.Object);
        Mock<IDictionaryService> dictionaryService = new(MockBehavior.Strict);
        dictionaryService
            .Setup(s => s.GetValue(It.IsAny<object>()))
            .Returns(null!);
        dictionaryService
            .Setup(s => s.SetValue(It.IsAny<object>(), It.IsAny<object>()));
        mockSite
            .Setup(s => s.GetService(typeof(IDictionaryService)))
            .Returns(dictionaryService.Object);
        mockSite
            .Setup(s => s.GetService(typeof(ComponentCache)))
            .Returns(null!);
        mockSite
            .Setup(s => s.GetService(typeof(BehaviorService)))
            .Returns(null!);
        control.Site = mockSite.Object;

        designer.Initialize(control);
        designer.AutoResizeHandles.Should().BeTrue();
    }
}
