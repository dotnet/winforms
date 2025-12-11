// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms.PropertyGridInternal;
using static System.Windows.Forms.PropertyGridInternal.GridEntry;

namespace System.Windows.Forms.Tests;

public class MultiPropertyDescriptorGridEntryTests
{
    private sealed class DummyComponent : Component { }

    private sealed class TestGridEntry : GridEntry
    {
        public TestGridEntry(PropertyGrid ownerGrid)
            : base(ownerGrid, null)
        {
        }
    }

    private static MultiPropertyDescriptorGridEntry CreateEntryWithObjects(object[] objects, PropertyDescriptor[] descriptors)
    {
        using PropertyGrid ownerGrid = new();
        TestGridEntry parent = new(ownerGrid);

        return new MultiPropertyDescriptorGridEntry(ownerGrid, parent, objects, descriptors, false);
    }

    [Fact]
    public void Container_AllObjectsAreIComponentWithSameContainer_ReturnsContainer()
    {
        using Container container = new();
        using DummyComponent dummyComponent1 = new();
        using DummyComponent dummyComponent2 = new();
        container.Add(dummyComponent1);
        container.Add(dummyComponent2);

        PropertyDescriptor[] propertyDescriptors = [TypeDescriptor.GetProperties(dummyComponent1)[0], TypeDescriptor.GetProperties(dummyComponent2)[0]];
        object[] objects = [dummyComponent1, dummyComponent2];

        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, propertyDescriptors);

        IContainer? result = multiPropertyDescriptorGridEntry.Container;

