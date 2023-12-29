// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.PropertyGridInternal.Tests.AccessibleObjects;

public class DropDownButton_DropDownButtonAccessibleObjectTests
{
    [WinFormsFact]
    public void DropDownButtonAccessibleObject_Ctor_Default()
    {
        using DropDownButton dropDownButton = new();
        DropDownButton.DropDownButtonAccessibleObject accessibleObject =
            new DropDownButton.DropDownButtonAccessibleObject(dropDownButton);

        Assert.Equal(dropDownButton, accessibleObject.Owner);
        Assert.False(dropDownButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void DropDownButtonAccessibleObject_ControlType_IsButton_IfAccessibleRoleIsDefault()
    {
        using DropDownButton dropDownButton = new();
        // AccessibleRole is not set = Default

        UIA_CONTROLTYPE_ID actual = (UIA_CONTROLTYPE_ID)(int)dropDownButton.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId, actual);
        Assert.False(dropDownButton.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId, true)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleRolePropertyId, (int)AccessibleRole.PushButton)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_ValueValuePropertyId, null)]
    public void DomainUpDownAccessibleObject_GetPropertyValue_ReturnsExpected(int property, object expected)
    {
        using DropDownButton dropDownButton = new();
        AccessibleObject accessibleObject = dropDownButton.AccessibilityObject;
        VARIANT actual = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)property);
        if (expected is null)
        {
            Assert.Equal(VARIANT.Empty, actual);
        }
        else
        {
            Assert.Equal(expected, actual.ToObject());
        }

        Assert.False(dropDownButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void DropDownButtonAccessibleObject_Role_IsPushButton_ByDefault()
    {
        using DropDownButton dropDownButton = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = dropDownButton.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.PushButton, actual);
        Assert.False(dropDownButton.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)NavigateDirection.NavigateDirection_FirstChild)]
    [InlineData((int)NavigateDirection.NavigateDirection_LastChild)]
    public void DropDownButtonAccessibleObject_FragmentNavigate_ChildrenAreNull(int direction)
    {
        using DropDownButton dropDownButton = new();

        object actual = dropDownButton.AccessibilityObject.FragmentNavigate((NavigateDirection)direction);

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

        object actual = dropDownButton.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);

        Assert.Equal(gridView.SelectedGridEntry.AccessibilityObject, actual);
        Assert.False(control.IsHandleCreated);
        Assert.True(dropDownButton.IsHandleCreated); // Setting Visible property forces Handle creation
    }
}
