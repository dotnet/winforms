// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.PropertyGridInternal.Tests.AccessibleObjects;

public class CommandsPane_CommandsPaneAccessibleObjectTests
{
    [WinFormsFact]
    public void CommandsPaneAccessibleObject_Ctor_Default()
    {
        using PropertyGrid propertyGrid = new();
        using CommandsPane commandsPane = new(propertyGrid);
        CommandsPane.CommandsPaneAccessibleObject accessibleObject =
            new CommandsPane.CommandsPaneAccessibleObject(commandsPane, propertyGrid);

        Assert.Equal(commandsPane, accessibleObject.Owner);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(commandsPane.IsHandleCreated);
    }

    [WinFormsFact]
    public void CommandsPaneAccessibleObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
    {
        using PropertyGrid propertyGrid = new();
        using CommandsPane commandsPane = new(propertyGrid);
        // AccessibleRole is not set = Default

        UIA_CONTROLTYPE_ID actual = (UIA_CONTROLTYPE_ID)(int)commandsPane.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId, actual);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(commandsPane.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Client)]
    [InlineData(false, AccessibleRole.None)]
    public void CommandsPaneAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
    {
        using PropertyGrid propertyGrid = new();
        using CommandsPane commandsPane = new(propertyGrid);
        // AccessibleRole is not set = Default

        if (createControl)
        {
            commandsPane.CreateControl();
        }

        AccessibleRole actual = commandsPane.AccessibilityObject.Role;

        Assert.Equal(expectedRole, actual);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.Equal(createControl, commandsPane.IsHandleCreated);
    }
}
