// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class FlowLayoutPanelAccessibilityObjectTests
{
    [WinFormsFact]
    public void FlowLayoutPanelAccessibilityObject_Ctor_Default()
    {
        using FlowLayoutPanel flowLayoutPanel = new();
        flowLayoutPanel.CreateControl();

        Assert.NotNull(flowLayoutPanel.AccessibilityObject);
        Assert.True(flowLayoutPanel.IsHandleCreated);
    }

    [WinFormsFact]
    public void FlowLayoutPanelAccessibilityObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
    {
        using FlowLayoutPanel flowLayoutPanel = new();
        flowLayoutPanel.CreateControl();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)flowLayoutPanel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId, actual);
        Assert.True(flowLayoutPanel.IsHandleCreated);
    }

    [WinFormsFact]
    public void FlowLayoutPanelAccessibilityObject_Role_IsClient_ByDefault()
    {
        using FlowLayoutPanel flowLayoutPanel = new();
        flowLayoutPanel.CreateControl();
        // AccessibleRole is not set = Default

        AccessibleRole actual = flowLayoutPanel.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.Client, actual);
        Assert.True(flowLayoutPanel.IsHandleCreated);
    }
}
