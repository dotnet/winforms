// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ContextMenuTests
    {
        [Fact]
        public void ContextMenu_Ctor_Default()
        {
            var menu = new ContextMenu();
            Assert.Empty(menu.MenuItems);
            Assert.False(menu.IsParent);
            Assert.Equal(RightToLeft.No, menu.RightToLeft);
            Assert.Null(menu.SourceControl);
            Assert.Empty(menu.Name);
            Assert.Null(menu.Site);
            Assert.Null(menu.Container);
            Assert.Null(menu.Tag);
        }

        public static IEnumerable<object[]> Ctor_MenuItemArray_TestData()
        {
            yield return new object[] { null, false };
            yield return new object[] { Array.Empty<MenuItem>(), false };
            yield return new object[] { new MenuItem[] { new MenuItem() }, true };
        }

        [Theory]
        [MemberData(nameof(Ctor_MenuItemArray_TestData))]
        public void ContextMenu_Ctor_MenuItemArray(MenuItem[] items, bool expectedIsParent)
        {
            var menu = new ContextMenu(items);
            Assert.Equal(items ?? Array.Empty<MenuItem>(), menu.MenuItems.Cast<MenuItem>());
            for (int i = 0; i < (items?.Length ?? 0); i++)
            {
                Assert.Equal(i, menu.MenuItems[i].Index);
                Assert.Equal(menu, menu.MenuItems[i].Parent);
            }
            Assert.Equal(expectedIsParent, menu.IsParent);
            Assert.Equal(RightToLeft.No, menu.RightToLeft);
            Assert.Null(menu.SourceControl);
            Assert.Empty(menu.Name);
            Assert.Null(menu.Site);
            Assert.Null(menu.Container);
            Assert.Null(menu.Tag);
        }

        [Fact]
        public void ContextMenu_Ctor_NullItemInMenuItemArray_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("item", () => new ContextMenu(new MenuItem[] { null }));
        }

        [Fact]
        public void ContextMenu_SourceControl_GetProcessingKeyMessage_Succes()
        {
            var control = new Control
            {
                ContextMenu = new ContextMenu()
            };

            ContextMenu menu = control.ContextMenu;
            Assert.Null(menu.SourceControl);

            var message = new Message
            {
                Msg = 0x0100 // WM_KEYDOWN
            };
            control.PreProcessMessage(ref message);
            Assert.Same(control, menu.SourceControl);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void ContextMenu_RightToLeft_SetWithoutSourceControl_GetReturnsExpected(RightToLeft value, RightToLeft expectedValue)
        {
            var menu = new ContextMenu
            {
                RightToLeft = value
            };
            Assert.Equal(expectedValue, menu.RightToLeft);
        }

        [Theory]
        [InlineData(RightToLeft.Yes, RightToLeft.Yes)]
        [InlineData(RightToLeft.No, RightToLeft.No)]
        [InlineData(RightToLeft.Inherit, RightToLeft.Yes)]
        public void ContextMenu_RightToLeft_SetWithSourceControl_GetReturnsExpected(RightToLeft value, RightToLeft expectedValue)
        {
            var control = new Control
            {
                ContextMenu = new ContextMenu(),
                RightToLeft = RightToLeft.Yes
            };

            ContextMenu menu = control.ContextMenu;
            var message = new Message
            {
                Msg = 0x0100 // WM_KEYDOWN
            };
            control.PreProcessMessage(ref message);

            menu.RightToLeft = value;
            Assert.Equal(expectedValue, menu.RightToLeft);
        }

        [Fact]
        public void ContextMenu_RightToLeft_SetCreated_GetReturnsExpected()
        {
            using (var menu = new ContextMenu(new MenuItem[] { new MenuItem("text") }))
            {
                Assert.NotEqual(IntPtr.Zero, menu.Handle);
                menu.RightToLeft = RightToLeft.Yes;
                menu.RightToLeft = RightToLeft.No;
                Assert.Equal(RightToLeft.No, menu.RightToLeft);
            }
        }

        [Theory]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(RightToLeft), MemberType = typeof(CommonTestHelper))]
        public void ContextMenu_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            var menu = new ContextMenu();
            Assert.Throws<InvalidEnumArgumentException>("RightToLeft", () => menu.RightToLeft = value);
        }

        public static IEnumerable<object[]> ProcessCmdKey_TestData()
        {
            yield return new object[] { new MenuItem { Shortcut = Shortcut.CtrlA }, true, 1, 0 };
            yield return new object[] { new MenuItem { Shortcut = Shortcut.CtrlA, Enabled = false }, false, 0, 0 };
            yield return new object[] { new MenuItem("text", new MenuItem[] { new MenuItem() }) { Shortcut = Shortcut.CtrlA }, true, 0, 1 };
            yield return new object[] { new MenuItem("text", new MenuItem[] { new MenuItem() }) { Shortcut = Shortcut.CtrlA, Enabled = false }, false, 0, 0 };
        }

        [Theory]
        [MemberData(nameof(ProcessCmdKey_TestData))]
        public void ContextMenu_ProcessCmdKey_HasItemWithShoutcutKey_ReturnsExpected(MenuItem menuItem, bool expectedResult, int expectedOnClickCallCount, int expectedOnPopupCallCount)
        {
            var control = new Control();
            int onClickCallCount = 0;
            menuItem.Click += (sender, e) =>
            {
                onClickCallCount++;
                Assert.Same(menuItem, sender);
                Assert.Equal(EventArgs.Empty, e);
            };

            int onPopupCallCount = 0;
            menuItem.Popup += (sender, e) =>
            {
                onPopupCallCount++;
                Assert.Same(menuItem, sender);
                Assert.Equal(EventArgs.Empty, e);
            };

            var menu = new ContextMenu(new MenuItem[] { menuItem });
            var message = new Message();
            Assert.Equal(expectedResult, menu.ProcessCmdKey(ref message, Keys.Control | Keys.A, control));
            Assert.Same(control, menu.SourceControl);
            Assert.Equal(expectedOnClickCallCount, onClickCallCount);
            Assert.Equal(expectedOnPopupCallCount, onPopupCallCount);
        }

        [Fact]
        public void ContextMenu_Show_ControlPoint_Success()
        {
            var menu = new ContextMenu();
            var control = new Control
            {
                Visible = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(menu, sender);
                callCount++;
            };
            menu.Popup += handler;

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            menu.Show(control, new Point(1, 2));
            Assert.Equal(1, callCount);
            Assert.Same(control, menu.SourceControl);
        }

        [Theory]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(LeftRightAlignment), MemberType = typeof(CommonTestHelper))]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(LeftRightAlignment), MemberType = typeof(CommonTestHelper))]
        public void ContextMenu_Show_ControlPointLeftRightAlignment_Success(LeftRightAlignment alignment)
        {
            var menu = new ContextMenu();
            var control = new Control
            {
                Visible = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(menu, sender);
                callCount++;
            };
            menu.Popup += handler;

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            menu.Show(control, new Point(1, 2), alignment);
            Assert.Equal(1, callCount);
            Assert.Same(control, menu.SourceControl);
        }

        [Fact]
        public void ContextMenu_Show_NullControl_ThrowsArgumentNullException()
        {
            var menu = new ContextMenu();
            Assert.Throws<ArgumentNullException>("control", () => menu.Show(null, new Point(1, 2)));
            Assert.Throws<ArgumentNullException>("control", () => menu.Show(null, new Point(1, 2), LeftRightAlignment.Left));
        }

        [Fact]
        public void ContextMenu_Show_NotVisibleControl_ThrowsArgumentException()
        {
            var menu = new ContextMenu();
            var control = new Control
            {
                Visible = false
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Throws<ArgumentException>("control", () => menu.Show(control, new Point(1, 2)));
            Assert.Throws<ArgumentException>("control", () => menu.Show(control, new Point(1, 2), LeftRightAlignment.Left));
        }

        [Fact]
        public void ContextMenu_Show_ControlHasNoHandle_ThrowsArgumentException()
        {
            var menu = new ContextMenu();
            var control = new Control
            {
                Visible = true
            };
            Assert.Throws<ArgumentException>("control", () => menu.Show(control, new Point(1, 2)));
            Assert.Throws<ArgumentException>("control", () => menu.Show(control, new Point(1, 2), LeftRightAlignment.Left));
        }

        [Fact]
        public void ContextMenu_OnCollapse_Invoke_Success()
        {
            var menu = new ContextMenu();

            // No handler.
            menu.OnCollapse(null);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(menu, sender);
                callCount++;
            };

            menu.Collapse += handler;
            menu.OnCollapse(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            menu.Collapse -= handler;
            menu.OnCollapse(null);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void ContextMenu_OnPopup_Invoke_Success()
        {
            var menu = new ContextMenu();

            // No handler.
            menu.OnPopup(null);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(menu, sender);
                callCount++;
            };

            menu.Popup += handler;
            menu.OnPopup(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            menu.Popup -= handler;
            menu.OnPopup(null);
            Assert.Equal(1, callCount);
        }
    }
}
