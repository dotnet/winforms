// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Reflection;
using Xunit;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class PropertyDescriptorGridEntryAccessibleObjectTests
    {
        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_Ctor_Default()
        {
            using var propertyGrid = new PropertyGrid();
            var propertyDescriptorGridEntryTestEntity = new PropertyDescriptorGridEntryTestEntity(propertyGrid, null, false);
            var propertyDescriptorGridEntryAccessibleObject = propertyDescriptorGridEntryTestEntity.TestPropertyDescriptorGridEntryAccessibleObject;

            Assert.NotNull(propertyDescriptorGridEntryAccessibleObject);

            TypeInfo propertyDescriptorGridEntryAccessibleObjectTypeInfo = propertyDescriptorGridEntryAccessibleObject.GetType().GetTypeInfo();
            FieldInfo owningPropertyDescriptorGridEntryField = propertyDescriptorGridEntryAccessibleObjectTypeInfo.GetDeclaredField("_owningPropertyDescriptorGridEntry");
            var owningGridEntry = owningPropertyDescriptorGridEntryField.GetValue(propertyDescriptorGridEntryAccessibleObject);

            Assert.Equal(propertyDescriptorGridEntryTestEntity, owningGridEntry);
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_ExpandCollapseState_collapsed_by_default()
        {
            using var propertyGrid = new PropertyGrid();
            var propertyDescriptorGridEntryTestEntity = new PropertyDescriptorGridEntryTestEntity(propertyGrid, null, false);
            var propertyDescriptorGridEntryAccessibleObject = propertyDescriptorGridEntryTestEntity.TestPropertyDescriptorGridEntryAccessibleObject;

            var expandCollapseState = propertyDescriptorGridEntryAccessibleObject.ExpandCollapseState;
            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, expandCollapseState);
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_ExpandCollapseState_reflects_ExpandablePropertyState()
        {
            using Form form = new Form();
            using PropertyGrid propertyGrid = new PropertyGrid();
            var testEntity = new TestEntity();
            testEntity.FontProperty = new Font(FontFamily.GenericSansSerif, 1);
            propertyGrid.SelectedObject = testEntity;

            form.Controls.Add(propertyGrid);

            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;

            int firstPropertyIndex = 1; // Index 0 corresponds to the category grid entry.
            PropertyDescriptorGridEntry gridEntry =
                (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[firstPropertyIndex];

            var selectedGridEntry = propertyGridView.TestAccessor().SelectedGridEntry;
            Assert.Equal(gridEntry.PropertyName, selectedGridEntry.PropertyName);

            AccessibleObject selectedGridEntryAccessibleObject = gridEntry.AccessibilityObject;

            gridEntry.InternalExpanded = false;
            Assert.Equal(UiaCore.ExpandCollapseState.Collapsed, selectedGridEntryAccessibleObject.ExpandCollapseState);

            gridEntry.InternalExpanded = true;
            Assert.Equal(UiaCore.ExpandCollapseState.Expanded, selectedGridEntryAccessibleObject.ExpandCollapseState);
        }

        [WinFormsFact]
        public void PropertyDescriptorGridEntryAccessibleObject_Navigates_to_ListBoxAccessibleObject()
        {
            using var form = new Form();
            using var propertyGrid = new PropertyGrid();
            using var button = new Button();

            propertyGrid.SelectedObject = button;
            form.Controls.Add(propertyGrid);
            form.Controls.Add(button);

            using PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView as PropertyGridView;

            int thirdPropertyIndex = 3; // Index of AccessibleRole property which has a ListBox as editor.
            PropertyDescriptorGridEntry gridEntry = (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[thirdPropertyIndex];

            propertyGridView.TestAccessor().SelectedGridEntry = gridEntry;

            using TestDropDownHolder dropDownHolder = new TestDropDownHolder(propertyGridView, propertyGridView.DropDownListBox);
            propertyGridView.TestAccessor().Dynamic.dropDownHolder = dropDownHolder;
            dropDownHolder.SetState(0x00000002, true);

            var listboxFieldAccessibleObject = gridEntry.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.Equal("GridViewListBoxAccessibleObject", listboxFieldAccessibleObject.GetType().Name);
        }

        private class PropertyDescriptorGridEntryTestEntity : PropertyDescriptorGridEntry
        {
            private PropertyDescriptorGridEntryAccessibleObject _accessibleObject { get; set; }

            public PropertyDescriptorGridEntryTestEntity(PropertyGrid ownerGrid, GridEntry peParent, bool hide)
                : base(ownerGrid, peParent, hide)
            {
                _accessibleObject = new PropertyDescriptorGridEntryAccessibleObject(this);
            }

            public GridEntryAccessibleObject TestPropertyDescriptorGridEntryAccessibleObject
            {
                get
                {
                    return _accessibleObject;
                }
            }
        }

        private class TestEntity
        {
            public Font FontProperty
            {
                get; set;
            }
        }

        private class TestDropDownHolder : PropertyGridView.DropDownHolder
        {
            private readonly Control _component;

            public TestDropDownHolder(PropertyGridView propertyGridView, Control control)
                : base(propertyGridView)
            {
                _component = control;
            }

            public override Control Component => _component;

            public void SetState(int flag, bool value)
            {
                base.SetState((States)flag, value);
            }
        }
    }
}
