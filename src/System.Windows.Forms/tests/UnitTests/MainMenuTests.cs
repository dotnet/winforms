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
    public class MainMenuTests
    {
        [WinFormsFact]
        public void MainMenu_Ctor_Default()
        {
            using var menu = new SubMainMenu();
            Assert.True(menu.CanRaiseEvents);
            Assert.Null(menu.Container);
            Assert.False(menu.DesignMode);
            Assert.NotNull(menu.Events);
            Assert.Same(menu.Events, menu.Events);
            Assert.Empty(menu.MenuItems);
            Assert.Empty(menu.Name);
            Assert.False(menu.IsParent);
            Assert.Equal(RightToLeft.Inherit, menu.RightToLeft);
            Assert.Null(menu.GetForm());
            Assert.Null(menu.Site);
            Assert.Null(menu.Tag);
        }

        [WinFormsFact]
        public void MainMenu_Ctor_IContainer()
        {
            var container = new Container();
            using var menu = new SubMainMenu(container);
            Assert.True(menu.CanRaiseEvents);
            Assert.Same(container, menu.Container);
            Assert.False(menu.DesignMode);
            Assert.NotNull(menu.Events);
            Assert.Same(menu.Events, menu.Events);
            Assert.False(menu.IsParent);
            Assert.Empty(menu.MenuItems);
            Assert.Empty(menu.Name);
            Assert.Equal(RightToLeft.Inherit, menu.RightToLeft);
            Assert.Null(menu.GetForm());
            Assert.NotNull(menu.Site);
            Assert.Null(menu.Tag);
        }

        [WinFormsFact]
        public void MainMenu_Ctor_NullContainer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("container", () => new MainMenu((IContainer)null));
        }

        public static IEnumerable<object[]> Ctor_MenuItemArray_TestData()
        {
            using var menuItem = new MenuItem();
            yield return new object[] { null, false, Array.Empty<MenuItem>() };
            yield return new object[] { Array.Empty<MenuItem>(), false, Array.Empty<MenuItem>() };
            yield return new object[] { new MenuItem[] { menuItem }, true, new MenuItem[] { menuItem } };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_MenuItemArray_TestData))]
        public void MainMenu_Ctor_MenuItemArray(MenuItem[] items, bool expectedIsParent, MenuItem[] expectedItems)
        {
            using var menu = new SubMainMenu(items);
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
            Assert.Empty(menu.Name);
            Assert.Equal(RightToLeft.Inherit, menu.RightToLeft);
            Assert.Null(menu.GetForm());
            Assert.Null(menu.Site);
            Assert.Null(menu.Tag);
        }

        [WinFormsFact]
        public void MainMenu_Ctor_NullItemInMenuItemArray_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("item", () => new MainMenu(new MenuItem[] { null }));
        }

        [WinFormsFact]
        public void MainMenu_GetForm_AddedToForm_ReturnsExpected()
        {
            using var menu = new MainMenu();
            using var form = new Form
            {
                Menu = menu
            };
            Assert.Equal(form, menu.GetForm());
        }

        [WinFormsTheory]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(RightToLeft), MemberType = typeof(CommonTestHelper))]
        public void MainMenu_RightToLeft_SetWithoutForm_GetReturnsExpected(RightToLeft value)
        {
            using var menu = new MainMenu
            {
                RightToLeft = value
            };
            Assert.Equal(value, menu.RightToLeft);
            
            // Set same.
            menu.RightToLeft = value;
            Assert.Equal(value, menu.RightToLeft);
        }

        [WinFormsTheory]
        [InlineData(RightToLeft.Yes, RightToLeft.Yes)]
        [InlineData(RightToLeft.No, RightToLeft.No)]
        [InlineData(RightToLeft.Inherit, RightToLeft.Yes)]
        public void MainMenu_RightToLeft_SetWithSourceControl_GetReturnsExpected(RightToLeft value, RightToLeft expectedValue)
        {
            using var menu = new MainMenu();
            var form = new Form
            {
                Menu = menu,
                RightToLeft = RightToLeft.Yes
            };

            menu.RightToLeft = value;
            Assert.Equal(expectedValue, menu.RightToLeft);
            
            // Set same.
            menu.RightToLeft = value;
            Assert.Equal(expectedValue, menu.RightToLeft);
        }

        [WinFormsTheory]
        [InlineData(RightToLeft.Yes, RightToLeft.Yes)]
        [InlineData(RightToLeft.No, RightToLeft.No)]
        [InlineData(RightToLeft.Inherit, RightToLeft.Inherit)]
        public void MainMenu_RightToLeft_SetCreated_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            using var menu = new MainMenu(new MenuItem[] { new MenuItem("text") });
            Assert.NotEqual(IntPtr.Zero, menu.Handle);

            menu.RightToLeft = value;
            Assert.Equal(expected, menu.RightToLeft);

            // Set same.
            menu.RightToLeft = value;
            Assert.Equal(expected, menu.RightToLeft);
        }

        [WinFormsTheory]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(RightToLeft), MemberType = typeof(CommonTestHelper))]
        public void MainMenu_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            using var menu = new MainMenu();
            Assert.Throws<InvalidEnumArgumentException>("RightToLeft", () => menu.RightToLeft = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void MainMenu_OnCollapse_Invoke_CallsCollapse(EventArgs eventArgs)
        {
            using var menu = new SubMainMenu();
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

        public static IEnumerable<object[]> CloneMenu_TestData()
        {
            yield return new object[] { Array.Empty<MenuItem>() };
            yield return new object[] { new MenuItem[] { new MenuItem("text") } };
        }

        [WinFormsTheory]
        [MemberData(nameof(CloneMenu_TestData))]
        public void MainMenu_CloneMenu_Invoke_Success(MenuItem[] items)
        {
            using var source = new MainMenu(items)
            {
                RightToLeft = RightToLeft.No
            };
            using MainMenu menu = source.CloneMenu();
            Assert.NotSame(source, menu);
            Assert.Equal(items.Select(m => m.Name), menu.MenuItems.Cast<MenuItem>().Select(m => m.Name));
            Assert.Equal(source.IsParent, menu.IsParent);
            Assert.Equal(RightToLeft.Inherit, menu.RightToLeft);
            Assert.Null(menu.GetForm());
            Assert.Empty(menu.Name);
            Assert.Null(menu.Site);
            Assert.Null(menu.Container);
            Assert.Null(menu.Tag);
        }

        [WinFormsFact]
        public void MainMenu_CreateMenuHandle_Invoke_ReturnsExpected()
        {
            using var menu = new SubMainMenu();
            Assert.NotEqual(IntPtr.Zero, menu.CreateMenuHandle());
        }

        [WinFormsFact]
        public void MainMenu_Dispose_HasForm_Success()
        {
            using var menu = new MainMenu();
            using var form = new Form
            {
                Menu = menu
            };

            menu.Dispose();
            Assert.Null(menu.GetForm());
            Assert.Null(form.Menu);
        }

        [WinFormsFact]
        public void MainMenu_Dispose_HasOwnerForm_Success()
        {
            var parentForm = new Form { IsMdiContainer = true };
            var form = new Form
            {
                Menu = new MainMenu(),
                MdiParent = parentForm
            };

            MainMenu menu = form.MergedMenu;
            menu.Dispose();
            Assert.Null(menu.GetForm());
            Assert.Null(form.Menu);
        }

        private class SubMainMenu : MainMenu
        {
            public SubMainMenu() : base()
            {
            }
            
            public SubMainMenu(IContainer container) : base(container)
            {
            }
            
            public SubMainMenu(MenuItem[] items) : base(items)
            {
            }

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new bool DesignMode => base.DesignMode;

            public new EventHandlerList Events => base.Events;

            public new IntPtr CreateMenuHandle() => base.CreateMenuHandle();

            public new void OnCollapse(EventArgs e) => base.OnCollapse(e);
        }
    }
}
