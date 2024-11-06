// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class GridItemCollectionTests
{
    [Fact]
    public void GridItemCollection_Empty_Get_ReturnsExpected()
    {
        GridItemCollection collection = GridItemCollection.Empty;
        Assert.Same(collection, GridItemCollection.Empty);
        Assert.Empty(collection);
    }

    [Fact]
    public void GridItemCollection_ICollection_GetProperties_ReturnsExpected()
    {
        ICollection collection = GridItemCollection.Empty;
        Assert.Empty(collection);
        Assert.False(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void GridItemCollection_CopyTo_Empty_Nop(int index)
    {
        ICollection collection = GridItemCollection.Empty;
        object[] array = [1, 2, 3];
        collection.CopyTo(array, index);
        Assert.Equal([1, 2, 3], array);
    }

    [Fact]
    public void GridItemCollection_GetEnumerator_Empty_ReturnsExpected()
    {
        GridItemCollection collection = GridItemCollection.Empty;
        IEnumerator enumerator = collection.GetEnumerator();
        Assert.False(enumerator.MoveNext());
    }

    [Theory]
    [StringWithNullData]
    public void GridItemCollection_Item_GetEmpty_ReturnsNull(string label)
    {
        GridItemCollection collection = GridItemCollection.Empty;
        Assert.Null(collection[label]);
    }

    private class SubGridItem : GridItem
    {
        public override GridItemCollection GridItems => GridItemCollection.Empty;

        public override GridItemType GridItemType => GridItemType.Property;

        public override string Label => "label";

        public override GridItem Parent => null;

        public override PropertyDescriptor PropertyDescriptor => null;

        public override object Value => "value";

        public override bool Select() => true;
    }
}
