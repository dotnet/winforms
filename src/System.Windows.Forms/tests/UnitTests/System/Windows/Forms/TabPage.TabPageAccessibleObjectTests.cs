// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.UiaCore;
using static System.Windows.Forms.TabControl;
using static System.Windows.Forms.TabPage;

namespace System.Windows.Forms.Tests
{
    public class TabPage_TabPageAccessibilityObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TabPageAccessibilityObject_Ctor_Default()
        {
            using TabPage tabPage = new();
            tabPage.CreateControl();

            Assert.NotNull(tabPage.AccessibilityObject);
            Assert.True(tabPage.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibilityObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
        {
            using TabPage tabPage = new();
            tabPage.CreateControl();
            // AccessibleRole is not set = Default

            object actual = tabPage.AccessibilityObject.GetPropertyValue(UIA.ControlTypePropertyId);

            Assert.Equal(UIA.PaneControlTypeId, actual);
            Assert.True(tabPage.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Client)]
        [InlineData(false, AccessibleRole.None)]
        public void TabPageAccessibilityObject_Role_ReturnsExpected(bool createControl, AccessibleRole expectedAccessibleRole)
        {
            using TabPage tabPage = new();

            if (createControl)
            {
                tabPage.CreateControl();
            }

            Assert.Equal(expectedAccessibleRole, tabPage.AccessibilityObject.Role);
            Assert.Equal(createControl, tabPage.IsHandleCreated);
        }

        public static IEnumerable<object[]> TabPageAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(TabPageAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void TabPageAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using TabPage tabPage = new();
            tabPage.AccessibleRole = role;

            object actual = tabPage.AccessibilityObject.GetPropertyValue(UIA.ControlTypePropertyId);
            UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(tabPage.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void TabPageAccessibleObject_Bounds_ReturnsExpected(bool createControl, bool boundsIsEmpty)
        {
            using TabPage tabPage = new();

            if (createControl)
            {
                tabPage.CreateControl();
            }

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.Equal(boundsIsEmpty, accessibleObject.Bounds.IsEmpty);
            Assert.Equal(createControl, tabPage.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Accessible Name", "Text", "Accessible Name")]
        [InlineData("", "Text", "")]
        [InlineData(null, "Text", "Text")]
        [InlineData(null, null, null)]
        public void TabPageAccessibleObject_Name_ReturnsExpected(string accessibleName, string tabPageText, string expectedName)
        {
            using TabPage tabPage = new()
            {
                AccessibleName = accessibleName,
                Text = tabPageText,
                Name = "Name",
            };

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.Equal(expectedName, accessibleObject.Name);
            Assert.False(tabPage.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Test")]
        [InlineData("")]
        [InlineData(null)]
        public void TabPageAccessibleObject_Description_ReturnsExpected(string accessibleDescription)
        {
            using TabPage tabPage = new();
            tabPage.AccessibleDescription = accessibleDescription;

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.Equal(accessibleDescription, accessibleObject.Description);
            Assert.False(tabPage.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabPageAccessibleObject_RuntimeId_ReturnsExpected(bool createControl)
        {
            using TabPage tabPage = new();

            if (createControl)
            {
                tabPage.CreateControl();
            }

            Assert.NotNull(tabPage.AccessibilityObject.RuntimeId);
            Assert.Equal(tabPage.HandleInternal, (IntPtr)tabPage.AccessibilityObject.RuntimeId[1]);
            Assert.Equal(createControl, tabPage.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("Test")]
        [InlineData("")]
        [InlineData(null)]
        public void TabPageAccessibleObject_AccessibleDefaultActionDescription_ReturnsExpected(string accessibleDefaultActionDescription)
        {
            using TabPage tabPage = new();
            tabPage.AccessibleDefaultActionDescription = accessibleDefaultActionDescription;

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.Equal(accessibleDefaultActionDescription, accessibleObject.DefaultAction);
            Assert.False(tabPage.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, AccessibleStates.Focusable)]
        [InlineData(true, false, AccessibleStates.Focusable | AccessibleStates.Unavailable)]
        [InlineData(false, true, AccessibleStates.None)]
        [InlineData(false, false, AccessibleStates.None)]
        public void TabPageAccessibleObject_State_ReturnExpected(bool createControl, bool enabled, AccessibleStates expectedAccessibleStates)
        {
            using TabPage tabPage = new() { Enabled = enabled };

            if (createControl)
            {
                tabPage.CreateControl();
            }

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.Equal(expectedAccessibleStates, accessibleObject.State);
            Assert.Equal(createControl, tabPage.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibleObject_FragmentNaviage_ReturnsNull_IfTabPageHasNotTabControl()
        {
            using TabPage tabPage = new();
            tabPage.CreateControl();

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.Parent));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.True(tabPage.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibleObject_FragmentNaviage_ReturnsNull_IfHandleIsNotCreated()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.Add(new TabPage());

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(pages[0].AccessibilityObject);

            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.Parent));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.False(pages[0].IsHandleCreated);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibleObject_FragmentNaviage_ReturnsExpected_IfSingleItem()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            TabPageCollection pages = tabControl.TabPages;
            pages.Add(new TabPage());

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(pages[0].AccessibilityObject);

            Assert.Equal(tabControl.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.Parent));
            Assert.Equal(pages[0].TabAccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.True(pages[0].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibleObject_FragmentNaviage_ReturnsExpected_IfThreeItem()
        {
            using TabControl tabControl = new();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new(), new() });
            tabControl.CreateControl();

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(pages[0].AccessibilityObject);

            Assert.Equal(tabControl.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.Parent));
            Assert.Equal(pages[0].TabAccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.True(pages[0].IsHandleCreated);
            Assert.False(pages[1].IsHandleCreated);
            Assert.False(pages[2].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibleObject_FragmentNaviage_ReturnsExpected_AfterChaningSelectedTab()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            TabPageCollection pages = tabControl.TabPages;
            pages.AddRange(new TabPage[] { new(), new(), new() });

            TabPageAccessibleObject accessibleObject1 = Assert.IsType<TabPageAccessibleObject>(pages[0].AccessibilityObject);
            TabPageAccessibleObject accessibleObject2 = Assert.IsType<TabPageAccessibleObject>(pages[1].AccessibilityObject);
            TabPageAccessibleObject accessibleObject3 = Assert.IsType<TabPageAccessibleObject>(pages[2].AccessibilityObject);

            // First tab is selected
            Assert.Equal(tabControl.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.Parent));
            Assert.Equal(pages[0].TabAccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.LastChild));

            Assert.Equal(tabControl.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.Parent));
            Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.LastChild));

            Assert.Equal(tabControl.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.Parent));
            Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.LastChild));

            // Second tab is selected
            tabControl.SelectedIndex = 1;

            Assert.Equal(tabControl.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.Parent));
            Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.LastChild));

