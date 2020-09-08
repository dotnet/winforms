// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Xunit;
using System.Reflection;
using static Interop;
using System.Drawing;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class PropertyDescriptorGridEntryAccessibleObjectTests
    {
        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_Navigates_to_DropDownControlHolder()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using TestPropertyGridView testPropertyGridView = new TestPropertyGridView(null, propertyGrid);

            TestPropertyDescriptorGridEntry gridEntry = new TestPropertyDescriptorGridEntry(propertyGrid, null, false);
            testPropertyGridView.TestAccessor().Dynamic.selectedGridEntry = gridEntry;

            TestDropDownHolder dropDownHolder = new TestDropDownHolder(testPropertyGridView);
            dropDownHolder.SetState(0x00000002, true); // Control class States.Visible flag
            testPropertyGridView.TestAccessor().Dynamic.dropDownHolder = dropDownHolder;
            gridEntry.parentPE = new TestGridEntry(propertyGrid, null, testPropertyGridView);

            UiaCore.IRawElementProviderFragment firstChild = gridEntry.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.NotNull(firstChild);
            Assert.Equal(typeof(PropertyGridView.DropDownHolder.DropDownHolderAccessibleObject), firstChild.GetType());
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_Ctor_Default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            TestPropertyDescriptorGridEntry propertyDescriptorGridEntryTestEntity = new TestPropertyDescriptorGridEntry(propertyGrid, null, false);
            AccessibleObject propertyDescriptorGridEntryAccessibleObject = propertyDescriptorGridEntryTestEntity.AccessibilityObject;

            Assert.NotNull(propertyDescriptorGridEntryAccessibleObject);

            TypeInfo propertyDescriptorGridEntryAccessibleObjectTypeInfo = propertyDescriptorGridEntryAccessibleObject.GetType().GetTypeInfo();
            FieldInfo owningPropertyDescriptorGridEntryField = propertyDescriptorGridEntryAccessibleObjectTypeInfo.GetDeclaredField("_owningPropertyDescriptorGridEntry");
            object owningGridEntry = owningPropertyDescriptorGridEntryField.GetValue(propertyDescriptorGridEntryAccessibleObject);

            Assert.Equal(propertyDescriptorGridEntryTestEntity, owningGridEntry);
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_ExpandCollapseState_collapsed_by_default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            TestPropertyDescriptorGridEntry propertyDescriptorGridEntryTestEntity = new TestPropertyDescriptorGridEntry(propertyGrid, null, false);
            AccessibleObject propertyDescriptorGridEntryAccessibleObject = propertyDescriptorGridEntryTestEntity.AccessibilityObject;

            UiaCore.ExpandCollapseState expandCollapseState = propertyDescriptorGridEntryAccessibleObject.ExpandCollapseState;
            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, expandCollapseState);
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_ExpandCollapseState_reflects_ExpandablePropertyState()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            TestEntity testEntity = new TestEntity();
            testEntity.SizeProperty = Size.Empty;
            propertyGrid.SelectedObject = testEntity;

            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
            Type propertyGridViewType = typeof(PropertyGridView);
            FieldInfo[] propertyGridViewFields = propertyGridViewType.GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            int firstPropertyIndex = 1; // Index 0 corresponds to the category grid entry.
            PropertyDescriptorGridEntry gridEntry = (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[firstPropertyIndex];

            FieldInfo selectedGridEntryField = propertyGridViewFields.First(f => f.Name == "selectedGridEntry");
            PropertyDescriptorGridEntry selectedGridEntry = selectedGridEntryField.GetValue(propertyGridView) as PropertyDescriptorGridEntry;
            Assert.Equal(gridEntry.PropertyName, selectedGridEntry.PropertyName);

            AccessibleObject selectedGridEntryAccessibleObject = gridEntry.AccessibilityObject;

            gridEntry.InternalExpanded = false;
            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, selectedGridEntryAccessibleObject.ExpandCollapseState);

            gridEntry.InternalExpanded = true;
            Assert.Equal(UiaCore.ExpandCollapseState.Expanded, selectedGridEntryAccessibleObject.ExpandCollapseState);
        }

        private class TestGridEntry : GridEntry
        {
            PropertyGridView _propertyGridView;

            public TestGridEntry(PropertyGrid ownerGrid, GridEntry peParent, PropertyGridView propertyGridView)
                : base(ownerGrid, peParent)
            {
                _propertyGridView = propertyGridView;
            }

            internal override PropertyGridView GridEntryHost
            {
                get
                {
                    return _propertyGridView;
                }
            }
        }

        private class TestPropertyDescriptorGridEntry : PropertyDescriptorGridEntry
        {
            private GridEntryCollection _collection;

            public TestPropertyDescriptorGridEntry(PropertyGrid ownerGrid, GridEntry peParent, bool hide)
                : base(ownerGrid, peParent, hide)
            {
            }

            public override GridEntryCollection Children
            {
                get
                {
                    if (_collection is null)
                    {
                        _collection = new GridEntryCollection(this, Array.Empty<GridEntry>());
                    }

                    return _collection;
                }
            }

            internal override bool Enumerable => false;
        }

        private class TestPropertyGridView : PropertyGridView
        {
            private Control _parent;

            public TestPropertyGridView(IServiceProvider serviceProvider, PropertyGrid propertyGrid)
                : base(serviceProvider, propertyGrid)
            {
                _parent = propertyGrid;
            }

            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new TestPropertyGridViewAccessibleObject(this, null);
            }

            internal override Control ParentInternal
            {
                get => _parent;
                set => _parent = value;
            }
        }

        private class TestPropertyGridViewAccessibleObject : PropertyGridView.PropertyGridViewAccessibleObject
        {
            public TestPropertyGridViewAccessibleObject(PropertyGridView owner, PropertyGrid parentPropertyGrid)
                : base(owner, parentPropertyGrid)
            {
            }
        }

        private class TestDropDownHolder : PropertyGridView.DropDownHolder
        {
            public TestDropDownHolder(PropertyGridView psheet)
                : base(psheet)
            {
            }

            internal void SetState(int flag, bool value)
            {
                SetState((States)flag, value);
            }
        }

        internal class TestEntity
        {
            public Size SizeProperty
            {
                get; set;
            }
        }
    }
}
