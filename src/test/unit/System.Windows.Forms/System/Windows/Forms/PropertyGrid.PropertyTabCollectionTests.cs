// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.Tests;

public class PropertyTabCollectionTests : IDisposable
{
    private readonly PropertyGrid _propertyGrid;
    private readonly PropertyGrid.PropertyTabCollection _propertyTabCollection;

    public PropertyTabCollectionTests()
    {
        _propertyGrid = new PropertyGrid();
        _propertyTabCollection = new PropertyGrid.PropertyTabCollection(_propertyGrid);
    }

    public void Dispose()
    {
        _propertyGrid.Dispose();
    }

    [WinFormsTheory]
    [InlineData(typeof(PropertyGrid))]
    public void Count_ReturnsCorrectCount(Type ownerType)
    {
        PropertyGrid owner = Activator.CreateInstance(ownerType) as PropertyGrid;
        TestPropertyTabCollection propertyTabCollection = new(owner);
        propertyTabCollection.Count.Should().Be(1); // PropertyGrid initially contains one PropertiesTab
    }

    [WinFormsTheory]
    [InlineData(typeof(PropertyGrid), 0, typeof(PropertyGridInternal.PropertiesTab))]
    [InlineData(typeof(PropertyGrid), 1, typeof(TestPropertyTab))]
    public void Indexer_ReturnsCorrectTab(Type ownerType, int index, Type expectedTabType)
    {
        PropertyGrid owner = Activator.CreateInstance(ownerType) as PropertyGrid;
        TestPropertyTabCollection propertyTabCollection = new(owner);
        propertyTabCollection.AddTabType(typeof(TestPropertyTab));

        PropertyTab tab = propertyTabCollection[index];
        tab.Should().BeOfType(expectedTabType);
    }

    [WinFormsTheory]
    [InlineData(typeof(PropertyGrid), typeof(TestPropertyTab), 2, false)]
    [InlineData(typeof(PropertyGrid), typeof(TestPropertyTab), 2, true)]
    public void AddTabType_WithDifferentInputs(Type ownerType, Type tabType, int expectedCount, bool addTwice)
    {
        PropertyGrid owner = Activator.CreateInstance(ownerType) as PropertyGrid;
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
        int initialCount = _propertyTabCollection.Count;
        _propertyTabCollection.AddTabType(typeof(TestPropertyTab));

        _propertyTabCollection.Count.Should().Be(initialCount + 1);
        _propertyTabCollection[initialCount].Should().BeOfType<TestPropertyTab>();
    }

    [WinFormsFact]
    public void AddTabType_WithScope_AddsTab()
    {
        int initialCount = _propertyTabCollection.Count;
        _propertyTabCollection.AddTabType(typeof(TestPropertyTab), PropertyTabScope.Component);

        _propertyTabCollection.Count.Should().Be(initialCount + 1);
        _propertyTabCollection[initialCount].Should().BeOfType<TestPropertyTab>();
    }

    [WinFormsFact]
    public void RemoveTabType_RemovesTab()
    {
        _propertyTabCollection.AddTabType(typeof(TestPropertyTab));
        int countAfterAdd = _propertyTabCollection.Count;

        _propertyTabCollection.RemoveTabType(typeof(TestPropertyTab));

        _propertyTabCollection.Count.Should().Be(countAfterAdd - 1);
        _propertyTabCollection.Cast<PropertyTab>().Should().NotContain(tab => tab is TestPropertyTab);
    }

    [WinFormsFact]
    public void Clear_RemovesTabsOfGivenScope()
    {
        _propertyTabCollection.AddTabType(typeof(TestPropertyTab), PropertyTabScope.Component);
        int countAfterAdd = _propertyTabCollection.Count;

        _propertyTabCollection.Clear(PropertyTabScope.Component);

        _propertyTabCollection.Count.Should().BeLessThan(countAfterAdd);
    }

    [WinFormsFact]
    public void CopyTo_CopiesTabsToArray()
    {
        _propertyTabCollection.AddTabType(typeof(TestPropertyTab));
        PropertyTab[] array = new PropertyTab[_propertyTabCollection.Count];

        ((ICollection)_propertyTabCollection).CopyTo(array, 0);

        array.Should().ContainItemsAssignableTo<PropertyTab>();
        array.Should().Contain(tab => tab is TestPropertyTab);
    }

    [WinFormsFact]
    public void GetEnumerator_EnumeratesTabs()
    {
        _propertyTabCollection.AddTabType(typeof(TestPropertyTab));

        int count = 0;
        foreach (PropertyTab tab in _propertyTabCollection)
        {
            tab.Should().NotBeNull();
            count++;
        }

        count.Should().Be(_propertyTabCollection.Count);
    }

    [WinFormsFact]
    public void SyncRoot_ReturnsSelf()
    {
        object syncRoot = ((ICollection)_propertyTabCollection).SyncRoot;

        syncRoot.Should().BeSameAs(_propertyTabCollection);
    }

    [WinFormsFact]
    public void IsSynchronized_ReturnsFalse()
    {
        bool isSynchronized = ((ICollection)_propertyTabCollection).IsSynchronized;

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

        public override Bitmap Bitmap => _bitmap ??= new(1, 1);

        public override void Dispose()
        {
            _bitmap?.Dispose();
            base.Dispose();
        }
    }
}
