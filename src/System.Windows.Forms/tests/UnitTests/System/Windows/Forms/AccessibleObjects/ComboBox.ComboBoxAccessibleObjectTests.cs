// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ComboBox_ComboBoxAccessibleObjectTests
{
    [WinFormsTheory]
    [InlineData(true, AccessibleRole.ComboBox)]
    [InlineData(false, AccessibleRole.None)]
    public void ComboBoxAccessibleObject_Ctor_Default(bool createControl, AccessibleRole expectedAccessibleRole)
    {
        using ComboBox control = new();
        if (createControl)
        {
            control.CreateControl();
        }

        Assert.Equal(createControl, control.IsHandleCreated);
        ComboBox.ComboBoxAccessibleObject accessibleObject = new(control);
        Assert.Equal(createControl, control.IsHandleCreated);
        Assert.NotNull(accessibleObject.Owner);
        Assert.Equal(expectedAccessibleRole, accessibleObject.Role);
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.DropDownList)]
    public void ComboBoxAccessibleObject_ExpandCollapse_Set_CollapsedState_IfControlIsCreated(ComboBoxStyle comboBoxStyle)
    {
        using ComboBox control = new ComboBox
        {
            DropDownStyle = comboBoxStyle
        };
        control.CreateControl();
        AccessibleObject accessibleObject = control.AccessibilityObject;

        accessibleObject.Expand();
        Assert.NotEqual(AccessibleStates.Collapsed, accessibleObject.State & AccessibleStates.Collapsed);
        Assert.Equal(AccessibleStates.Expanded, accessibleObject.State & AccessibleStates.Expanded);

        accessibleObject.Collapse();
        Assert.Equal(AccessibleStates.Collapsed, accessibleObject.State & AccessibleStates.Collapsed);
        Assert.NotEqual(AccessibleStates.Expanded, accessibleObject.State & AccessibleStates.Expanded);

        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.DropDownList)]
    public void ComboBoxAccessibleObject_ExpandCollapse_Set_CollapsedState_IfControlIsNotCreated(ComboBoxStyle comboBoxStyle)
    {
        using ComboBox control = new ComboBox
        {
            DropDownStyle = comboBoxStyle
        };

        AccessibleObject accessibleObject = control.AccessibilityObject;
        Assert.False(control.IsHandleCreated);

        accessibleObject.Expand();
        Assert.Equal(AccessibleStates.None, accessibleObject.State);
        Assert.NotEqual(AccessibleStates.Collapsed, accessibleObject.State & AccessibleStates.Collapsed);
        Assert.NotEqual(AccessibleStates.Expanded, accessibleObject.State & AccessibleStates.Expanded);

        accessibleObject.Collapse();
        Assert.Equal(AccessibleStates.None, accessibleObject.State);
        Assert.NotEqual(AccessibleStates.Collapsed, accessibleObject.State & AccessibleStates.Collapsed);
        Assert.NotEqual(AccessibleStates.Expanded, accessibleObject.State & AccessibleStates.Expanded);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.DropDownList)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBoxAccessibleObject_FragmentNavigate_FirstChild_ReturnNull_IfHandleNotCreated(ComboBoxStyle comboBoxStyle)
    {
        using ComboBox comboBox = new ComboBox
        {
            DropDownStyle = comboBoxStyle
        };

        AccessibleObject accessibleObject = comboBox.AccessibilityObject;
        IRawElementProviderFragment.Interface firstChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);

        Assert.Null(firstChild);
        Assert.False(comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("Test text")]
    [InlineData(null)]
    public void ComboBoxEditAccessibleObject_NameNotNull(string name)
    {
        using ComboBox control = new();
        control.AccessibleName = name;
        control.CreateControl(false);
        object editAccessibleName = control.ChildEditAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId);
        Assert.NotNull(editAccessibleName);
    }

    [WinFormsFact]
    public void ComboBoxEditAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
    {
        const string name = "Test text";
        using ComboBox comboBox = new();

        Assert.Equal(VARIANT.Empty, comboBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId));
        Assert.Equal(VARIANT.Empty, comboBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleNamePropertyId));

        comboBox.AccessibleName = name;
        comboBox.CreateControl(false);

        Assert.Equal(name, ((BSTR)comboBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree());
        Assert.Equal(name, ((BSTR)comboBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleNamePropertyId)).ToStringAndFree());
        Assert.True(comboBox.IsHandleCreated);
    }

    public static IEnumerable<object[]> ComboBoxAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_TestData()
    {
        foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
        {
            foreach (bool createControl in new[] { true, false })
            {
                foreach (bool droppedDown in new[] { true, false })
                {
                    bool childListDisplayed = droppedDown || comboBoxStyle == ComboBoxStyle.Simple;
                    yield return new object[] { comboBoxStyle, createControl, droppedDown, childListDisplayed };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ComboBoxAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_TestData))]
    public void ComboBoxAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected(
        ComboBoxStyle comboBoxStyle,
        bool createControl,
        bool droppedDown,
        bool childListDisplayed)
    {
        using ComboBox comboBox = new ComboBox
        {
            DropDownStyle = comboBoxStyle
        };

        if (createControl)
        {
            comboBox.CreateControl();
        }

        comboBox.DroppedDown = droppedDown;
        AccessibleObject firstChild = comboBox.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild) as AccessibleObject;

        Assert.NotNull(firstChild);
        Assert.Equal(childListDisplayed, firstChild == comboBox.ChildListAccessibleObject);
        Assert.True(comboBox.IsHandleCreated);
    }

    public static IEnumerable<object[]> ComboBoxAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_TestData()
    {
        foreach (ComboBoxStyle comboBoxStyle in Enum.GetValues(typeof(ComboBoxStyle)))
        {
            foreach (bool createControl in new[] { true, false })
            {
                foreach (bool droppedDown in new[] { true, false })
                {
                    yield return new object[] { comboBoxStyle, createControl, droppedDown };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ComboBoxAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_TestData))]
    public void ComboBoxAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected(ComboBoxStyle comboBoxStyle, bool createControl, bool droppedDown)
    {
        using ComboBox comboBox = new ComboBox
        {
            DropDownStyle = comboBoxStyle
        };

        if (createControl)
        {
            comboBox.CreateControl();
        }

        comboBox.DroppedDown = droppedDown;
        AccessibleObject lastChild = comboBox.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild) as AccessibleObject;
        AccessibleObject expectedLastChild = comboBoxStyle == ComboBoxStyle.Simple
            ? comboBox.ChildEditAccessibleObject
            : GetComboBoxAccessibleObject(comboBox).DropDownButtonUiaProvider;

        Assert.NotNull(lastChild);
        Assert.Equal(expectedLastChild, lastChild);
        Assert.True(comboBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown)]
    [InlineData(ComboBoxStyle.DropDownList)]
    [InlineData(ComboBoxStyle.Simple)]
    public void ComboBoxAccessibleObject_FragmentNavigate_LastChild_ReturnNull_IfHandleNotCreated(ComboBoxStyle comboBoxStyle)
    {
        using ComboBox comboBox = new ComboBox
        {
            DropDownStyle = comboBoxStyle
        };

        AccessibleObject accessibleObject = comboBox.AccessibilityObject;
        IRawElementProviderFragment.Interface lastChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);

        Assert.Null(lastChild);
        Assert.False(comboBox.IsHandleCreated);
    }

    private ComboBox.ComboBoxAccessibleObject GetComboBoxAccessibleObject(ComboBox comboBox)
    {
        return comboBox.AccessibilityObject as ComboBox.ComboBoxAccessibleObject;
    }

    [WinFormsFact]
    public void ComboBoxAccessibleObject_ControlType_IsComboBox_IfAccessibleRoleIsDefault()
    {
        using ComboBox control = new();
        // AccessibleRole is not set = Default

        VARIANT actual = control.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ComboBoxControlTypeId, (UIA_CONTROLTYPE_ID)(int)actual);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ComboBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ComboBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ComboBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ComboBox comboBox = new();
        comboBox.AccessibleRole = role;

        VARIANT actual = comboBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, (UIA_CONTROLTYPE_ID)(int)actual);
        Assert.False(comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBoxEditAccessibleObject_GetPropertyValue_NativeWindowHandle_ReturnsExpected()
    {
        using ComboBox comboBox = new();
        comboBox.CreateControl(false);
        VARIANT actual = comboBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId);

        Assert.True(comboBox.IsHandleCreated);
        Assert.Equal((int)(nint)comboBox.InternalHandle, (int)actual);
    }

    [WinFormsTheory]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsExpandCollapsePatternAvailablePropertyId))]
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
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId))]
    public void ComboBoxAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
    {
        using ComboBox comboBox = new();
        comboBox.CreateControl(false);
        ComboBox.ComboBoxAccessibleObject accessibleObject = (ComboBox.ComboBoxAccessibleObject)comboBox.AccessibilityObject;
        var result = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId);

        Assert.Equal(expected, !result.IsEmpty && (bool)result);
        Assert.True(comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBoxAccessibleObject_DefaultAction_IfAccessibleDefaultActionDescriptionIsNotNull_ReturnsAccessibleDefaultActionDescription()
    {
        using ComboBox comboBox = new() { AccessibleDefaultActionDescription = "Test" };

        Assert.Equal(comboBox.AccessibleDefaultActionDescription, comboBox.AccessibilityObject.DefaultAction);
        Assert.False(comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBoxAccessibleObject_DefaultAction_IfHandleIsNotCreated_ReturnsNull()
    {
        using ComboBox comboBox = new();

        Assert.Empty(comboBox.AccessibilityObject.DefaultAction);
        Assert.False(comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBoxAccessibleObject_DefaultAction_IfHandleIsCreatedAndDropDownStyleIsSimple_ReturnsNull()
    {
        using ComboBox comboBox = new() { DropDownStyle = ComboBoxStyle.Simple };
        comboBox.CreateControl();

        Assert.True(comboBox.IsHandleCreated);
        Assert.Empty(comboBox.AccessibilityObject.DefaultAction);
    }

    public static IEnumerable<object[]> ComboBoxAccessibleObject_DefaultAction_IfHandleIsCreated_ReturnsExpected_TestData()
    {
        yield return new object[] { ComboBoxStyle.DropDown, false, SR.AccessibleActionExpand };
        yield return new object[] { ComboBoxStyle.DropDown, true, SR.AccessibleActionCollapse };
        yield return new object[] { ComboBoxStyle.DropDownList, false, SR.AccessibleActionExpand };
        yield return new object[] { ComboBoxStyle.DropDownList, true, SR.AccessibleActionCollapse };
    }

    [WinFormsTheory]
    [MemberData(nameof(ComboBoxAccessibleObject_DefaultAction_IfHandleIsCreated_ReturnsExpected_TestData))]
    public void ComboBoxAccessibleObject_DefaultAction_IfHandleIsCreated_ReturnsExpected(ComboBoxStyle style, bool droppedDown, string expectedAction)
    {
        using ComboBox comboBox = new() { DropDownStyle = style };
        comboBox.CreateControl();
        comboBox.DroppedDown = droppedDown;

        Assert.True(comboBox.IsHandleCreated);
        Assert.Equal(expectedAction, comboBox.AccessibilityObject.DefaultAction);
    }

    [WinFormsTheory]
    [MemberData(nameof(ComboBoxAccessibleObject_DefaultAction_IfHandleIsCreated_ReturnsExpected_TestData))]
    public void ComboBoxAccessibleObject_GetPropertyValue_IfHandleIsCreated_ReturnsExpected(ComboBoxStyle style, bool droppedDown, string expectedAction)
    {
        using ComboBox comboBox = new() { DropDownStyle = style };
        comboBox.CreateControl();
        comboBox.DroppedDown = droppedDown;

        Assert.Equal(expectedAction, ((BSTR)comboBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId)).ToStringAndFree());
        Assert.True(comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBoxAccessibleObject_DoDefaultAction_IfHandleIsNotCreated_DoesNotExpand()
    {
        using ComboBox comboBox = new();

        Assert.False(comboBox.DroppedDown);

        comboBox.AccessibilityObject.DoDefaultAction();

        Assert.False(comboBox.DroppedDown);
        Assert.False(comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBoxAccessibleObject_DoDefaultAction_IfHandleIsCreatedAndDropDownStyleIsSimple_DoesNotCollapse()
    {
        using ComboBox comboBox = new() { DropDownStyle = ComboBoxStyle.Simple };
        comboBox.CreateControl();

        Assert.True(comboBox.IsHandleCreated);
        Assert.True(comboBox.DroppedDown);

        comboBox.AccessibilityObject.DoDefaultAction();

        Assert.True(comboBox.DroppedDown);
    }

    [WinFormsTheory]
    [InlineData(ComboBoxStyle.DropDown, false, true)]
    [InlineData(ComboBoxStyle.DropDown, true, false)]
    [InlineData(ComboBoxStyle.DropDownList, false, true)]
    [InlineData(ComboBoxStyle.DropDownList, true, false)]
    public void ComboBoxAccessibleObject_DoDefaultAction_IfHandleIsCreated_DoesExpected(ComboBoxStyle style, bool droppedDown, bool expectedDroppedDown)
    {
        using ComboBox comboBox = new() { DropDownStyle = style };
        comboBox.CreateControl();
        comboBox.DroppedDown = droppedDown;

        Assert.True(comboBox.IsHandleCreated);

        comboBox.AccessibilityObject.DoDefaultAction();

        Assert.Equal(expectedDroppedDown, comboBox.DroppedDown);
    }

    [WinFormsFact]
    public void ComboBox_ReleaseUiaProvider_ClearsItemsAccessibleObjects()
    {
        using ComboBox comboBox = CreateComboBoxWithItems();
        comboBox.CreateControl();
        InitComboBoxItemsAccessibleObjects(comboBox);
        var accessibleObject = (ComboBox.ComboBoxAccessibleObject)comboBox.AccessibilityObject;

        Assert.Equal(comboBox.Items.Count, accessibleObject.ItemAccessibleObjects.Count);

        comboBox.ReleaseUiaProvider(comboBox.HWND);

        Assert.Empty(accessibleObject.ItemAccessibleObjects);
        Assert.True(comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBoxItems_Clear_ClearsItemsAccessibleObjects()
    {
        using ComboBox comboBox = CreateComboBoxWithItems();
        comboBox.CreateControl();
        InitComboBoxItemsAccessibleObjects(comboBox);
        var accessibleObject = (ComboBox.ComboBoxAccessibleObject)comboBox.AccessibilityObject;

        Assert.Equal(comboBox.Items.Count, accessibleObject.ItemAccessibleObjects.Count);

        comboBox.Items.Clear();

        Assert.Empty(accessibleObject.ItemAccessibleObjects);
        Assert.True(comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBoxItems_Remove_RemovesItemAccessibleObject()
    {
        using ComboBox comboBox = CreateComboBoxWithItems();
        comboBox.CreateControl();
        InitComboBoxItemsAccessibleObjects(comboBox);
        var accessibleObject = (ComboBox.ComboBoxAccessibleObject)comboBox.AccessibilityObject;
        ComboBox.ObjectCollection.Entry item = comboBox.Items.InnerList[0];

        Assert.True(accessibleObject.ItemAccessibleObjects.ContainsKey(item));
        Assert.Equal(comboBox.Items.Count, accessibleObject.ItemAccessibleObjects.Count);

        comboBox.Items.Remove(item);

        Assert.False(accessibleObject.ItemAccessibleObjects.ContainsKey(item));
        Assert.True(comboBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComboBoxItems_Remove_RemovesItemAccessibleObjectCorrectly_IfOneIsNotCreated()
    {
        using ComboBox comboBox = CreateComboBoxWithItems();
        comboBox.CreateControl();
        InitComboBoxItemsAccessibleObjects(comboBox);

        // Add a new item, but don't create an accessible object for it.
        comboBox.Items.Insert(0, "h");

        var accessibleObject = (ComboBox.ComboBoxAccessibleObject)comboBox.AccessibilityObject;
        ComboBox.ObjectCollection.Entry item = comboBox.Items.InnerList[0];

        Assert.False(accessibleObject.ItemAccessibleObjects.ContainsKey(item));
        // One item's accessible object is not created.
        Assert.Equal(comboBox.Items.Count - 1, accessibleObject.ItemAccessibleObjects.Count);

        // It shouldn't throw an exception when trying to remove the item's accessible object,
        // that is not created for the tested item, from the ItemAccessibleObjects collection.
        comboBox.Items.Remove(item);

        Assert.Equal(comboBox.Items.Count, accessibleObject.ItemAccessibleObjects.Count);
        Assert.True(comboBox.IsHandleCreated);
    }

    private ComboBox CreateComboBoxWithItems()
    {
        ComboBox comboBox = new();
        comboBox.Items.AddRange(
        [
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g"
        ]);

        return comboBox;
    }

    private void InitComboBoxItemsAccessibleObjects(ComboBox comboBox)
    {
        var listAccessibleObject = (ComboBox.ComboBoxChildListUiaProvider)comboBox.ChildListAccessibleObject;
        int childCount = listAccessibleObject.GetChildFragmentCount();

        // Force items accessiblity objects creation
        for (int i = 0; i < childCount; i++)
        {
            listAccessibleObject.GetChildFragment(i);
        }
    }
}