        result.Should().BeSameAs(container);
    }

    [Fact]
    public void Container_ObjectsAreIComponentWithDifferentContainers_ReturnsNull()
    {
        using Container container1 = new();
        using Container container2 = new();
        using DummyComponent dummyComponent1 = new();
        using DummyComponent dummyComponent2 = new();
        container1.Add(dummyComponent1);
        container2.Add(dummyComponent2);

        PropertyDescriptor[] propertyDescriptors = [TypeDescriptor.GetProperties(dummyComponent1)[0], TypeDescriptor.GetProperties(dummyComponent2)[0]];
        object[] objects = [dummyComponent1, dummyComponent2];

        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, propertyDescriptors);

        IContainer? result = multiPropertyDescriptorGridEntry.Container;

        result.Should().BeNull();
    }

    [Fact]
    public void Container_ObjectsAreNotIComponent_ReturnsNull()
    {
        object[] objects = [new object(), new object()];
        using DummyComponent dummyComponent = new();
        PropertyDescriptor[] propertyDescriptors = [TypeDescriptor.GetProperties(dummyComponent)[0], TypeDescriptor.GetProperties(dummyComponent)[0]];

        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, propertyDescriptors);

        IContainer? result = multiPropertyDescriptorGridEntry.Container;

        result.Should().BeNull();
    }

    [Fact]
    public void Container_ObjectsAreIComponent_SomeWithoutSite_ReturnsNull()
    {
        using DummyComponent dummyComponent1 = new();
        using DummyComponent dummyComponent2 = new();
        using Container container = new();
        container.Add(dummyComponent1);

        PropertyDescriptor[] propertyDescriptors = [TypeDescriptor.GetProperties(dummyComponent1)[0], TypeDescriptor.GetProperties(dummyComponent2)[0]];
        object[] objects = [dummyComponent1, dummyComponent2];

        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, propertyDescriptors);

        IContainer? result = multiPropertyDescriptorGridEntry.Container;

        result.Should().BeNull();
    }

    [Fact]
    public void Container_EmptyObjects_ReturnsNull()
    {
        object[] objects = [];
        using DummyComponent dummyComponent = new();
        PropertyDescriptor[] propertyDescriptors = [TypeDescriptor.GetProperties(dummyComponent)[0]];

        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, propertyDescriptors);

        IContainer? result = multiPropertyDescriptorGridEntry.Container;

        result.Should().BeNull();
    }

    [Fact]
    public void Expandable_WhenFlagExpandableAndDescriptionExist_ReturnsTrue()
    {
        using Button button = new();
        PropertyDescriptor[] propertyDescriptors = [TypeDescriptor.GetProperties(button)[0]];
        object[] objects = [button];
        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, propertyDescriptors);

        multiPropertyDescriptorGridEntry.TestAccessor.Dynamic.SetFlag(Flags.Expandable, true);

        string? description = propertyDescriptors[0].Description;
        description.Should().NotBeNull();

        bool result = multiPropertyDescriptorGridEntry.Expandable;

        result.Should().BeTrue();
    }

    [Fact]
    public void Expandable_ExpandableFailedFlag_ReturnsFalse()
    {
        using Button button = new();
        PropertyDescriptor[] propertyDescriptors = [TypeDescriptor.GetProperties(button)[0]];
        object[] objects = [button];
        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, propertyDescriptors);

        multiPropertyDescriptorGridEntry.TestAccessor.Dynamic.SetFlag(Flags.ExpandableFailed, true);

        bool result = multiPropertyDescriptorGridEntry.Expandable;

        result.Should().BeFalse();
    }

    [Fact]
    public void GetComponents_ReturnsCopyOfObjects()
    {
        using DummyComponent dummyComponent1 = new();
        using DummyComponent dummyComponent2 = new();
        PropertyDescriptor[] propertyDescriptors = [TypeDescriptor.GetProperties(dummyComponent1)[0], TypeDescriptor.GetProperties(dummyComponent2)[0]];
        object[] objects = [dummyComponent1, dummyComponent2];
        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, propertyDescriptors);

        IComponent[] result = multiPropertyDescriptorGridEntry.GetComponents();

        result.Should().HaveCount(2);
        result[0].Should().BeSameAs(dummyComponent1);
        result[1].Should().BeSameAs(dummyComponent2);
    }

    [Fact]
    public void GetPropertyTextValue_ValueIsNullAndMergedDescriptorReturnsNull_ReturnsEmptyString()
    {
        using DummyComponent dummyComponent1 = new();
        PropertyDescriptor[] propertyDescriptors = [TypeDescriptor.GetProperties(dummyComponent1)[0]];
        object[] objects = [dummyComponent1];
        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, propertyDescriptors);

        string result = multiPropertyDescriptorGridEntry.GetPropertyTextValue(null);

        result.Should().Be("(none)");
    }

    [Fact]
    public void SendNotification_GridEntry_CreatesTransactionAndCommits()
    {
        using DummyComponent dummyComponent1 = new();
        PropertyDescriptor[] propertyDescriptors = [TypeDescriptor.GetProperties(dummyComponent1)[0]];
        object[] objects = [dummyComponent1];
        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, propertyDescriptors);

        using PropertyGrid propertyGrid = new();
        GridEntry entryParam = new TestGridEntry(propertyGrid);

        bool result = multiPropertyDescriptorGridEntry.SendNotification(entryParam, Notify.Reset);

        result.Should().BeFalse();
    }

    [Fact]
    public void NotifyParentsOfChanges_NotifiesParentProperties()
    {
        using DummyComponent dummyComponent1 = new();
        PropertyDescriptor[] propertyDescriptors = [TypeDescriptor.GetProperties(dummyComponent1)[0]];
        object[] objects = [dummyComponent1];
        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, propertyDescriptors);

        Action action = () => multiPropertyDescriptorGridEntry
            .TestAccessor
            .Dynamic
            .NotifyParentsOfChanges(multiPropertyDescriptorGridEntry);

        action.Should().NotThrow();
    }

    [Fact]
    public void SendNotification_GridEntry_HandlesResetAndDoubleClick()
    {
        using DummyComponent dummyComponent1 = new();
        PropertyDescriptor[] propertyDescriptors = [TypeDescriptor.GetProperties(dummyComponent1)[0]];
        object[] objects = [dummyComponent1];
        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, propertyDescriptors);

        using PropertyGrid propertyGrid = new();
        TestGridEntry gridEntry = new(propertyGrid);

        bool resultReset = multiPropertyDescriptorGridEntry.SendNotification(gridEntry, Notify.Reset);
        bool resultDoubleClick = multiPropertyDescriptorGridEntry.SendNotification(gridEntry, Notify.DoubleClick);

        resultReset.Should().BeFalse();
        resultDoubleClick.Should().BeFalse();
    }

    [Fact]
    public void OwnersEqual_ComparesOwnersCorrectly()
    {
        object o1 = new();
        object o2 = new();

        var accessor = typeof(MultiPropertyDescriptorGridEntry).TestAccessor;
        bool result1 = accessor.Dynamic.OwnersEqual(o1, o1);
        bool result2 = accessor.Dynamic.OwnersEqual(new[] { o1, o2 }, new[] { o1, o2 });
        bool result3 = accessor.Dynamic.OwnersEqual(new[] { o1 }, new[] { o2 });

        result1.Should().BeTrue();
        result2.Should().BeTrue();
        result3.Should().BeFalse();
    }

    [Fact]
    public void OnComponentChanging_CallsChangeServiceForEachObject()
    {
        using DummyComponent dummyComponent1 = new();
        using DummyComponent dummyComponent2 = new();
        PropertyDescriptor[] descriptors = [TypeDescriptor.GetProperties(dummyComponent1)[0], TypeDescriptor.GetProperties(dummyComponent2)[0]];
        object[] objects = [dummyComponent1, dummyComponent2];
        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, descriptors);

        bool result = multiPropertyDescriptorGridEntry.OnComponentChanging();

        result.Should().BeTrue();
    }

    [Fact]
    public void OnComponentChanged_DoesNothing_WhenServiceIsNull()
    {
        using DummyComponent dummyComponent1 = new();
        using DummyComponent dummyComponent2 = new();
        PropertyDescriptor[] propertyDescriptors =
        [
            TypeDescriptor.GetProperties(dummyComponent1)[0],
            TypeDescriptor.GetProperties(dummyComponent2)[0]
        ];
        object[] objects = [dummyComponent1, dummyComponent2];
        MultiPropertyDescriptorGridEntry multiPropertyDescriptorGridEntry = CreateEntryWithObjects(objects, propertyDescriptors);

        multiPropertyDescriptorGridEntry.Invoking(e => e.OnComponentChanged()).Should().NotThrow();
    }
}
