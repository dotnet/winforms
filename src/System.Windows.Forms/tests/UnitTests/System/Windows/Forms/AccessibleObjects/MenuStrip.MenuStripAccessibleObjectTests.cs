// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.Tests.AccessibleObjects.ToolStripAccessibleObjectTests;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class MenuStrip_MenuStripAccessibleObjectTests
{
    [WinFormsFact]
    public void MenuStripAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected()
    {
        using MenuStrip menuStrip = new()
        {
            Name = "Name1",
            AccessibleName = "Test Name"
        };

        AccessibleObject menuStripAccessibleObject = menuStrip.AccessibilityObject;
        var accessibleName = menuStripAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId);

        Assert.Equal("Test Name", ((BSTR)accessibleName).ToStringAndFree());
    }

    [WinFormsFact]
    public void MenuStripAccessibleObject_GetPropertyValue_IsControlElement_ReturnsExpected()
    {
        using MenuStrip menuStrip = new();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsControlElementPropertyId));
    }

    [WinFormsFact]
    public void MenuStripAccessibleObject_GetPropertyValue_IsContentElement_ReturnsExpected()
    {
        using MenuStrip menuStrip = new();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;

        Assert.False((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsContentElementPropertyId));
    }

    [WinFormsFact]
    public void MenuStripAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
    {
        using MenuStrip menuStrip = new();
        AccessibleObject menuStripAccessibleObject = menuStrip.AccessibilityObject;

        bool supportsLegacyIAccessiblePatternId = menuStripAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId);

        Assert.True(supportsLegacyIAccessiblePatternId);
    }

    [WinFormsFact]
    public void MenuStripAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected()
    {
        using MenuStrip menuStrip = new()
        {
            AccessibleRole = AccessibleRole.Link
        };

        AccessibleObject menuStripAccessibleObject = menuStrip.AccessibilityObject;
        var accessibleObjectRole = menuStripAccessibleObject.Role;

        Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
    }

    [WinFormsFact]
    public void MenuStripAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected()
    {
        using MenuStrip menuStrip = new()
        {
            AccessibleDescription = "Test Description"
        };

        AccessibleObject menuStripAccessibleObject = menuStrip.AccessibilityObject;
        string accessibleObjectDescription = menuStripAccessibleObject.Description;

        Assert.Equal("Test Description", accessibleObjectDescription);
    }

    [WinFormsFact]
    public void MenuStripAccessibleObject_ControlType_IsMenuBar_IfAccessibleRoleIsDefault()
    {
        using MenuStrip menuStrip = new();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)menuStrip.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_MenuBarControlTypeId, actual);
        Assert.False(menuStrip.IsHandleCreated);
    }

    [WinFormsFact]
    public void MenuStripAccessibleObject_Role_IsMenuBar_ByDefault()
    {
        using MenuStrip menuStrip = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = menuStrip.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.MenuBar, actual);
        Assert.False(menuStrip.IsHandleCreated);
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(MenuStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void MenuStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using MenuStrip menuStrip = new();
        menuStrip.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)menuStrip.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(menuStrip.IsHandleCreated);
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleNotCreated_TestData()
    {
        Array directions = Enum.GetValues<NavigateDirection>();

        foreach (NavigateDirection direction in directions)
        {
            yield return new object[] { direction };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleNotCreated_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleNotCreated(int navigateDirection)
    {
        using MenuStrip menuStrip = CreateMenuStrip(itemCount: 1, createControl: false);

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate((NavigateDirection)navigateDirection));
        Assert.False(menuStrip.IsHandleCreated);
    }

    [WinFormsFact]
    public void MenuStripAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using MenuStrip menuStrip = CreateMenuStrip();
        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
    }

    [WinFormsFact]
    public void MenuStripAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using MenuStrip menuStrip = CreateMenuStrip();
        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData()
    {
        Size horizontalSize = new(300, 30);
        Size verticalSize = new(30, 300);

        bool[] canOverflowValues = [true, false];

        foreach (bool canOverflow in canOverflowValues)
        {
            yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalSize, canOverflow };
            yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalSize, canOverflow };
            yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalSize, canOverflow };
            yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalSize, canOverflow };
        }
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData()
    {
        Size horizontalSize = new(300, 30);
        Size verticalSize = new(30, 300);

        bool[] canOverflowValues = [true, false];

        foreach (bool canOverflow in canOverflowValues)
        {
            yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.Table, null, verticalSize, canOverflow };
            yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.Table, null, verticalSize, canOverflow };

            yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.Flow, null, horizontalSize, canOverflow };
            yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.Flow, null, horizontalSize, canOverflow };
        }
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData()
    {
        foreach (object[] data in MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData())
        {
            yield return data;
        }

        foreach (object[] data in MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData())
        {
            yield return data;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfToolStripIsEmpty(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData()
    {
        Size horizontalSize = new(300, 30);
        Size verticalSize = new(30, 300);

        bool[] canOverflowValues = [true, false];

        foreach (bool canOverflow in canOverflowValues)
        {
            yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalSize, canOverflow };
            yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalSize, canOverflow };
            yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalSize, canOverflow };
            yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalSize, canOverflow };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfGripVisible(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 1);

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Grip.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfGripVisibleAndToolStripIsEmpty(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Grip.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndOverflow_TestData()
    {
        Size horizontalOverflowSize = new(10, 25);
        Size verticalOverflowSize = new(25, 10);

        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalOverflowSize };
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndOverflow_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfOverflow(ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using MenuStrip menuStrip = CreateMenuStrip(ToolStripGripStyle.Hidden, layout, dock, size, canOverflow: true, itemCount: 1);

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.OverflowButton.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndCannotOverflowAndStackLayout_TestData()
    {
        Size horizontalOverflowSize = new(10, 25);
        Size verticalOverflowSize = new(25, 10);

        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalOverflowSize };
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndCannotOverflowAndStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfCanNotOverflowAndStackLayout(ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using MenuStrip menuStrip = CreateMenuStrip(ToolStripGripStyle.Hidden, layout, dock, size, canOverflow: false, itemCount: 3);

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndCannotOverflowAndNonStackLayout_TestData()
    {
        Size overflowSize = new(10, 10);

        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.Table, overflowSize };
        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.Table, overflowSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.Flow, overflowSize };
        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.Flow, overflowSize };
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndCannotOverflowAndNonStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfCanNotOverflowAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, Size size)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock: null, size, canOverflow: false, itemCount: 3);

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 1);

        menuStrip.Items.Insert(0, CreateSkippedItem());

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfAllItemsSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        menuStrip.Items.Add(CreateSkippedItem());
        menuStrip.Items.Add(CreateSkippedItem());

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        menuStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        menuStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemSkippedAndSecondAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        menuStrip.Items.Insert(0, CreateSkippedItem());

        menuStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[2].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemSkippedAndSecondAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        menuStrip.Items.Insert(0, CreateSkippedItem());

        menuStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfAllItemsAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        menuStrip.Items[0].Alignment = ToolStripItemAlignment.Right;
        menuStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfAllItemsAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        menuStrip.Items[0].Alignment = ToolStripItemAlignment.Right;
        menuStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData()
    {
        Size horizontalSize = new(300, 30);
        Size verticalSize = new(30, 300);

        bool[] canOverflowValues = [true, false];

        foreach (ToolStripGripStyle grip in Enum.GetValues<ToolStripGripStyle>())
            foreach (bool canOverflow in canOverflowValues)
            {
                yield return new object[] { grip, ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalSize, canOverflow };
                yield return new object[] { grip, ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalSize, canOverflow };
                yield return new object[] { grip, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalSize, canOverflow };
                yield return new object[] { grip, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalSize, canOverflow };
            }
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData()
    {
        Size horizontalSize = new(300, 30);
        Size verticalSize = new(30, 300);

        bool[] canOverflowValues = [true, false];

        foreach (ToolStripGripStyle grip in Enum.GetValues<ToolStripGripStyle>())
            foreach (bool canOverflow in canOverflowValues)
            {
                yield return new object[] { grip, ToolStripLayoutStyle.Table, null, verticalSize, canOverflow };
                yield return new object[] { grip, ToolStripLayoutStyle.Flow, null, horizontalSize, canOverflow };
            }
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_TestData()
    {
        foreach (object[] data in MenuStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData())
        {
            yield return data;
        }

        foreach (object[] data in MenuStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData())
        {
            yield return data;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfToolStripIsEmpty(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfToolStripIsEmptyAndGripVisible(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Grip.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfLastItemSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 1);

        menuStrip.Items.Add(CreateSkippedItem());

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfAllItemsSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        menuStrip.Items.Add(CreateSkippedItem());
        menuStrip.Items.Add(CreateSkippedItem());

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfAllItemsSkippedAndGripVisible(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        menuStrip.Items.Add(CreateSkippedItem());
        menuStrip.Items.Add(CreateSkippedItem());

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Grip.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfFirstItemAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        menuStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfFirstItemAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        menuStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfFirstItemSkippedAndAlignedAndLastSkippedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        menuStrip.Items.Insert(0, CreateSkippedItem());
        menuStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        menuStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        menuStrip.Items.Add(CreateSkippedItem());
        menuStrip.Items[3].Alignment = ToolStripItemAlignment.Right;

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfFirstItemSkippedAndAlignedAndLastSkippedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        menuStrip.Items.Insert(0, CreateSkippedItem());
        menuStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        menuStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        menuStrip.Items.Add(CreateSkippedItem());
        menuStrip.Items[3].Alignment = ToolStripItemAlignment.Right;

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[2].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfAllItemsAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        menuStrip.Items[0].Alignment = ToolStripItemAlignment.Right;
        menuStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfAllItemsAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        menuStrip.Items[0].Alignment = ToolStripItemAlignment.Right;
        menuStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfOverflow_TestData()
    {
        Size horizontalOverflowSize = new(10, 25);
        Size verticalOverflowSize = new(25, 10);

        foreach (ToolStripGripStyle grip in Enum.GetValues<ToolStripGripStyle>())
        {
            yield return new object[] { grip, ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalOverflowSize };
            yield return new object[] { grip, ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalOverflowSize };
            yield return new object[] { grip, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalOverflowSize };
            yield return new object[] { grip, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalOverflowSize };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfOverflow_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfOverflow(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow: true, itemCount: 3);

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.OverflowButton.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfCannotOverflow_TestData()
    {
        Size horizontalOverflowSize = new(60, 30);
        Size horizontalOverflowGripSize = new(70, 30);
        Size verticalOverflowSize = new(30, 30);
        Size verticalOverflowGripSize = new(30, 40);
        Size bothOverflowSize = new(25, 25);

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalOverflowGripSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalOverflowSize };

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalOverflowGripSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalOverflowSize };

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalOverflowGripSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalOverflowSize };

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalOverflowGripSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalOverflowSize };

        foreach (ToolStripGripStyle grip in Enum.GetValues<ToolStripGripStyle>())
        {
            yield return new object[] { grip, ToolStripLayoutStyle.Table, null, bothOverflowSize };
            yield return new object[] { grip, ToolStripLayoutStyle.Flow, null, bothOverflowSize };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfCannotOverflow_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfCannotOverflow(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow: false, itemCount: 3);

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemSkipped_TestData()
    {
        Size horizontalOverflowSize = new(60, 30);
        Size horizontalOverflowGripSize = new(70, 30);
        Size verticalOverflowSize = new(30, 55);
        Size verticalOverflowGripSize = new(30, 65);
        Size bothOverflowSize = new(28, 28);

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalOverflowGripSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalOverflowSize };

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalOverflowGripSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalOverflowSize };

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalOverflowGripSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalOverflowSize };

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalOverflowGripSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalOverflowSize };

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.Table, null, bothOverflowSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.Table, null, bothOverflowSize };

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.Flow, null, bothOverflowSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.Flow, null, bothOverflowSize };
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemSkipped_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfCannotOverflowAndItemSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow: false, itemCount: 2);

        menuStrip.Items.Insert(1, CreateSkippedItem());

        menuStrip.PerformLayout();

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemAlignedAndStackLayout_TestData()
    {
        Size horizontalOverflowSize = new(60, 30);
        Size horizontalOverflowGripSize = new(70, 30);
        Size verticalOverflowSize = new(30, 70);
        Size verticalOverflowGripSize = new(30, 60);

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalOverflowGripSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalOverflowSize };

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalOverflowGripSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalOverflowSize };

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalOverflowGripSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalOverflowSize };

        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalOverflowGripSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalOverflowSize };
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemAlignedAndStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfCannotOverflowAndFirstItemAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow: false, itemCount: 3);

        menuStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> MenuStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemAlignedAndNonStackLayout_TestData()
    {
        Size overflowSize = new(30, 40);

        foreach (ToolStripGripStyle grip in Enum.GetValues<ToolStripGripStyle>())
        {
            yield return new object[] { grip, ToolStripLayoutStyle.Table, null, overflowSize };
            yield return new object[] { grip, ToolStripLayoutStyle.Flow, null, overflowSize };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MenuStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemAlignedAndNonStackLayout_TestData))]
    public void MenuStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfCannotOverflowAndFirstItemAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using MenuStrip menuStrip = CreateMenuStrip(grip, layout, dock, size, canOverflow: false, itemCount: 3);

        menuStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        AccessibleObject accessibleObject = menuStrip.AccessibilityObject;
        AccessibleObject expected = menuStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    private MenuStrip CreateMenuStrip(
       ToolStripGripStyle? gripStyle = null,
       ToolStripLayoutStyle? layoutStyle = null,
       DockStyle? dock = null,
       Size? size = null,
       bool? canOverflow = null,
       int itemCount = 0,
       bool createControl = true
    )
    {
        MenuStrip menuStrip = new();

        if (layoutStyle is not null)
        {
            menuStrip.LayoutStyle = layoutStyle.Value;
        }

        if (gripStyle is not null)
        {
            menuStrip.GripStyle = gripStyle.Value;
        }

        if (dock is not null)
        {
            menuStrip.Dock = dock.Value;
        }

        if (size is not null)
        {
            menuStrip.AutoSize = false;
            menuStrip.Size = size.Value;
        }

        if (canOverflow is not null)
        {
            menuStrip.CanOverflow = canOverflow.Value;
        }

        for (int i = 0; i < itemCount; i++)
        {
            menuStrip.Items.Add(CreateMenuItem());
        }

        menuStrip.PerformLayout();

        if (createControl)
        {
            menuStrip.CreateControl();
        }

        return menuStrip;

        static ToolStripItem CreateMenuItem()
        {
            return new ToolStripMenuItem()
            {
                AutoSize = false,
                Size = new Size(50, 25),
                Overflow = ToolStripItemOverflow.AsNeeded
            };
        }
    }
}
