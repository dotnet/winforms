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
        using Component component = new();

        DataGridViewComponentPropertyGridSite site = new(null, component);

        site.Component.Should().Be(component);
    }

    [Fact]
    public void GetService_ShouldReturnNull_WhenServiceProviderIsNull()
    {
        using Component component = new();
        DataGridViewComponentPropertyGridSite site = new(null, component);

        var service = site.GetService(typeof(object));

        service.Should().BeNull();
    }

    [Fact]
    public void GetService_ShouldReturnNull_WhenInGetServiceIsTrue()
    {
        using Component component = new();
        Mock<IServiceProvider> serviceProviderMock = new();
        DataGridViewComponentPropertyGridSite site = new(serviceProviderMock.Object, component);

        site.TestAccessor().Dynamic._inGetService = true;

        var service = site.GetService(typeof(object));

        service.Should().BeNull();
    }

    [Fact]
    public void GetService_ShouldReturnService_WhenServiceProviderIsNotNullAndInGetServiceIsFalse()
    {
        using Component component = new();
        Mock<IServiceProvider> serviceProviderMock = new();
        object expectedService = new();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(object))).Returns(expectedService);
        DataGridViewComponentPropertyGridSite site = new(serviceProviderMock.Object, component);

        var service = site.GetService(typeof(object));

        service.Should().Be(expectedService);
    }
}
