// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.Control;
using static System.Windows.Forms.PropertyGridInternal.PropertyGridView;

namespace System.Windows.Forms.PropertyGridInternal.Tests.AccessibleObjects;

public class PropertyGridView_GridViewListBoxAccessibleObjectTest
{
    [WinFormsFact]
    public void GridViewListBoxAccessibleObject_Ctor_Default()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        using GridViewListBox gridViewListBox = new(propertyGridView);

        Type type = gridViewListBox.AccessibilityObject.GetType();
        ControlAccessibleObject accessibleObject = (ControlAccessibleObject)Activator.CreateInstance(type, gridViewListBox);

        Assert.Equal(gridViewListBox, accessibleObject.Owner);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(gridViewListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void GridViewListBoxAccessibleObject_ControlType_IsList_IfAccessibleRoleIsDefault()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        AccessibleObject accessibleObject = propertyGridView.DropDownListBoxAccessibleObject;
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ListControlTypeId, actual);
        Assert.False(propertyGrid.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.List)]
    [InlineData(false, AccessibleRole.None)]
    public void GridViewListBoxAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
        using GridViewListBox gridViewListBox = new(propertyGridView);
        // AccessibleRole is not set = Default

        if (createControl)
        {
            gridViewListBox.CreateControl();
        }

        AccessibleRole actual = gridViewListBox.AccessibilityObject.Role;

        Assert.Equal(expectedRole, actual);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.Equal(createControl, gridViewListBox.IsHandleCreated);
    }
}
