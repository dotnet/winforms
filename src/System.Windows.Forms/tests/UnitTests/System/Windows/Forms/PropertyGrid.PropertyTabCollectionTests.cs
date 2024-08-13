// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
                if (_bitmap is null)
                {
                    _bitmap = new Bitmap(1, 1);
                }

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
