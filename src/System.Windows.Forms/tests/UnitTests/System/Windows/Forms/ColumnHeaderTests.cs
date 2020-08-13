// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms.Tests
{
    public class ColumnHeaderTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ColumnHeader_Ctor_Default()
        {
            using var header = new SubColumnHeader();
            Assert.True(header.CanRaiseEvents);
            Assert.Null(header.Container);
            Assert.False(header.DesignMode);
            Assert.Equal(-1, header.DisplayIndex);
            Assert.NotNull(header.Events);
            Assert.Same(header.Events, header.Events);
            Assert.Equal(-1, header.ImageIndex);
            Assert.Equal(string.Empty, header.ImageKey);
            Assert.Null(header.ImageList);
            Assert.Equal(-1, header.Index);
            Assert.Null(header.ListView);
            Assert.Empty(header.Name);
            Assert.Null(header.Site);
            Assert.Null(header.Tag);
            Assert.Equal("ColumnHeader", header.Text);
            Assert.Equal(HorizontalAlignment.Left, header.TextAlign);
            Assert.Equal(60, header.Width);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_Ctor_Int(int imageIndex)
        {
            using var header = new SubColumnHeader(imageIndex);
            Assert.True(header.CanRaiseEvents);
            Assert.Null(header.Container);
            Assert.False(header.DesignMode);
            Assert.Equal(-1, header.DisplayIndex);
            Assert.NotNull(header.Events);
            Assert.Same(header.Events, header.Events);
            Assert.Equal(imageIndex, header.ImageIndex);
            Assert.Equal(string.Empty, header.ImageKey);
            Assert.Null(header.ImageList);
            Assert.Equal(-1, header.Index);
            Assert.Null(header.ListView);
            Assert.Empty(header.Name);
            Assert.Null(header.Site);
            Assert.Null(header.Tag);
            Assert.Equal("ColumnHeader", header.Text);
            Assert.Equal(HorizontalAlignment.Left, header.TextAlign);
            Assert.Equal(60, header.Width);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Ctor_String(string imageKey, string expectedImageKey)
        {
            using var header = new SubColumnHeader(imageKey);
            Assert.True(header.CanRaiseEvents);
            Assert.Null(header.Container);
            Assert.False(header.DesignMode);
            Assert.Equal(-1, header.DisplayIndex);
            Assert.NotNull(header.Events);
            Assert.Same(header.Events, header.Events);
            Assert.Equal(-1, header.ImageIndex);
            Assert.Equal(expectedImageKey, header.ImageKey);
            Assert.Null(header.ImageList);
            Assert.Equal(-1, header.Index);
            Assert.Null(header.ListView);
            Assert.Empty(header.Name);
            Assert.Null(header.Site);
            Assert.Null(header.Tag);
            Assert.Equal("ColumnHeader", header.Text);
            Assert.Equal(HorizontalAlignment.Left, header.TextAlign);
            Assert.Equal(60, header.Width);
        }

        [WinFormsFact]
        public void ColumnHeader_DisplayIndex_GetWithListView_ReturnsExpected()
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.Equal(0, header.DisplayIndex);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_DisplayIndex_SetWithoutListView_GetReturnsExpected(int value)
        {
            using var header = new ColumnHeader
            {
                DisplayIndex = value
            };
            Assert.Equal(value, header.DisplayIndex);

            // Set same.
            header.DisplayIndex = value;
            Assert.Equal(value, header.DisplayIndex);
        }

        public static IEnumerable<object[]> DisplayIndex_Set_TestData()
        {
            yield return new object[] { 0, 0, new int[] { 0, 1, 2 } };
            yield return new object[] { 0, 1, new int[] { 1, 0, 2 } };
            yield return new object[] { 0, 2, new int[] { 2, 0, 1 } };

            yield return new object[] { 1, 0, new int[] { 1, 0, 2 } };
            yield return new object[] { 1, 1, new int[] { 0, 1, 2 } };
            yield return new object[] { 1, 2, new int[] { 0, 2, 1 } };

            yield return new object[] { 2, 0, new int[] { 1, 2, 0 } };
            yield return new object[] { 2, 1, new int[] { 0, 2, 1 } };
            yield return new object[] { 2, 2, new int[] { 0, 1, 2 } };
        }

        [WinFormsTheory]
        [MemberData(nameof(DisplayIndex_Set_TestData))]
        public void ColumnHeader_DisplayIndex_SetWithListView_GetReturnsExpected(int columnIndex, int value, int[] expectedDisplayIndices)
        {
            using var listView = new ListView();
            using var header1 = new ColumnHeader();
            using var header2 = new ColumnHeader();
            using var header3 = new ColumnHeader();
            listView.Columns.Add(header1);
            listView.Columns.Add(header2);
            listView.Columns.Add(header3);

            listView.Columns[columnIndex].DisplayIndex = value;
            Assert.Equal(expectedDisplayIndices[0], header1.DisplayIndex);
            Assert.Equal(expectedDisplayIndices[1], header2.DisplayIndex);
            Assert.Equal(expectedDisplayIndices[2], header3.DisplayIndex);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.Columns[columnIndex].DisplayIndex = value;
            Assert.Equal(expectedDisplayIndices[0], header1.DisplayIndex);
            Assert.Equal(expectedDisplayIndices[1], header2.DisplayIndex);
            Assert.Equal(expectedDisplayIndices[2], header3.DisplayIndex);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DisplayIndex_Set_TestData))]
        public void ColumnHeader_DisplayIndex_SetWithListViewWithHandle_GetReturnsExpected(int columnIndex, int value, int[] expectedDisplayIndices)
        {
            using var listView = new ListView();
            using var header1 = new ColumnHeader();
            using var header2 = new ColumnHeader();
            using var header3 = new ColumnHeader();
            listView.Columns.Add(header1);
            listView.Columns.Add(header2);
            listView.Columns.Add(header3);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.Columns[columnIndex].DisplayIndex = value;
            Assert.Equal(expectedDisplayIndices[0], header1.DisplayIndex);
            Assert.Equal(expectedDisplayIndices[1], header2.DisplayIndex);
            Assert.Equal(expectedDisplayIndices[2], header3.DisplayIndex);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.Columns[columnIndex].DisplayIndex = value;
            Assert.Equal(expectedDisplayIndices[0], header1.DisplayIndex);
            Assert.Equal(expectedDisplayIndices[1], header2.DisplayIndex);
            Assert.Equal(expectedDisplayIndices[2], header3.DisplayIndex);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> DisplayIndex_SetGetColOrderArray_TestData()
        {
            yield return new object[] { 0, 0, new int[] { 0, 1, 2 } };
            yield return new object[] { 0, 1, new int[] { 1, 0, 2 } };
            yield return new object[] { 0, 2, new int[] { 1, 2, 0 } };

            yield return new object[] { 1, 0, new int[] { 1, 0, 2 } };
            yield return new object[] { 1, 1, new int[] { 0, 1, 2 } };
            yield return new object[] { 1, 2, new int[] { 0, 2, 1 } };

            yield return new object[] { 2, 0, new int[] { 2, 0, 1 } };
            yield return new object[] { 2, 1, new int[] { 0, 2, 1 } };
            yield return new object[] { 2, 2, new int[] { 0, 1, 2 } };
        }

        [WinFormsTheory]
        [MemberData(nameof(DisplayIndex_SetGetColOrderArray_TestData))]
        public void ColumnHeader_DisplayIndex_GetColumnOrderArray_Success(int columnIndex, int value, int[] expectedDisplayIndices)
        {
            using var listView = new ListView();
            using var header1 = new ColumnHeader();
            using var header2 = new ColumnHeader();
            using var header3 = new ColumnHeader();
            listView.Columns.Add(header1);
            listView.Columns.Add(header2);
            listView.Columns.Add(header3);

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            listView.Columns[columnIndex].DisplayIndex = value;
            var result = new int[3];
            Assert.Equal((IntPtr)1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETCOLUMNORDERARRAY, (IntPtr)3, ref result[0]));
            Assert.Equal(expectedDisplayIndices, result);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ColumnHeader_DisplayIndex_SetInvalidWithListView_ThrowsArgumentOutOfRangeException(int value)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.Throws<ArgumentOutOfRangeException>("DisplayIndex", () => header.DisplayIndex = value);
        }

        [WinFormsFact]
        public void ColumnHeader_DisplayIndex_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ColumnHeader))[nameof(ColumnHeader.DisplayIndex)];
            using var item = new ColumnHeader();
            Assert.False(property.ShouldSerializeValue(item));

            item.DisplayIndex = -1;
            Assert.Equal(-1, item.DisplayIndex);
            Assert.False(property.ShouldSerializeValue(item));

            // Set custom
            item.DisplayIndex = 0;
            Assert.Equal(0, item.DisplayIndex);
            Assert.True(property.ShouldSerializeValue(item));

            property.ResetValue(item);
            Assert.Equal(0, item.DisplayIndex);
            Assert.True(property.ShouldSerializeValue(item));
        }

        [WinFormsFact]
        public void ColumnHeader_DisplayIndex_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ColumnHeader))[nameof(ColumnHeader.DisplayIndex)];
            using var item = new ColumnHeader();
            Assert.False(property.CanResetValue(item));

            item.DisplayIndex = -1;
            Assert.Equal(-1, item.DisplayIndex);
            Assert.False(property.CanResetValue(item));

            // Set custom
            item.DisplayIndex = 0;
            Assert.Equal(0, item.DisplayIndex);
            Assert.False(property.CanResetValue(item));

            property.ResetValue(item);
            Assert.Equal(0, item.DisplayIndex);
            Assert.False(property.CanResetValue(item));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_ImageIndex_SetWithoutListView_GetReturnsExpected(int value)
        {
            using var header = new ColumnHeader
            {
                ImageIndex = value
            };
            Assert.Equal(value, header.ImageIndex);
            Assert.Empty(header.ImageKey);

            // Set same.
            header.ImageIndex = value;
            Assert.Equal(value, header.ImageIndex);
            Assert.Empty(header.ImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_ImageIndex_SetWithImageKey_GetReturnsExpected(int value)
        {
            using var header = new ColumnHeader
            {
                ImageKey = "ImageKey",
                ImageIndex = value
            };
            Assert.Equal(value, header.ImageIndex);
            Assert.Equal(ImageList.Indexer.DefaultKey, header.ImageKey);

            // Set same.
            header.ImageIndex = value;
            Assert.Equal(value, header.ImageIndex);
            Assert.Equal(ImageList.Indexer.DefaultKey, header.ImageKey);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_ImageIndex_SetWithListView_GetReturnsExpected(int value)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.ImageIndex = value;
            Assert.Equal(value, header.ImageIndex);
            Assert.Empty(header.ImageKey);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            header.ImageIndex = value;
            Assert.Equal(value, header.ImageIndex);
            Assert.Empty(header.ImageKey);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_ImageIndex_SetWithListViewWithEmptyList_GetReturnsExpected(int value)
        {
            using var imageList = new ImageList();
            using var listView = new ListView
            {
                SmallImageList = imageList
            };
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.ImageIndex = value;
            Assert.Equal(-1, header.ImageIndex);
            Assert.Empty(header.ImageKey);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            header.ImageIndex = value;
            Assert.Equal(-1, header.ImageIndex);
            Assert.Empty(header.ImageKey);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        public void ColumnHeader_ImageIndex_SetWithListViewWithNotEmptyList_GetReturnsExpected(int value, int expected)
        {
            using var image1 = new Bitmap(10, 10);
            using var image2 = new Bitmap(10, 10);
            using var imageList = new ImageList();
            imageList.Images.Add(image1);
            imageList.Images.Add(image2);
            using var listView = new ListView
            {
                SmallImageList = imageList
            };
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.ImageIndex = value;
            Assert.Equal(expected, header.ImageIndex);
            Assert.Empty(header.ImageKey);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            header.ImageIndex = value;
            Assert.Equal(expected, header.ImageIndex);
            Assert.Empty(header.ImageKey);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_ImageIndex_SetWithListViewWithHandle_GetReturnsExpected(int value)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            header.ImageIndex = value;
            Assert.Equal(value, header.ImageIndex);
            Assert.Empty(header.ImageKey);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            header.ImageIndex = value;
            Assert.Equal(value, header.ImageIndex);
            Assert.Empty(header.ImageKey);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_ImageIndex_GetColumnWithoutImageList_Success(int value)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            header.ImageIndex = value;
            var column = new ComCtl32.LVCOLUMNW
            {
                mask = ComCtl32.LVCF.IMAGE
            };
            Assert.Equal((IntPtr)1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETCOLUMNW, (IntPtr)0, ref column));
            Assert.Equal(0, column.iImage);
        }

        [WinFormsTheory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 0)]
        public void ColumnHeader_ImageIndex_GetColumnWithImageList_Success(int value, int expected)
        {
            using var image1 = new Bitmap(10, 10);
            using var image2 = new Bitmap(10, 10);
            using var imageList = new ImageList();
            imageList.Images.Add(image1);
            imageList.Images.Add(image2);
            using var listView = new ListView
            {
                SmallImageList = imageList
            };
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            header.ImageIndex = value;
            var column = new ComCtl32.LVCOLUMNW
            {
                mask = ComCtl32.LVCF.IMAGE | ComCtl32.LVCF.FMT,
                fmt = ComCtl32.LVCFMT.IMAGE
            };
            Assert.Equal((IntPtr)1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETCOLUMNW, (IntPtr)0, ref column));
            Assert.Equal(expected, column.iImage);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        public void ColumnHeader_ImageIndex_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            using var header = new ColumnHeader();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => header.ImageIndex = value);
        }

        [WinFormsFact]
        public void ColumnHeader_ImageIndex_SetInvalidSetColumn_ThrowsInvalidOperationException()
        {
            using var listView = new InvalidSetColumnListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            listView.MakeInvalid = true;
            Assert.Throws<InvalidOperationException>(() => header.ImageIndex = 0);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_ImageKey_SetWithoutListView_GetReturnsExpected(string value, string expected)
        {
            using var header = new ColumnHeader
            {
                ImageKey = value
            };
            Assert.Equal(expected, header.ImageKey);
            Assert.Equal(-1, header.ImageIndex);

            // Set same.
            header.ImageKey = value;
            Assert.Equal(expected, header.ImageKey);
            Assert.Equal(-1, header.ImageIndex);
        }

        [WinFormsTheory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("ImageKey", "ImageKey")]
        public void ColumnHeader_ImageKey_SetWithImageIndex_GetReturnsExpected(string value, string expectedImageKey)
        {
            using var header = new ColumnHeader
            {
                ImageIndex = 0,
                ImageKey = value
            };
            Assert.Equal(expectedImageKey, header.ImageKey);
            Assert.Equal(ImageList.Indexer.DefaultIndex, header.ImageIndex);

            // Set same.
            header.ImageKey = value;
            Assert.Equal(expectedImageKey, header.ImageKey);
            Assert.Equal(ImageList.Indexer.DefaultIndex, header.ImageIndex);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_ImageKey_SetWithListView_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.ImageKey = value;
            Assert.Equal(expected, header.ImageKey);
            Assert.Equal(-1, header.ImageIndex);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            header.ImageKey = value;
            Assert.Equal(expected, header.ImageKey);
            Assert.Equal(-1, header.ImageIndex);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_ImageKey_SetWithListViewWithEmptyList_GetReturnsExpected(string value, string expected)
        {
            using var imageList = new ImageList();
            using var listView = new ListView
            {
                SmallImageList = imageList
            };
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.ImageKey = value;
            Assert.Equal(expected, header.ImageKey);
            Assert.Equal(-1, header.ImageIndex);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            header.ImageKey = value;
            Assert.Equal(expected, header.ImageKey);
            Assert.Equal(-1, header.ImageIndex);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("Image1", "Image1")]
        [InlineData("image1", "image1")]
        [InlineData("Image2", "Image2")]
        [InlineData("NoSuchImage", "NoSuchImage")]
        public void ColumnHeader_ImageKey_SetWithListViewWithNotEmptyList_GetReturnsExpected(string value, string expected)
        {
            using var image1 = new Bitmap(10, 10);
            using var image2 = new Bitmap(10, 10);
            using var imageList = new ImageList();
            imageList.Images.Add("Image1", image1);
            imageList.Images.Add("Image2", image2);
            using var listView = new ListView
            {
                SmallImageList = imageList
            };
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.ImageKey = value;
            Assert.Equal(expected, header.ImageKey);
            Assert.Equal(-1, header.ImageIndex);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            header.ImageKey = value;
            Assert.Equal(expected, header.ImageKey);
            Assert.Equal(-1, header.ImageIndex);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_ImageKey_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            header.ImageKey = value;
            Assert.Equal(expected, header.ImageKey);
            Assert.Equal(-1, header.ImageIndex);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            header.ImageKey = value;
            Assert.Equal(expected, header.ImageKey);
            Assert.Equal(-1, header.ImageIndex);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Image1")]
        [InlineData("image1")]
        [InlineData("Image2")]
        [InlineData("NoSuchImage")]
        public void ColumnHeader_ImageKey_GetColumnWithoutImageList_Success(string value)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            header.ImageKey = value;
            var column = new ComCtl32.LVCOLUMNW
            {
                mask = ComCtl32.LVCF.IMAGE
            };
            Assert.Equal((IntPtr)1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETCOLUMNW, (IntPtr)0, ref column));
            Assert.Equal(0, column.iImage);
        }

        [WinFormsTheory]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        [InlineData("Image1", 0)]
        [InlineData("image1", 0)]
        [InlineData("Image2", 1)]
        [InlineData("NoSuchImage", 0)]
        public void ColumnHeader_ImageKey_GetColumnWithImageList_Success(string value, int expected)
        {
            using var image1 = new Bitmap(10, 10);
            using var image2 = new Bitmap(10, 10);
            using var imageList = new ImageList();
            imageList.Images.Add("Image1", image1);
            imageList.Images.Add("Image2", image2);
            using var listView = new ListView
            {
                SmallImageList = imageList
            };
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            header.ImageKey = value;
            var column = new ComCtl32.LVCOLUMNW
            {
                mask = ComCtl32.LVCF.IMAGE | ComCtl32.LVCF.FMT,
                fmt = ComCtl32.LVCFMT.IMAGE
            };
            Assert.Equal((IntPtr)1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETCOLUMNW, (IntPtr)0, ref column));
            Assert.Equal(expected, column.iImage);
        }

        [WinFormsFact]
        public void ColumnHeader_ImageKey_SetInvalidSetColumn_ThrowsInvalidOperationException()
        {
            using var listView = new InvalidSetColumnListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            listView.MakeInvalid = true;
            Assert.Throws<InvalidOperationException>(() => header.ImageKey = "Key");
        }

        [WinFormsFact]
        public void ColumnHeader_ImageList_GetWithListViewWithoutImageList_ReturnsExpected()
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.Null(header.ImageList);
        }

        [WinFormsFact]
        public void ColumnHeader_ImageList_GetWithListViewWithImageList_ReturnsExpected()
        {
            using var imageList = new ImageList();
            using var listView = new ListView
            {
                SmallImageList = imageList
            };
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.Same(listView.SmallImageList, header.ImageList);
        }

        [WinFormsFact]
        public void ColumnHeader_Index_GetWithListView_ReturnsExpected()
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.Equal(0, header.Index);
        }

        [WinFormsFact]
        public void ColumnHeader_ListView_GetWithListView_ReturnsExpected()
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.Same(listView, header.ListView);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Name_GetWithSite_ReturnsExpected(string name, string expected)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(x => x.Name)
                .Returns(name);
            mockSite
                .Setup(x => x.Container)
                .Returns((IContainer)null);
            using var header = new ColumnHeader
            {
                Site = mockSite.Object
            };
            Assert.Equal(expected, header.Name);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Name_SetWithoutListView_GetReturnsExpected(string value, string expected)
        {
            using var header = new ColumnHeader
            {
                Name = value
            };
            Assert.Equal(expected, header.Name);

            // Set same.
            header.Name = value;
            Assert.Equal(expected, header.Name);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Name_SetWithListView_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.Name = value;
            Assert.Equal(expected, header.Name);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            header.Name = value;
            Assert.Equal(expected, header.Name);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Name_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            header.Name = value;
            Assert.Equal(expected, header.Name);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            header.Name = value;
            Assert.Equal(expected, header.Name);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Name_SetWithSite_GetReturnsExpected(string value, string expected)
        {
            using var header = new ColumnHeader
            {
                Site = Mock.Of<ISite>(),
                Name = value
            };
            Assert.Equal(expected, header.Name);
            Assert.Equal(value, header.Site.Name);

            // Set same.
            header.Name = value;
            Assert.Equal(expected, header.Name);
            Assert.Equal(value, header.Site.Name);
        }

        [WinFormsFact]
        public void ColumnHeader_Name_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ColumnHeader))[nameof(ColumnHeader.Name)];
            using var header = new ColumnHeader();
            Assert.False(property.CanResetValue(header));

            // Set null.
            header.Name = null;
            Assert.Empty(header.Name);
            Assert.False(property.CanResetValue(header));

            // Set empty.
            header.Name = string.Empty;
            Assert.Empty(header.Name);
            Assert.False(property.CanResetValue(header));

            // Set custom
            header.Name = "name";
            Assert.Equal("name", header.Name);
            Assert.False(property.CanResetValue(header));

            property.ResetValue(header);
            Assert.Equal("name", header.Name);
            Assert.False(property.CanResetValue(header));
        }

        [WinFormsFact]
        public void ColumnHeader_Name_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ColumnHeader))[nameof(ColumnHeader.Name)];
            using var header = new ColumnHeader();
            Assert.False(property.ShouldSerializeValue(header));

            // Set null.
            header.Name = null;
            Assert.Empty(header.Name);
            Assert.False(property.ShouldSerializeValue(header));

            // Set empty.
            header.Name = string.Empty;
            Assert.Empty(header.Name);
            Assert.False(property.ShouldSerializeValue(header));

            // Set custom
            header.Name = "name";
            Assert.Equal("name", header.Name);
            Assert.True(property.ShouldSerializeValue(header));

            property.ResetValue(header);
            Assert.Equal("name", header.Name);
            Assert.True(property.ShouldSerializeValue(header));
        }

        [WinFormsTheory]
        [InlineData("name", "name", true)]
        [InlineData(null, "", false)]
        public void ColumnHeader_Name_WithSite_ShouldSerializeValue_Success(string name, string resultingName, bool result)
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ColumnHeader))[nameof(ColumnHeader.Name)];

            // Get name from the Site
            using ColumnHeader header = new ColumnHeader();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Name)
                .Returns(name);
            mockSite
                .Setup(s => s.Component)
                .Returns(header);
            // Container is accessed when disposing the ColumnHeader component.
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            header.Site = mockSite.Object;

            Assert.Equal(resultingName, header.Name);
            Assert.Equal(result, property.ShouldSerializeValue(header));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ColumnHeader_Tag_Set_GetReturnsExpected(object value)
        {
            using var header = new ColumnHeader
            {
                Tag = value
            };
            Assert.Equal(value, header.Tag);

            // Set same.
            header.Tag = value;
            Assert.Equal(value, header.Tag);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Text_SetWithoutListView_GetReturnsExpected(string value, string expected)
        {
            using var header = new ColumnHeader
            {
                Text = value
            };
            Assert.Equal(expected, header.Text);

            // Set same.
            header.Text = value;
            Assert.Equal(expected, header.Text);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Text_SetWithListView_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.Text = value;
            Assert.Equal(expected, header.Text);

            // Set same.
            header.Text = value;
            Assert.Equal(expected, header.Text);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Text_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            header.Text = value;
            Assert.Equal(expected, header.Text);

            // Set same.
            header.Text = value;
            Assert.Equal(expected, header.Text);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public unsafe void ColumnHeader_Text_GetColumn_Success(string value, string expected)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            header.Text = value;
            char* buffer = stackalloc char[256];
            var column = new ComCtl32.LVCOLUMNW
            {
                mask = ComCtl32.LVCF.TEXT,
                pszText = buffer,
                cchTextMax = 256
            };
            Assert.Equal((IntPtr)1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETCOLUMNW, (IntPtr)0, ref column));
            Assert.Equal(expected, new string(column.pszText));
        }

        [WinFormsTheory]
        [InlineData(RightToLeft.No, true, HorizontalAlignment.Left)]
        [InlineData(RightToLeft.Yes, true, HorizontalAlignment.Left)]
        [InlineData(RightToLeft.Inherit, true, HorizontalAlignment.Left)]
        [InlineData(RightToLeft.No, false, HorizontalAlignment.Left)]
        [InlineData(RightToLeft.Yes, false, HorizontalAlignment.Right)]
        [InlineData(RightToLeft.Inherit, false, HorizontalAlignment.Left)]
        public void ColumnHeader_TextAlign_GetWithListView_ReturnsExpected(RightToLeft rightToLeft, bool rightToLeftLayout, HorizontalAlignment expected)
        {
            using var listView = new ListView
            {
                RightToLeft = rightToLeft,
                RightToLeftLayout = rightToLeftLayout
            };
            using var header1 = new ColumnHeader();
            using var header2 = new ColumnHeader();
            listView.Columns.Add(header1);
            listView.Columns.Add(header2);
            Assert.Equal(HorizontalAlignment.Left, header1.TextAlign);
            Assert.Equal(expected, header2.TextAlign);

            // Get again to test caching.
            Assert.Equal(HorizontalAlignment.Left, header1.TextAlign);
            Assert.Equal(expected, header2.TextAlign);

            // Change listView.
            listView.RightToLeft = RightToLeft.No;
            listView.RightToLeftLayout = false;
            Assert.Equal(HorizontalAlignment.Left, header1.TextAlign);
            Assert.Equal(expected, header2.TextAlign);
        }

        [WinFormsFact]
        public void ColumnHeader_Text_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ColumnHeader))[nameof(ColumnHeader.Text)];
            using var header = new ColumnHeader();
            Assert.False(property.CanResetValue(header));

            // Set null.
            header.Text = null;
            Assert.Empty(header.Text);
            Assert.True(property.CanResetValue(header));

            // Set empty.
            header.Text = string.Empty;
            Assert.Empty(header.Text);
            Assert.True(property.CanResetValue(header));

            // Set custom
            header.Text = "text";
            Assert.Equal("text", header.Text);
            Assert.True(property.CanResetValue(header));

            property.ResetValue(header);
            Assert.Empty(header.Text);
            Assert.True(property.CanResetValue(header));
        }

        [WinFormsFact]
        public void ColumnHeader_Text_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ColumnHeader))[nameof(ColumnHeader.Text)];
            using var header = new ColumnHeader();
            Assert.False(property.ShouldSerializeValue(header));

            // Set null.
            header.Text = null;
            Assert.Empty(header.Text);
            Assert.True(property.ShouldSerializeValue(header));

            // Set empty.
            header.Text = string.Empty;
            Assert.Empty(header.Text);
            Assert.True(property.ShouldSerializeValue(header));

            // Set custom
            header.Text = "text";
            Assert.Equal("text", header.Text);
            Assert.True(property.ShouldSerializeValue(header));

            property.ResetValue(header);
            Assert.Empty(header.Text);
            Assert.True(property.ShouldSerializeValue(header));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HorizontalAlignment))]
        public void ColumnHeader_TextAlign_SetWithoutListView_GetReturnsExpected(HorizontalAlignment value)
        {
            using var header = new ColumnHeader
            {
                TextAlign = value
            };
            Assert.Equal(value, header.TextAlign);

            // Set same.
            header.TextAlign = value;
            Assert.Equal(value, header.TextAlign);
        }

        [WinFormsTheory]
        [InlineData(0, HorizontalAlignment.Center, HorizontalAlignment.Left)]
        [InlineData(0, HorizontalAlignment.Left, HorizontalAlignment.Left)]
        [InlineData(0, HorizontalAlignment.Right, HorizontalAlignment.Left)]
        [InlineData(1, HorizontalAlignment.Center, HorizontalAlignment.Center)]
        [InlineData(1, HorizontalAlignment.Left, HorizontalAlignment.Left)]
        [InlineData(1, HorizontalAlignment.Right, HorizontalAlignment.Right)]
        public void ColumnHeader_TextAlign_SetWithListView_GetReturnsExpected(int columnIndex, HorizontalAlignment value, HorizontalAlignment expected)
        {
            using var listView = new ListView();
            using var header1 = new ColumnHeader();
            using var header2 = new ColumnHeader();
            listView.Columns.Add(header1);
            listView.Columns.Add(header2);

            listView.Columns[columnIndex].TextAlign = value;
            Assert.Equal(expected, listView.Columns[columnIndex].TextAlign);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            listView.Columns[columnIndex].TextAlign = value;
            Assert.Equal(expected, listView.Columns[columnIndex].TextAlign);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0, HorizontalAlignment.Center, HorizontalAlignment.Left)]
        [InlineData(0, HorizontalAlignment.Left, HorizontalAlignment.Left)]
        [InlineData(0, HorizontalAlignment.Right, HorizontalAlignment.Left)]
        [InlineData(1, HorizontalAlignment.Center, HorizontalAlignment.Center)]
        [InlineData(1, HorizontalAlignment.Left, HorizontalAlignment.Left)]
        [InlineData(1, HorizontalAlignment.Right, HorizontalAlignment.Right)]
        public void ColumnHeader_TextAlign_SetWithListViewWithHandle_GetReturnsExpected(int columnIndex, HorizontalAlignment value, HorizontalAlignment expected)
        {
            using var listView = new ListView();
            using var header1 = new ColumnHeader();
            using var header2 = new ColumnHeader();
            listView.Columns.Add(header1);
            listView.Columns.Add(header2);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            listView.Columns[columnIndex].TextAlign = value;
            Assert.Equal(expected, listView.Columns[columnIndex].TextAlign);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            listView.Columns[columnIndex].TextAlign = value;
            Assert.Equal(expected, listView.Columns[columnIndex].TextAlign);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(0, HorizontalAlignment.Center, 0x4000)]
        [InlineData(0, HorizontalAlignment.Left, 0x4000)]
        [InlineData(0, HorizontalAlignment.Right, 0x4000)]
        [InlineData(1, HorizontalAlignment.Center, 0x4002)]
        [InlineData(1, HorizontalAlignment.Left, 0x4000)]
        [InlineData(1, HorizontalAlignment.Right, 0x4001)]
        public unsafe void ColumnHeader_TextAlign_GetColumn_Success(int columnIndex, HorizontalAlignment value, int expected)
        {
            using var listView = new ListView();
            using var header1 = new ColumnHeader();
            using var header2 = new ColumnHeader();
            listView.Columns.Add(header1);
            listView.Columns.Add(header2);

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            listView.Columns[columnIndex].TextAlign = value;
            var column = new ComCtl32.LVCOLUMNW
            {
                mask = ComCtl32.LVCF.FMT
            };
            Assert.Equal((IntPtr)1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETCOLUMNW, (IntPtr)columnIndex, ref column));
            Assert.Equal(expected, (int)column.fmt);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(HorizontalAlignment))]
        public void ColumnHeader_TextAlign_SetInvalid_ThrowsInvalidEnumArgumentException(HorizontalAlignment value)
        {
            using var header = new ColumnHeader();
            Assert.Throws<InvalidEnumArgumentException>("value", () => header.TextAlign = value);
        }

        [WinFormsFact]
        public void ColumnHeader_Width_GetWithListView_GetReturnsExpected()
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            Assert.Equal(60, header.Width);
        }

        [WinFormsFact]
        public void ColumnHeader_Width_GetWithListViewWithHandle_GetReturnsExpected()
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(60, header.Width);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ColumnHeader_Width_GetWithListViewWithHandleWithDetails_GetReturnsExpected()
        {
            using var listView = new ListView
            {
                View = View.Details
            };
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(header.Width, header.Width);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Width_Set_TestData()
        {
            yield return new object[] { -3 };
            yield return new object[] { -2 };
            yield return new object[] { -1 };
            yield return new object[] { 0 };
            yield return new object[] { 1 };
            yield return new object[] { 60 };
            yield return new object[] { 75 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Width_Set_TestData))]
        public void ColumnHeader_Width_SetWithoutListView_GetReturnsExpected(int value)
        {
            using var header = new ColumnHeader
            {
                Width = value
            };
            Assert.Equal(value, header.Width);

            // Set same.
            header.Width = value;
            Assert.Equal(value, header.Width);
        }

        [WinFormsTheory]
        [MemberData(nameof(Width_Set_TestData))]
        public void ColumnHeader_Width_SetWithListView_GetReturnsExpected(int value)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.Width = value;
            Assert.Equal(value, header.Width);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            header.Width = value;
            Assert.Equal(value, header.Width);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Width_Set_TestData))]
        public void ColumnHeader_Width_SetWithListViewWithHandle_GetReturnsExpected(int value)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            header.Width = value;
            Assert.Equal(value, header.Width);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            header.Width = value;
            Assert.Equal(value, header.Width);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(60)]
        [InlineData(75)]
        public void ColumnHeader_Width_GetColumnWidth_ReturnsExpected(int value)
        {
            using var listView = new ListView
            {
                View = View.Details
            };
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            header.Width = value;
            Assert.Equal((IntPtr)value, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETCOLUMNWIDTH, IntPtr.Zero, IntPtr.Zero));
        }

        [WinFormsTheory]
        [InlineData(-3)]
        [InlineData(-2)]
        [InlineData(-1)]
        public void ColumnHeader_Width_GetColumnWidthCustom_ReturnsExpected(int value)
        {
            using var listView = new ListView
            {
                View = View.Details
            };
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            header.Width = value;
            Assert.True(User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETCOLUMNWIDTH, IntPtr.Zero, IntPtr.Zero).ToInt32() > 0);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColumnHeaderAutoResizeStyle))]
        public void ColumnHeader_AutoSize_WithoutListView_Nop(ColumnHeaderAutoResizeStyle headerAutoResize)
        {
            using var header = new ColumnHeader();
            header.AutoResize(headerAutoResize);
            Assert.Equal(60, header.Width);

            // Call again.
            header.AutoResize(headerAutoResize);
            Assert.Equal(60, header.Width);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColumnHeaderAutoResizeStyle))]
        public void ColumnHeader_AutoSize_WithListView_Success(ColumnHeaderAutoResizeStyle headerAutoResize)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.AutoResize(headerAutoResize);
            Assert.Equal(60, header.Width);
            Assert.True(listView.IsHandleCreated);

            // Call again.
            header.AutoResize(headerAutoResize);
            Assert.Equal(60, header.Width);
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColumnHeaderAutoResizeStyle))]
        public void ColumnHeader_AutoSize_WithListViewWithHandle_Success(ColumnHeaderAutoResizeStyle headerAutoResize)
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            header.AutoResize(headerAutoResize);
            Assert.Equal(60, header.Width);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            header.AutoResize(headerAutoResize);
            Assert.Equal(60, header.Width);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ColumnHeaderAutoResizeStyle))]
        public void ColumnHeader_AutoSize_InvalidHeaderAutoResize_ThrowsInvalidEnumArgumentException(ColumnHeaderAutoResizeStyle headerAutoResize)
        {
            using var header = new ColumnHeader();
            Assert.Throws<InvalidEnumArgumentException>("headerAutoResize", () => header.AutoResize(headerAutoResize));
        }

        [WinFormsFact]
        public void ColumnHeader_Clone_Invoke_ReturnsExpected()
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            using var source = new ColumnHeader
            {
                DisplayIndex = 1,
                ImageKey = "imageKey",
                ImageIndex = 1,
                Name = "name",
                Site = mockSite.Object,
                Tag = "tag",
                Text = "text",
                TextAlign = HorizontalAlignment.Center,
                Width = 10
            };
            using ColumnHeader header = Assert.IsType<ColumnHeader>(source.Clone());
            Assert.NotSame(header, source);
            Assert.Null(header.Container);
            Assert.Equal(-1, header.DisplayIndex);
            Assert.Equal(-1, header.ImageIndex);
            Assert.Equal(string.Empty, header.ImageKey);
            Assert.Null(header.ImageList);
            Assert.Equal(-1, header.Index);
            Assert.Null(header.ListView);
            Assert.Empty(header.Name);
            Assert.Null(header.Site);
            Assert.Null(header.Tag);
            Assert.Equal("text", header.Text);
            Assert.Equal(HorizontalAlignment.Center, header.TextAlign);
            Assert.Equal(10, header.Width);
        }

        [WinFormsFact]
        public void ColumnHeader_Clone_InvokeWithListView_ReturnsExpected()
        {
            using var listView = new ListView();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            using var source = new ColumnHeader
            {
                DisplayIndex = 1,
                ImageKey = "imageKey",
                ImageIndex = 1,
                Name = "name",
                Site = mockSite.Object,
                Tag = "tag",
                Text = "text",
                TextAlign = HorizontalAlignment.Center,
                Width = 10
            };
            listView.Columns.Add(source);

            using ColumnHeader header = Assert.IsType<ColumnHeader>(source.Clone());
            Assert.NotSame(header, source);
            Assert.Null(header.Container);
            Assert.Equal(-1, header.DisplayIndex);
            Assert.Equal(-1, header.ImageIndex);
            Assert.Equal(string.Empty, header.ImageKey);
            Assert.Null(header.ImageList);
            Assert.Equal(-1, header.Index);
            Assert.Null(header.ListView);
            Assert.Empty(header.Name);
            Assert.Null(header.Site);
            Assert.Null(header.Tag);
            Assert.Equal("text", header.Text);
            Assert.Equal(HorizontalAlignment.Center, header.TextAlign);
            Assert.Equal(10, header.Width);
        }

        [WinFormsFact]
        public void ColumnHeader_Clone_InvokeSubClass_ReturnsExpected()
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            using var source = new SubColumnHeader
            {
                DisplayIndex = 1,
                ImageKey = "imageKey",
                ImageIndex = 1,
                Name = "name",
                Site = mockSite.Object,
                Tag = "tag",
                Text = "text",
                TextAlign = HorizontalAlignment.Center,
                Width = 10
            };
            using SubColumnHeader header = Assert.IsType<SubColumnHeader>(source.Clone());
            Assert.NotSame(header, source);
            Assert.True(header.CanRaiseEvents);
            Assert.Null(header.Container);
            Assert.False(header.DesignMode);
            Assert.Equal(-1, header.DisplayIndex);
            Assert.NotNull(header.Events);
            Assert.Same(header.Events, header.Events);
            Assert.Equal(-1, header.ImageIndex);
            Assert.Equal(string.Empty, header.ImageKey);
            Assert.Null(header.ImageList);
            Assert.Equal(-1, header.Index);
            Assert.Null(header.ListView);
            Assert.Empty(header.Name);
            Assert.Null(header.Site);
            Assert.Null(header.Tag);
            Assert.Equal("text", header.Text);
            Assert.Equal(HorizontalAlignment.Center, header.TextAlign);
            Assert.Equal(10, header.Width);
        }

        [WinFormsFact]
        public void ColumnHeader_Dispose_WithoutListView_Success()
        {
            using var header = new ColumnHeader();
            header.Dispose();
            header.Dispose();
        }

        [WinFormsFact]
        public void ColumnHeader_Dispose_WithListView_Success()
        {
            using var listView = new ListView();
            using var header = new ColumnHeader();
            listView.Columns.Add(header);
            header.Dispose();
            Assert.Empty(listView.Columns);

            header.Dispose();
            Assert.Empty(listView.Columns);
        }

        [WinFormsFact]
        public void ColumnHeader_ToString_Invoke_ReturnsExpected()
        {
            using var header = new ColumnHeader();
            Assert.Equal($"ColumnHeader: Text: ColumnHeader", header.ToString());
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ColumnHeader_ToString_InvokeWithText_ReturnsExpected(string value)
        {
            using var header = new ColumnHeader
            {
                Text = value
            };
            Assert.Equal($"ColumnHeader: Text: {header.Text}", header.ToString());
        }

        private class InvalidSetColumnListView : ListView
        {
            public bool MakeInvalid { get; set; }

            protected override void WndProc(ref Message m)
            {
                if (MakeInvalid && m.Msg == (int)LVM.SETCOLUMNW)
                {
                    m.Result = IntPtr.Zero;
                }
                else
                {
                    base.WndProc(ref m);
                }
            }
        }

        [WinFormsFact]
        public void ColumnHeader_ImageIndexer_SetImageList_Nop()
        {
            using (new NoAssertContext())
            {
                using var imageList = new ImageList();
                using var imageList2 = new ImageList();
                using var listView = new ListView
                {
                    SmallImageList = imageList
                };
                using var header = new ColumnHeader();
                listView.Columns.Add(header);
                var indexer = new ColumnHeader.ColumnHeaderImageListIndexer(header);
                indexer.ImageList = imageList2;
                Assert.Same(imageList, indexer.ImageList);
            }
        }

        private class SubColumnHeader : ColumnHeader
        {
            public SubColumnHeader() : base()
            {
            }

            public SubColumnHeader(int imageIndex) : base(imageIndex)
            {
            }

            public SubColumnHeader(string imageKey) : base(imageKey)
            {
            }

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new EventHandlerList Events => base.Events;

            public new bool DesignMode => base.DesignMode;
        }
    }
}
