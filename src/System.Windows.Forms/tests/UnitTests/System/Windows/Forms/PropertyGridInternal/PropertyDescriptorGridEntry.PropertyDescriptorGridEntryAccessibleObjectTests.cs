// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class PropertyDescriptorGridEntryAccessibleObjectTests
    {
        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_Navigates_to_DropDownControlHolder()
        {
            using PropertyGrid propertyGrid = new();
            using PropertyGridView propertyGridView = new(serviceProvider: null, propertyGrid);

            TestPropertyGridViewAccessibleObject accessibleObject = new(propertyGridView, parentPropertyGrid: null);
            propertyGridView.Properties.SetObject(propertyGrid.TestAccessor().Dynamic.s_accessibilityProperty, accessibleObject);

            TestPropertyDescriptorGridEntry gridEntry = new(propertyGrid, null, false);
            propertyGridView.TestAccessor().Dynamic._selectedGridEntry = gridEntry;

            PropertyGridView.DropDownHolder dropDownHolder = new(propertyGridView);
            dropDownHolder.TestAccessor().Dynamic.SetState(0x00000002, true); // Control class States.Visible flag
            propertyGridView.TestAccessor().Dynamic._dropDownHolder = dropDownHolder;
            gridEntry.TestAccessor().Dynamic._parent = new TestGridEntry(propertyGrid, null, propertyGridView);

            UiaCore.IRawElementProviderFragment firstChild = gridEntry.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.NotNull(firstChild);
            Assert.Equal(typeof(PropertyGridView.DropDownHolder.DropDownHolderAccessibleObject), firstChild.GetType());
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_Ctor_Default()
        {
            using PropertyGrid propertyGrid = new();
            TestPropertyDescriptorGridEntry propertyDescriptorGridEntryTestEntity = new(propertyGrid, null, false);
            AccessibleObject propertyDescriptorGridEntryAccessibleObject = propertyDescriptorGridEntryTestEntity.AccessibilityObject;

            Assert.NotNull(propertyDescriptorGridEntryAccessibleObject);

            object owningGridEntry = propertyDescriptorGridEntryAccessibleObject.TestAccessor().Dynamic._owningPropertyDescriptorGridEntry;

            Assert.Equal(propertyDescriptorGridEntryTestEntity, owningGridEntry);
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_ExpandCollapseState_collapsed_by_default()
        {
            using PropertyGrid propertyGrid = new();
            TestPropertyDescriptorGridEntry propertyDescriptorGridEntryTestEntity = new(propertyGrid, null, false);
            AccessibleObject propertyDescriptorGridEntryAccessibleObject = propertyDescriptorGridEntryTestEntity.AccessibilityObject;

            UiaCore.ExpandCollapseState expandCollapseState = propertyDescriptorGridEntryAccessibleObject.ExpandCollapseState;
            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, expandCollapseState);
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_ExpandCollapseState_reflects_ExpandablePropertyState()
        {
            using PropertyGrid propertyGrid = new();
            TestEntity testEntity = new();
            testEntity.SizeProperty = Size.Empty;
            propertyGrid.SelectedObject = testEntity;

            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;

            int firstPropertyIndex = 1; // Index 0 corresponds to the category grid entry.
            var gridEntry = (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[firstPropertyIndex];

            var selectedGridEntry = propertyGridView.TestAccessor().Dynamic._selectedGridEntry as PropertyDescriptorGridEntry;
            Assert.Equal(gridEntry.PropertyName, selectedGridEntry.PropertyName);

            AccessibleObject selectedGridEntryAccessibleObject = gridEntry.AccessibilityObject;

            gridEntry.InternalExpanded = false;
            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, selectedGridEntryAccessibleObject.ExpandCollapseState);

            gridEntry.InternalExpanded = true;
            Assert.Equal(UiaCore.ExpandCollapseState.Expanded, selectedGridEntryAccessibleObject.ExpandCollapseState);
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_IsPatternSupported_IfExpandCollapsePatternAndEnumerable_ReturnsTrue()
        {
            using PropertyGrid propertyGrid = new();
            using PropertyGridView propertyGridView = new(serviceProvider: null, propertyGrid);

            TestGridEntry parent = new(propertyGrid, peParent: null, propertyGridView);
            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(typeof(TestEntity)).
                Find(nameof(TestEntity.SizeProperty), ignoreCase: false);

            EnumerablePropertyDescriptorGridEntry gridEntry = new(propertyGrid, parent, propertyDescriptor, hide: false);
            AccessibleObject accessibleObject = gridEntry.AccessibilityObject;

            Assert.True(gridEntry.Enumerable);
            Assert.True(accessibleObject.IsPatternSupported(UiaCore.UIA.ExpandCollapsePatternId));
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(propertyGridView.IsHandleCreated);
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_IsPatternSupported_IfExpandCollapsePatternAndDropDownEditable_ReturnsTrue()
        {
            using PropertyGrid propertyGrid = new();
            using PropertyGridView propertyGridView = new(serviceProvider: null, propertyGrid);

            TestGridEntry parent = new(propertyGrid, peParent: null, propertyGridView);
            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(typeof(TestEntity)).
                Find(nameof(TestEntity.SizeProperty), ignoreCase: false);

            DropDownEditablePropertyDescriptorGridEntry gridEntry = new(propertyGrid, parent, propertyDescriptor, hide: false);
            AccessibleObject accessibleObject = gridEntry.AccessibilityObject;

            Assert.True(gridEntry.NeedsDropDownButton);
            Assert.True(accessibleObject.IsPatternSupported(UiaCore.UIA.ExpandCollapsePatternId));
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(propertyGridView.IsHandleCreated);
        }

        private class TestGridEntry : GridEntry
        {
            readonly PropertyGridView _propertyGridView;

            public TestGridEntry(PropertyGrid ownerGrid, GridEntry peParent, PropertyGridView propertyGridView)
                : base(ownerGrid, peParent)
            {
                _propertyGridView = propertyGridView;
            }

            internal override PropertyGridView OwnerGridView => _propertyGridView;
        }

        private class TestPropertyDescriptorGridEntry : PropertyDescriptorGridEntry
        {
            private GridEntryCollection _collection;

            public TestPropertyDescriptorGridEntry(PropertyGrid ownerGrid, GridEntry parent, bool hide)
                : base(ownerGrid, parent, hide)
            {
            }

            public override GridEntryCollection Children
            {
                get
                {
                    _collection ??= new GridEntryCollection();

                    return _collection;
                }
            }

            internal override bool Enumerable => false;
        }

        private class TestPropertyGridViewAccessibleObject : PropertyGridView.PropertyGridViewAccessibleObject
        {
            public TestPropertyGridViewAccessibleObject(PropertyGridView owner, PropertyGrid parentPropertyGrid)
                : base(owner, parentPropertyGrid)
            {
            }
        }

        internal class TestEntity
        {
            public Size SizeProperty
            {
                get; set;
            }
        }

        private class EnumerablePropertyDescriptorGridEntry : PropertyDescriptorGridEntry
        {
            public EnumerablePropertyDescriptorGridEntry(PropertyGrid ownerGrid, GridEntry parent, PropertyDescriptor propertyDescriptor, bool hide)
                : base(ownerGrid, parent, propertyDescriptor, hide)
            {
            }

            internal override bool Enumerable => true;
        }

        private class DropDownEditablePropertyDescriptorGridEntry : PropertyDescriptorGridEntry
        {
            public DropDownEditablePropertyDescriptorGridEntry(PropertyGrid ownerGrid, GridEntry parent, PropertyDescriptor propertyDescriptor, bool hide)
                : base(ownerGrid, parent, propertyDescriptor, hide)
            {
            }

            public override bool NeedsDropDownButton => true;
        }
    }
}
