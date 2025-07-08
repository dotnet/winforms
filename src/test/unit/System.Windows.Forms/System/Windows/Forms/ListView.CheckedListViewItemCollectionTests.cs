// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class ListView_CheckedListViewItemCollectionTests
{
    [WinFormsFact]
    public void CheckedListViewItemCollection_Ctor_OwnerIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => { new ListView.CheckedListViewItemCollection(null!); });
    }

    [WinFormsFact]
    public void Count_ReturnsCheckedItemCount_WhenNotVirtualMode()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedListViewItemCollection collection = new(listView);

        collection.Count.Should().Be(2);
    }

    [WinFormsFact]
    public void Count_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.VirtualMode = true;
        ListView.CheckedListViewItemCollection collection = new(listView);

        Action act = () => { int _ = collection.Count; };

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void Indexer_ReturnsCheckedItem_ByIndex()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListViewItem item1 = new() { Name = "a", Checked = false };
        ListViewItem item2 = new() { Name = "b", Checked = true };
        ListViewItem item3 = new() { Name = "c", Checked = true };
        listView.Items.AddRange([item1, item2, item3]);
        ListView.CheckedListViewItemCollection collection = new(listView);

        collection[0].Should().Be(item2);
        collection[1].Should().Be(item3);
    }

    [WinFormsFact]
    public void Indexer_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.VirtualMode = true;
        ListView.CheckedListViewItemCollection collection = new(listView);

        Action act = () => { var _ = collection[0]; };

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void IList_Indexer_Get_ReturnsCheckedItem()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListViewItem item1 = new() { Checked = false };
        ListViewItem item2 = new() { Checked = true };
        listView.Items.AddRange([item1, item2]);
        ListView.CheckedListViewItemCollection collection = new(listView);
        IList ilist = collection;

        ilist[0].Should().Be(item2);
    }

    [WinFormsFact]
    public void IList_Indexer_Set_ThrowsNotSupportedException()
    {
        using ListView listView = new();
        ListView.CheckedListViewItemCollection collection = new(listView);
        IList ilist = collection;

        Action act = () => ilist[0] = new ListViewItem();

        act.Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void StringIndexer_ReturnsCheckedItem_ByKey()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListViewItem item1 = new() { Name = "foo", Checked = true };
        ListViewItem item2 = new() { Name = "bar", Checked = true };
        listView.Items.AddRange([item1, item2]);
        ListView.CheckedListViewItemCollection collection = new(listView);

        collection["foo"].Should().Be(item1);
        collection["bar"].Should().Be(item2);
    }

    [WinFormsFact]
    public void StringIndexer_ReturnsNull_ForNullOrEmptyKey()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListView.CheckedListViewItemCollection collection = new(listView);

        collection[null].Should().BeNull();
        collection[string.Empty].Should().BeNull();
    }

    [WinFormsFact]
    public void StringIndexer_ReturnsNull_IfKeyNotFound()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListViewItem item = new() { Name = "foo", Checked = true };
        listView.Items.Add(item);
        ListView.CheckedListViewItemCollection collection = new(listView);

        collection["bar"].Should().BeNull();
    }

    [WinFormsFact]
    public void StringIndexer_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.VirtualMode = true;
        ListView.CheckedListViewItemCollection collection = new(listView);

        Action act = () => { var _ = collection["foo"]; };

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void ICollection_SyncRoot_ReturnsSelf()
    {
        using ListView listView = new();
        ListView.CheckedListViewItemCollection collection = new(listView);

        ListView.CheckedListViewItemCollection syncRoot = (ListView.CheckedListViewItemCollection)((ICollection)collection).SyncRoot;

        syncRoot.Should().BeSameAs(collection);
    }

    [WinFormsFact]
    public void ICollection_IsSynchronized_IsFalse()
    {
        using ListView listView = new();
        ListView.CheckedListViewItemCollection collection = new(listView);

        bool isSynchronized = ((ICollection)collection).IsSynchronized;

        isSynchronized.Should().BeFalse();
    }

    [WinFormsFact]
    public void IList_IsFixedSize_IsTrue()
    {
        using ListView listView = new();
        ListView.CheckedListViewItemCollection collection = new(listView);

        bool isFixedSize = ((IList)collection).IsFixedSize;

        isFixedSize.Should().BeTrue();
    }

    [WinFormsFact]
    public void IsReadOnly_IsTrue()
    {
        using ListView listView = new();
        ListView.CheckedListViewItemCollection collection = new(listView);

        collection.IsReadOnly.Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]   // Checked and owned
    [InlineData(false, true, false)] // Not checked but owned
    [InlineData(true, false, false)] // Checked but not owned
    public void Contains_ReturnsExpected_BasedOnCheckedAndOwnership(bool isChecked, bool isOwned, bool expected)
    {
        using ListView listView1 = new();
        using ListView listView2 = new();
        listView1.CheckBoxes = true;
        listView2.CheckBoxes = true;

        ListViewItem item = new() { Checked = isChecked };
        if (isOwned)
        {
            listView1.Items.Add(item);
        }
        else
        {
            listView2.Items.Add(item);
        }

        ListView.CheckedListViewItemCollection collection = new(listView1);

        collection.Contains(item).Should().Be(expected);
    }

    [WinFormsFact]
    public void Contains_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        using ListView listView = new();
        listView.VirtualMode = true;
        ListView.CheckedListViewItemCollection collection = new(listView);
        ListViewItem item = new();

        Action act = () => collection.Contains(item);

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void IList_Contains_ReturnsTrue_IfListViewItemIsCheckedAndOwned()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListViewItem item = new() { Checked = true };
        listView.Items.Add(item);
        ListView.CheckedListViewItemCollection collection = new(listView);
        IList ilist = collection;

        ilist.Contains(item).Should().BeTrue();
    }

    [WinFormsFact]
    public void IList_Contains_ReturnsFalse_IfNotListViewItem()
    {
        using ListView listView = new();
        ListView.CheckedListViewItemCollection collection = new(listView);
        IList ilist = collection;

        ilist.Contains("not an item").Should().BeFalse();
    }

    [WinFormsFact]
    public void IList_Contains_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        using ListView listView = new();
        listView.VirtualMode = true;
        ListView.CheckedListViewItemCollection collection = new(listView);
        IList ilist = collection;

        Action act = () => ilist.Contains(new ListViewItem());

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsTheory]
    [InlineData("foo", true)]
    [InlineData("bar", false)]
    [InlineData(null, false)]
    [InlineData("", false)]
    public void ContainsKey_ReturnsExpected(string? key, bool expected)
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListViewItem item = new() { Name = "foo", Checked = true };
        listView.Items.Add(item);
        ListView.CheckedListViewItemCollection collection = new(listView);

        collection.ContainsKey(key).Should().Be(expected);
    }

    [WinFormsFact]
    public void ContainsKey_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        using ListView listView = new();
        listView.VirtualMode = true;
        ListView.CheckedListViewItemCollection collection = new(listView);

        Action act = () => collection.ContainsKey("foo");

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void IndexOf_ReturnsIndex_IfItemIsCheckedAndOwned()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListViewItem item1 = new() { Checked = true };
        ListViewItem item2 = new() { Checked = true };
        listView.Items.AddRange([item1, item2]);
        ListView.CheckedListViewItemCollection collection = new(listView);

        collection.IndexOf(item2).Should().Be(1);
    }

    [WinFormsFact]
    public void IndexOf_ReturnsMinusOne_IfItemIsNotChecked()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListViewItem item = new() { Checked = false };
        listView.Items.Add(item);
        ListView.CheckedListViewItemCollection collection = new(listView);

        collection.IndexOf(item).Should().Be(-1);
    }

    [WinFormsFact]
    public void IndexOf_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        using ListView listView = new();
        listView.VirtualMode = true;
        ListView.CheckedListViewItemCollection collection = new(listView);

        Action act = () => collection.IndexOf(new ListViewItem());

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void IndexOfKey_ReturnsIndex_IfKeyExists()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListViewItem item1 = new() { Name = "foo", Checked = true };
        ListViewItem item2 = new() { Name = "bar", Checked = true };
        listView.Items.AddRange([item1, item2]);
        ListView.CheckedListViewItemCollection collection = new(listView);

        collection.IndexOfKey("bar").Should().Be(1);
    }

    [WinFormsFact]
    public void IndexOfKey_ReturnsMinusOne_IfKeyIsNullOrEmpty()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListView.CheckedListViewItemCollection collection = new(listView);

        collection.IndexOfKey(null).Should().Be(-1);
        collection.IndexOfKey(string.Empty).Should().Be(-1);
    }

    [WinFormsFact]
    public void IndexOfKey_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        using ListView listView = new();
        listView.VirtualMode = true;
        ListView.CheckedListViewItemCollection collection = new(listView);

        Action act = () => collection.IndexOfKey("foo");

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void IList_IndexOf_ReturnsIndex_IfListViewItemIsCheckedAndOwned()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListViewItem item1 = new() { Checked = true };
        ListViewItem item2 = new() { Checked = true };
        listView.Items.AddRange([item1, item2]);
        ListView.CheckedListViewItemCollection collection = new(listView);
        IList ilist = collection;

        ilist.IndexOf(item2).Should().Be(1);
    }

    [WinFormsFact]
    public void IList_IndexOf_ReturnsMinusOne_IfNotListViewItem()
    {
        using ListView listView = new();
        ListView.CheckedListViewItemCollection collection = new(listView);
        IList ilist = collection;

        ilist.IndexOf("not an item").Should().Be(-1);
    }

    [WinFormsFact]
    public void IList_IndexOf_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        using ListView listView = new();
        listView.VirtualMode = true;
        ListView.CheckedListViewItemCollection collection = new(listView);
        IList ilist = collection;

        Action act = () => ilist.IndexOf(new ListViewItem());

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsTheory]
    [InlineData("Add")]
    [InlineData("Clear")]
    [InlineData("Insert")]
    [InlineData("Remove")]
    [InlineData("RemoveAt")]
    public void IList_Methods_ThrowNotSupportedException(string methodName)
    {
        using ListView listView = new();
        ListView.CheckedListViewItemCollection collection = new(listView);
        IList ilist = collection;

        Action act = methodName switch
        {
            "Add" => () => ilist.Add(new ListViewItem()),
            "Clear" => ilist.Clear,
            "Insert" => () => ilist.Insert(0, new ListViewItem()),
            "Remove" => () => ilist.Remove(new ListViewItem()),
            "RemoveAt" => () => ilist.RemoveAt(0),
            _ => throw new ArgumentException($"Invalid method name: {methodName}", nameof(methodName))
        };

        act.Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void CopyTo_CopiesCheckedItems()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListViewItem item1 = new() { Checked = true };
        ListViewItem item2 = new() { Checked = false };
        ListViewItem item3 = new() { Checked = true };
        listView.Items.AddRange([item1, item2, item3]);
        ListView.CheckedListViewItemCollection collection = new(listView);
        ListViewItem[] array = new ListViewItem[2];

        collection.CopyTo(array, 0);

        array.Should().Equal(item1, item3);
    }

    [WinFormsFact]
    public void CopyTo_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        using ListView listView = new();
        listView.VirtualMode = true;
        ListView.CheckedListViewItemCollection collection = new(listView);
        ListViewItem[] array = new ListViewItem[1];

        Action act = () => collection.CopyTo(array, 0);

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void GetEnumerator_EnumeratesCheckedItems()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        ListViewItem item1 = new() { Checked = true };
        ListViewItem item2 = new() { Checked = false };
        ListViewItem item3 = new() { Checked = true };
        listView.Items.AddRange([item1, item2, item3]);
        ListView.CheckedListViewItemCollection collection = new(listView);

        ListViewItem[] items = collection.Cast<ListViewItem>().ToArray();

        items.Should().Equal(item1, item3);
    }

    [WinFormsFact]
    public void GetEnumerator_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        using ListView listView = new();
        listView.VirtualMode = true;
        ListView.CheckedListViewItemCollection collection = new(listView);

        Action act = () => collection.GetEnumerator();

        act.Should().Throw<InvalidOperationException>();
    }
}
