// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;

public class DesignerExtendersTests
{
    [Fact]
    public void Dispose_RemovesExtenderProviders()
    {
        Mock<IExtenderProviderService> extenderServiceMock = new();
        DesignerExtenders designerExtenders = new(extenderServiceMock.Object);

        IExtenderProvider[] providers = designerExtenders.TestAccessor().Dynamic._providers;
        IExtenderProviderService extenderService = designerExtenders.TestAccessor().Dynamic._extenderService;

        providers.Should().NotBeNull();
        extenderService.Should().NotBeNull();

        designerExtenders.Dispose();
        extenderServiceMock.Verify(s => s.RemoveExtenderProvider(It.IsAny<IExtenderProvider>()), Times.Exactly(2));
        designerExtenders.Invoking(d => d.Dispose()).Should().NotThrow();

        providers = designerExtenders.TestAccessor().Dynamic._providers;
        extenderService = designerExtenders.TestAccessor().Dynamic._extenderService;

        providers.Should().BeNull();
        extenderService.Should().BeNull();
    }
}
