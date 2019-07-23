// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ColumnHeaderTests
    {
        [Fact]
        public void ColumnHeader_Ctor_Default()
        {
            var header = new ColumnHeader();
            Assert.Equal(-1, header.DisplayIndex);
            Assert.Equal(-1, header.ImageIndex);
            Assert.Equal(string.Empty, header.ImageKey);
            Assert.Null(header.ImageList);
            Assert.Equal(-1, header.Index);
            Assert.Null(header.ListView);
            Assert.Empty(header.Name);
            Assert.Null(header.Tag);
            Assert.Equal("ColumnHeader", header.Text);
            Assert.Equal(HorizontalAlignment.Left, header.TextAlign);
            Assert.Equal(60, header.Width);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_Ctor_Int(int imageIndex)
        {
            var header = new ColumnHeader(imageIndex);
            Assert.Equal(-1, header.DisplayIndex);
            Assert.Equal(imageIndex, header.ImageIndex);
            Assert.Equal(string.Empty, header.ImageKey);
            Assert.Null(header.ImageList);
            Assert.Equal(-1, header.Index);
            Assert.Null(header.ListView);
            Assert.Empty(header.Name);
            Assert.Null(header.Tag);
            Assert.Equal("ColumnHeader", header.Text);
            Assert.Equal(HorizontalAlignment.Left, header.TextAlign);
            Assert.Equal(60, header.Width);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Ctor_String(string imageKey, string expectedImageKey)
        {
            var header = new ColumnHeader(imageKey);
            Assert.Equal(-1, header.DisplayIndex);
            Assert.Equal(-1, header.ImageIndex);
            Assert.Equal(expectedImageKey, header.ImageKey);
            Assert.Null(header.ImageList);
            Assert.Equal(-1, header.Index);
            Assert.Null(header.ListView);
            Assert.Empty(header.Name);
            Assert.Null(header.Tag);
            Assert.Equal("ColumnHeader", header.Text);
            Assert.Equal(HorizontalAlignment.Left, header.TextAlign);
            Assert.Equal(60, header.Width);
        }

        [Fact]
        public void ColumnHeader_DisplayIndex_GetWithListView_ReturnsExpected()
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.Equal(0, header.DisplayIndex);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_DisplayIndex_SetWithoutListView_GetReturnsExpected(int value)
        {
            var header = new ColumnHeader
            {
                DisplayIndex = value
            };
            Assert.Equal(value, header.DisplayIndex);

            // Set same.
            header.DisplayIndex = value;
            Assert.Equal(value, header.DisplayIndex);
        }

        [Theory]
        [InlineData(0)]
        public void ColumnHeader_DisplayIndex_SetWithListView_GetReturnsExpected(int value)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.DisplayIndex = value;
            Assert.Equal(value, header.DisplayIndex);

            // Set same.
            header.DisplayIndex = value;
            Assert.Equal(value, header.DisplayIndex);
        }

        [Theory]
        [InlineData(0)]
        public void ColumnHeader_DisplayIndex_SetWithListViewWithHandle_GetReturnsExpected(int value)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            header.DisplayIndex = value;
            Assert.Equal(value, header.DisplayIndex);

            // Set same.
            header.DisplayIndex = value;
            Assert.Equal(value, header.DisplayIndex);
        }

        [Theory]
        [InlineData(0, 1, 2)]
        [InlineData(1, 0, 2)]
        [InlineData(2, 0, 1)]
        public void ColumnHeader_DisplayIndex_SetFirstOfMultipleColumns_GetReturnsExpected(int value, int expectedHeader2, int expectedHeader3)
        {
            var listView = new ListView();
            var header1 = new ColumnHeader();
            var header2 = new ColumnHeader();
            var header3 = new ColumnHeader();
            listView.Columns.Add(header1);
            listView.Columns.Add(header2);
            listView.Columns.Add(header3);

            header1.DisplayIndex = value;
            Assert.Equal(value, header1.DisplayIndex);
            Assert.Equal(expectedHeader2, header2.DisplayIndex);
            Assert.Equal(expectedHeader3, header3.DisplayIndex);

            // Set same.
            header1.DisplayIndex = value;
            Assert.Equal(value, header1.DisplayIndex);
            Assert.Equal(expectedHeader2, header2.DisplayIndex);
            Assert.Equal(expectedHeader3, header3.DisplayIndex);
        }

        [Theory]
        [InlineData(0, 1, 2)]
        [InlineData(1, 0, 2)]
        [InlineData(2, 0, 1)]
        public void ColumnHeader_DisplayIndex_SetSecondOfMultipleColumns_GetReturnsExpected(int value, int expectedHeader1, int expectedHeader3)
        {
            var listView = new ListView();
            var header1 = new ColumnHeader();
            var header2 = new ColumnHeader();
            var header3 = new ColumnHeader();
            listView.Columns.Add(header1);
            listView.Columns.Add(header2);
            listView.Columns.Add(header3);

            header2.DisplayIndex = value;
            Assert.Equal(value, header2.DisplayIndex);
            Assert.Equal(expectedHeader1, header1.DisplayIndex);
            Assert.Equal(expectedHeader3, header3.DisplayIndex);

            // Set same.
            header2.DisplayIndex = value;
            Assert.Equal(value, header2.DisplayIndex);
            Assert.Equal(expectedHeader1, header1.DisplayIndex);
            Assert.Equal(expectedHeader3, header3.DisplayIndex);
        }

        [Theory]
        [InlineData(0, 1, 2)]
        [InlineData(1, 0, 2)]
        [InlineData(2, 0, 1)]
        public void ColumnHeader_DisplayIndex_SetLastOfMultipleColumns_GetReturnsExpected(int value, int expectedHeader1, int expectedHeader2)
        {
            var listView = new ListView();
            var header1 = new ColumnHeader();
            var header2 = new ColumnHeader();
            var header3 = new ColumnHeader();
            listView.Columns.Add(header1);
            listView.Columns.Add(header2);
            listView.Columns.Add(header3);

            header3.DisplayIndex = value;
            Assert.Equal(value, header3.DisplayIndex);
            Assert.Equal(expectedHeader1, header1.DisplayIndex);
            Assert.Equal(expectedHeader2, header2.DisplayIndex);

            // Set same.
            header3.DisplayIndex = value;
            Assert.Equal(value, header3.DisplayIndex);
            Assert.Equal(expectedHeader1, header1.DisplayIndex);
            Assert.Equal(expectedHeader2, header2.DisplayIndex);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ColumnHeader_DisplayIndex_SetInvalidWithListView_ThrowsArgumentOutOfRangeException(int value)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.Throws<ArgumentOutOfRangeException>("DisplayIndex", () => header.DisplayIndex = value);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_ImageIndex_SetWithoutListView_GetReturnsExpected(int value)
        {
            var header = new ColumnHeader
            {
                ImageIndex = value
            };
            Assert.Equal(value, header.ImageIndex);

            // Set same.
            header.ImageIndex = value;
            Assert.Equal(value, header.ImageIndex);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_ImageIndex_SetWithListView_GetReturnsExpected(int value)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.ImageIndex = value;
            Assert.Equal(value, header.ImageIndex);

            // Set same.
            header.ImageIndex = value;
            Assert.Equal(value, header.ImageIndex);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_ImageIndex_SetWithListViewWithList_GetReturnsExpected(int value)
        {
            var listView = new ListView
            {
                SmallImageList = new ImageList()
            };
            var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.ImageIndex = value;
            Assert.Equal(-1, header.ImageIndex);

            // Set same.
            header.ImageIndex = value;
            Assert.Equal(-1, header.ImageIndex);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ColumnHeader_ImageIndex_SetWithListViewWithHandle_GetReturnsExpected(int value)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            header.ImageIndex = value;
            Assert.Equal(value, header.ImageIndex);

            // Set same.
            header.ImageIndex = value;
            Assert.Equal(value, header.ImageIndex);
        }

        [Theory]
        [InlineData(-2)]
        public void ColumnHeader_ImageIndex_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            var header = new ColumnHeader();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => header.ImageIndex = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_ImageKey_SetWithoutListView_GetReturnsExpected(string value, string expected)
        {
            var header = new ColumnHeader
            {
                ImageKey = value
            };
            Assert.Equal(expected, header.ImageKey);

            // Set same.
            header.ImageKey = value;
            Assert.Equal(expected, header.ImageKey);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_ImageKey_SetWithListView_GetReturnsExpected(string value, string expected)
        {
            var listView = new ListView();
            var header = new ColumnHeader
            {
                ImageKey = value
            };
            listView.Columns.Add(header);
            Assert.Equal(expected, header.ImageKey);

            // Set same.
            header.ImageKey = value;
            Assert.Equal(expected, header.ImageKey);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_ImageKey_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
        {
            var listView = new ListView();
            var header = new ColumnHeader
            {
                ImageKey = value
            };
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            Assert.Equal(expected, header.ImageKey);

            // Set same.
            header.ImageKey = value;
            Assert.Equal(expected, header.ImageKey);
        }

        [Fact]
        public void ColumnHeader_ImageList_GetWithListView_ReturnsExpected()
        {
            var listView = new ListView
            {
                SmallImageList = new ImageList()
            };
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.Same(listView.SmallImageList, header.ImageList);
        }

        [Fact]
        public void ColumnHeader_Index_GetWithListView_ReturnsExpected()
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.Equal(0, header.Index);
        }

        [Fact]
        public void ColumnHeader_ListView_GetWithListView_ReturnsExpected()
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.Same(listView, header.ListView);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Name_GetWithSite_ReturnsExpected(string name, string expected)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(x => x.Name)
                .Returns(name);

            var header = new ColumnHeader
            {
                Site = mockSite.Object
            };
            Assert.Equal(expected, header.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Name_SetWithoutListView_GetReturnsExpected(string value, string expected)
        {
            var header = new ColumnHeader
            {
                Name = value
            };
            Assert.Equal(expected, header.Name);

            // Set same.
            header.Name = value;
            Assert.Equal(expected, header.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Name_SetWithListView_GetReturnsExpected(string value, string expected)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.Name = value;
            Assert.Equal(expected, header.Name);

            // Set same.
            header.Name = value;
            Assert.Equal(expected, header.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Name_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            header.Name = value;
            Assert.Equal(expected, header.Name);

            // Set same.
            header.Name = value;
            Assert.Equal(expected, header.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Name_SetWithSite_GetReturnsExpected(string value, string expected)
        {
            var header = new ColumnHeader
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ColumnHeader_Tag_Set_GetReturnsExpected(object value)
        {
            var header = new ColumnHeader
            {
                Tag = value
            };
            Assert.Equal(value, header.Tag);

            // Set same.
            header.Tag = value;
            Assert.Equal(value, header.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Text_SetWithoutListView_GetReturnsExpected(string value, string expected)
        {
            var header = new ColumnHeader
            {
                Text = value
            };
            Assert.Equal(expected, header.Text);

            // Set same.
            header.Text = value;
            Assert.Equal(expected, header.Text);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Text_SetWithListView_GetReturnsExpected(string value, string expected)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.Text = value;
            Assert.Equal(expected, header.Text);

            // Set same.
            header.Text = value;
            Assert.Equal(expected, header.Text);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ColumnHeader_Text_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            header.Text = value;
            Assert.Equal(expected, header.Text);

            // Set same.
            header.Text = value;
            Assert.Equal(expected, header.Text);
        }

        [Theory]
        [InlineData(RightToLeft.No, true, HorizontalAlignment.Left)]
        [InlineData(RightToLeft.Yes, true, HorizontalAlignment.Left)]
        [InlineData(RightToLeft.Inherit, true, HorizontalAlignment.Left)]
        [InlineData(RightToLeft.No, false, HorizontalAlignment.Left)]
        [InlineData(RightToLeft.Yes, false, HorizontalAlignment.Right)]
        [InlineData(RightToLeft.Inherit, false, HorizontalAlignment.Left)]
        public void ColumnHeader_TextAlign_GetWithListView_ReturnsExpected(RightToLeft rightToLeft, bool rightToLeftLayout, HorizontalAlignment expected)
        {
            var listView = new ListView
            {
                RightToLeft = rightToLeft,
                RightToLeftLayout = rightToLeftLayout
            };
            var header1 = new ColumnHeader();
            var header2 = new ColumnHeader();
            listView.Columns.Add(header1);
            listView.Columns.Add(header2);
            Assert.Equal(HorizontalAlignment.Left, header1.TextAlign);
            Assert.Equal(expected, header2.TextAlign);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HorizontalAlignment))]
        public void ColumnHeader_TextAlign_SetWithoutListView_GetReturnsExpected(HorizontalAlignment value)
        {
            var header = new ColumnHeader
            {
                TextAlign = value
            };
            Assert.Equal(value, header.TextAlign);

            // Set same.
            header.TextAlign = value;
            Assert.Equal(value, header.TextAlign);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HorizontalAlignment))]
        public void ColumnHeader_TextAlign_SetWithListView_GetReturnsExpected(HorizontalAlignment value)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.TextAlign = value;
            Assert.Equal(HorizontalAlignment.Left, header.TextAlign);

            // Set same.
            header.TextAlign = value;
            Assert.Equal(HorizontalAlignment.Left, header.TextAlign);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HorizontalAlignment))]
        public void ColumnHeader_TextAlign_SetWithListViewNonZeroIndex_GetReturnsExpected(HorizontalAlignment value)
        {
            var listView = new ListView();
            listView.Columns.Add(new ColumnHeader());
            var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.TextAlign = value;
            Assert.Equal(value, header.TextAlign);

            // Set same.
            header.TextAlign = value;
            Assert.Equal(value, header.TextAlign);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HorizontalAlignment))]
        public void ColumnHeader_TextAlign_SetWithListViewWithHandle_GetReturnsExpected(HorizontalAlignment value)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            header.TextAlign = value;
            Assert.Equal(HorizontalAlignment.Left, header.TextAlign);

            // Set same.
            header.TextAlign = value;
            Assert.Equal(HorizontalAlignment.Left, header.TextAlign);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HorizontalAlignment))]
        public void ColumnHeader_TextAlign_SetWithListViewWithHandleNonZeroIndex_GetReturnsExpected(HorizontalAlignment value)
        {
            var listView = new ListView();
            listView.Columns.Add(new ColumnHeader());
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            header.TextAlign = value;
            Assert.Equal(value, header.TextAlign);

            // Set same.
            header.TextAlign = value;
            Assert.Equal(value, header.TextAlign);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(HorizontalAlignment))]
        public void ColumnHeader_TextAlign_SetInvalid_ThrowsInvalidEnumArgumentException(HorizontalAlignment value)
        {
            var header = new ColumnHeader();
            Assert.Throws<InvalidEnumArgumentException>("value", () => header.TextAlign = value);
        }

        [Fact]
        public void ColumnHeader_Width_GetWithListView_GetReturnsExpected()
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);

            Assert.Equal(60, header.Width);
        }

        [Fact]
        public void ColumnHeader_Width_GetWithListViewWithHandle_GetReturnsExpected()
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            Assert.Equal(60, header.Width);
        }

        [Fact]
        public void ColumnHeader_Width_GetWithListViewWithHandleWithDetails_GetReturnsExpected()
        {
            var listView = new ListView
            {
                View = View.Details
            };
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            Assert.Equal(header.Width, header.Width);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void ColumnHeader_Width_SetWithoutListView_GetReturnsExpected(int value)
        {
            var header = new ColumnHeader
            {
                Width = value
            };
            Assert.Equal(value, header.Width);

            // Set same.
            header.Width = value;
            Assert.Equal(value, header.Width);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void ColumnHeader_Width_SetWithListView_GetReturnsExpected(int value)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);

            header.Width = value;
            Assert.Equal(value, header.Width);

            // Set same.
            header.Width = value;
            Assert.Equal(value, header.Width);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void ColumnHeader_Width_SetWithListViewWithHandle_GetReturnsExpected(int value)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            header.Width = value;
            Assert.Equal(value, header.Width);

            // Set same.
            header.Width = value;
            Assert.Equal(value, header.Width);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColumnHeaderAutoResizeStyle))]
        public void ColumnHeader_AutoSize_WithoutListView_Nop(ColumnHeaderAutoResizeStyle headerAutoResize)
        {
            var header = new ColumnHeader();
            header.AutoResize(headerAutoResize);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColumnHeaderAutoResizeStyle))]
        public void ColumnHeader_AutoSize_WithListView_Success(ColumnHeaderAutoResizeStyle headerAutoResize)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            header.AutoResize(headerAutoResize);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColumnHeaderAutoResizeStyle))]
        public void ColumnHeader_AutoSize_WithListViewWithHandle_Success(ColumnHeaderAutoResizeStyle headerAutoResize)
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            header.AutoResize(headerAutoResize);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ColumnHeaderAutoResizeStyle))]
        public void ColumnHeader_AutoSize_InvalidHeaderAutoResize_ThrowsInvalidEnumArgumentException(ColumnHeaderAutoResizeStyle headerAutoResize)
        {
            var header = new ColumnHeader();
            Assert.Throws<InvalidEnumArgumentException>("headerAutoResize", () => header.AutoResize(headerAutoResize));
        }

        [Fact]
        public void ColumnHeader_Clone_Invoke_ReturnsExpected()
        {
            ColumnHeader source = new ColumnHeader
            {
                ImageKey = "imageKey",
                ImageIndex = 1,
                Name = "name",
                Tag = "tag",
                Text = "text",
                TextAlign = HorizontalAlignment.Center,
                Width = 10
            };
            ColumnHeader header = Assert.IsType<ColumnHeader>(source.Clone());
            Assert.Equal(-1, header.DisplayIndex);
            Assert.Equal(-1, header.ImageIndex);
            Assert.Equal(string.Empty, header.ImageKey);
            Assert.Null(header.ImageList);
            Assert.Equal(-1, header.Index);
            Assert.Null(header.ListView);
            Assert.Empty(header.Name);
            Assert.Null(header.Tag);
            Assert.Equal("text", header.Text);
            Assert.Equal(HorizontalAlignment.Center, header.TextAlign);
            Assert.Equal(10, header.Width);
        }

        [Fact]
        public void ColumnHeader_Clone_InvokeSubClass_ReturnsExpected()
        {
            CustomColumnHeader source = new CustomColumnHeader
            {
                ImageKey = "imageKey",
                ImageIndex = 1,
                Name = "name",
                Tag = "tag",
                Text = "text",
                TextAlign = HorizontalAlignment.Center,
                Width = 10
            };
            CustomColumnHeader header = Assert.IsType<CustomColumnHeader>(source.Clone());
            Assert.Equal(-1, header.DisplayIndex);
            Assert.Equal(-1, header.ImageIndex);
            Assert.Equal(string.Empty, header.ImageKey);
            Assert.Null(header.ImageList);
            Assert.Equal(-1, header.Index);
            Assert.Null(header.ListView);
            Assert.Empty(header.Name);
            Assert.Null(header.Tag);
            Assert.Equal("text", header.Text);
            Assert.Equal(HorizontalAlignment.Center, header.TextAlign);
            Assert.Equal(10, header.Width);
        }

        [Fact]
        public void ColumnHeader_Dispose_WithoutListView_Success()
        {
            var header = new ColumnHeader();
            header.Dispose();
            header.Dispose();
        }

        [Fact]
        public void ColumnHeader_Dispose_WithListView_Success()
        {
            var listView = new ListView();
            var header = new ColumnHeader();
            listView.Columns.Add(header);
            header.Dispose();
            Assert.Empty(listView.Columns);

            header.Dispose();
            Assert.Empty(listView.Columns);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ColumnHeader_ToString_Invoke_ReturnsExpected(string value)
        {
            var header = new ColumnHeader
            {
                Text = value
            };
            Assert.Equal($"ColumnHeader: Text: {header.Text}", header.ToString());
        }

        private class CustomColumnHeader : ColumnHeader
        {
        }
    }
}
