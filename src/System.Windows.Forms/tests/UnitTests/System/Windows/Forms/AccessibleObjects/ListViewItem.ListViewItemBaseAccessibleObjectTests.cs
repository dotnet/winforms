// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListViewItem;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ListViewItem_ListViewItemBaseAccessibleObjectTests
{
    [WinFormsFact]
    public void ListViewItemBaseAccessibleObject_Ctor_OwnerListViewItemCannotBeNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SubListViewItemBaseAccessibleObject(null));
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
    [BoolData]
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

        accessibleObject.State.Should().NotHaveFlag(AccessibleStates.Selected);

        accessibleObject.DoDefaultAction();

        accessibleObject.State.Should().NotHaveFlag(AccessibleStates.Selected);
        control.IsHandleCreated.Should().BeFalse();
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

        accessibleObject.State.Should().NotHaveFlag(AccessibleStates.Selected);

        accessibleObject.DoDefaultAction();

        accessibleObject.State.Should().NotHaveFlag(AccessibleStates.Selected);
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void ListViewItemBaseAccessibleObject_DoDefaultAction_WorksExpected()
    {
        using ListView control = new();
        ListViewItem item = new();
        control.Items.Add(item);
        control.CreateControl();

        AccessibleObject accessibleObject = item.AccessibilityObject;

        accessibleObject.State.Should().NotHaveFlag(AccessibleStates.Selected);

        accessibleObject.DoDefaultAction();

        accessibleObject.State.Should().HaveFlag(AccessibleStates.Selected);
        control.IsHandleCreated.Should().BeTrue();
    }

    [WinFormsFact]
    public void ListViewItemBaseAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using ListView control = new();
        ListViewItem item = new();
        control.Items.Add(item);

        AccessibleObject accessibleObject = item.AccessibilityObject;
        var actual = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);

        Assert.Equal(control.AccessibilityObject, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemBaseAccessibleObject_FragmentNavigate_ToSibling_ReturnsNull()
    {
        using ListView control = new();
        control.Items.AddRange((ListViewItem[])[new(), new(), new()]);

        AccessibleObject accessibleObject1 = control.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Items[1].AccessibilityObject;

        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemBaseAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
    {
        using ListView control = new();
        ListViewItem item = new();
        control.Items.Add(item);

        AccessibleObject accessibleObject = item.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
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

        var actual = (UIA_CONTROLTYPE_ID)(int)item.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ListItemControlTypeId, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemBaseAccessibleObject_GetPropertyValue_FrameworkProperty_ReturnsExpected()
    {
        using ListView control = new();
        ListViewItem item = new();
        control.Items.Add(item);

        using var actual = item.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_FrameworkIdPropertyId);

        Assert.Equal("WinForm", ((BSTR)actual).ToString());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemBaseAccessibleObject_GetPropertyValue_ReturnsExpected()
    {
        using ListView control = new();
        ListViewItem item = new();
        control.Items.Add(item);

        Assert.Equal(SR.AccessibleActionDoubleClick, ((BSTR)item.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId)).ToStringAndFree());
        Assert.Equal(VARIANT.Empty, item.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId));
        Assert.True((bool)item.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsInvokePatternAvailablePropertyId));
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
        ListViewItem item = new()
        {
            Checked = itemIsChecked
        };
        listView.Items.Add(item);

        AccessibleObject itemAccessibleObject = item.AccessibilityObject;

        Assert.Equal(itemIsChecked ? SR.AccessibleActionUncheck : SR.AccessibleActionCheck, itemAccessibleObject.DefaultAction);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_ScrollItemPatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_SelectionItemPatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_InvokePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TogglePatternId)]
    public void ListViewItemBaseAccessibleObject_IsPatternSupported_ReturnsExpected(int patternId)
    {
        using ListView control = new() { CheckBoxes = true };
        ListViewItem item = new();
        control.Items.Add(item);

        Assert.True(item.AccessibilityObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
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
    [InlineData(true, (int)ToggleState.ToggleState_On)]
    [InlineData(false, (int)ToggleState.ToggleState_Off)]
    public void ListViewItemBaseAccessibleObject_ToggleState_ReturnsExpected(bool isChecked, int expected)
    {
        using ListView control = new();
        ListViewItem item = new() { Checked = isChecked };
        control.Items.Add(item);

        Assert.Equal((ToggleState)expected, item.AccessibilityObject.ToggleState);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(false, (int)ToggleState.ToggleState_Off, (int)ToggleState.ToggleState_On)]
    [InlineData(true, (int)ToggleState.ToggleState_On, (int)ToggleState.ToggleState_Off)]
    public void ListViewItemBaseAccessibleObject_Toggle_WorksExpected(bool isChecked, int before, int expected)
    {
        using ListView control = new();
        ListViewItem item = new() { Checked = isChecked };
        control.Items.Add(item);

        AccessibleObject accessibleObject = item.AccessibilityObject;

        Assert.Equal((ToggleState)before, accessibleObject.ToggleState);

        accessibleObject.Toggle();

        Assert.Equal((ToggleState)expected, accessibleObject.ToggleState);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemBaseAccessibleObject_GetPropertyValue_AutomationId_ReturnsExpected()
    {
        using ListView listView = new();
        ListViewItem item = new();
        listView.Items.Add(item);
        var accessibleObject = (ListViewItemBaseAccessibleObject)item.AccessibilityObject;

        string expected = $"{nameof(ListViewItem)}-{accessibleObject.CurrentIndex}";
        Assert.Equal(expected, ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AutomationIdPropertyId)).ToStringAndFree());
        Assert.False(listView.IsHandleCreated);
    }

    private class SubListViewItemBaseAccessibleObject : ListViewItemBaseAccessibleObject
    {
        protected override View View => View.List;

        public SubListViewItemBaseAccessibleObject(ListViewItem owningItem) : base(owningItem)
        {
        }
    }

    // More tests for this class has been created already in ListViewItem_ListViewItemAccessibleObjectTests
}
