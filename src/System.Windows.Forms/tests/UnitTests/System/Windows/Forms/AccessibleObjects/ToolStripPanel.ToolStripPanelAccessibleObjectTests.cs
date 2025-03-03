// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripPanel;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripPanel_ToolStripPanelAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripPanelAccessibleObject_Ctor_Default()
    {
        using ToolStripPanel control = new();
        var accessibleObject = (ToolStripPanelAccessibleObject)control.AccessibilityObject;

        Assert.Equal(control, accessibleObject.Owner);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripPanelAccessibleObject_GetPropertyValue_IsKeyboardFocusable_False()
    {
        using ToolStripPanel control = new();

        var accessibleObject = (ToolStripPanelAccessibleObject)control.AccessibilityObject;
        bool value = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId);

        Assert.False(value);
        Assert.False(control.IsHandleCreated);
    }
}
