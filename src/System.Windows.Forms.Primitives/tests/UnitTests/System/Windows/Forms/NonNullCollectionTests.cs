// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class NonNullCollectionTests
{
    [Fact]
    public void NonNullCollection_Constructor_ThrowsWithNull()
    {
        Assert.Throws<ArgumentNullException>("items", () => new TestCollection(null!));
    }

    [Fact]
    public void NonNullCollection_Constructor_ThrowsWithNullInCollection()
    {
        Assert.Throws<ArgumentNullException>("items", () => new TestCollection(new object[] { null! }));
    }

    [Fact]
    public void NonNullCollection_Add_ThrowsWithNull()
    {
        TestCollection collection = new();
        Assert.Throws<ArgumentNullException>("item", () => collection.Add(null!));
    }

    [Fact]
    public void NonNullCollection_Add_CallsItemAdded()
    {
        TestCollection collection = new();
        object item = new();
        collection.Add(item);
        Assert.Same(item, collection.LastAdded);
        Assert.Equal(1, collection.AddCount);
    }

    [Fact]
    public void NonNullCollection_IListAdd_ThrowsWithNull()
    {
        TestCollection collection = new();
        Assert.Throws<ArgumentNullException>("value", () => ((IList)collection).Add(null));
    }

    [Fact]
    public void NonNullCollection_IListAdd_CallsItemAdded()
    {
        TestCollection collection = new();
        object item = new();
        ((IList)collection).Add(item);
        Assert.Same(item, collection.LastAdded);
        Assert.Equal(1, collection.AddCount);
    }

    [Fact]
    public void NonNullCollection_Indexer_ThrowsWithNull()
    {
        TestCollection collection = new() { new() };
        Assert.Throws<ArgumentNullException>("value", () => collection[0] = null!);
    }

    [Fact]
    public void NonNullCollection_Indexer_CallsItemAdded()
    {
        TestCollection collection = new(new object[] { new() });
        object item = new();
        collection[0] = item;
        Assert.Same(item, collection.LastAdded);
        Assert.Equal(2, collection.AddCount);
    }

    [Fact]
    public void NonNullCollection_IListIndexer_ThrowsWithNull()
    {
        TestCollection collection = new() { new() };
        Assert.Throws<ArgumentNullException>("value", () => ((IList)collection)[0] = null);
    }

    [Fact]
    public void NonNullCollection_IListIndexer_CallsItemAdded()
    {
        TestCollection collection = new(new object[] { new() });
        object item = new();
        ((IList)collection)[0] = item;
        Assert.Same(item, collection.LastAdded);
        Assert.Equal(2, collection.AddCount);
    }

    [Fact]
    public void NonNullCollection_Insert_ThrowsWithNull()
    {
        TestCollection collection = new();
        Assert.Throws<ArgumentNullException>("item", () => collection.Insert(0, null!));
    }

    [Fact]
    public void NonNullCollection_Insert_CallsItemAdded()
    {
        TestCollection collection = new();
        object item = new();
        collection.Insert(0, item);
        Assert.Same(item, collection.LastAdded);
        Assert.Equal(1, collection.AddCount);
    }

    [Fact]
    public void NonNullCollection_IListInsert_ThrowsWithNull()
    {
        TestCollection collection = new();
        Assert.Throws<ArgumentNullException>("value", () => ((IList)collection).Insert(0, null));
    }

    [Fact]
    public void NonNullCollection_IListInsert_CallsItemAdded()
    {
        TestCollection collection = new();
        object item = new();
        ((IList)collection).Insert(0, item);
        Assert.Same(item, collection.LastAdded);
        Assert.Equal(1, collection.AddCount);
    }

    [Fact]
    public void NonNullCollection_AddRange_ThrowsWithNull()
    {
        TestCollection collection = new();
        Assert.Throws<ArgumentNullException>("items", () => collection.AddRange(null!));
    }

    [Fact]
    public void NonNullCollection_AddRange_ThrowsWithNullInCollection()
    {
        TestCollection collection = new();
        Assert.Throws<ArgumentNullException>("items", () => collection.AddRange(new object[] { null! }));
    }

    [Fact]
    public void NonNullCollection_AddRange_CallsItemAdded()
    {
        TestCollection collection = new();
        object item = new();
        collection.AddRange(new object[] { item });
        Assert.Same(item, collection.LastAdded);
        Assert.Equal(1, collection.AddCount);

        collection.AddRange(new object[] { new(), new(), new() });
        Assert.Equal(4, collection.AddCount);
    }

    private class TestCollection : NonNullCollection<object>
    {
        public object? LastAdded { get; set; }

        public int AddCount { get; set; }

        public TestCollection()
        {
        }

        public TestCollection(IEnumerable<object> items) : base(items)
        {
        }

        protected override void ItemAdded(object item)
        {
            LastAdded = item;
            AddCount++;
            base.ItemAdded(item);
        }
    }
}
