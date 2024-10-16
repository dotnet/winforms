// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Tests;

public class ColumnHeaderTests
{
    [WinFormsFact]
    public void ColumnHeader_Ctor_Default()
    {
        using SubColumnHeader header = new();
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
        using SubColumnHeader header = new(imageIndex);
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
    [NormalizedStringData]
    public void ColumnHeader_Ctor_String(string imageKey, string expectedImageKey)
    {
        using SubColumnHeader header = new(imageKey);
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
        using ListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);
        Assert.Equal(0, header.DisplayIndex);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ColumnHeader_DisplayIndex_SetWithoutListView_GetReturnsExpected(int value)
    {
        using ColumnHeader header = new()
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
        using ListView listView = new();
        using ColumnHeader header1 = new();
        using ColumnHeader header2 = new();
        using ColumnHeader header3 = new();
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
        using ListView listView = new();
        using ColumnHeader header1 = new();
        using ColumnHeader header2 = new();
        using ColumnHeader header3 = new();
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
        using ListView listView = new();
        using ColumnHeader header1 = new();
        using ColumnHeader header2 = new();
        using ColumnHeader header3 = new();
        listView.Columns.Add(header1);
        listView.Columns.Add(header2);
        listView.Columns.Add(header3);

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        listView.Columns[columnIndex].DisplayIndex = value;
        int[] result = new int[3];
        Assert.Equal(1, (int)PInvokeCore.SendMessage(listView, PInvoke.LVM_GETCOLUMNORDERARRAY, 3, ref result[0]));
        Assert.Equal(expectedDisplayIndices, result);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ColumnHeader_DisplayIndex_SetInvalidWithListView_ThrowsArgumentOutOfRangeException(int value)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);
        Assert.Throws<ArgumentOutOfRangeException>("DisplayIndex", () => header.DisplayIndex = value);
    }

