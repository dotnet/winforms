// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ChangeToolStripParentVerbTests : IDisposable
{
    private readonly Mock<IDesignerHost> _designerHostMock = new();
    private readonly Mock<IServiceProvider> _serviceProviderMock = new();
    private readonly Mock<ISite> _siteMock = new();
    private readonly Mock<IComponentChangeService> _componentChangeServiceMock = new()  ;
    private readonly ToolStripDesigner _designer = new();
    private ToolStrip _toolStrip = new();

    public void Dispose()
    {
        _toolStrip?.Dispose();
        _designer.Dispose();
    }

    private ToolStrip MockMinimalControl()
    {
        Mock<ISelectionService> mockSelectionService = new();
        _siteMock
            .Setup(s => s.GetService(typeof(ISelectionService)))
            .Returns(mockSelectionService.Object);
        _siteMock
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(_designerHostMock.Object);
        _serviceProviderMock
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(_designerHostMock.Object);
        _serviceProviderMock
            .Setup(s => s.GetService(typeof(IComponentChangeService)))
            .Returns(new Mock<IComponentChangeService>().Object);
        _designerHostMock
            .Setup(dh => dh.GetService(typeof(IComponentChangeService)))
            .Returns(_componentChangeServiceMock.Object);
        _designerHostMock
            .Setup(dh => dh.GetDesigner(It.IsAny<IComponent>()))
            .Returns(new Mock<ParentControlDesigner>().Object);

        ToolStrip toolStrip = new() { Site = _siteMock.Object };

        return toolStrip;
    }

    [Fact]
    public void ChangeParent_WithoutDesignerActionUIService_DoesNotChangeParent()
    {
        Control? oldParent = _toolStrip.Parent;
        _toolStrip = MockMinimalControl();
        _designer.Initialize(_toolStrip);

        ChangeToolStripParentVerb changeToolStripParentVerb = new(_designer);

        changeToolStripParentVerb.ChangeParent();
        Control? newParent = _toolStrip.Parent;
        newParent.Should().Be(oldParent);
    }
}
