// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms.Design.Tests;

public class BindingSourceDesignerTests
{
    [Fact]
    public void BindingUpdatedByUser_SetValue_ShouldUpdateField()
    {
        using BindingSourceDesigner designer = new()
        {
            BindingUpdatedByUser = true
        };

        FieldInfo? fieldInfo = designer.GetType().GetField("_bindingUpdatedByUser", BindingFlags.NonPublic | BindingFlags.Instance);
        fieldInfo.Should().NotBeNull();

        object? fieldValue = fieldInfo?.GetValue(designer);

        fieldValue.Should().NotBeNull();
        fieldValue.Should().Be(true);
    }

    [Fact]
    public void Initialize_ShouldSubscribeToComponentChangeServiceEvents()
    {
        Mock<IComponentChangeService> componentChangeServiceMock = new();
        Mock<IServiceProvider> serviceProviderMock = new();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IComponentChangeService))).Returns(componentChangeServiceMock.Object);

        Mock<IComponent> componentMock = new();
        Mock<ISite> siteMock = new();
        siteMock.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(componentChangeServiceMock.Object);
        componentMock.Setup(c => c.Site).Returns(siteMock.Object);

        using BindingSource bindingSource = new() { DataSource = componentMock.Object };
        using BindingSourceDesigner designer = new();
        designer.Initialize(bindingSource);

        ComponentEventArgs args = new(componentMock.Object);

        MethodInfo? onComponentRemovingMethod = typeof(BindingSourceDesigner).GetMethod("OnComponentRemoving", BindingFlags.NonPublic | BindingFlags.Instance);
        onComponentRemovingMethod?.Invoke(designer, [null, args]);

        bindingSource.DataSource.Should().BeNull();
    }
}
