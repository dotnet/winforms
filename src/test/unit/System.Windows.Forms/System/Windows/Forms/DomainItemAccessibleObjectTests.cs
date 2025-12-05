// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

#pragma warning disable WFDEV003

public class DomainItemAccessibleObjectTests
{
    [Fact]
    public void Ctor_SetsName()
    {
        DomainUpDown.DomainItemAccessibleObject obj = new("item1", parent: null!);

        obj.Name.Should().Be("item1");
        obj.Value.Should().Be("item1");
    }

    [Fact]
    public void Name_GetSet_Works()
    {
        DomainUpDown.DomainItemAccessibleObject obj = new("initial", parent: null!);

        obj.Name.Should().Be("initial");
        obj.Name = "changed";
        obj.Name.Should().Be("changed");
        obj.Value.Should().Be("changed");
    }

    [Fact]
    public void DomainItemAccessibleObject_InternalProperties_AreCorrect()
    {
        DomainUpDown.DomainItemAccessibleObject obj = new("test", parent: null!);

        obj.CanGetNameInternal.Should().BeFalse();
        obj.CanSetNameInternal.Should().BeFalse();
        obj.Role.Should().Be(AccessibleRole.ListItem);
        obj.State.Should().Be(AccessibleStates.Selectable);
        obj.Value.Should().Be("test");
        obj.CanGetValueInternal.Should().BeFalse();
    }

    [Fact]
    public void RuntimeId_ReturnsExpectedFormat()
    {
        DomainUpDown.DomainItemAccessibleObject obj = new("test", parent: null!);
        int[] runtimeId = obj.RuntimeId;

        runtimeId.Should().BeOfType<int[]>();
        runtimeId.Length.Should().Be(2);
        runtimeId[0].Should().Be(AccessibleObject.RuntimeIDFirstItem);
        runtimeId[1].Should().Be(obj.GetHashCode());
    }
}
