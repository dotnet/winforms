// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class AccessibleRoleControlTypeMapTests
{
    public static IEnumerable<object[]> AccessibleRoleControlTypeMap_Contains_AllRoles_TestData()
    {
        Array roles = Enum.GetValues(typeof(AccessibleRole));

        foreach (AccessibleRole role in roles)
        {
            yield return new object[] { role };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(AccessibleRoleControlTypeMap_Contains_AllRoles_TestData))]
    public void AccessibleRoleControlTypeMap_Contains_AllRoles(AccessibleRole role)
    {
        // Check if the map contains the role
        // If so the map returns an exist UIA_ControlTypeId
        UIA_CONTROLTYPE_ID actual = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.True(actual is >= UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId and <= UIA_CONTROLTYPE_ID.UIA_AppBarControlTypeId);
    }

    [WinFormsFact]
    public void AccessibleRoleControlTypeMap_GetControlType_ThrowsException_IfRoleArgumentIsIncorrect()
    {
        AccessibleRole incorrectRole = (AccessibleRole)(-999);

        Assert.Throws<KeyNotFoundException>(() => AccessibleRoleControlTypeMap.GetControlType(incorrectRole));
    }

    public static IEnumerable<object[]> AccessibleRoleControlTypeMap_GetControlType_ReturnsExpectedValue_TestData()
    {
        yield return new object[] { AccessibleRole.Alert, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Animation, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Application, UIA_CONTROLTYPE_ID.UIA_WindowControlTypeId };
        yield return new object[] { AccessibleRole.Border, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.ButtonDropDown, UIA_CONTROLTYPE_ID.UIA_SplitButtonControlTypeId };
        yield return new object[] { AccessibleRole.ButtonDropDownGrid, UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId };
        yield return new object[] { AccessibleRole.ButtonMenu, UIA_CONTROLTYPE_ID.UIA_MenuItemControlTypeId };
        yield return new object[] { AccessibleRole.Caret, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Cell, UIA_CONTROLTYPE_ID.UIA_DataItemControlTypeId };
        yield return new object[] { AccessibleRole.Character, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Chart, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.CheckButton, UIA_CONTROLTYPE_ID.UIA_CheckBoxControlTypeId };
        yield return new object[] { AccessibleRole.Client, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Clock, UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId };
        yield return new object[] { AccessibleRole.Column, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.ColumnHeader, UIA_CONTROLTYPE_ID.UIA_HeaderControlTypeId };
        yield return new object[] { AccessibleRole.ComboBox, UIA_CONTROLTYPE_ID.UIA_ComboBoxControlTypeId };
        yield return new object[] { AccessibleRole.Cursor, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Default, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Diagram, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Dial, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Dialog, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Document, UIA_CONTROLTYPE_ID.UIA_DocumentControlTypeId };
        yield return new object[] { AccessibleRole.DropList, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Equation, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Graphic, UIA_CONTROLTYPE_ID.UIA_ImageControlTypeId };
        yield return new object[] { AccessibleRole.Grip, UIA_CONTROLTYPE_ID.UIA_ThumbControlTypeId };
        yield return new object[] { AccessibleRole.Grouping, UIA_CONTROLTYPE_ID.UIA_GroupControlTypeId };
        yield return new object[] { AccessibleRole.HelpBalloon, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.HotkeyField, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Indicator, UIA_CONTROLTYPE_ID.UIA_ThumbControlTypeId };
        yield return new object[] { AccessibleRole.IpAddress, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Link, UIA_CONTROLTYPE_ID.UIA_HyperlinkControlTypeId };
        yield return new object[] { AccessibleRole.List, UIA_CONTROLTYPE_ID.UIA_ListControlTypeId };
        yield return new object[] { AccessibleRole.ListItem, UIA_CONTROLTYPE_ID.UIA_ListItemControlTypeId };
        yield return new object[] { AccessibleRole.MenuBar, UIA_CONTROLTYPE_ID.UIA_MenuBarControlTypeId };
        yield return new object[] { AccessibleRole.MenuItem, UIA_CONTROLTYPE_ID.UIA_MenuItemControlTypeId };
        yield return new object[] { AccessibleRole.MenuPopup, UIA_CONTROLTYPE_ID.UIA_MenuControlTypeId };
        yield return new object[] { AccessibleRole.None, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Outline, UIA_CONTROLTYPE_ID.UIA_TreeControlTypeId };
        yield return new object[] { AccessibleRole.OutlineButton, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.OutlineItem, UIA_CONTROLTYPE_ID.UIA_TreeItemControlTypeId };
        yield return new object[] { AccessibleRole.PageTab, UIA_CONTROLTYPE_ID.UIA_TabItemControlTypeId };
        yield return new object[] { AccessibleRole.PageTabList, UIA_CONTROLTYPE_ID.UIA_TabControlTypeId };
        yield return new object[] { AccessibleRole.Pane, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.ProgressBar, UIA_CONTROLTYPE_ID.UIA_ProgressBarControlTypeId };
        yield return new object[] { AccessibleRole.PropertyPage, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.PushButton, UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId };
        yield return new object[] { AccessibleRole.RadioButton, UIA_CONTROLTYPE_ID.UIA_RadioButtonControlTypeId };
        yield return new object[] { AccessibleRole.Row, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.RowHeader, UIA_CONTROLTYPE_ID.UIA_HeaderControlTypeId };
        yield return new object[] { AccessibleRole.ScrollBar, UIA_CONTROLTYPE_ID.UIA_ScrollBarControlTypeId };
        yield return new object[] { AccessibleRole.Separator, UIA_CONTROLTYPE_ID.UIA_SeparatorControlTypeId };
        yield return new object[] { AccessibleRole.Slider, UIA_CONTROLTYPE_ID.UIA_SliderControlTypeId };
        yield return new object[] { AccessibleRole.Sound, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.SpinButton, UIA_CONTROLTYPE_ID.UIA_SpinnerControlTypeId };
        yield return new object[] { AccessibleRole.SplitButton, UIA_CONTROLTYPE_ID.UIA_SplitButtonControlTypeId };
        yield return new object[] { AccessibleRole.StaticText, UIA_CONTROLTYPE_ID.UIA_TextControlTypeId };
        yield return new object[] { AccessibleRole.StatusBar, UIA_CONTROLTYPE_ID.UIA_StatusBarControlTypeId };
        yield return new object[] { AccessibleRole.Table, UIA_CONTROLTYPE_ID.UIA_TableControlTypeId };
        yield return new object[] { AccessibleRole.Text, UIA_CONTROLTYPE_ID.UIA_EditControlTypeId };
        yield return new object[] { AccessibleRole.TitleBar, UIA_CONTROLTYPE_ID.UIA_TitleBarControlTypeId };
        yield return new object[] { AccessibleRole.ToolBar, UIA_CONTROLTYPE_ID.UIA_ToolBarControlTypeId };
        yield return new object[] { AccessibleRole.ToolTip, UIA_CONTROLTYPE_ID.UIA_ToolTipControlTypeId };
        yield return new object[] { AccessibleRole.WhiteSpace, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId };
        yield return new object[] { AccessibleRole.Window, UIA_CONTROLTYPE_ID.UIA_WindowControlTypeId };
    }

    [WinFormsTheory]
    [MemberData(nameof(AccessibleRoleControlTypeMap_GetControlType_ReturnsExpectedValue_TestData))]
    public void AccessibleRoleControlTypeMap_GetControlType_ReturnsExpectedValue(AccessibleRole role, int expectedType)
    {
        // UIA is less accessible than the test
        // so we have to use "int" type for "expectedType" argument
        UIA_CONTROLTYPE_ID actual = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal((UIA_CONTROLTYPE_ID)expectedType, actual);
    }
}
