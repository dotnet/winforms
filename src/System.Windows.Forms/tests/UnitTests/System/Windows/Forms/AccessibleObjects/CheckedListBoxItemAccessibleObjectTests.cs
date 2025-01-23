// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.CheckedListBox;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class CheckedListBoxItemAccessibleObjectTests
{
    [WinFormsFact]
    public void CheckedListBoxItemAccessibleObject_Ctor_OwningCheckedListBoxIsNull_RaiseException()
    {
        using CheckedListBox checkedListBox = new();
        var accessibleObject = (CheckedListBoxAccessibleObject)checkedListBox.AccessibilityObject;

        Assert.Throws<ArgumentNullException>(() => { new CheckedListBoxItemAccessibleObject(null, new ItemArray.Entry("A"), accessibleObject); });
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBoxItemAccessibleObject_Ctor_ItemCheckedListBoxIsNull_RaiseException()
    {
        using CheckedListBox checkedListBox = new();
        var accessibleObject = (CheckedListBoxAccessibleObject)checkedListBox.AccessibilityObject;

        Assert.Throws<ArgumentNullException>(() => { new CheckedListBoxItemAccessibleObject(checkedListBox, null, accessibleObject); });
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBoxItemAccessibleObject_Ctor_ParentAccessibleObjectIsNull_RaiseException()
    {
        using CheckedListBox checkedListBox = new();

        Assert.Throws<ArgumentNullException>(() => { new CheckedListBoxItemAccessibleObject(checkedListBox, new ItemArray.Entry("A"), null); });
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("A")]
    [InlineData("")]
    public void CheckedListBoxItemAccessibleObject_Name_ReturnsExpected(string testName)
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add(testName);

        Assert.Equal(testName, checkedListBox.AccessibilityObject.GetChild(0).Name);
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBoxItemAccessibleObject_Role_IsCheckButton()
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");

        Assert.Equal(AccessibleRole.CheckButton, checkedListBox.AccessibilityObject.GetChild(0).Role);
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBoxItemAccessibleObject_CurrentIndex_IsExpected()
    {
        using CheckedListBox checkedListBox = new();

        checkedListBox.Items.Add(0);
        checkedListBox.Items.Add(1);
        checkedListBox.Items.Add(2);

        AccessibleObject accessibleObject = checkedListBox.AccessibilityObject;

        Assert.Equal(0, accessibleObject.GetChild(0).TestAccessor().Dynamic.CurrentIndex);
        Assert.Equal(1, accessibleObject.GetChild(1).TestAccessor().Dynamic.CurrentIndex);
        Assert.Equal(2, accessibleObject.GetChild(2).TestAccessor().Dynamic.CurrentIndex);

        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_TogglePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_InvokePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_ValuePatternId)]
    public void CheckedListBoxItemAccessibleObject_IsPatternSupported_ReturnsExpected(int patternId)
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");

        AccessibleObject itemAccessibleObject = checkedListBox.AccessibilityObject.GetChild(0);

        Assert.True(itemAccessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBoxItemAccessibleObject_RuntimeId_NotNull()
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");

        Assert.NotNull(checkedListBox.AccessibilityObject.GetChild(0).RuntimeId);
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void CheckedListBoxItemAccessibleObject_FragmentNavigate_Parent_IsExpected(int itemIndex)
    {
        using CheckedListBox checkedListBox = new();
        AccessibleObject expected = checkedListBox.AccessibilityObject;

        checkedListBox.Items.Add(0);
        checkedListBox.Items.Add(1);
        checkedListBox.Items.Add(2);

        AccessibleObject itemAccessibleObject = checkedListBox.AccessibilityObject.GetChild(itemIndex);
        IRawElementProviderFragment.Interface actual = itemAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);

        Assert.Equal(expected, actual);
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBoxItemAccessibleObject_FragmentNavigate_NextSibling_IsExpected()
    {
        using CheckedListBox checkedListBox = new();

        checkedListBox.Items.Add(0);
        checkedListBox.Items.Add(1);
        checkedListBox.Items.Add(2);

        AccessibleObject itemAccessibleObject1 = checkedListBox.AccessibilityObject.GetChild(0);
        AccessibleObject itemAccessibleObject2 = checkedListBox.AccessibilityObject.GetChild(1);
        AccessibleObject itemAccessibleObject3 = checkedListBox.AccessibilityObject.GetChild(2);

        Assert.Equal(itemAccessibleObject2, itemAccessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(itemAccessibleObject3, itemAccessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(itemAccessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBoxItemAccessibleObject_FragmentNavigate_PreviousSibling_IsExpected()
    {
        using CheckedListBox checkedListBox = new();

        checkedListBox.Items.Add(0);
        checkedListBox.Items.Add(1);
        checkedListBox.Items.Add(2);

        AccessibleObject itemAccessibleObject1 = checkedListBox.AccessibilityObject.GetChild(0);
        AccessibleObject itemAccessibleObject2 = checkedListBox.AccessibilityObject.GetChild(1);
        AccessibleObject itemAccessibleObject3 = checkedListBox.AccessibilityObject.GetChild(2);

        Assert.Null(itemAccessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(itemAccessibleObject1, itemAccessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(itemAccessibleObject2, itemAccessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBoxItemAccessibleObject_DefaultAction_IfHandleIsNotCreated_ReturnsEmptyString()
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");

        Assert.Equal(string.Empty, checkedListBox.AccessibilityObject.GetChild(0).DefaultAction);
        Assert.False(checkedListBox.IsHandleCreated);
    }

    public static IEnumerable<object[]> CheckedListBoxItemAccessibleObject_DefaultAction_ControlType_IfHandleIsCreated_ReturnsExpected_TestData()
    {
        yield return new object[] { true, SR.AccessibleActionUncheck };
        yield return new object[] { false, SR.AccessibleActionCheck };
    }

    [WinFormsTheory]
    [MemberData(nameof(CheckedListBoxItemAccessibleObject_DefaultAction_ControlType_IfHandleIsCreated_ReturnsExpected_TestData))]
    public void CheckedListBoxItemAccessibleObject_DefaultAction_IfHandleIsCreated_ReturnsExpected(bool isChecked, string expected)
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");
        checkedListBox.SetItemChecked(0, isChecked);
        checkedListBox.CreateControl();

        Assert.Equal(expected, checkedListBox.AccessibilityObject.GetChild(0).DefaultAction);
        Assert.True(checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(CheckedListBoxItemAccessibleObject_DefaultAction_ControlType_IfHandleIsCreated_ReturnsExpected_TestData))]
    public void CheckedListBoxItemAccessibleObject_GetPropertyValue_LegacyIAccessibleDefaultActionPropertyId_ReturnsExpected(bool isChecked, string expected)
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");
        checkedListBox.SetItemChecked(0, isChecked);
        checkedListBox.CreateControl();

        Assert.Equal(expected, ((BSTR)checkedListBox.AccessibilityObject.GetChild(0).GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId)).ToStringAndFree());
        Assert.True(checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckedListBoxItemAccessibleObject_Value_ReturnsExpected(bool isChecked)
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");
        checkedListBox.SetItemChecked(0, isChecked);

        Assert.Equal(isChecked.ToString(), checkedListBox.AccessibilityObject.GetChild(0).Value);
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckedListBoxItemAccessibleObject_DoDefaultAction_IfHandleIsNotCreated_DoesNothing(bool isChecked)
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");
        checkedListBox.SetItemChecked(0, isChecked);

        Assert.Equal(isChecked, checkedListBox.GetItemChecked(0));

        checkedListBox.AccessibilityObject.GetChild(0).DoDefaultAction();

        Assert.Equal(isChecked, checkedListBox.GetItemChecked(0));
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckedListBoxItemAccessibleObject_DoDefaultAction_IfHandleIsCreated_WorksExpected(bool isChecked)
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");
        checkedListBox.SetItemChecked(0, isChecked);
        checkedListBox.CreateControl();

        Assert.Equal(isChecked, checkedListBox.GetItemChecked(0));

        checkedListBox.AccessibilityObject.GetChild(0).DoDefaultAction();

        Assert.Equal(!isChecked, checkedListBox.GetItemChecked(0));
        Assert.True(checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, CheckState.Checked, (int)ToggleState.ToggleState_On)]
    [InlineData(false, CheckState.Unchecked, (int)ToggleState.ToggleState_Off)]
    [InlineData(true, CheckState.Indeterminate, (int)ToggleState.ToggleState_Indeterminate)]
    public void CheckedListBoxItemAccessibleObject_ToggleState_ReturnsExpected(bool checkValue, CheckState checkState, int toggleState)
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");
        checkedListBox.SetItemCheckState(0, checkState);

        Assert.Equal((ToggleState)toggleState, checkedListBox.AccessibilityObject.GetChild(0).ToggleState);
        Assert.Equal(checkValue.ToString(), ((BSTR)checkedListBox.AccessibilityObject.GetChild(0).GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId)).ToStringAndFree());
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckedListBoxItemAccessibleObject_Toggle_IfHandleIsNotCreated_DoesNothing(bool isChecked)
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");
        checkedListBox.SetItemChecked(0, isChecked);

        Assert.Equal(isChecked, checkedListBox.GetItemChecked(0));

        checkedListBox.AccessibilityObject.GetChild(0).Toggle();

        Assert.Equal(isChecked, checkedListBox.GetItemChecked(0));
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckedListBoxItemAccessibleObject_Toggle_IfHandleIsCreated_WorksExpected(bool isChecked)
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");
        checkedListBox.SetItemChecked(0, isChecked);
        checkedListBox.CreateControl();

        Assert.Equal(isChecked, checkedListBox.GetItemChecked(0));

        checkedListBox.AccessibilityObject.GetChild(0).Toggle();

        Assert.Equal(!isChecked, checkedListBox.GetItemChecked(0));
        Assert.True(checkedListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBoxItemAccessibleObject_GetPropertyValue_IsInvokePatternAvailablePropertyId_ReturnsExpected()
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");

        Assert.True((bool)checkedListBox.AccessibilityObject.GetChild(0).GetPropertyValue(UIA_PROPERTY_ID.UIA_IsInvokePatternAvailablePropertyId));
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void CheckedListBoxItemAccessibleObject_GetPropertyValue_ValueValuePropertyId_ReturnsExpected(bool isChecked)
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("A");
        checkedListBox.SetItemChecked(0, isChecked);

        AccessibleObject accessibleObject = checkedListBox.AccessibilityObject.GetChild(0);
        Assert.Equal(isChecked, bool.Parse(((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId)).ToStringAndFree()));
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBoxItemAccessibleObject_State_ReturnsNone_WhenHandleNotCreated()
    {
        using CheckedListBox checkedListBox = new();
        var item = new CheckedListBoxItemAccessibleObject(
            checkedListBox,
            new ItemArray.Entry("Item 1"),
            (CheckedListBoxAccessibleObject)checkedListBox.AccessibilityObject);

        item.State.Should().Be(AccessibleStates.None);
    }

    [WinFormsTheory]
    [InlineData(null, AccessibleStates.Indeterminate)]
    [InlineData(true, AccessibleStates.Checked)]
    [InlineData(false, AccessibleStates.None)]
    public void CheckedListBoxItemAccessibleObject_State_ReturnsExpected(bool? isChecked, AccessibleStates expectedState)
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("Item 1",
            isChecked.HasValue
                ? (isChecked.Value ? CheckState.Checked : CheckState.Unchecked)
                : CheckState.Indeterminate);
        checkedListBox.CreateControl();
        var item = new CheckedListBoxItemAccessibleObject(
            checkedListBox,
            checkedListBox.Items.InnerArray.GetEntryObject(0, 0),
            (CheckedListBoxAccessibleObject)checkedListBox.AccessibilityObject);

        item.State.Should().HaveFlag(expectedState);
    }

    [WinFormsFact]
    public void CheckedListBoxItemAccessibleObject_State_ReturnsExpected_ForSelectedAndFocusedItem()
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Items.Add("Item 1");
        checkedListBox.CreateControl();
        checkedListBox.SelectedIndex = 0; // Select and focus the first item
        var item = new CheckedListBoxItemAccessibleObject(
            checkedListBox,
            checkedListBox.Items.InnerArray.GetEntryObject(0, 0),
            (CheckedListBoxAccessibleObject)checkedListBox.AccessibilityObject);

        item.State.Should().HaveFlag(AccessibleStates.Selected)
                       .And.HaveFlag(AccessibleStates.Focused);
    }
}
