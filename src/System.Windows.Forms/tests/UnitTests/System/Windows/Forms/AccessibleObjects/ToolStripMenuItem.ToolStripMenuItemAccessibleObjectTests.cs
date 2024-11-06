// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripMenuItem;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripMenuItem_ToolStripMenuItemAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripMenuItemAccessibleObject_Ctor_Default()
    {
        using ToolStripMenuItem toolStripMenuItem = new();
        ToolStripMenuItemAccessibleObject accessibleObject = new(toolStripMenuItem);

        Assert.Equal(toolStripMenuItem, accessibleObject.Owner);
    }

    [WinFormsFact]
    public void ToolStripMenuItemAccessibleObject_InvokePattern_Invoke_NoPopUpDialog()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            callCount++;
        };

        using ToolStripMenuItem item1 = new();
        item1.Click += handler;
        Assert.True(item1.AccessibilityObject.IsPatternSupported(UIA_PATTERN_ID.UIA_InvokePatternId));

        item1.AccessibilityObject.Invoke();
        callCount.Should().Be(1);
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/10244")]
    [WinFormsFact(Skip = "https://github.com/dotnet/winforms/issues/10244")]
    [SkipOnArchitecture(TestArchitectures.X86 | TestArchitectures.X64,
        "InvokePattern.Invoke blocks on a menu item")]
    public void ToolStripMenuItemAccessibleObject_InvokePattern_Invoke_WithPopUpDialog()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            MessageBox.Show("TestDialog");
            callCount++;
        };

        using ToolStripMenuItem item1 = new();
        item1.Click += handler;
        item1.AccessibilityObject.Invoke();

        foreach (Form form in Application.OpenForms)
        {
            if (form.Text == "TestDialog")
            {
                form.Close();
                break;
            }
        }

        callCount.Should().Be(1);
    }

    [WinFormsFact]
    public void ToolStripMenuItemAccessibleObject_ControlType_IsMenuItem_IfAccessibleRoleIsDefault()
    {
        using ToolStripMenuItem toolStripMenuItem = new();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)toolStripMenuItem.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_MenuItemControlTypeId, actual);
    }

    [WinFormsFact]
    public void ToolStripMenuItemAccessibleObject_Role_IsMenuItem_ByDefault()
    {
        using ToolStripMenuItem toolStripMenuItem = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = toolStripMenuItem.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.MenuItem, actual);
    }

    public static IEnumerable<object[]> ToolStripMenuItemAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
    {
        Array roles = Enum.GetValues(typeof(AccessibleRole));

        foreach (AccessibleRole role in roles)
        {
            if (role == AccessibleRole.Default)
            {
                continue; // The test checks custom roles
            }

            yield return new object[] { role };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripMenuItemAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripMenuItemAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripMenuItem toolStripMenuItem = new();
        toolStripMenuItem.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)toolStripMenuItem.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
    }

    [WinFormsFact]
    public void ToolStripMenuItemAccessibleObject_GetPropertyValue_PositionInSet_ReturnsExpected_IfNoParent()
    {
        using ToolStripMenuItem toolStripMenuItem = new();

        AccessibleObject accessibilityObject = toolStripMenuItem.AccessibilityObject;

        Assert.Equal(VARIANT.Empty, accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_PositionInSetPropertyId));
    }

    [WinFormsFact]
    public void ToolStripMenuItemAccessibleObject_GetPropertyValue_PositionInSet_ReturnsExpected()
    {
        using MenuStrip menuStrip = new();

        using ToolStripMenuItem item1 = new();
        menuStrip.Items.Add(item1);
        menuStrip.PerformLayout();

        Assert.Single(menuStrip.Items);
        Assert.Equal(1, (int)item1.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_PositionInSetPropertyId));

        using ToolStripSeparator separator = new();
        menuStrip.Items.Add(separator);
        menuStrip.PerformLayout();

        Assert.Equal(2, menuStrip.Items.Count);
        Assert.Equal(1, (int)item1.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_PositionInSetPropertyId));
        Assert.Equal(VARIANT.Empty, separator.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_PositionInSetPropertyId));

        using ToolStripMenuItem item2 = new();
        menuStrip.Items.Add(item2);
        menuStrip.PerformLayout();

        Assert.Equal(3, menuStrip.Items.Count);
        Assert.Equal(1, (int)item1.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_PositionInSetPropertyId));
        Assert.Equal(VARIANT.Empty, separator.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_PositionInSetPropertyId));
        Assert.Equal(2, (int)item2.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_PositionInSetPropertyId));
    }

    [WinFormsFact]
    public void ToolStripMenuItemAccessibleObject_GetPropertyValue_SizeOfSet_ReturnsExpected_IfNoParent()
    {
        using ToolStripMenuItem toolStripMenuItem = new();

        AccessibleObject accessibilityObject = toolStripMenuItem.AccessibilityObject;

        Assert.Equal(VARIANT.Empty, accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_SizeOfSetPropertyId));
    }

    [WinFormsFact]
    public void ToolStripMenuItemAccessibleObject_GetPropertyValue_SizeOfSet_ReturnsExpected()
    {
        using MenuStrip menuStrip = new();

        using ToolStripMenuItem item1 = new();
        menuStrip.Items.Add(item1);
        menuStrip.PerformLayout();

        Assert.Single(menuStrip.Items);
        Assert.Equal(1, (int)item1.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_SizeOfSetPropertyId));

        using ToolStripSeparator separator = new();
        menuStrip.Items.Add(separator);
        menuStrip.PerformLayout();

        Assert.Equal(2, menuStrip.Items.Count);
        Assert.Equal(1, (int)item1.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_SizeOfSetPropertyId));
        Assert.Equal(VARIANT.Empty, separator.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_SizeOfSetPropertyId));

        using ToolStripMenuItem item2 = new();
        menuStrip.Items.Add(item2);
        menuStrip.PerformLayout();

        Assert.Equal(3, menuStrip.Items.Count);
        Assert.Equal(2, (int)item1.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_SizeOfSetPropertyId));
        Assert.Equal(VARIANT.Empty, separator.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_SizeOfSetPropertyId));
        Assert.Equal(2, (int)item2.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_SizeOfSetPropertyId));
    }

    [WinFormsTheory]
    [InlineData(true, CheckState.Checked, true)]
    [InlineData(true, CheckState.Unchecked, true)]
    [InlineData(true, CheckState.Indeterminate, true)]
    [InlineData(false, CheckState.Checked, true)]
    [InlineData(false, CheckState.Unchecked, false)]
    [InlineData(false, CheckState.Indeterminate, true)]
    public void ToolStripMenuItemAccessibleObject_IsTogglePatternSupported_ReturnsExpected(bool checkOnClick, CheckState checkState, bool expected)
    {
        using ToolStripMenuItem toolStripMenuItem = new()
        {
            CheckOnClick = checkOnClick,
            CheckState = checkState
        };

        object actual = toolStripMenuItem.AccessibilityObject.IsPatternSupported(UIA_PATTERN_ID.UIA_TogglePatternId);

        Assert.Equal(expected, actual);
    }

    [WinFormsTheory]
    [InlineData(CheckState.Checked, (int)ToggleState.ToggleState_On)]
    [InlineData(CheckState.Unchecked, (int)ToggleState.ToggleState_Off)]
    [InlineData(CheckState.Indeterminate, (int)ToggleState.ToggleState_Indeterminate)]
    public void ToolStripMenuItemAccessibleObject_ToggleState_ReturnsExpected(CheckState checkState, int expectedToggleState)
    {
        using ToolStripMenuItem toolStripMenuItem = new()
        {
            CheckState = checkState
        };

        object actual = toolStripMenuItem.AccessibilityObject.ToggleState;

        Assert.Equal((ToggleState)expectedToggleState, actual);
    }

    [WinFormsFact]
    public void ToolStripMenuItemAccessibleObject_Toggle_Invoke()
    {
        using ToolStripMenuItem toolStripMenuItem = new()
        {
            CheckOnClick = true
        };

        int clickCounter = 0;

        toolStripMenuItem.Click += (s, e) => { clickCounter++; };

        Assert.Equal(ToggleState.ToggleState_Off, toolStripMenuItem.AccessibilityObject.ToggleState);

        toolStripMenuItem.AccessibilityObject.Toggle();

        Assert.Equal(ToggleState.ToggleState_On, toolStripMenuItem.AccessibilityObject.ToggleState);

        toolStripMenuItem.AccessibilityObject.Toggle();

        Assert.Equal(ToggleState.ToggleState_Off, toolStripMenuItem.AccessibilityObject.ToggleState);

        Assert.Equal(0, clickCounter);
    }
}
