// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.PropertyGridInternal.Tests.AccessibleObjects;

public class PropertyGridView_DropDownHolder_DropDownHolderAccessibleObjectTests
{
    [WinFormsFact]
    public void DropDownHolder_AccessibilityObject_Constructor_initializes_fields()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;

        using PropertyGridView.DropDownHolder dropDownHolderControl = new(propertyGridView);
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
        using PropertyGrid propertyGrid = new();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        using PropertyGridView.DropDownHolder ownerControl = new(propertyGridView);
        Control.ControlAccessibleObject accessibilityObject = ownerControl.AccessibilityObject as Control.ControlAccessibleObject;

        Assert.Equal(ownerControl, accessibilityObject.Owner);
        Assert.Equal(SR.PropertyGridViewDropDownControlHolderAccessibleName,
            ((BSTR)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree());

        AccessibleObject selectedGridEntryAccessibleObject = null;
        Assert.Equal(selectedGridEntryAccessibleObject, accessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));

        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[0];
        PropertyDescriptorGridEntry gridEntry = new(propertyGrid, null, property, false);
        propertyGridView.TestAccessor().Dynamic._selectedGridEntry = gridEntry;

        ownerControl.Visible = true;

        selectedGridEntryAccessibleObject = gridEntry.AccessibilityObject;
        Assert.Equal(selectedGridEntryAccessibleObject, accessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));

        Assert.Equal(propertyGridView.AccessibilityObject, accessibilityObject.FragmentRoot);
    }

    [WinFormsFact]
    public void DropDownHolderAccessibleObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        using PropertyGridView.DropDownHolder dropDownControlHolder = new(propertyGridView);
        // AccessibleRole is not set = Default

        VARIANT actual = dropDownControlHolder.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId, (UIA_CONTROLTYPE_ID)(int)actual);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(dropDownControlHolder.IsHandleCreated);
    }
}
