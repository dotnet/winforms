// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class AccessibleRoleControlTypeMapTests : IClassFixture<ThreadExceptionFixture>
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
            UIA actual = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.True(actual >= UIA.ButtonControlTypeId && actual <= UIA.AppBarControlTypeId);
        }

        [WinFormsFact]
        public void AccessibleRoleControlTypeMap_GetControlType_ThrowsException_IfRoleArgumentIsIncorrect()
        {
            AccessibleRole incorrectRole = (AccessibleRole)(-999);

            Assert.Throws<KeyNotFoundException>(() => AccessibleRoleControlTypeMap.GetControlType(incorrectRole));
        }

        public static IEnumerable<object[]> AccessibleRoleControlTypeMap_GetControlType_ReturnsExpectedValue_TestData()
        {
            yield return new object[] { AccessibleRole.Alert, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Animation, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Application, UIA.WindowControlTypeId };
            yield return new object[] { AccessibleRole.Border, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.ButtonDropDown, UIA.SplitButtonControlTypeId };
            yield return new object[] { AccessibleRole.ButtonDropDownGrid, UIA.ButtonControlTypeId };
            yield return new object[] { AccessibleRole.ButtonMenu, UIA.MenuItemControlTypeId };
            yield return new object[] { AccessibleRole.Caret, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Cell, UIA.DataItemControlTypeId };
            yield return new object[] { AccessibleRole.Character, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Chart, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.CheckButton, UIA.CheckBoxControlTypeId };
            yield return new object[] { AccessibleRole.Client, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Clock, UIA.ButtonControlTypeId };
            yield return new object[] { AccessibleRole.Column, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.ColumnHeader, UIA.HeaderControlTypeId };
            yield return new object[] { AccessibleRole.ComboBox, UIA.ComboBoxControlTypeId };
            yield return new object[] { AccessibleRole.Cursor, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Default, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Diagram, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Dial, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Dialog, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Document, UIA.DocumentControlTypeId };
            yield return new object[] { AccessibleRole.DropList, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Equation, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Graphic, UIA.ImageControlTypeId };
            yield return new object[] { AccessibleRole.Grip, UIA.ThumbControlTypeId };
            yield return new object[] { AccessibleRole.Grouping, UIA.GroupControlTypeId };
            yield return new object[] { AccessibleRole.HelpBalloon, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.HotkeyField, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Indicator, UIA.ThumbControlTypeId };
            yield return new object[] { AccessibleRole.IpAddress, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Link, UIA.HyperlinkControlTypeId };
            yield return new object[] { AccessibleRole.List, UIA.ListControlTypeId };
            yield return new object[] { AccessibleRole.ListItem, UIA.ListItemControlTypeId };
            yield return new object[] { AccessibleRole.MenuBar, UIA.MenuBarControlTypeId };
            yield return new object[] { AccessibleRole.MenuItem, UIA.MenuItemControlTypeId };
            yield return new object[] { AccessibleRole.MenuPopup, UIA.MenuControlTypeId };
            yield return new object[] { AccessibleRole.None, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Outline, UIA.TreeControlTypeId };
            yield return new object[] { AccessibleRole.OutlineButton, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.OutlineItem, UIA.TreeItemControlTypeId };
            yield return new object[] { AccessibleRole.PageTab, UIA.TabItemControlTypeId };
            yield return new object[] { AccessibleRole.PageTabList, UIA.TabControlTypeId };
            yield return new object[] { AccessibleRole.Pane, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.ProgressBar, UIA.ProgressBarControlTypeId };
            yield return new object[] { AccessibleRole.PropertyPage, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.PushButton, UIA.ButtonControlTypeId };
            yield return new object[] { AccessibleRole.RadioButton, UIA.RadioButtonControlTypeId };
            yield return new object[] { AccessibleRole.Row, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.RowHeader, UIA.HeaderControlTypeId };
            yield return new object[] { AccessibleRole.ScrollBar, UIA.ScrollBarControlTypeId };
            yield return new object[] { AccessibleRole.Separator, UIA.SeparatorControlTypeId };
            yield return new object[] { AccessibleRole.Slider, UIA.SliderControlTypeId };
            yield return new object[] { AccessibleRole.Sound, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.SpinButton, UIA.SpinnerControlTypeId };
            yield return new object[] { AccessibleRole.SplitButton, UIA.SplitButtonControlTypeId };
            yield return new object[] { AccessibleRole.StaticText, UIA.TextControlTypeId };
            yield return new object[] { AccessibleRole.StatusBar, UIA.StatusBarControlTypeId };
            yield return new object[] { AccessibleRole.Table, UIA.TableControlTypeId };
            yield return new object[] { AccessibleRole.Text, UIA.EditControlTypeId };
            yield return new object[] { AccessibleRole.TitleBar, UIA.TitleBarControlTypeId };
            yield return new object[] { AccessibleRole.ToolBar, UIA.ToolBarControlTypeId };
            yield return new object[] { AccessibleRole.ToolTip, UIA.ToolTipControlTypeId };
            yield return new object[] { AccessibleRole.WhiteSpace, UIA.PaneControlTypeId };
            yield return new object[] { AccessibleRole.Window, UIA.WindowControlTypeId };
        }

        [WinFormsTheory]
        [MemberData(nameof(AccessibleRoleControlTypeMap_GetControlType_ReturnsExpectedValue_TestData))]
        public void AccessibleRoleControlTypeMap_GetControlType_ReturnsExpectedValue(AccessibleRole role, int expectedType)
        {
            // UIA is less accessible than the test
            // so we have to use "int" type for "expectedType" argument
            UIA actual = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal((UIA)expectedType, actual);
        }
    }
}
