// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Reflection;
using Xunit;
using static System.Windows.Forms.Control;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class PropertyGridView_GridViewEdit_GridViewEditAccessibleObjectTests
    {
        [WinFormsFact]
        public void GridViewEditAccessibleObject_created_for_string_property()
        {
            TestEntityWithTextField testEntity = new TestEntityWithTextField
            {
                TextProperty = "Test"
            };

            using PropertyGrid propertyGrid = new PropertyGrid
            {
                SelectedObject = testEntity
            };

            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
            int firstPropertyIndex = 1; // Index 0 corresponds to the category grid entry.
            PropertyDescriptorGridEntry gridEntry = (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[firstPropertyIndex];
            PropertyDescriptorGridEntry selectedGridEntry = propertyGridView.TestAccessor().Dynamic._selectedGridEntry;

            Assert.Equal(gridEntry.PropertyName, selectedGridEntry.PropertyName);
            // Force the entry edit control Handle creation.
            // GridViewEditAccessibleObject exists, if its control is already created.
            // In UI case an entry edit control is created when an PropertyGridView gets focus.
            Assert.NotEqual(IntPtr.Zero, propertyGridView.TestAccessor().Dynamic.Edit.Handle);

            AccessibleObject selectedGridEntryAccessibleObject = gridEntry.AccessibilityObject;
            UiaCore.IRawElementProviderFragment editFieldAccessibleObject = selectedGridEntryAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.NotNull(editFieldAccessibleObject);

            Assert.Equal("GridViewEditAccessibleObject", editFieldAccessibleObject.GetType().Name);
        }

        [WinFormsFact]
        public void GridViewEditAccessibleObject_FragmentNavigate_navigates_correctly()
        {
            using PropertyGrid propertyGrid = new PropertyGrid
            {
                SelectedObject = Point.Empty
            };
            propertyGrid.CreateControl();
            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;

            int firstPropertyIndex = 2; // Index of Text property which has a RichEdit control as an editor.
            PropertyDescriptorGridEntry gridEntry = (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[firstPropertyIndex];

            propertyGridView.TestAccessor().Dynamic._selectedGridEntry = gridEntry;

            // Force the entry edit control Handle creation.
            // GridViewEditAccessibleObject exists, if its control is already created.
            // In UI case an entry edit control is created when an PropertyGridView gets focus.
            Assert.NotEqual(IntPtr.Zero, propertyGridView.TestAccessor().Dynamic.Edit.Handle);

            UiaCore.IRawElementProviderFragment editFieldAccessibleObject = gridEntry.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.Equal("GridViewEditAccessibleObject", editFieldAccessibleObject.GetType().Name);

            // The case with drop down holder:
            using TestDropDownHolder dropDownHolder = new TestDropDownHolder(propertyGridView);
            dropDownHolder.CreateControl();
            propertyGridView.TestAccessor().Dynamic._dropDownHolder = dropDownHolder;

            dropDownHolder.SetState(0x00000002, true); // Control class States.Visible flag
            UiaCore.IRawElementProviderFragment dropDownHolderAccessibleObject = gridEntry.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);

            Assert.Equal("DropDownHolderAccessibleObject", dropDownHolderAccessibleObject.GetType().Name);
            Assert.True(propertyGridView.DropDownVisible);
            object previousAccessibleObject = editFieldAccessibleObject.Navigate(UiaCore.NavigateDirection.PreviousSibling);
            Assert.NotNull(previousAccessibleObject);
            Assert.Same(dropDownHolder.AccessibilityObject, previousAccessibleObject);
        }

        public class TestEntityWithTextField
        {
            public string TextProperty { get; set; }
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

        [WinFormsFact]
        public void GridViewEditAccessibleObject_ctor_default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            Type gridViewEditType = typeof(PropertyGridView).GetNestedType("GridViewEdit", BindingFlags.NonPublic);
            Assert.NotNull(gridViewEditType);
            TextBox gridViewEdit = (TextBox)Activator.CreateInstance(gridViewEditType, gridView);
            Type accessibleObjectType = gridViewEditType.GetNestedType("GridViewEditAccessibleObject", BindingFlags.NonPublic);
            Assert.NotNull(accessibleObjectType);
            ControlAccessibleObject accessibleObject = (ControlAccessibleObject)Activator.CreateInstance(accessibleObjectType, gridViewEdit);
            Assert.Equal(gridViewEdit, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void GridViewEditAccessibleObject_ctor_ThrowsException_IfOwnerIsNull()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            Type gridViewEditType = typeof(PropertyGridView).GetNestedType("GridViewEdit", BindingFlags.NonPublic);
            Assert.NotNull(gridViewEditType);
            TextBox gridViewEdit = (TextBox)Activator.CreateInstance(gridViewEditType, gridView);
            Type accessibleObjectType = gridViewEditType.GetNestedType("GridViewEditAccessibleObject", BindingFlags.NonPublic);
            Assert.NotNull(accessibleObjectType);
            Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(accessibleObjectType, (TextBox)null));
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.IsTextPatternAvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsTextPattern2AvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsValuePatternAvailablePropertyId)]
        public void GridViewEditAccessibleObject_GetPropertyValue_PatternsSuported(int propertyID)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            AccessibleObject accessibleObject = gridView.EditAccessibleObject;
            Assert.True((bool)accessibleObject.GetPropertyValue((UiaCore.UIA)propertyID));
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.ValuePatternId)]
        [InlineData((int)UiaCore.UIA.TextPatternId)]
        [InlineData((int)UiaCore.UIA.TextPattern2Id)]
        public void GridViewEditAccessibleObject_IsPatternSupported_PatternsSuported(int patternId)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            AccessibleObject accessibleObject = gridView.EditAccessibleObject;
            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)patternId));
        }

        [WinFormsFact]
        public void GridViewEditAccessibleObject_ControlType_IsEdit_IfAccessibleRoleIsDefault()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            AccessibleObject accessibleObject = gridView.EditAccessibleObject;
            // AccessibleRole is not set = Default

            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.EditControlTypeId, actual);
            Assert.False(propertyGrid.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Text)]
        [InlineData(false, AccessibleRole.None)]
        public void GridViewEditAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            // AccessibleRole is not set = Default

            if (createControl)
            {
                gridView.TestAccessor().Dynamic.Edit.CreateControl(true); // "true" means ignoring Visible value
            }

            AccessibleRole actual = gridView.EditAccessibleObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.False(propertyGrid.IsHandleCreated);
        }

        [WinFormsFact]
        public void GridViewTextBoxAccessibleObject_FragmentRoot_ReturnsExpected()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            AccessibleObject accessibleObject = gridView.EditAccessibleObject;

            Assert.Equal(propertyGrid.AccessibilityObject, accessibleObject.FragmentRoot);
        }
    }
}
