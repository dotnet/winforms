// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static System.Windows.Forms.ToolStripPanel;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests;

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
        bool value = (bool)accessibleObject.GetPropertyValue(UIA.IsKeyboardFocusablePropertyId);

        Assert.False(value);
        Assert.False(control.IsHandleCreated);
    }
}
