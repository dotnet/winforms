// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripTextBox_ToolStripTextBoxAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripTextBoxAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected()
    {
        using ToolStripTextBox toolStripTextBox = new()
        {
            Name = "Name1",
            AccessibleName = "Test Name"
        };

        AccessibleObject toolStripTextBoxAccessibleObject = toolStripTextBox.AccessibilityObject;
        var accessibleName = toolStripTextBoxAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId);

        Assert.Equal("Test Name", ((BSTR)accessibleName).ToStringAndFree());
    }

    [WinFormsFact]
    public void ToolStripTextBoxAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
    {
        using ToolStripTextBox toolStripTextBox = new();
        AccessibleObject toolStripTextBoxAccessibleObject = toolStripTextBox.AccessibilityObject;

        bool supportsLegacyIAccessiblePatternId = toolStripTextBoxAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId);

        Assert.True(supportsLegacyIAccessiblePatternId);
    }

    [WinFormsFact]
    public void ToolStripTextBoxAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected()
    {
        using ToolStripTextBox toolStripTextBox = new()
        {
            AccessibleRole = AccessibleRole.Link
        };

        AccessibleObject toolStripTextBoxAccessibleObject = toolStripTextBox.AccessibilityObject;
        var accessibleObjectRole = toolStripTextBoxAccessibleObject.Role;

        Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
    }

    [WinFormsFact]
    public void ToolStripTextBoxAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected()
    {
        using ToolStripTextBox toolStripTextBox = new()
        {
            AccessibleDescription = "Test Description"
        };

        AccessibleObject toolStripTextBoxAccessibleObject = toolStripTextBox.AccessibilityObject;
        string accessibleObjectDescription = toolStripTextBoxAccessibleObject.Description;

        Assert.Equal("Test Description", accessibleObjectDescription);
    }
}
