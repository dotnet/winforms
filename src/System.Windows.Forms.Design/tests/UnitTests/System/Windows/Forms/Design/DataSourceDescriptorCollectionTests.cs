// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class DataSourceDescriptorCollectionTests
{
    [Fact]
    public void Add_ShouldAddItem()
    {
        using MockDataSourceDescriptor descriptor = new();
        DataSourceDescriptorCollection collection = new();

        int index = collection.Add(descriptor);

        index.Should().Be(0);
        collection.Count.Should().Be(1);
        collection[0].Should().Be(descriptor);
    }

    [Fact]
    public void IndexOf_ShouldReturnCorrectIndex()
    {
        using MockDataSourceDescriptor descriptor = new();
        DataSourceDescriptorCollection collection = new();

        collection.Add(descriptor);
        int index = collection.IndexOf(descriptor);

        index.Should().Be(0);
    }

    [Fact]
    public void Insert_ShouldInsertItemAtIndex()
    {
        using MockDataSourceDescriptor descriptor1 = new();
        using MockDataSourceDescriptor descriptor2 = new();
        DataSourceDescriptorCollection collection = new();

        collection.Add(descriptor1);
        collection.Insert(0, descriptor2);

        collection[0].Should().Be(descriptor2);
        collection[1].Should().Be(descriptor1);
    }

    [Fact]
    public void Contains_ShouldReturnTrueIfItemExists()
    {
        using MockDataSourceDescriptor descriptor = new();
        DataSourceDescriptorCollection collection = new();

        collection.Add(descriptor);
        bool contains = collection.Contains(descriptor);

        contains.Should().BeTrue();
    }

    [Fact]
    public void CopyTo_ShouldCopyItemsToArray()
    {
        using MockDataSourceDescriptor descriptor1 = new();
        using MockDataSourceDescriptor descriptor2 = new();
        DataSourceDescriptorCollection collection = new();

        collection.Add(descriptor1);
        collection.Add(descriptor2);

        var array = new DataSourceDescriptor[2];
        collection.CopyTo(array, 0);

        array[0].Should().Be(descriptor1);
        array[1].Should().Be(descriptor2);
    }

    [Fact]
    public void Remove_ShouldRemoveItem()
    {
        using MockDataSourceDescriptor descriptor = new();
        DataSourceDescriptorCollection collection = new();

        collection.Add(descriptor);
        collection.Remove(descriptor);

        collection.Count.Should().Be(0);
        collection.Contains(descriptor).Should().BeFalse();
    }

    [Fact]
    public void Indexer_ShouldGetAndSetItem()
    {
        using MockDataSourceDescriptor descriptor1 = new();
        using MockDataSourceDescriptor descriptor2 = new();
        DataSourceDescriptorCollection collection = new();

        collection.Add(descriptor1);
        collection[0] = descriptor2;

        collection[0].Should().Be(descriptor2);
    }

    private class MockDataSourceDescriptor : DataSourceDescriptor, IDisposable
    {
        public override string Name => "Mock";
        public override Bitmap Image => new(1, 1);
        public override string TypeName => "MockType";
        public override bool IsDesignable => true;
        public void Dispose() => Image.Dispose();
    }
}
