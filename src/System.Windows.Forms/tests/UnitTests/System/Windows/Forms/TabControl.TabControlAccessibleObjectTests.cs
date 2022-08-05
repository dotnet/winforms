﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static Interop;
using static Interop.UiaCore;
using static System.Windows.Forms.TabControl;

namespace System.Windows.Forms.Tests
{
    public class TabControl_TabControlAccessibilityObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TabControlAccessibilityObject_Ctor_Default()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();

            Assert.NotNull(tabControl.AccessibilityObject);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibilityObject_ControlType_IsTabControl_IfAccessibleRoleIsDefault()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            // AccessibleRole is not set = Default

            object actual = tabControl.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.TabControlTypeId, actual);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibilityObject_Role_IsPageTabList_ByDefault()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            // AccessibleRole is not set = Default

            AccessibleRole actual = tabControl.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.PageTabList, actual);
            Assert.True(tabControl.IsHandleCreated);
        }

        public static IEnumerable<object[]> TabControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(TabControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void TabControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using TabControl tabControl = new();
            tabControl.AccessibleRole = role;

            object actual = tabControl.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void TabControlAccessibleObject_Bounds_ReturnsExpected(bool createControl, bool boundsIsEmpty)
        {
            using TabControl tabControl = new();
            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            if (createControl)
            {
                tabControl.CreateControl();
            }

            Assert.Equal(boundsIsEmpty, accessibleObject.Bounds.IsEmpty);
            Assert.Equal(createControl, tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, AccessibleStates.Focusable)]
        [InlineData(true, false, AccessibleStates.Focusable | AccessibleStates.Unavailable)]
        [InlineData(false, true, AccessibleStates.None)]
        [InlineData(false, false, AccessibleStates.None)]
        public void TabControlAccessibleObject_State_ReturnsExpected(bool createControl, bool enabled, AccessibleStates expectedAccessibleStates)
        {
            using TabControl tabControl = new() { Enabled = enabled };

            if (createControl)
            {
                tabControl.CreateControl();
            }

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(expectedAccessibleStates, accessibleObject.State);
            Assert.Equal(createControl, tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabControlAccessibleObject_IsSelectionRequired_ReturnsTrue(bool createControl)
        {
            using TabControl tabControl = new();

            if (createControl)
            {
                tabControl.CreateControl();
            }

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.True(accessibleObject.IsSelectionRequired);
            Assert.Equal(createControl, tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_GetChildCount_ReturnsMinusOne_IfHandleIsNotCreated()
        {
            using TabControl tabControl = new();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(-1, accessibleObject.GetChildCount());
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_GetChildCount_ReturnsZero_IfTabPagesListIsEmpty()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(0, accessibleObject.GetChildCount());
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_GetChildCount_ReturnsExpected()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new(), new() });

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(4, accessibleObject.GetChildCount());
            Assert.True(pages[0].IsHandleCreated);
            Assert.True(pages[1].IsHandleCreated);
            Assert.True(pages[2].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_GetChild_ReturnsNull_IfHandleIsNotCreated()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new() });

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Null(accessibleObject.GetChild(0));
            Assert.Null(accessibleObject.GetChild(1));
            Assert.Null(accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));
            Assert.Null(accessibleObject.GetChild(4));
            Assert.False(pages[0].IsHandleCreated);
            Assert.False(pages[1].IsHandleCreated);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_GetChild_ReturnsNull_IfTabPagesListIsEmpty()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Null(accessibleObject.GetChild(0));
            Assert.Null(accessibleObject.GetChild(1));
            Assert.Null(accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));
            Assert.Null(accessibleObject.GetChild(4));
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_GetChild_ReturnsExpected()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new() });
            tabControl.CreateControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Equal(pages[0].AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Equal(pages[0].TabAccessibilityObject, accessibleObject.GetChild(1));
            Assert.Equal(pages[1].TabAccessibilityObject, accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));
            Assert.Null(accessibleObject.GetChild(4));
            Assert.True(pages[0].IsHandleCreated);
            Assert.False(pages[1].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_GetChild_ReturnsExpectd_AfterUpdatingSelectedTab()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new(), new() });
            tabControl.CreateControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(pages[0].AccessibilityObject, accessibleObject.GetChild(0));

            tabControl.SelectedIndex = 1;

            Assert.Equal(pages[1].AccessibilityObject, accessibleObject.GetChild(0));

            tabControl.SelectedIndex = 2;

            Assert.Equal(pages[2].AccessibilityObject, accessibleObject.GetChild(0));
            Assert.True(pages[0].IsHandleCreated);
            Assert.True(pages[1].IsHandleCreated);
            Assert.True(pages[2].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_HitTest_ReturnsNull_IfHandleIsNotCreated()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new() });

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Null(accessibleObject.HitTest(10, 33));
            Assert.False(pages[0].IsHandleCreated);
            Assert.False(pages[1].IsHandleCreated);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_HitTest_ReturnsTabControlAccessibleObject_IfTabPagesListIsEmpty()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            tabControl.CreateControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(accessibleObject, accessibleObject.HitTest(10, 33));
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_HitTest_ReturnsTabPane()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new() });
            tabControl.CreateControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);
            AccessibleObject expectedAccessibleObject = tabControl.SelectedTab.AccessibilityObject;
            Point point = expectedAccessibleObject.Bounds.Location;

            Assert.Equal(expectedAccessibleObject, accessibleObject.HitTest(point.X, point.Y));
            Assert.True(pages[0].IsHandleCreated);
            Assert.False(pages[1].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_HitTest_ReturnsFirstItem()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new() });
            tabControl.CreateControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);
            AccessibleObject expectedAccessibleObject = pages[0].TabAccessibilityObject;
            Point point = expectedAccessibleObject.Bounds.Location;

            Assert.Equal(expectedAccessibleObject, accessibleObject.HitTest(point.X, point.Y));
            Assert.True(pages[0].IsHandleCreated);
            Assert.False(pages[1].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_HitTest_ReturnsSecondItem()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new() });
            tabControl.CreateControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);
            AccessibleObject expectedAccessibleObject = pages[1].TabAccessibilityObject;
            Point point = expectedAccessibleObject.Bounds.Location;

            Assert.Equal(expectedAccessibleObject, accessibleObject.HitTest(point.X, point.Y));
            Assert.True(pages[0].IsHandleCreated);
            Assert.False(pages[1].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_FragmentNavigate_Child_ReturnsNull_IfHandleIsNotCreated()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new() });

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.False(pages[0].IsHandleCreated);
            Assert.False(pages[1].IsHandleCreated);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_FragmentNavigate_Child_ReturnsNull_IfTabPagesListIsEmpty()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfSingleItem()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.Add(new TabPage());
            tabControl.CreateControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(tabControl.SelectedTab.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Equal(pages[0].TabAccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.True(pages[0].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfThreeItems()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new(), new() });
            tabControl.CreateControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(tabControl.SelectedTab.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Equal(pages[0].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Equal(pages[2].TabAccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.True(pages[0].IsHandleCreated);
            Assert.False(pages[1].IsHandleCreated);
            Assert.False(pages[2].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfSecondTabIsSelected()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new(), new() });
            tabControl.CreateControl();

            tabControl.SelectedIndex = 1;
            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(tabControl.SelectedTab.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Equal(pages[1].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Equal(pages[2].TabAccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.True(pages[0].IsHandleCreated);
            Assert.True(pages[1].IsHandleCreated);
            Assert.False(pages[2].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpectd_AfterUpdatingSelectedTab()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new(), new() });
            tabControl.CreateControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(pages[0].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));

            tabControl.SelectedIndex = 1;

            Assert.Equal(pages[1].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));

            tabControl.SelectedIndex = 2;

            Assert.Equal(pages[2].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.True(pages[0].IsHandleCreated);
            Assert.True(pages[1].IsHandleCreated);
            Assert.True(pages[2].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void TabControlAccessibleObject_GetSelection_ReturnsExpected(int selectedIndex)
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new(), new() });
            tabControl.CreateControl();
            tabControl.SelectedIndex = selectedIndex;

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);
            IRawElementProviderSimple[] selectedAccessibleObjects = accessibleObject.GetSelection();

            Assert.Equal(1, selectedAccessibleObjects.Length);
            Assert.Equal(pages[selectedIndex].TabAccessibilityObject, selectedAccessibleObjects[0]);
            Assert.True(pages[0].IsHandleCreated);
            Assert.Equal(selectedIndex == 1, pages[1].IsHandleCreated);
            Assert.Equal(selectedIndex == 2, pages[2].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void TabControlAccessibleObject_GetSelection_ReturnsEmptyArray_IfHandleIsNotCreated(int selectedIndex)
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new(), new() });
            tabControl.SelectedIndex = selectedIndex;

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);
            IRawElementProviderSimple[] selectedAccessibleObjects = accessibleObject.GetSelection();

            Assert.Equal(0, selectedAccessibleObjects.Length);
            Assert.False(pages[0].IsHandleCreated);
            Assert.False(pages[1].IsHandleCreated);
            Assert.False(pages[2].IsHandleCreated);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_GetSelection_ReturnsEmptyArray_IfTabPagesListIsEmpty()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);
            IRawElementProviderSimple[] selectedAccessibleObjects = accessibleObject.GetSelection();

            Assert.Equal(0, selectedAccessibleObjects.Length);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_Support_SelectionPattern()
        {
            using TabControl tabControl = new TabControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.True(accessibleObject.IsPatternSupported(UIA.SelectionPatternId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_GetPropertyValue_IsSelectionPatternAvailable_ReturnsTrue()
        {
            using TabControl tabControl = new TabControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.True((bool)accessibleObject.GetPropertyValue(UIA.IsSelectionPatternAvailablePropertyId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_Support_LegacyIAccessible()
        {
            using TabControl tabControl = new TabControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.True(accessibleObject.IsPatternSupported(UIA.LegacyIAccessiblePatternId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_GetPropertyValue_IsLegacyIAccessiblePatternAvailable_ReturnsTrue()
        {
            using TabControl tabControl = new TabControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.True((bool)accessibleObject.GetPropertyValue(UIA.IsLegacyIAccessiblePatternAvailablePropertyId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Test")]
        [InlineData("")]
        [InlineData(null)]
        public void TabControlAccessibleObject_Name_ReturnsExpected(string accessibleName)
        {
            using TabControl tabControl = new TabControl();
            tabControl.AccessibleName = accessibleName;

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(accessibleName, accessibleObject.Name);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Test")]
        [InlineData("")]
        [InlineData(null)]
        public void TabControlAccessibleObject_Description_ReturnsExpected(string accessibleDescription)
        {
            using TabControl tabControl = new();
            tabControl.AccessibleDescription = accessibleDescription;

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(accessibleDescription, accessibleObject.Description);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Test")]
        [InlineData("")]
        [InlineData(null)]
        public void TabControlAccessibleObject_AccessibleDefaultActionDescription_ReturnsExpected(string accessibleDefaultActionDescription)
        {
            using TabControl tabControl = new();
            tabControl.AccessibleDefaultActionDescription = accessibleDefaultActionDescription;

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(accessibleDefaultActionDescription, accessibleObject.DefaultAction);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Test")]
        [InlineData("")]
        [InlineData(null)]
        public void TabControlAccessibleObject_GetProperyValue_LegacyIAccessibleDefaultActionPropertyId_ReturnsExpected(string accessibleDefaultActionDescription)
        {
            using TabControl tabControl = new();
            tabControl.AccessibleDefaultActionDescription = accessibleDefaultActionDescription;
            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(!string.IsNullOrEmpty(accessibleDefaultActionDescription) ? accessibleDefaultActionDescription : null,
                accessibleObject.GetPropertyValue(UIA.LegacyIAccessibleDefaultActionPropertyId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabControlAccessibleObject_RuntimeId_ReturnsExpected(bool createControl)
        {
            using TabControl tabControl = new();

            if (createControl)
            {
                tabControl.CreateControl();
            }

            Assert.NotNull(tabControl.AccessibilityObject.RuntimeId);
            Assert.Equal(tabControl.InternalHandle, (IntPtr)tabControl.AccessibilityObject.RuntimeId[1]);
            Assert.Equal(createControl, tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_GetPropertyValue_HasKeyboardFocusPropertyId_ReturnsFalse()
        {
            using TabControl tabControl = new TabControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.False((bool)accessibleObject.GetPropertyValue(UIA.HasKeyboardFocusPropertyId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_GetPropertyValue_IsKeyboardFocusablePropertyId_ReturnsTrue()
        {
            using TabControl tabControl = new TabControl();

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.True((bool)accessibleObject.GetPropertyValue(UIA.IsKeyboardFocusablePropertyId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabControlAccessibleObject_GetPropertyValue_IsEnabledPropertyId_ReturnsTrue(bool enabled)
        {
            using TabControl tabControl = new TabControl() { Enabled = enabled };

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(enabled, (bool)accessibleObject.GetPropertyValue(UIA.IsEnabledPropertyId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabControlAccessibleObject_GetPropertyValue_NativeWindowHandlePropertyId_ReturnsTrue(bool createControl)
        {
            using TabControl tabControl = new TabControl();

            if (createControl)
            {
                tabControl.CreateControl();
            }

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(tabControl.InternalHandle, (HWND)accessibleObject.GetPropertyValue(UIA.NativeWindowHandlePropertyId));
            Assert.Equal(createControl, tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, "&Name", "Alt+n")]
        [InlineData(false, "&Name", "Alt+n")]
        [InlineData(true, "Name", null)]
        [InlineData(false, "Name", null)]
        public void TabControlAccessibleObject_KeyboardShortcut_ReturnExpected(bool createControl, string text, string expectedKeyboardShortcut)
        {
            using TabControl tabControl = new() { Text = text };

            if (createControl)
            {
                tabControl.CreateControl();
            }

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(expectedKeyboardShortcut, accessibleObject.KeyboardShortcut);
            Assert.Equal(createControl, tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, "&Name", "Alt+n")]
        [InlineData(false, "&Name", "Alt+n")]
        [InlineData(true, "Name", "")]
        [InlineData(false, "Name", "")]
        public void TabControlAccessibleObject_GetPropertyValue_AccessKey_ReturnExpected(bool createControl, string text, string expectedKeyboardShortcut)
        {
            using TabControl tabControl = new() { Text = text };

            if (createControl)
            {
                tabControl.CreateControl();
            }

            TabControlAccessibleObject accessibleObject = Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

            Assert.Equal(expectedKeyboardShortcut, accessibleObject.GetPropertyValue(UIA.AccessKeyPropertyId));
            Assert.Equal(createControl, tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabControlAccessibleObject_GetPropertyValue_RuntimeId_ReturnsExpected()
        {
            using TabControl tabControl = new();

            object actual = tabControl.AccessibilityObject.GetPropertyValue(UiaCore.UIA.RuntimeIdPropertyId);

            Assert.Equal(tabControl.AccessibilityObject.RuntimeId, actual);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabControlAccessibleObject_GetPropertyValue_IsEnabled_ReturnsExpected(bool enabled)
        {
            using TabControl tabControl = new()
            {
                Enabled = enabled
            };

            object actual = tabControl.AccessibilityObject.GetPropertyValue(UIA.IsEnabledPropertyId);

            Assert.Equal(tabControl.Enabled, actual);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.AutomationIdPropertyId, "TabControl1")]
        public void TabControlAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using TabControl tabControl = new()
            {
                Name = "TabControl1",
                AccessibleName = "TestName"
            };

            object actual = tabControl.AccessibilityObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, actual);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(false, ((int)UIA.IsExpandCollapsePatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsGridItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsGridPatternAvailablePropertyId))]
        [InlineData(true, ((int)UIA.IsLegacyIAccessiblePatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsMultipleViewPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsScrollItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsScrollPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsSelectionItemPatternAvailablePropertyId))]
        [InlineData(true, ((int)UIA.IsSelectionPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTableItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTablePatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTextPattern2AvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTextPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTogglePatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsValuePatternAvailablePropertyId))]
        public void TabControlAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
        {
            using TabControl tabControl = new();
            TabControlAccessibleObject accessibleObject = (TabControlAccessibleObject)tabControl.AccessibilityObject;

            Assert.Equal(expected, accessibleObject.GetPropertyValue((UIA)propertyId) ?? false);
            Assert.False(tabControl.IsHandleCreated);
        }
    }
}
