// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class MenuItemCollectionTests
    {
        [Fact]
        public void MenuItemCollection_Ctor_Menu()
        {
            var menu = new SubMenu(new MenuItem[] { new MenuItem() });
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Single(collection);
        }

        [Fact]
        public void MenuItemCollection_IList_Properties_ReturnsExpected()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            IList collection = new Menu.MenuItemCollection(menu);
            Assert.False(collection.IsFixedSize);
            Assert.False(collection.IsReadOnly);
            Assert.False(collection.IsSynchronized);
            Assert.Same(collection, collection.SyncRoot);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void MenuItemCollection_Add_String_Success(string caption, string expectedText)
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menu);
            MenuItem menuItem = collection.Add(caption);
            Assert.Same(menuItem, Assert.Single(collection));
            Assert.Equal(expectedText, menuItem.Text);
            Assert.Empty(menuItem.MenuItems);
            Assert.Equal(menu, menuItem.Parent);
            Assert.Equal(0, menuItem.Index);
        }

        public static IEnumerable<object[]> Add_StringEventHandler_TestData()
        {
            EventHandler onClick = (sender, e) => { };
            yield return new object[] { null, null, string.Empty };
            yield return new object[] { string.Empty, onClick, string.Empty };
            yield return new object[] { "caption", onClick, "caption" };
        }

        [Theory]
        [MemberData(nameof(Add_StringEventHandler_TestData))]
        public void MenuItemCollection_Add_StringEventHandler_Success(string caption, EventHandler onClick, string expectedText)
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menu);
            MenuItem menuItem = collection.Add(caption, onClick);
            Assert.Same(menuItem, Assert.Single(collection));
            Assert.Same(expectedText, menuItem.Text);
            Assert.Empty(menuItem.MenuItems);
            Assert.Equal(menu, menuItem.Parent);
            Assert.Equal(0, menuItem.Index);
        }

        public static IEnumerable<object[]> Add_StringMenuItemArray_TestData()
        {
            yield return new object[] { null, null, string.Empty };
            yield return new object[] { string.Empty, Array.Empty<MenuItem>(), string.Empty };
            yield return new object[] { "caption", new MenuItem[] { new MenuItem() }, "caption" };
        }

        [Theory]
        [MemberData(nameof(Add_StringMenuItemArray_TestData))]
        public void MenuItemCollection_Add_StringMenuItemArray_Success(string caption, MenuItem[] items, string expectedText)
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menu);
            MenuItem menuItem = collection.Add(caption, items);
            Assert.Same(menuItem, Assert.Single(collection));
            Assert.Same(expectedText, menuItem.Text);
            Assert.Equal(items ?? Array.Empty<MenuItem>(), menuItem.MenuItems.Cast<MenuItem>());
            Assert.Equal(menu, menuItem.Parent);
            Assert.Equal(0, menuItem.Index);
        }

        [Fact]
        public void MenuItemCollection_Add_MenuItem_Success()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menu);

            var menuItem1 = new MenuItem("text1");
            Assert.Equal(0, collection.Add(menuItem1));
            Assert.Same(menuItem1, Assert.Single(collection));
            Assert.Equal(menu, menuItem1.Parent);
            Assert.Equal(0, menuItem1.Index);

            var menuItem2 = new MenuItem("text2");
            Assert.Equal(1, collection.Add(menuItem2));
            Assert.Equal(2, collection.Count);
            Assert.Same(menuItem1, collection[0]);
            Assert.Same(menuItem2, collection[1]);
            Assert.Equal(menu, menuItem1.Parent);
            Assert.Equal(0, menuItem1.Index);
            Assert.Equal(menu, menuItem2.Parent);
            Assert.Equal(1, menuItem2.Index);
        }

        [Fact]
        public void MenuItemCollection_Add_IndexMenuItem_Success()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menu);

            var menuItem1 = new MenuItem("text1");
            Assert.Equal(0, collection.Add(0, menuItem1));
            Assert.Same(menuItem1, Assert.Single(collection));
            Assert.Equal(menu, menuItem1.Parent);
            Assert.Equal(0, menuItem1.Index);

            var menuItem2 = new MenuItem("text1");
            Assert.Equal(0, collection.Add(0, menuItem2));
            Assert.Equal(2, collection.Count);
            Assert.Same(menuItem2, collection[0]);
            Assert.Same(menuItem1, collection[1]);
            Assert.Equal(menu, menuItem1.Parent);
            Assert.Equal(1, menuItem1.Index);
            Assert.Equal(menu, menuItem2.Parent);
            Assert.Equal(0, menuItem2.Index);
        }

        [Fact]
        public void MenuItemCollection_Add_SelfMenuItem_Nop()
        {
            var menuItem = new MenuItem("text", Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menuItem);
            Assert.Empty(collection);
        }

        [Fact]
        public void MenuItemCollection_Add_AlreadyInSameCollection_Success()
        {
            var menuItem1 = new MenuItem();
            var menuItem2 = new MenuItem();
            var parent = new MenuItem("text", Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(parent)
            {
                menuItem1,
                menuItem2
            };

            Assert.Equal(0, collection.Add(0, menuItem2));
            Assert.Equal(2, collection.Count);
            Assert.Same(menuItem2, collection[0]);
            Assert.Same(menuItem1, collection[1]);
            Assert.Equal(parent, menuItem1.Parent);
            Assert.Equal(1, menuItem1.Index);
            Assert.Equal(parent, menuItem2.Parent);
            Assert.Equal(0, menuItem2.Index);

            Assert.Equal(1, collection.Add(2, menuItem2));
            Assert.Equal(2, collection.Count);
            Assert.Same(menuItem1, collection[0]);
            Assert.Same(menuItem2, collection[1]);
            Assert.Equal(parent, menuItem1.Parent);
            Assert.Equal(0, menuItem1.Index);
            Assert.Equal(parent, menuItem2.Parent);
            Assert.Equal(1, menuItem2.Index);
        }

        [Fact]
        public void MenuItemCollection_Add_AlreadyInDifferentCollection_Success()
        {
            var oldParent = new MenuItem("text", Array.Empty<MenuItem>());
            var oldCollection = new Menu.MenuItemCollection(oldParent);
            var newParent = new MenuItem("text", Array.Empty<MenuItem>());
            var newCollection = new Menu.MenuItemCollection(newParent);

            var menuItem = new MenuItem();
            oldCollection.Add(menuItem);

            Assert.Equal(0, newCollection.Add(menuItem));
            Assert.Empty(oldCollection);
            Assert.Same(menuItem, Assert.Single(newCollection));
            Assert.Equal(newParent, menuItem.Parent);
            Assert.Equal(0, menuItem.Index);
        }

        [Fact]
        public void MenuItemCollection_Add_MenuItemToCreatedMenu_Success()
        {
            using (var menu = new SubMenu(Array.Empty<MenuItem>()))
            {
                Assert.NotEqual(IntPtr.Zero, menu.Handle);

                var collection = new Menu.MenuItemCollection(menu);

                var menuItem1 = new MenuItem("text1");
                Assert.Equal(0, collection.Add(menuItem1));
                Assert.Same(menuItem1, Assert.Single(collection));
                Assert.Equal(menu, menuItem1.Parent);
                Assert.Equal(0, menuItem1.Index);

                var menuItem2 = new MenuItem("text2");
                Assert.Equal(1, collection.Add(menuItem2));
                Assert.Equal(2, collection.Count);
                Assert.Same(menuItem1, collection[0]);
                Assert.Same(menuItem2, collection[1]);
                Assert.Equal(menu, menuItem1.Parent);
                Assert.Equal(0, menuItem1.Index);
                Assert.Equal(menu, menuItem2.Parent);
                Assert.Equal(1, menuItem2.Index);
            }
        }

        [Fact]
        public void MenuItemCollection_Add_CreatedMenuItemToMenu_Success()
        {
            var menuItem = new MenuItem("text1");
            using (var otherMenu = new SubMenu(new MenuItem[] { menuItem }))
            {
                Assert.NotEqual(IntPtr.Zero, otherMenu.Handle);

                var menu = new SubMenu(Array.Empty<MenuItem>());
                var collection = new Menu.MenuItemCollection(menu);

                Assert.Equal(0, collection.Add(menuItem));
                Assert.Same(menuItem, Assert.Single(collection));
                Assert.Equal(menu, menuItem.Parent);
                Assert.Equal(0, menuItem.Index);
            }
        }

        [Fact]
        public void MenuItemCollection_Add_NullMenuItem_ThrowsArgumentNullException()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Throws<ArgumentNullException>("item", () => collection.Add((MenuItem)null));
            Assert.Throws<ArgumentNullException>("item", () => collection.Add(0, null));
        }

        [Fact]
        public void MenuItemCollection_Add_SelfWithParent_ThrowsArgumentException()
        {
            var menuItem = new MenuItem("text", Array.Empty<MenuItem>());
            var parent = new MenuItem("parent", new MenuItem[] { menuItem });
            var collection = new Menu.MenuItemCollection(menuItem);
            Assert.Throws<ArgumentException>("item", () => collection.Add(menuItem));
        }

        [Fact]
        public void MenuItemCollection_Add_ParentWithGrandparent_ThrowsArgumentException()
        {
            var menuItem = new MenuItem();
            var parent = new MenuItem("parent", new MenuItem[] { menuItem });
            var grandparent = new MenuItem("grandparent", new MenuItem[] { parent });

            var oldCollection = new Menu.MenuItemCollection(menuItem);
            Assert.Throws<ArgumentException>("item", () => oldCollection.Add(parent));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void MenuItemCollection_Add_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Add(index, new MenuItem()));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(2)]
        public void MenuItemCollection_Add_AlreadyInCollection_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            var menuItem = new MenuItem();
            var parent = new MenuItem("text", Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(parent)
            {
                menuItem
            };
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Add(index, menuItem));
        }

        [Fact]
        public void MenuItemCollection_Add_IListMenuItem_Success()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            IList collection = new Menu.MenuItemCollection(menu);

            var menuItem1 = new MenuItem("text1");
            Assert.Equal(0, collection.Add(menuItem1));
            Assert.Same(menuItem1, Assert.Single(collection));
            Assert.Equal(menu, menuItem1.Parent);
            Assert.Equal(0, menuItem1.Index);

            var menuItem2 = new MenuItem("text2");
            Assert.Equal(1, collection.Add(menuItem2));
            Assert.Equal(2, collection.Count);
            Assert.Same(menuItem1, collection[0]);
            Assert.Same(menuItem2, collection[1]);
            Assert.Equal(menu, menuItem1.Parent);
            Assert.Equal(0, menuItem1.Index);
            Assert.Equal(menu, menuItem2.Parent);
            Assert.Equal(1, menuItem2.Index);
        }

        [Theory]
        [InlineData("value")]
        [InlineData(null)]
        public void MenuItemCollection_Add_IListNotMenuItem_ThrowsArgumentException(object value)
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            IList collection = new Menu.MenuItemCollection(menu);
            Assert.Throws<ArgumentException>("value", () => collection.Add(value));
        }

        public static IEnumerable<object[]> AddRange_TestData()
        {
            yield return new object[] { Array.Empty<MenuItem>() };
            yield return new object[] { new MenuItem[] { new MenuItem(), new MenuItem() } };
        }

        [Theory]
        [MemberData(nameof(AddRange_TestData))]
        public void MenuItemCollection_AddRange_Invoke_Success(MenuItem[] items)
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menu);
            collection.AddRange(items);
            Assert.Equal(items.Length, collection.Count);
            Assert.Equal(items, collection.Cast<MenuItem>());
        }

        [Fact]
        public void MenuItemCollection_AddRange_NullItems_ThrowsArgumentNullException()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Throws<ArgumentNullException>("items", () => collection.AddRange(null));
        }

        [Fact]
        public void MenuItemCollection_AddRange_NullValueInItems_ThrowsArgumentNullException()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Throws<ArgumentNullException>("item", () => collection.AddRange(new MenuItem[] { null }));
        }

        [Fact]
        public void MenuItemCollection_Clear_InvokeOnMenu_Success()
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem });
            var collection = new Menu.MenuItemCollection(menu);
            collection.Clear();
            Assert.Empty(collection);
            Assert.Null(menuItem.Parent);
            Assert.Equal(-1, menuItem.Index);

            collection.Clear();
            Assert.Empty(collection);
        }

        [Fact]
        public void MenuItemCollection_Clear_InvokeOnMenuItem_Success()
        {
            var menuItem = new MenuItem();
            var menu = new MenuItem("text", new MenuItem[] { menuItem });
            var collection = new Menu.MenuItemCollection(menu);
            collection.Clear();
            Assert.Empty(collection);
            Assert.Null(menuItem.Parent);
            Assert.Equal(-1, menuItem.Index);

            collection.Clear();
            Assert.Empty(collection);
        }

        [Fact]
        public void MenuItemCollection_CopyTo_NotEmpty_Success()
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menu)
            {
                menuItem
            };
            var array = new object[] { 1, 2, 3 };
            collection.CopyTo(array, 1);
            Assert.Equal(new object[] { 1, menuItem, 3 }, array);
        }

        [Fact]
        public void MenuItemCollection_CopyTo_Empty_Nop()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menu);
            var array = new object[] { 1, 2, 3 };
            collection.CopyTo(array, 1);
            Assert.Equal(new object[] { 1, 2, 3 }, array);
        }

        public static IEnumerable<object[]> Contains_TestData()
        {
            var menuItem = new MenuItem();
            yield return new object[] { new SubMenu(new MenuItem[] { menuItem }), menuItem, true };
            yield return new object[] { new SubMenu(new MenuItem[] { new MenuItem() }), new MenuItem(), false };
            yield return new object[] { new SubMenu(new MenuItem[] { new MenuItem() }), null, false };
        }

        [Theory]
        [MemberData(nameof(Contains_TestData))]
        public void MenuItemCollection_Contains_Invoke_ReturnsExpected(Menu menu, MenuItem value, bool expected)
        {
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Equal(expected, collection.Contains(value));
        }

        [Theory]
        [MemberData(nameof(Contains_TestData))]
        public void MenuItemCollection_Contains_IListInvoke_ReturnsExpected(Menu menu, MenuItem value, bool expected)
        {
            IList collection = new Menu.MenuItemCollection(menu);
            Assert.Equal(expected, collection.Contains(value));
        }

        [Fact]
        public void MenuItemCollection_Contains_IListNotMenuItem_ReturnsMinusOne()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            IList collection = new Menu.MenuItemCollection(menu);
            Assert.False(collection.Contains("value"));
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("name", true)]
        [InlineData("NAME", true)]
        [InlineData("noSuchName", false)]
        public void MenuItemCollection_ContainsKey_Invoke_ReturnsExpected(string key, bool expected)
        {
            var menu = new SubMenu(new MenuItem[] { new MenuItem { Name = "name" } });
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Equal(expected, collection.ContainsKey(key));
        }

        public static IEnumerable<object[]> Find_TestData()
        {
            yield return new object[] { new MenuItem[] { new MenuItem() }, "noSuchKey", false, Array.Empty<MenuItem>() };
            yield return new object[] { new MenuItem[] { new MenuItem() }, "noSuchKey", true, Array.Empty<MenuItem>() };

            foreach (bool searchAllChildren in new bool[] { true, false })
            {
                var menuItem1 = new MenuItem { Name = "name" };
                var menuItem2 = new MenuItem { Name = "NAME" };
                var menuItem3 = new MenuItem { Name = "otherName" };
                var menuItem4 = new MenuItem { Name = "name" };
                yield return new object[] { new MenuItem[] { menuItem1, menuItem2, menuItem3 }, "name", searchAllChildren, new MenuItem[] { menuItem1, menuItem2 } };
            }

            yield return new object[] { new MenuItem[] { new MenuItem { Name = "name" } }, "noSuchName", true, Array.Empty<MenuItem>() };
            yield return new object[] { new MenuItem[] { new MenuItem { Name = "name" } }, "noSuchName", false, Array.Empty<MenuItem>() };
            yield return new object[] { new MenuItem[] { new MenuItem("text", new MenuItem[] { new MenuItem { Name = "name2" } }) { Name = "name" }, }, "noSuchName", true, Array.Empty<MenuItem>() };
            yield return new object[] { new MenuItem[] { new MenuItem("text", new MenuItem[] { new MenuItem { Name = "name2" } }) { Name = "name" }, }, "noSuchName", false, Array.Empty<MenuItem>() };

            var menuItemChild = new MenuItem { Name = "name" };
            yield return new object[] { new MenuItem[] { new MenuItem("text", new MenuItem[] { menuItemChild }) }, "name", true, new MenuItem[] { menuItemChild } };
            yield return new object[] { new MenuItem[] { new MenuItem("text", new MenuItem[] { new MenuItem { Name = "name" } }) }, "name", false, Array.Empty<MenuItem>() };
        }

        [Theory]
        [MemberData(nameof(Find_TestData))]
        public void MenuItemCollection_Find_Invoke_ReturnsExpected(MenuItem[] items, string key, bool searchAllChildren, MenuItem[] expected)
        {
            var menu = new SubMenu(items);
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Equal(expected, collection.Find(key, searchAllChildren));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void MenuItemCollection_Find_NullOrEmptyKey_ThrowsArgumentNullException(string key)
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Throws<ArgumentNullException>("key", () => collection.Find(key, searchAllChildren: false));
        }

        public static IEnumerable<object[]> IndexOf_TestData()
        {
            var menuItem = new MenuItem();
            yield return new object[] { new SubMenu(new MenuItem[] { menuItem }), menuItem, 0 };
            yield return new object[] { new SubMenu(new MenuItem[] { new MenuItem() }), new MenuItem(), -1 };
            yield return new object[] { new SubMenu(new MenuItem[] { new MenuItem() }), null, -1 };
        }

        [Theory]
        [MemberData(nameof(IndexOf_TestData))]
        public void MenuItemCollection_IndexOf_Invoke_ReturnsExpected(Menu menu, MenuItem value, int expected)
        {
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Equal(expected, collection.IndexOf(value));
        }

        [Theory]
        [MemberData(nameof(IndexOf_TestData))]
        public void MenuItemCollection_IndexOf_IListInvoke_ReturnsExpected(Menu menu, MenuItem value, int expected)
        {
            IList collection = new Menu.MenuItemCollection(menu);
            Assert.Equal(expected, collection.IndexOf(value));
        }

        [Fact]
        public void MenuItemCollection_IndexOf_IListNotMenuItem_ReturnsMinusOne()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            IList collection = new Menu.MenuItemCollection(menu);
            Assert.Equal(-1, collection.IndexOf("value"));
        }

        [Theory]
        [InlineData(null, -1)]
        [InlineData("", -1)]
        [InlineData("name", 0)]
        [InlineData("NAME", 0)]
        [InlineData("noSuchName", -1)]
        public void MenuItemCollection_IndexOfKey_Invoke_ReturnsExpected(string key, int expected)
        {
            var menu = new SubMenu(new MenuItem[] { new MenuItem { Name = "name" } });
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Equal(expected, collection.IndexOfKey(key));

            // Call again to validate caching behaviour.
            Assert.Equal(expected, collection.IndexOfKey(key));
            Assert.Equal(-1, collection.IndexOfKey("noSuchKey"));
        }

        [Fact]
        public void MenuItemCollection_Insert_IListInvoke_Success()
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            IList collection = new Menu.MenuItemCollection(menu);

            var menuItem1 = new MenuItem("text1");
            collection.Insert(0, menuItem1);
            Assert.Same(menuItem1, Assert.Single(collection));
            Assert.Equal(menu, menuItem1.Parent);
            Assert.Equal(0, menuItem1.Index);

            var menuItem2 = new MenuItem("text1");
            collection.Insert(0, menuItem2);
            Assert.Equal(2, collection.Count);
            Assert.Same(menuItem2, collection[0]);
            Assert.Same(menuItem1, collection[1]);
            Assert.Equal(menu, menuItem1.Parent);
            Assert.Equal(1, menuItem1.Index);
            Assert.Equal(menu, menuItem2.Parent);
            Assert.Equal(0, menuItem2.Index);
        }

        [Theory]
        [InlineData("value")]
        [InlineData(null)]
        public void MenuItemCollection_Insert_IListNotMenuItem_ThrowsArgumentException(object value)
        {
            var menu = new SubMenu(Array.Empty<MenuItem>());
            IList collection = new Menu.MenuItemCollection(menu);
            Assert.Throws<ArgumentException>("value", () => collection.Insert(0, value));
        }

        [Fact]
        public void MenuItemCollection_Item_GetValidIndex_ReturnsExpected()
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem });
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Equal(menuItem, collection[0]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void MenuItemCollection_Item_GetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            var menu = new SubMenu(new MenuItem[] { new MenuItem() });
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [Fact]
        public void MenuItemCollection_Item_IListGetValidIndex_ReturnsExpected()
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem });
            IList collection = new Menu.MenuItemCollection(menu);
            Assert.Equal(menuItem, collection[0]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void MenuItemCollection_Item_IListGetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            var menu = new SubMenu(new MenuItem[] { new MenuItem() });
            IList collection = new Menu.MenuItemCollection(menu);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void MenuItemCollection_Item_IListSet_ThrowsNotSupportedException(int index)
        {
            var menu = new SubMenu(new MenuItem[] { new MenuItem() });
            IList collection = new Menu.MenuItemCollection(menu);
            Assert.Throws<NotSupportedException>(() => collection[index] = new MenuItem());
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("name", true)]
        [InlineData("NAME", true)]
        [InlineData("noSuchName", false)]
        public void MenuItemCollection_Item_GetKey_ReturnsExpected(string key, bool expected)
        {
            var menuItem = new MenuItem { Name = "name" };
            var menu = new SubMenu(new MenuItem[] { menuItem });
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Equal(expected ? menuItem : null, collection[key]);
        }

        [Fact]
        public void MenuItemCollection_Remove_MenuItem_Success()
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem });
            var collection = new Menu.MenuItemCollection(menu);
            collection.Remove(menuItem);
            Assert.Empty(collection);
            Assert.Null(menuItem.Parent);
            Assert.Equal(-1, menuItem.Index);
        }

        [Fact]
        public void MenuItemCollection_Remove_DifferentOwnerMenuMenuItem_Nop()
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem });
            var collection = new Menu.MenuItemCollection(menu);

            var otherMenuItem = new MenuItem();
            var otherMenu = new SubMenu(new MenuItem[] { otherMenuItem });
            collection.Remove(otherMenuItem);
            Assert.Equal(menuItem, Assert.Single(collection));
        }

        [Fact]
        public void MenuItemCollection_Remove_NoMenuMenuItem_Nop()
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem });
            var collection = new Menu.MenuItemCollection(menu);
            collection.Remove(new MenuItem());
            Assert.Equal(menuItem, Assert.Single(collection));
        }

        [Fact]
        public void MenuItemCollection_Remove_IListMenuItem_Success()
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem });
            IList collection = new Menu.MenuItemCollection(menu);
            collection.Remove(menuItem);
            Assert.Empty(collection);
            Assert.Null(menuItem.Parent);
            Assert.Equal(-1, menuItem.Index);
        }

        [Theory]
        [InlineData("value")]
        [InlineData(null)]
        public void MenuItemCollection_Remove_IListNotMenuItem_Nop(object value)
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem });
            IList collection = new Menu.MenuItemCollection(menu);
            collection.Remove(value);
            Assert.Equal(menuItem, Assert.Single(collection));
        }

        [Fact]
        public void MenuItemCollection_RemoveAt_Invoke_Success()
        {
            var menuItem = new MenuItem();
            var menu = new SubMenu(new MenuItem[] { menuItem });
            var collection = new Menu.MenuItemCollection(menu);
            collection.RemoveAt(0);

            Assert.Empty(collection);
            Assert.Null(menuItem.Parent);
            Assert.Equal(-1, menuItem.Index);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void MenuItemCollection_RemoveAt_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            var menu = new SubMenu(new MenuItem[] { new MenuItem() });
            var collection = new Menu.MenuItemCollection(menu);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
        }

        [Theory]
        [InlineData("name")]
        [InlineData("NAME")]
        public void MenuItemCollection_RemoveByKey_KeyExists_Success(string key)
        {
            var menuItem = new MenuItem { Name = "name" };
            var menu = new SubMenu(new MenuItem[] { menuItem });
            var collection = new Menu.MenuItemCollection(menu);
            collection.RemoveByKey(key);
            Assert.Empty(collection);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("noSuchName")]
        public void MenuItemCollection_RemoveByKey_NoSuchKey_Nop(string key)
        {
            var menuItem = new MenuItem { Name = "name" };
            var menu = new SubMenu(new MenuItem[] { menuItem });
            var collection = new Menu.MenuItemCollection(menu);
            collection.RemoveByKey(key);
            Assert.Equal(menuItem, Assert.Single(collection));
        }

        private class SubMenu : Menu
        {
            public SubMenu(params MenuItem[] items) : base(items)
            {
            }
        }
    }
}
