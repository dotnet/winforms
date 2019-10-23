// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ContextMenuTests
    {
        [WinFormsFact]
        public void ContextMenu_Ctor_Default()
        {
            using var menu = new SubContextMenu();
            Assert.True(menu.CanRaiseEvents);
            Assert.Null(menu.Container);
            Assert.False(menu.DesignMode);
            Assert.NotNull(menu.Events);
            Assert.Same(menu.Events, menu.Events);
            Assert.False(menu.IsParent);
            Assert.Empty(menu.MenuItems);
            Assert.Same(menu.MenuItems, menu.MenuItems);
            Assert.Empty(menu.Name);
            Assert.Equal(RightToLeft.No, menu.RightToLeft);
            Assert.Null(menu.Site);
            Assert.Null(menu.SourceControl);
            Assert.Null(menu.Tag);
        }

        public static IEnumerable<object[]> Ctor_MenuItemArray_TestData()
        {
            var menuItem = new MenuItem();
            yield return new object[] { null, false, Array.Empty<MenuItem>() };
            yield return new object[] { Array.Empty<MenuItem>(), false, Array.Empty<MenuItem>() };
            yield return new object[] { new MenuItem[] { menuItem }, true, new MenuItem[] { menuItem } };
        }

        [Theory]
        [MemberData(nameof(Ctor_MenuItemArray_TestData))]
        public void ContextMenu_Ctor_MenuItemArray(MenuItem[] items, bool expectedIsParent, MenuItem[] expectedItems)
        {
            using var menu = new SubContextMenu(items);
            Assert.True(menu.CanRaiseEvents);
            Assert.Null(menu.Container);
            Assert.False(menu.DesignMode);
            Assert.NotNull(menu.Events);
            Assert.Same(menu.Events, menu.Events);
            Assert.Equal(expectedIsParent, menu.IsParent);
            Assert.Equal(expectedItems, menu.MenuItems.Cast<MenuItem>());
            for (int i = 0; i < menu.MenuItems.Count; i++)
            {
                Assert.Equal(i, menu.MenuItems[i].Index);
                Assert.Equal(menu, menu.MenuItems[i].Parent);
            }
            Assert.Same(menu.MenuItems, menu.MenuItems);
            Assert.Empty(menu.Name);
            Assert.Equal(RightToLeft.No, menu.RightToLeft);
            Assert.Null(menu.Site);
            Assert.Null(menu.SourceControl);
            Assert.Null(menu.Tag);
        }

        [WinFormsFact]
        public void ContextMenu_Ctor_NullItemInMenuItemArray_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("item", () => new ContextMenu(new MenuItem[] { null }));
        }

        [WinFormsTheory]
        [InlineData((int)User32.WindowMessage.WM_KEYDOWN)]
        [InlineData((int)User32.WindowMessage.WM_SYSKEYDOWN)]
        public void ContextMenu_SourceControl_GetProcessingKeyMessage_Success(int windowMessage)
        {
            using var menu = new ContextMenu();
            using var control = new Control
            {
                ContextMenu = menu
            };
            Assert.Null(menu.SourceControl);

            var msg = new Message
            {
                Msg = windowMessage
            };
            control.PreProcessMessage(ref msg);
            Assert.Same(control, menu.SourceControl);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void ContextMenu_RightToLeft_SetWithoutSourceControl_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            using var menu = new ContextMenu
            {
                RightToLeft = value
            };
            Assert.Equal(expected, menu.RightToLeft);
            
            // Set same.
            menu.RightToLeft = value;
            Assert.Equal(expected, menu.RightToLeft);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void ContextMenu_RightToLeft_SetWithItems_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            using var menu = new ContextMenu(new MenuItem[] { new MenuItem() })
            {
                RightToLeft = value
            };
            Assert.Equal(expected, menu.RightToLeft);
            
            // Set same.
            menu.RightToLeft = value;
            Assert.Equal(expected, menu.RightToLeft);
        }

        [Theory]
        [InlineData(RightToLeft.Yes, RightToLeft.Yes)]
        [InlineData(RightToLeft.No, RightToLeft.No)]
        [InlineData(RightToLeft.Inherit, RightToLeft.Yes)]
        public void ContextMenu_RightToLeft_SetWithSourceControl_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            using var menu = new ContextMenu();
            using var control = new Control
            {
                ContextMenu = menu,
                RightToLeft = RightToLeft.Yes
            };
            var msg = new Message
            {
                Msg = (int)User32.WindowMessage.WM_KEYDOWN
            };
            control.PreProcessMessage(ref msg);

            menu.RightToLeft = value;
            Assert.Equal(expected, menu.RightToLeft);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void ContextMenu_RightToLeft_SetCreated_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            using var menu = new ContextMenu(new MenuItem[] { new MenuItem("text") });
            Assert.NotEqual(IntPtr.Zero, menu.Handle);

            menu.RightToLeft = value;
            Assert.Equal(expected, menu.RightToLeft);

            // Set same.
            menu.RightToLeft = value;
            Assert.Equal(expected, menu.RightToLeft);
        }

        [Theory]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(RightToLeft), MemberType = typeof(CommonTestHelper))]
        public void ContextMenu_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            using var menu = new ContextMenu();
            Assert.Throws<InvalidEnumArgumentException>("RightToLeft", () => menu.RightToLeft = value);
        }

        public static IEnumerable<object[]> ProcessCmdKey_TestData()
        {
            foreach (Control control in new Control[] { null, new Control() })
            {
                yield return new object[] { new MenuItem { Shortcut = Shortcut.CtrlA }, control, true, 1, 0 };
                yield return new object[] { new MenuItem { Shortcut = Shortcut.CtrlA, Enabled = false }, control, false, 0, 0 };
                yield return new object[] { new MenuItem("text", new MenuItem[] { new MenuItem() }) { Shortcut = Shortcut.CtrlA }, control, true, 0, 1 };
                yield return new object[] { new MenuItem("text", new MenuItem[] { new MenuItem() }) { Shortcut = Shortcut.CtrlA, Enabled = false }, control, false, 0, 0 };
            }
        }

        [Theory]
        [MemberData(nameof(ProcessCmdKey_TestData))]
        public void ContextMenu_ProcessCmdKey_HasItemWithShoutcutKey_ReturnsExpected(MenuItem menuItem, Control control, bool expectedResult, int expectedOnClickCallCount, int expectedOnPopupCallCount)
        {
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

            using var menu = new SubContextMenu(new MenuItem[] { menuItem });
            var msg = new Message();
            Assert.Equal(expectedResult, menu.ProcessCmdKey(ref msg, Keys.Control | Keys.A, control));
            Assert.Same(control, menu.SourceControl);
            Assert.Equal(expectedOnClickCallCount, onClickCallCount);
            Assert.Equal(expectedOnPopupCallCount, onPopupCallCount);
        }

        [WinFormsFact]
        public void ContextMenu_Show_InvokeControlPoint_Success()
        {
            using var menu = new ContextMenu();
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

        [WinFormsTheory]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(LeftRightAlignment), MemberType = typeof(CommonTestHelper))]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(LeftRightAlignment), MemberType = typeof(CommonTestHelper))]
        public void ContextMenu_Show_InvokeControlPointLeftRightAlignment_Success(LeftRightAlignment alignment)
        {
            using var menu = new ContextMenu();
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

        [WinFormsFact]
        public void ContextMenu_Show_NullControl_ThrowsArgumentNullException()
        {
            using var menu = new ContextMenu();
            Assert.Throws<ArgumentNullException>("control", () => menu.Show(null, new Point(1, 2)));
            Assert.Throws<ArgumentNullException>("control", () => menu.Show(null, new Point(1, 2), LeftRightAlignment.Left));
        }

        [WinFormsFact]
        public void ContextMenu_Show_NotVisibleControl_ThrowsArgumentException()
        {
            using var menu = new ContextMenu();
            var control = new Control
            {
                Visible = false
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Throws<ArgumentException>("control", () => menu.Show(control, new Point(1, 2)));
            Assert.Throws<ArgumentException>("control", () => menu.Show(control, new Point(1, 2), LeftRightAlignment.Left));
        }

        [WinFormsFact]
        public void ContextMenu_Show_ControlHasNoHandle_ThrowsArgumentException()
        {
            using var menu = new ContextMenu();
            var control = new Control
            {
                Visible = true
            };
            Assert.Throws<ArgumentException>("control", () => menu.Show(control, new Point(1, 2)));
            Assert.Throws<ArgumentException>("control", () => menu.Show(control, new Point(1, 2), LeftRightAlignment.Left));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ContextMenu_OnCollapse_Invoke_CallsCollapse(EventArgs eventArgs)
        {
            using var menu = new SubContextMenu();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(menu, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            menu.Collapse += handler;
            menu.OnCollapse(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           menu.Collapse -= handler;
           menu.OnCollapse(eventArgs);
           Assert.Equal(1, callCount);
        }
        
        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ContextMenu_OnPopup_Invoke_CallsPopup(EventArgs eventArgs)
        {
            using var menu = new SubContextMenu();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(menu, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            menu.Popup += handler;
            menu.OnPopup(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           menu.Popup -= handler;
           menu.OnPopup(eventArgs);
           Assert.Equal(1, callCount);
        }

        private class SubContextMenu : ContextMenu
        {
            public SubContextMenu() : base()
            {
            }

            public SubContextMenu(MenuItem[] menuItems) : base(menuItems)
            {
            }

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new bool DesignMode => base.DesignMode;

            public new EventHandlerList Events => base.Events;

            public new void OnCollapse(EventArgs e) => base.OnCollapse(e);

            public new void OnPopup(EventArgs e) => base.OnPopup(e);

            public new bool ProcessCmdKey(ref Message msg, Keys keyData, Control control) => base.ProcessCmdKey(ref msg, keyData, control);
        }
    }
}
