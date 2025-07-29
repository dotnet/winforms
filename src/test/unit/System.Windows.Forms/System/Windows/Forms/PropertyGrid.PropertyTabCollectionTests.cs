// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.Tests;

public class PropertyTabCollectionTests
{
    [WinFormsTheory]
    [InlineData(typeof(PropertyGrid))]
    public void Count_ReturnsCorrectCount(Type ownerType)
    {
        var owner = Activator.CreateInstance(ownerType) as PropertyGrid;
        TestPropertyTabCollection propertyTabCollection = new(owner);
        propertyTabCollection.Count.Should().Be(1); // PropertyGrid initially contains one PropertiesTab
    }

    [WinFormsTheory]
    [InlineData(typeof(PropertyGrid), 0, typeof(PropertyGridInternal.PropertiesTab))]
    [InlineData(typeof(PropertyGrid), 1, typeof(TestPropertyTab))]
    public void Indexer_ReturnsCorrectTab(Type ownerType, int index, Type expectedTabType)
    {
        var owner = Activator.CreateInstance(ownerType) as PropertyGrid;
        TestPropertyTabCollection propertyTabCollection = new(owner);
        propertyTabCollection.AddTabType(typeof(TestPropertyTab));

        var tab = propertyTabCollection[index];
        tab.Should().BeOfType(expectedTabType);
    }

    [WinFormsTheory]
    [InlineData(typeof(PropertyGrid), typeof(TestPropertyTab), 2, false)]
    [InlineData(typeof(PropertyGrid), typeof(TestPropertyTab), 2, true)]
    public void AddTabType_WithDifferentInputs(Type ownerType, Type tabType, int expectedCount, bool addTwice)
    {
        var owner = Activator.CreateInstance(ownerType) as PropertyGrid;
        TestPropertyTabCollection propertyTabCollection = new(owner);
        int initialCount = propertyTabCollection.Count;

        propertyTabCollection.AddTabType(tabType);
        if (addTwice)
        {
            propertyTabCollection.AddTabType(tabType);
        }

        propertyTabCollection.Count.Should().Be(expectedCount);
        if (propertyTabCollection.Count > initialCount)
        {
            propertyTabCollection[initialCount].Should().BeOfType<TestPropertyTab>();
        }
    }

    [WinFormsFact]
    public void AddTabType_AddsTab()
    {
        using PropertyGrid grid = new();
        PropertyGrid.PropertyTabCollection collection = new(grid);

        int initialCount = collection.Count;
        collection.AddTabType(typeof(TestPropertyTab));

        collection.Count.Should().Be(initialCount + 1);
        collection[initialCount].Should().BeOfType<TestPropertyTab>();
    }

    [WinFormsFact]
    public void AddTabType_WithScope_AddsTab()
    {
        using PropertyGrid grid = new();
        PropertyGrid.PropertyTabCollection collection = new(grid);

        int initialCount = collection.Count;
        collection.AddTabType(typeof(TestPropertyTab), PropertyTabScope.Component);

        collection.Count.Should().Be(initialCount + 1);
        collection[initialCount].Should().BeOfType<TestPropertyTab>();
    }

    [WinFormsFact]
    public void RemoveTabType_RemovesTab()
    {
        using PropertyGrid grid = new();
        PropertyGrid.PropertyTabCollection collection = new(grid);
        collection.AddTabType(typeof(TestPropertyTab));
        int countAfterAdd = collection.Count;

        collection.RemoveTabType(typeof(TestPropertyTab));

        collection.Count.Should().Be(countAfterAdd - 1);
        collection.Cast<PropertyTab>().Should().NotContain(tab => tab is TestPropertyTab);
    }

    [WinFormsFact]
    public void Clear_RemovesTabsOfGivenScope()
    {
        using PropertyGrid grid = new();
        PropertyGrid.PropertyTabCollection collection = new(grid);
        collection.AddTabType(typeof(TestPropertyTab), PropertyTabScope.Component);
        int countAfterAdd = collection.Count;

        collection.Clear(PropertyTabScope.Component);

        collection.Count.Should().BeLessThan(countAfterAdd);
    }

    [WinFormsFact]
    public void CopyTo_CopiesTabsToArray()
    {
        using PropertyGrid grid = new();
        PropertyGrid.PropertyTabCollection collection = new(grid);
        collection.AddTabType(typeof(TestPropertyTab));
        PropertyTab[] array = new PropertyTab[collection.Count];

        ((ICollection)collection).CopyTo(array, 0);

        array.Should().ContainItemsAssignableTo<PropertyTab>();
        array.Should().Contain(tab => tab is TestPropertyTab);
    }

    [WinFormsFact]
    public void GetEnumerator_EnumeratesTabs()
    {
        using PropertyGrid grid = new();
        PropertyGrid.PropertyTabCollection collection = new(grid);
        collection.AddTabType(typeof(TestPropertyTab));

        int count = 0;
        foreach (PropertyTab tab in collection)
        {
            tab.Should().NotBeNull();
            count++;
        }

        count.Should().Be(collection.Count);
    }

    [WinFormsFact]
    public void SyncRoot_ReturnsSelf()
    {
        using PropertyGrid grid = new();
        PropertyGrid.PropertyTabCollection collection = new(grid);

        object syncRoot = ((ICollection)collection).SyncRoot;

        syncRoot.Should().BeSameAs(collection);
    }

    [WinFormsFact]
    public void IsSynchronized_ReturnsFalse()
    {
        using PropertyGrid grid = new();
        PropertyGrid.PropertyTabCollection collection = new(grid);

        bool isSynchronized = ((ICollection)collection).IsSynchronized;

        isSynchronized.Should().BeFalse();
    }

    public class TestPropertyTabCollection : PropertyGrid.PropertyTabCollection
    {
        public TestPropertyTabCollection(PropertyGrid ownerPropertyGrid) : base(ownerPropertyGrid)
        {
        }
    }

    public class TestPropertyTab : PropertyTab, IDisposable
    {
        private Bitmap _bitmap;

        public override string TabName => "Test Tab";

        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes) => TypeDescriptor.GetProperties(component, attributes);

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes) => TypeDescriptor.GetProperties(component, attributes);

        public override Bitmap Bitmap
        {
            get
            {
                _bitmap ??= new Bitmap(1, 1);

                return _bitmap;
            }
        }

        public override void Dispose()
        {
            _bitmap?.Dispose();
            base.Dispose();
        }
    }
}
