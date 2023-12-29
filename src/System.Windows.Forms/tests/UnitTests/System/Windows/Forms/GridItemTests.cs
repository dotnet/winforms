// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class GridItemTests
{
    [Fact]
    public void GridItem_Expandable_Get_ReturnsFalse()
    {
        SubGridItem item = new();
        Assert.False(item.Expandable);
    }

    [Fact]
    public void GridItem_Expanded_Get_ReturnsFalse()
    {
        SubGridItem item = new();
        Assert.False(item.Expanded);
    }

    [Theory]
    [BoolData]
    public void GridItem_Expanded_Set_ThrowsNotSupportedException(bool value)
    {
        SubGridItem item = new();
        Assert.Throws<NotSupportedException>(() => item.Expanded = value);
    }

    [Theory]
    [StringWithNullData]
    public void GridItem_Tag_Set_GetReturnsExpected(object value)
    {
        SubGridItem item = new()
        {
            Tag = value
        };
        Assert.Same(value, item.Tag);

        // Set same.
        item.Tag = value;
        Assert.Same(value, item.Tag);
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
