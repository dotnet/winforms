// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class Panel_PanelAccessibleObjectTests
{
    [WinFormsFact]
    public void PanelAccessibleObject_Ctor_NullControl_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("ownerControl", () => new Panel.PanelAccessibleObject(owner: null));
    }

    [WinFormsFact]
    public void PanelAccessibleObject_Ctor_Default()
    {
        using Panel panel = new();
        Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

        Assert.NotNull(panelAccessibleObject.Owner);
        Assert.False(panel.IsHandleCreated);
    }

    [WinFormsFact]
    public void PanelAccessibleObject_Description_ReturnsExpected()
    {
        string expected = "PanelTestDescription";

        using Panel panel = new()
        {
            AccessibleDescription = expected,
        };

        Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

        Assert.Equal(expected, panelAccessibleObject.Description);
        Assert.False(panel.IsHandleCreated);
    }

    [WinFormsFact]
    public void PanelAccessibleObject_Name_ReturnsExpected()
    {
        string expected = "PanelTestName";

        using Panel panel = new()
        {
            AccessibleName = expected
        };

        Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

        Assert.Equal(expected, panelAccessibleObject.Name);
        Assert.False(panel.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_NamePropertyId, "PanelTestName")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_AutomationIdPropertyId, "Panel1")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_ControlTypePropertyId, (int)UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId)] // If AccessibleRole is Default
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId, false)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId, null)]
    public void PanelAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
    {
        using Panel panel = new()
        {
            Name = "Panel1",
            AccessibleName = "PanelTestName"
        };

        Panel.PanelAccessibleObject panelAccessibleObject = new(panel);
        using VARIANT actual = panelAccessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID);
        if (expected is null)
        {
            Assert.Equal(VARIANT.Empty, actual);
        }
        else
        {
            Assert.Equal(expected, actual.ToObject());
        }

        Assert.False(panel.IsHandleCreated);
    }

    public static IEnumerable<object[]> PanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(PanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void PanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using Panel panel = new();
        panel.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)panel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(panel.IsHandleCreated);
    }

    [WinFormsFact]
    public void PanelAccessibleObject_IsPatternSupported_Invoke_ReturnsTrue_ForLegacyIAccessiblePatternId()
    {
        using Panel panel = new();
        Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

        Assert.True(panelAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId));
        Assert.False(panel.IsHandleCreated);
    }

    [WinFormsFact]
    public void PanelAccessibleObject_Bounds_ReturnsEmpty_IfControlIsNotCreated()
    {
        using Panel panel = new();
        Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

        Assert.Equal(Rectangle.Empty, panelAccessibleObject.Bounds);
        Assert.False(panel.IsHandleCreated);
    }

    [WinFormsFact]
    public void PanelAccessibleObject_Bounds_ReturnsExpected()
    {
        Rectangle expected = new(0, 0, 10, 10);

        using Panel panel = new()
        {
            Bounds = expected
        };

        panel.CreateControl();
        Assert.True(panel.IsHandleCreated);

        Panel.PanelAccessibleObject panelAccessibleObject = new(panel);
        Assert.Equal(expected, panel.RectangleToClient(panelAccessibleObject.Bounds));
    }

    [WinFormsFact]
    public void PanelAccessibleObject_FragmentRoot_ReturnsExpected()
    {
        using Panel panel = new();
        Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

        Assert.Equal(panelAccessibleObject, panelAccessibleObject.FragmentRoot);
        Assert.False(panel.IsHandleCreated);
    }

    [WinFormsFact]
    public void PanelAccessibleObject_GetChildCount_ReturnsMinusOne_IfHandleIsNotCreated()
    {
        using Panel panel = new();
        Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

        Assert.False(panel.IsHandleCreated);
        Assert.Equal(-1, panelAccessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void PanelAccessibleObject_GetChildCount_ReturnsZero_IfPanelHasNoControls()
    {
        using Panel panel = new();
        panel.CreateControl();
        Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

        Assert.True(panel.IsHandleCreated);
        Assert.Equal(0, panelAccessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void PanelAccessibleObject_GetChildCount_ReturnsExpected()
    {
        using Panel parentPanel = new();
        parentPanel.CreateControl();

        using Panel childPanel = new();
        childPanel.CreateControl();
        parentPanel.Controls.Add(childPanel);

        Panel.PanelAccessibleObject panelAccessibleObject = new(parentPanel);

        Assert.True(parentPanel.IsHandleCreated);
        Assert.True(childPanel.IsHandleCreated);
        Assert.Equal(1, panelAccessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void PanelAccessibleObject_GetChild_ReturnsNull_IfHandleIsNotCreated()
    {
        using Panel panel = new();
        Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

        Assert.False(panel.IsHandleCreated);
        Assert.Null(panelAccessibleObject.GetChild(0));
    }

    [WinFormsFact]
    public void PanelAccessibleObject_GetChild_ReturnsNull_IfPanelHasNoControls()
    {
        using Panel panel = new();
        panel.CreateControl();
        Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

        Assert.True(panel.IsHandleCreated);
        Assert.Null(panelAccessibleObject.GetChild(0));
    }

    [WinFormsFact]
    public void PanelAccessibleObject_GetChild_ReturnsExpected()
    {
        using Panel parentPanel = new();
        parentPanel.CreateControl();

        using Panel childPanel = new();
        childPanel.CreateControl();
        parentPanel.Controls.Add(childPanel);

        Panel.PanelAccessibleObject panelAccessibleObject = new(parentPanel);

        Assert.True(parentPanel.IsHandleCreated);
        Assert.True(childPanel.IsHandleCreated);
        Assert.Equal(childPanel.AccessibilityObject, panelAccessibleObject.GetChild(0));
    }

    [WinFormsFact]
    public void PanelAccessibleObject_ChildrenButton_ReturnsExpected()
    {
        using Panel panel = new();
        panel.CreateControl();
        using Button buttonFirst = new();
        buttonFirst.CreateControl();
        panel.Controls.Add(buttonFirst);
        using Button buttonLast = new();
        buttonLast.CreateControl();
        panel.Controls.Add(buttonLast);

        Assert.Equal(buttonFirst.AccessibilityObject, panel.AccessibilityObject.Navigate(AccessibleNavigation.FirstChild));
        Assert.Equal(buttonLast.AccessibilityObject, panel.AccessibilityObject.Navigate(AccessibleNavigation.LastChild));
    }

    [WinFormsFact]
    public void PanelAccessibleObject_BoundingRectangle_IsCorrect()
    {
        using Form form = new();
        using Panel panel1 = new();
        using Button button1 = new();

        panel1.AutoScroll = true;
        panel1.Controls.Add(button1);
        panel1.Location = new(50, 50);
        panel1.Size = new(200, 140);

        button1.Location = new(17, 16);
        button1.Size = new(237, 153);

        form.ClientSize = new(300, 200);
        form.Controls.Add(panel1);

        form.Show();

        Rectangle boundingRectangle = panel1.AccessibilityObject.BoundingRectangle;

        int horizontalScrollBarHeight = SystemInformation.HorizontalScrollBarHeight;
        int verticalScrollBarWidth = SystemInformation.VerticalScrollBarWidth;

        Rectangle expected = panel1.RectangleToScreen(panel1.ClientRectangle);

        Assert.Equal(boundingRectangle.Width, expected.Width + verticalScrollBarWidth);
        Assert.Equal(boundingRectangle.Height, expected.Height + horizontalScrollBarHeight);
    }
}