    [WinFormsFact]
    public void ColumnHeader_DisplayIndex_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ColumnHeader))[nameof(ColumnHeader.DisplayIndex)];
        using ColumnHeader item = new();
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
        using ColumnHeader item = new();
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
        using ColumnHeader header = new()
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
        using ColumnHeader header = new()
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
        using ListView listView = new();
        using ColumnHeader header = new();
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
        using ImageList imageList = new();
        using ListView listView = new()
        {
            SmallImageList = imageList
        };
        using ColumnHeader header = new();
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
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image1);
        imageList.Images.Add(image2);
        using ListView listView = new()
        {
            SmallImageList = imageList
        };
        using ColumnHeader header = new();
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
        using ListView listView = new();
        using ColumnHeader header = new();
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
        using ListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        header.ImageIndex = value;
        LVCOLUMNW column = new()
        {
            mask = LVCOLUMNW_MASK.LVCF_IMAGE
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(listView, PInvoke.LVM_GETCOLUMNW, 0, ref column));
        Assert.Equal(0, column.iImage);
    }

    [WinFormsTheory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 0)]
    public void ColumnHeader_ImageIndex_GetColumnWithImageList_Success(int value, int expected)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image1);
        imageList.Images.Add(image2);
        using ListView listView = new()
        {
            SmallImageList = imageList
        };
        using ColumnHeader header = new();
        listView.Columns.Add(header);

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        header.ImageIndex = value;
        LVCOLUMNW column = new()
        {
            mask = LVCOLUMNW_MASK.LVCF_IMAGE | LVCOLUMNW_MASK.LVCF_FMT,
            fmt = LVCOLUMNW_FORMAT.LVCFMT_IMAGE
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(listView, PInvoke.LVM_GETCOLUMNW, 0, ref column));
        Assert.Equal(expected, column.iImage);
    }

    [WinFormsTheory]
    [InlineData(-2)]
    public void ColumnHeader_ImageIndex_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
    {
        using ColumnHeader header = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => header.ImageIndex = value);
    }

    [WinFormsFact]
    public void ColumnHeader_ImageIndex_SetInvalidSetColumn_ThrowsInvalidOperationException()
    {
        using InvalidSetColumnListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);
        Assert.NotEqual(IntPtr.Zero, listView.Handle);

        listView.MakeInvalid = true;
        Assert.Throws<InvalidOperationException>(() => header.ImageIndex = 0);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ColumnHeader_ImageKey_SetWithoutListView_GetReturnsExpected(string value, string expected)
    {
        using ColumnHeader header = new()
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
        using ColumnHeader header = new()
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
    [NormalizedStringData]
    public void ColumnHeader_ImageKey_SetWithListView_GetReturnsExpected(string value, string expected)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
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
    [NormalizedStringData]
    public void ColumnHeader_ImageKey_SetWithListViewWithEmptyList_GetReturnsExpected(string value, string expected)
    {
        using ImageList imageList = new();
        using ListView listView = new()
        {
            SmallImageList = imageList
        };
        using ColumnHeader header = new();
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
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add("Image1", image1);
        imageList.Images.Add("Image2", image2);
        using ListView listView = new()
        {
            SmallImageList = imageList
        };
        using ColumnHeader header = new();
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
    [NormalizedStringData]
    public void ColumnHeader_ImageKey_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
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
        using ListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        header.ImageKey = value;
        LVCOLUMNW column = new()
        {
            mask = LVCOLUMNW_MASK.LVCF_IMAGE
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(listView, PInvoke.LVM_GETCOLUMNW, 0, ref column));
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
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add("Image1", image1);
        imageList.Images.Add("Image2", image2);
        using ListView listView = new()
        {
            SmallImageList = imageList
        };
        using ColumnHeader header = new();
        listView.Columns.Add(header);

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        header.ImageKey = value;
        LVCOLUMNW column = new()
        {
            mask = LVCOLUMNW_MASK.LVCF_IMAGE | LVCOLUMNW_MASK.LVCF_FMT,
            fmt = LVCOLUMNW_FORMAT.LVCFMT_IMAGE
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(listView, PInvoke.LVM_GETCOLUMNW, 0, ref column));
        Assert.Equal(expected, column.iImage);
    }

    [WinFormsFact]
    public void ColumnHeader_ImageKey_SetInvalidSetColumn_ThrowsInvalidOperationException()
    {
        using InvalidSetColumnListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);
        Assert.NotEqual(IntPtr.Zero, listView.Handle);

        listView.MakeInvalid = true;
        Assert.Throws<InvalidOperationException>(() => header.ImageKey = "Key");
    }

    [WinFormsFact]
    public void ColumnHeader_ImageList_GetWithListViewWithoutImageList_ReturnsExpected()
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);
        Assert.Null(header.ImageList);
    }

    [WinFormsFact]
    public void ColumnHeader_ImageList_GetWithListViewWithImageList_ReturnsExpected()
    {
        using ImageList imageList = new();
        using ListView listView = new()
        {
            SmallImageList = imageList
        };
        using ColumnHeader header = new();
        listView.Columns.Add(header);
        Assert.Same(listView.SmallImageList, header.ImageList);
    }

    [WinFormsFact]
    public void ColumnHeader_Index_GetWithListView_ReturnsExpected()
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);
        Assert.Equal(0, header.Index);
    }

    [WinFormsFact]
    public void ColumnHeader_ListView_GetWithListView_ReturnsExpected()
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);
        Assert.Same(listView, header.ListView);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ColumnHeader_Name_GetWithSite_ReturnsExpected(string name, string expected)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(x => x.Name)
            .Returns(name);
        mockSite
            .Setup(x => x.Container)
            .Returns((IContainer)null);
        using ColumnHeader header = new()
        {
            Site = mockSite.Object
        };
        Assert.Equal(expected, header.Name);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ColumnHeader_Name_SetWithoutListView_GetReturnsExpected(string value, string expected)
    {
        using ColumnHeader header = new()
        {
            Name = value
        };
        Assert.Equal(expected, header.Name);

        // Set same.
        header.Name = value;
        Assert.Equal(expected, header.Name);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ColumnHeader_Name_SetWithListView_GetReturnsExpected(string value, string expected)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
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
    [NormalizedStringData]
    public void ColumnHeader_Name_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
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
    [NormalizedStringData]
    public void ColumnHeader_Name_SetWithSite_GetReturnsExpected(string value, string expected)
    {
        using ColumnHeader header = new()
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
        using ColumnHeader header = new();
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
        using ColumnHeader header = new();
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
        using ColumnHeader header = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
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
    [StringWithNullData]
    public void ColumnHeader_Tag_Set_GetReturnsExpected(object value)
    {
        using ColumnHeader header = new()
        {
            Tag = value
        };
        Assert.Equal(value, header.Tag);

        // Set same.
        header.Tag = value;
        Assert.Equal(value, header.Tag);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ColumnHeader_Text_SetWithoutListView_GetReturnsExpected(string value, string expected)
    {
        using ColumnHeader header = new()
        {
            Text = value
        };
        Assert.Equal(expected, header.Text);

        // Set same.
        header.Text = value;
        Assert.Equal(expected, header.Text);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ColumnHeader_Text_SetWithListView_GetReturnsExpected(string value, string expected)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);

        header.Text = value;
        Assert.Equal(expected, header.Text);

        // Set same.
        header.Text = value;
        Assert.Equal(expected, header.Text);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ColumnHeader_Text_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);
        Assert.NotEqual(IntPtr.Zero, listView.Handle);

        header.Text = value;
        Assert.Equal(expected, header.Text);

        // Set same.
        header.Text = value;
        Assert.Equal(expected, header.Text);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public unsafe void ColumnHeader_Text_GetColumn_Success(string value, string expected)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        header.Text = value;
        char* buffer = stackalloc char[256];
        LVCOLUMNW column = new()
        {
            mask = LVCOLUMNW_MASK.LVCF_TEXT,
            pszText = buffer,
            cchTextMax = 256
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(listView, PInvoke.LVM_GETCOLUMNW, 0, ref column));
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
        using ListView listView = new()
        {
            RightToLeft = rightToLeft,
            RightToLeftLayout = rightToLeftLayout
        };
        using ColumnHeader header1 = new();
        using ColumnHeader header2 = new();
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
        using ColumnHeader header = new();
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
        using ColumnHeader header = new();
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
    [EnumData<HorizontalAlignment>]
    public void ColumnHeader_TextAlign_SetWithoutListView_GetReturnsExpected(HorizontalAlignment value)
    {
        using ColumnHeader header = new()
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
        using ListView listView = new();
        using ColumnHeader header1 = new();
        using ColumnHeader header2 = new();
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
        using ListView listView = new();
        using ColumnHeader header1 = new();
        using ColumnHeader header2 = new();
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
        using ListView listView = new();
        using ColumnHeader header1 = new();
        using ColumnHeader header2 = new();
        listView.Columns.Add(header1);
        listView.Columns.Add(header2);

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        listView.Columns[columnIndex].TextAlign = value;
        LVCOLUMNW column = new()
        {
            mask = LVCOLUMNW_MASK.LVCF_FMT
        };
        Assert.Equal(1, (int)PInvokeCore.SendMessage(listView, PInvoke.LVM_GETCOLUMNW, (WPARAM)columnIndex, ref column));
        Assert.Equal(expected, (int)column.fmt);
    }

    [WinFormsTheory]
    [InvalidEnumData<HorizontalAlignment>]
    public void ColumnHeader_TextAlign_SetInvalid_ThrowsInvalidEnumArgumentException(HorizontalAlignment value)
    {
        using ColumnHeader header = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => header.TextAlign = value);
    }

    [WinFormsFact]
    public void ColumnHeader_Width_GetWithListView_GetReturnsExpected()
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);

        Assert.Equal(60, header.Width);
    }

    [WinFormsFact]
    public void ColumnHeader_Width_GetWithListViewWithHandle_GetReturnsExpected()
    {
        using ListView listView = new();
        using ColumnHeader header = new();
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
        using ListView listView = new()
        {
            View = View.Details
        };
        using ColumnHeader header = new();
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
        using ColumnHeader header = new()
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
        using ListView listView = new();
        using ColumnHeader header = new();
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
        using ListView listView = new();
        using ColumnHeader header = new();
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
        using ListView listView = new()
        {
            View = View.Details
        };
        using ColumnHeader header = new();
        listView.Columns.Add(header);

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        header.Width = value;
        Assert.Equal(value, (int)PInvokeCore.SendMessage(listView, PInvoke.LVM_GETCOLUMNWIDTH));
    }

    [WinFormsTheory]
    [InlineData(-3)]
    [InlineData(-2)]
    [InlineData(-1)]
    public void ColumnHeader_Width_GetColumnWidthCustom_ReturnsExpected(int value)
    {
        using ListView listView = new()
        {
            View = View.Details
        };
        using ColumnHeader header = new();
        listView.Columns.Add(header);

        Assert.NotEqual(IntPtr.Zero, listView.Handle);
        header.Width = value;
        Assert.True(PInvokeCore.SendMessage(listView, PInvoke.LVM_GETCOLUMNWIDTH) > 0);
    }

    [WinFormsTheory]
    [EnumData<ColumnHeaderAutoResizeStyle>]
    public void ColumnHeader_AutoSize_WithoutListView_Nop(ColumnHeaderAutoResizeStyle headerAutoResize)
    {
        using ColumnHeader header = new();
        header.AutoResize(headerAutoResize);
        Assert.Equal(60, header.Width);

        // Call again.
        header.AutoResize(headerAutoResize);
        Assert.Equal(60, header.Width);
    }

    [WinFormsTheory]
    [EnumData<ColumnHeaderAutoResizeStyle>]
    public void ColumnHeader_AutoSize_WithListView_Success(ColumnHeaderAutoResizeStyle headerAutoResize)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
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
    [EnumData<ColumnHeaderAutoResizeStyle>]
    public void ColumnHeader_AutoSize_WithListViewWithHandle_Success(ColumnHeaderAutoResizeStyle headerAutoResize)
    {
        using ListView listView = new();
        using ColumnHeader header = new();
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
    [InvalidEnumData<ColumnHeaderAutoResizeStyle>]
    public void ColumnHeader_AutoSize_InvalidHeaderAutoResize_ThrowsInvalidEnumArgumentException(ColumnHeaderAutoResizeStyle headerAutoResize)
    {
        using ColumnHeader header = new();
        Assert.Throws<InvalidEnumArgumentException>("headerAutoResize", () => header.AutoResize(headerAutoResize));
    }

    [WinFormsFact]
    public void ColumnHeader_Clone_Invoke_ReturnsExpected()
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using ColumnHeader source = new()
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
        using ListView listView = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using ColumnHeader source = new()
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
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubColumnHeader source = new()
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
        using ColumnHeader header = new();
        header.Dispose();
        header.Dispose();
    }

    [WinFormsFact]
    public void ColumnHeader_Dispose_WithListView_Success()
    {
        using ListView listView = new();
        using ColumnHeader header = new();
        listView.Columns.Add(header);
        header.Dispose();
        Assert.Empty(listView.Columns);

        header.Dispose();
        Assert.Empty(listView.Columns);
    }

    [WinFormsFact]
    public void ColumnHeader_ToString_Invoke_ReturnsExpected()
    {
        using ColumnHeader header = new();
        Assert.Equal($"ColumnHeader: Text: ColumnHeader", header.ToString());
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ColumnHeader_ToString_InvokeWithText_ReturnsExpected(string value)
    {
        using ColumnHeader header = new()
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
            if (MakeInvalid && m.Msg == (int)PInvoke.LVM_SETCOLUMNW)
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
            using ImageList imageList = new();
            using ImageList imageList2 = new();
            using ListView listView = new()
            {
                SmallImageList = imageList
            };
            using ColumnHeader header = new();
            listView.Columns.Add(header);
            var indexer = new ColumnHeader.ColumnHeaderImageListIndexer(header)
            {
                ImageList = imageList2
            };
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
