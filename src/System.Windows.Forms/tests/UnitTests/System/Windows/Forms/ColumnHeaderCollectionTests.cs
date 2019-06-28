// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ColumnHeaderCollectionTests
    {
        [Fact]
        public void ColumnHeaderCollection_Ctor_ListView()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            Assert.Empty(collection);
        }

        [Fact]
        public void ColumnHeaderCollection_Ctor_NullOwner_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("owner", () => new ListView.ColumnHeaderCollection(null));
        }

        [Fact]
        public void ColumnHeaderCollection_IList_GetProperties_ReturnsExpected()
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);
            Assert.Equal(0, collection.Count);
            Assert.False(collection.IsFixedSize);
            Assert.False(collection.IsReadOnly);
            Assert.True(collection.IsSynchronized);
            Assert.Same(collection, collection.SyncRoot);
        }

        [Fact]
        public void ColumnHeaderCollection_Item_GetValidIndex_ReturnsExpected()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);

            var header = new ColumnHeader();
            collection.Add(header);
            Assert.Same(header, collection[0]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ColumnHeaderCollection_Item_GetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView)
            {
                new ColumnHeader()
            };
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [Fact]
        public void ColumnHeaderCollection_IListItem_GetValidIndex_ReturnsExpected()
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);

            var header = new ColumnHeader();
            collection.Add(header);
            Assert.Same(header, collection[0]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ColumnHeaderCollection_IListItem_GetInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView)
            {
                new ColumnHeader()
            };
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ColumnHeaderCollection_IListItem_Set_ThrowsNotSupportedException(int index)
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView)
            {
                new ColumnHeader()
            };
            Assert.Throws<NotSupportedException>(() => collection[index] = new ColumnHeader());
        }

        [Theory]
        [InlineData(null, -1)]
        [InlineData("", -1)]
        [InlineData("longer", -1)]
        [InlineData("sm", -1)]
        [InlineData("text", 1)]
        [InlineData("tsxt", -1)]
        [InlineData("TEXT", 1)]
        public void ColumnHeaderCollection_Item_GetString_ReturnsExpected(string key, int expectedIndex)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header1 = new ColumnHeader();
            var header2 = new ColumnHeader
            {
                Name = "text"
            };
            collection.Add(header1);
            collection.Add(header2);

            Assert.Equal(expectedIndex != -1 ? collection[expectedIndex] : null, collection[key]);
        }

        [Fact]
        public void ColumnHeaderCollection_Add_ColumnHeader_Success()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
            collection.Add(header);
            Assert.Same(header, Assert.Single(collection));
            Assert.Equal(listView, header.ListView);
            Assert.Equal(0, header.Index);
            Assert.Equal(0, header.DisplayIndex);
        }

        [Fact]
        public void ColumnHeaderCollection_Add_ExistsInSameCollection_ThrowsArgumentException()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
            collection.Add(header);
            Assert.Throws<ArgumentException>("ch", () => collection.Add(header));
        }

        [Fact]
        public void ColumnHeaderCollection_Add_ExistsInOtherCollection_ThrowsArgumentException()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var otherListView = new ListView();
            var otherCollection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
            otherCollection.Add(header);
            Assert.Throws<ArgumentException>("ch", () => collection.Add(header));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeaderCollection_Add_String_Success(string text, string expectedText)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView)
            {
                text
            };
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
            Assert.Equal(expectedText, header.Text);
            Assert.Equal(listView, header.ListView);
        }

        public static IEnumerable<object[]> Add_String_Int_TestData()
        {
            yield return new object[] { null, -1, string.Empty };
            yield return new object[] { string.Empty, 0, string.Empty };
            yield return new object[] { "text", 1, "text" };
        }

        [Theory]
        [MemberData(nameof(Add_String_Int_TestData))]
        public void ColumnHeaderCollection_Add_String_Int_Success(string text, int width, string expectedText)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView)
            {
                { text, width }
            };
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
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

        [Theory]
        [MemberData(nameof(Add_String_Int_HorizontalAlignment_TestData))]
        public void ColumnHeaderCollection_Add_String_Int_HorizontalAlignment_Success(string text, int width, HorizontalAlignment textAlign, string expectedText)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView)
            {
                { text, width, textAlign }
            };
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
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

        [Theory]
        [MemberData(nameof(Add_String_String_TestData))]
        public void ColumnHeaderCollection_Add_String_String_Success(string name, string text, string expectedName, string expectedText)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView)
            {
                { name, text }
            };
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
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

        [Theory]
        [MemberData(nameof(Add_String_String_Int_TestData))]
        public void ColumnHeaderCollection_Add_String_String_Int_Success(string name, string text, int width, string expectedName, string expectedText)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView)
            {
                { name, text, width }
            };
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
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

        [Theory]
        [MemberData(nameof(Add_String_String_Int_HorizontalAlignment_Int_TestData))]
        public void ColumnHeaderCollection_Add_String_String_Int_HorizontalAlignment_Int_Success(string name, string text, int width, HorizontalAlignment textAlign, int imageIndex, string expectedName, string expectedText)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView)
            {
                { name, text, width, textAlign, imageIndex }
            };
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
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

        [Theory]
        [MemberData(nameof(Add_String_String_Int_HorizontalAlignment_String_TestData))]
        public void ColumnHeaderCollection_Add_String_String_Int_HorizontalAlignment_String_Success(string name, string text, int width, HorizontalAlignment textAlign, string imageKey, string expectedName, string expectedText, string expectedImageKey)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView)
            {
                { name, text, width, textAlign, imageKey }
            };
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
            Assert.Equal(expectedName, header.Name);
            Assert.Equal(expectedText, header.Text);
            Assert.Equal(width, header.Width);
            Assert.Equal(textAlign, header.TextAlign);
            Assert.Equal(expectedImageKey, header.ImageKey);
            Assert.Equal(listView, header.ListView);
        }

        [Fact]
        public void ColumnHeaderCollection_Add_NullItem_ThrowsArgumentNullException()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            Assert.Throws<ArgumentNullException>("ch", () => collection.Add((ColumnHeader)null));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(HorizontalAlignment))]
        public void ColumnHeaderCollection_Add_InvalidTextAlign_ThrowsInvalidEnumArgumentException(HorizontalAlignment textAlign)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            Assert.Throws<InvalidEnumArgumentException>("value", () => collection.Add("text", 1, textAlign));
            Assert.Throws<InvalidEnumArgumentException>("value", () => collection.Add("name", "text", 1, textAlign, "imageKey"));
            Assert.Throws<InvalidEnumArgumentException>("value", () => collection.Add("name", "text", 1, textAlign, 1));
        }

        [Fact]
        public void ColumnHeaderCollection_IList_Add_Success()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
            collection.Add(header);
            Assert.Same(header, Assert.Single(collection));
            Assert.Equal(listView, header.ListView);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("text")]
        public void ColumnHeaderCollection_IListAdd_InvalidValue_ThrowsArgumentException(object value)
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);
            Assert.Throws<ArgumentException>("value", () => collection.Add(value));
        }

        [Fact]
        public void ColumnHeaderCollection_AddRange_ColumnHeaderArray_Success()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header1 = new ColumnHeader("text1");
            var header2 = new ColumnHeader("text2");
            var items = new ColumnHeader[] { header1, header2 };
            collection.AddRange(items);

            Assert.Equal(2, collection.Count);
            Assert.Same(header1, collection[0]);
            Assert.Same(header2, collection[1]);

            Assert.Equal(0, header1.DisplayIndex);
            Assert.Equal(1, header2.DisplayIndex);
        }

        [Fact]
        public void ColumnHeaderCollection_AddRange_ColumnHeaderArrayWithDisplayIndex_Success()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header1 = new ColumnHeader("text1")
            {
                DisplayIndex = 1
            };
            var header2 = new ColumnHeader("text2")
            {
                DisplayIndex = 0
            };
            var header3 = new ColumnHeader("text3")
            {
                DisplayIndex = 2
            };
            var header4 = new ColumnHeader("text4")
            {
                DisplayIndex = 2
            };
            var header5 = new ColumnHeader("text5")
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

        [Fact]
        public void ColumnHeaderCollection_AddRange_NullValues_ThrowsArgumentNullException()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            Assert.Throws<ArgumentNullException>("values", () => collection.AddRange(null));
        }

        [Fact]
        public void ColumnHeaderCollection_AddRange_NullValueInValues_ThrowsArgumentNullException()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            Assert.Throws<ArgumentNullException>("values", () => collection.AddRange(new ColumnHeader[] { null }));
        }

        [Fact]
        public void ColumnHeaderCollection_Clear_Invoke_Success()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
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

        [Fact]
        public void ColumnHeaderCollection_Clear_InvokeWithHandle_Success()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
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

        [Fact]
        public void ColumnHeaderCollection_Clear_InvokeTile_Success()
        {
            var listView = new ListView
            {
                View = View.Tile
            };
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
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

        [Fact]
        public void ColumnHeaderCollection_Clear_InvokeTileWithHandle_Success()
        {
            var listView = new ListView
            {
                View = View.Tile
            };
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
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

        [Fact]
        public void ColumnHeaderCollection_Clear_Empty_Success()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);

            collection.Clear();
            Assert.Empty(collection);
        }

        [Fact]
        public void ColumnHeaderCollection_Contains_Invoke_ReturnsExpected()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
            collection.Add(header);

            Assert.True(collection.Contains(header));
            Assert.False(collection.Contains(new ColumnHeader()));
            Assert.False(collection.Contains(null));
        }

        [Fact]
        public void ColumnHeaderCollection_Contains_Empty_ReturnsFalse()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);

            Assert.False(collection.Contains(new ColumnHeader()));
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
        public void ColumnHeaderCollection_ContainsKey_Invoke_ReturnsExpected(string key, bool expected)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header1 = new ColumnHeader();
            var header2 = new ColumnHeader
            {
                Name = "text"
            };
            collection.Add(header1);
            collection.Add(header2);

            Assert.Equal(expected, collection.ContainsKey(key));
        }

        [Fact]
        public void ColumnHeaderCollection_ContainsKey_Empty_ReturnsFalse()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);

            Assert.False(collection.ContainsKey("text"));
            Assert.False(collection.ContainsKey(null));
        }

        [Fact]
        public void ColumnHeaderCollection_IListContains_Invoke_ReturnsExpected()
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
            collection.Add(header);

            Assert.True(collection.Contains(header));
            Assert.False(collection.Contains(new ColumnHeader()));
            Assert.False(collection.Contains(new object()));
            Assert.False(collection.Contains(null));
        }

        [Fact]
        public void ColumnHeaderCollection_IListContains_Empty_ReturnsFalse()
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);

            Assert.False(collection.Contains(new ColumnHeader()));
            Assert.False(collection.Contains(new object()));
            Assert.False(collection.Contains(null));
        }

        [Fact]
        public void ColumnHeaderCollection_IndexOf_Invoke_ReturnsExpected()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
            collection.Add(header);

            Assert.Equal(0, collection.IndexOf(header));
            Assert.Equal(-1, collection.IndexOf(new ColumnHeader()));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [Fact]
        public void ColumnHeaderCollection_IndexOf_Empty_ReturnsFalse()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);

            Assert.Equal(-1, collection.IndexOf(new ColumnHeader()));
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
        public void ColumnHeaderCollection_IndexOfKey_Invoke_ReturnsExpected(string key, int expected)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header1 = new ColumnHeader();
            var header2 = new ColumnHeader
            {
                Name = "text"
            };
            collection.Add(header1);
            collection.Add(header2);

            Assert.Equal(expected, collection.IndexOfKey(key));

            // Call again to validate caching behaviour.
            Assert.Equal(expected, collection.IndexOfKey(key));
            Assert.Equal(-1, collection.IndexOfKey("noSuchKey"));
        }

        [Fact]
        public void ColumnHeaderCollection_IndexOfKey_Empty_ReturnsFalse()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);

            Assert.Equal(-1, collection.IndexOfKey("text"));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [Fact]
        public void ColumnHeaderCollection_IListIndexOf_Invoke_ReturnsExpected()
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
            collection.Add(header);

            Assert.Equal(0, collection.IndexOf(header));
            Assert.Equal(-1, collection.IndexOf(new ColumnHeader()));
            Assert.Equal(-1, collection.IndexOf(new object()));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [Fact]
        public void ColumnHeaderCollection_IListIndexOf_Empty_ReturnsMinusOne()
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);

            Assert.Equal(-1, collection.IndexOf(new ColumnHeader()));
            Assert.Equal(-1, collection.IndexOf(new object()));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [Fact]
        public void ColumnHeaderCollection_Insert_ColumnHeader_Success()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
            collection.Add(new ColumnHeader());
            collection.Insert(1, header);
            Assert.Equal(2, collection.Count);
            Assert.Same(header, collection[1]);
            Assert.Same(listView, header.ListView);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeaderCollection_Insert_String_Success(string text, string expectedText)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            collection.Insert(0, text);
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
            Assert.Equal(expectedText, header.Text);
            Assert.Equal(listView, header.ListView);
        }

        [Theory]
        [MemberData(nameof(Add_String_Int_TestData))]
        public void ColumnHeaderCollection_Insert_String_Int_Success(string text, int width, string expectedText)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            collection.Insert(0, text, width);
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
            Assert.Equal(expectedText, header.Text);
            Assert.Equal(width, header.Width);
            Assert.Equal(listView, header.ListView);
        }

        [Theory]
        [MemberData(nameof(Add_String_Int_HorizontalAlignment_TestData))]
        public void ColumnHeaderCollection_Insert_String_Int_HorizontalAlignment_Success(string text, int width, HorizontalAlignment textAlign, string expectedText)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            collection.Insert(0, text, width, textAlign);
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
            Assert.Equal(expectedText, header.Text);
            Assert.Equal(width, header.Width);
            Assert.Equal(textAlign, header.TextAlign);
            Assert.Equal(listView, header.ListView);
        }

        [Theory]
        [MemberData(nameof(Add_String_String_TestData))]
        public void ColumnHeaderCollection_Insert_String_String_Success(string name, string text, string expectedName, string expectedText)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            collection.Insert(0, name, text);
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
            Assert.Equal(expectedName, header.Name);
            Assert.Equal(expectedText, header.Text);
            Assert.Equal(listView, header.ListView);
        }

        [Theory]
        [MemberData(nameof(Add_String_String_Int_TestData))]
        public void ColumnHeaderCollection_Insert_String_String_Int_Success(string name, string text, int width, string expectedName, string expectedText)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            collection.Insert(0, name, text, width);
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
            Assert.Equal(expectedName, header.Name);
            Assert.Equal(expectedText, header.Text);
            Assert.Equal(width, header.Width);
            Assert.Equal(listView, header.ListView);
        }

        [Theory]
        [MemberData(nameof(Add_String_String_Int_HorizontalAlignment_Int_TestData))]
        public void ColumnHeaderCollection_Insert_String_String_Int_HorizontalAlignment_Int_Success(string name, string text, int width, HorizontalAlignment textAlign, int imageIndex, string expectedName, string expectedText)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            collection.Insert(0, name, text, width, textAlign, imageIndex);
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
            Assert.Equal(expectedName, header.Name);
            Assert.Equal(expectedText, header.Text);
            Assert.Equal(width, header.Width);
            Assert.Equal(textAlign, header.TextAlign);
            Assert.Equal(imageIndex, header.ImageIndex);
            Assert.Equal(listView, header.ListView);
        }

        [Theory]
        [MemberData(nameof(Add_String_String_Int_HorizontalAlignment_String_TestData))]
        public void ColumnHeaderCollection_Insert_String_String_Int_HorizontalAlignment_String_Success(string name, string text, int width, HorizontalAlignment textAlign, string imageKey, string expectedName, string expectedText, string expectedImageKey)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            collection.Insert(0, name, text, width, textAlign, imageKey);
            ColumnHeader header = Assert.Single(collection.Cast<ColumnHeader>());
            Assert.Equal(expectedName, header.Name);
            Assert.Equal(expectedText, header.Text);
            Assert.Equal(width, header.Width);
            Assert.Equal(textAlign, header.TextAlign);
            Assert.Equal(expectedImageKey, header.ImageKey);
            Assert.Equal(listView, header.ListView);
        }

        [Fact]
        public void ColumnHeaderCollection_Insert_NullItem_ThrowsArgumentNullException()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView)
            {
                new ColumnHeader()
            };
            Assert.Throws<ArgumentNullException>("ch", () => collection.Insert(1, (ColumnHeader)null));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ColumnHeaderCollection_Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, (ColumnHeader)null));
        }

        [Fact]
        public void ColumnHeaderCollection_IListInsert_ColumnHeader_Success()
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
            collection.Add(new ColumnHeader());
            collection.Insert(1, header);
            Assert.Equal(2, collection.Count);
            Assert.Same(header, collection[1]);
            Assert.Same(listView, header.ListView);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("text")]
        public void ColumnHeaderCollection_IListInsert_InvalidItem_Nop(object value)
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);
            collection.Insert(0, value);
            Assert.Empty(collection);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ColumnHeaderCollection_IListInsert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, new ColumnHeader()));
        }

        [Fact]
        public void ColumnHeaderCollection_Remove_ColumnHeader_Success()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
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

        [Fact]
        public void ColumnHeaderCollection_IListRemove_ColumnHeader_Success()
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
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

        [Theory]
        [InlineData(null)]
        [InlineData("text")]
        public void ColumnHeaderCollection_IListRemove_InvalidItem_Nop(object value)
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
            collection.Add(header);

            collection.Remove(value);
            Assert.Same(header, Assert.Single(collection));
        }

        [Fact]
        public void ColumnHeaderCollection_RemoveAt_ValidIndex_Success()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
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

        [Fact]
        public void ColumnHeaderCollection_RemoveAt_HasHandle_Success()
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
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

        [Fact]
        public void ColumnHeaderCollection_RemoveAt_HasHandleWithTile_Success()
        {
            var listView = new ListView
            {
                View = View.Tile
            };
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
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

        [Theory]
        [InlineData(-1)]
        [InlineData(2)]
        public void ColumnHeaderCollection_RemoveAt_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView)
            {
                new ColumnHeader()
            };
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ColumnHeaderCollection_RemoveAt_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
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
        public void ColumnHeaderCollection_RemoveByKey_Invoke_Success(string key, int expectedCount)
        {
            var listView = new ListView();
            var collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader
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

        [Fact]
        public void ColumnHeaderCollection_CopyTo_NonEmpty_Success()
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);
            var header = new ColumnHeader();
            collection.Add(header);

            var array = new object[] { 1, 2, 3 };
            collection.CopyTo(array, 1);
            Assert.Equal(new object[] { 1, header, 3 }, array);
        }

        [Fact]
        public void ColumnHeaderCollection_CopyTo_Empty_Nop()
        {
            var listView = new ListView();
            IList collection = new ListView.ColumnHeaderCollection(listView);
            var array = new object[] { 1, 2, 3 };
            collection.CopyTo(array, 0);
            Assert.Equal(new object[] { 1, 2, 3 }, array);
        }
    }
}