            Assert.Equal(tabControl.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.Parent));
            Assert.Equal(pages[0].TabAccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.LastChild));

            Assert.Equal(tabControl.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.Parent));
            Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.LastChild));

            // Third tab is selected
            tabControl.SelectedIndex = 2;

            Assert.Equal(tabControl.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.Parent));
            Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.LastChild));

            Assert.Equal(tabControl.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.Parent));
            Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.LastChild));

            Assert.Equal(tabControl.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.Parent));
            Assert.Equal(pages[0].TabAccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.LastChild));

            Assert.True(pages[0].IsHandleCreated);
            Assert.True(pages[1].IsHandleCreated);
            Assert.True(pages[2].IsHandleCreated);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibleObject_Supports_LegacyIAccessiblePattern()
        {
            using TabPage tabPage = new();

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.True(accessibleObject.IsPatternSupported(UIA.LegacyIAccessiblePatternId));
            Assert.False(tabPage.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibleObject_DoesNotSupports_ValuePattern()
        {
            using TabPage tabPage = new();

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.False(accessibleObject.IsPatternSupported(UIA.ValuePatternId));
            Assert.False(tabPage.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibleObject_GetPropertyValue_IsLegacyIAccessiblePatternAvailable_ReturnsTrue()
        {
            using TabPage tabPage = new();

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.True((bool)accessibleObject.GetPropertyValue(UIA.IsLegacyIAccessiblePatternAvailablePropertyId));
            Assert.False(tabPage.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibleObject_GetPropertyValue_IsValuePatternAvailable_ReturnsFalse()
        {
            using TabPage tabPage = new();

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.False((bool)accessibleObject.GetPropertyValue(UIA.IsValuePatternAvailablePropertyId));
            Assert.False(tabPage.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibleObject_GetPropertyValue_HasKeyboardFocusPropertyId_ReturnsFalse()
        {
            using TabPage tabPage = new();

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.False((bool)accessibleObject.GetPropertyValue(UIA.HasKeyboardFocusPropertyId));
            Assert.False(tabPage.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabPageAccessibleObject_GetPropertyValue_IsKeyboardFocusablePropertyId_ReturnsTrue()
        {
            using TabPage tabPage = new();

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.True((bool)accessibleObject.GetPropertyValue(UIA.IsKeyboardFocusablePropertyId));
            Assert.False(tabPage.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, true)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, false)]
        public void TabPageAccessibleObject_GetPropertyValue_IsEnabledPropertyId_ReturnsExpected(bool tabControlEnabled, bool tabPageEnabled, bool expectedEnabled)
        {
            using TabControl tabControl = new() { Enabled = tabControlEnabled };
            TabPageCollection pages = tabControl.TabPages;
            pages.Add(new TabPage() { Enabled = tabPageEnabled });

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(pages[0].AccessibilityObject);

            Assert.Equal(expectedEnabled, (bool)accessibleObject.GetPropertyValue(UIA.IsEnabledPropertyId));
            Assert.False(pages[0].IsHandleCreated);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabPageAccessibleObject_GetPropertyValue_NativeWindowHandlePropertyId_ReturnsTrue(bool createControl)
        {
            using TabPage tabPage = new();

            if (createControl)
            {
                tabPage.CreateControl();
            }

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.Equal(tabPage.InternalHandle, (HWND)accessibleObject.GetPropertyValue(UIA.NativeWindowHandlePropertyId));
            Assert.Equal(createControl, tabPage.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, "&Name", "Alt+n")]
        [InlineData(false, "&Name", "Alt+n")]
        [InlineData(true, "Name", null)]
        [InlineData(false, "Name", null)]
        public void TabPageAccessibleObject_KeyboardShortcut_ReturnExpected(bool createControl, string text, string expectedKeyboardShortcut)
        {
            using TabPage tabPage = new() { Text = text };

            if (createControl)
            {
                tabPage.CreateControl();
            }

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.Equal(expectedKeyboardShortcut, accessibleObject.KeyboardShortcut);
            Assert.Equal(createControl, tabPage.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, "&Name", "Alt+n")]
        [InlineData(false, "&Name", "Alt+n")]
        [InlineData(true, "Name", "")]
        [InlineData(false, "Name", "")]
        public void TabPageAccessibleObject_GetPropertyValue_AccessKey_ReturnExpected(bool createControl, string text, string expectedKeyboardShortcut)
        {
            using TabPage tabPage = new() { Text = text };

            if (createControl)
            {
                tabPage.CreateControl();
            }

            TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

            Assert.Equal(expectedKeyboardShortcut, accessibleObject.GetPropertyValue(UIA.AccessKeyPropertyId));
            Assert.Equal(createControl, tabPage.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.AutomationIdPropertyId, "TabPage1")]
        public void TabPageAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using TabPage tabPage = new()
            {
                Name = "TabPage1",
                AccessibleName = "TestName"
            };

            object actual = tabPage.AccessibilityObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, actual);
            Assert.False(tabPage.IsHandleCreated);
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
        [InlineData(false, ((int)UIA.IsSelectionPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTableItemPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTablePatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTextPattern2AvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTextPatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsTogglePatternAvailablePropertyId))]
        [InlineData(false, ((int)UIA.IsValuePatternAvailablePropertyId))]
        public void TabPageAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
        {
            using TabPage tabPage = new();
            TabPageAccessibleObject accessibleObject = (TabPageAccessibleObject)tabPage.AccessibilityObject;

            Assert.Equal(expected, accessibleObject.GetPropertyValue((UIA)propertyId) ?? false);
            Assert.False(tabPage.IsHandleCreated);
        }
    }
}
