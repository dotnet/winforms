// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class DataSourceGroupCollectionTests
{
    private DataSourceGroupCollection CreateCollection() => new();
    private MockDataSourceGroup CreateMockGroup() => new();

    [Fact]
    public void Add_ShouldAddItem()
    {
        DataSourceGroupCollection collection = CreateCollection();
        MockDataSourceGroup group = CreateMockGroup();

        collection.Add(group);

        collection.Count.Should().Be(1);
        collection[0].Should().Be(group);
    }

    [Fact]
    public void IndexOf_ShouldReturnCorrectIndex()
    {
        DataSourceGroupCollection collection = CreateCollection();
        MockDataSourceGroup group = CreateMockGroup();
        collection.Add(group);

        int index = collection.IndexOf(group);

        index.Should().Be(0);
    }

    [Fact]
    public void Insert_ShouldInsertItemAtIndex()
    {
        DataSourceGroupCollection collection = CreateCollection();
        MockDataSourceGroup group1 = CreateMockGroup();
        MockDataSourceGroup group2 = CreateMockGroup();
        collection.Add(group1);

        collection.Insert(0, group2);

        collection[0].Should().Be(group2);
        collection[1].Should().Be(group1);
    }

    [Fact]
    public void Contains_ShouldReturnTrueIfItemExists()
    {
        DataSourceGroupCollection collection = CreateCollection();
        MockDataSourceGroup group = CreateMockGroup();
        collection.Add(group);

        collection.Contains(group).Should().BeTrue();
    }

    [Fact]
    public void CopyTo_ShouldCopyItemsToArray()
    {
        DataSourceGroupCollection collection = CreateCollection();
        MockDataSourceGroup group = CreateMockGroup();
        collection.Add(group);
        var array = new DataSourceGroup[1];

        collection.CopyTo(array, 0);

        array[0].Should().Be(group);
    }

    [Fact]
    public void Remove_ShouldRemoveItem()
    {
        DataSourceGroupCollection collection = CreateCollection();
        MockDataSourceGroup group = CreateMockGroup();
        collection.Add(group);

        collection.Remove(group);

        collection.Count.Should().Be(0);
    }

    [Fact]
    public void Indexer_ShouldGetAndSetItem()
    {
        DataSourceGroupCollection collection = CreateCollection();
        MockDataSourceGroup group1 = CreateMockGroup();
        MockDataSourceGroup group2 = CreateMockGroup();
        collection.Add(group1);

        collection[0] = group2;

        collection[0].Should().Be(group2);
    }

    private class MockDataSourceGroup : DataSourceGroup
    {
        public override string Name => "Mock";
        public override Bitmap Image => new(1, 1);
        public override DataSourceDescriptorCollection DataSources { get; } = new();
        public override bool IsDefault => false;
    }
}
