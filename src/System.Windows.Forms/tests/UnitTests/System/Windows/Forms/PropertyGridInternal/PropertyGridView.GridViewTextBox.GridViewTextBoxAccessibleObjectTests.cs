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
    public class PropertyGridView_GridViewTextBox_GridViewTextBoxAccessibleObjectTests
    {
        [WinFormsFact]
        public void GridViewTextBoxAccessibleObject_created_for_string_property()
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
            Assert.NotEqual(IntPtr.Zero, propertyGridView.TestAccessor().Dynamic.EditTextBox.Handle);

            AccessibleObject selectedGridEntryAccessibleObject = gridEntry.AccessibilityObject;
            UiaCore.IRawElementProviderFragment editFieldAccessibleObject = selectedGridEntryAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.NotNull(editFieldAccessibleObject);

            Assert.Equal("GridViewTextBoxAccessibleObject", editFieldAccessibleObject.GetType().Name);
        }

        [WinFormsFact]
        public void GridViewTextBoxAccessibleObject_FragmentNavigate_navigates_correctly()
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
            Assert.NotEqual(IntPtr.Zero, propertyGridView.TestAccessor().Dynamic.EditTextBox.Handle);

            UiaCore.IRawElementProviderFragment editFieldAccessibleObject = gridEntry.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            Assert.Equal("GridViewTextBoxAccessibleObject", editFieldAccessibleObject.GetType().Name);

            // The case with drop down holder:
            using PropertyGridView.DropDownHolder dropDownHolder = new(propertyGridView);
            dropDownHolder.CreateControl();
            propertyGridView.TestAccessor().Dynamic._dropDownHolder = dropDownHolder;

            dropDownHolder.TestAccessor().Dynamic.SetState(0x00000002, true); // Control class States.Visible flag
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

        [WinFormsFact]
        public void GridViewTextBoxAccessibleObject_ctor_default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            Type gridViewTextBoxType = typeof(PropertyGridView).GetNestedType("GridViewTextBox", BindingFlags.NonPublic);
            Assert.NotNull(gridViewTextBoxType);
            TextBox gridViewTextBox = (TextBox)Activator.CreateInstance(gridViewTextBoxType, gridView);
            Type accessibleObjectType = gridViewTextBoxType.GetNestedType("GridViewTextBoxAccessibleObject", BindingFlags.NonPublic);
            Assert.NotNull(accessibleObjectType);
            ControlAccessibleObject accessibleObject = (ControlAccessibleObject)Activator.CreateInstance(accessibleObjectType, gridViewTextBox);
            Assert.Equal(gridViewTextBox, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void GridViewTextBoxAccessibleObject_ctor_ThrowsException_IfOwnerIsNull()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            Type gridViewTextBoxType = typeof(PropertyGridView).GetNestedType("GridViewTextBox", BindingFlags.NonPublic);
            Assert.NotNull(gridViewTextBoxType);
            TextBox gridViewTextBox = (TextBox)Activator.CreateInstance(gridViewTextBoxType, gridView);
            Type accessibleObjectType = gridViewTextBoxType.GetNestedType("GridViewTextBoxAccessibleObject", BindingFlags.NonPublic);
            Assert.NotNull(accessibleObjectType);
            Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(accessibleObjectType, (TextBox)null));
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.IsTextPatternAvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsTextPattern2AvailablePropertyId)]
        [InlineData((int)UiaCore.UIA.IsValuePatternAvailablePropertyId)]
        public void GridViewTextBoxAccessibleObject_GetPropertyValue_PatternsSuported(int propertyID)
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
        public void GridViewTextBoxAccessibleObject_IsPatternSupported_PatternsSuported(int patternId)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            AccessibleObject accessibleObject = gridView.EditAccessibleObject;
            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)patternId));
        }

        [WinFormsFact]
        public void GridViewTextBoxAccessibleObject_ControlType_IsEdit_IfAccessibleRoleIsDefault()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            AccessibleObject accessibleObject = gridView.EditAccessibleObject;

            // AccessibleRole is not set = Default

            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.EditControlTypeId, actual);
            Assert.False(propertyGrid.IsHandleCreated);
        }

        [WinFormsFact]
        public void GridViewTextBoxAccessibleObject_GetPropertyValue_FrameworkIdPropertyId_ReturnsExpected()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;
            AccessibleObject accessibleObject = gridView.EditAccessibleObject;

            Assert.Equal(NativeMethods.WinFormFrameworkId, accessibleObject.GetPropertyValue(UiaCore.UIA.FrameworkIdPropertyId));
            Assert.False(propertyGrid.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Text)]
        [InlineData(false, AccessibleRole.None)]
        public void GridViewTextBoxAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView gridView = propertyGrid.TestAccessor().GridView;

            // AccessibleRole is not set = Default

            if (createControl)
            {
                gridView.TestAccessor().Dynamic.EditTextBox.CreateControl(true); // "true" means ignoring Visible value
            }

            AccessibleRole actual = gridView.EditAccessibleObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.False(propertyGrid.IsHandleCreated);
        }

        [WinFormsFact]
        public void GridViewTextBoxAccessibleObject_RuntimeId_ReturnsNull()
        {
            using PropertyGrid propertyGrid = new() { SelectedObject = new TestEntityWithTextField() { TextProperty = "Test" } };

            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
            int firstPropertyIndex = 1; // Index 0 corresponds to the category grid entry.
            PropertyDescriptorGridEntry gridEntry = (PropertyDescriptorGridEntry)propertyGridView.AccessibilityGetGridEntries()[firstPropertyIndex];

            // Force the entry edit control Handle creation.
            // GridViewEditAccessibleObject exists, if its control is already created.
            // In UI case an entry edit control is created when an PropertyGridView gets focus.
            Assert.NotEqual(IntPtr.Zero, propertyGridView.TestAccessor().Dynamic.EditTextBox.Handle);

            AccessibleObject editFieldAccessibleObject = (AccessibleObject)gridEntry.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);
            propertyGridView.TestAccessor().Dynamic._selectedGridEntry = null;

            Assert.NotNull(editFieldAccessibleObject.RuntimeId);
        }
    }
}
