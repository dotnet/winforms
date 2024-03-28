// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.PropertyGridInternal.Tests.AccessibleObjects;

public class DetailsButton_DetailsButtonAccessibleObjectTests
{
    [WinFormsFact]
    public void DetailsButtonAccessibleObject_Ctor_Default()
    {
        using PropertyGrid propertyGrid = new();
        using GridErrorDialog gridErrorDlg = new(propertyGrid);
        using DetailsButton detailsButton = new(gridErrorDlg);
        DetailsButton.DetailsButtonAccessibleObject accessibleObject = new(detailsButton);

        Assert.Equal(detailsButton, accessibleObject.Owner);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(gridErrorDlg.IsHandleCreated);
        Assert.False(detailsButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void DetailsButtonAccessibleObject_ControlType_IsButton_IfAccessibleRoleIsDefault()
    {
        using PropertyGrid propertyGrid = new();
        using GridErrorDialog gridErrorDlg = new(propertyGrid);
        using DetailsButton detailsButton = new(gridErrorDlg);
        // AccessibleRole is not set = Default

        UIA_CONTROLTYPE_ID actual = (UIA_CONTROLTYPE_ID)(int)detailsButton.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId, actual);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(gridErrorDlg.IsHandleCreated);
        Assert.False(detailsButton.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.PushButton)]
    [InlineData(false, AccessibleRole.None)]
    public void DetailsButtonAccessibleObject_Role_IsPushButton_ByDefault(bool createControl, AccessibleRole expectedRole)
    {
        using PropertyGrid propertyGrid = new();
        using GridErrorDialog gridErrorDlg = new(propertyGrid);
        using DetailsButton detailsButton = new(gridErrorDlg);
        // AccessibleRole is not set = Default

        if (createControl)
        {
            detailsButton.CreateControl();
        }

        AccessibleRole actual = detailsButton.AccessibilityObject.Role;

        Assert.Equal(expectedRole, actual);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(gridErrorDlg.IsHandleCreated);
        Assert.Equal(createControl, detailsButton.IsHandleCreated);
    }
}
