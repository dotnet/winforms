// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using Moq;

namespace System.ComponentModel.Design.Tests;

public class UndoUnitTests : UndoEngine
{
    public UndoUnitTests() : base(GetServiceProvider())
    {
    }

    private static IServiceProvider GetServiceProvider()
    {
        Mock<IServiceProvider> mockServiceProvider = new();
        Mock<IDesignerHost> mockDesignerHost = new();
        Mock<IComponentChangeService> mockComponentChangeService = new();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerHost)))
            .Returns(mockDesignerHost.Object);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IComponentChangeService)))
            .Returns(mockComponentChangeService.Object);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ComponentSerializationService)))
            .Returns(new CodeDomComponentSerializationService());
        return mockServiceProvider.Object;
    }

    [Theory]
    [NormalizedStringData]
    public void UndoUnit_Ctor_UndoEngine_String(string name, string expectedName)
    {
        SubUndoUnit unit = new(this, name);
        Assert.Same(this, unit.UndoEngine);
        Assert.Equal(expectedName, unit.Name);
        Assert.True(unit.IsEmpty);
    }

    [Fact]
    public void UndoUnit_NullEngine_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("engine", () => new UndoUnit(null, "name"));
    }

    protected override void AddUndoUnit(UndoUnit unit)
    {
    }

    protected class SubUndoUnit : UndoUnit
    {
        public SubUndoUnit(UndoEngine engine, string name) : base(engine, name)
        {
        }

        public new UndoEngine UndoEngine => base.UndoEngine;
    }
}
