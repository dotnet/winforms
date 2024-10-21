// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel;

namespace System.Windows.Forms.Design.Tests;

public class DataGridViewComponentPropertyGridSiteTests
{
    [Fact]
    public void Constructor_ShouldInitializeComponent()
    {
        Mock<IComponent> componentMock = new();

        DataGridViewComponentPropertyGridSite site = new(null, componentMock.Object);

        site.Component.Should().Be(componentMock.Object);
    }

    [Fact]
    public void GetService_ShouldReturnNull_WhenServiceProviderIsNull()
    {
        Mock<IComponent> componentMock = new();
        DataGridViewComponentPropertyGridSite site = new(null, componentMock.Object);

        var service = site.GetService(typeof(object));

        service.Should().BeNull();
    }

    [Fact]
    public void GetService_ShouldReturnNull_WhenInGetServiceIsTrue()
    {
        Mock<IComponent> componentMock = new();
        Mock<IServiceProvider> serviceProviderMock = new();
        DataGridViewComponentPropertyGridSite site = new(serviceProviderMock.Object, componentMock.Object);

        site.TestAccessor().Dynamic._inGetService = true;

        var service = site.GetService(typeof(object));

        service.Should().BeNull();
    }

    [Fact]
    public void GetService_ShouldReturnService_WhenServiceProviderIsNotNullAndInGetServiceIsFalse()
    {
        Mock<IComponent> componentMock = new();
        Mock<IServiceProvider> serviceProviderMock = new();
        object expectedService = new();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(object))).Returns(expectedService);
        DataGridViewComponentPropertyGridSite site = new(serviceProviderMock.Object, componentMock.Object);

        var service = site.GetService(typeof(object));

        service.Should().Be(expectedService);
    }
}
