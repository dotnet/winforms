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
        _designerExtenders.Dispose();

        _extenderServiceMock.Verify(s => s.RemoveExtenderProvider(It.IsAny<IExtenderProvider>()), Times.Exactly(2));
        _designerExtenders.Invoking(d => d.Dispose()).Should().NotThrow();

        var providersField = typeof(DesignerExtenders).GetField("_providers", BindingFlags.NonPublic | BindingFlags.Instance);
        var extenderServiceField = typeof(DesignerExtenders).GetField("_extenderService", BindingFlags.NonPublic | BindingFlags.Instance);

        providersField.Should().NotBeNull();
        extenderServiceField.Should().NotBeNull();

        providersField!.GetValue(_designerExtenders).Should().BeNull();
        extenderServiceField!.GetValue(_designerExtenders).Should().BeNull();
    }
}
