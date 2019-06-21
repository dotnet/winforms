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
    public class MenuTests
    {
        public static IEnumerable<object[]> Ctor_MenuItemArray_TestData()
        {
            yield return new object[] { null, false };
            yield return new object[] { Array.Empty<MenuItem>(), false };
            yield return new object[] { new MenuItem[] { new MenuItem() }, true };
        }

        [Theory]
        [MemberData(nameof(Ctor_MenuItemArray_TestData))]
        public void Menu_Ctor_MenuItemArray(MenuItem[] items, bool expectedIsParent)
        {
            var menu = new SubMenu(items);
            Assert.Equal(items ?? Array.Empty<MenuItem>(), menu.MenuItems.Cast<MenuItem>());
            for (int i = 0; i < (items?.Length ?? 0); i++)
            {
                Assert.Equal(i, menu.MenuItems[i].Index);
                Assert.Equal(menu, menu.MenuItems[i].Parent);
            }
            Assert.Equal(expectedIsParent, menu.IsParent);
            Assert.Empty(menu.Name);
            Assert.Null(menu.Site);
            Assert.Null(menu.Container);
            Assert.Null(menu.Tag);
        }

        [Fact]
        public void Menu_Ctor_NullItemInMenuItemArray_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("item", () => new SubMenu(new MenuItem[] { null }));
        }

        [Fact]
        public void Menu_FindHandle_Get_ReturnsExpected()
        {
            Assert.Equal(0, Menu.FindHandle);
        }

        [Fact]
        public void Menu_FindShortcut_Get_ReturnsExpected()
        {
            Assert.Equal(1, Menu.FindShortcut);
        }

        public static IEnumerable<object[]> HandleMenuItems_TestData()
        {
            yield return new object[] { Array.Empty<MenuItem>() };
            yield return new object[]
            {
                new MenuItem[]
                {
                    // Not visible.
                    new MenuItem { Visible = false },

                    // Has state.
                    new MenuItem { BarBreak = true, Break = true, Checked = true, DefaultItem = true, Enabled = false },

                    // Has shortcut.
                    new MenuItem { Shortcut = Shortcut.CtrlA },

                    // Has no shortcut.
                    new MenuItem { ShowShortcut = false, Shortcut = Shortcut.CtrlA },

                    // Has owner draw.
                    new MenuItem("text") { OwnerDraw = true },

                    // Has children.
                    new MenuItem("text", new MenuItem[] { new MenuItem("child") }),
                }
            };
        }

        [Theory]
        [MemberData(nameof(HandleMenuItems_TestData))]
        public void Menu_Handle_Get_ReturnsExpected(MenuItem[] items)
        {
            using (var menu = new SubMenu(items))
            {
                Assert.NotEqual(IntPtr.Zero, menu.Handle);
                Assert.Equal(menu.Handle, menu.Handle);
            }
        }

        [Fact]
        public void Menu_Handle_GetWithDashTextInNormalMenu_ReturnsExpected()
        {
            var menuItem = new MenuItem("-");
            using (var menu = new SubMenu(new MenuItem[] { menuItem }))
            {
                Assert.NotEqual(IntPtr.Zero, menu.Handle);
                Assert.Equal(menu.Handle, menu.Handle);

                // Does not affect the text.
                Assert.Equal("-", menuItem.Text);
            }
        }

        [Fact]
        public void Menu_Handle_GetWithDashTextInMainMenu_ReturnsExpected()
        {
            var menuItem = new MenuItem("-");
            using (var menu = new MainMenu(new MenuItem[] { menuItem }))
            {
                Assert.NotEqual(IntPtr.Zero, menu.Handle);
                Assert.Equal(menu.Handle, menu.Handle);

                // Sets the separator to a space.
                Assert.Equal(" ", menuItem.Text);
            }
        }

        [Fact]
        public void Menu_Handle_GetWithRightToLeftItem_ReturnsExpected()
        {
            using (var menu = new ContextMenu(new MenuItem[] { new MenuItem("text") }))
            {
                menu.RightToLeft = RightToLeft.Yes;
                Assert.NotEqual(IntPtr.Zero, menu.Handle);
                Assert.Equal(menu.Handle, menu.Handle);
            }
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Menu_Name_GetWithSite_ReturnsExpected(string name, string expected)
        {
            var menu = new SubMenu(Array.Empty<MenuItem>())
            {
                Site = Mock.Of<ISite>(s => s.Name == name)
            };
            Assert.Equal(expected, menu.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Menu_Name_SetWithoutSite_GetReturnsExpected(string value, string expected)
        {
            var menu = new SubMenu(Array.Empty<MenuItem>())
            {
                Name = value
            };
            Assert.Equal(expected, menu.Name);

            // Set same.
            menu.Name = value;
            Assert.Equal(expected, menu.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Menu_Name_SetWithSite_GetReturnsExpected(string value, string expected)
        {
            var menu = new SubMenu(Array.Empty<MenuItem>())
            {
                Site = Mock.Of<ISite>(),
                Name = value
            };
            Assert.Equal(expected, menu.Name);
            Assert.Equal(value?.Length == 0 ? null : value, menu.Site.Name);
        }

        public static IEnumerable<object[]> MdiListItem_TestData()
        {
            var listItem1 = new MenuItem { MdiList = true };
            var listItem2 = new MenuItem { MdiList = true };

            yield return new object[] { Array.Empty<MenuItem>(), null };
            yield return new object[] { new MenuItem[] { listItem1 }, listItem1 };
            yield return new object[] { new MenuItem[] { new MenuItem() }, null };
            yield return new object[] { new MenuItem[] { new MenuItem("parent", new MenuItem[] { listItem2 }) }, listItem2 };
            yield return new object[] { new MenuItem[] { new MenuItem("parent", new MenuItem[] { new MenuItem() }) }, null };
        }

        [Theory]
        [MemberData(nameof(MdiListItem_TestData))]
        public void Menu_MdiListItem_Get_ReturnsExpected(MenuItem[] items, MenuItem expected)
        {
            var menu = new SubMenu(items);
            Assert.Equal(expected, menu.MdiListItem);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Menu_Tag_Set_GetReturnsExpected(object value)
        {
            var menu = new SubMenu(Array.Empty<MenuItem>())
            {
                Tag = value
            };
            Assert.Same(value, menu.Tag);

            // Set same.
            menu.Tag = value;
            Assert.Same(value, menu.Tag);
        }

        public static IEnumerable<object[]> FindMenuItem_TestData()
        {
            yield return new object[] { Array.Empty<MenuItem>(), MenuItem.FindHandle, IntPtr.Zero, null };
            yield return new object[] { Array.Empty<MenuItem>(), MenuItem.FindShortcut, IntPtr.Zero, null };
            yield return new object[] { Array.Empty<MenuItem>(), 2, IntPtr.Zero, null };

            var menuItem1 = new MenuItem();
            var menuItem2 = new MenuItem();
            yield return new object[] { new MenuItem[] { menuItem1 }, MenuItem.FindHandle, IntPtr.Zero, menuItem1 };
            yield return new object[] { new MenuItem[] { new MenuItem() }, MenuItem.FindHandle, (IntPtr)1, null };
            yield return new object[] { new MenuItem[] { menuItem2 }, MenuItem.FindShortcut, IntPtr.Zero, menuItem2 };
            yield return new object[] { new MenuItem[] { new MenuItem() }, MenuItem.FindShortcut, (IntPtr)1, null };
            yield return new object[] { new MenuItem[] { new MenuItem() }, 2, IntPtr.Zero, null };

            var shortcutItem1 = new MenuItem
            {
                Shortcut = Shortcut.CtrlA
            };
            yield return new object[] { new MenuItem[] { shortcutItem1 }, MenuItem.FindShortcut, (IntPtr)0x20041, shortcutItem1 };
            yield return new object[] { new MenuItem[] { new MenuItem { Shortcut = Shortcut.CtrlA } }, MenuItem.FindShortcut, (IntPtr)0x20042, null };

            var parent = new MenuItem("parent", new MenuItem[] { new MenuItem() });
            var shortcutItem2 = new MenuItem
            {
                Shortcut = Shortcut.CtrlA
            };
            yield return new object[] { new MenuItem[] { parent }, MenuItem.FindHandle, IntPtr.Zero, parent };
            yield return new object[] { new MenuItem[] { new MenuItem("parent", new MenuItem[] { new MenuItem() }) }, MenuItem.FindHandle, (IntPtr)1, null };
            yield return new object[] { new MenuItem[] { new MenuItem("parent", new MenuItem[] { new MenuItem() }) }, MenuItem.FindHandle, (IntPtr)1, null };
            yield return new object[] { new MenuItem[] { new MenuItem("parent", new MenuItem[] { shortcutItem2 }) }, MenuItem.FindShortcut, (IntPtr)0x20041, shortcutItem2 };
            yield return new object[] { new MenuItem[] { new MenuItem("parent", new MenuItem[] { new MenuItem() { Shortcut = Shortcut.CtrlA } }) }, MenuItem.FindShortcut, (IntPtr)0x20042, null };
        }

        [Theory]
        [MemberData(nameof(FindMenuItem_TestData))]
        public void Menu_FindMenuItem_Invoke_ReturnsExpected(MenuItem[] items, int type, IntPtr value, MenuItem expected)
        {
            var menu = new SubMenu(items);
            Assert.Equal(expected, menu.FindMenuItem(type, value));
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
        public void Menu_ProcessCmdKey_HasItemWithShoutcutKey_ReturnsExpected(MenuItem menuItem, bool expectedResult, int expectedOnClickCallCount, int expectedOnPopupCallCount)
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

            var menu = new SubMenu(new MenuItem[] { menuItem });
            var message = new Message();
            Assert.Equal(expectedResult, menu.ProcessCmdKey(ref message, Keys.Control | Keys.A));
            Assert.Equal(expectedOnClickCallCount, onClickCallCount);
            Assert.Equal(expectedOnPopupCallCount, onPopupCallCount);
        }

        public static IEnumerable<object[]> ProcessCmdKey_MenuItemParent_TestData()
        {
            var enabledParentChild = new MenuItem { Shortcut = Shortcut.CtrlA };
            var enabledParent = new MenuItem("text", new MenuItem[] { enabledParentChild });
            yield return new object[] { enabledParent, enabledParentChild, true, 1, 0, 0, 1 };

            var enabledParentChildWithItems = new MenuItem("text", new MenuItem[] { new MenuItem() }) { Shortcut = Shortcut.CtrlA };
            var enabledParentWithItems = new MenuItem("text", new MenuItem[] { enabledParentChildWithItems });
            yield return new object[] { enabledParentWithItems, enabledParentChildWithItems, true, 0, 1, 0, 1 };

            var disabledParentChild = new MenuItem { Shortcut = Shortcut.CtrlA };
            var disabledParent = new MenuItem("text", new MenuItem[] { disabledParentChild }) { Enabled = false };
            yield return new object[] { disabledParent, disabledParentChild, false, 0, 0, 0, 0 };
        }

        [Theory]
        [MemberData(nameof(ProcessCmdKey_MenuItemParent_TestData))]
        public void Menu_ProcessCmdKey_MenuItemParent_ReturnsExpected(MenuItem parent, MenuItem child, bool expectedResult, int expectedChildOnClickCallCount, int expectedChildOnPopupCallCount, int expectedParentOnClickCallCount, int expectedParentOnPopupCallCount)
        {
            int childOnClickCallCount = 0;
            child.Click += (sender, e) =>
            {
                childOnClickCallCount++;
                Assert.Same(child, sender);
                Assert.Equal(EventArgs.Empty, e);
            };

            int childOnPopupCallCount = 0;
            child.Popup += (sender, e) =>
            {
                childOnPopupCallCount++;
                Assert.Same(child, sender);
                Assert.Equal(EventArgs.Empty, e);
            };

            int parentOnClickCallCount = 0;
            parent.Click += (sender, e) =>
            {
                parentOnClickCallCount++;
                Assert.Same(parent, sender);
                Assert.Equal(EventArgs.Empty, e);
            };

            int parentOnPopupCallCount = 0;
            parent.Popup += (sender, e) =>
            {
                parentOnPopupCallCount++;
                Assert.Same(parent, sender);
                Assert.Equal(EventArgs.Empty, e);
            };

            var message = new Message();
            Assert.Equal(expectedResult, parent.ProcessCmdKey(ref message, Keys.Control | Keys.A));
            Assert.Equal(expectedChildOnClickCallCount, childOnClickCallCount);
            Assert.Equal(expectedChildOnPopupCallCount, childOnPopupCallCount);
            Assert.Equal(expectedParentOnClickCallCount, parentOnClickCallCount);
            Assert.Equal(expectedParentOnPopupCallCount, parentOnPopupCallCount);
        }

        [Fact]
        public void Menu_ProcessCmdKey_ParentMenuItemPopupHandlerRemovesChild_ReturnsFalse()
        {
            var child = new MenuItem { Shortcut = Shortcut.CtrlA };
            var parent = new MenuItem("text", new MenuItem[] { child });
            var message = new Message();

            parent.Popup += (sender, e) =>
            {
                parent.MenuItems.Remove(child);
            };

            Assert.False(parent.ProcessCmdKey(ref message, Keys.Control | Keys.A));
        }

        [Fact]
        public void Menu_ProcessCmdKey_NoSuchShortcutKey_ReturnsFalse()
        {
            var menu = new SubMenu(new MenuItem[] { new MenuItem() });
            var message = new Message();
            Assert.False(menu.ProcessCmdKey(ref message, Keys.Control | Keys.A));
        }

        [Fact]
        public void Menu_Dispose_NoChildren_Success()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            menu.Dispose();
            menu.Dispose();
        }

        [Fact]
        public void Menu_Dispose_WithChildren_Success()
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem });
            menu.Dispose();
            Assert.Null(menuItem.Parent);
            Assert.Empty(menu.MenuItems);

            menu.Dispose();
            Assert.Empty(menu.MenuItems);
        }

        [Fact]
        public void Menu_Dispose_ItemHasSite_Success()
        {
            var menuItem = new MenuItem
            {
                Site = Mock.Of<ISite>()
            };
            var menu = new SubMenu(new MenuItem[] { menuItem });
            menu.Dispose();
            Assert.Null(menuItem.Parent);
            Assert.Empty(menu.MenuItems);

            menu.Dispose();
            Assert.Empty(menu.MenuItems);
        }

        [Fact]
        public void Menu_Dispose_ItemHasSiteWithContainer_Success()
        {
            var container = new Container();
            var menuItem = new MenuItem
            {
                Site = Mock.Of<ISite>(s => s.Container == container)
            };
            container.Add(menuItem);
            var menu = new SubMenu(new MenuItem[] { menuItem });
            menu.Dispose();
            Assert.Empty(container.Components);
            Assert.Null(menuItem.Parent);
            Assert.Empty(menu.MenuItems);

            menu.Dispose();
            Assert.Empty(menu.MenuItems);
        }

        public static IEnumerable<object[]> FindMergePosition_TestData()
        {
            yield return new object[] { Array.Empty<MenuItem>(), -1, 0 };
            yield return new object[] { new MenuItem[] { new MenuItem { MergeOrder = 10 }, new MenuItem { MergeOrder = 12 }, new MenuItem(), new MenuItem(), new MenuItem() }, 12, 5 };
            yield return new object[] { new MenuItem[] { new MenuItem { MergeOrder = 10 }, new MenuItem { MergeOrder = 12 }, new MenuItem(), new MenuItem(), new MenuItem() }, 11, 5 };
            yield return new object[] { new MenuItem[] { new MenuItem { MergeOrder = 10 }, new MenuItem { MergeOrder = 12 }, new MenuItem(), new MenuItem(), new MenuItem() }, -1, 0 };
            yield return new object[] { new MenuItem[] { new MenuItem { MergeOrder = 10 }, new MenuItem(), new MenuItem(), new MenuItem { MergeOrder = 12 }, new MenuItem() }, 0, 5 };
            yield return new object[] { new MenuItem[] { new MenuItem { MergeOrder = 10 }, new MenuItem(), new MenuItem(), new MenuItem { MergeOrder = 12 }, new MenuItem() }, 5, 5 };
            yield return new object[] { new MenuItem[] { new MenuItem() }, 0, 1 };
        }

        [Theory]
        [MemberData(nameof(FindMergePosition_TestData))]
        public void Menu_FindMergePosition_Invoke_ReturnsExpected(MenuItem[] items, int mergeOrder, int expected)
        {
            var menu = new SubMenu(items);
            Assert.Equal(expected, menu.FindMergePosition(mergeOrder));
        }

        public static IEnumerable<object[]> GetContextMenu_TestData()
        {
            yield return new object[] { new SubMenu(Array.Empty<MenuItem>()), null };
            yield return new object[] { new MenuItem(), null };

            var menuItem1 = new MenuItem();
            var menu1 = new ContextMenu(new MenuItem[] { menuItem1 });
            yield return new object[] { menu1, menu1 };
            yield return new object[] { menuItem1, menu1 };

            var menuItem2 = new MenuItem();
            var menu2 = new SubMenu(new MenuItem[] { menuItem2 });
            yield return new object[] { menu2, null };
            yield return new object[] { menuItem2, null };
        }

        [Theory]
        [MemberData(nameof(GetContextMenu_TestData))]
        public void Menu_GetContextMenu_Invoke_ReturnsExpected(Menu menu, ContextMenu expected)
        {
            Assert.Equal(expected, menu.GetContextMenu());
        }

        public static IEnumerable<object[]> GetMainMenu_TestData()
        {
            yield return new object[] { new SubMenu(Array.Empty<MenuItem>()), null };
            yield return new object[] { new MenuItem(), null };

            var menuItem1 = new MenuItem();
            var menu1 = new MainMenu(new MenuItem[] { menuItem1 });
            yield return new object[] { menu1, menu1 };
            yield return new object[] { menuItem1, menu1 };

            var menuItem2 = new MenuItem();
            var menu2 = new SubMenu(new MenuItem[] { menuItem2 });
            yield return new object[] { menu2, null };
            yield return new object[] { menuItem2, null };
        }

        [Theory]
        [MemberData(nameof(GetMainMenu_TestData))]
        public void Menu_GetMainMenu_Invoke_ReturnsExpected(Menu menu, MainMenu expected)
        {
            Assert.Equal(expected, menu.GetMainMenu());
        }

        public static IEnumerable<object[]> MergeMenu_TestData()
        {
            {
                var menu1 = new MenuItem("1");
                var menu2 = new MenuItem("2");
                yield return new object[]
                {
                    new MenuItem[] { menu1 },
                    new MenuItem[] { menu2 },
                    new MenuItem[] { menu1, menu2 }
                };
            }
            {
                var menu1 = new MenuItem("1");
                var menu2 = new MenuItem("2") { MergeType = MenuMerge.MergeItems };
                yield return new object[]
                {
                    new MenuItem[] { menu1 },
                    new MenuItem[] { menu2 },
                    new MenuItem[] { menu1, menu2 }
                };
            }
            {
                var menu1 = new MenuItem("1");
                var menu2 = new MenuItem("2");
                var menu3 = new MenuItem("3") { MergeType = MenuMerge.Replace };
                yield return new object[]
                {
                    new MenuItem[] { menu1, menu2 },
                    new MenuItem[] { menu3 },
                    new MenuItem[] { menu1, menu2, menu3 }
                };
            }
            {
                var menu1 = new MenuItem("1");
                var menu2 = new MenuItem("2") { MergeType = MenuMerge.Remove };
                yield return new object[]
                {
                    new MenuItem[] { menu1 },
                    new MenuItem[] { menu2 },
                    new MenuItem[] { menu1 }
                };
            }
            {
                var menu1 = new MenuItem("1") { MergeOrder = 1 };
                var menu2 = new MenuItem("2");
                yield return new object[]
                {
                    new MenuItem[] { menu1 },
                    new MenuItem[] { menu2 },
                    new MenuItem[] { menu2, menu1 }
                };
            }
            {
                var menu1 = new MenuItem("1") { MergeOrder = -1 };
                var menu2 = new MenuItem("2");
                yield return new object[]
                {
                    new MenuItem[] { menu1 },
                    new MenuItem[] { menu2 },
                    new MenuItem[] { menu1, menu2 }
                };
            }
            {
                var menu1 = new MenuItem("1") { MergeOrder = 1 };
                var menu2 = new MenuItem("2") { MergeOrder = 1 };
                var menu3 = new MenuItem("3") { MergeOrder = 2 };
                var menu4 = new MenuItem("4") { MergeOrder = 2 };
                var menu5 = new MenuItem("5") { MergeOrder = 3 };
                var menu6 = new MenuItem("6") { MergeOrder = 0 };
                yield return new object[]
                {
                    new MenuItem[] { menu1, menu2, menu3 },
                    new MenuItem[] { menu4, menu5, menu6 },
                    new MenuItem[] { menu6, menu1, menu2, menu3, menu4, menu5 }
                };
            }
            {
                var menu1 = new MenuItem("1") { MergeOrder = 1, MergeType = MenuMerge.Replace };
                var menu2 = new MenuItem("2") { MergeOrder = 1, MergeType = MenuMerge.Replace };
                var menu3 = new MenuItem("3") { MergeOrder = 2, MergeType = MenuMerge.Replace };
                var menu4 = new MenuItem("4") { MergeOrder = 2, MergeType = MenuMerge.Replace };
                var menu5 = new MenuItem("5") { MergeOrder = 3, MergeType = MenuMerge.Replace };
                var menu6 = new MenuItem("6") { MergeOrder = 0, MergeType = MenuMerge.Replace };
                yield return new object[]
                {
                    new MenuItem[] { menu1, menu2, menu3 },
                    new MenuItem[] { menu4, menu5, menu6 },
                    new MenuItem[] { menu6, menu1, menu3, menu4, menu5 }
                };
            }
            {
                var menu1 = new MenuItem("1") { MergeOrder = 1, MergeType = MenuMerge.Remove };
                var menu2 = new MenuItem("2") { MergeOrder = 1, MergeType = MenuMerge.Remove };
                var menu3 = new MenuItem("3") { MergeOrder = 2, MergeType = MenuMerge.Remove };
                var menu4 = new MenuItem("4") { MergeOrder = 2, MergeType = MenuMerge.Remove };
                var menu5 = new MenuItem("5") { MergeOrder = 3, MergeType = MenuMerge.Remove };
                var menu6 = new MenuItem("6") { MergeOrder = 0, MergeType = MenuMerge.Remove };
                yield return new object[]
                {
                    new MenuItem[] { menu1, menu2, menu3 },
                    new MenuItem[] { menu4, menu5, menu6 },
                    new MenuItem[] { menu1, menu2, menu3 }
                };
            }
            {
                var menu1 = new MenuItem("1") { MergeOrder = 1, MergeType = MenuMerge.MergeItems };
                var menu2 = new MenuItem("2") { MergeOrder = 1, MergeType = MenuMerge.MergeItems };
                var menu3 = new MenuItem("3") { MergeOrder = 2, MergeType = MenuMerge.MergeItems };
                var menu4 = new MenuItem("4") { MergeOrder = 2, MergeType = MenuMerge.MergeItems };
                var menu5 = new MenuItem("5") { MergeOrder = 3, MergeType = MenuMerge.MergeItems };
                var menu6 = new MenuItem("6") { MergeOrder = 0, MergeType = MenuMerge.MergeItems };
                yield return new object[]
                {
                    new MenuItem[] { menu1, menu2, menu3 },
                    new MenuItem[] { menu4, menu5, menu6 },
                    new MenuItem[] { menu6, menu1, menu2, menu4, menu5 }
                };
            }
            {
                var menu1 = new MenuItem("1");
                var menu2 = new MenuItem("2");
                yield return new object[]
                {
                    new MenuItem[] { menu1, menu2 },
                    null,
                    new MenuItem[] { menu1, menu2 }
                };
            }
            {
                var menu1 = new MenuItem("1");
                var menu2 = new MenuItem("2");
                yield return new object[]
                {
                    null,
                    new MenuItem[] { menu1, menu2 },
                    new MenuItem[] { menu1, menu2 }
                };
            }
            {
                yield return new object[]
                {
                    null,
                    null,
                    Array.Empty<MenuItem>()
                };
            }
        }

        [Theory]
        [MemberData(nameof(MergeMenu_TestData))]
        public void Menu_MergeMenu_Invoke_ReturnsExpected(MenuItem[] destinationItems, MenuItem[] sourceItems, MenuItem[] expectedItems)
        {
            var menu = new SubMenu(destinationItems);
            var menuSrc = new SubMenu(sourceItems);
            menu.MergeMenu(menuSrc);
            AssertEqualMenuItems(expectedItems, menu.MenuItems.Cast<MenuItem>().ToArray());
        }

        [Fact]
        public void Menu_MergeMenu_SameSourceAndDestination_ThrowsArgumentException()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            Assert.Throws<ArgumentException>("menuSrc", () => menu.MergeMenu(menu));
        }

        [Fact]
        public void Menu_MergeMenu_NullSource_ThrowsArgumentNullException()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            Assert.Throws<ArgumentNullException>("menuSrc", () => menu.MergeMenu(null));
        }

        public static IEnumerable<object[]> CloneMenu_TestData()
        {
            yield return new object[] { Array.Empty<MenuItem>() };
            yield return new object[] { new MenuItem[] { new MenuItem("text") } };
        }

        [Theory]
        [MemberData(nameof(CloneMenu_TestData))]
        public void Menu_CloneMenu_Invoke_Success(MenuItem[] items)
        {
            var menu = new SubMenu(new MenuItem[] { new MenuItem() });
            menu.CloneMenu(new SubMenu(items));
            AssertEqualMenuItems(items, menu.MenuItems.Cast<MenuItem>().ToArray());
            for (int i = 0; i < items.Length; i++)
            {
                Assert.Equal(i, menu.MenuItems[i].Index);
                Assert.Equal(menu, menu.MenuItems[i].Parent);
            }
        }

        [Fact]
        public void Menu_CloneMenu_NullMenuSource_ThrowsArgumentNullException()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            Assert.Throws<ArgumentNullException>("menuSrc", () => menu.CloneMenu(null));
        }

        [Fact]
        public void Menu_ToString_Invoke_ReturnsExpected()
        {
            var menu = new SubMenu(new MenuItem[] { new MenuItem() });
            Assert.Equal("System.Windows.Forms.Tests.MenuTests+SubMenu, Items.Count: 1", menu.ToString());
        }

        internal static void AssertEqualMenuItems(MenuItem[] expected, MenuItem[] actual)
        {
            Assert.Equal(expected.Length, actual.Length);
            try
            {
                Assert.Equal(expected.Select(m => m.Text), actual.Cast<MenuItem>().Select(m => m.Text));
            }
            catch (ObjectDisposedException)
            {
            }
            for (int i = 0; i < expected.Length; i++)
            {
                AssertEqualMenuItem(expected[i], actual[i]);
            }
        }

        internal static void AssertEqualMenuItem(MenuItem expected, MenuItem actual)
        {
            try
            {
                Assert.Equal(expected.Name, actual.Name);
            }
            catch (ObjectDisposedException)
            {
            }
            AssertEqualMenuItems(expected.MenuItems.Cast<MenuItem>().ToArray(), actual.MenuItems.Cast<MenuItem>().ToArray());
        }

        private class SubMenu : Menu
        {
            public SubMenu(MenuItem[] items) : base(items)
            {
            }

            public new int FindMergePosition(int mergeOrder) => base.FindMergePosition(mergeOrder);

            public new void CloneMenu(Menu menuSrc) => base.CloneMenu(menuSrc);

            public new bool ProcessCmdKey(ref Message msg, Keys keyData) => base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
