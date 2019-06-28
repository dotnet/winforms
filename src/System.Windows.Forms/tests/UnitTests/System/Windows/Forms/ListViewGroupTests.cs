// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListViewGroupTests
    {
        [Fact]
        public void ListViewGroup_Ctor_Default()
        {
            var group = new ListViewGroup();
            Assert.Equal("ListViewGroup", group.Header);
            Assert.Equal(HorizontalAlignment.Left, group.HeaderAlignment);
            Assert.Empty(group.Items);
            Assert.Same(group.Items, group.Items);
            Assert.Null(group.ListView);
            Assert.Null(group.Name);
            Assert.Null(group.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListViewGroup_Ctor_String(string header, string expectedHeader)
        {
            var group = new ListViewGroup(header);
            Assert.Equal(expectedHeader, group.Header);
            Assert.Equal(HorizontalAlignment.Left, group.HeaderAlignment);
            Assert.Empty(group.Items);
            Assert.Same(group.Items, group.Items);
            Assert.Null(group.ListView);
            Assert.Null(group.Name);
            Assert.Null(group.Tag);
        }

        public static IEnumerable<object[]> Ctor_String_HorizontalAlignment_TestData()
        {
            yield return new object[] { null, HorizontalAlignment.Left, string.Empty };
            yield return new object[] { string.Empty, HorizontalAlignment.Right, string.Empty };
            yield return new object[] { "reasonable", HorizontalAlignment.Center, "reasonable" };
            yield return new object[] { "reasonable", (HorizontalAlignment)(HorizontalAlignment.Left - 1), "reasonable" };
            yield return new object[] { "reasonable", (HorizontalAlignment)(HorizontalAlignment.Center + 1), "reasonable" };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_HorizontalAlignment_TestData))]
        public void ListViewGroup_Ctor_String_HorizontalAlignment(string header, HorizontalAlignment headerAlignment, string expectedHeader)
        {
            var group = new ListViewGroup(header, headerAlignment);
            Assert.Equal(expectedHeader, group.Header);
            Assert.Equal(headerAlignment, group.HeaderAlignment);
            Assert.Empty(group.Items);
            Assert.Equal(group.Items, group.Items);
            Assert.Null(group.ListView);
            Assert.Null(group.Name);
            Assert.Null(group.Tag);
        }

        public static IEnumerable<object[]> Ctor_String_String_TestData()
        {
            yield return new object[] { null, null, string.Empty };
            yield return new object[] { string.Empty, string.Empty, string.Empty };
            yield return new object[] { "key", "header", "header" };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_String_TestData))]
        public void ListViewGroup_Ctor_String_String(string key, string header, string expectedHeader)
        {
            var group = new ListViewGroup(key, header);
            Assert.Equal(expectedHeader, group.Header);
            Assert.Equal(HorizontalAlignment.Left, group.HeaderAlignment);
            Assert.Empty(group.Items);
            Assert.Same(group.Items, group.Items);
            Assert.Null(group.ListView);
            Assert.Equal(key, group.Name);
            Assert.Null(group.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListViewGroup_Header_SetWithListView_GetReturnsExpected(string value, string expected)
        {
            var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);

            group.Header = value;
            Assert.Equal(expected, group.Header);

            // Set same.
            group.Header = value;
            Assert.Equal(expected, group.Header);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListViewGroup_Header_SetWithoutListView_GetReturnsExpected(string value, string expected)
        {
            var group = new ListViewGroup
            {
                Header = value
            };
            Assert.Equal(expected, group.Header);

            // Set same.
            group.Header = value;
            Assert.Equal(expected, group.Header);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HorizontalAlignment))]
        public void ListViewGroup_HeaderAlignment_SetWithListView_GetReturnsExpected(HorizontalAlignment value)
        {
            var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);

            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);

            // Set same.
            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HorizontalAlignment))]
        public void ListViewGroup_HeaderAlignment_SetWithListViewWithHandle_GetReturnsExpected(HorizontalAlignment value)
        {
            var listView = new ListView();
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            var group = new ListViewGroup();
            listView.Groups.Add(group);

            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);

            // Set same.
            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HorizontalAlignment))]
        public void ListViewGroup_HeaderAlignment_SetWithoutListView_GetReturnsExpected(HorizontalAlignment value)
        {
            var group = new ListViewGroup
            {
                HeaderAlignment = value
            };
            Assert.Equal(value, group.HeaderAlignment);

            // Set same.
            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(HorizontalAlignment))]
        public void ListViewGroup_HeaderAlignment_SetInvalid_ThrowsInvalidEnumArgumentException(HorizontalAlignment value)
        {
            var group = new ListViewGroup();
            Assert.Throws<InvalidEnumArgumentException>("value", () => group.HeaderAlignment = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ListViewGroup_Name_Set_GetReturnsExpected(string value)
        {
            var group = new ListViewGroup
            {
                Name = value
            };
            Assert.Same(value, group.Name);

            // Set same.
            group.Name = value;
            Assert.Same(value, group.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ListViewGroup_Tag_Set_GetReturnsExpected(string value)
        {
            var group = new ListViewGroup
            {
                Tag = value
            };
            Assert.Same(value, group.Tag);

            // Set same.
            group.Tag = value;
            Assert.Same(value, group.Tag);
        }

        public static IEnumerable<object[]> Serialize_Deserialize_TestData()
        {
            yield return new object[] { new ListViewGroup() };
            yield return new object[] { new ListViewGroup("header", HorizontalAlignment.Center) { Name = "name", Tag = "tag" } };

            var groupWithItems = new ListViewGroup();
            groupWithItems.Items.Add(new ListViewItem("text"));
            yield return new object[] { groupWithItems };
        }

        [Theory]
        [MemberData(nameof(Serialize_Deserialize_TestData))]
        public void ListViewGroup_Serialize_Deserialize_Success(ListViewGroup group)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, group);
                stream.Seek(0, SeekOrigin.Begin);

                ListViewGroup result = Assert.IsType<ListViewGroup>(formatter.Deserialize(stream));
                Assert.Equal(group.Header, result.Header);
                Assert.Equal(group.HeaderAlignment, result.HeaderAlignment);
                Assert.Equal(group.Items.Cast<ListViewItem>().Select(i => i.Text), result.Items.Cast<ListViewItem>().Select(i => i.Text));
                Assert.Equal(group.Name, result.Name);
                Assert.Equal(group.Tag, result.Tag);
            }
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListViewGroup_ToString_Invoke_ReturnsExpected(string header, string expected)
        {
            var group = new ListViewGroup(header);
            Assert.Equal(expected, group.ToString());
        }
    }
}
