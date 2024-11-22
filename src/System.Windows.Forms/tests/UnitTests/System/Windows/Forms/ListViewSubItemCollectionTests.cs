// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ListViewSubItemCollectionTests
{
    [Fact]
    public void ListViewSubItemCollection_Ctor_ListViewItem()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        Assert.Empty(collection);
    }

    [Fact]
    public void ListViewSubItemCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new ListViewItem.ListViewSubItemCollection(null));
    }

    [Fact]
    public void ListViewSubItemCollection_IList_GetProperties_ReturnsExpected()
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);
        Assert.Empty(collection);
        Assert.False(collection.IsFixedSize);
        Assert.False(collection.IsReadOnly);
        Assert.True(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [Fact]
    public void ListViewSubItemCollection_Item_GetValidIndex_ReturnsExpected()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);

        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);
        Assert.Same(subItem, collection[0]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ListViewSubItemCollection_Item_GetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item)
        {
            new ListViewItem.ListViewSubItem()
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [Fact]
    public void ListViewSubItemCollection_Item_SetValidIndex_GetReturnsExpected()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item)
        {
            new ListViewItem.ListViewSubItem()
        };

        var subItem = new ListViewItem.ListViewSubItem();
        collection[0] = subItem;
        Assert.Same(subItem, collection[0]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ListViewSubItemCollection_Item_SetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item)
        {
            new ListViewItem.ListViewSubItem()
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = new ListViewItem.ListViewSubItem());
    }

    [Fact]
    public void ListViewSubItemCollection_Item_SetNull_ThrowsArgumentNullException()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item)
        {
            new ListViewItem.ListViewSubItem()
        };
        Assert.Throws<ArgumentNullException>("value", () => collection[0] = null);
    }

    [Fact]
    public void ListViewSubItemCollection_IListItem_GetValidIndex_ReturnsExpected()
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);

        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);
        Assert.Same(subItem, collection[0]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ListViewSubItemCollection_IListItem_GetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item)
        {
            new ListViewItem.ListViewSubItem()
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [Fact]
    public void ListViewSubItemCollection_IListItem_SetValidIndex_GetReturnsExpected()
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item)
        {
            new ListViewItem.ListViewSubItem()
        };

        var subItem = new ListViewItem.ListViewSubItem();
        collection[0] = subItem;
        Assert.Same(subItem, collection[0]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ListViewSubItemCollection_IListItem_SetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item)
        {
            new ListViewItem.ListViewSubItem()
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("text")]
    public void ListViewSubItemCollection_IList_SetInvalidValue_ThrowsArgumentException(object value)
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);
        Assert.Throws<ArgumentException>("value", () => collection[0] = value);
    }

    [Theory]
    [InlineData(null, -1)]
    [InlineData("", -1)]
    [InlineData("longer", -1)]
    [InlineData("sm", -1)]
    [InlineData("text", 1)]
    [InlineData("tsxt", -1)]
    [InlineData("TEXT", 1)]
    public void ListViewSubItemCollection_Item_GetString_ReturnsExpected(string key, int expectedIndex)
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem1 = new ListViewItem.ListViewSubItem();
        var subItem2 = new ListViewItem.ListViewSubItem
        {
            Name = "text"
        };
        collection.Add(subItem1);
        collection.Add(subItem2);

        Assert.Equal(expectedIndex != -1 ? collection[expectedIndex] : null, collection[key]);
    }

    [Fact]
    public void ListViewSubItemCollection_Add_ListViewSubItem_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);
        Assert.Same(subItem, Assert.Single(collection));
        Assert.Equal(item, subItem._owner);
    }

    [Fact]
    public void ListViewSubItemCollection_Add_ManyItems_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        for (int i = 0; i < 4096; i++)
        {
            var subItem = new ListViewItem.ListViewSubItem();
            collection.Add(subItem);
            Assert.Same(subItem, collection[i]);
        }

        Assert.Throws<InvalidOperationException>(() => collection.Add(new ListViewItem.ListViewSubItem()));
    }

    [Fact]
    public void ListViewSubItemCollection_Add_ListViewSubItemExists_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);
        collection.Add(subItem);
        Assert.Equal(2, collection.Count);
        Assert.Equal(subItem, collection[0]);
        Assert.Equal(subItem, collection[1]);
        Assert.Equal(item, subItem._owner);
    }

    [Fact]
    public void ListViewSubItemCollection_Add_ListViewSubItemExistsInOtherCollection_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);

        var otherCollection = new ListViewItem.ListViewSubItemCollection(item);

        var subItem = new ListViewItem.ListViewSubItem();
        otherCollection.Add(subItem);

        collection.Add(subItem);
        Assert.Same(subItem, collection[0]);
        Assert.Same(subItem, otherCollection[0]);
        Assert.Equal(item, subItem._owner);
    }

    [Theory]
    [NormalizedStringData]
    public void ListViewSubItemCollection_Add_String_Success(string text, string expectedText)
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item)
        {
            text
        };
        ListViewItem.ListViewSubItem subItem = Assert.Single(collection.Cast<ListViewItem.ListViewSubItem>());
        Assert.Equal(expectedText, subItem.Text);
        Assert.Equal(item, subItem._owner);
    }

    public static IEnumerable<object[]> Add_String_Color_Color_Font_TestData()
    {
        yield return new object[] { null, Color.Empty, Color.Empty, null, SystemColors.WindowText, SystemColors.Window, string.Empty };
        yield return new object[] { string.Empty, Color.Red, Color.Blue, SystemFonts.MenuFont, Color.Red, Color.Blue, string.Empty };
        yield return new object[] { "reasonable", Color.Red, Color.Blue, SystemFonts.MenuFont, Color.Red, Color.Blue, "reasonable" };
    }

    [Theory]
    [MemberData(nameof(Add_String_Color_Color_Font_TestData))]
    public void ListViewSubItemCollection_Add_String_Color_Color_Font_Success(string text, Color foreColor, Color backColor, Font font, Color expectedForeColor, Color expectedBackColor, string expectedText)
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item)
        {
            { text, foreColor, backColor, font }
        };
        ListViewItem.ListViewSubItem subItem = Assert.Single(collection.Cast<ListViewItem.ListViewSubItem>());
        Assert.Same(expectedText, subItem.Text);
        Assert.Equal(expectedForeColor, subItem.ForeColor);
        Assert.Equal(expectedBackColor, subItem.BackColor);
        Assert.Equal(font ?? Control.DefaultFont, subItem.Font);
        Assert.Equal(item, subItem._owner);
    }

    [Fact]
    public void ListViewSubItemCollection_Add_NullItem_ThrowsArgumentNullException()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        Assert.Throws<ArgumentNullException>("item", () => collection.Add((ListViewItem.ListViewSubItem)null));
    }

    [Fact]
    public void ListViewSubItemCollection_IListAdd_ListViewSubItem_Success()
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);
        Assert.Same(subItem, Assert.Single(collection));
        Assert.Equal(item, subItem._owner);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("text")]
    public void ListViewSubItemCollection_IListAdd_NotListViewSubItem_ThrowsArgumentException(object value)
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);
        Assert.Throws<ArgumentException>("item", () => collection.Add(value));
    }

    [Fact]
    public void ListViewSubItemCollection_AddRange_ListViewSubItemArray_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem1 = new ListViewItem.ListViewSubItem(null, "text1");
        var subItem2 = new ListViewItem.ListViewSubItem(null, "text2");
        var items = new ListViewItem.ListViewSubItem[] { subItem1, null, subItem2 };
        collection.AddRange(items);

        Assert.Equal(2, collection.Count);
        Assert.Same(subItem1, collection[0]);
        Assert.Same(subItem2, collection[1]);
    }

    [Fact]
    public void ListViewSubItemCollection_AddRange_LargeListViewSubItemArrayWithItems_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem1 = new ListViewItem.ListViewSubItem(null, "text1");
        var subItem2 = new ListViewItem.ListViewSubItem(null, "text2");
        var subItem3 = new ListViewItem.ListViewSubItem(null, "text3");
        var subItem4 = new ListViewItem.ListViewSubItem(null, "text4");
        var items = new ListViewItem.ListViewSubItem[] { subItem1, null, subItem2, subItem3, subItem4 };
        collection.AddRange(items);

        Assert.Equal(4, collection.Count);
        Assert.Same(subItem1, collection[0]);
        Assert.Same(subItem2, collection[1]);
        Assert.Same(subItem3, collection[2]);
        Assert.Same(subItem4, collection[3]);
    }

    [Fact]
    public void ListViewSubItemCollection_AddRange_LargeListViewSubItemArrayWithoutItems_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem1 = new ListViewItem.ListViewSubItem(null, "text1");
        var subItem2 = new ListViewItem.ListViewSubItem(null, "text2");
        var subItem3 = new ListViewItem.ListViewSubItem(null, "text3");
        var subItem4 = new ListViewItem.ListViewSubItem(null, "text4");
        var subItem5 = new ListViewItem.ListViewSubItem(null, "text5");
        var subItem6 = new ListViewItem.ListViewSubItem(null, "text6");
        var subItem7 = new ListViewItem.ListViewSubItem(null, "text7");
        var subItem8 = new ListViewItem.ListViewSubItem(null, "text8");
        var subItem9 = new ListViewItem.ListViewSubItem(null, "text8");
        var items = new ListViewItem.ListViewSubItem[] { subItem2, null, subItem3, subItem4, subItem5, subItem6, subItem7, subItem8, subItem9 };
        collection.Add(subItem1);
        collection.AddRange(items);

        Assert.Equal(9, collection.Count);
        Assert.Same(subItem1, collection[0]);
        Assert.Same(subItem2, collection[1]);
        Assert.Same(subItem3, collection[2]);
        Assert.Same(subItem4, collection[3]);
        Assert.Same(subItem5, collection[4]);
        Assert.Same(subItem6, collection[5]);
        Assert.Same(subItem7, collection[6]);
        Assert.Same(subItem8, collection[7]);
        Assert.Same(subItem9, collection[8]);
    }

    [Fact]
    public void ListViewSubItemCollection_AddRange_StringArray_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        string[] items = ["text1", null, "text2"];
        collection.AddRange(items);

        Assert.Equal(2, collection.Count);
        Assert.Equal("text1", collection[0].Text);
        Assert.Equal("text2", collection[1].Text);
    }

    public static IEnumerable<object[]> AddRange_StringArrayWithStyles_TestData()
    {
        yield return new object[] { Color.Empty, Color.Empty, null, SystemColors.WindowText, SystemColors.Window };
        yield return new object[] { Color.Red, Color.Blue, SystemFonts.MenuFont, Color.Red, Color.Blue };
        yield return new object[] { Color.Red, Color.Blue, SystemFonts.MenuFont, Color.Red, Color.Blue };
    }

    [Theory]
    [MemberData(nameof(AddRange_StringArrayWithStyles_TestData))]
    public void ListViewSubItemCollection_AddRange_StringArrayWithStyles_Success(Color foreColor, Color backColor, Font font, Color expectedForeColor, Color expectedBackColor)
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        string[] items = ["text1", null, "text2"];
        collection.AddRange(items, foreColor, backColor, font);

        Assert.Equal(2, collection.Count);
        Assert.Equal("text1", collection[0].Text);
        Assert.Equal(expectedForeColor, collection[0].ForeColor);
        Assert.Equal(expectedBackColor, collection[0].BackColor);
        Assert.Equal(font ?? Control.DefaultFont, collection[0].Font);
        Assert.Equal("text2", collection[1].Text);
        Assert.Equal(expectedForeColor, collection[1].ForeColor);
        Assert.Equal(expectedBackColor, collection[1].BackColor);
        Assert.Equal(font ?? Control.DefaultFont, collection[1].Font);
    }

    [Fact]
    public void ListViewSubItemCollection_AddRange_NullList_ThrowsArgumentNullException()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        Assert.Throws<ArgumentNullException>("items", () => collection.AddRange((ListViewItem.ListViewSubItem[])null));
        Assert.Throws<ArgumentNullException>("items", () => collection.AddRange((string[])null));
        Assert.Throws<ArgumentNullException>("items", () => collection.AddRange((string[])null, Color.Red, Color.Blue, SystemFonts.MenuFont));
    }

    [Fact]
    public void ListViewSubItemCollection_Clear_Invoke_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Null(subItem._owner);

        // Clear again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Null(subItem._owner);
    }

    [Fact]
    public void ListViewSubItemCollection_Clear_Empty_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);

        collection.Clear();
        Assert.Empty(collection);
    }

    [Fact]
    public void ListViewSubItemCollection_Contains_Invoke_ReturnsExpected()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);

        Assert.True(collection.Contains(subItem));
        Assert.False(collection.Contains(new ListViewItem.ListViewSubItem()));
        Assert.False(collection.Contains(null));
    }

    [Fact]
    public void ListViewSubItemCollection_Contains_Empty_ReturnsFalse()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);

        Assert.False(collection.Contains(new ListViewItem.ListViewSubItem()));
        Assert.False(collection.Contains(null));
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("longer", false)]
    [InlineData("sm", false)]
    [InlineData("text", true)]
    [InlineData("tsxt", false)]
    [InlineData("TEXT", true)]
    public void ListViewSubItemCollection_ContainsKey_Invoke_ReturnsExpected(string key, bool expected)
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem1 = new ListViewItem.ListViewSubItem();
        var subItem2 = new ListViewItem.ListViewSubItem
        {
            Name = "text"
        };
        collection.Add(subItem1);
        collection.Add(subItem2);

        Assert.Equal(expected, collection.ContainsKey(key));
    }

    [Fact]
    public void ListViewSubItemCollection_ContainsKey_Empty_ReturnsFalse()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);

        Assert.False(collection.ContainsKey("text"));
        Assert.False(collection.ContainsKey(null));
    }

    [Fact]
    public void ListViewSubItemCollection_IListContains_Invoke_ReturnsExpected()
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);

        Assert.True(collection.Contains(subItem));
        Assert.False(collection.Contains(new ListViewItem.ListViewSubItem()));
        Assert.False(collection.Contains(new object()));
        Assert.False(collection.Contains(null));
    }

    [Fact]
    public void ListViewSubItemCollection_IListContains_Empty_ReturnsFalse()
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);

        Assert.False(collection.Contains(new ListViewItem.ListViewSubItem()));
        Assert.False(collection.Contains(new object()));
        Assert.False(collection.Contains(null));
    }

    [Fact]
    public void ListViewSubItemCollection_IndexOf_Invoke_ReturnsExpected()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);

        Assert.Equal(0, collection.IndexOf(subItem));
        Assert.Equal(-1, collection.IndexOf(new ListViewItem.ListViewSubItem()));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [Fact]
    public void ListViewSubItemCollection_IndexOf_Empty_ReturnsFalse()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);

        Assert.Equal(-1, collection.IndexOf(new ListViewItem.ListViewSubItem()));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [Theory]
    [InlineData(null, -1)]
    [InlineData("", -1)]
    [InlineData("longer", -1)]
    [InlineData("sm", -1)]
    [InlineData("text", 1)]
    [InlineData("tsxt", -1)]
    [InlineData("TEXT", 1)]
    public void ListViewSubItemCollection_IndexOfKey_Invoke_ReturnsExpected(string key, int expected)
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem1 = new ListViewItem.ListViewSubItem();
        var subItem2 = new ListViewItem.ListViewSubItem
        {
            Name = "text"
        };
        collection.Add(subItem1);
        collection.Add(subItem2);

        Assert.Equal(expected, collection.IndexOfKey(key));

        // Call again to validate caching behavior.
        Assert.Equal(expected, collection.IndexOfKey(key));
        Assert.Equal(-1, collection.IndexOfKey("noSuchKey"));
    }

    [Fact]
    public void ListViewSubItemCollection_IndexOfKey_Empty_ReturnsFalse()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);

        Assert.Equal(-1, collection.IndexOfKey("text"));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [Fact]
    public void ListViewSubItemCollection_IListIndexOf_Invoke_ReturnsExpected()
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);

        Assert.Equal(0, collection.IndexOf(subItem));
        Assert.Equal(-1, collection.IndexOf(new ListViewItem.ListViewSubItem()));
        Assert.Equal(-1, collection.IndexOf(new object()));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [Fact]
    public void ListViewSubItemCollection_IListIndexOf_Empty_ReturnsMinusOne()
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);

        Assert.Equal(-1, collection.IndexOf(new ListViewItem.ListViewSubItem()));
        Assert.Equal(-1, collection.IndexOf(new object()));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [Fact]
    public void ListViewSubItemCollection_Insert_ListViewSubItem_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(new ListViewItem.ListViewSubItem());
        collection.Insert(1, subItem);
        Assert.Equal(2, collection.Count);
        Assert.Same(subItem, collection[1]);
        Assert.Same(item, subItem._owner);
    }

    [Fact]
    public void ListViewSubItemCollection_Insert_ManyItems_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        for (int i = 0; i < 4096; i++)
        {
            var subItem = new ListViewItem.ListViewSubItem();
            collection.Insert(0, subItem);
            Assert.Same(subItem, collection[0]);
        }

        Assert.Throws<InvalidOperationException>(() => collection.Add(new ListViewItem.ListViewSubItem()));
    }

    [Fact]
    public void ListViewSubItemCollection_Insert_NullItem_ThrowsArgumentNullException()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item)
        {
            new ListViewItem.ListViewSubItem()
        };
        Assert.Throws<ArgumentNullException>("item", () => collection.Insert(1, null));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ListViewSubItemCollection_Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, null));
    }

    [Fact]
    public void ListViewSubItemCollection_IListInsert_ListViewSubItem_Success()
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(new ListViewItem.ListViewSubItem());
        collection.Insert(1, subItem);
        Assert.Equal(2, collection.Count);
        Assert.Same(subItem, collection[1]);
        Assert.Same(item, subItem._owner);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("text")]
    public void ListViewSubItemCollection_IListInsert_InvalidItem_ThrowsArgumentException(object value)
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);
        Assert.Throws<ArgumentException>("item", () => collection.Insert(0, value));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ListViewSubItemCollection_IListInsert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, new ListViewItem.ListViewSubItem()));
    }

    [Fact]
    public void ListViewSubItemCollection_Remove_ListViewSubItem_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);

        // Remove null.
        collection.Remove(null);
        Assert.Same(subItem, Assert.Single(collection));

        collection.Remove(subItem);
        Assert.Empty(collection);
        Assert.Null(subItem._owner);

        // Remove again.
        collection.Remove(subItem);
        Assert.Empty(collection);
        Assert.Null(subItem._owner);
    }

    [Fact]
    public void ListViewSubItemCollection_IListRemove_ListViewSubItem_Success()
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);

        collection.Remove(subItem);
        Assert.Empty(collection);
        Assert.Null(subItem._owner);

        // Remove again.
        collection.Remove(subItem);
        Assert.Empty(collection);
        Assert.Null(subItem._owner);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("text")]
    public void ListViewSubItemCollection_IListRemove_InvalidItem_Nop(object value)
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);

        collection.Remove(value);
        Assert.Same(subItem, Assert.Single(collection));
    }

    [Fact]
    public void ListViewSubItemCollection_RemoveAt_ValidIndex_Success()
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem1 = new ListViewItem.ListViewSubItem();
        var subItem2 = new ListViewItem.ListViewSubItem();
        var subItem3 = new ListViewItem.ListViewSubItem();
        var subItem4 = new ListViewItem.ListViewSubItem();
        collection.Add(subItem1);
        collection.Add(subItem2);
        collection.Add(subItem3);
        collection.Add(subItem4);

        // Remove from start.
        collection.RemoveAt(0);
        Assert.Equal(3, collection.Count);
        Assert.Null(subItem1._owner);
        Assert.Same(item, subItem2._owner);
        Assert.Same(item, subItem3._owner);
        Assert.Same(item, subItem4._owner);

        // Remove from middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Null(subItem1._owner);
        Assert.Same(item, subItem2._owner);
        Assert.Null(subItem3._owner);
        Assert.Same(item, subItem4._owner);

        // Remove from end.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Null(subItem1._owner);
        Assert.Same(item, subItem2._owner);
        Assert.Null(subItem3._owner);
        Assert.Null(subItem4._owner);

        // Remove only.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Null(subItem1._owner);
        Assert.Null(subItem2._owner);
        Assert.Null(subItem3._owner);
        Assert.Null(subItem4._owner);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void ListViewSubItemCollection_RemoveAt_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item)
        {
            new ListViewItem.ListViewSubItem()
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ListViewSubItemCollection_RemoveAt_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
    }

    [Theory]
    [InlineData(null, 1)]
    [InlineData("", 1)]
    [InlineData("longer", 1)]
    [InlineData("sm", 1)]
    [InlineData("text", 0)]
    [InlineData("tsxt", 1)]
    [InlineData("TEXT", 0)]
    public void ListViewSubItemCollection_RemoveByKey_Invoke_Success(string key, int expectedCount)
    {
        ListViewItem item = new();
        var collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem
        {
            Name = "text"
        };
        collection.Add(subItem);

        collection.RemoveByKey(key);
        Assert.Equal(expectedCount, collection.Count);
        if (expectedCount == 1)
        {
            Assert.Same(item, subItem._owner);
        }
        else
        {
            Assert.Null(subItem._owner);
        }
    }

    [Fact]
    public void ListViewSubItemCollection_CopyTo_NonEmpty_Success()
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);
        var subItem = new ListViewItem.ListViewSubItem();
        collection.Add(subItem);

        object[] array = [1, 2, 3];
        collection.CopyTo(array, 1);
        Assert.Equal([1, subItem, 3], array);
    }

    [Fact]
    public void ListViewSubItemCollection_CopyTo_Empty_Nop()
    {
        ListViewItem item = new();
        IList collection = new ListViewItem.ListViewSubItemCollection(item);
        object[] array = [1, 2, 3];
        collection.CopyTo(array, 0);
        Assert.Equal([1, 2, 3], array);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_Add_SetOwner()
    {
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem subItem = new();

        Assert.Null(subItem._owner);

        listViewItem.SubItems.Add(subItem);

        Assert.Same(listViewItem, subItem._owner);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_Add_String_SetOwner()
    {
        ListViewItem listViewItem = new();

        listViewItem.SubItems.Add("Test");

        Assert.Same(listViewItem, listViewItem.SubItems[1]._owner);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_Add_String_Color_SetOwner()
    {
        ListViewItem listViewItem = new();

        listViewItem.SubItems.Add("Test", Color.White, Color.Black, SystemFonts.MenuFont);

        Assert.Same(listViewItem, listViewItem.SubItems[1]._owner);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_AddRange_SetOwner()
    {
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem subItem1 = new();
        ListViewItem.ListViewSubItem subItem2 = new();

        Assert.Null(subItem1._owner);
        Assert.Null(subItem2._owner);

        listViewItem.SubItems.AddRange([subItem1, subItem2]);

        Assert.Same(listViewItem, subItem1._owner);
        Assert.Same(listViewItem, subItem2._owner);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_AddRange_String_SetOwner()
    {
        ListViewItem listViewItem = new();

        listViewItem.SubItems.AddRange(["Test 1", "Test 2"]);

        Assert.Same(listViewItem, listViewItem.SubItems[1]._owner);
        Assert.Same(listViewItem, listViewItem.SubItems[2]._owner);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_AddRange_String_Color_SetOwner()
    {
        ListViewItem listViewItem = new();

        listViewItem.SubItems.AddRange(["Test 1", "Test 2"], Color.White, Color.Black, SystemFonts.MenuFont);

        Assert.Same(listViewItem, listViewItem.SubItems[1]._owner);
        Assert.Same(listViewItem, listViewItem.SubItems[2]._owner);
    }

    [WinFormsFact]
    public void ListViewSubItem_Clear_RemoveOwner()
    {
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem subItem1 = new();
        ListViewItem.ListViewSubItem subItem2 = new();
        ListViewItem.ListViewSubItem oldSubItem = listViewItem.SubItems[0];

        listViewItem.SubItems.Add(subItem1);
        listViewItem.SubItems.Add(subItem2);

        Assert.Same(listViewItem, oldSubItem._owner);
        Assert.Same(listViewItem, subItem1._owner);
        Assert.Same(listViewItem, subItem1._owner);

        listViewItem.SubItems.Clear();

        Assert.Same(listViewItem, listViewItem.SubItems[0]._owner);
        Assert.Null(oldSubItem._owner);
        Assert.Null(subItem1._owner);
        Assert.Null(subItem1._owner);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_IList_Add_SetOwner()
    {
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem subItem = new();

        Assert.Null(subItem._owner);

        ((IList)listViewItem.SubItems).Add(subItem);

        Assert.Same(listViewItem, subItem._owner);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_IList_Insert_AddOwner()
    {
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem subItem1 = new();
        ListViewItem.ListViewSubItem subItem2 = new();
        ListViewItem.ListViewSubItem oldSubItem1 = listViewItem.SubItems[0];
        ListViewItem.ListViewSubItem oldSubItem2 = new();
        listViewItem.SubItems.Add(oldSubItem2);

        Assert.Null(subItem1._owner);
        Assert.Null(subItem2._owner);
        Assert.Same(listViewItem, oldSubItem1._owner);
        Assert.Same(listViewItem, oldSubItem2._owner);

        ((IList)listViewItem.SubItems).Insert(0, subItem1);
        ((IList)listViewItem.SubItems).Insert(1, subItem2);

        Assert.Equal(4, listViewItem.SubItems.Count);

        Assert.Same(subItem1, listViewItem.SubItems[0]);
        Assert.Same(subItem2, listViewItem.SubItems[1]);
        Assert.Same(oldSubItem1, listViewItem.SubItems[2]);
        Assert.Same(oldSubItem2, listViewItem.SubItems[3]);

        Assert.Same(listViewItem, oldSubItem1._owner);
        Assert.Same(listViewItem, oldSubItem2._owner);
        Assert.Same(listViewItem, subItem1._owner);
        Assert.Same(listViewItem, subItem2._owner);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_IList_Remove_RemoveOwner()
    {
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem oldSubItem = listViewItem.SubItems[0];
        ListViewItem.ListViewSubItem subItem = new();
        listViewItem.SubItems.Add(subItem);

        Assert.Same(listViewItem, subItem._owner);
        Assert.Same(listViewItem, oldSubItem._owner);

        listViewItem.SubItems.Remove(subItem);
        listViewItem.SubItems.Remove(oldSubItem);

        Assert.Same(listViewItem, listViewItem.SubItems[0]._owner);
        Assert.Null(subItem._owner);
        Assert.Null(oldSubItem._owner);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_IList_Set_UpdateOwner()
    {
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem subItem = new();
        ListViewItem.ListViewSubItem oldSubItem = listViewItem.SubItems[0];

        Assert.Same(listViewItem, oldSubItem._owner);
        Assert.Null(subItem._owner);

        ((IList)listViewItem.SubItems)[0] = subItem;

        Assert.Null(oldSubItem._owner);
        Assert.Same(listViewItem, subItem._owner);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_Insert_UpdateOwner()
    {
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem subItem1 = new();
        ListViewItem.ListViewSubItem subItem2 = new();
        ListViewItem.ListViewSubItem defaultSubItem = listViewItem.SubItems[0];

        Assert.Null(subItem1._owner);
        Assert.Null(subItem2._owner);
        Assert.Same(listViewItem, defaultSubItem._owner);

        listViewItem.SubItems.Insert(0, subItem1);
        listViewItem.SubItems.Insert(1, subItem2);

        Assert.Equal(3, listViewItem.SubItems.Count);

        Assert.Same(listViewItem, defaultSubItem._owner);
        Assert.Same(listViewItem, subItem1._owner);
        Assert.Same(listViewItem, subItem2._owner);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_Remove_RemoveOwner()
    {
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem oldSubItem = listViewItem.SubItems[0];
        ListViewItem.ListViewSubItem subItem = new();
        listViewItem.SubItems.Add(subItem);

        Assert.Same(listViewItem, subItem._owner);
        Assert.Same(listViewItem, oldSubItem._owner);

        listViewItem.SubItems.Remove(subItem);
        listViewItem.SubItems.Remove(oldSubItem);

        Assert.Null(subItem._owner);
        Assert.Null(oldSubItem._owner);
        Assert.Same(listViewItem, listViewItem.SubItems[0]._owner);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_RemoveAt_RemoveOwner()
    {
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem oldSubItem = listViewItem.SubItems[0];
        ListViewItem.ListViewSubItem subItem = new();
        listViewItem.SubItems.Add(subItem);

        Assert.Same(listViewItem, subItem._owner);
        Assert.Same(listViewItem, oldSubItem._owner);

        listViewItem.SubItems.RemoveAt(1);
        listViewItem.SubItems.RemoveAt(0);

        Assert.Null(subItem._owner);
        Assert.Null(oldSubItem._owner);
        Assert.Same(listViewItem, listViewItem.SubItems[0]._owner);
    }

    [WinFormsFact]
    public void ListViewSubItemCollection_RemoveByKey_RemoveOwner()
    {
        ListViewItem listViewItem = new("Test 1");
        ListViewItem.ListViewSubItem oldSubItem = listViewItem.SubItems[0];
        oldSubItem.Name = "Test 1";
        ListViewItem.ListViewSubItem subItem = new(listViewItem, "Test 2") { Name = "Test 2" };
        listViewItem.SubItems.Add(subItem);

        Assert.Same(listViewItem, subItem._owner);
        Assert.Same(listViewItem, oldSubItem._owner);

        listViewItem.SubItems.RemoveByKey("Test 2");
        listViewItem.SubItems.RemoveByKey("Test 1");

        Assert.Null(subItem._owner);
        Assert.Null(oldSubItem._owner);
        Assert.Same(listViewItem, listViewItem.SubItems[0]._owner);
    }
}
