// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.TabControl;
using static System.Windows.Forms.TabPage;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class TabPage_TabAccessibleObjectTests
{
    [WinFormsTheory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void TabAccessibleObject_Bounds_ReturnsExpected(bool createControl, bool boundsIsEmpty)
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        if (createControl)
        {
            tabControl.CreateControl();
        }

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.Equal(boundsIsEmpty, accessibleObject.Bounds.IsEmpty);
        Assert.Equal(createControl, pages[0].IsHandleCreated);
        Assert.Equal(createControl, tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibilityObject_IsDisconnected_WhenTabPageReleasesUiaProvider()
    {
        using TabPage tabPage = new();
        EnforceTabAccessibilityObjectCreation(tabPage);

        tabPage.ReleaseUiaProvider(tabPage.HWND);

        Assert.Null(tabPage.TestAccessor().Dynamic._tabAccessibilityObject);
        Assert.True(tabPage.IsHandleCreated);

        static void EnforceTabAccessibilityObjectCreation(TabPage tabPage)
        {
            _ = tabPage.TabAccessibilityObject;
            Assert.NotNull(tabPage.TestAccessor().Dynamic._tabAccessibilityObject);
        }
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Test", "Test")]
    public void TabAccessibleObject_Name_ReturnsTabPageText(string text, string expectedText)
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage() { Text = text, Name = "Test" });

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.Equal(expectedText, accessibleObject.Name);
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_DefaultAction_ReturnsNull_IfHandleIsNotCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.Null(accessibleObject.DefaultAction);
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_DefaultAction_Returns_NotEmptyString_IfHandleIsCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());
        tabControl.CreateControl();

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.NotEmpty(accessibleObject.DefaultAction);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.PageTab)]
    [InlineData(false, AccessibleRole.None)]
    public void TabAccessibleObject_Role_ReturnsNone_IfHandleIsNotCreated(bool createControl, AccessibleRole expectedAccessibleRole)
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        if (createControl)
        {
            tabControl.CreateControl();
        }

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.Equal(expectedAccessibleRole, accessibleObject.Role);
        Assert.Equal(createControl, pages[0].IsHandleCreated);
        Assert.Equal(createControl, tabControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void TabAccessibleObject_State_ReturnsExpected_IfHandleIsCreated(bool tabControlEnabled, bool tabPageEnabled)
    {
        using TabControl tabControl = new() { Enabled = tabControlEnabled };
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new() { Enabled = tabPageEnabled }, new() { Enabled = tabPageEnabled }]);
        tabControl.CreateControl();

        TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);
        TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.Equal(AccessibleStates.Focusable | AccessibleStates.Selectable | AccessibleStates.Selected, accessibleObject1.State);
        Assert.Equal(AccessibleStates.Focusable | AccessibleStates.Selectable, accessibleObject2.State);
        Assert.True(pages[0].IsHandleCreated);
        Assert.False(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_State_ReturnsNone_IfHandleIsNotCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.Equal(AccessibleStates.None, accessibleObject.State);
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_FragmentRoot_ReturnsTabControlAccessibleObject()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.Equal(tabControl.AccessibilityObject, accessibleObject.FragmentRoot);
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleIsNotCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);

        TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);
        TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(pages[1].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_FragmentNavigate_Parent_ReturnsTabControlAccessibleObject()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());
        tabControl.CreateControl();

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.Equal(tabControl.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);
        tabControl.CreateControl();

        TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);
        TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.Equal(pages[0].AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.True(pages[0].IsHandleCreated);
        Assert.False(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_AfterChaningSelectedTab()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);
        tabControl.CreateControl();

        TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);
        TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.Equal(pages[0].AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        tabControl.SelectedIndex = 1;

        Assert.Equal(pages[1].AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_Supports_LegacyIAccessiblePattern()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.True(accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId));
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_GetPropertyValue_IsLegacyIAccessiblePatternAvailable_ReturnsTrue()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId));
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_Supports_SelectionItemPattern()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.True(accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_SelectionItemPatternId));
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_GetPropertyValue_IsSelectionItemPatternAvailable_ReturnsTrue()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsSelectionItemPatternAvailablePropertyId));
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_DoesNotSupport_InvokePattern()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.False(accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_InvokePatternId));
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_GetPropertyValue_IsInvokePatternPatternAvailable_ReturnsTrue()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.False((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsInvokePatternAvailablePropertyId));
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_DoDefaultAction_WorksCorrectly()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);
        tabControl.CreateControl();

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.Equal(0, tabControl.SelectedIndex);

        accessibleObject.DoDefaultAction();

        Assert.Equal(1, tabControl.SelectedIndex);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_DoDefaultAction_DoesNotAffectTabControl_IfHandleIsNotCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.Equal(-1, tabControl.SelectedIndex);

        accessibleObject.DoDefaultAction();

        Assert.Equal(-1, tabControl.SelectedIndex);
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(pages[1].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_DoDefaultAction_DoesNotAffectTabControl_IfTabControlIsDisabled()
    {
        using TabControl tabControl = new() { Enabled = false };
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);
        tabControl.CreateControl();

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.Equal(0, tabControl.SelectedIndex);

        accessibleObject.DoDefaultAction();

        Assert.Equal(0, tabControl.SelectedIndex);
        Assert.True(pages[0].IsHandleCreated);
        Assert.False(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_SelectItem_WorksCorrectly()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);
        tabControl.CreateControl();

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.Equal(0, tabControl.SelectedIndex);

        accessibleObject.SelectItem();

        Assert.Equal(1, tabControl.SelectedIndex);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_SelectItem_DoesNotAffectTabControl_IfHandleIsNotCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.Equal(-1, tabControl.SelectedIndex);

        accessibleObject.SelectItem();

        Assert.Equal(-1, tabControl.SelectedIndex);
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(pages[1].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_SelectItem_DoesNotAffectTabControl_IfTabControlIsDisabled()
    {
        using TabControl tabControl = new() { Enabled = false };
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);
        tabControl.CreateControl();

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.Equal(0, tabControl.SelectedIndex);

        accessibleObject.SelectItem();

        Assert.Equal(0, tabControl.SelectedIndex);
        Assert.True(pages[0].IsHandleCreated);
        Assert.False(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_AddToSelection_WorksCorrectly()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);
        tabControl.CreateControl();

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.Equal(0, tabControl.SelectedIndex);

        accessibleObject.AddToSelection();

        Assert.Equal(1, tabControl.SelectedIndex);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_AddToSelection__DoesNotAffectTabControl_IfHandleIsNotCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.Equal(-1, tabControl.SelectedIndex);

        accessibleObject.AddToSelection();

        Assert.Equal(-1, tabControl.SelectedIndex);
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(pages[1].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_AddToSelection_DoesNotAffectTabControl_IfTabControlIsDisabled()
    {
        using TabControl tabControl = new() { Enabled = false };
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);
        tabControl.CreateControl();

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.Equal(0, tabControl.SelectedIndex);

        accessibleObject.AddToSelection();

        Assert.Equal(0, tabControl.SelectedIndex);
        Assert.True(pages[0].IsHandleCreated);
        Assert.False(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, -1)]
    public void TabAccessibleObject_RemoveFromSelection_DoesNotAffectTabControl(bool createControl, int expectedIndex)
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);

        if (createControl)
        {
            tabControl.CreateControl();
        }

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.Equal(expectedIndex, tabControl.SelectedIndex);

        accessibleObject.RemoveFromSelection();

        Assert.Equal(expectedIndex, tabControl.SelectedIndex);
        Assert.Equal(createControl, pages[0].IsHandleCreated);
        Assert.False(pages[1].IsHandleCreated);
        Assert.Equal(createControl, tabControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TabAccessibleObject_ItemSelectionContainer_ReturnsTabControlAccessibleObject(bool createControl)
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);

        if (createControl)
        {
            tabControl.CreateControl();
        }

        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.Equal(tabControl.AccessibilityObject, accessibleObject.ItemSelectionContainer);
        Assert.Equal(createControl, pages[0].IsHandleCreated);
        Assert.False(pages[1].IsHandleCreated);
        Assert.Equal(createControl, tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_ItemIsSelected_ReturnsExpected_IfHandleIsCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);
        tabControl.CreateControl();

        TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);
        TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.True(accessibleObject1.IsItemSelected);
        Assert.False(accessibleObject2.IsItemSelected);

        tabControl.SelectedIndex = 1;

        Assert.False(accessibleObject1.IsItemSelected);
        Assert.True(accessibleObject2.IsItemSelected);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_ItemIsSelected_ReturnsFalse_IfHandleIsNotCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.AddRange([new(), new()]);

        TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);
        TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.False(accessibleObject1.IsItemSelected);
        Assert.False(accessibleObject2.IsItemSelected);
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(pages[1].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TabAccessibleObject_DoDefaultAction_InvokeRaiseAutomationEvent(bool tabPageEnabled)
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new() { Enabled = tabPageEnabled }]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[1]);
        pages[1].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

        pages[1].TabAccessibilityObject.DoDefaultAction();
        Application.DoEvents();

        Assert.Equal(1, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(1, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_DoDefaultAction_DoesNotInvoke_RaiseAutomationEvent_IfTabAlreadySelected()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[0]);
        pages[0].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

        pages[0].TabAccessibilityObject.DoDefaultAction();

        Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_DoDefaultAction_DoesNotInvoke_RaiseAutomationEvent_IfAccessibleObjectIsNotCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[0]);
        pages[0].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        pages[0].TabAccessibilityObject.DoDefaultAction();

        Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TabAccessibleObject_DoDefaultAction_DoesNotInvoke_RaiseAutomationEvent_IfTabControlIsDisabled(bool tabPageEnabled)
    {
        using TabControl tabControl = new() { Enabled = false };
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new() { Enabled = tabPageEnabled }]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[1]);
        pages[1].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

        pages[1].TabAccessibilityObject.DoDefaultAction();

        Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_SetSelectedTab_InvokeRaiseAutomationEvent()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

        SubTabAccessibleObject tabAccessibleObject = new(pages[1]);
        pages[1].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        tabControl.SelectedTab = pages[1];
        Application.DoEvents();

        Assert.Equal(1, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(1, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_SetSelectedTab_DoesNotInvoke_RaiseAutomationEvent_IfAccessibleObjectIsNotCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[1]);
        pages[1].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        tabControl.SelectedTab = pages[1];

        Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_SetSelectedTab_DoesNotInvoke_RaiseAutomationEvent_IfTabAlreadySelected()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[0]);
        pages[0].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

        tabControl.SelectedTab = pages[0];

        Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_SetSelectedIndex_InvokeRaiseAutomationEvent()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[1]);
        pages[1].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

        tabControl.SelectedIndex = 1;
        Application.DoEvents();

        Assert.Equal(1, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(1, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_SetSelectedIndex_DoesNotInvoke_RaiseAutomationEvent_IfAccessibleObjectIsNotCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[1]);
        pages[1].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        tabControl.SelectedIndex = 1;

        Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_SetSelectedIndex_DoesNotInvoke_RaiseAutomationEvent_IfTabAlreadySelected()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[0]);
        pages[0].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

        tabControl.SelectedIndex = 0;

        Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_OnGotFocus_InvokeTabAccessibleObject_RaiseAutomationEvent()
    {
        using SubTabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[0]);
        pages[0].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

        tabControl.OnGotFocus();

        Assert.Equal(1, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_OnGotFocus_DoesNotInvoke_RaiseAutomationEvent_IfControlTabAccessibleObjectIsNotCreated()
    {
        using SubTabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[0]);
        pages[0].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        tabControl.OnGotFocus();

        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TabAccessibleObject_AddToSelection_InvokeRaiseAutomationEvent(bool tabPageEnabled)
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new() { Enabled = tabPageEnabled }]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[1]);
        pages[1].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

        pages[1].TabAccessibilityObject.AddToSelection();
        Application.DoEvents();

        Assert.Equal(1, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(1, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_AddToSelection_DoesNotInvoke_RaiseAutomationEvent_IfAccessibleObjectIsNotCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[0]);
        pages[0].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        pages[0].TabAccessibilityObject.AddToSelection();

        Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_AddToSelection_DoesNotInvoke_RaiseAutomationEvent_IfTabAlreadySelected()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[0]);
        pages[0].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        pages[0].TabAccessibilityObject.AddToSelection();

        Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TabAccessibleObject_AddToSelection_DoesNotInvoke_RaiseAutomationEvent_IfTabControlIsDisabled(bool tabPageEnabled)
    {
        using TabControl tabControl = new() { Enabled = false };
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new() { Enabled = tabPageEnabled }, new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[0]);
        pages[0].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

        pages[0].TabAccessibilityObject.AddToSelection();

        Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TabAccessibleObject_SelectItem_InvokeRaiseAutomationEvent(bool tabPageEnabled)
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new() { Enabled = tabPageEnabled }]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[1]);
        pages[1].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

        pages[1].TabAccessibilityObject.SelectItem();
        Application.DoEvents();

        Assert.Equal(1, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(1, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_SelectItem_DoesNotInvoke_RaiseAutomationEvent_IfAccessibleObjectIsNotCreated()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[0]);
        pages[0].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        pages[0].TabAccessibilityObject.SelectItem();

        Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_SelectItem_DoesNotInvoke_RaiseAutomationEvent_IfTabAlreadySelected()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new()]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[0]);
        pages[0].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

        pages[0].TabAccessibilityObject.SelectItem();

        Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TabAccessibleObject_SelectItem_DoesNotInvoke_RaiseAutomationEvent_IfTabControlIsDisabled(bool tabPageEnabled)
    {
        using TabControl tabControl = new() { Enabled = false };
        TabPageCollection pages = tabControl.TabPages;
        tabControl.CreateControl();
        pages.AddRange([new(), new() { Enabled = tabPageEnabled }]);

        SubTabAccessibleObject tabAccessibleObject = new(pages[1]);
        pages[1].TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);

        pages[1].TabAccessibilityObject.SelectItem();

        Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
        Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        Assert.True(pages[0].IsHandleCreated);
        Assert.True(pages[1].IsHandleCreated);
        Assert.True(tabControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TabAccessibleObject_RuntimeId_ReturnsExpected(bool createControl)
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;

        if (createControl)
        {
            tabControl.CreateControl();
        }

        pages.AddRange([new(), new()]);
        TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);
        TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(pages[1].TabAccessibilityObject);

        Assert.NotNull(accessibleObject1.RuntimeId);
        Assert.Equal(tabControl.HandleInternal, accessibleObject1.RuntimeId[1]);
        Assert.Equal(accessibleObject1.GetHashCode(), accessibleObject1.RuntimeId[2]);
        Assert.NotNull(accessibleObject2.RuntimeId);
        Assert.Equal(tabControl.HandleInternal, accessibleObject2.RuntimeId[1]);
        Assert.Equal(accessibleObject2.GetHashCode(), accessibleObject2.RuntimeId[2]);
        Assert.Equal(createControl, pages[0].IsHandleCreated);
        Assert.Equal(createControl, pages[1].IsHandleCreated);
        Assert.Equal(createControl, tabControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, false, true)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void TabAccessibleObject_GetPropertyValue_IsEnabledPropertyId_ReturnsExpected(bool tabControlEnabled, bool tabPageEnabled, bool expectedEnabled)
    {
        using TabControl tabControl = new() { Enabled = tabControlEnabled };
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage() { Enabled = tabPageEnabled });

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);
        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.Equal(expectedEnabled, (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_GetPropertyValue_IsKeyboardFocusablePropertyId_ReturnsTrue()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);
        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId));
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void TabAccessibleObject_GetPropertyValue_HasKeyboardFocusPropertyId_ReturnsTrue()
    {
        using TabControl tabControl = new();
        TabPageCollection pages = tabControl.TabPages;
        pages.Add(new TabPage());

        Assert.IsType<TabControlAccessibleObject>(tabControl.AccessibilityObject);
        TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(pages[0].TabAccessibilityObject);

        Assert.False((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId));
        Assert.False(pages[0].IsHandleCreated);
        Assert.False(tabControl.IsHandleCreated);
    }

    private class SubTabControl : TabControl
    {
        internal void OnGotFocus() => base.OnGotFocus(EventArgs.Empty);
    }

    private class SubTabAccessibleObject : TabAccessibleObject
    {
        internal SubTabAccessibleObject(TabPage owningTabPage) : base(owningTabPage)
        {
        }

        internal int CallSelectionItemEventCount { get; private set; }

        internal int CallFocusChangedEventCount { get; private set; }

        internal override bool RaiseAutomationEvent(UIA_EVENT_ID eventId)
        {
            switch (eventId)
            {
                case UIA_EVENT_ID.UIA_SelectionItem_ElementSelectedEventId:
                    CallSelectionItemEventCount++;
                    break;
                case UIA_EVENT_ID.UIA_AutomationFocusChangedEventId:
                    CallFocusChangedEventCount++;
                    break;
                default:
                    break;
            }

            return base.RaiseAutomationEvent(eventId);
        }
    }
}
