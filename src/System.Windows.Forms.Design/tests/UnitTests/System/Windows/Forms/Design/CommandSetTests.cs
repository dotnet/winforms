// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Moq;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;

public class CommandSetTests
{
    [Fact]
    public void Dispose_DisposesResourcesCorrectly()
    {
        Mock<ISite> mockSite = new();
        Mock<IEventHandlerService> mockEventHandlerService = new();
        Mock<IDesignerHost> mockDesignerHost = new();
        Mock<IMenuCommandService> mockMenuCommandService = new();
        Mock<ISelectionService> mockSelectionService = new();
        Mock<IDictionaryService> mockDictionaryService = new();

        mockSite.Setup(s => s.GetService(typeof(IEventHandlerService))).Returns(mockEventHandlerService.Object);
        mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(mockDesignerHost.Object);
        mockSite.Setup(s => s.GetService(typeof(IMenuCommandService))).Returns(mockMenuCommandService.Object);
        mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(mockSelectionService.Object);
        mockSite.Setup(s => s.GetService(typeof(IDictionaryService))).Returns(mockDictionaryService.Object);

        dynamic accessor;
        using (CommandSet commandSet = new(mockSite.Object))
        {
            accessor = commandSet.TestAccessor().Dynamic;
        }

        mockMenuCommandService.Verify(m => m.RemoveCommand(It.IsAny<MenuCommand>()), Times.AtLeastOnce);
        mockEventHandlerService.VerifyRemove(e => e.EventHandlerChanged -= It.IsAny<EventHandler>(), Times.Once);
        mockDesignerHost.VerifyRemove(h => h.Activated -= It.IsAny<EventHandler>(), Times.Once);
        mockSelectionService.VerifyRemove(s => s.SelectionChanged -= It.IsAny<EventHandler>(), Times.Once);

        ((object)accessor.SelectionService).Should().BeNull();
        ((object)accessor.BehaviorService).Should().BeNull();
        ((object)accessor._menuService).Should().BeNull();
    }
}
