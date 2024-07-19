// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.Tests
{
    public class PropertyTabCollectionTests
    {
        [WinFormsTheory]
        [InlineData(null, 0)]
        [InlineData(typeof(PropertyGrid), 1)] // PropertyGrid initially contains one PropertiesTab
        public void Count_ReturnsCorrectCount(Type ownerType, int expectedCount)
        {
            if (ownerType is null)
            {
                expectedCount.Should().Be(0);
            }
            else
            {
                var owner = Activator.CreateInstance(ownerType) as PropertyGrid;
                TestPropertyTabCollection propertyTabCollection = new(owner);
                propertyTabCollection.Count.Should().Be(expectedCount);
            }
        }

        [WinFormsTheory]
        [InlineData(null, 0, null, typeof(InvalidOperationException))]
        [InlineData(typeof(PropertyGrid), 0, typeof(PropertyGridInternal.PropertiesTab), null)]
        [InlineData(typeof(PropertyGrid), 1, typeof(TestPropertyTab), null)]
        [InlineData(typeof(PropertyGrid), 2, null, typeof(ArgumentOutOfRangeException))]
        public void Indexer_ReturnsCorrectTab_OrThrows(Type ownerType, int index, Type expectedTabType, Type expectedExceptionType)
        {
            TestPropertyTabCollection propertyTabCollection = null;
            if (ownerType is object)
            {
                var owner = Activator.CreateInstance(ownerType) as PropertyGrid;
                propertyTabCollection = new(owner);
                propertyTabCollection.AddTabType(typeof(TestPropertyTab));
            }

            Action act = () =>
            {
                if (propertyTabCollection is null)
                {
                    throw new InvalidOperationException();
                }

                var tab = propertyTabCollection[index];
            };

            if (expectedExceptionType is null)
            {
                act.Should().NotThrow();
                var tab = propertyTabCollection[index];
                tab.Should().BeOfType(expectedTabType);
            }
            else
            {
                act.Should().Throw<Exception>().Which.Should().BeOfType(expectedExceptionType);
            }
        }

        [WinFormsTheory]
        [InlineData(null, typeof(TestPropertyTab), typeof(ArgumentNullException), 0, false)]
        [InlineData(typeof(PropertyGrid), typeof(TestPropertyTab), null, 2, false)]
        [InlineData(typeof(PropertyGrid), typeof(TestPropertyTab), null, 2, true)]
        public void AddTabType_WithDifferentInputs(Type ownerType, Type tabType, Type expectedExceptionType, int expectedCount, bool addTwice)
        {
            if (ownerType is null)
            {
                Action act = () => new TestPropertyTabCollection(null).AddTabType(tabType);
                act.Should().Throw<Exception>().Which.Should().BeOfType(expectedExceptionType);
            }
            else
            {
                var owner = Activator.CreateInstance(ownerType) as PropertyGrid;
                TestPropertyTabCollection propertyTabCollection = new(owner);
                int initialCount = propertyTabCollection.Count;

                Action act = () =>
                {
                    propertyTabCollection.AddTabType(tabType);
                    if (addTwice)
                    {
                        propertyTabCollection.AddTabType(tabType);
                    }
                };

                if (expectedExceptionType is null)
                {
                    act.Should().NotThrow();
                    propertyTabCollection.Count.Should().Be(expectedCount);
                    if (propertyTabCollection.Count > initialCount)
                    {
                        propertyTabCollection[initialCount].Should().BeOfType<TestPropertyTab>();
                    }
                }
                else
                {
                    act.Should().Throw<Exception>().Which.Should().BeOfType(expectedExceptionType);
                }
            }
        }

        public class TestPropertyTabCollection : PropertyGrid.PropertyTabCollection
        {
            public TestPropertyTabCollection(PropertyGrid ownerPropertyGrid) : base(ownerPropertyGrid)
            {
            }
        }

        public class TestPropertyTab : PropertyTab
        {
            public override string TabName => "Test Tab";

            public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
            {
                return TypeDescriptor.GetProperties(component, attributes);
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
            {
                return TypeDescriptor.GetProperties(component, attributes);
            }

            public override Bitmap Bitmap => new(1, 1);
        }
    }
}
