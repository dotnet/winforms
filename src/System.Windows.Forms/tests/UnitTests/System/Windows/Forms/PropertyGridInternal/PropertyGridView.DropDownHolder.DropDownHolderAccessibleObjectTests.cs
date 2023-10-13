﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Windows.Win32.UI.Accessibility;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests;

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
        using PropertyGrid propertyGrid = new();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        using PropertyGridView.DropDownHolder ownerControl = new(propertyGridView);
        Control.ControlAccessibleObject accessibilityObject = ownerControl.AccessibilityObject as Control.ControlAccessibleObject;

        Assert.Equal(ownerControl, accessibilityObject.Owner);
        Assert.Equal(SR.PropertyGridViewDropDownControlHolderAccessibleName,
            accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId));

        AccessibleObject selectedGridEntryAccessibleObject = null;
        Assert.Equal(selectedGridEntryAccessibleObject, accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));

        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PropertyGrid))[0];
        PropertyDescriptorGridEntry gridEntry = new PropertyDescriptorGridEntry(propertyGrid, null, property, false);
        propertyGridView.TestAccessor().Dynamic._selectedGridEntry = gridEntry;

        ownerControl.Visible = true;

        selectedGridEntryAccessibleObject = gridEntry.AccessibilityObject;
        Assert.Equal(selectedGridEntryAccessibleObject, accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));

        Assert.Equal(propertyGridView.AccessibilityObject, accessibilityObject.FragmentRoot);
    }

    [WinFormsFact]
    public void DropDownHolderAccessibleObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
    {
        using PropertyGrid propertyGrid = new PropertyGrid();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        using PropertyGridView.DropDownHolder dropDownControlHolder = new PropertyGridView.DropDownHolder(propertyGridView);
        // AccessibleRole is not set = Default

        object actual = dropDownControlHolder.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId, actual);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(dropDownControlHolder.IsHandleCreated);
    }
}
