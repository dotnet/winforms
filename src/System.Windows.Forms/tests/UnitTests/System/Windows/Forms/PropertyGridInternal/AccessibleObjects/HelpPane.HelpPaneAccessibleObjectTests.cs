// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.PropertyGridInternal.Tests.AccessibleObjects;

public class HelpPane_HelpPaneAccessibleObjectTests
{
    [WinFormsFact]
    public void HelpPaneAccessibleObject_Ctor_Default()
    {
        using PropertyGrid propertyGrid = new();
        using HelpPane helpPane = new(propertyGrid);
        HelpPane.HelpPaneAccessibleObject accessibleObject =
            new HelpPane.HelpPaneAccessibleObject(helpPane, propertyGrid);

        Assert.Equal(helpPane, accessibleObject.Owner);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(helpPane.IsHandleCreated);
    }

    [WinFormsFact]
    public void HelpPaneAccessibleObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
    {
        using PropertyGrid propertyGrid = new();
        using HelpPane helpPane = new(propertyGrid);
        // AccessibleRole is not set = Default

        UIA_CONTROLTYPE_ID actual = (UIA_CONTROLTYPE_ID)(int)helpPane.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId, actual);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(helpPane.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Client)]
    [InlineData(false, AccessibleRole.None)]
    public void HelpPaneAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
    {
        using PropertyGrid propertyGrid = new();
        using HelpPane helpPane = new(propertyGrid);
        // AccessibleRole is not set = Default

        if (createControl)
        {
            helpPane.CreateControl();
        }

        AccessibleRole actual = helpPane.AccessibilityObject.Role;

        Assert.Equal(expectedRole, actual);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.Equal(createControl, helpPane.IsHandleCreated);
    }
}
