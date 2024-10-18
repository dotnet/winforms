// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;

namespace System.Windows.Forms.Design.Tests;

public class DesignerExtendersTests : IDisposable
{
    private readonly Mock<IExtenderProviderService> _extenderServiceMock;
    private readonly DesignerExtenders _designerExtenders;

    public DesignerExtendersTests()
    {
        _extenderServiceMock = new();
        _designerExtenders = new(_extenderServiceMock.Object);
    }

    public void Dispose()
    {
        _designerExtenders.Dispose();
    }

    [Fact]
    public void Dispose_RemovesExtenderProviders()
    {
        IExtenderProvider[] providersField = _designerExtenders.TestAccessor().Dynamic._providers;
        IExtenderProviderService extenderServiceField = _designerExtenders.TestAccessor().Dynamic._extenderService;

        providersField.Should().NotBeNull();
        extenderServiceField.Should().NotBeNull();

        _designerExtenders.Dispose();
        _extenderServiceMock.Verify(s => s.RemoveExtenderProvider(It.IsAny<IExtenderProvider>()), Times.Exactly(2));
        _designerExtenders.Invoking(d => d.Dispose()).Should().NotThrow();

        providersField = _designerExtenders.TestAccessor().Dynamic._providers;
        extenderServiceField = _designerExtenders.TestAccessor().Dynamic._extenderService;

        providersField.Should().BeNull();
        extenderServiceField.Should().BeNull();
    }
}
