// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Design.Tests;

public class ToolboxItemCollectionTests
{
    [Fact]
    public void ToolboxItemCollection_Ctor_ToolboxItemArray()
    {
        ToolboxItem item = new();
        ToolboxItemCollection collection = new((ToolboxItem[])[item]);
        Assert.Same(item, Assert.Single(collection));
        Assert.Same(item, collection[0]);
        Assert.True(collection.Contains(item));
        Assert.Equal(0, collection.IndexOf(item));
    }

    [Fact]
    public void ToolboxItemCollection_Ctor_NullToolboxItemArray_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("c", () => new ToolboxItemCollection((ToolboxItem[])null));
    }

    [Fact]
    public void ToolboxItemCollection_Ctor_ToolboxItemCollection()
    {
        ToolboxItem item = new();
        ToolboxItemCollection value = new((ToolboxItem[])[item]);
        ToolboxItemCollection collection = new(value);
        Assert.Same(item, Assert.Single(collection));
        Assert.Same(item, collection[0]);
        Assert.True(collection.Contains(item));
        Assert.Equal(0, collection.IndexOf(item));
    }

    [Fact]
    public void ToolboxItemCollection_Ctor_NullToolboxItemCollection_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("c", () => new ToolboxItemCollection((ToolboxItemCollection)null));
    }

    [Fact]
    public void ToolboxItemCollection_Contains_NoSuchValue_ReturnsFalse()
    {
        ToolboxItem item = new();
        ToolboxItemCollection collection = new((ToolboxItem[])[item]);
        Assert.False(collection.Contains(new ToolboxItem { DisplayName = "Other" }));
        Assert.False(collection.Contains(null));
    }

    [Fact]
    public void ToolboxItemCollection_IndexOf_NoSuchValue_ReturnsNegativeOne()
    {
        ToolboxItem item = new();
        ToolboxItemCollection collection = new((ToolboxItem[])[item]);
        Assert.Equal(-1, collection.IndexOf(new ToolboxItem { DisplayName = "Other" }));
        Assert.Equal(-1, collection.IndexOf(null));
    }

    [Fact]
    public void ToolboxItemCollection_CopyTo_Invoke_Success()
    {
        ToolboxItem item = new();
        ToolboxItemCollection collection = new((ToolboxItem[])[item]);

        var array = new ToolboxItem[3];
        collection.CopyTo(array, 1);
        Assert.Equal([null, item, null], array);
    }
}
