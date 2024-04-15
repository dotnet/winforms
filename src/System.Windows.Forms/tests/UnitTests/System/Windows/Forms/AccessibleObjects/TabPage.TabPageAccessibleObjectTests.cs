// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.TabControl;
using static System.Windows.Forms.TabPage;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class TabPage_TabPageAccessibilityObjectTests
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

        var actual = (UIA_CONTROLTYPE_ID)(int)tabPage.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId, actual);
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

        var actual = (UIA_CONTROLTYPE_ID)(int)tabPage.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

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
        Assert.Equal(tabPage.HandleInternal, tabPage.AccessibilityObject.RuntimeId[1]);
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

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(tabPage.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabPageAccessibleObject_FragmentNaviage_ReturnsNull_IfHandleIsNotCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(pages[0].AccessibilityObject);

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
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

        Assert.Equal(tabControl.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(pages[0].TabAccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabPageAccessibleObject_FragmentNaviage_ReturnsExpected_IfThreeItem()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new(), new()]);
        tabControl.CreateControl();

        TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(pages[0].AccessibilityObject);

        Assert.Equal(tabControl.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(pages[0].TabAccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
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
        pages.AddRange([new(), new(), new()]);

        TabPageAccessibleObject accessibleObject1 = Assert.IsType<TabPageAccessibleObject>(pages[0].AccessibilityObject);
        TabPageAccessibleObject accessibleObject2 = Assert.IsType<TabPageAccessibleObject>(pages[1].AccessibilityObject);
        TabPageAccessibleObject accessibleObject3 = Assert.IsType<TabPageAccessibleObject>(pages[2].AccessibilityObject);

        // First tab is selected
        Assert.Equal(tabControl.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(pages[0].TabAccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(tabControl.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(tabControl.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        // Second tab is selected
        tabControl.SelectedIndex = 1;

        Assert.Equal(tabControl.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(tabControl.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(pages[0].TabAccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(tabControl.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        // Third tab is selected
        tabControl.SelectedIndex = 2;

        Assert.Equal(tabControl.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(tabControl.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(tabControl.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(pages[0].TabAccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

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

        Assert.True(accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId));
        Assert.False(tabPage.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabPageAccessibleObject_DoesNotSupports_ValuePattern()
    {
        using TabPage tabPage = new();

        TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

        Assert.False(accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_ValuePatternId));
        Assert.False(tabPage.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabPageAccessibleObject_GetPropertyValue_IsLegacyIAccessiblePatternAvailable_ReturnsTrue()
    {
        using TabPage tabPage = new();

        TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId));
        Assert.False(tabPage.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabPageAccessibleObject_GetPropertyValue_IsValuePatternAvailable_ReturnsFalse()
    {
        using TabPage tabPage = new();

        TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

        Assert.False((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId));
        Assert.False(tabPage.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabPageAccessibleObject_GetPropertyValue_HasKeyboardFocusPropertyId_ReturnsFalse()
    {
        using TabPage tabPage = new();

        TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

        Assert.False((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId));
        Assert.False(tabPage.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabPageAccessibleObject_GetPropertyValue_IsKeyboardFocusablePropertyId_ReturnsTrue()
    {
        using TabPage tabPage = new();

        TabPageAccessibleObject accessibleObject = Assert.IsType<TabPageAccessibleObject>(tabPage.AccessibilityObject);

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId));
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

        Assert.Equal(expectedEnabled, (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
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

        Assert.Equal((int)tabPage.InternalHandle, (int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId));
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

        Assert.Equal(expectedKeyboardShortcut, ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AccessKeyPropertyId)).ToStringAndFree());
        Assert.Equal(createControl, tabPage.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_NamePropertyId, "TestName")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_AutomationIdPropertyId, "TabPage1")]
    public void TabPageAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, string expected)
    {
        using TabPage tabPage = new()
        {
            Name = "TabPage1",
            AccessibleName = "TestName"
        };

        string actual = ((BSTR)tabPage.AccessibilityObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID)).ToStringAndFree();

        Assert.Equal(expected, actual);
        Assert.False(tabPage.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsExpandCollapsePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsGridItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsGridPatternAvailablePropertyId))]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsMultipleViewPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsScrollItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsScrollPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsSelectionItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsSelectionPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTableItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTablePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTogglePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId))]
    public void TabPageAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
    {
        using TabPage tabPage = new();
        TabPageAccessibleObject accessibleObject = (TabPageAccessibleObject)tabPage.AccessibilityObject;
        var result = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId);
        Assert.Equal(expected, !result.IsEmpty && (bool)result);
        Assert.False(tabPage.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabPageAccessibleObject_BoundingRectangle_IsCorrect()
    {
        using Form form = new();
        using TabControl tabControl = new();
        using Button button = new();
        using TabPage tabPage = new();

        tabControl.Controls.Add(tabPage);
        tabControl.Location = new(22, 25);
        tabControl.Size = new(151, 104);

        button.Location = new(16, 12);
        button.Size = new(198, 86);

        tabPage.AutoScroll = true;
        tabPage.Controls.Add(button);
        tabPage.Location = new(4, 24);
        tabPage.Size = new(143, 76);

        form.ClientSize = new(520, 305);
        form.Controls.Add(tabControl);

        form.Show();

        Rectangle boundingRectangle = tabPage.AccessibilityObject.BoundingRectangle;

        int horizontalScrollBarHeight = SystemInformation.HorizontalScrollBarHeight;
        int verticalScrollBarWidth = SystemInformation.VerticalScrollBarWidth;

        Rectangle expected = tabPage.RectangleToScreen(tabPage.ClientRectangle);

        Assert.Equal(boundingRectangle.Width, expected.Width + verticalScrollBarWidth);
        Assert.Equal(boundingRectangle.Height, expected.Height + horizontalScrollBarHeight);
    }
}
