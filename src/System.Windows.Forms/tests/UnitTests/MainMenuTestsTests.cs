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
    public class MainMenuTestsTests
    {
        [Fact]
        public void MainMenu_Ctor_Default()
        {
            var menu = new MainMenu();
            Assert.Empty(menu.MenuItems);
            Assert.False(menu.IsParent);
            Assert.Equal(RightToLeft.Inherit, menu.RightToLeft);
            Assert.Null(menu.GetForm());
            Assert.Empty(menu.Name);
            Assert.Null(menu.Site);
            Assert.Null(menu.Container);
            Assert.Null(menu.Tag);
        }

        [Fact]
        public void MainMenu_Ctor_IContainer()
        {
            var container = new Container();
            var menu = new MainMenu(container);
            Assert.Empty(menu.MenuItems);
            Assert.False(menu.IsParent);
            Assert.Equal(RightToLeft.Inherit, menu.RightToLeft);
            Assert.Null(menu.GetForm());
            Assert.Empty(menu.Name);
            Assert.NotNull(menu.Site);
            Assert.Equal(container, menu.Container);
            Assert.Null(menu.Tag);
        }

        [Fact]
        public void MainMenu_Ctor_NullContainer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("container", () => new MainMenu((IContainer)null));
        }

        public static IEnumerable<object[]> Ctor_MenuItemArray_TestData()
        {
            yield return new object[] { null, false };
            yield return new object[] { Array.Empty<MenuItem>(), false };
            yield return new object[] { new MenuItem[] { new MenuItem() }, true };
        }

        [Theory]
        [MemberData(nameof(Ctor_MenuItemArray_TestData))]
        public void MainMenu_Ctor_MenuItemArray(MenuItem[] items, bool expectedIsParent)
        {
            var menu = new MainMenu(items);
            Assert.Equal(items ?? Array.Empty<MenuItem>(), menu.MenuItems.Cast<MenuItem>());
            for (int i = 0; i < (items?.Length ?? 0); i++)
            {
                Assert.Equal(i, menu.MenuItems[i].Index);
                Assert.Equal(menu, menu.MenuItems[i].Parent);
            }
            Assert.Equal(expectedIsParent, menu.IsParent);
            Assert.Equal(RightToLeft.Inherit, menu.RightToLeft);
            Assert.Null(menu.GetForm());
            Assert.Empty(menu.Name);
            Assert.Null(menu.Site);
            Assert.Null(menu.Container);
            Assert.Null(menu.Tag);
        }

        [Fact]
        public void MainMenu_Ctor_NullItemInMenuItemArray_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("item", () => new MainMenu(new MenuItem[] { null }));
        }

        [Fact]
        public void MainMenu_GetForm_AddedToForm_ReturnsExpected()
        {
            var form = new Form
            {
                Menu = new MainMenu()
            };

            MainMenu menu = form.Menu;
            Assert.Equal(form, menu.GetForm());
            Assert.Equal(form, menu.GetFormUnsafe());
        }

        [Theory]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(RightToLeft), MemberType = typeof(CommonTestHelper))]
        public void MainMenu_RightToLeft_SetWithoutForm_GetReturnsExpected(RightToLeft value)
        {
            var menu = new MainMenu
            {
                RightToLeft = value
            };
            Assert.Equal(value, menu.RightToLeft);
        }

        [Theory]
        [InlineData(RightToLeft.Yes, RightToLeft.Yes)]
        [InlineData(RightToLeft.No, RightToLeft.No)]
        [InlineData(RightToLeft.Inherit, RightToLeft.Yes)]
        public void MainMenu_RightToLeft_SetWithSourceControl_GetReturnsExpected(RightToLeft value, RightToLeft expectedValue)
        {
            var form = new Form
            {
                Menu = new MainMenu(),
                RightToLeft = RightToLeft.Yes
            };
            MainMenu menu = form.Menu;

            menu.RightToLeft = value;
            Assert.Equal(expectedValue, menu.RightToLeft);
        }

        [Fact]
        public void MainMenu_RightToLeft_SetCreated_GetReturnsExpected()
        {
            using (var menu = new MainMenu(new MenuItem[] { new MenuItem("text") }))
            {
                Assert.NotEqual(IntPtr.Zero, menu.Handle);
                menu.RightToLeft = RightToLeft.Yes;
                menu.RightToLeft = RightToLeft.No;
                Assert.Equal(RightToLeft.No, menu.RightToLeft);
            }
        }

        [Theory]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(RightToLeft), MemberType = typeof(CommonTestHelper))]
        public void MainMenu_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            var menu = new MainMenu();
            Assert.Throws<InvalidEnumArgumentException>("RightToLeft", () => menu.RightToLeft = value);
        }

        [Fact]
        public void MainMenu_OnCollapse_Invoke_Success()
        {
            var menu = new MainMenu();

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

        public static IEnumerable<object[]> CloneMenu_TestData()
        {
            yield return new object[] { Array.Empty<MenuItem>() };
            yield return new object[] { new MenuItem[] { new MenuItem("text") } };
        }

        [Theory]
        [MemberData(nameof(CloneMenu_TestData))]
        public void MainMenu_CloneMenu_Invoke_Success(MenuItem[] items)
        {
            var source = new MainMenu(items)
            {
                RightToLeft = RightToLeft.No
            };
            MainMenu menu = source.CloneMenu();
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

        [Fact]
        public void MainMenu_Dispose_HasForm_Success()
        {
            var form = new Form
            {
                Menu = new MainMenu()
            };

            MainMenu menu = form.Menu;
            menu.Dispose();
            Assert.Null(menu.GetForm());
            Assert.Null(form.Menu);
        }

        [Fact]
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

        private class SubContextMenu : MainMenu
        {
            public new void OnCollapse(EventArgs e) => base.OnCollapse(e);
        }
    }
}
