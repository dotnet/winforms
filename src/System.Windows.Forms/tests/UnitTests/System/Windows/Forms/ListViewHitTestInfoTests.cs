// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ListViewHitTestInfoTests
{
    public static IEnumerable<object[]> Ctor_ListViewItem_ListViewSubItem_ListViewHitTestLocations_TestData()
    {
        yield return new object[] { null, null, ListViewHitTestLocations.None };
        yield return new object[] { new ListViewItem(), new ListViewItem.ListViewSubItem(), ListViewHitTestLocations.None - 1 };
    }

    [Theory]
    [MemberData(nameof(Ctor_ListViewItem_ListViewSubItem_ListViewHitTestLocations_TestData))]
    public void ListViewHitTestInfo_Ctor_ListViewItem_ListViewSubItem_ListViewHitTestLocations(ListViewItem hitItem, ListViewItem.ListViewSubItem hitSubItem, ListViewHitTestLocations hitTestLocations)
    {
        ListViewHitTestInfo info = new(hitItem, hitSubItem, hitTestLocations);
        Assert.Equal(hitItem, info.Item);
        Assert.Equal(hitSubItem, info.SubItem);
        Assert.Equal(hitTestLocations, info.Location);
    }
}
