// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.UiaCore;

namespace System.Windows.Forms
{
    /// <summary>
    ///  For details see [root]\docs\accessibility\accessible-role-controltype.md
    /// </summary>
    internal static class AccessibleRoleControlTypeMap
    {
        private static readonly Dictionary<AccessibleRole, UIA> s_map = new Dictionary<AccessibleRole, UIA>
        {
            { AccessibleRole.Alert, UIA.PaneControlTypeId },
            { AccessibleRole.Animation, UIA.PaneControlTypeId },
            { AccessibleRole.Application, UIA.WindowControlTypeId },
            { AccessibleRole.Border, UIA.PaneControlTypeId },
            { AccessibleRole.ButtonDropDown, UIA.SplitButtonControlTypeId },
            { AccessibleRole.ButtonDropDownGrid, UIA.ButtonControlTypeId },
            { AccessibleRole.ButtonMenu, UIA.MenuItemControlTypeId },
            { AccessibleRole.Caret, UIA.PaneControlTypeId },
            { AccessibleRole.Cell, UIA.DataItemControlTypeId },
            { AccessibleRole.Character, UIA.PaneControlTypeId },
            { AccessibleRole.Chart, UIA.PaneControlTypeId },
            { AccessibleRole.CheckButton, UIA.CheckBoxControlTypeId },
            { AccessibleRole.Client, UIA.PaneControlTypeId },
            { AccessibleRole.Clock, UIA.ButtonControlTypeId },
            { AccessibleRole.Column, UIA.PaneControlTypeId },
            { AccessibleRole.ColumnHeader, UIA.HeaderControlTypeId },
            { AccessibleRole.ComboBox, UIA.ComboBoxControlTypeId },
            { AccessibleRole.Cursor, UIA.PaneControlTypeId },
            { AccessibleRole.Default, UIA.PaneControlTypeId },
            { AccessibleRole.Diagram, UIA.PaneControlTypeId },
            { AccessibleRole.Dial, UIA.PaneControlTypeId },
            { AccessibleRole.Dialog, UIA.PaneControlTypeId },
            { AccessibleRole.Document, UIA.DocumentControlTypeId },
            { AccessibleRole.DropList, UIA.PaneControlTypeId },
            { AccessibleRole.Equation, UIA.PaneControlTypeId },
            { AccessibleRole.Graphic, UIA.ImageControlTypeId },
            { AccessibleRole.Grip, UIA.ThumbControlTypeId },
            { AccessibleRole.Grouping, UIA.GroupControlTypeId },
            { AccessibleRole.HelpBalloon, UIA.PaneControlTypeId },
            { AccessibleRole.HotkeyField, UIA.PaneControlTypeId },
            { AccessibleRole.Indicator, UIA.ThumbControlTypeId },
            { AccessibleRole.IpAddress, UIA.PaneControlTypeId },
            { AccessibleRole.Link, UIA.HyperlinkControlTypeId },
            { AccessibleRole.List, UIA.ListControlTypeId },
            { AccessibleRole.ListItem, UIA.ListItemControlTypeId },
            { AccessibleRole.MenuBar, UIA.MenuBarControlTypeId },
            { AccessibleRole.MenuItem, UIA.MenuItemControlTypeId },
            { AccessibleRole.MenuPopup, UIA.MenuControlTypeId },
            { AccessibleRole.None, UIA.PaneControlTypeId },
            { AccessibleRole.Outline, UIA.TreeControlTypeId },
            { AccessibleRole.OutlineButton, UIA.PaneControlTypeId },
            { AccessibleRole.OutlineItem, UIA.TreeItemControlTypeId },
            { AccessibleRole.PageTab, UIA.TabItemControlTypeId },
            { AccessibleRole.PageTabList, UIA.TabControlTypeId },
            { AccessibleRole.Pane, UIA.PaneControlTypeId },
            { AccessibleRole.ProgressBar, UIA.ProgressBarControlTypeId },
            { AccessibleRole.PropertyPage, UIA.PaneControlTypeId },
            { AccessibleRole.PushButton, UIA.ButtonControlTypeId },
            { AccessibleRole.RadioButton, UIA.RadioButtonControlTypeId },
            { AccessibleRole.Row, UIA.PaneControlTypeId },
            { AccessibleRole.RowHeader, UIA.HeaderControlTypeId },
            { AccessibleRole.ScrollBar, UIA.ScrollBarControlTypeId },
            { AccessibleRole.Separator, UIA.SeparatorControlTypeId },
            { AccessibleRole.Slider, UIA.SliderControlTypeId },
            { AccessibleRole.Sound, UIA.PaneControlTypeId },
            { AccessibleRole.SpinButton, UIA.SpinnerControlTypeId },
            { AccessibleRole.SplitButton, UIA.SplitButtonControlTypeId },
            { AccessibleRole.StaticText, UIA.TextControlTypeId },
            { AccessibleRole.StatusBar, UIA.StatusBarControlTypeId },
            { AccessibleRole.Table, UIA.TableControlTypeId },
            { AccessibleRole.Text, UIA.EditControlTypeId },
            { AccessibleRole.TitleBar, UIA.TitleBarControlTypeId },
            { AccessibleRole.ToolBar, UIA.ToolBarControlTypeId },
            { AccessibleRole.ToolTip, UIA.ToolTipControlTypeId },
            { AccessibleRole.WhiteSpace, UIA.PaneControlTypeId },
            { AccessibleRole.Window, UIA.WindowControlTypeId }
        };

        public static UIA GetControlType(AccessibleRole role) => s_map[role];
    }
}
