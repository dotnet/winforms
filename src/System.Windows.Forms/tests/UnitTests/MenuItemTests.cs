// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class MenuItemTests
    {
        [Fact]
        public void MenuItem_Ctor_Default()
        {
            var menuItem = new MenuItem();
            Assert.False(menuItem.BarBreak);
            Assert.False(menuItem.Break);
            Assert.False(menuItem.Checked);
            Assert.False(menuItem.DefaultItem);
            Assert.True(menuItem.Enabled);
            Assert.Equal(-1, menuItem.Index);
            Assert.False(menuItem.IsParent);
            Assert.False(menuItem.MdiList);
            Assert.Empty(menuItem.MenuItems);
            Assert.Equal(0, menuItem.MergeOrder);
            Assert.Equal(MenuMerge.Add, menuItem.MergeType);
            Assert.Equal('\0', menuItem.Mnemonic);
            Assert.False(menuItem.OwnerDraw);
            Assert.Null(menuItem.Parent);
            Assert.False(menuItem.RadioCheck);
            Assert.True(menuItem.ShowShortcut);
            Assert.Equal(Shortcut.None, menuItem.Shortcut);
            Assert.True(menuItem.Visible);
            Assert.Empty(menuItem.Name);
            Assert.Null(menuItem.Site);
            Assert.Null(menuItem.Container);
            Assert.Null(menuItem.Tag);
            Assert.Empty(menuItem.Text);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void MenuItem_Ctor_String(string text, string expectedText)
        {
            var menuItem = new MenuItem(text);
            Assert.False(menuItem.BarBreak);
            Assert.False(menuItem.Break);
            Assert.False(menuItem.Checked);
            Assert.False(menuItem.DefaultItem);
            Assert.True(menuItem.Enabled);
            Assert.Equal(-1, menuItem.Index);
            Assert.False(menuItem.IsParent);
            Assert.False(menuItem.MdiList);
            Assert.Empty(menuItem.MenuItems);
            Assert.Equal(0, menuItem.MergeOrder);
            Assert.Equal(MenuMerge.Add, menuItem.MergeType);
            Assert.Equal('\0', menuItem.Mnemonic);
            Assert.False(menuItem.OwnerDraw);
            Assert.Null(menuItem.Parent);
            Assert.False(menuItem.RadioCheck);
            Assert.True(menuItem.ShowShortcut);
            Assert.Equal(Shortcut.None, menuItem.Shortcut);
            Assert.True(menuItem.Visible);
            Assert.Empty(menuItem.Name);
            Assert.Null(menuItem.Site);
            Assert.Null(menuItem.Container);
            Assert.Null(menuItem.Tag);
            Assert.Equal(expectedText, menuItem.Text);
        }

        public static IEnumerable<object[]> Ctor_String_EventHandler_TestData()
        {
            EventHandler onClick = (sender, e) => { };
            yield return new object[] { null, null, string.Empty };
            yield return new object[] { string.Empty, onClick, string.Empty };
            yield return new object[] { "text", onClick, "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_EventHandler_TestData))]
        public void MenuItem_Ctor_String_EventHandler(string text, EventHandler onClick, string expectedText)
        {
            var menuItem = new MenuItem(text, onClick);
            Assert.False(menuItem.BarBreak);
            Assert.False(menuItem.Break);
            Assert.False(menuItem.Checked);
            Assert.False(menuItem.DefaultItem);
            Assert.True(menuItem.Enabled);
            Assert.Equal(-1, menuItem.Index);
            Assert.False(menuItem.IsParent);
            Assert.False(menuItem.MdiList);
            Assert.Empty(menuItem.MenuItems);
            Assert.Equal(0, menuItem.MergeOrder);
            Assert.Equal(MenuMerge.Add, menuItem.MergeType);
            Assert.Equal('\0', menuItem.Mnemonic);
            Assert.False(menuItem.OwnerDraw);
            Assert.Null(menuItem.Parent);
            Assert.False(menuItem.RadioCheck);
            Assert.True(menuItem.ShowShortcut);
            Assert.Equal(Shortcut.None, menuItem.Shortcut);
            Assert.True(menuItem.Visible);
            Assert.Empty(menuItem.Name);
            Assert.Null(menuItem.Site);
            Assert.Null(menuItem.Container);
            Assert.Null(menuItem.Tag);
            Assert.Same(expectedText, menuItem.Text);
        }

        [Fact]
        public void MenuItem_Ctor_String_EventHandler_OnClick()
        {
            SubMenuItem menuItem = null;
            int callCount = 0;
            EventHandler onClick = (sender, e) =>
            {
                Assert.Same(menuItem, sender);
                callCount++;
            };
            menuItem = new SubMenuItem("text", onClick);
            menuItem.OnClick(null);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> Ctor_String_EventHandler_Shortcut_TestData()
        {
            EventHandler onClick = (sender, e) => { };
            yield return new object[] { null, null, Shortcut.None, string.Empty };
            yield return new object[] { string.Empty, onClick, (Shortcut)(Shortcut.None - 1), string.Empty };
            yield return new object[] { "text", onClick, Shortcut.CtrlA, "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_EventHandler_Shortcut_TestData))]
        public void MenuItem_Ctor_String_EventHandler_Shortcut(string text, EventHandler onClick, Shortcut shortcut, string expectedText)
        {
            var menuItem = new MenuItem(text, onClick, shortcut);
            Assert.False(menuItem.BarBreak);
            Assert.False(menuItem.Break);
            Assert.False(menuItem.Checked);
            Assert.False(menuItem.DefaultItem);
            Assert.True(menuItem.Enabled);
            Assert.Equal(-1, menuItem.Index);
            Assert.False(menuItem.IsParent);
            Assert.False(menuItem.MdiList);
            Assert.Empty(menuItem.MenuItems);
            Assert.Equal(0, menuItem.MergeOrder);
            Assert.Equal(MenuMerge.Add, menuItem.MergeType);
            Assert.Equal('\0', menuItem.Mnemonic);
            Assert.False(menuItem.OwnerDraw);
            Assert.Null(menuItem.Parent);
            Assert.False(menuItem.RadioCheck);
            Assert.True(menuItem.ShowShortcut);
            Assert.Equal(shortcut, menuItem.Shortcut);
            Assert.True(menuItem.Visible);
            Assert.Empty(menuItem.Name);
            Assert.Null(menuItem.Site);
            Assert.Null(menuItem.Container);
            Assert.Null(menuItem.Tag);
            Assert.Same(expectedText, menuItem.Text);
        }

        [Fact]
        public void MenuItem_Ctor_String_EventHandler_Shortcut_OnClick()
        {
            SubMenuItem menuItem = null;
            int callCount = 0;
            EventHandler onClick = (sender, e) =>
            {
                Assert.Same(menuItem, sender);
                callCount++;
            };
            menuItem = new SubMenuItem("text", onClick, Shortcut.None);
            menuItem.OnClick(null);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> Ctor_String_MenuItemArray_TestData()
        {
            yield return new object[] { null, null, false, string.Empty };
            yield return new object[] { string.Empty, Array.Empty<MenuItem>(), false, string.Empty };
            yield return new object[] { "text", new MenuItem[] { new MenuItem() }, true, "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_MenuItemArray_TestData))]
        public void MenuItem_Ctor_String_MenuItemArray(string text, MenuItem[] items, bool expectedIsParent, string expectedText)
        {
            var menuItem = new MenuItem(text, items);
            Assert.False(menuItem.BarBreak);
            Assert.False(menuItem.Break);
            Assert.False(menuItem.Checked);
            Assert.False(menuItem.DefaultItem);
            Assert.True(menuItem.Enabled);
            Assert.Equal(-1, menuItem.Index);
            Assert.Equal(expectedIsParent, menuItem.IsParent);
            Assert.False(menuItem.MdiList);
            Assert.Equal(items ?? Array.Empty<MenuItem>(), menuItem.MenuItems.Cast<MenuItem>());
            Assert.Equal(0, menuItem.MergeOrder);
            Assert.Equal(MenuMerge.Add, menuItem.MergeType);
            Assert.Equal('\0', menuItem.Mnemonic);
            Assert.False(menuItem.OwnerDraw);
            Assert.Null(menuItem.Parent);
            Assert.False(menuItem.RadioCheck);
            Assert.True(menuItem.ShowShortcut);
            Assert.Equal(Shortcut.None, menuItem.Shortcut);
            Assert.True(menuItem.Visible);
            Assert.Empty(menuItem.Name);
            Assert.Null(menuItem.Site);
            Assert.Null(menuItem.Container);
            Assert.Null(menuItem.Tag);
            Assert.Same(expectedText, menuItem.Text);
        }

        public static IEnumerable<object[]> Ctor_MergeType_Int_Shortcut_String_EventHandler_EventHandler_EventHandler_MenuItemArray_TestData()
        {
            EventHandler onClick = (sender, e) => { };
            EventHandler onPopup = (sender, e) => { };
            EventHandler onSelect = (sender, e) => { };

            yield return new object[] { (MenuMerge)(MenuMerge.Add - 1), -1, (Shortcut)(Shortcut.None - 1), null, null, null, null, null, false, string.Empty };
            yield return new object[] { MenuMerge.Add, 0, Shortcut.None, string.Empty, onClick, onPopup, onSelect, Array.Empty<MenuItem>(), false, string.Empty };
            yield return new object[] { MenuMerge.MergeItems, 1, Shortcut.CtrlA, "text", onClick, onPopup, onSelect, new MenuItem[] { new MenuItem() }, true, "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_MergeType_Int_Shortcut_String_EventHandler_EventHandler_EventHandler_MenuItemArray_TestData))]
        public void MenuItem_Ctor_MenuMerge_Int_Shortcut_String_EventHandler_EventHandler_EventHandler_MenuItemArray(MenuMerge mergeType, int mergeOrder, Shortcut shortcut, string text, EventHandler onClick, EventHandler onPopup, EventHandler onSelect, MenuItem[] items, bool expectedIsParent, string expectedText)
        {
            var menuItem = new MenuItem(mergeType, mergeOrder, shortcut, text, onClick, onPopup, onSelect, items);
            Assert.False(menuItem.BarBreak);
            Assert.False(menuItem.Break);
            Assert.False(menuItem.Checked);
            Assert.False(menuItem.DefaultItem);
            Assert.True(menuItem.Enabled);
            Assert.Equal(-1, menuItem.Index);
            Assert.Equal(expectedIsParent, menuItem.IsParent);
            Assert.False(menuItem.MdiList);
            Assert.Equal(items ?? Array.Empty<MenuItem>(), menuItem.MenuItems.Cast<MenuItem>());
            Assert.Equal(mergeOrder, menuItem.MergeOrder);
            Assert.Equal(mergeType, menuItem.MergeType);
            Assert.Equal('\0', menuItem.Mnemonic);
            Assert.False(menuItem.OwnerDraw);
            Assert.Null(menuItem.Parent);
            Assert.False(menuItem.RadioCheck);
            Assert.True(menuItem.ShowShortcut);
            Assert.Equal(shortcut, menuItem.Shortcut);
            Assert.True(menuItem.Visible);
            Assert.Empty(menuItem.Name);
            Assert.Null(menuItem.Site);
            Assert.Null(menuItem.Container);
            Assert.Null(menuItem.Tag);
            Assert.Same(expectedText, menuItem.Text);
        }

        [Fact]
        public void MenuItem_Ctor_MenuMerge_Int_Shortcut_String_EventHandler_EventHandler_EventHandler_MenuItemArray_OnClick()
        {
            SubMenuItem menuItem = null;
            int callCount = 0;
            EventHandler onClick = (sender, e) =>
            {
                Assert.Same(menuItem, sender);
                callCount++;
            };
            menuItem = new SubMenuItem(MenuMerge.Add, 0, Shortcut.None, string.Empty, onClick, null, null, Array.Empty<MenuItem>());
            menuItem.OnClick(null);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void MenuItem_Ctor_MenuMerge_Int_Shortcut_String_EventHandler_EventHandler_EventHandler_MenuItemArray_OnPopup()
        {
            SubMenuItem menuItem = null;
            int callCount = 0;
            EventHandler onPopup = (sender, e) =>
            {
                Assert.Same(menuItem, sender);
                callCount++;
            };
            menuItem = new SubMenuItem(MenuMerge.Add, 0, Shortcut.None, string.Empty, null, onPopup, null, Array.Empty<MenuItem>());
            menuItem.OnPopup(null);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void MenuItem_Ctor_MenuMerge_Int_Shortcut_String_EventHandler_EventHandler_EventHandler_MenuItemArray_OnSelect()
        {
            SubMenuItem menuItem = null;
            int callCount = 0;
            EventHandler onSelect = (sender, e) =>
            {
                Assert.Same(menuItem, sender);
                callCount++;
            };
            menuItem = new SubMenuItem(MenuMerge.Add, 0, Shortcut.None, string.Empty, null, null, onSelect, Array.Empty<MenuItem>());
            menuItem.OnSelect(null);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuItem_BarBreak_Set_GetReturnsExpected(bool value)
        {
            var menuItem = new MenuItem
            {
                BarBreak = value
            };
            Assert.Equal(value, menuItem.BarBreak);
        }

        [Fact]
        public void MenuItem_BarBreak_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.BarBreak);
            Assert.Throws<ObjectDisposedException>(() => menuItem.BarBreak = true);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuItem_Break_Set_GetReturnsExpected(bool value)
        {
            var menuItem = new MenuItem
            {
                Break = value
            };
            Assert.Equal(value, menuItem.Break);
        }

        [Fact]
        public void MenuItem_Break_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.Break);
            Assert.Throws<ObjectDisposedException>(() => menuItem.Break = true);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuItem_Checked_Set_GetReturnsExpected(bool value)
        {
            var menuItem = new MenuItem
            {
                Checked = value
            };
            Assert.Equal(value, menuItem.Checked);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuItem_Checked_SetWithParent_GetReturnsExpected(bool value)
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem });
            menuItem.Checked = value;
            Assert.Equal(value, menuItem.Checked);
        }

        [Fact]
        public void MenuItem_Checked_SetWithMainMenuParent_ThrowsArgumentException()
        {
            var menuItem = new MenuItem();
            var menu = new MainMenu(new MenuItem[] { menuItem });
            Assert.Throws<ArgumentException>("value", () => menuItem.Checked = true);
            Assert.False(menuItem.Checked);

            menuItem.Checked = false;
            Assert.False(menuItem.Checked);
        }

        [Fact]
        public void MenuItem_Checked_SetWithChildren_ThrowsArgumentException()
        {
            var menuItem = new MenuItem("text", new MenuItem[] { new MenuItem() });
            Assert.Throws<ArgumentException>("value", () => menuItem.Checked = true);
            Assert.False(menuItem.Checked);

            menuItem.Checked = false;
            Assert.False(menuItem.Checked);
        }

        [Fact]
        public void MenuItem_Checked_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.Checked);
            Assert.Throws<ObjectDisposedException>(() => menuItem.Checked = true);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuItem_DefaultItem_Set_GetReturnsExpected(bool value)
        {
            var menuItem = new MenuItem
            {
                DefaultItem = value
            };
            Assert.Equal(value, menuItem.DefaultItem);
        }

        [Fact]
        public void MenuItem_DefaultItem_SetWithParent_GetReturnsExpected()
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem });
            menuItem.DefaultItem = true;
            Assert.True(menuItem.DefaultItem);

            // Set same.
            menuItem.DefaultItem = true;
            Assert.True(menuItem.DefaultItem);

            menuItem.DefaultItem = false;
            Assert.False(menuItem.DefaultItem);

            // Set same.
            menuItem.DefaultItem = false;
            Assert.False(menuItem.DefaultItem);
        }

        [Fact]
        public void MenuItem_DefaultItem_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.DefaultItem);
            Assert.Throws<ObjectDisposedException>(() => menuItem.DefaultItem = true);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuItem_Enabled_Set_GetReturnsExpected(bool value)
        {
            var menuItem = new MenuItem
            {
                Enabled = value
            };
            Assert.Equal(value, menuItem.Enabled);
        }

        [Fact]
        public void MenuItem_Enabled_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.Enabled);
            Assert.Throws<ObjectDisposedException>(() => menuItem.Enabled = true);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void MenuItem_Index_SetWithParent_GetReturnsExpected(int value)
        {
            var menuItem1 = new MenuItem();
            var menuItem2 = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem1, menuItem2 });
            menuItem1.Index = value;
            Assert.Equal(value, menuItem1.Index);
            if (value == 0)
            {
                Assert.Equal(new MenuItem[] { menuItem1, menuItem2 }, menu.MenuItems.Cast<MenuItem>());
            }
            else
            {
                Assert.Equal(new MenuItem[] { menuItem2, menuItem1 }, menu.MenuItems.Cast<MenuItem>());
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void MenuItem_Index_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem });
            Assert.Throws<ArgumentOutOfRangeException>("value", () => menuItem.Index = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void MenuItem_Index_SetWithoutParent_Nop(int value)
        {
            var menuItem = new MenuItem
            {
                Index = value
            };
            Assert.Equal(-1, menuItem.Index);
        }

        public static IEnumerable<object[]> IsParent_TestData()
        {
            yield return new object[] { new MenuItem { MdiList = true }, new SubMenu(), true };
            yield return new object[] { new MenuItem { MdiList = true }, null, false };
            yield return new object[] { new MenuItem { MdiList = false }, new SubMenu(), false };
            yield return new object[] { new MenuItem { MdiList = false }, new MenuItem(), false };
            yield return new object[] { new MenuItem { MdiList = false }, null, false };

            foreach (bool mdiList in new bool[] { true, false })
            {
                yield return new object[] { new MenuItem { MdiList = mdiList }, new MenuItem(), false };
                yield return new object[] { new MenuItem { MdiList = mdiList }, null, false };
                yield return new object[] { new MenuItem("text", new MenuItem[] { new MenuItem() }) { MdiList = mdiList }, null, true };
            }

            var disposedItem = new MenuItem("text", Array.Empty<MenuItem>());
            disposedItem.Dispose();
            yield return new object[] { disposedItem, new SubMenu(), false };

            yield return new object[] { new MenuItem { MdiList = true }, new MainMenu(), true };

            var nonMdiForm = new Form { Menu = new MainMenu() };
            yield return new object[] { new MenuItem { MdiList = true }, nonMdiForm.Menu, true };

            var formWithNoMdiChildren = new Form { Menu = new MainMenu() };
            formWithNoMdiChildren.Controls.Add(new MdiClient());
            yield return new object[] { new MenuItem { MdiList = true }, formWithNoMdiChildren.Menu, true };

            var formWithMdiChildren = new Form { Menu = new MainMenu() };
            var client = new MdiClient();
            formWithMdiChildren.Controls.Add(client);
            client.Controls.Add(new Form { MdiParent = formWithMdiChildren });
            yield return new object[] { new MenuItem { MdiList = true }, formWithMdiChildren.Menu, true };
        }

        [Theory]
        [MemberData(nameof(IsParent_TestData))]
        public void MenuItem_IsParent_HasParent_ReturnsExpected(MenuItem menuItem, Menu parent, bool expected)
        {
            parent?.MenuItems.Add(menuItem);
            Assert.Equal(expected, menuItem.IsParent);
        }

        [Fact]
        public void MenuItem_IsParent_MdiSeparator_ReturnsFalse()
        {
            var parentForm = new Form { Menu = new MainMenu() };
            var parentFormClient = new MdiClient();
            parentForm.Controls.Add(parentFormClient);
            parentFormClient.Controls.Add(new Form { MdiParent = parentForm });
            var menuItem = new SubMenuItem("text", new MenuItem[] { new MenuItem() }) { MdiList = true };
            var parentMenuItem = new MenuItem("parent", new MenuItem[] { menuItem });
            parentForm.Menu.MenuItems.Add(parentMenuItem);

            // Has a normal child and a MDI item.
            menuItem.OnPopup(null);
            Assert.True(menuItem.IsParent);

            // Has only MDI items.
            menuItem.MenuItems.RemoveAt(0);
            Assert.Equal("-", Assert.Single(menuItem.MenuItems.Cast<MenuItem>()).Text);
            Assert.True(menuItem.IsParent);

            // Parent form does not have MDI forms.
            parentForm.IsMdiContainer = false;
            Assert.Equal("-", Assert.Single(menuItem.MenuItems.Cast<MenuItem>()).Text);
            Assert.False(menuItem.IsParent);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuItem_MdiList_Set_GetReturnsExpected(bool value)
        {
            var menuItem = new MenuItem
            {
                MdiList = value
            };
            Assert.Equal(value, menuItem.MdiList);
        }

        [Fact]
        public void MenuItem_MdiList_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.MdiList);
            Assert.Throws<ObjectDisposedException>(() => menuItem.MdiList = true);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void MenuItem_MergeOrder_Set_GetReturnsExpected(int value)
        {
            var menuItem = new MenuItem
            {
                MergeOrder = value
            };
            Assert.Equal(value, menuItem.MergeOrder);
        }

        [Fact]
        public void MenuItem_MergeOrder_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.MergeOrder);
            Assert.Throws<ObjectDisposedException>(() => menuItem.MergeOrder = 1);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(MenuMerge))]
        public void MenuItem_MergeType_Set_GetReturnsExpected(MenuMerge value)
        {
            var menuItem = new MenuItem
            {
                MergeType = value
            };
            Assert.Equal(value, menuItem.MergeType);
        }

        [Fact]
        public void MenuItem_MergeType_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.MergeType);
            Assert.Throws<ObjectDisposedException>(() => menuItem.MergeType = MenuMerge.Add);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(MenuMerge))]
        public void MenuItem_MergeType_SetInvalid_ThrowsInvalidEnumArgumentException(MenuMerge value)
        {
            var menuItem = new MenuItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => menuItem.MergeType = value);
        }

        [Theory]
        [InlineData("", '\0')]
        [InlineData("text", '\0')]
        [InlineData("&&abc", '\0')]
        [InlineData("&abc", 'A')]
        [InlineData("&", '\0')]
        [InlineData("&&", '\0')]
        public void MenuItem_Mnemonic_Get_ReturnsExpected(string text, char expected)
        {
            var menuItem = new MenuItem(text);
            Assert.Equal(expected, menuItem.Mnemonic);
        }

        [Fact]
        public void MenuItem_Mneumonic_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.Mnemonic);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuItem_OwnerDraw_Set_GetReturnsExpected(bool value)
        {
            var menuItem = new MenuItem
            {
                OwnerDraw = value
            };
            Assert.Equal(value, menuItem.OwnerDraw);
        }

        [Fact]
        public void MenuItem_OwnerDraw_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.OwnerDraw);
            Assert.Throws<ObjectDisposedException>(() => menuItem.OwnerDraw = true);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuItem_RadioCheck_Set_GetReturnsExpected(bool value)
        {
            var menuItem = new MenuItem
            {
                RadioCheck = value
            };
            Assert.Equal(value, menuItem.RadioCheck);
        }

        [Fact]
        public void MenuItem_RadioCheck_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.RadioCheck);
            Assert.Throws<ObjectDisposedException>(() => menuItem.RadioCheck = true);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuItem_ShowShortcut_Set_GetReturnsExpected(bool value)
        {
            var menuItem = new MenuItem
            {
                ShowShortcut = value
            };
            Assert.Equal(value, menuItem.ShowShortcut);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuItem_ShowShortcut_SetCreated_GetReturnsExpected(bool value)
        {
            var menuItem = new MenuItem();
            using (var menu = new SubMenu(new MenuItem[] { menuItem }))
            {
                menuItem.ShowShortcut = value;
                Assert.Equal(value, menuItem.ShowShortcut);
            }
        }

        [Fact]
        public void MenuItem_ShowShortcut_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.ShowShortcut);
            Assert.Throws<ObjectDisposedException>(() => menuItem.ShowShortcut = true);
        }

        [Theory]
        [InlineData(Shortcut.None)]
        [InlineData(Shortcut.Ctrl0)]
        public void MenuItem_Shortcut_Set_GetReturnsExpected(Shortcut value)
        {
            var menuItem = new MenuItem
            {
                Shortcut = value
            };
            Assert.Equal(value, menuItem.Shortcut);
        }

        [Theory]
        [InlineData(Shortcut.None)]
        [InlineData(Shortcut.Ctrl0)]
        public void MenuItem_Shortcut_SetCreated_GetReturnsExpected(Shortcut value)
        {
            var menuItem = new MenuItem();
            using (var menu = new SubMenu(new MenuItem[] { menuItem }))
            {
                menuItem.Shortcut = value;
                Assert.Equal(value, menuItem.Shortcut);
            }
        }

        [Theory]
        [InlineData((Shortcut)(Shortcut.None - 1))]
        public void MenuItem_Shortcut_SetInvalid_ThrowsInvalidEnumArgumentException(Shortcut value)
        {
            var menuItem = new MenuItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => menuItem.Shortcut = value);
        }

        [Fact]
        public void MenuItem_Shortcut_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.Shortcut);
            Assert.Throws<ObjectDisposedException>(() => menuItem.Shortcut = Shortcut.None);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void MenuItem_Text_Set_GetReturnsExpected(string value, string expected)
        {
            var menuItem = new MenuItem
            {
                Text = value
            };
            Assert.Equal(expected, menuItem.Text);

            // Set same.
            menuItem.Text = value;
            Assert.Equal(expected, menuItem.Text);
        }

        [Fact]
        public void MenuItem_Text_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.Text);
            Assert.Throws<ObjectDisposedException>(() => menuItem.Text = string.Empty);
        }

        [Fact]
        public void MenuItem_Visible_Set_GetReturnsExpected()
        {
            var menuItem = new MenuItem
            {
                Visible = false
            };
            Assert.False(menuItem.Visible);

            menuItem.Visible = true;
            Assert.True(menuItem.Visible);
        }

        [Fact]
        public void MenuItem_Visible_SetWithMainMenuParent_GetReturnsExpected()
        {
            var menuItem = new MenuItem();
            var menu = new MainMenu(new MenuItem[] { menuItem });
            menuItem.Visible = false;
            Assert.False(menuItem.Visible);

            menuItem.Visible = true;
            Assert.True(menuItem.Visible);
        }

        [Fact]
        public void MenuItem_Visible_SetWithHandle_GetReturnsExpected()
        {
            var menuItem = new MenuItem();
            using (var menu = new SubMenu(new MenuItem[] { menuItem }))
            {
                Assert.NotEqual(IntPtr.Zero, menu.Handle);
                menuItem.Visible = false;
                Assert.False(menuItem.Visible);

                menuItem.Visible = true;
                Assert.True(menuItem.Visible);
            }
        }

        [Fact]
        public void MenuItem_Visible_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.Visible);
            Assert.Throws<ObjectDisposedException>(() => menuItem.Visible = true);
        }

        [Fact]
        public void MenuItem_MenuID_Get_ReturnsExpected()
        {
            var menuItem = new SubMenuItem();
            Assert.NotEqual(0, menuItem.MenuID);
        }

        [Fact]
        public void MenuItem_MenuID_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new SubMenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.MenuID);
        }

        [Fact]
        public void MenuItem_OnClick_Invoke_Success()
        {
            var menuItem = new SubMenuItem();

            // No handler.
            menuItem.OnClick(null);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(menuItem, sender);
                callCount++;
            };

            menuItem.Click += handler;
            menuItem.OnClick(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            menuItem.Click -= handler;
            menuItem.OnClick(null);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void MenuItem_OnClick_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new SubMenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.OnClick(null));
        }

        [Fact]
        public void MenuItem_Click_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new SubMenuItem();
            menuItem.Dispose();
            EventHandler handler = (sender, e) => { };
            Assert.Throws<ObjectDisposedException>(() => menuItem.Click += handler);
            Assert.Throws<ObjectDisposedException>(() => menuItem.Click -= handler);
        }

        [Fact]
        public void MenuItem_OnDrawItem_Invoke_Success()
        {
            var menuItem = new SubMenuItem();

            // No handler.
            menuItem.OnDrawItem(null);

            // Handler.
            int callCount = 0;
            DrawItemEventHandler handler = (sender, e) =>
            {
                Assert.Equal(menuItem, sender);
                callCount++;
            };

            menuItem.DrawItem += handler;
            menuItem.OnDrawItem(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            menuItem.DrawItem -= handler;
            menuItem.OnDrawItem(null);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void MenuItem_OnDrawItem_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new SubMenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.OnDrawItem(null));
        }

        [Fact]
        public void MenuItem_DrawItem_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new SubMenuItem();
            menuItem.Dispose();
            DrawItemEventHandler handler = (sender, e) => { };
            Assert.Throws<ObjectDisposedException>(() => menuItem.DrawItem += handler);
            Assert.Throws<ObjectDisposedException>(() => menuItem.DrawItem -= handler);
        }

        [Fact]
        public void MenuItem_OnInitMenuPopup_Invoke_Success()
        {
            var menuItem = new SubMenuItem();

            // No handler.
            menuItem.OnInitMenuPopup(null);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(menuItem, sender);
                callCount++;
            };

            menuItem.Popup += handler;
            menuItem.OnInitMenuPopup(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            menuItem.Popup -= handler;
            menuItem.OnInitMenuPopup(null);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void MenuItem_OnInitMenuPopup_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new SubMenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.OnInitMenuPopup(null));
        }

        [Fact]
        public void MenuItem_OnMeasureItem_Invoke_Success()
        {
            var menuItem = new SubMenuItem();

            // No handler.
            menuItem.OnMeasureItem(null);

            // Handler.
            int callCount = 0;
            MeasureItemEventHandler handler = (sender, e) =>
            {
                Assert.Equal(menuItem, sender);
                callCount++;
            };

            menuItem.MeasureItem += handler;
            menuItem.OnMeasureItem(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            menuItem.MeasureItem -= handler;
            menuItem.OnMeasureItem(null);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void MenuItem_OnMeasureItem_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new SubMenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.OnMeasureItem(null));
        }

        [Fact]
        public void MenuItem_MeasureItem_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new SubMenuItem();
            menuItem.Dispose();
            MeasureItemEventHandler handler = (sender, e) => { };
            Assert.Throws<ObjectDisposedException>(() => menuItem.MeasureItem += handler);
            Assert.Throws<ObjectDisposedException>(() => menuItem.MeasureItem -= handler);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuItem_OnPopup_Invoke_Success(bool mdiList)
        {
            var menuItem = new SubMenuItem
            {
                MdiList = mdiList
            };

            // No handler.
            menuItem.OnPopup(null);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(menuItem, sender);
                callCount++;
            };

            menuItem.Popup += handler;
            menuItem.OnPopup(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            menuItem.Popup -= handler;
            menuItem.OnPopup(null);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void MenuItem_OnPopup_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new SubMenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.OnPopup(null));
        }

        [Fact]
        public void MenuItem_Popup_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new SubMenuItem();
            menuItem.Dispose();
            EventHandler handler = (sender, e) => { };
            Assert.Throws<ObjectDisposedException>(() => menuItem.Popup += handler);
            Assert.Throws<ObjectDisposedException>(() => menuItem.Popup -= handler);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void MenuItem_OnPopup_InvokeWithChildren_Success(bool mdiList)
        {
            var menuItem = new SubMenuItem("text", new MenuItem[] { new MenuItem("text") { MdiList = mdiList } });

            // No handler.
            menuItem.OnPopup(null);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(menuItem, sender);
                callCount++;
            };

            menuItem.Popup += handler;
            menuItem.OnPopup(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            menuItem.Popup -= handler;
            menuItem.OnPopup(null);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void MenuItem_OnPopup_MdiChildrenWithoutParent_DoesNotAddSeparator()
        {
            var menuItem = new SubMenuItem("text", new MenuItem[] { new MenuItem("child") }) { MdiList = true };

            menuItem.OnPopup(null);
            Assert.Equal(new string[] { "child" }, menuItem.MenuItems.Cast<MenuItem>().Select(m => m.Text));

            // Calling OnPopup again should not add duplicate items.
            menuItem.OnPopup(null);
            Assert.Equal(new string[] { "child" }, menuItem.MenuItems.Cast<MenuItem>().Select(m => m.Text));
        }

        public static IEnumerable<object[]> OnPopup_MdiChildren_TestData()
        {
            var formWithNoMdiChildren = new Form { Menu = new MainMenu() };
            formWithNoMdiChildren.Controls.Add(new MdiClient());
            yield return new object[] { new SubMenuItem("text", new MenuItem[] { new MenuItem("child") }) { MdiList = true }, new MainMenu(), new string[] { "child" } };
            yield return new object[] { new SubMenuItem("text") { MdiList = true }, formWithNoMdiChildren.Menu, Array.Empty<string>() };

            var formWithNoVisibleMdiChildren = new Form { Menu = new MainMenu() };
            var formWithNoVisibleMdiChildrenClient = new MdiClient();
            formWithNoVisibleMdiChildren.Controls.Add(formWithNoVisibleMdiChildrenClient);
            formWithNoVisibleMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithNoVisibleMdiChildren });
            yield return new object[] { new SubMenuItem("text", new MenuItem[] { new MenuItem("child") }) { MdiList = true }, formWithNoVisibleMdiChildren.Menu, new string[] { "child", "-" } };
            yield return new object[] { new SubMenuItem("text"), formWithNoVisibleMdiChildren.Menu, Array.Empty<string>() };

            var formWithMdiChildren = new Form { Menu = new MainMenu(), Visible = true };
            var formWithMdiChildrenClient = new MdiClient();
            formWithMdiChildren.Controls.Add(formWithMdiChildrenClient);
            formWithMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithMdiChildren, Visible = true, Text = "Form" });
            yield return new object[] { new SubMenuItem("text", new MenuItem[] { new MenuItem("child") }) { MdiList = true }, formWithMdiChildren.Menu, new string[] { "child", "-", "&1 Form", "&2 Form" } };

            var formWithManyMdiChildren = new Form { Menu = new MainMenu(), Visible = true };
            var formWithManyMdiChildrenClient = new MdiClient();
            formWithManyMdiChildren.Controls.Add(formWithManyMdiChildrenClient);
            formWithManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithManyMdiChildren, Visible = true, Text = "Form1" });
            formWithManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithManyMdiChildren, Visible = true, Text = "Form2" });
            formWithManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithManyMdiChildren, Visible = true, Text = "Form3" });
            formWithManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithManyMdiChildren, Visible = true, Text = "Form4" });
            formWithManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithManyMdiChildren, Visible = true, Text = "Form5" });
            formWithManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithManyMdiChildren, Visible = true, Text = "Form6" });
            formWithManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithManyMdiChildren, Visible = true, Text = "Form7" });
            formWithManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithManyMdiChildren, Visible = true, Text = "Form8" });
            formWithManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithManyMdiChildren, Visible = true, Text = "Form9" });
            formWithManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithManyMdiChildren, Visible = true, Text = "Form10" });
            yield return new object[] { new SubMenuItem("text", new MenuItem[] { new MenuItem("child") }) { MdiList = true }, formWithManyMdiChildren.Menu, new string[] { "child", "-", "&1 Form1", "&2 Form1", "&3 Form2", "&4 Form2", "&5 Form3", "&6 Form3", "&7 Form4", "&8 Form4", "&9 Form10", "&10 Form10", "&More Windows..." } };

            var formWithActiveMdiChildren = new SubForm { Menu = new MainMenu(), Visible = true };
            var formWithActiveMdiChildrenClient = new MdiClient();
            formWithActiveMdiChildren.Controls.Add(formWithActiveMdiChildrenClient);
            var activeForm1 = new Form { MdiParent = formWithActiveMdiChildren, Visible = true, Text = "Form2" };
            formWithActiveMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithActiveMdiChildren, Visible = true, Text = "Form1" });
            formWithActiveMdiChildrenClient.Controls.Add(activeForm1);
            formWithActiveMdiChildren.ActivateMdiChild(activeForm1);
            yield return new object[] { new SubMenuItem("text", new MenuItem[] { new MenuItem("child") }) { MdiList = true }, formWithActiveMdiChildren.Menu, new string[] { "child", "-", "&1 Form2", "&2 Form1", "&3 Form1", "&4 Form2" } };

            var formWithActiveManyMdiChildren = new SubForm { Menu = new MainMenu(), Visible = true };
            var formWithActiveManyMdiChildrenClient = new MdiClient();
            formWithActiveManyMdiChildren.Controls.Add(formWithActiveManyMdiChildrenClient);
            var activeForm2 = new Form { MdiParent = formWithActiveManyMdiChildren, Visible = true, Text = "Form11" };
            formWithActiveManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithActiveManyMdiChildren, Visible = true, Text = "Form1" });
            formWithActiveManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithActiveManyMdiChildren, Visible = true, Text = "Form2" });
            formWithActiveManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithActiveManyMdiChildren, Visible = true, Text = "Form3" });
            formWithActiveManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithActiveManyMdiChildren, Visible = true, Text = "Form4" });
            formWithActiveManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithActiveManyMdiChildren, Visible = true, Text = "Form5" });
            formWithActiveManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithActiveManyMdiChildren, Visible = true, Text = "Form6" });
            formWithActiveManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithActiveManyMdiChildren, Visible = true, Text = "Form7" });
            formWithActiveManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithActiveManyMdiChildren, Visible = true, Text = "Form8" });
            formWithActiveManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithActiveManyMdiChildren, Visible = true, Text = "Form9" });
            formWithActiveManyMdiChildrenClient.Controls.Add(new Form { MdiParent = formWithActiveManyMdiChildren, Visible = true, Text = "Form10" });
            formWithActiveManyMdiChildrenClient.Controls.Add(activeForm2);
            formWithActiveManyMdiChildren.ActivateMdiChild(activeForm2);
            yield return new object[] { new SubMenuItem("text", new MenuItem[] { new MenuItem("child") }) { MdiList = true }, formWithActiveManyMdiChildren.Menu, new string[] { "child", "-", "&1 Form11", "&2 Form1", "&3 Form1", "&4 Form2", "&5 Form2", "&6 Form3", "&7 Form3", "&8 Form4", "&9 Form4", "&10 Form11", "&More Windows..." } };
        }

        [Theory]
        [MemberData(nameof(OnPopup_MdiChildren_TestData))]
        public void MenuItem_OnPopup_MdiChildren_AddsSeparator(SubMenuItem menuItem, Menu parent, string[] expectedItems)
        {
            try
            {
                parent.MenuItems.Add(menuItem);

                menuItem.OnPopup(null);
                Assert.Equal(expectedItems, menuItem.MenuItems.Cast<MenuItem>().Select(m => m.Text));

                // Calling OnPopup again should not add duplicate items.
                menuItem.OnPopup(null);
                Assert.Equal(expectedItems, menuItem.MenuItems.Cast<MenuItem>().Select(m => m.Text));
            }
            catch
            {
                string expected = "Expected: " + string.Join(", ", expectedItems.Select(i => "\"" + i + "\""));
                string actual = "Actual:   " + string.Join(", ", menuItem.MenuItems.Cast<MenuItem>().Select(m => m.Text).Select(i => "\"" + i + "\""));
                throw new Exception(expected + Environment.NewLine + actual);
            }
        }

        [Fact]
        public void MenuItem_PerformClick_MdiSeparator_Nop()
        {
            var parentForm = new Form { Menu = new MainMenu() };
            var parentFormClient = new MdiClient();
            parentForm.Controls.Add(parentFormClient);
            parentFormClient.Controls.Add(new Form { MdiParent = parentForm });
            var menuItem = new SubMenuItem("text", new MenuItem[] { new MenuItem() }) { MdiList = true };
            parentForm.Menu.MenuItems.Add(menuItem);
            menuItem.OnPopup(null);

            MenuItem separator = menuItem.MenuItems[1];
            Assert.Equal("-", separator.Text);
            separator.PerformClick();
        }

        [Fact]
        public void MenuItem_OnSelect_Invoke_Success()
        {
            var menuItem = new SubMenuItem();

            // No handler.
            menuItem.OnSelect(null);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(menuItem, sender);
                callCount++;
            };

            menuItem.Select += handler;
            menuItem.OnSelect(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            menuItem.Select -= handler;
            menuItem.OnSelect(null);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void MenuItem_OnSelect_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new SubMenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.OnSelect(null));
        }

        [Fact]
        public void MenuItem_Select_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new SubMenuItem();
            menuItem.Dispose();
            EventHandler handler = (sender, e) => { };
            Assert.Throws<ObjectDisposedException>(() => menuItem.Select += handler);
            Assert.Throws<ObjectDisposedException>(() => menuItem.Select -= handler);
        }

        [Fact]
        public void MenuItem_PerformClick_Invoke_Success()
        {
            var menuItem = new SubMenuItem();

            // No handler.
            menuItem.PerformClick();

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(EventArgs.Empty, e);
                Assert.Equal(menuItem, sender);
                callCount++;
            };

            menuItem.Click += handler;
            menuItem.PerformClick();
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            menuItem.Click -= handler;
            menuItem.PerformClick();
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void MenuItem_PerformSelect_Invoke_Success()
        {
            var menuItem = new SubMenuItem();

            // No handler.
            menuItem.PerformSelect();

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(EventArgs.Empty, e);
                Assert.Equal(menuItem, sender);
                callCount++;
            };

            menuItem.Select += handler;
            menuItem.PerformSelect();
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            menuItem.Select -= handler;
            menuItem.PerformSelect();
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> CloneMenu_TestData()
        {
            yield return new object[] { Array.Empty<MenuItem>() };
            yield return new object[] { new MenuItem[] { new MenuItem("text") } };
        }

        [Theory]
        [MemberData(nameof(CloneMenu_TestData))]
        public void MenuItem_CloneMenu_InvokeNew_Success(MenuItem[] items)
        {
            var source = new MenuItem("text", items)
            {
                BarBreak = true,
                Break = true,
                Checked = items.Length == 0,
                Enabled = false,
                MdiList = true,
                MergeOrder = -1,
                MergeType = MenuMerge.Remove,
                Name = "name",
                OwnerDraw = true,
                RadioCheck = true,
                ShowShortcut = false,
                Shortcut = Shortcut.CtrlA,
                Site = Mock.Of<ISite>(),
                Tag = "tag",
                Visible = false
            };
            var menu = new SubMenu(new MenuItem[] { new MenuItem("parent", new MenuItem[] { source }) });
            MenuItem menuItem = source.CloneMenu();
            Assert.NotSame(source, menuItem);

            Assert.Equal(source.BarBreak, menuItem.BarBreak);
            Assert.Equal(source.Break, menuItem.Break);
            Assert.Equal(source.Checked, menuItem.Checked);
            Assert.Equal(source.DefaultItem, menuItem.DefaultItem);
            Assert.Equal(source.Enabled, menuItem.Enabled);
            Assert.Equal(-1, menuItem.Index);
            Assert.Equal(source.IsParent, menuItem.IsParent);
            Assert.Equal(source.MdiList, menuItem.MdiList);
            MenuTests.AssertEqualMenuItems(items, menuItem.MenuItems.Cast<MenuItem>().ToArray());
            for (int i = 0; i < items.Length; i++)
            {
                Assert.Equal(i, menuItem.MenuItems[i].Index);
                Assert.Equal(menuItem, menuItem.MenuItems[i].Parent);
            }
            Assert.Equal(source.MergeOrder, menuItem.MergeOrder);
            Assert.Equal(source.MergeType, menuItem.MergeType);
            Assert.Equal(source.Mnemonic, menuItem.Mnemonic);
            Assert.Equal(source.OwnerDraw, menuItem.OwnerDraw);
            Assert.Null(menuItem.Parent);
            Assert.Equal(source.RadioCheck, menuItem.RadioCheck);
            Assert.Equal(source.ShowShortcut, menuItem.ShowShortcut);
            Assert.Equal(source.Shortcut, menuItem.Shortcut);
            Assert.Equal(source.Visible, menuItem.Visible);
            Assert.Empty(menuItem.Name);
            Assert.Null(menuItem.Site);
            Assert.Null(menuItem.Container);
            Assert.Null(menuItem.Tag);
            Assert.Equal(source.Text, menuItem.Text);
        }

        [Theory]
        [MemberData(nameof(CloneMenu_TestData))]
        public void MenuItem_CloneMenu_InvokeExisting_Success(MenuItem[] items)
        {
            var source = new MenuItem("text", items)
            {
                BarBreak = true,
                Break = true,
                Checked = items.Length == 0,
                Enabled = false,
                MdiList = true,
                MergeOrder = -1,
                MergeType = MenuMerge.Remove,
                OwnerDraw = true,
                Site = Mock.Of<ISite>(),
                Tag = "tag"
            };
            var menu = new SubMenu(new MenuItem[] { new MenuItem("parent", new MenuItem[] { source }) });
            var menuItem = new SubMenuItem();
            menuItem.CloneMenu(source);

            Assert.False(menuItem.BarBreak);
            Assert.False(menuItem.Break);
            Assert.False(menuItem.Checked);
            Assert.False(menuItem.DefaultItem);
            Assert.True(menuItem.Enabled);
            Assert.Equal(-1, menuItem.Index);
            Assert.Equal(source.IsParent, menuItem.IsParent);
            Assert.False(menuItem.MdiList);
            MenuTests.AssertEqualMenuItems(items, menuItem.MenuItems.Cast<MenuItem>().ToArray());
            for (int i = 0; i < items.Length; i++)
            {
                Assert.Equal(i, menuItem.MenuItems[i].Index);
                Assert.Equal(menuItem, menuItem.MenuItems[i].Parent);
            }
            Assert.Equal(0, menuItem.MergeOrder);
            Assert.Equal(MenuMerge.Add, menuItem.MergeType);
            Assert.Equal('\0', menuItem.Mnemonic);
            Assert.False(menuItem.OwnerDraw);
            Assert.Null(menuItem.Parent);
            Assert.False(menuItem.RadioCheck);
            Assert.True(menuItem.ShowShortcut);
            Assert.Equal(Shortcut.None, menuItem.Shortcut);
            Assert.True(menuItem.Visible);
            Assert.Empty(menuItem.Name);
            Assert.Null(menuItem.Site);
            Assert.Null(menuItem.Container);
            Assert.Null(menuItem.Tag);
            Assert.Empty(menuItem.Text);
        }

        [Fact]
        public void MenuItem_CloneMenu_NullMenuSource_ThrowsArgumentNullException()
        {
            var menu = new SubMenuItem("text");
            Assert.Throws<ArgumentNullException>("menuSrc", () => menu.CloneMenu(null));
        }

        [Theory]
        [MemberData(nameof(CloneMenu_TestData))]
        public void MenuItem_MergeMenu_InvokeNew_Success(MenuItem[] items)
        {
            var source = new MenuItem("text", items)
            {
                BarBreak = true,
                Break = true,
                Checked = items.Length == 0,
                Enabled = false,
                MdiList = true,
                MergeOrder = -1,
                MergeType = MenuMerge.Remove,
                Name = "name",
                OwnerDraw = true,
                RadioCheck = true,
                ShowShortcut = false,
                Shortcut = Shortcut.CtrlA,
                Site = Mock.Of<ISite>(),
                Tag = "tag",
                Visible = false
            };
            var menu = new SubMenu(new MenuItem[] { new MenuItem("parent", new MenuItem[] { source }) });
            MenuItem menuItem = source.MergeMenu();
            Assert.NotSame(source, menuItem);

            Assert.Equal(source.BarBreak, menuItem.BarBreak);
            Assert.Equal(source.Break, menuItem.Break);
            Assert.Equal(source.Checked, menuItem.Checked);
            Assert.Equal(source.DefaultItem, menuItem.DefaultItem);
            Assert.Equal(source.Enabled, menuItem.Enabled);
            Assert.Equal(-1, menuItem.Index);
            Assert.Equal(source.IsParent, menuItem.IsParent);
            Assert.Equal(source.MdiList, menuItem.MdiList);
            MenuTests.AssertEqualMenuItems(items, menuItem.MenuItems.Cast<MenuItem>().ToArray());
            for (int i = 0; i < items.Length; i++)
            {
                Assert.Equal(i, menuItem.MenuItems[i].Index);
                Assert.Equal(menuItem, menuItem.MenuItems[i].Parent);
            }
            Assert.Equal(source.MergeOrder, menuItem.MergeOrder);
            Assert.Equal(source.MergeType, menuItem.MergeType);
            Assert.Equal(source.Mnemonic, menuItem.Mnemonic);
            Assert.Equal(source.OwnerDraw, menuItem.OwnerDraw);
            Assert.Null(menuItem.Parent);
            Assert.Equal(source.RadioCheck, menuItem.RadioCheck);
            Assert.Equal(source.ShowShortcut, menuItem.ShowShortcut);
            Assert.Equal(source.Shortcut, menuItem.Shortcut);
            Assert.Equal(source.Visible, menuItem.Visible);
            Assert.Empty(menuItem.Name);
            Assert.Null(menuItem.Site);
            Assert.Null(menuItem.Container);
            Assert.Null(menuItem.Tag);
            Assert.Equal(source.Text, menuItem.Text);
        }

        [Theory]
        [MemberData(nameof(CloneMenu_TestData))]
        public void MenuItem_MergeMenu_InvokeExisting_Success(MenuItem[] items)
        {
            var source = new MenuItem("text", items)
            {
                BarBreak = true,
                Break = true,
                Checked = items.Length == 0,
                Enabled = false,
                MdiList = true,
                MergeOrder = -1,
                MergeType = MenuMerge.Remove,
                Name = "name",
                OwnerDraw = true,
                RadioCheck = true,
                ShowShortcut = false,
                Shortcut = Shortcut.CtrlA,
                Site = Mock.Of<ISite>(),
                Tag = "tag",
                Visible = false
            };
            var menu = new SubMenu(new MenuItem[] { new MenuItem("parent", new MenuItem[] { source }) });
            var menuItem = new MenuItem();
            menuItem.MergeMenu(source);

            Assert.Equal(source.BarBreak, menuItem.BarBreak);
            Assert.Equal(source.Break, menuItem.Break);
            Assert.Equal(source.Checked, menuItem.Checked);
            Assert.Equal(source.DefaultItem, menuItem.DefaultItem);
            Assert.Equal(source.Enabled, menuItem.Enabled);
            Assert.Equal(-1, menuItem.Index);
            Assert.Equal(source.IsParent, menuItem.IsParent);
            Assert.Equal(source.MdiList, menuItem.MdiList);
            MenuTests.AssertEqualMenuItems(items, menuItem.MenuItems.Cast<MenuItem>().ToArray());
            for (int i = 0; i < items.Length; i++)
            {
                Assert.Equal(i, menuItem.MenuItems[i].Index);
                Assert.Equal(menuItem, menuItem.MenuItems[i].Parent);
            }
            Assert.Equal(source.MergeOrder, menuItem.MergeOrder);
            Assert.Equal(source.MergeType, menuItem.MergeType);
            Assert.Equal(source.Mnemonic, menuItem.Mnemonic);
            Assert.Equal(source.OwnerDraw, menuItem.OwnerDraw);
            Assert.Null(menuItem.Parent);
            Assert.Equal(source.RadioCheck, menuItem.RadioCheck);
            Assert.Equal(source.ShowShortcut, menuItem.ShowShortcut);
            Assert.Equal(source.Shortcut, menuItem.Shortcut);
            Assert.Equal(source.Visible, menuItem.Visible);
            Assert.Empty(menuItem.Name);
            Assert.Null(menuItem.Site);
            Assert.Null(menuItem.Container);
            Assert.Null(menuItem.Tag);
            Assert.Equal(source.Text, menuItem.Text);
        }

        [Fact]
        public void MenuItem_MergeMenu_OnClick_Success()
        {
            var source = new SubMenuItem();

            int sourceCallCount = 0;
            source.Click += (sender, e) =>
            {
                sourceCallCount++;
                Assert.Equal(source, sender);
                Assert.Equal(EventArgs.Empty, e);
            };

            SubMenuItem destination = Assert.IsType<SubMenuItem>(source.MergeMenu());
            int destinationCallCount = 0;
            destination.Click += (sender, e) =>
            {
                destinationCallCount++;
                Assert.Equal(source, sender);
                Assert.Equal(EventArgs.Empty, e);
            };

            destination.OnClick(EventArgs.Empty);
            Assert.Equal(1, sourceCallCount);
            Assert.Equal(1, destinationCallCount);

            source.OnClick(EventArgs.Empty);
            Assert.Equal(2, sourceCallCount);
            Assert.Equal(2, destinationCallCount);
        }

        [Fact]
        public void MenuItem_MergeMenu_OnDrawItem_Success()
        {
            var source = new SubMenuItem();

            int sourceCallCount = 0;
            source.DrawItem += (sender, e) =>
            {
                sourceCallCount++;
                Assert.Equal(source, sender);
                Assert.Null(e);
            };

            SubMenuItem destination = Assert.IsType<SubMenuItem>(source.MergeMenu());
            int destinationCallCount = 0;
            destination.DrawItem += (sender, e) =>
            {
                destinationCallCount++;
                Assert.Equal(source, sender);
                Assert.Null(e);
            };

            destination.OnDrawItem(null);
            Assert.Equal(1, sourceCallCount);
            Assert.Equal(1, destinationCallCount);

            source.OnDrawItem(null);
            Assert.Equal(2, sourceCallCount);
            Assert.Equal(2, destinationCallCount);
        }

        [Fact]
        public void MenuItem_MergeMenu_OnMeasureItem_Success()
        {
            var source = new SubMenuItem();

            int sourceCallCount = 0;
            source.MeasureItem += (sender, e) =>
            {
                sourceCallCount++;
                Assert.Equal(source, sender);
                Assert.Null(e);
            };

            SubMenuItem destination = Assert.IsType<SubMenuItem>(source.MergeMenu());
            int destinationCallCount = 0;
            destination.MeasureItem += (sender, e) =>
            {
                destinationCallCount++;
                Assert.Equal(source, sender);
                Assert.Null(e);
            };

            destination.OnMeasureItem(null);
            Assert.Equal(1, sourceCallCount);
            Assert.Equal(1, destinationCallCount);

            source.OnMeasureItem(null);
            Assert.Equal(2, sourceCallCount);
            Assert.Equal(2, destinationCallCount);
        }

        [Fact]
        public void MenuItem_MergeMenu_OnPopup_Success()
        {
            var source = new SubMenuItem();

            int sourceCallCount = 0;
            source.Popup += (sender, e) =>
            {
                sourceCallCount++;
                Assert.Equal(source, sender);
                Assert.Equal(EventArgs.Empty, e);
            };

            SubMenuItem destination = Assert.IsType<SubMenuItem>(source.MergeMenu());
            int destinationCallCount = 0;
            destination.Popup += (sender, e) =>
            {
                destinationCallCount++;
                Assert.Equal(source, sender);
                Assert.Equal(EventArgs.Empty, e);
            };

            destination.OnPopup(EventArgs.Empty);
            Assert.Equal(1, sourceCallCount);
            Assert.Equal(1, destinationCallCount);

            source.OnPopup(EventArgs.Empty);
            Assert.Equal(2, sourceCallCount);
            Assert.Equal(2, destinationCallCount);
        }

        [Fact]
        public void MenuItem_MergeMenu_OnSelect_Success()
        {
            var source = new SubMenuItem();

            int sourceCallCount = 0;
            source.Select += (sender, e) =>
            {
                sourceCallCount++;
                Assert.Equal(source, sender);
                Assert.Equal(EventArgs.Empty, e);
            };

            SubMenuItem destination = Assert.IsType<SubMenuItem>(source.MergeMenu());
            int destinationCallCount = 0;
            destination.Select += (sender, e) =>
            {
                destinationCallCount++;
                Assert.Equal(source, sender);
                Assert.Equal(EventArgs.Empty, e);
            };

            destination.OnSelect(EventArgs.Empty);
            Assert.Equal(1, sourceCallCount);
            Assert.Equal(1, destinationCallCount);

            source.OnSelect(EventArgs.Empty);
            Assert.Equal(2, sourceCallCount);
            Assert.Equal(2, destinationCallCount);
        }

        [Fact]
        public void MenuItem_MergeMenu_NullMenuSource_ThrowsArgumentNullException()
        {
            var menu = new MenuItem("text");
            Assert.Throws<ArgumentNullException>("menuSrc", () => menu.MergeMenu(null));
        }

        [Fact]
        public void MenuItem_MergeMenu_Disposed_ThrowsObjectDisposedException()
        {
            var menuItem = new MenuItem();
            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.MergeMenu());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void MenuItem_ToString_Invoke_ReturnsExpected(string text)
        {
            var menuItem = new MenuItem
            {
                Text = text
            };
            Assert.Equal("System.Windows.Forms.MenuItem, Items.Count: 0, Text: " + text, menuItem.ToString());
        }

        [Fact]
        public void MenuItem_Dispose_NoParentOrChildren_Success()
        {
            var menuItem = new MenuItem("text", Array.Empty<MenuItem>());
            menuItem.Dispose();
            menuItem.Dispose();
        }

        [Fact]
        public void MenuItem_Dispose_NoParent_Success()
        {
            var childMenuItem = new MenuItem();
            var menuItem = new MenuItem("text", new MenuItem[] { childMenuItem });
            menuItem.Dispose();
            Assert.Null(childMenuItem.Parent);
            Assert.Empty(menuItem.MenuItems);

            menuItem.Dispose();
            Assert.Empty(menuItem.MenuItems);
        }

        [Fact]
        public void MenuItem_Dispose_HasParent_Success()
        {
            var childMenuItem = new MenuItem();
            var menuItem = new MenuItem("text", new MenuItem[] { childMenuItem });
            childMenuItem.Dispose();
            Assert.Null(childMenuItem.Parent);
            Assert.Empty(menuItem.MenuItems);

            childMenuItem.Dispose();
            Assert.Empty(menuItem.MenuItems);
        }

        [Fact]
        public void MenuItem_Dispose_HasMenuId_Success()
        {
            var menuItem = new SubMenuItem();
            Assert.NotEqual(0, menuItem.MenuID);

            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.MenuID);

            menuItem.Dispose();
            Assert.Throws<ObjectDisposedException>(() => menuItem.MenuID);
        }

        public class SubMenuItem : MenuItem
        {
            public SubMenuItem()
            {
            }

            public SubMenuItem(string text) : base(text)
            {
            }

            public SubMenuItem(string text, EventHandler onClick) : base(text, onClick)
            {
            }

            public SubMenuItem(string text, MenuItem[] items) : base(text, items)
            {
            }

            public SubMenuItem(string text, EventHandler onClick, Shortcut shortcut) : base(text, onClick, shortcut)
            {
            }

            public SubMenuItem(MenuMerge mergeType, int mergeOrder, Shortcut shortcut, string text, EventHandler onClick, EventHandler onPopup, EventHandler onSelect, MenuItem[] items) : base(mergeType, mergeOrder, shortcut, text, onClick, onPopup, onSelect, items)
            {
            }

            public new int MenuID => base.MenuID;

            public new void OnClick(EventArgs e) => base.OnClick(e);

            public new void OnDrawItem(DrawItemEventArgs e) => base.OnDrawItem(e);

            public new void OnInitMenuPopup(EventArgs e) => base.OnInitMenuPopup(e);

            public new void OnMeasureItem(MeasureItemEventArgs e) => base.OnMeasureItem(e);

            public new void OnPopup(EventArgs e) => base.OnPopup(e);

            public new void OnSelect(EventArgs e) => base.OnSelect(e);

            public new void CloneMenu(Menu menuSrc) => base.CloneMenu(menuSrc);
        }

        private class SubMenu : Menu
        {
            public SubMenu(params MenuItem[] items) : base(items)
            {
            }
        }

        private class SubForm : Form
        {
            public new void ActivateMdiChild(Form form) => base.ActivateMdiChild(form);
        }
    }
}
