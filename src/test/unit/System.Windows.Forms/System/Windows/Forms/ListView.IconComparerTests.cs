// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ListView_IconComparerTests
{
    [WinFormsTheory]
    [InlineData(SortOrder.Ascending)]
    [InlineData(SortOrder.Descending)]
    [InlineData(SortOrder.None)]
    public void Constructor_SetsSortOrder_Correctly(SortOrder order)
    {
        ListView.IconComparer comparer = new(order);

        ((SortOrder)comparer.TestAccessor().Dynamic._sortOrder).Should().Be(order);
    }

    [WinFormsTheory]
    [InlineData(SortOrder.Ascending, SortOrder.Descending)]
    [InlineData(SortOrder.Descending, SortOrder.Ascending)]
    [InlineData(SortOrder.None, SortOrder.Ascending)]
    public void SortOrder_Setter_UpdatesSortOrder(SortOrder initialOrder, SortOrder updatedOrder)
    {
        ListView.IconComparer comparer = new(initialOrder)
        {
            SortOrder = updatedOrder
        };

        ((SortOrder)comparer.TestAccessor().Dynamic._sortOrder).Should().Be(updatedOrder);
    }

    [WinFormsTheory]
    [InlineData(SortOrder.Ascending, "A", "B", -1)]
    [InlineData(SortOrder.Ascending, "B", "A", 1)]
    [InlineData(SortOrder.Ascending, "A", "A", 0)]
    [InlineData(SortOrder.Descending, "A", "B", 1)]
    [InlineData(SortOrder.Descending, "B", "A", -1)]
    [InlineData(SortOrder.Descending, "A", "A", 0)]
    public void Compare_ComparesListViewItems_ByText_AndSortOrder(SortOrder order, string text1, string text2, int expectedSign)
    {
        ListView.IconComparer comparer = new(order);
        ListViewItem item1 = new(text1);
        ListViewItem item2 = new(text2);
        int result = comparer.Compare(item1, item2);

        Math.Sign(result).Should().Be(expectedSign);
    }

    [WinFormsFact]
    public void Compare_NullItems_ReturnsExpected()
    {
        ListView.IconComparer comparer = new(SortOrder.Ascending);
        ListViewItem item = new("A");

        comparer.Compare(null, null).Should().Be(0);
        comparer.Compare(null, item).Should().BeLessThan(0);
        comparer.Compare(item, null).Should().BeGreaterThan(0);
    }
}
