// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripDropDownButton;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripDropDownButton_ToolStripDropDownButtonAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripDropDownButtonAccessibleObject_Ctor_Default()
    {
        using ToolStripDropDownButton toolStripDropDownButton = new();
        ToolStripDropDownButtonAccessibleObject accessibleObject = new(toolStripDropDownButton);

        Assert.Equal(toolStripDropDownButton, accessibleObject.Owner);
    }

    [WinFormsFact]
    public void ToolStripDropDownButtonAccessibleObject_ControlType_IsButton_IfAccessibleRoleIsDefault()
    {
        using ToolStripDropDownButton toolStripDropDownButton = new();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)toolStripDropDownButton.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId, actual);
    }

    [WinFormsFact]
    public void ToolStripDropDownButtonAccessibleObject_Role_IsMenuItem_ByDefault()
    {
        using ToolStripDropDownButton toolStripDropDownButton = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = toolStripDropDownButton.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.MenuItem, actual);
    }

    public static IEnumerable<object[]> ToolStripDropDownButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripDropDownButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripDropDownButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripDropDownButton toolStripDropDownButton = new();
        toolStripDropDownButton.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)toolStripDropDownButton.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
    }

    [WinFormsFact]
    public void ToolStripDropDownButtonAccessibleObject_FragmentNavigate_Child_ReturnExpected()
    {
        using ToolStrip toolStrip = new();

        using ToolStripDropDownButton dropDownItem = new();
        dropDownItem.DropDownItems.Add(string.Empty);

        toolStrip.Items.Add(dropDownItem);

        toolStrip.CreateControl();
        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.Items[0].AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        dropDownItem.DropDown.Show();

        AccessibleObject expected = dropDownItem.DropDown.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsFact]
    public void ToolStripDropDownButton_ConstructorParameters_ShouldInitializeCorrectly()
    {
        string text = "Test text";
        using Bitmap image = new(10, 10);
        bool wasClicked = false;
        EventHandler onClick = (sender, e) => { wasClicked = true; };
        string name = "Test name";
        ToolStripItem[] dropDownItems = [new ToolStripMenuItem("Test item")];

        static ToolStripDropDownButton CreateToolStripDropDownButton(string text = null, Bitmap image = null, EventHandler onClick = null, string name = null, ToolStripItem[] dropDownItems = null)
        {
            ToolStripDropDownButton toolStripDropDownButton = new(text, image, dropDownItems);
            toolStripDropDownButton.Click += onClick;
            toolStripDropDownButton.Name = name;

            return toolStripDropDownButton;
        }

        var toolStripDropDownButton = CreateToolStripDropDownButton(text, image, onClick, name, dropDownItems);
        toolStripDropDownButton.PerformClick();

        toolStripDropDownButton.Text.Should().Be(text);
        toolStripDropDownButton.Image.Should().Be(image);
        wasClicked.Should().BeTrue();
        toolStripDropDownButton.Name.Should().Be(name);
        toolStripDropDownButton.DropDownItems.Should().BeEquivalentTo(dropDownItems);
    }
}
