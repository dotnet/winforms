// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class PropertyValueChangedEventArgsTests
{
    public static IEnumerable<object[]> Ctor_GridItem_Object_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new SubGridItem(), 1 };
    }

    [Theory]
    [MemberData(nameof(Ctor_GridItem_Object_TestData))]
    public void Ctor_GridItem_Object(GridItem changedItem, object oldValue)
    {
        PropertyValueChangedEventArgs e = new(changedItem, oldValue);
        Assert.Equal(changedItem, e.ChangedItem);
        Assert.Equal(oldValue, e.OldValue);
    }

    private class SubGridItem : GridItem
    {
        public override GridItemCollection GridItems { get; }

        public override GridItemType GridItemType { get; }

        public override string Label { get; }

        public override GridItem Parent { get; }

        public override PropertyDescriptor PropertyDescriptor { get; }

        public override object Value { get; }

        public override bool Select() => false;
    }
}
