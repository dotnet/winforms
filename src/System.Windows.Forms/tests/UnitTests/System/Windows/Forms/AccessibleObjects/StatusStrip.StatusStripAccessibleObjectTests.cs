// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.Tests.AccessibleObjects.ToolStripAccessibleObjectTests;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class StatusStrip_StatusStripAccessibleObjectTests
{
    [WinFormsFact]
    public void StatusStripAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected()
    {
        using StatusStrip statusStrip = new()
        {
            Name = "Name1",
            AccessibleName = "Test Name"
        };

        AccessibleObject statusStripAccessibleObject = statusStrip.AccessibilityObject;
        var accessibleName = statusStripAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId);

        Assert.Equal("Test Name", ((BSTR)accessibleName).ToStringAndFree());
    }

    [WinFormsFact]
    public void StatusStripAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
    {
        using StatusStrip statusStrip = new();
        AccessibleObject statusStripAccessibleObject = statusStrip.AccessibilityObject;

        bool supportsLegacyIAccessiblePatternId = statusStripAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId);

        Assert.True(supportsLegacyIAccessiblePatternId);
    }

    [WinFormsFact]
    public void StatusStripAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected()
    {
        using StatusStrip statusStrip = new()
        {
            AccessibleRole = AccessibleRole.Link
        };

        AccessibleObject statusStripAccessibleObject = statusStrip.AccessibilityObject;
        var accessibleObjectRole = statusStripAccessibleObject.Role;

        Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
    }

    [WinFormsFact]
    public void StatusStripAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected()
    {
        using StatusStrip statusStrip = new()
        {
            AccessibleDescription = "Test Description"
        };

        AccessibleObject statusStripAccessibleObject = statusStrip.AccessibilityObject;
        string accessibleObjectDescription = statusStripAccessibleObject.Description;

        Assert.Equal("Test Description", accessibleObjectDescription);
    }

    [WinFormsFact]
    public void StatusStripAccessibleObject_ControlType_IsStatusBar_IfAccessibleRoleIsDefault()
    {
        using StatusStrip statusStrip = new();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)statusStrip.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_StatusBarControlTypeId, actual);
        Assert.False(statusStrip.IsHandleCreated);
    }

    [WinFormsFact]
    public void StatusStripAccessibleObject_Role_IsStatusBar_ByDefault()
    {
        using StatusStrip statusStrip = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = statusStrip.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.StatusBar, actual);
        Assert.False(statusStrip.IsHandleCreated);
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(StatusStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void StatusStripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using StatusStrip statusStrip = new();
        statusStrip.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)statusStrip.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(statusStrip.IsHandleCreated);
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleNotCreated_TestData()
    {
        Array directions = Enum.GetValues<NavigateDirection>();

        foreach (NavigateDirection direction in directions)
        {
            yield return new object[] { direction };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleNotCreated_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleNotCreated(int navigateDirection)
    {
        using StatusStrip statusStrip = CreateStatusStrip(itemCount: 1, createControl: false);

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate((NavigateDirection)navigateDirection));
        Assert.False(statusStrip.IsHandleCreated);
    }

    [WinFormsFact]
    public void StatusStripAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using StatusStrip statusStrip = CreateStatusStrip();
        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
    }

    [WinFormsFact]
    public void StatusStripAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using StatusStrip statusStrip = CreateStatusStrip();
        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData()
    {
        Size horizontalSize = new(300, 30);
        Size verticalSize = new(70, 300);

        bool[] canOverflowValues = [true, false];

        foreach (bool canOverflow in canOverflowValues)
        {
            yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalSize, canOverflow };
            yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalSize, canOverflow };
            yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalSize, canOverflow };
            yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalSize, canOverflow };
        }
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData()
    {
        Size horizontalSize = new(300, 30);
        Size verticalSize = new(70, 300);

        bool[] canOverflowValues = [true, false];

        foreach (bool canOverflow in canOverflowValues)
        {
            yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.Table, null, verticalSize, canOverflow };
            yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.Table, null, verticalSize, canOverflow };

            yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.Flow, null, horizontalSize, canOverflow };
            yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.Flow, null, horizontalSize, canOverflow };
        }
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData()
    {
        foreach (object[] data in StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData())
        {
            yield return data;
        }

        foreach (object[] data in StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData())
        {
            yield return data;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfToolStripIsEmpty(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData()
    {
        Size horizontalSize = new(300, 30);
        Size verticalSize = new(70, 300);

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
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfGripVisible(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 1);

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Grip.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfGripVisibleAndToolStripIsEmpty(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Grip.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndOverflow_TestData()
    {
        Size horizontalOverflowSize = new(10, 25);
        Size verticalOverflowSize = new(25, 10);

        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalOverflowSize };
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndOverflow_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfOverflow(ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using StatusStrip statusStrip = CreateStatusStrip(ToolStripGripStyle.Hidden, layout, dock, size, canOverflow: true, itemCount: 1);

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.OverflowButton.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndCannotOverflowAndStackLayout_TestData()
    {
        Size horizontalOverflowSize = new(10, 25);
        Size verticalOverflowSize = new(25, 10);

        yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, null, horizontalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, null, verticalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Top, horizontalOverflowSize };
        yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, DockStyle.Left, verticalOverflowSize };
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndCannotOverflowAndStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfCanNotOverflowAndStackLayout(ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using StatusStrip statusStrip = CreateStatusStrip(ToolStripGripStyle.Hidden, layout, dock, size, canOverflow: false, itemCount: 1);

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndCannotOverflowAndNonStackLayout_TestData()
    {
        Size overflowSize = new(70, 10);

        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.Table, overflowSize };
        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.Table, overflowSize };
        yield return new object[] { ToolStripGripStyle.Hidden, ToolStripLayoutStyle.Flow, overflowSize };
        yield return new object[] { ToolStripGripStyle.Visible, ToolStripLayoutStyle.Flow, overflowSize };
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndCannotOverflowAndNonStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfCanNotOverflowAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, Size size)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock: null, size, canOverflow: false, itemCount: 3);

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 1);

        statusStrip.Items.Insert(0, CreateSkippedItem());

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfAllItemsSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        statusStrip.Items.Add(CreateSkippedItem());
        statusStrip.Items.Add(CreateSkippedItem());

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        statusStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        statusStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemSkippedAndSecondAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        statusStrip.Items.Insert(0, CreateSkippedItem());

        statusStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[2].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfFirstItemSkippedAndSecondAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        statusStrip.Items.Insert(0, CreateSkippedItem());

        statusStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfAllItemsAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        statusStrip.Items[0].Alignment = ToolStripItemAlignment.Right;
        statusStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGripAndNonStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfAllItemsAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        statusStrip.Items[0].Alignment = ToolStripItemAlignment.Right;
        statusStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData()
    {
        Size horizontalSize = new(300, 30);
        Size verticalSize = new(70, 300);

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

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData()
    {
        Size horizontalSize = new(300, 30);
        Size verticalSize = new(140, 300);

        bool[] canOverflowValues = [true, false];

        foreach (ToolStripGripStyle grip in Enum.GetValues<ToolStripGripStyle>())
            foreach (bool canOverflow in canOverflowValues)
            {
                yield return new object[] { grip, ToolStripLayoutStyle.Table, null, verticalSize, canOverflow };
                yield return new object[] { grip, ToolStripLayoutStyle.Flow, null, horizontalSize, canOverflow };
            }
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_TestData()
    {
        foreach (object[] data in StatusStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData())
        {
            yield return data;
        }

        foreach (object[] data in StatusStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData())
        {
            yield return data;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfToolStripIsEmpty(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfToolStripIsEmptyAndGripVisible(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Grip.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfLastItemSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 1);

        statusStrip.Items.Add(CreateSkippedItem());

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNoGrip_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfAllItemsSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        statusStrip.Items.Add(CreateSkippedItem());
        statusStrip.Items.Add(CreateSkippedItem());

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfGripVisible_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfAllItemsSkippedAndGripVisible(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 0);

        statusStrip.Items.Add(CreateSkippedItem());
        statusStrip.Items.Add(CreateSkippedItem());

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Grip.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfFirstItemAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        statusStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfFirstItemAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        statusStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfFirstItemSkippedAndAlignedAndLastSkippedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        statusStrip.Items.Insert(0, CreateSkippedItem());
        statusStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        statusStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        statusStrip.Items.Add(CreateSkippedItem());
        statusStrip.Items[3].Alignment = ToolStripItemAlignment.Right;

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfFirstItemSkippedAndAlignedAndLastSkippedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        statusStrip.Items.Insert(0, CreateSkippedItem());
        statusStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        statusStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        statusStrip.Items.Add(CreateSkippedItem());
        statusStrip.Items[3].Alignment = ToolStripItemAlignment.Right;

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[2].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfAllItemsAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        statusStrip.Items[0].Alignment = ToolStripItemAlignment.Right;
        statusStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfNonStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfAllItemsAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size, bool canOverflow)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow, itemCount: 2);

        statusStrip.Items[0].Alignment = ToolStripItemAlignment.Right;
        statusStrip.Items[1].Alignment = ToolStripItemAlignment.Right;

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfOverflow_TestData()
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
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfOverflow_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfOverflow(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow: true, itemCount: 3);

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.OverflowButton.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfCannotOverflow_TestData()
    {
        Size horizontalOverflowSize = new(70, 30);
        Size horizontalOverflowGripSize = new(80, 30);
        Size verticalOverflowSize = new(80, 55);
        Size verticalOverflowGripSize = new(80, 65);
        Size bothOverflowSize = new(70, 25);

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
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfCannotOverflow_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfCannotOverflow(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow: false, itemCount: 3);

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemSkipped_TestData()
    {
        Size horizontalOverflowSize = new(70, 30);
        Size horizontalOverflowGripSize = new(80, 30);
        Size verticalOverflowSize = new(30, 55);
        Size verticalOverflowGripSize = new(30, 65);
        Size bothOverflowSize = new(70, 30);

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
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemSkipped_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfCannotOverflowAndItemSkipped(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow: false, itemCount: 2);

        statusStrip.Items.Insert(1, CreateSkippedItem());

        statusStrip.PerformLayout();

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemAlignedAndStackLayout_TestData()
    {
        Size horizontalOverflowSize = new(70, 30);
        Size horizontalOverflowGripSize = new(80, 30);
        Size verticalOverflowSize = new(60, 60);
        Size verticalOverflowGripSize = new(70, 70);

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
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemAlignedAndStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfCannotOverflowAndFirstItemAlignedAndStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow: false, itemCount: 3);

        statusStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    public static IEnumerable<object[]> StatusStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemAlignedAndNonStackLayout_TestData()
    {
        Size horizontalOverflowSize = new(115, 60);
        Size verticalOverflowSize = new(70, 60);

        foreach (ToolStripGripStyle grip in Enum.GetValues<ToolStripGripStyle>())
        {
            yield return new object[] { grip, ToolStripLayoutStyle.Table, null, horizontalOverflowSize };
            yield return new object[] { grip, ToolStripLayoutStyle.Flow, null, verticalOverflowSize };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(StatusStripAccessibleObject_FragmentNavigate_IfCannotOverflowAndItemAlignedAndNonStackLayout_TestData))]
    public void StatusStripAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfCannotOverflowAndFirstItemAlignedAndNonStackLayout(ToolStripGripStyle grip, ToolStripLayoutStyle layout, DockStyle? dock, Size size)
    {
        using StatusStrip statusStrip = CreateStatusStrip(grip, layout, dock, size, canOverflow: false, itemCount: 3);

        statusStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        AccessibleObject accessibleObject = statusStrip.AccessibilityObject;
        AccessibleObject expected = statusStrip.Items[1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    private StatusStrip CreateStatusStrip(
       ToolStripGripStyle? gripStyle = null,
       ToolStripLayoutStyle? layoutStyle = null,
       DockStyle? dock = null,
       Size? size = null,
       bool? canOverflow = null,
       int itemCount = 0,
       bool createControl = true
    )
    {
        StatusStrip statusStrip = new();

        if (layoutStyle is not null)
        {
            statusStrip.LayoutStyle = layoutStyle.Value;
        }

        if (gripStyle is not null)
        {
            statusStrip.GripStyle = gripStyle.Value;
        }

        if (dock is not null)
        {
            statusStrip.Dock = dock.Value;
        }

        if (size is not null)
        {
            statusStrip.AutoSize = false;
            statusStrip.Size = size.Value;
        }

        if (canOverflow is not null)
        {
            statusStrip.CanOverflow = canOverflow.Value;
        }

        for (int i = 0; i < itemCount; i++)
        {
            statusStrip.Items.Add(CreateStatusStripItem());
        }

        statusStrip.PerformLayout();

        if (createControl)
        {
            statusStrip.CreateControl();
        }

        return statusStrip;

        static ToolStripItem CreateStatusStripItem()
        {
            return new ToolStripStatusLabel()
            {
                AutoSize = false,
                Size = new Size(50, 25)
            };
        }
    }
}
