// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ComponentTrayTests : IDisposable
{
    private readonly ComponentTray _componentTray;

    public ComponentTrayTests() => _componentTray = new(new Mock<IDesigner>().Object, MockServiceProvider().Object);

    public void Dispose() => _componentTray.Dispose();

    [Fact]
    public void Constructor_Default()
    {
        _componentTray.Should().BeOfType<ComponentTray>();
        _componentTray.Visible.Should().BeTrue();
        _componentTray.AutoScroll.Should().BeTrue();
        _componentTray.AllowDrop.Should().BeTrue();
        _componentTray.Text.Should().Be("ComponentTray");
        _componentTray.Controls.Count.Should().Be(0);
    }

    [Fact]
    public void AutoArrange_Get_Sets_Properly()
    {
        _componentTray.AutoArrange.Should().BeFalse();
        _componentTray.AutoArrange = true;
        _componentTray.AutoArrange.Should().BeTrue();
    }

    private Mock<IServiceProvider> MockServiceProvider()
    {
        Dictionary<Type, object> serviceMap = new()
        {
            { typeof(IExtenderProviderService), new Mock<IExtenderProviderService>().Object},
            { typeof(IDesignerHost), new Mock<IDesignerHost>().Object},
            { typeof(IEventHandlerService), new Mock<IEventHandlerService>().Object },
            { typeof(IMenuCommandService), new Mock<IMenuCommandService>().Object},
            { typeof(IComponentChangeService), new Mock<IComponentChangeService>().Object},
            { typeof(ISelectionService), new Mock<ISelectionService>().Object},
        };

        Mock<IServiceProvider> serviceProviderMock = new();
        foreach (KeyValuePair<Type, object> service in serviceMap)
        {
            serviceProviderMock
                .Setup(s => s.GetService(service.Key))
                .Returns(service.Value);
        }

        return serviceProviderMock;
    }
}
