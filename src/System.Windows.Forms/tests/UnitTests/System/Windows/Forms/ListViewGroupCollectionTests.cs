// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListViewGroupCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ListViewGroupCollection_IList_GetProperties_ReturnsExpected()
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            Assert.Equal(0, collection.Count);
            Assert.False(collection.IsFixedSize);
            Assert.False(collection.IsReadOnly);
            Assert.True(collection.IsSynchronized);
            Assert.Same(collection, collection.SyncRoot);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Item_GetValidIndex_ReturnsExpected()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;

            var group = new ListViewGroup();
            collection.Add(group);
            Assert.Same(group, collection[0]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ListViewGroupCollection_Item_GetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            collection.Add(new ListViewGroup());
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Item_SetValidIndex_GetReturnsExpected()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            collection.Add(new ListViewGroup());

            var group = new ListViewGroup();
            collection[0] = group;
            Assert.Same(group, collection[0]);
            Assert.Same(listView, group.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Item_SetListViewGroupWithHandle_Success()
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            ListViewGroupCollection collection = listView.Groups;
            collection.Add(new ListViewGroup());

            var group = new ListViewGroup();
            collection[0] = group;
            Assert.Same(group, collection[0]);
            Assert.Same(listView, group.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Item_SetListViewGroupWithItems_Success()
        {
            using var listView = new ListView();
            var item = new ListViewItem();

            ListViewGroupCollection collection = listView.Groups;
            collection.Add(new ListViewGroup());

            var group = new ListViewGroup();
            group.Items.Add(new ListViewItem());
            group.Items.Add(item);
            listView.Items.Add(item);

            collection[0] = group;
            Assert.Same(group, Assert.Single(collection));
            Assert.Same(listView, group.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Item_SetListViewGroupWithItemsWithHandle_Success()
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            var item = new ListViewItem();

            ListViewGroupCollection collection = listView.Groups;
            collection.Add(new ListViewGroup());

            var group = new ListViewGroup();
            group.Items.Add(new ListViewItem());
            group.Items.Add(item);
            listView.Items.Add(item);

            collection[0] = group;
            Assert.Same(group, Assert.Single(collection));
            Assert.Same(listView, group.ListView);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ListViewGroupCollection_Item_SetAlreadyInCollection_Nop(int index)
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;

            var group1 = new ListViewGroup();
            var group2 = new ListViewGroup();
            collection.Add(group1);
            collection.Add(group2);

            collection[index] = group2;
            Assert.Same(group1, collection[0]);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Item_SetAlreadyInOtherCollection_GetReturnsExpected()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            collection.Add(new ListViewGroup());

            using var otherListView = new ListView();
            ListViewGroupCollection otherCollection = otherListView.Groups;

            var group = new ListViewGroup();
            otherCollection.Add(group);

            collection[0] = group;
            Assert.Same(group, collection[0]);
            Assert.Same(listView, group.ListView);
            Assert.Equal(group, Assert.Single(collection));
            Assert.Equal(group, Assert.Single(otherCollection));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Item_SetHasItemsFromOtherListView_ThrowsArgumentException()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var oldGroup = new ListViewGroup();
            collection.Add(oldGroup);
            using var otherListView = new ListView();

            var item = new ListViewItem();
            var group = new ListViewGroup();
            group.Items.Add(item);
            otherListView.Items.Add(item);

            Assert.Throws<ArgumentException>(null, () => collection[0] = group);
            Assert.Same(oldGroup, Assert.Single(collection.Cast<ListViewGroup>()));
            Assert.Null(group.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Item_SetNull_ThrowsArgumentNullException()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            collection.Add(new ListViewGroup());
            Assert.Throws<ArgumentNullException>("value", () => collection[0] = null);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ListViewGroupCollection_Item_SetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            collection.Add(new ListViewGroup());
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = new ListViewGroup());
        }

        [WinFormsFact]
        public void ListViewGroupCollection_IListItem_GetValidIndex_ReturnsExpected()
        {
            using var listView = new ListView();
            IList collection = listView.Groups;

            var group = new ListViewGroup();
            collection.Add(group);
            Assert.Same(group, collection[0]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ListViewGroupCollection_IListItem_GetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            collection.Add(new ListViewGroup());
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_IListItem_SetValidIndex_GetReturnsExpected()
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            collection.Add(new ListViewGroup());

            var group = new ListViewGroup();
            collection[0] = group;
            Assert.Same(group, collection[0]);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_IListItem_SetAlreadyInCollection_Nop()
        {
            using var listView = new ListView();
            IList collection = listView.Groups;

            var group1 = new ListViewGroup();
            var group2 = new ListViewGroup();
            collection.Add(group1);
            collection.Add(group2);

            collection[0] = group2;
            Assert.Same(group1, collection[0]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ListViewGroupCollection_IListItem_SetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            collection.Add(new ListViewGroup());
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = new ListViewGroup());
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("text")]
        public void ListViewGroupCollection_IListItem_SetInvalidValue_Nop(object value)
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            collection[0] = value;
        }

        [WinFormsTheory]
        [InlineData(null, 0)]
        [InlineData("text", 1)]
        public void ListViewGroupCollection_Item_GetStringExists_ReturnsExpected(string key, int expectedIndex)
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group1 = new ListViewGroup();
            var group2 = new ListViewGroup
            {
                Name = "text"
            };
            collection.Add(group1);
            collection.Add(group2);

            Assert.Equal(collection[expectedIndex], collection[key]);
        }

        [WinFormsTheory]
        [InlineData("")]
        [InlineData("longer")]
        [InlineData("sm")]
        [InlineData("tsxt")]
        [InlineData("TEXT")]
        public void ListViewGroupCollection_Item_GetNoSuchString_ReturnsNull(string key)
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group1 = new ListViewGroup();
            var group2 = new ListViewGroup
            {
                Name = "text"
            };
            collection.Add(group1);
            collection.Add(group2);

            Assert.Null(collection[key]);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Item_GetStringEmptyCollection_ReturnsNull()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            Assert.Null(collection["text"]);
        }

        [WinFormsTheory]
        [InlineData(null, 0)]
        [InlineData("text", 1)]
        public void ListViewGroupCollection_Item_SetStringExists_GetReturnsExpected(string key, int expectedIndex)
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group1 = new ListViewGroup();
            var group2 = new ListViewGroup
            {
                Name = "text"
            };
            collection.Add(group1);
            collection.Add(group2);

            var group3 = new ListViewGroup();
            collection[key] = group3;
            Assert.Same(group3, collection[expectedIndex]);
        }

        [WinFormsTheory]
        [InlineData("")]
        [InlineData("longer")]
        [InlineData("sm")]
        [InlineData("tsxt")]
        [InlineData("TEXT")]
        public void ListViewGroupCollection_Item_SetNoSuchString_Nop(string key)
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group1 = new ListViewGroup();
            var group2 = new ListViewGroup
            {
                Name = "text"
            };
            collection.Add(group1);
            collection.Add(group2);

            var group3 = new ListViewGroup();
            collection[key] = group3;
            Assert.Same(group1, collection[0]);
            Assert.Same(group2, collection[1]);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Item_SetStringEmptyCollection_Nop()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            collection["text"] = new ListViewGroup();
            Assert.Empty(collection);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Item_SetStringNullValue_ThrowsArgumentNullException()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            Assert.Throws<ArgumentNullException>("value", () => collection["key"] = null);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Add_ListViewGroup_Success()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group1 = new ListViewGroup();
            Assert.Equal(0, collection.Add(group1));
            Assert.Same(group1, Assert.Single(collection));
            Assert.Same(listView, group1.ListView);

            // Add another.
            var group2 = new ListViewGroup();
            Assert.Equal(1, collection.Add(group2));
            Assert.Equal(new ListViewGroup[] { group1, group2 }, collection.Cast<ListViewGroup>());
            Assert.Same(listView, group2.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Add_ListViewGroupWithHandle_Success()
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            ListViewGroupCollection collection = listView.Groups;
            var group1 = new ListViewGroup();
            Assert.Equal(0, collection.Add(group1));
            Assert.Same(group1, Assert.Single(collection));
            Assert.Same(listView, group1.ListView);

            // Add another.
            var group2 = new ListViewGroup();
            Assert.Equal(1, collection.Add(group2));
            Assert.Equal(new ListViewGroup[] { group1, group2 }, collection.Cast<ListViewGroup>());
            Assert.Same(listView, group2.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Add_ListViewGroupWithItems_Success()
        {
            using var listView = new ListView();
            var item = new ListViewItem();

            ListViewGroupCollection collection = listView.Groups;
            var group = new ListViewGroup();
            group.Items.Add(new ListViewItem());
            group.Items.Add(item);
            listView.Items.Add(item);

            Assert.Equal(0, collection.Add(group));
            Assert.Same(group, Assert.Single(collection));
            Assert.Same(listView, group.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Add_ListViewGroupWithItemsWithHandle_Success()
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            var item = new ListViewItem();

            ListViewGroupCollection collection = listView.Groups;
            var group1 = new ListViewGroup();
            group1.Items.Add(new ListViewItem());
            group1.Items.Add(item);
            listView.Items.Add(item);

            Assert.Equal(0, collection.Add(group1));
            Assert.Same(group1, Assert.Single(collection));
            Assert.Same(listView, group1.ListView);

            var group2 = new ListViewGroup();
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Add_AlreadyInCollection_Nop()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group1 = new ListViewGroup();
            var group2 = new ListViewGroup();
            Assert.Equal(0, collection.Add(group1));
            Assert.Equal(1, collection.Add(group2));

            Assert.Equal(-1, collection.Add(group1));
            Assert.Equal(2, collection.Count);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Addd_AlreadyInOtherCollection_GetReturnsExpected()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;

            using var otherListView = new ListView();
            ListViewGroupCollection otherCollection = otherListView.Groups;

            var group = new ListViewGroup();
            otherCollection.Add(group);

            // The group appears to belong to two list views.
            collection.Add(group);
            Assert.Same(group, collection[0]);
            Assert.Same(listView, group.ListView);
            Assert.Equal(group, Assert.Single(collection));
            Assert.Equal(group, Assert.Single(otherCollection));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Add_NullGroup_ThrowsArgumentNullException()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            Assert.Throws<ArgumentNullException>("group", () => collection.Add(null));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Add_HasItemsFromOtherListView_ThrowsArgumentException()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            using var otherListView = new ListView();

            var item = new ListViewItem();
            var group = new ListViewGroup();
            group.Items.Add(item);
            otherListView.Items.Add(item);

            Assert.Throws<ArgumentException>(null, () => collection.Add(group));
            Assert.Empty(collection);
        }

        [WinFormsTheory]
        [InlineData(null, null, "")]
        [InlineData("", "", "")]
        [InlineData("key", "headerText", "headerText")]
        public void ListViewGroupCollection_Add_StringString_Success(string key, string headerText, string expectedHeaderText)
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            collection.Add(key, headerText);

            ListViewGroup group = Assert.Single(collection.Cast<ListViewGroup>());
            Assert.Equal(key, group.Name);
            Assert.Equal(expectedHeaderText, group.Header);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_IListAdd_ListViewGroup_Success()
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(group);
            Assert.Same(group, Assert.Single(collection));
            Assert.Same(listView, group.ListView);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("text")]
        public void ListViewGroupCollection_IListAdd_NotListViewGroup_ThrowsArgumentException(object value)
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            Assert.Throws<ArgumentException>("value", () => collection.Add(value));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_AddRange_ListViewGroupArray_Success()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group1 = new ListViewGroup();
            var group2 = new ListViewGroup();
            var items = new ListViewGroup[] { group1, group2 };
            collection.AddRange(items);

            Assert.Equal(2, collection.Count);
            Assert.Same(group1, collection[0]);
            Assert.Same(group2, collection[1]);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_AddRange_ListViewGroupCollection_Success()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            using var otherListView = new ListView();
            ListViewGroupCollection otherCollection = listView.Groups;

            var group1 = new ListViewGroup();
            var group2 = new ListViewGroup();
            otherCollection.Add(group1);
            otherCollection.Add(group2);

            collection.AddRange(otherCollection);

            Assert.Equal(2, collection.Count);
            Assert.Same(group1, collection[0]);
            Assert.Same(group2, collection[1]);
            Assert.Equal(2, otherCollection.Count);
            Assert.Same(group1, otherCollection[0]);
            Assert.Same(group2, otherCollection[1]);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_AddRange_NullGroups_ThrowsArgumentNullException()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            Assert.Throws<ArgumentNullException>("groups", () => collection.AddRange((ListViewGroup[])null));
            Assert.Throws<ArgumentNullException>("groups", () => collection.AddRange((ListViewGroupCollection)null));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_AddRange_NullValueInGroups_ThrowsArgumentNullException()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group = new ListViewGroup();
            Assert.Throws<ArgumentNullException>("group", () => collection.AddRange(new ListViewGroup[] { group, null }));
            Assert.Same(group, Assert.Single(collection.Cast<ListViewGroup>()));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Clear_Invoke_Success()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(group);

            collection.Clear();
            Assert.Empty(collection);
            Assert.Null(group.ListView);

            // Clear again.
            collection.Clear();
            Assert.Empty(collection);
            Assert.Null(group.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Clear_InvokeListViewHasHandle_Success()
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            ListViewGroupCollection collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(group);

            collection.Clear();
            Assert.Empty(collection);
            Assert.Null(group.ListView);

            // Clear again.
            collection.Clear();
            Assert.Empty(collection);
            Assert.Null(group.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Clear_Empty_Success()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;

            collection.Clear();
            Assert.Empty(collection);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Contains_Invoke_ReturnsExpected()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(group);

            Assert.True(collection.Contains(group));
            Assert.False(collection.Contains(new ListViewGroup()));
            Assert.False(collection.Contains(null));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Contains_Empty_ReturnsFalse()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;

            Assert.False(collection.Contains(new ListViewGroup()));
            Assert.False(collection.Contains(null));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_IListContains_Invoke_ReturnsExpected()
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(group);

            Assert.True(collection.Contains(group));
            Assert.False(collection.Contains(new ListViewGroup()));
            Assert.False(collection.Contains(new object()));
            Assert.False(collection.Contains(null));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_IListContains_Empty_ReturnsFalse()
        {
            using var listView = new ListView();
            IList collection = listView.Groups;

            Assert.False(collection.Contains(new ListViewGroup()));
            Assert.False(collection.Contains(new object()));
            Assert.False(collection.Contains(null));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_IndexOf_Invoke_ReturnsExpected()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(group);

            Assert.Equal(0, collection.IndexOf(group));
            Assert.Equal(-1, collection.IndexOf(new ListViewGroup()));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_IndexOf_Empty_ReturnsFalse()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;

            Assert.Equal(-1, collection.IndexOf(new ListViewGroup()));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_IListIndexOf_Invoke_ReturnsExpected()
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(group);

            Assert.Equal(0, collection.IndexOf(group));
            Assert.Equal(-1, collection.IndexOf(new ListViewGroup()));
            Assert.Equal(-1, collection.IndexOf(new object()));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_IListIndexOf_Empty_ReturnsMinusOne()
        {
            using var listView = new ListView();
            IList collection = listView.Groups;

            Assert.Equal(-1, collection.IndexOf(new ListViewGroup()));
            Assert.Equal(-1, collection.IndexOf(new object()));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Insert_ListViewGroup_Success()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(new ListViewGroup());
            collection.Insert(1, group);
            Assert.Equal(2, collection.Count);
            Assert.Same(group, collection[1]);
            Assert.Same(listView, group.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Insert_ListViewGroupWithHandle_Success()
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            ListViewGroupCollection collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(new ListViewGroup());
            collection.Insert(1, group);
            Assert.Equal(2, collection.Count);
            Assert.Same(group, collection[1]);
            Assert.Same(listView, group.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Insert_ListViewGroupWithItems_Success()
        {
            using var listView = new ListView();
            var item = new ListViewItem();

            ListViewGroupCollection collection = listView.Groups;
            var group = new ListViewGroup();
            group.Items.Add(new ListViewItem());
            group.Items.Add(item);
            listView.Items.Add(item);

            collection.Insert(0, group);
            Assert.Same(group, Assert.Single(collection));
            Assert.Same(listView, group.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Insert_ListViewGroupWithItemsWithHandle_Success()
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            var item = new ListViewItem();

            ListViewGroupCollection collection = listView.Groups;
            var group = new ListViewGroup();
            group.Items.Add(new ListViewItem());
            group.Items.Add(item);
            listView.Items.Add(item);

            collection.Insert(0, group);
            Assert.Same(group, Assert.Single(collection));
            Assert.Same(listView, group.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Insert_AlreadyInOtherCollection_GetReturnsExpected()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            collection.Add(new ListViewGroup());

            using var otherListView = new ListView();
            ListViewGroupCollection otherCollection = otherListView.Groups;

            var group = new ListViewGroup();
            otherCollection.Add(group);

            // The group appears to belong to two list views.
            collection.Insert(0, group);
            Assert.Same(group, collection[0]);
            Assert.Same(listView, group.ListView);
            Assert.Equal(group, collection[0]);
            Assert.Equal(group, Assert.Single(otherCollection));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ListViewGroupCollection_Insert_SetAlreadyInCollection_Nop(int index)
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;

            var group1 = new ListViewGroup();
            var group2 = new ListViewGroup();
            collection.Add(group1);
            collection.Add(group2);

            collection.Insert(index, group2);
            Assert.Same(group1, collection[0]);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Insert_HasItemsFromOtherListView_ThrowsArgumentException()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            using var otherListView = new ListView();

            var item = new ListViewItem();
            var group = new ListViewGroup();
            group.Items.Add(item);
            otherListView.Items.Add(item);

            Assert.Throws<ArgumentException>(null, () => collection.Insert(0, group));
            Assert.Empty(collection);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(2)]
        public void ListViewGroupCollection_Insert_NullGroup_ThrowsArgumentNullException(int index)
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            collection.Add(new ListViewGroup());
            Assert.Throws<ArgumentNullException>("group", () => collection.Insert(index, null));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ListViewGroupCollection_Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, new ListViewGroup()));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_IListInsert_ListViewGroup_Success()
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(new ListViewGroup());
            collection.Insert(1, group);
            Assert.Equal(2, collection.Count);
            Assert.Same(group, collection[1]);
            Assert.Same(listView, group.ListView);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("text")]
        public void ListViewGroupCollection_IListInsert_InvalidItem_Nop(object value)
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            collection.Insert(-1, value);
            collection.Insert(0, value);
            Assert.Empty(collection);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ListViewGroupCollection_IListInsert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, new ListViewGroup()));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Remove_ListViewGroup_Success()
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(group);

            // Remove null.
            collection.Remove(null);
            Assert.Same(group, Assert.Single(collection));

            collection.Remove(group);
            Assert.Empty(collection);
            Assert.Null(group.ListView);

            // Remove again.
            collection.Remove(group);
            Assert.Empty(collection);
            Assert.Null(group.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_Remove_ListViewGroupWithHandle_Success()
        {
            using var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            IList collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(group);

            // Remove null.
            collection.Remove(null);
            Assert.Same(group, Assert.Single(collection));

            collection.Remove(group);
            Assert.Empty(collection);
            Assert.Null(group.ListView);

            // Remove again.
            collection.Remove(group);
            Assert.Empty(collection);
            Assert.Null(group.ListView);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_IListRemove_ListViewGroup_Success()
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(group);

            collection.Remove(group);
            Assert.Empty(collection);
            Assert.Null(group.ListView);

            // Remove again.
            collection.Remove(group);
            Assert.Empty(collection);
            Assert.Null(group.ListView);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("text")]
        public void ListViewGroupCollection_IListRemove_InvalidItem_Nop(object value)
        {
            using var listView = new ListView();
            IList collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(group);

            collection.Remove(value);
            Assert.Same(group, Assert.Single(collection));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_RemoveAt_ValidIndex_Success()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(group);
            collection.Add(new ListViewGroup());
            collection.Add(new ListViewGroup());
            collection.Add(new ListViewGroup());

            // Remove from start.
            collection.RemoveAt(0);
            Assert.Equal(3, collection.Count);

            // Remove from middle.
            collection.RemoveAt(1);
            Assert.Equal(2, collection.Count);

            // Remove from end.
            collection.RemoveAt(1);
            Assert.Single(collection);

            // Remove only.
            collection.RemoveAt(0);
            Assert.Empty(collection);
            Assert.Null(group.ListView);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ListViewGroupCollection_RemoveAt_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
        }

        [WinFormsFact]
        public void ListViewGroupCollection_CopyTo_NonEmpty_Success()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var group = new ListViewGroup();
            collection.Add(group);

            var array = new object[] { 1, 2, 3 };
            collection.CopyTo(array, 1);
            Assert.Equal(new object[] { 1, group, 3 }, array);
        }

        [WinFormsFact]
        public void ListViewGroupCollection_CopyTo_Empty_Nop()
        {
            using var listView = new ListView();
            ListViewGroupCollection collection = listView.Groups;
            var array = new object[] { 1, 2, 3 };
            collection.CopyTo(array, 0);
            Assert.Equal(new object[] { 1, 2, 3 }, array);
        }
    }
}
