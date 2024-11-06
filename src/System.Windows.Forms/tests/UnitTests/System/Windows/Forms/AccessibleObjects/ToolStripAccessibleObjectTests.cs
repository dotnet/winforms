// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripAccessibleObject_Ctor_Default()
    {
        using ToolStrip toolStrip = new();

        var accessibleObject = new ToolStrip.ToolStripAccessibleObject(toolStrip);
        Assert.NotNull(accessibleObject.Owner);
        Assert.Equal(AccessibleRole.ToolBar, accessibleObject.Role);
    }

    [WinFormsFact]
    public void ToolStripAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected()
    {
        using ToolStrip toolStrip = new()
        {
            Name = "Name1",
            AccessibleName = "Test Name"
        };

        AccessibleObject toolStripAccessibleObject = toolStrip.AccessibilityObject;
        var accessibleName = toolStripAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId);

        Assert.Equal("Test Name", ((BSTR)accessibleName).ToStringAndFree());
    }

    [WinFormsFact]
    public void ToolStripAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
    {
        using ToolStrip toolStrip = new();
        AccessibleObject toolStripAccessibleObject = toolStrip.AccessibilityObject;

        bool supportsLegacyIAccessiblePatternId = toolStripAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId);

        Assert.True(supportsLegacyIAccessiblePatternId);
    }

    [WinFormsFact]
    public void ToolStripAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected()
    {
        using ToolStrip toolStrip = new()
        {
            AccessibleRole = AccessibleRole.Link
        };

        AccessibleObject toolStripAccessibleObject = toolStrip.AccessibilityObject;
        var accessibleObjectRole = toolStripAccessibleObject.Role;

        Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
    }

    [WinFormsFact]
    public void ToolStripAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected()
    {
        using ToolStrip toolStrip = new()
        {
            AccessibleDescription = "Test Description"
        };

        AccessibleObject toolStripAccessibleObject = toolStrip.AccessibilityObject;
        string accessibleObjectDescription = toolStripAccessibleObject.Description;

        Assert.Equal("Test Description", accessibleObjectDescription);
    }

    [WinFormsFact]
    public void ToolStripAccessibleObject_ControlType_IsToolBar_IfAccessibleRoleIsDefault()
    {
        using ToolStrip toolStrip = new();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)toolStrip.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ToolBarControlTypeId, actual);
        Assert.False(toolStrip.IsHandleCreated);
    }

    public static IEnumerable<object[]> ToolStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStrip toolStrip = new();
        toolStrip.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)toolStrip.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(toolStrip.IsHandleCreated);
    }

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleNotCreated_TestData()
    {
        Array directions = Enum.GetValues<NavigateDirection>();

        foreach (NavigateDirection direction in directions)
        {
            yield return new object[] { direction };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleNotCreated_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleNotCreated(int navigateDirection)
    {
        using ToolStrip toolStrip = CreateToolStrip(itemCount: 1, createControl: false);

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate((NavigateDirection)navigateDirection));
        Assert.False(toolStrip.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using ToolStrip toolStrip = CreateToolStrip();
        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
    }

    [WinFormsFact]
    public void ToolStripAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using ToolStrip toolStrip = CreateToolStrip();
        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public unsafe void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ThumbButton()
    {
        using ToolStrip toolStrip = new();
        toolStrip.CreateControl();

        var accessibleObject = toolStrip.AccessibilityObject;

        IRawElementProviderFragment.Interface firstChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);
        Assert.NotNull(firstChild);
        using VARIANT actual = default;
        Assert.True(((IRawElementProviderSimple.Interface)firstChild).GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId, &actual).Succeeded);
        Assert.Equal((int)UIA_CONTROLTYPE_ID.UIA_ThumbControlTypeId, actual.ToObject());
    }

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData()
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

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData()
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

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData()
    {
        foreach (object[] data in ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData())
        {
            yield return data;
        }

        foreach (object[] data in ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData())
        {
            yield return data;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfToolStripIsEmpty(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData()
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
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfGripVisible(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 1);

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Grip.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfGripVisibleAndToolStripIsEmpty(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Grip.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndOverflow_TestData()
    {
        Size horizontalOverflowSize = new(10, 25);
        Size verticalOverflowSize = new(25, 10);

        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalOverflowSize };
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndOverflow_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfOverflow(ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using ToolStrip toolStrip = CreateToolStrip(ToolStripGripStyle.Hidden, layout, dock, size, canOverflow: true, itemCount: 1);

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.OverflowButton.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndCannotOverflowAndStackLayout_TestData()
    {
        Size horizontalOverflowSize = new(10, 25);
        Size verticalOverflowSize = new(25, 10);

        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalOverflowSize };
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndCannotOverflowAndStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfCanNotOverflowAndStackLayout(ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using ToolStrip toolStrip = CreateToolStrip(ToolStripGripStyle.Hidden, layout, dock, size, canOverflow: false, itemCount: 3);

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndCannotOverflowAndNonStackLayout_TestData()
    {
        Size overflowSize = new(10, 10);

        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.Table, overflowSize };
        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.Table, overflowSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.Flow, overflowSize };
        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.Flow, overflowSize };
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndCannotOverflowAndNonStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfCanNotOverflowAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, Size size)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock: null, size, canOverflow: false, itemCount: 3);

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 1);

        toolStrip.Items.Insert(0, CreateSkippedItem());

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfAllItemsSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        toolStrip.Items.Add(CreateSkippedItem());
        toolStrip.Items.Add(CreateSkippedItem());

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemSkippedAndSecondAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        toolStrip.Items.Insert(0, CreateSkippedItem());

        toolStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[2].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemSkippedAndSecondAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        toolStrip.Items.Insert(0, CreateSkippedItem());

        toolStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfAllItemsAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;
        toolStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfAllItemsAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;
        toolStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData()
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

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData()
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

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_TestData()
    {
        foreach (object[] data in ToolStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData())
        {
            yield return data;
        }

        foreach (object[] data in ToolStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData())
        {
            yield return data;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfToolStripIsEmpty(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfToolStripIsEmptyAndGripVisible(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Grip.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfLastItemSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 1);

        toolStrip.Items.Add(CreateSkippedItem());

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfAllItemsSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        toolStrip.Items.Add(CreateSkippedItem());
        toolStrip.Items.Add(CreateSkippedItem());

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfAllItemsSkippedAndGripVisible(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        toolStrip.Items.Add(CreateSkippedItem());
        toolStrip.Items.Add(CreateSkippedItem());

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Grip.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfFirstItemAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfFirstItemAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfFirstItemSkippedAndAlignedAndLastSkippedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        toolStrip.Items.Insert(0, CreateSkippedItem());
        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        toolStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        toolStrip.Items.Add(CreateSkippedItem());
        toolStrip.Items[3].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfFirstItemSkippedAndAlignedAndLastSkippedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        toolStrip.Items.Insert(0, CreateSkippedItem());
        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        toolStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        toolStrip.Items.Add(CreateSkippedItem());
        toolStrip.Items[3].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[2].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfAllItemsAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;
        toolStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfAllItemsAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;
        toolStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfOverflow_TestData()
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
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfOverflow_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfOverflow(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow: true, itemCount: 3);

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.OverflowButton.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfCannotOverflow_TestData()
    {
        Size horizontalOverflowSize = new(30, 30);
        Size horizontalOverflowGripSize = new(40, 30);
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
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfCannotOverflow_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfCannotOverflow(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow: false, itemCount: 3);

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemSkipped_TestData()
    {
        Size horizontalOverflowSize = new(40, 30);
        Size horizontalOverflowGripSize = new(50, 30);
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
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemSkipped_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfCannotOverflowAndItemSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow: false, itemCount: 2);

        toolStrip.Items.Insert(1, CreateSkippedItem());

        toolStrip.PerformLayout();

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemAlignedAndStackLayout_TestData()
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
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemAlignedAndStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfCannotOverflowAndFirstItemAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow: false, itemCount: 3);

        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> ToolStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemAlignedAndNonStackLayout_TestData()
    {
        Size overflowSize = new(30, 40);

        foreach (ToolStripGripStyle grip in Enum.GetValues<ToolStripGripStyle>())
        {
            yield return new object[] { grip, ToolStripLayoutStyle.Table, null, overflowSize };
            yield return new object[] { grip, ToolStripLayoutStyle.Flow, null, overflowSize };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemAlignedAndNonStackLayout_TestData))]
    public void ToolStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfCannotOverflowAndFirstItemAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using ToolStrip toolStrip = CreateToolStrip(grip, layout, dock, size, canOverflow: false, itemCount: 3);

        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        AccessibleObject accessibleObject = toolStrip.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    internal static ToolStripItem CreateSkippedItem()
    {
        Label emptyLabel = new();

        return new ToolStripControlHost(emptyLabel);
    }

    internal static ToolStripLayoutStyle[] GetStackToolStripLayoutStyles()
    {
        return
        [
            ToolStripLayoutStyle.HorizontalStackWithOverflow,
            ToolStripLayoutStyle.VerticalStackWithOverflow,
            ToolStripLayoutStyle.StackWithOverflow
        ];
    }

    internal static ToolStripLayoutStyle[] GetNonStackToolStripLayoutStyles()
    {
        return
        [
            ToolStripLayoutStyle.Table,
            ToolStripLayoutStyle.Flow
        ];
    }

    internal static IEnumerable<(ToolStripLayoutStyle, ToolStripGripStyle)> GetLayoutsWithGripVisible()
    {
        foreach (ToolStripLayoutStyle layout in GetStackToolStripLayoutStyles())
        {
            yield return (layout, ToolStripGripStyle.Visible);
        }
    }

    internal static IEnumerable<(ToolStripLayoutStyle, ToolStripGripStyle)> GetLayoutsWithGripNotVisible()
    {
        foreach (ToolStripLayoutStyle layout in GetStackToolStripLayoutStyles())
        {
            yield return (layout, ToolStripGripStyle.Hidden);
        }

        foreach (ToolStripLayoutStyle layout in GetNonStackToolStripLayoutStyles())
        {
            yield return (layout, ToolStripGripStyle.Visible);
            yield return (layout, ToolStripGripStyle.Hidden);
        }
    }

    private ToolStrip CreateToolStrip(
        ToolStripGripStyle? gripStyle = null,
        ToolStripLayoutStyle? layoutStyle = null,
        DockStyle? dock = null,
        Size? size = null,
        bool? canOverflow = null,
        int itemCount = 0,
        bool createControl = true
    )
    {
        ToolStrip toolStrip = new();

        if (layoutStyle is not null)
        {
            toolStrip.LayoutStyle = layoutStyle.Value;
        }

        if (gripStyle is not null)
        {
            toolStrip.GripStyle = gripStyle.Value;
        }

        if (dock is not null)
        {
            toolStrip.Dock = dock.Value;
        }

        if (size is not null)
        {
            toolStrip.AutoSize = false;
            toolStrip.Size = size.Value;
        }

        if (canOverflow is not null)
        {
            toolStrip.CanOverflow = canOverflow.Value;
        }

        for (int i = 0; i < itemCount; i++)
        {
            toolStrip.Items.Add(CreateToolStripItem());
        }

        toolStrip.PerformLayout();

        if (createControl)
        {
            toolStrip.CreateControl();
        }

        return toolStrip;

        static ToolStripItem CreateToolStripItem()
        {
            return new ToolStripButton()
            {
                AutoSize = false,
                Size = new Size(25, 25)
            };
        }
    }
}
