// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;
using static Interop.UiaCore;
using static System.Windows.Forms.TabPage;

namespace System.Windows.Forms.Tests
{
    public class TabPage_TabAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void TabAccessibleObject_Bounds_ReturnsExpected(bool createControl, bool boundsIsEmpty)
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());

            if (createControl)
            {
                tabControl.CreateControl();
            }

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.Equal(boundsIsEmpty, accessibleObject.Bounds.IsEmpty);
            Assert.Equal(createControl, tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("Test", "Test")]
        public void TabAccessibleObject_Name_ReturnsTabPageText(string text, string expectedText)
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage() { Text = text, Name = "Test" });

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.Equal(expectedText, accessibleObject.Name);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_DefaultAction_ReturnsNull_IfHandleIsNotCreated()
        {
            using TabControl tabControl = new TabControl();
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.Null(accessibleObject.DefaultAction);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_DefaultAction_Returns_NotEmptyString_IfHandleIsCreated()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.CreateControl();

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.NotEmpty(accessibleObject.DefaultAction);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.PageTab)]
        [InlineData(false, AccessibleRole.None)]
        public void TabAccessibleObject_Role_ReturnsNone_IfHandleIsNotCreated(bool createControl, AccessibleRole expectedAccessibleRole)
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());

            if (createControl)
            {
                tabControl.CreateControl();
            }

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.Equal(expectedAccessibleRole, accessibleObject.Role);
            Assert.Equal(createControl, tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void TabAccessibleObject_State_ReturnsExpected_IfHandleIsCreated(bool tabControlEnabled, bool tabPageEnabled)
        {
            using TabControl tabControl = new() { Enabled = tabControlEnabled } ;
            tabControl.TabPages.Add(new TabPage() { Enabled = tabPageEnabled });
            tabControl.TabPages.Add(new TabPage() { Enabled = tabPageEnabled });
            tabControl.CreateControl();

            TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);
            TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.Equal(AccessibleStates.Focusable | AccessibleStates.Selectable | AccessibleStates.Selected, accessibleObject1.State);
            Assert.Equal(AccessibleStates.Focusable | AccessibleStates.Selectable, accessibleObject2.State);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_State_ReturnsNone_IfHandleIsNotCreated()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.Equal(AccessibleStates.None, accessibleObject.State);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_FragmentRoot_ReturnsTabControlAccessibleObject()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.Equal(tabControl.AccessibilityObject, accessibleObject.FragmentRoot);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleIsNotCreated()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);
            TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_FragmentNavigate_Parent_ReturnsTabControlAccessibleObject()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.CreateControl();

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.Equal(tabControl.AccessibilityObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());
            tabControl.CreateControl();

            TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);
            TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.Equal(tabControl.TabPages[0].AccessibilityObject, accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_AfterChaningSelectedTab()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());
            tabControl.CreateControl();

            TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);
            TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.Equal(tabControl.TabPages[0].AccessibilityObject, accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            tabControl.SelectedIndex = 1;

            Assert.Equal(tabControl.TabPages[1].AccessibilityObject, accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_Supports_LegacyIAccessiblePattern()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.True(accessibleObject.IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_GetPropertyValue_IsLegacyIAccessiblePatternAvailable_ReturnsTrue()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.True((bool)accessibleObject.GetPropertyValue(UIA.IsLegacyIAccessiblePatternAvailablePropertyId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_Supports_SelectionItemPattern()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.True(accessibleObject.IsPatternSupported(UiaCore.UIA.SelectionItemPatternId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_GetPropertyValue_IsSelectionItemPatternAvailable_ReturnsTrue()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.True((bool)accessibleObject.GetPropertyValue(UIA.IsSelectionItemPatternAvailablePropertyId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_DoesNotSupport_InvokePattern()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.False(accessibleObject.IsPatternSupported(UiaCore.UIA.InvokePatternId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_GetPropertyValue_IsInvokePatternPatternAvailable_ReturnsTrue()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.False((bool)accessibleObject.GetPropertyValue(UIA.IsInvokePatternAvailablePropertyId));
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_DoDefaultAction_WorksCorrectly()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());
            tabControl.CreateControl();

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.Equal(0, tabControl.SelectedIndex);

            accessibleObject.DoDefaultAction();

            Assert.Equal(1, tabControl.SelectedIndex);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_DoDefaultAction_DoesNotAffectTabControl_IfHandleIsNotCreated()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.Equal(-1, tabControl.SelectedIndex);

            accessibleObject.DoDefaultAction();

            Assert.Equal(-1, tabControl.SelectedIndex);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_DoDefaultAction_DoesNotAffectTabControl_IfTabControlIsDisabled()
        {
            using TabControl tabControl = new() { Enabled = false };
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());
            tabControl.CreateControl();

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.Equal(0, tabControl.SelectedIndex);

            accessibleObject.DoDefaultAction();

            Assert.Equal(0, tabControl.SelectedIndex);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_SelectItem_WorksCorrectly()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());
            tabControl.CreateControl();

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.Equal(0, tabControl.SelectedIndex);

            accessibleObject.SelectItem();

            Assert.Equal(1, tabControl.SelectedIndex);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_SelectItem_DoesNotAffectTabControl_IfHandleIsNotCreated()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.Equal(-1, tabControl.SelectedIndex);

            accessibleObject.SelectItem();

            Assert.Equal(-1, tabControl.SelectedIndex);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_SelectItem_DoesNotAffectTabControl_IfTabControlIsDisabled()
        {
            using TabControl tabControl = new() { Enabled = false };
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());
            tabControl.CreateControl();

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.Equal(0, tabControl.SelectedIndex);

            accessibleObject.SelectItem();

            Assert.Equal(0, tabControl.SelectedIndex);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_AddToSelection_WorksCorrectly()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());
            tabControl.CreateControl();

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.Equal(0, tabControl.SelectedIndex);

            accessibleObject.AddToSelection();

            Assert.Equal(1, tabControl.SelectedIndex);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_AddToSelection__DoesNotAffectTabControl_IfHandleIsNotCreated()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.Equal(-1, tabControl.SelectedIndex);

            accessibleObject.AddToSelection();

            Assert.Equal(-1, tabControl.SelectedIndex);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_AddToSelection_DoesNotAffectTabControl_IfTabControlIsDisabled()
        {
            using TabControl tabControl = new() { Enabled = false };
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());
            tabControl.CreateControl();

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.Equal(0, tabControl.SelectedIndex);

            accessibleObject.AddToSelection();

            Assert.Equal(0, tabControl.SelectedIndex);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0)]
        [InlineData(false, -1)]
        public void TabAccessibleObject_RemoveFromSelection_DoesNotAffectTabControl(bool createControl, int expectedIndex)
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());

            if (createControl)
            {
                tabControl.CreateControl();
            }

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.Equal(expectedIndex, tabControl.SelectedIndex);

            accessibleObject.RemoveFromSelection();

            Assert.Equal(expectedIndex, tabControl.SelectedIndex);
            Assert.Equal(createControl, tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabAccessibleObject_ItemSelectionContainer_ReturnsTabControlAccessibleObject(bool createControl)
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());

            if (createControl)
            {
                tabControl.CreateControl();
            }

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);

            Assert.Equal(tabControl.AccessibilityObject, accessibleObject.ItemSelectionContainer);
            Assert.Equal(createControl, tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_ItemIsSelected_ReturnsExpected_IfHandleIsCreated()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());
            tabControl.CreateControl();

            TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);
            TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.True(accessibleObject1.IsItemSelected);
            Assert.False(accessibleObject2.IsItemSelected);

            tabControl.SelectedIndex = 1;

            Assert.False(accessibleObject1.IsItemSelected);
            Assert.True(accessibleObject2.IsItemSelected);
            Assert.True(tabControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_ItemIsSelected_ReturnsFalse_IfHandleIsNotCreated()
        {
            using TabControl tabControl = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());

            TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);
            TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.False(accessibleObject1.IsItemSelected);
            Assert.False(accessibleObject2.IsItemSelected);
            Assert.False(tabControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabAccessibleObject_DoDefaultAction_InvokeRaiseAutomationEvent(bool tabPageEnabled)
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            using TabPage tabPage = new() { Enabled = tabPageEnabled };
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(tabPage);
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabPage.TabAccessibilityObject.DoDefaultAction();

            Assert.Equal(1, tabAccessibleObject.CallSelectionItemEventCount);
            Assert.Equal(1, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsFact]
        public void TabAccessibleObject_DoDefaultAction_DoesNotInvoke_RaiseAutomationEvent_IfTabAlreadySelected()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            using TabPage tabPage = new();
            tabControl.TabPages.Add(tabPage);
            tabControl.TabPages.Add(new TabPage());
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabPage.TabAccessibilityObject.DoDefaultAction();

            Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
            Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabAccessibleObject_DoDefaultAction_DoesNotInvoke_RaiseAutomationEvent_IfTabControlIsDisabled(bool tabPageEnabled)
        {
            using TabControl tabControl = new() { Enabled = false };
            tabControl.CreateControl();
            using TabPage tabPage = new() { Enabled = tabPageEnabled };
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(tabPage);
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabPage.TabAccessibilityObject.DoDefaultAction();

            Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
            Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsFact]
        public void TabAccessibleObject_SetSelectedTab_InvokeRaiseAutomationEvent()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            using TabPage tabPage = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(tabPage);
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabControl.SelectedTab = tabPage;

            Assert.Equal(1, tabAccessibleObject.CallSelectionItemEventCount);
            Assert.Equal(1, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsFact]
        public void TabAccessibleObject_SetSelectedTab_DoesNotInvoke_RaiseAutomationEvent_IfTabAlreadySelected()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            using TabPage tabPage = new();
            tabControl.TabPages.Add(tabPage);
            tabControl.TabPages.Add(new TabPage());
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabControl.SelectedTab = tabPage;

            Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
            Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsFact]
        public void TabAccessibleObject_SetSelectedIndex_InvokeRaiseAutomationEvent()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            using TabPage tabPage = new();
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(tabPage);
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabControl.SelectedIndex = 1;

            Assert.Equal(1, tabAccessibleObject.CallSelectionItemEventCount);
            Assert.Equal(1, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsFact]
        public void TabAccessibleObject_SetSelectedIndex_DoesNotInvoke_RaiseAutomationEvent_IfTabAlreadySelected()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            using TabPage tabPage = new();
            tabControl.TabPages.Add(tabPage);
            tabControl.TabPages.Add(new TabPage());
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabControl.SelectedIndex = 0;

            Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
            Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsFact]
        public void TabAccessibleObject_OnGotFocus_InvokeTabAccessibleObject_RaiseAutomationEvent()
        {
            using SubTabControl tabControl = new();
            tabControl.CreateControl();
            using TabPage tabPage = new();
            tabControl.TabPages.Add(tabPage);
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabControl.OnGotFocus();

            Assert.Equal(1, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabAccessibleObject_AddToSelection_InvokeRaiseAutomationEvent(bool tabPageEnabled)
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            using TabPage tabPage = new() { Enabled = tabPageEnabled };
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(tabPage);
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabPage.TabAccessibilityObject.AddToSelection();

            Assert.Equal(1, tabAccessibleObject.CallSelectionItemEventCount);
            Assert.Equal(1, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsFact]
        public void TabAccessibleObject_AddToSelection_DoesNotInvoke_RaiseAutomationEvent_IfTabAlreadySelected()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            using TabPage tabPage = new();
            tabControl.TabPages.Add(tabPage);
            tabControl.TabPages.Add(new TabPage());
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabPage.TabAccessibilityObject.AddToSelection();

            Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
            Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabAccessibleObject_AddToSelection_DoesNotInvoke_RaiseAutomationEvent_IfTabControlIsDisabled(bool tabPageEnabled)
        {
            using TabControl tabControl = new() { Enabled = false };
            tabControl.CreateControl();
            using TabPage tabPage = new() { Enabled = tabPageEnabled };
            tabControl.TabPages.Add(tabPage);
            tabControl.TabPages.Add(new TabPage());
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabPage.TabAccessibilityObject.AddToSelection();

            Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
            Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabAccessibleObject_SelectItem_InvokeRaiseAutomationEvent(bool tabPageEnabled)
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            using TabPage tabPage = new() { Enabled = tabPageEnabled };
            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(tabPage);
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabPage.TabAccessibilityObject.SelectItem();

            Assert.Equal(1, tabAccessibleObject.CallSelectionItemEventCount);
            Assert.Equal(1, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsFact]
        public void TabAccessibleObject_SelectItem_DoesNotInvoke_RaiseAutomationEvent_IfTabAlreadySelected()
        {
            using TabControl tabControl = new();
            tabControl.CreateControl();
            using TabPage tabPage = new();
            tabControl.TabPages.Add(tabPage);
            tabControl.TabPages.Add(new TabPage());
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabPage.TabAccessibilityObject.SelectItem();

            Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
            Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabAccessibleObject_SelectItem_DoesNotInvoke_RaiseAutomationEvent_IfTabControlIsDisabled(bool tabPageEnabled)
        {
            using TabControl tabControl = new() { Enabled = false };
            tabControl.CreateControl();
            using TabPage tabPage = new() { Enabled = tabPageEnabled };
            tabControl.TabPages.Add(tabPage);
            tabControl.TabPages.Add(new TabPage());
            SubTabAccessibleObject tabAccessibleObject = new(tabPage);
            tabPage.TestAccessor().Dynamic._tabAccessibilityObject = tabAccessibleObject;

            tabPage.TabAccessibilityObject.SelectItem();

            Assert.Equal(0, tabAccessibleObject.CallSelectionItemEventCount);
            Assert.Equal(0, tabAccessibleObject.CallFocusChangedEventCount);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TabAccessibleObject_RuntimeId_ReturnsExpected(bool createControl)
        {
            using TabControl tabControl = new();

            if (createControl)
            {
                tabControl.CreateControl();
            }

            tabControl.TabPages.Add(new TabPage());
            tabControl.TabPages.Add(new TabPage());
            TabAccessibleObject accessibleObject1 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[0].TabAccessibilityObject);
            TabAccessibleObject accessibleObject2 = Assert.IsType<TabAccessibleObject>(tabControl.TabPages[1].TabAccessibilityObject);

            Assert.NotNull(accessibleObject1.RuntimeId);
            Assert.Equal(tabControl.HandleInternal, (IntPtr)accessibleObject1.RuntimeId[1]);
            Assert.Equal(accessibleObject1.GetChildId(), accessibleObject1.RuntimeId[2]);
            Assert.NotNull(accessibleObject2.RuntimeId);
            Assert.Equal(tabControl.HandleInternal, (IntPtr)accessibleObject2.RuntimeId[1]);
            Assert.Equal(accessibleObject2.GetChildId(), accessibleObject2.RuntimeId[2]);
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
            using TabPage tabPage = new() { Enabled = tabPageEnabled };
            tabControl.TabPages.Add(tabPage);

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabPage.TabAccessibilityObject);

            Assert.Equal(expectedEnabled, (bool)accessibleObject.GetPropertyValue(UIA.IsEnabledPropertyId));
            Assert.False(tabPage.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_GetPropertyValue_IsKeyboardFocusablePropertyId_ReturnsTrue()
        {
            using TabControl tabControl = new();
            using TabPage tabPage = new();
            tabControl.TabPages.Add(tabPage);

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabPage.TabAccessibilityObject);

            Assert.True((bool)accessibleObject.GetPropertyValue(UIA.IsKeyboardFocusablePropertyId));
            Assert.False(tabPage.IsHandleCreated);
        }

        [WinFormsFact]
        public void TabAccessibleObject_GetPropertyValue_HasKeyboardFocusPropertyId_ReturnsTrue()
        {
            using TabControl tabControl = new();
            using TabPage tabPage = new();
            tabControl.TabPages.Add(tabPage);

            TabAccessibleObject accessibleObject = Assert.IsType<TabAccessibleObject>(tabPage.TabAccessibilityObject);

            Assert.False((bool)accessibleObject.GetPropertyValue(UIA.HasKeyboardFocusPropertyId));
            Assert.False(tabPage.IsHandleCreated);
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

            internal override bool RaiseAutomationEvent(UiaCore.UIA eventId)
            {
                switch (eventId)
                {
                    case UiaCore.UIA.SelectionItem_ElementSelectedEventId:
                        CallSelectionItemEventCount++;
                        break;
                    case UiaCore.UIA.AutomationFocusChangedEventId:
                        CallFocusChangedEventCount++;
                        break;
                    default:
                        break;
                }

                return base.RaiseAutomationEvent(eventId);
            }
        }
    }
}
