// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.DotNet.RemoteExecutor;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ListViewGroupTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
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

        [WinFormsTheory]
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

        [WinFormsTheory]
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

        [WinFormsTheory]
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("te\0xt", "te\0xt")]
        public void ListViewGroup_Header_SetWithoutListViewGroup_GetReturnsExpected(string value, string expected)
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("te\0xt", "te\0xt")]
        public void ListViewGroup_Header_SetWithListViewGroup_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);

            group.Header = value;
            Assert.Equal(expected, group.Header);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            group.Header = value;
            Assert.Equal(expected, group.Header);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("header", "header")]
        [InlineData("te\0xt", "te\0xt")]
        [InlineData("ListViewGroup", "ListViewGroup")]
        public void ListViewGroup_Header_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            group.Header = value;
            Assert.Equal(expected, group.Header);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            group.Header = value;
            Assert.Equal(expected, group.Header);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Header_GetGroupInfo_TestData()
        {
            yield return new object[] { null, string.Empty };
            yield return new object[] { string.Empty, string.Empty };
            yield return new object[] { "text", "text" };
            yield return new object[] { "te\0t", "te" };
        }

        [WinFormsFact]
        public unsafe void ListViewGroup_Header_GetGroupInfo_Success()
        {
            // Run this from another thread as we call Application.EnableVisualStyles.
            RemoteExecutor.Invoke(() =>
            {
                foreach (object[] data in Header_GetGroupInfo_TestData())
                {
                    string value = (string)data[0];
                    string expected = (string)data[1];

                    Application.EnableVisualStyles();

                    using var listView = new ListView();
                    var group = new ListViewGroup();
                    listView.Groups.Add(group);

                    Assert.NotEqual(IntPtr.Zero, listView.Handle);
                    group.Header = value;

                    Assert.Equal((IntPtr)1, User32.SendMessageW(listView.Handle, (User32.WM)ComCtl32.LVM.GETGROUPCOUNT, IntPtr.Zero, IntPtr.Zero));
                    char* buffer = stackalloc char[256];
                    var lvgroup = new ComCtl32.LVGROUPW
                    {
                        cbSize = (uint)sizeof(ComCtl32.LVGROUPW),
                        mask = ComCtl32.LVGF.HEADER | ComCtl32.LVGF.GROUPID | ComCtl32.LVGF.ALIGN,
                        pszHeader = buffer,
                        cchHeader = 256
                    };
                    Assert.Equal((IntPtr)1, User32.SendMessageW(listView.Handle, (User32.WM)ComCtl32.LVM.GETGROUPINFOBYINDEX, (IntPtr)0, ref lvgroup));
                    Assert.Equal(expected, new string(lvgroup.pszHeader));
                    Assert.True(lvgroup.iGroupId >= 0);
                    Assert.Equal(ComCtl32.LVGA.HEADER_LEFT, lvgroup.uAlign);
                }
            }).Dispose();
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HorizontalAlignment))]
        public void ListViewGroup_HeaderAlignment_SetWithoutListViewGroup_GetReturnsExpected(HorizontalAlignment value)
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HorizontalAlignment))]
        public void ListViewGroup_HeaderAlignment_SetWithListViewGroup_GetReturnsExpected(HorizontalAlignment value)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);

            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HorizontalAlignment))]
        public void ListViewGroup_HeaderAlignment_SetWithListViewWithHandle_GetReturnsExpected(HorizontalAlignment value)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(HorizontalAlignment.Left, 0x00000001)]
        [InlineData(HorizontalAlignment.Center, 0x00000002)]
        [InlineData(HorizontalAlignment.Right, 0x00000004)]
        public unsafe void ListView_HeaderAlignment_GetGroupInfo_Success(HorizontalAlignment valueParam, int expectedAlignParam)
        {
            // Run this from another thread as we call Application.EnableVisualStyles.
            RemoteExecutor.Invoke((valueString, expectedAlignString) =>
            {
                HorizontalAlignment value = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), valueString);
                int expectedAlign = int.Parse(expectedAlignString);

                Application.EnableVisualStyles();
                using var listView = new ListView();
                var group1 = new ListViewGroup();
                listView.Groups.Add(group1);

                Assert.NotEqual(IntPtr.Zero, listView.Handle);
                group1.HeaderAlignment = value;

                Assert.Equal((IntPtr)1, User32.SendMessageW(listView.Handle, (User32.WM)ComCtl32.LVM.GETGROUPCOUNT, IntPtr.Zero, IntPtr.Zero));
                char* buffer = stackalloc char[256];
                var lvgroup = new ComCtl32.LVGROUPW
                {
                    cbSize = (uint)sizeof(ComCtl32.LVGROUPW),
                    mask = ComCtl32.LVGF.HEADER | ComCtl32.LVGF.GROUPID | ComCtl32.LVGF.ALIGN,
                    pszHeader = buffer,
                    cchHeader = 256
                };
                Assert.Equal((IntPtr)1, User32.SendMessageW(listView.Handle, (User32.WM)ComCtl32.LVM.GETGROUPINFOBYINDEX, (IntPtr)0, ref lvgroup));
                Assert.Equal("ListViewGroup", new string(lvgroup.pszHeader));
                Assert.True(lvgroup.iGroupId >= 0);
                Assert.Equal(expectedAlign, (int)lvgroup.uAlign);
            }, valueParam.ToString(), expectedAlignParam.ToString()).Dispose();
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(HorizontalAlignment))]
        public void ListViewGroup_HeaderAlignment_SetInvalid_ThrowsInvalidEnumArgumentException(HorizontalAlignment value)
        {
            var group = new ListViewGroup();
            Assert.Throws<InvalidEnumArgumentException>("value", () => group.HeaderAlignment = value);
        }

        [WinFormsTheory]
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

        [WinFormsTheory]
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

            var groupWithEmptyItems = new ListViewGroup();
            Assert.Empty(groupWithEmptyItems.Items);
            yield return new object[] { groupWithEmptyItems };

            var groupWithItems = new ListViewGroup();
            groupWithItems.Items.Add(new ListViewItem("text"));
            yield return new object[] { groupWithItems };
        }

        [WinFormsTheory]
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListViewGroup_ToString_Invoke_ReturnsExpected(string header, string expected)
        {
            var group = new ListViewGroup(header);
            Assert.Equal(expected, group.ToString());
        }

        [WinFormsFact]
        public void ListViewGroup_ISerializableGetObjectData_InvokeSimple_Success()
        {
            var group = new ListViewGroup();
            ISerializable iSerializable = group;
            var info = new SerializationInfo(typeof(ListViewGroup), new FormatterConverter());
            var context = new StreamingContext();

            iSerializable.GetObjectData(info, context);
            Assert.Equal("ListViewGroup", info.GetString("Header"));
            Assert.Equal(HorizontalAlignment.Left, info.GetValue("HeaderAlignment", typeof(HorizontalAlignment)));
            Assert.Throws<SerializationException>(() => info.GetString("Name"));
            Assert.Null(info.GetValue("Tag", typeof(object)));
            Assert.Throws<SerializationException>(() => info.GetInt32("ItemsCount"));
        }

        [WinFormsFact]
        public void ListViewGroup_ISerializableGetObjectData_InvokeWithEmptyItems_Success()
        {
            var group = new ListViewGroup();
            Assert.Empty(group.Items);

            ISerializable iSerializable = group;
            var info = new SerializationInfo(typeof(ListViewGroup), new FormatterConverter());
            var context = new StreamingContext();

            iSerializable.GetObjectData(info, context);
            Assert.Equal("ListViewGroup", info.GetString("Header"));
            Assert.Equal(HorizontalAlignment.Left, info.GetValue("HeaderAlignment", typeof(HorizontalAlignment)));
            Assert.Throws<SerializationException>(() => info.GetString("Name"));
            Assert.Null(info.GetValue("Tag", typeof(object)));
            Assert.Throws<SerializationException>(() => info.GetInt32("ItemsCount"));
        }

        [WinFormsFact]
        public void ListViewGroup_ISerializableGetObjectData_InvokeWithItems_Success()
        {
            var group = new ListViewGroup();
            group.Items.Add(new ListViewItem("text"));

            ISerializable iSerializable = group;
            var info = new SerializationInfo(typeof(ListViewGroup), new FormatterConverter());
            var context = new StreamingContext();

            iSerializable.GetObjectData(info, context);
            Assert.Equal("ListViewGroup", info.GetString("Header"));
            Assert.Equal(HorizontalAlignment.Left, info.GetValue("HeaderAlignment", typeof(HorizontalAlignment)));
            Assert.Throws<SerializationException>(() => info.GetString("Name"));
            Assert.Null(info.GetValue("Tag", typeof(object)));
            Assert.Equal(1, info.GetInt32("ItemsCount"));
            Assert.Equal("text", ((ListViewItem)info.GetValue("Item0", typeof(ListViewItem))).Text);
        }

        [WinFormsFact]
        public void ListViewGroup_ISerializableGetObjectData_NullInfo_ThrowsNullReferenceException()
        {
            var group = new ListViewGroup();
            ISerializable iSerializable = group;
            var context = new StreamingContext();
            Assert.Throws<NullReferenceException>(() => iSerializable.GetObjectData(null, context));
        }
    }
}
