// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class PropertyGridToolStrip_PropertyGridToolStripAccessibleObjectTests
{
    [WinFormsFact]
    public void PropertyGridToolStripAccessibleObject_Ctor_Default()
    {
        using PropertyGrid propertyGrid = new PropertyGrid();
        using PropertyGridToolStrip propertyGridToolStrip = new PropertyGridToolStrip(propertyGrid);
        PropertyGridToolStrip.PropertyGridToolStripAccessibleObject accessibleObject =
            new PropertyGridToolStrip.PropertyGridToolStripAccessibleObject(propertyGridToolStrip, propertyGrid);

        Assert.Equal(propertyGridToolStrip, accessibleObject.Owner);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(propertyGridToolStrip.IsHandleCreated);
    }

    [WinFormsFact]
    public void PropertyGridToolStripAccessibleObject_ControlType_IsToolBar_IfAccessibleRoleIsDefault()
    {
        using PropertyGrid propertyGrid = new PropertyGrid();
        using PropertyGridToolStrip propertyGridToolStrip = new PropertyGridToolStrip(propertyGrid);
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)propertyGridToolStrip.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ToolBarControlTypeId, actual);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(propertyGridToolStrip.IsHandleCreated);
    }

    [WinFormsFact]
    public void PropertyGridToolStripAccessibleObject_Role_IsToolBar_ByDefault()
    {
        using PropertyGrid propertyGrid = new PropertyGrid();
        using PropertyGridToolStrip propertyGridToolStrip = new PropertyGridToolStrip(propertyGrid);
        // AccessibleRole is not set = Default

        AccessibleRole actual = propertyGridToolStrip.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.ToolBar, actual);
        Assert.False(propertyGrid.IsHandleCreated);
        Assert.False(propertyGridToolStrip.IsHandleCreated);
    }
}
