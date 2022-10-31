// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Xunit;
using static System.Windows.Forms.ListViewItem;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ListViewItem_ListViewItemBaseAccessibleObjectTests
    {
        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_Ctor_OwnerListViewItemCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ListViewItemBaseAccessibleObject(null));
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_Role_ReturnsExpected()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);

            Assert.Equal(AccessibleRole.ListItem, item.AccessibilityObject.Role);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_DefaultAction_ReturnsExpected()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);

            Assert.Equal(SR.AccessibleActionDoubleClick, item.AccessibilityObject.DefaultAction);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_CurrentIndex_ReturnsExpected()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);

            var accessibleObject = (ListViewItemBaseAccessibleObject)item.AccessibilityObject;

            Assert.Equal(item.Index, accessibleObject.CurrentIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_FragmentRoot_ReturnsExpected()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);

            Assert.Equal(control.AccessibilityObject, item.AccessibilityObject.FragmentRoot);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ListViewItemBaseAccessibleObject_IsItemSelected_ReturnsExpected(bool isSelected)
        {
            using ListView control = new();
            ListViewItem item = new() { Selected = isSelected };
            control.Items.Add(item);

            Assert.Equal(isSelected, item.AccessibilityObject.IsItemSelected);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_DoDefaultAction_DoesNothing_IfControlIsNotCreated()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);

            AccessibleObject accessibleObject = item.AccessibilityObject;

            Assert.False((accessibleObject.State & AccessibleStates.Selected) != 0);

            accessibleObject.DoDefaultAction();

            Assert.False((accessibleObject.State & AccessibleStates.Selected) != 0);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_AddToSelection_WorksExpected()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);
            control.CreateControl();

            AccessibleObject accessibleObject = item.AccessibilityObject;

            Assert.False(accessibleObject.IsItemSelected);

            accessibleObject.AddToSelection();

            Assert.True(accessibleObject.IsItemSelected);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_DoDefaultAction_IfControlIsNotCreated()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);

            AccessibleObject accessibleObject = item.AccessibilityObject;

            Assert.False((accessibleObject.State & AccessibleStates.Selected) != 0);

            accessibleObject.DoDefaultAction();

            Assert.False((accessibleObject.State & AccessibleStates.Selected) != 0);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_DoDefaultAction_WorksExpected()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);
            control.CreateControl();

            AccessibleObject accessibleObject = item.AccessibilityObject;

            Assert.False((accessibleObject.State & AccessibleStates.Selected) != 0);

            accessibleObject.DoDefaultAction();

            Assert.True((accessibleObject.State & AccessibleStates.Selected) != 0);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);

            AccessibleObject accessibleObject = item.AccessibilityObject;
            var actual = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent);

            Assert.Equal(control.AccessibilityObject, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_FragmentNavigate_ToSibling_ReturnsNull()
        {
            using ListView control = new();
            control.Items.AddRange(new ListViewItem[] { new(), new(), new() });

            AccessibleObject accessibleObject1 = control.Items[0].AccessibilityObject;
            AccessibleObject accessibleObject2 = control.Items[1].AccessibilityObject;

            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);

            AccessibleObject accessibleObject = item.AccessibilityObject;

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.Tile)]
        [InlineData(View.List)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.LargeIcon)]
        public void ListViewItemBaseAccessibleObject_GetChild_ReturnsNull_IfViewIsNotDetailsOrTile(View view)
        {
            using ListView control = new() { View = view };
            ListViewItem item = new();
            control.Items.Add(item);

            AccessibleObject accessibleObject = item.AccessibilityObject;

            Assert.Null(item.AccessibilityObject.GetChild(0));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.Tile)]
        [InlineData(View.List)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.LargeIcon)]
        public void ListViewItemBaseAccessibleObject_GetChildCount_ReturnsNull_IfViewIsNotDetailsOrTile(View view)
        {
            using ListView control = new() { View = view };
            ListViewItem item = new();
            control.Items.Add(item);

            AccessibleObject accessibleObject = item.AccessibilityObject;

            Assert.Equal(-1, item.AccessibilityObject.GetChildCount());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_GetSubItemBounds_ReturnsExpected()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);

            var accessibleObject = (ListViewItemBaseAccessibleObject)item.AccessibilityObject;

            Assert.Equal(Rectangle.Empty, accessibleObject.GetSubItemBounds(0));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_GetPropertyValue_ControlType_ReturnsExpected()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);

            var actual = item.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ListItemControlTypeId, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_GetPropertyValue_FrameworkProperty_ReturnsExpected()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);

            var actual = item.AccessibilityObject.GetPropertyValue(UiaCore.UIA.FrameworkIdPropertyId);

            Assert.Equal(NativeMethods.WinFormFrameworkId, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_GetPropertyValue_ReturnsExpected()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);

            Assert.Equal(SR.AccessibleActionDoubleClick, item.AccessibilityObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId));
            Assert.Null(item.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ValueValuePropertyId));
            Assert.True((bool)item.AccessibilityObject.GetPropertyValue(UiaCore.UIA.IsInvokePatternAvailablePropertyId));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ListViewItemBaseAccessibleObject_IsCheckable_IfListViewHasCheckBoxes(bool itemIsChecked)
        {
            using ListView listView = new();
            listView.CheckBoxes = true;
            ListViewItem item = new();
            listView.Items.Add(item);
            item.Checked = itemIsChecked;

            AccessibleObject itemAccessibleObject = item.AccessibilityObject;
            itemAccessibleObject.DoDefaultAction();

            Assert.Equal(!itemIsChecked, item.Checked);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_IfCheckableListViewItem_HasCheckButtonRole()
        {
            using ListView listView = new();
            listView.CheckBoxes = true;
            ListViewItem item = new();
            listView.Items.Add(item);

            AccessibleObject itemAccessibleObject = item.AccessibilityObject;

            Assert.Equal(AccessibleRole.CheckButton, itemAccessibleObject.Role);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ListViewItemBaseAccessibleObject_IfCheckableListViewItem_HasExpectedDefaultAction(bool itemIsChecked)
        {
            using ListView listView = new();
            listView.CheckBoxes = true;
            ListViewItem item = new();
            item.Checked = itemIsChecked;
            listView.Items.Add(item);

            AccessibleObject itemAccessibleObject = item.AccessibilityObject;

            Assert.Equal(itemIsChecked ? SR.AccessibleActionUncheck : SR.AccessibleActionCheck, itemAccessibleObject.DefaultAction);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.ScrollItemPatternId)]
        [InlineData((int)UiaCore.UIA.LegacyIAccessiblePatternId)]
        [InlineData((int)UiaCore.UIA.SelectionItemPatternId)]
        [InlineData((int)UiaCore.UIA.InvokePatternId)]
        [InlineData((int)UiaCore.UIA.TogglePatternId)]
        public void ListViewItemBaseAccessibleObject_IsPatternSupported_ReturnsExpected(int patternId)
        {
            using ListView control = new() { CheckBoxes = true };
            ListViewItem item = new();
            control.Items.Add(item);

            Assert.True(item.AccessibilityObject.IsPatternSupported((UiaCore.UIA)patternId));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_ItemSelectionContainer_ReturnsExpected()
        {
            using ListView control = new();
            ListViewItem item = new();
            control.Items.Add(item);

            Assert.Equal(control.AccessibilityObject, item.AccessibilityObject.ItemSelectionContainer);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, (int)UiaCore.ToggleState.On)]
        [InlineData(false, (int)UiaCore.ToggleState.Off)]
        public void ListViewItemBaseAccessibleObject_ToggleState_ReturnsExpected(bool isChecked, int expected)
        {
            using ListView control = new();
            ListViewItem item = new() { Checked = isChecked };
            control.Items.Add(item);

            Assert.Equal((UiaCore.ToggleState)expected, item.AccessibilityObject.ToggleState);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(false, (int)UiaCore.ToggleState.Off, (int)UiaCore.ToggleState.On)]
        [InlineData(true, (int)UiaCore.ToggleState.On, (int)UiaCore.ToggleState.Off)]
        public void ListViewItemBaseAccessibleObject_Toggle_WorksExpected(bool isChecked, int before, int expected)
        {
            using ListView control = new();
            ListViewItem item = new() { Checked = isChecked };
            control.Items.Add(item);

            AccessibleObject accessibleObject = item.AccessibilityObject;

            Assert.Equal((UiaCore.ToggleState)before, accessibleObject.ToggleState);

            accessibleObject.Toggle();

            Assert.Equal((UiaCore.ToggleState)expected, accessibleObject.ToggleState);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemBaseAccessibleObject_GetPropertyValue_AutomationId_ReturnsExpected()
        {
            using ListView listView = new();
            ListViewItem item = new();
            listView.Items.Add(item);
            var accessibleObject = (ListViewItemBaseAccessibleObject)item.AccessibilityObject;

            var expected = string.Format("{0}-{1}", nameof(ListViewItem), accessibleObject.CurrentIndex);
            Assert.Equal(expected, accessibleObject.GetPropertyValue(UiaCore.UIA.AutomationIdPropertyId));
            Assert.False(listView.IsHandleCreated);
        }

        // More tests for this class has been created already in ListViewItem_ListViewItemAccessibleObjectTests
    }
}
