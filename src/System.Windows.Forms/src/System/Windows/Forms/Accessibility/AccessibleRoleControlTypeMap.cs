// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  For details see [root]\docs\accessibility\accessible-role-controltype.md
/// </summary>
internal static class AccessibleRoleControlTypeMap
{
    private static readonly Dictionary<AccessibleRole, UIA_CONTROLTYPE_ID> s_map = new()
    {
        { AccessibleRole.Alert, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Animation, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Application, UIA_CONTROLTYPE_ID.UIA_WindowControlTypeId },
        { AccessibleRole.Border, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.ButtonDropDown, UIA_CONTROLTYPE_ID.UIA_SplitButtonControlTypeId },
        { AccessibleRole.ButtonDropDownGrid, UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId },
        { AccessibleRole.ButtonMenu, UIA_CONTROLTYPE_ID.UIA_MenuItemControlTypeId },
        { AccessibleRole.Caret, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Cell, UIA_CONTROLTYPE_ID.UIA_DataItemControlTypeId },
        { AccessibleRole.Character, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Chart, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.CheckButton, UIA_CONTROLTYPE_ID.UIA_CheckBoxControlTypeId },
        { AccessibleRole.Client, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Clock, UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId },
        { AccessibleRole.Column, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.ColumnHeader, UIA_CONTROLTYPE_ID.UIA_HeaderControlTypeId },
        { AccessibleRole.ComboBox, UIA_CONTROLTYPE_ID.UIA_ComboBoxControlTypeId },
        { AccessibleRole.Cursor, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Default, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Diagram, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Dial, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Dialog, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Document, UIA_CONTROLTYPE_ID.UIA_DocumentControlTypeId },
        { AccessibleRole.DropList, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Equation, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Graphic, UIA_CONTROLTYPE_ID.UIA_ImageControlTypeId },
        { AccessibleRole.Grip, UIA_CONTROLTYPE_ID.UIA_ThumbControlTypeId },
        { AccessibleRole.Grouping, UIA_CONTROLTYPE_ID.UIA_GroupControlTypeId },
        { AccessibleRole.HelpBalloon, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.HotkeyField, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Indicator, UIA_CONTROLTYPE_ID.UIA_ThumbControlTypeId },
        { AccessibleRole.IpAddress, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Link, UIA_CONTROLTYPE_ID.UIA_HyperlinkControlTypeId },
        { AccessibleRole.List, UIA_CONTROLTYPE_ID.UIA_ListControlTypeId },
        { AccessibleRole.ListItem, UIA_CONTROLTYPE_ID.UIA_ListItemControlTypeId },
        { AccessibleRole.MenuBar, UIA_CONTROLTYPE_ID.UIA_MenuBarControlTypeId },
        { AccessibleRole.MenuItem, UIA_CONTROLTYPE_ID.UIA_MenuItemControlTypeId },
        { AccessibleRole.MenuPopup, UIA_CONTROLTYPE_ID.UIA_MenuControlTypeId },
        { AccessibleRole.None, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Outline, UIA_CONTROLTYPE_ID.UIA_TreeControlTypeId },
        { AccessibleRole.OutlineButton, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.OutlineItem, UIA_CONTROLTYPE_ID.UIA_TreeItemControlTypeId },
        { AccessibleRole.PageTab, UIA_CONTROLTYPE_ID.UIA_TabItemControlTypeId },
        { AccessibleRole.PageTabList, UIA_CONTROLTYPE_ID.UIA_TabControlTypeId },
        { AccessibleRole.Pane, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.ProgressBar, UIA_CONTROLTYPE_ID.UIA_ProgressBarControlTypeId },
        { AccessibleRole.PropertyPage, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.PushButton, UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId },
        { AccessibleRole.RadioButton, UIA_CONTROLTYPE_ID.UIA_RadioButtonControlTypeId },
        { AccessibleRole.Row, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.RowHeader, UIA_CONTROLTYPE_ID.UIA_HeaderControlTypeId },
        { AccessibleRole.ScrollBar, UIA_CONTROLTYPE_ID.UIA_ScrollBarControlTypeId },
        { AccessibleRole.Separator, UIA_CONTROLTYPE_ID.UIA_SeparatorControlTypeId },
        { AccessibleRole.Slider, UIA_CONTROLTYPE_ID.UIA_SliderControlTypeId },
        { AccessibleRole.Sound, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.SpinButton, UIA_CONTROLTYPE_ID.UIA_SpinnerControlTypeId },
        { AccessibleRole.SplitButton, UIA_CONTROLTYPE_ID.UIA_SplitButtonControlTypeId },
        { AccessibleRole.StaticText, UIA_CONTROLTYPE_ID.UIA_TextControlTypeId },
        { AccessibleRole.StatusBar, UIA_CONTROLTYPE_ID.UIA_StatusBarControlTypeId },
        { AccessibleRole.Table, UIA_CONTROLTYPE_ID.UIA_TableControlTypeId },
        { AccessibleRole.Text, UIA_CONTROLTYPE_ID.UIA_EditControlTypeId },
        { AccessibleRole.TitleBar, UIA_CONTROLTYPE_ID.UIA_TitleBarControlTypeId },
        { AccessibleRole.ToolBar, UIA_CONTROLTYPE_ID.UIA_ToolBarControlTypeId },
        { AccessibleRole.ToolTip, UIA_CONTROLTYPE_ID.UIA_ToolTipControlTypeId },
        { AccessibleRole.WhiteSpace, UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId },
        { AccessibleRole.Window, UIA_CONTROLTYPE_ID.UIA_WindowControlTypeId }
    };

    public static UIA_CONTROLTYPE_ID GetControlType(AccessibleRole role) => s_map[role];
}
