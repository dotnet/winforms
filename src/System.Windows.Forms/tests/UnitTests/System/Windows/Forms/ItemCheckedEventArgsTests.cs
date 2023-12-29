// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ItemCheckedEventArgsTests
{
    [WinFormsFact]
    public void ItemCheckedEventArgs_Ctor_NullListViewItem_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ItemCheckedEventArgs(null));
    }

    [WinFormsFact]
    public void Ctor_ListViewItem()
    {
        ListViewItem listViewItem = new();
        ItemCheckedEventArgs e = new(listViewItem);
        Assert.Equal(listViewItem, e.Item);
    }
}
