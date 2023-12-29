// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class PropertyGridToolStripButtonTests
{
    [WinFormsFact]
    public void PropertyGridToolStripButton_AccessibilityObject_ReturnsPropertyGridToolStripButtonAccessibleObject()
    {
        using PropertyGrid propertyGrid = new();
        ToolStripButton[] toolStripButtons = propertyGrid.TestAccessor().Dynamic._viewSortButtons;

        Assert.IsType<PropertyGridToolStripButton.PropertyGridToolStripButtonAccessibleObject>(toolStripButtons[0].AccessibilityObject);
        Assert.IsType<PropertyGridToolStripButton.PropertyGridToolStripButtonAccessibleObject>(toolStripButtons[1].AccessibilityObject);
    }
}
