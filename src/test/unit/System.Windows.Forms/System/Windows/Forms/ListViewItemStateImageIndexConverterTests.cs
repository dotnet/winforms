// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using static System.ComponentModel.TypeConverter;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ListViewItemStateImageIndexConverterTests
{
    [Fact]
    public void ListViewItemStateImageIndexConverter_IncludeNoneAsStandardValue_ReturnsFalse()
    {
        Assert.False(new ListViewItemStateImageIndexConverter().TestAccessor().Dynamic.IncludeNoneAsStandardValue);
    }

    [Fact]
    public void ListViewItemStateImageIndexConverter_GetStandardValues_Null_Context_ReturnsExpected()
    {
        ListViewItemStateImageIndexConverter converter = new();

        StandardValuesCollection result = converter.GetStandardValues(context: null);

        Assert.Empty(result);
    }

    [Fact]
    public void GetStandardValues_ContextWithoutListView_ReturnsEmpty()
    {
        object instance = new();
        ITypeDescriptorContext context = new TypeDescriptorContextStub(instance);
        ListViewItemStateImageIndexConverter converter = new();

        StandardValuesCollection result = converter.GetStandardValues(context);

        result.Cast<object>().Should().BeEmpty();
    }

    [Fact]
    public void GetStandardValues_WithListViewWithoutStateImageList_ReturnsEmpty()
    {
        using ListView listView = new();
        ListViewItem item = listView.Items.Add("test");

        ITypeDescriptorContext context = new TypeDescriptorContextStub(item);
        ListViewItemStateImageIndexConverter converter = new();

        StandardValuesCollection result = converter.GetStandardValues(context);

        result.Cast<object>().Should().BeEmpty();
    }

    [WinFormsFact]
    public void GetStandardValues_WithStateImageList_ReturnsCorrectIndexes()
    {
        using ListView listView = new();
        using ImageList imageList = new();
        using Bitmap bmp1 = new(16, 16);
        using Bitmap bmp2 = new(16, 16);
        imageList.Images.Add(bmp1);
        imageList.Images.Add(bmp2);
        listView.StateImageList = imageList;

        ListViewItem item = listView.Items.Add("test");

        ITypeDescriptorContext context = new TypeDescriptorContextStub(item);
        ListViewItemStateImageIndexConverter converter = new();

        StandardValuesCollection result = converter.GetStandardValues(context);

        result.Cast<object>().Should().Equal(0, 1);
    }

    // Minimal stub for ITypeDescriptorContext
    private sealed class TypeDescriptorContextStub : ITypeDescriptorContext
    {
        public TypeDescriptorContextStub(object instance) => Instance = instance;
        public object Instance { get; }
        public IContainer? Container => null;
        public object? GetService(Type serviceType) => null;
        public void OnComponentChanged() { }
        public bool OnComponentChanging() => false;
        public PropertyDescriptor? PropertyDescriptor => null;
    }
}
