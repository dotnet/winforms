// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripContainer_ToolStripContainerAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripContainerAccessibleObject_Ctor_Default()
    {
        using ToolStripContainer toolStripContainer = new();
        ToolStripContainer.ToolStripContainerAccessibleObject accessibleObject = new(toolStripContainer);

        Assert.Equal(toolStripContainer, accessibleObject.Owner);
        Assert.False(toolStripContainer.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_NamePropertyId, "TestName")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_AutomationIdPropertyId, "ToolStripContainer1")]
    public void ToolStripContainerAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, string expected)
    {
        using ToolStripContainer control = new()
        {
            Name = "ToolStripContainer1",
            AccessibleName = "TestName"
        };

        var accessibleObject = new ToolStripContainer.ToolStripContainerAccessibleObject(control);
        string value = ((BSTR)accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID)).ToStringAndFree();

        Assert.Equal(expected, value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripContainerAccessibleObject_GetPropertyValue_HasKeyboardFocus_ReturnsTrue_IfControlHasFocus()
    {
        using ToolStripContainer control = new();

        var accessibleObject = new ToolStripContainer.ToolStripContainerAccessibleObject(control);
        Assert.False(control.IsHandleCreated);
        control.FocusActiveControlInternal();
        bool value = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);

        Assert.True(value);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripContainerAccessibleObject_GetPropertyValue_HasKeyboardFocus_ReturnsFalse_IfControlHasNoFocus()
    {
        using ToolStripContainer control = new();

        var accessibleObject = new ToolStripContainer.ToolStripContainerAccessibleObject(control);
        bool value = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);

        Assert.False(value);
        Assert.False(control.IsHandleCreated);
    }
}
