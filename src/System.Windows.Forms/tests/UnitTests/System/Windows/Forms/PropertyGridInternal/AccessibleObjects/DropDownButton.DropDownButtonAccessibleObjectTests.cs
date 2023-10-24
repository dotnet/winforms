// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests.AccessibleObjects;

public class DropDownButton_DropDownButtonAccessibleObjectTests
{
    [WinFormsFact]
    public void DropDownButtonAccessibleObject_Ctor_Default()
    {
        using DropDownButton dropDownButton = new DropDownButton();
        DropDownButton.DropDownButtonAccessibleObject accessibleObject =
            new DropDownButton.DropDownButtonAccessibleObject(dropDownButton);

        Assert.Equal(dropDownButton, accessibleObject.Owner);
        Assert.False(dropDownButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void DropDownButtonAccessibleObject_ControlType_IsButton_IfAccessibleRoleIsDefault()
    {
        using DropDownButton dropDownButton = new DropDownButton();
        // AccessibleRole is not set = Default

        object actual = dropDownButton.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId, actual);
        Assert.False(dropDownButton.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId, true)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleRolePropertyId, AccessibleRole.PushButton)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_ValueValuePropertyId, null)]
    public void DomainUpDownAccessibleObject_GetPropertyValue_ReturnsExpected(int property, object expected)
    {
        using DropDownButton dropDownButton = new DropDownButton();
        AccessibleObject accessibleObject = dropDownButton.AccessibilityObject;
        object actual = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)property);

        Assert.Equal(expected, actual);
        Assert.False(dropDownButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void DropDownButtonAccessibleObject_Role_IsPushButton_ByDefault()
    {
        using DropDownButton dropDownButton = new DropDownButton();
        // AccessibleRole is not set = Default

        AccessibleRole actual = dropDownButton.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.PushButton, actual);
        Assert.False(dropDownButton.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UiaCore.NavigateDirection.FirstChild)]
    [InlineData((int)UiaCore.NavigateDirection.LastChild)]
    public void DropDownButtonAccessibleObject_FragmentNavigate_ChildrenAreNull(int direction)
    {
        using DropDownButton dropDownButton = new();

        object actual = dropDownButton.AccessibilityObject.FragmentNavigate((UiaCore.NavigateDirection)direction);

        Assert.Null(actual);
        Assert.False(dropDownButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void DropDownButtonAccessibleObject_FragmentNavigate_ParentIsGridEntry()
    {
        using PropertyGrid control = new();
        using Button button = new();
        control.SelectedObject = button;
        control.SelectedGridItem = control.GetCurrentEntries()[1].GridItems[5]; // FlatStyle property

        PropertyGridView gridView = control.TestAccessor().GridView;
        DropDownButton dropDownButton = gridView.DropDownButton;
        dropDownButton.Visible = true;

        object actual = dropDownButton.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent);

        Assert.Equal(gridView.SelectedGridEntry.AccessibilityObject, actual);
        Assert.False(control.IsHandleCreated);
        Assert.True(dropDownButton.IsHandleCreated); // Setting Visible property forces Handle creation
    }
}
