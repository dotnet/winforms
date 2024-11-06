// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms.Tests;

public class ColumnHeaderCollectionTests
{
    [WinFormsFact]
    public void ColumnHeaderCollection_Ctor_ListView()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        Assert.Empty(collection);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new ListView.ColumnHeaderCollection(null));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_IList_GetProperties_ReturnsExpected()
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);
        Assert.Empty(collection);
        Assert.False(collection.IsFixedSize);
        Assert.False(collection.IsReadOnly);
        Assert.True(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Item_GetValidIndex_ReturnsExpected()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);

        using ColumnHeader header = new();
        collection.Add(header);
        Assert.Same(header, collection[0]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ColumnHeaderCollection_Item_GetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        var collection = new ListView.ColumnHeaderCollection(listView)
        {
            header
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_IListItem_GetValidIndex_ReturnsExpected()
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);

        using ColumnHeader header = new();
        collection.Add(header);
        Assert.Same(header, collection[0]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ColumnHeaderCollection_IListItem_GetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        IList collection = new ListView.ColumnHeaderCollection(listView)
        {
            header
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ColumnHeaderCollection_IListItem_Set_ThrowsNotSupportedException(int index)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        IList collection = new ListView.ColumnHeaderCollection(listView)
        {
            header
        };
        Assert.Throws<NotSupportedException>(() => collection[index] = new ColumnHeader());
    }

    [WinFormsTheory]
    [InlineData(null, -1)]
    [InlineData("", -1)]
    [InlineData("longer", -1)]
    [InlineData("sm", -1)]
    [InlineData("text", 1)]
    [InlineData("tsxt", -1)]
    [InlineData("TEXT", 1)]
    public void ColumnHeaderCollection_Item_GetString_ReturnsExpected(string key, int expectedIndex)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header1 = new();
        using ColumnHeader header2 = new()
        {
            Name = "text"
        };
        collection.Add(header1);
        collection.Add(header2);

        Assert.Equal(expectedIndex != -1 ? collection[expectedIndex] : null, collection[key]);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Add_ColumnHeader_Success()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);
        Assert.Same(header, Assert.Single(collection));
        Assert.Equal(listView, header.ListView);
        Assert.Equal(0, header.Index);
        Assert.Equal(0, header.DisplayIndex);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Add_ExistsInSameCollection_ThrowsArgumentException()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);
        Assert.Throws<ArgumentException>("ch", () => collection.Add(header));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Add_ExistsInOtherCollection_ThrowsArgumentException()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ListView otherListView = new();
        var otherCollection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        otherCollection.Add(header);
        Assert.Throws<ArgumentException>("ch", () => collection.Add(header));
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ColumnHeaderCollection_Add_String_Success(string text, string expectedText)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView)
        {
            text
        };
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(listView, header.ListView);
    }

    public static IEnumerable<object[]> Add_String_Int_TestData()
    {
        yield return new object[] { null, -1, string.Empty };
        yield return new object[] { string.Empty, 0, string.Empty };
        yield return new object[] { "text", 1, "text" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_String_Int_TestData))]
    public void ColumnHeaderCollection_Add_String_Int_Success(string text, int width, string expectedText)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView)
        {
            { text, width }
        };
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(width, header.Width);
        Assert.Equal(listView, header.ListView);
    }

    public static IEnumerable<object[]> Add_String_Int_HorizontalAlignment_TestData()
    {
        yield return new object[] { null, -1, HorizontalAlignment.Left, string.Empty };
        yield return new object[] { string.Empty, 0, HorizontalAlignment.Center, string.Empty };
        yield return new object[] { "text", 1, HorizontalAlignment.Right, "text" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_String_Int_HorizontalAlignment_TestData))]
    public void ColumnHeaderCollection_Add_String_Int_HorizontalAlignment_Success(string text, int width, HorizontalAlignment textAlign, string expectedText)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView)
        {
            { text, width, textAlign }
        };
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(width, header.Width);
        Assert.Equal(textAlign, header.TextAlign);
        Assert.Equal(listView, header.ListView);
    }

    public static IEnumerable<object[]> Add_String_String_TestData()
    {
        yield return new object[] { null, null, string.Empty, string.Empty };
        yield return new object[] { string.Empty, string.Empty, string.Empty, string.Empty };
        yield return new object[] { "name", "text", "name", "text" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_String_String_TestData))]
    public void ColumnHeaderCollection_Add_String_String_Success(string name, string text, string expectedName, string expectedText)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView)
        {
            { name, text }
        };
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedName, header.Name);
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(listView, header.ListView);
    }

    public static IEnumerable<object[]> Add_String_String_Int_TestData()
    {
        yield return new object[] { null, null, -1, string.Empty, string.Empty };
        yield return new object[] { string.Empty, string.Empty, 0, string.Empty, string.Empty };
        yield return new object[] { "name", "text", 1, "name", "text" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_String_String_Int_TestData))]
    public void ColumnHeaderCollection_Add_String_String_Int_Success(string name, string text, int width, string expectedName, string expectedText)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView)
        {
            { name, text, width }
        };
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedName, header.Name);
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(width, header.Width);
        Assert.Equal(listView, header.ListView);
    }

    public static IEnumerable<object[]> Add_String_String_Int_HorizontalAlignment_Int_TestData()
    {
        yield return new object[] { null, null, -1, HorizontalAlignment.Left, -1, string.Empty, string.Empty };
        yield return new object[] { string.Empty, string.Empty, 0, HorizontalAlignment.Center, 0, string.Empty, string.Empty };
        yield return new object[] { "name", "text", 1, HorizontalAlignment.Right, 1, "name", "text" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_String_String_Int_HorizontalAlignment_Int_TestData))]
    public void ColumnHeaderCollection_Add_String_String_Int_HorizontalAlignment_Int_Success(string name, string text, int width, HorizontalAlignment textAlign, int imageIndex, string expectedName, string expectedText)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView)
        {
            { name, text, width, textAlign, imageIndex }
        };
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedName, header.Name);
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(width, header.Width);
        Assert.Equal(textAlign, header.TextAlign);
        Assert.Equal(imageIndex, header.ImageIndex);
        Assert.Equal(listView, header.ListView);
    }

    public static IEnumerable<object[]> Add_String_String_Int_HorizontalAlignment_String_TestData()
    {
        yield return new object[] { null, null, -1, HorizontalAlignment.Left, null, string.Empty, string.Empty, string.Empty };
        yield return new object[] { string.Empty, string.Empty, 0, HorizontalAlignment.Center, string.Empty, string.Empty, string.Empty, string.Empty };
        yield return new object[] { "name", "text", 1, HorizontalAlignment.Right, "imageKey", "name", "text", "imageKey" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_String_String_Int_HorizontalAlignment_String_TestData))]
    public void ColumnHeaderCollection_Add_String_String_Int_HorizontalAlignment_String_Success(string name, string text, int width, HorizontalAlignment textAlign, string imageKey, string expectedName, string expectedText, string expectedImageKey)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView)
        {
            { name, text, width, textAlign, imageKey }
        };
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedName, header.Name);
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(width, header.Width);
        Assert.Equal(textAlign, header.TextAlign);
        Assert.Equal(expectedImageKey, header.ImageKey);
        Assert.Equal(listView, header.ListView);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Add_NullItem_ThrowsArgumentNullException()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        Assert.Throws<ArgumentNullException>("ch", () => collection.Add((ColumnHeader)null));
    }

    [WinFormsTheory]
    [InvalidEnumData<HorizontalAlignment>]
    public void ColumnHeaderCollection_Add_InvalidTextAlign_ThrowsInvalidEnumArgumentException(HorizontalAlignment textAlign)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        Assert.Throws<InvalidEnumArgumentException>("value", () => collection.Add("text", 1, textAlign));
        Assert.Throws<InvalidEnumArgumentException>("value", () => collection.Add("name", "text", 1, textAlign, "imageKey"));
        Assert.Throws<InvalidEnumArgumentException>("value", () => collection.Add("name", "text", 1, textAlign, 1));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_IList_Add_Success()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);
        Assert.Same(header, Assert.Single(collection));
        Assert.Equal(listView, header.ListView);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("text")]
    public void ColumnHeaderCollection_IListAdd_InvalidValue_ThrowsArgumentException(object value)
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);
        Assert.Throws<ArgumentException>("value", () => collection.Add(value));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_AddRange_ColumnHeaderArray_Success()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header1 = new("text1");
        using ColumnHeader header2 = new("text2");
        var items = new ColumnHeader[] { header1, header2 };
        collection.AddRange(items);

        Assert.Equal(2, collection.Count);
        Assert.Same(header1, collection[0]);
        Assert.Same(header2, collection[1]);

        Assert.Equal(0, header1.DisplayIndex);
        Assert.Equal(1, header2.DisplayIndex);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_AddRange_ColumnHeaderArrayWithDisplayIndex_Success()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header1 = new("text1")
        {
            DisplayIndex = 1
        };
        using ColumnHeader header2 = new("text2")
        {
            DisplayIndex = 0
        };
        using ColumnHeader header3 = new("text3")
        {
            DisplayIndex = 2
        };
        using ColumnHeader header4 = new("text4")
        {
            DisplayIndex = 2
        };
        using ColumnHeader header5 = new("text5")
        {
            DisplayIndex = 10
        };
        var items = new ColumnHeader[] { header1, header2, header3, header4, header5 };
        collection.AddRange(items);

        Assert.Equal(5, collection.Count);
        Assert.Same(header1, collection[0]);
        Assert.Same(header2, collection[1]);
        Assert.Same(header3, collection[2]);
        Assert.Same(header4, collection[3]);
        Assert.Same(header5, collection[4]);

        Assert.Equal(0, header1.DisplayIndex);
        Assert.Equal(1, header2.DisplayIndex);
        Assert.Equal(2, header3.DisplayIndex);
        Assert.Equal(3, header4.DisplayIndex);
        Assert.Equal(4, header5.DisplayIndex);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_AddRange_NullValues_ThrowsArgumentNullException()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        Assert.Throws<ArgumentNullException>("values", () => collection.AddRange(null));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_AddRange_NullValueInValues_ThrowsArgumentNullException()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        Assert.Throws<ArgumentNullException>("values", () => collection.AddRange([null]));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Clear_Invoke_Success()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Null(header.ListView);
        Assert.Equal(-1, header.Index);
        Assert.Equal(0, header.DisplayIndex);

        // Clear again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Null(header.ListView);
        Assert.Equal(-1, header.Index);
        Assert.Equal(0, header.DisplayIndex);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Clear_InvokeWithHandle_Success()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);
        Assert.NotEqual(IntPtr.Zero, listView.Handle);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Null(header.ListView);
        Assert.Equal(-1, header.Index);
        Assert.Equal(0, header.DisplayIndex);

        // Clear again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Null(header.ListView);
        Assert.Equal(-1, header.Index);
        Assert.Equal(0, header.DisplayIndex);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Clear_InvokeTile_Success()
    {
        using ListView listView = new()
        {
            View = View.Tile
        };
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Null(header.ListView);
        Assert.Equal(-1, header.Index);
        Assert.Equal(0, header.DisplayIndex);

        // Clear again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Null(header.ListView);
        Assert.Equal(-1, header.Index);
        Assert.Equal(0, header.DisplayIndex);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Clear_InvokeTileWithHandle_Success()
    {
        using ListView listView = new()
        {
            View = View.Tile
        };
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);
        Assert.NotEqual(IntPtr.Zero, listView.Handle);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Null(header.ListView);
        Assert.Equal(-1, header.Index);
        Assert.Equal(0, header.DisplayIndex);

        // Clear again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Null(header.ListView);
        Assert.Equal(-1, header.Index);
        Assert.Equal(0, header.DisplayIndex);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Clear_Empty_Success()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);

        collection.Clear();
        Assert.Empty(collection);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Contains_Invoke_ReturnsExpected()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);

        Assert.True(collection.Contains(header));
        Assert.False(collection.Contains(new ColumnHeader()));
        Assert.False(collection.Contains(null));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Contains_Empty_ReturnsFalse()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);

        Assert.False(collection.Contains(new ColumnHeader()));
        Assert.False(collection.Contains(null));
    }

    [WinFormsTheory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("longer", false)]
    [InlineData("sm", false)]
    [InlineData("text", true)]
    [InlineData("tsxt", false)]
    [InlineData("TEXT", true)]
    public void ColumnHeaderCollection_ContainsKey_Invoke_ReturnsExpected(string key, bool expected)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header1 = new();
        using ColumnHeader header2 = new()
        {
            Name = "text"
        };
        collection.Add(header1);
        collection.Add(header2);

        Assert.Equal(expected, collection.ContainsKey(key));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_ContainsKey_Empty_ReturnsFalse()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);

        Assert.False(collection.ContainsKey("text"));
        Assert.False(collection.ContainsKey(null));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_IListContains_Invoke_ReturnsExpected()
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);

        Assert.True(collection.Contains(header));
        Assert.False(collection.Contains(new ColumnHeader()));
        Assert.False(collection.Contains(new object()));
        Assert.False(collection.Contains(null));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_IListContains_Empty_ReturnsFalse()
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);

        Assert.False(collection.Contains(new ColumnHeader()));
        Assert.False(collection.Contains(new object()));
        Assert.False(collection.Contains(null));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_IndexOf_Invoke_ReturnsExpected()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);

        Assert.Equal(0, collection.IndexOf(header));
        Assert.Equal(-1, collection.IndexOf(new ColumnHeader()));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_IndexOf_Empty_ReturnsFalse()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);

        Assert.Equal(-1, collection.IndexOf(new ColumnHeader()));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [WinFormsTheory]
    [InlineData(null, -1)]
    [InlineData("", -1)]
    [InlineData("longer", -1)]
    [InlineData("sm", -1)]
    [InlineData("text", 1)]
    [InlineData("tsxt", -1)]
    [InlineData("TEXT", 1)]
    public void ColumnHeaderCollection_IndexOfKey_Invoke_ReturnsExpected(string key, int expected)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header1 = new();
        using ColumnHeader header2 = new()
        {
            Name = "text"
        };
        collection.Add(header1);
        collection.Add(header2);

        Assert.Equal(expected, collection.IndexOfKey(key));

        // Call again to validate caching behavior.
        Assert.Equal(expected, collection.IndexOfKey(key));
        Assert.Equal(-1, collection.IndexOfKey("noSuchKey"));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_IndexOfKey_Empty_ReturnsFalse()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);

        Assert.Equal(-1, collection.IndexOfKey("text"));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_IListIndexOf_Invoke_ReturnsExpected()
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);

        Assert.Equal(0, collection.IndexOf(header));
        Assert.Equal(-1, collection.IndexOf(new ColumnHeader()));
        Assert.Equal(-1, collection.IndexOf(new object()));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_IListIndexOf_Empty_ReturnsMinusOne()
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);

        Assert.Equal(-1, collection.IndexOf(new ColumnHeader()));
        Assert.Equal(-1, collection.IndexOf(new object()));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Insert_ColumnHeader_Success()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(new ColumnHeader());
        collection.Insert(1, header);
        Assert.Equal(2, collection.Count);
        Assert.Same(header, collection[1]);
        Assert.Same(listView, header.ListView);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ColumnHeaderCollection_Insert_String_Success(string text, string expectedText)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        collection.Insert(0, text);
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(listView, header.ListView);
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_String_Int_TestData))]
    public void ColumnHeaderCollection_Insert_String_Int_Success(string text, int width, string expectedText)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        collection.Insert(0, text, width);
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(width, header.Width);
        Assert.Equal(listView, header.ListView);
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_String_Int_HorizontalAlignment_TestData))]
    public void ColumnHeaderCollection_Insert_String_Int_HorizontalAlignment_Success(string text, int width, HorizontalAlignment textAlign, string expectedText)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        collection.Insert(0, text, width, textAlign);
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(width, header.Width);
        Assert.Equal(textAlign, header.TextAlign);
        Assert.Equal(listView, header.ListView);
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_String_String_TestData))]
    public void ColumnHeaderCollection_Insert_String_String_Success(string name, string text, string expectedName, string expectedText)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        collection.Insert(0, name, text);
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedName, header.Name);
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(listView, header.ListView);
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_String_String_Int_TestData))]
    public void ColumnHeaderCollection_Insert_String_String_Int_Success(string name, string text, int width, string expectedName, string expectedText)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        collection.Insert(0, name, text, width);
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedName, header.Name);
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(width, header.Width);
        Assert.Equal(listView, header.ListView);
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_String_String_Int_HorizontalAlignment_Int_TestData))]
    public void ColumnHeaderCollection_Insert_String_String_Int_HorizontalAlignment_Int_Success(string name, string text, int width, HorizontalAlignment textAlign, int imageIndex, string expectedName, string expectedText)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        collection.Insert(0, name, text, width, textAlign, imageIndex);
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedName, header.Name);
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(width, header.Width);
        Assert.Equal(textAlign, header.TextAlign);
        Assert.Equal(imageIndex, header.ImageIndex);
        Assert.Equal(listView, header.ListView);
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_String_String_Int_HorizontalAlignment_String_TestData))]
    public void ColumnHeaderCollection_Insert_String_String_Int_HorizontalAlignment_String_Success(string name, string text, int width, HorizontalAlignment textAlign, string imageKey, string expectedName, string expectedText, string expectedImageKey)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        collection.Insert(0, name, text, width, textAlign, imageKey);
        using ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
        Assert.Equal(expectedName, header.Name);
        Assert.Equal(expectedText, header.Text);
        Assert.Equal(width, header.Width);
        Assert.Equal(textAlign, header.TextAlign);
        Assert.Equal(expectedImageKey, header.ImageKey);
        Assert.Equal(listView, header.ListView);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Insert_NullItem_ThrowsArgumentNullException()
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        var collection = new ListView.ColumnHeaderCollection(listView)
        {
            header
        };
        Assert.Throws<ArgumentNullException>("ch", () => collection.Insert(1, (ColumnHeader)null));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ColumnHeaderCollection_Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, (ColumnHeader)null));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_IListInsert_ColumnHeader_Success()
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(new ColumnHeader());
        collection.Insert(1, header);
        Assert.Equal(2, collection.Count);
        Assert.Same(header, collection[1]);
        Assert.Same(listView, header.ListView);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("text")]
    public void ColumnHeaderCollection_IListInsert_InvalidItem_Nop(object value)
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);
        collection.Insert(0, value);
        Assert.Empty(collection);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ColumnHeaderCollection_IListInsert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, new ColumnHeader()));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_Remove_ColumnHeader_Success()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);

        // Remove null.
        collection.Remove(null);
        Assert.Same(header, Assert.Single(collection));

        collection.Remove(header);
        Assert.Empty(collection);
        Assert.Null(header.ListView);
        Assert.Equal(-1, header.Index);
        Assert.Equal(-1, header.DisplayIndex);

        // Remove again.
        collection.Remove(header);
        Assert.Empty(collection);
        Assert.Null(header.ListView);
        Assert.Equal(-1, header.Index);
        Assert.Equal(-1, header.DisplayIndex);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_IListRemove_ColumnHeader_Success()
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);

        collection.Remove(header);
        Assert.Empty(collection);
        Assert.Null(header.ListView);
        Assert.Equal(-1, header.Index);
        Assert.Equal(-1, header.DisplayIndex);

        // Remove again.
        collection.Remove(header);
        Assert.Empty(collection);
        Assert.Null(header.ListView);
        Assert.Equal(-1, header.Index);
        Assert.Equal(-1, header.DisplayIndex);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("text")]
    public void ColumnHeaderCollection_IListRemove_InvalidItem_Nop(object value)
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);

        collection.Remove(value);
        Assert.Same(header, Assert.Single(collection));
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_RemoveAt_ValidIndex_Success()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);
        collection.Add(new ColumnHeader());
        collection.Add(new ColumnHeader());
        collection.Add(new ColumnHeader());

        // Remove from start.
        collection.RemoveAt(0);
        Assert.Equal(3, collection.Count);
        Assert.Equal(0, collection[0].DisplayIndex);
        Assert.Equal(1, collection[1].DisplayIndex);
        Assert.Equal(2, collection[2].DisplayIndex);

        // Remove from middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(0, collection[0].DisplayIndex);
        Assert.Equal(1, collection[1].DisplayIndex);

        // Remove from end.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(0, collection[0].DisplayIndex);

        // Remove only.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Null(header.ListView);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_RemoveAt_HasHandle_Success()
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);
        collection.Add(new ColumnHeader());
        collection.Add(new ColumnHeader());
        collection.Add(new ColumnHeader());
        Assert.NotEqual(IntPtr.Zero, listView.Handle);

        // Remove from start.
        collection.RemoveAt(0);
        Assert.Equal(3, collection.Count);
        Assert.Equal(0, collection[0].DisplayIndex);
        Assert.Equal(1, collection[1].DisplayIndex);
        Assert.Equal(2, collection[2].DisplayIndex);

        // Remove from middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(0, collection[0].DisplayIndex);
        Assert.Equal(1, collection[1].DisplayIndex);

        // Remove from end.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(0, collection[0].DisplayIndex);

        // Remove only.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Null(header.ListView);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_RemoveAt_HasHandleWithTile_Success()
    {
        using ListView listView = new()
        {
            View = View.Tile
        };
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);
        collection.Add(new ColumnHeader());
        collection.Add(new ColumnHeader());
        collection.Add(new ColumnHeader());
        Assert.NotEqual(IntPtr.Zero, listView.Handle);

        // Remove from start.
        collection.RemoveAt(0);
        Assert.Equal(3, collection.Count);
        Assert.Equal(0, collection[0].DisplayIndex);
        Assert.Equal(1, collection[1].DisplayIndex);
        Assert.Equal(2, collection[2].DisplayIndex);

        // Remove from middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(0, collection[0].DisplayIndex);
        Assert.Equal(1, collection[1].DisplayIndex);

        // Remove from end.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(0, collection[0].DisplayIndex);

        // Remove only.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Null(header.ListView);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    public void ColumnHeaderCollection_RemoveAt_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        var collection = new ListView.ColumnHeaderCollection(listView)
        {
            header
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ColumnHeaderCollection_RemoveAt_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
    }

    [WinFormsTheory]
    [InlineData(null, 1)]
    [InlineData("", 1)]
    [InlineData("longer", 1)]
    [InlineData("sm", 1)]
    [InlineData("text", 0)]
    [InlineData("tsxt", 1)]
    [InlineData("TEXT", 0)]
    public void ColumnHeaderCollection_RemoveByKey_Invoke_Success(string key, int expectedCount)
    {
        using ListView listView = new();
        var collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new()
        {
            Name = "text"
        };
        collection.Add(header);

        collection.RemoveByKey(key);
        Assert.Equal(expectedCount, collection.Count);
        if (expectedCount == 0)
        {
            Assert.Null(header.ListView);
        }
        else
        {
            Assert.Same(listView, header.ListView);
        }
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_CopyTo_NonEmpty_Success()
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);
        using ColumnHeader header = new();
        collection.Add(header);

        object[] array = [1, 2, 3];
        collection.CopyTo(array, 1);
        Assert.Equal([1, header, 3], array);
    }

    [WinFormsFact]
    public void ColumnHeaderCollection_CopyTo_Empty_Nop()
    {
        using ListView listView = new();
        IList collection = new ListView.ColumnHeaderCollection(listView);
        object[] array = [1, 2, 3];
        collection.CopyTo(array, 0);
        Assert.Equal([1, 2, 3], array);
    }
}
