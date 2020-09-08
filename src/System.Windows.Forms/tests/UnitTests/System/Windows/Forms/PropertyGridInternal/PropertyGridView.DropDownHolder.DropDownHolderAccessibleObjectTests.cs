// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Xunit;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class PropertyGridView_DropDownHolder_DropDownHolderAccessibleObjectTests
    {
        [WinFormsFact]
        public void DropDownHolder_AccessibilityObject_Constructor_initializes_fields()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;

            using PropertyGridView.DropDownHolder dropDownHolderControl = new PropertyGridView.DropDownHolder(propertyGridView);
            PropertyGridView.DropDownHolder.DropDownHolderAccessibleObject dropDownHolderControlAccessibilityObject =
                Assert.IsAssignableFrom<PropertyGridView.DropDownHolder.DropDownHolderAccessibleObject>(
                    dropDownHolderControl.AccessibilityObject);

            PropertyGridView.DropDownHolder dropDownHolder =
                dropDownHolderControlAccessibilityObject.TestAccessor().Dynamic._owningDropDownHolder;
            Assert.NotNull(dropDownHolder);
            Assert.Same(dropDownHolder, dropDownHolderControl);
        }

        [WinFormsFact]
        public void DropDownHolder_AccessibilityObject_Constructor_throws_error_if_passed_control_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var dropDownHolderAccessibleObject = new PropertyGridView.DropDownHolder.DropDownHolderAccessibleObject(null);
            });
        }

        [WinFormsFact]
        public void DropDownHolder_AccessibilityObject_ReturnsExpected()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;

            using PropertyGridView.DropDownHolder ownerControl = new PropertyGridView.DropDownHolder(propertyGridView);
            Control.ControlAccessibleObject accessibilityObject = ownerControl.AccessibilityObject as Control.ControlAccessibleObject;

            Assert.NotNull(accessibilityObject.Owner);
            Assert.Equal(ownerControl, accessibilityObject.Owner);

            Assert.Equal(SR.PropertyGridViewDropDownControlHolderAccessibleName,
                accessibilityObject.GetPropertyValue(UiaCore.UIA.NamePropertyId));

            AccessibleObject selectedGridEntryAccessibleObject = null;
            Assert.Equal(selectedGridEntryAccessibleObject, accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));

            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[0];
            PropertyDescriptorGridEntry gridEntry = new PropertyDescriptorGridEntry(propertyGrid, null, property, false);
            propertyGridView.TestAccessor().Dynamic.selectedGridEntry = gridEntry;

            ownerControl.Visible = true;

            selectedGridEntryAccessibleObject = gridEntry.AccessibilityObject;
            Assert.Equal(selectedGridEntryAccessibleObject, accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));

            AccessibleObject editAccessibleObject = propertyGridView.EditAccessibleObject;
            Assert.Equal(editAccessibleObject, accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            Assert.Null(accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));

            Assert.Equal(propertyGrid.AccessibilityObject, accessibilityObject.FragmentRoot);
        }
    }
}
