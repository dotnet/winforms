// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class ListView_CheckedListViewItemCollectionTests : IDisposable
{
    private readonly ListView _listView;
    private readonly ListView.CheckedListViewItemCollection _collection;

    public ListView_CheckedListViewItemCollectionTests()
    {
        _listView = new ListView();
        _collection = new ListView.CheckedListViewItemCollection(_listView);
    }

    public void Dispose() => _listView.Dispose();

    [WinFormsFact]
    public void CheckedListViewItemCollection_Ctor_OwnerIsNull_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>("owner", () => new ListView.CheckedListViewItemCollection(null!));

    [WinFormsFact]
    public void Count_ReturnsCheckedItemCount_WhenNotVirtualMode()
    {
        _listView.CheckBoxes = true;
        _listView.Items.AddRange([
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);

        _collection.Count.Should().Be(2);
    }

    [WinFormsFact]
    public void Count_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        _listView.CheckBoxes = true;
        _listView.VirtualMode = true;

        Action act = () => { int _ = _collection.Count; };

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void Indexer_ReturnsCheckedItem_ByIndex()
    {
        _listView.CheckBoxes = true;
        ListViewItem item1 = new() { Name = "a", Checked = false };
        ListViewItem item2 = new() { Name = "b", Checked = true };
        ListViewItem item3 = new() { Name = "c", Checked = true };
        _listView.Items.AddRange([item1, item2, item3]);

        _collection[0].Should().Be(item2);
        _collection[1].Should().Be(item3);
    }

    [WinFormsFact]
    public void Indexer_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        _listView.CheckBoxes = true;
        _listView.VirtualMode = true;

        Action act = () => { var _ = _collection[0]; };

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void IList_Indexer_Get_ReturnsCheckedItem()
    {
        _listView.CheckBoxes = true;
        ListViewItem item1 = new() { Checked = false };
        ListViewItem item2 = new() { Checked = true };
        _listView.Items.AddRange([item1, item2]);
        IList iList = _collection;

        iList[0].Should().Be(item2);
    }

    [WinFormsFact]
    public void IList_Indexer_Set_ThrowsNotSupportedException()
    {
        IList iList = _collection;

        Action act = () => iList[0] = new ListViewItem();

        act.Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void StringIndexer_ReturnsCheckedItem_ByKey()
    {
        _listView.CheckBoxes = true;
        ListViewItem item1 = new() { Name = "foo", Checked = true };
        ListViewItem item2 = new() { Name = "bar", Checked = true };
        _listView.Items.AddRange([item1, item2]);

        _collection["foo"].Should().Be(item1);
        _collection["bar"].Should().Be(item2);
    }

    [WinFormsFact]
    public void StringIndexer_ReturnsNull_ForNullOrEmptyKey()
    {
        _listView.CheckBoxes = true;

        _collection[null].Should().BeNull();
        _collection[string.Empty].Should().BeNull();
    }

    [WinFormsFact]
    public void StringIndexer_ReturnsNull_IfKeyNotFound()
    {
        _listView.CheckBoxes = true;
        ListViewItem item = new() { Name = "foo", Checked = true };
        _listView.Items.Add(item);

        _collection["bar"].Should().BeNull();
    }

    [WinFormsFact]
    public void StringIndexer_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        _listView.CheckBoxes = true;
        _listView.VirtualMode = true;

        Action act = () => { var _ = _collection["foo"]; };

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void ICollection_SyncRoot_ReturnsSelf()
    {
        ListView.CheckedListViewItemCollection syncRoot = (ListView.CheckedListViewItemCollection)((ICollection)_collection).SyncRoot;

        syncRoot.Should().BeSameAs(_collection);
    }

    [WinFormsFact]
    public void ICollection_IsSynchronized_IsFalse()
    {
        bool isSynchronized = ((ICollection)_collection).IsSynchronized;

        isSynchronized.Should().BeFalse();
    }

    [WinFormsFact]
    public void IList_IsFixedSize_IsTrue()
    {
        bool isFixedSize = ((IList)_collection).IsFixedSize;

        isFixedSize.Should().BeTrue();
    }

    [WinFormsFact]
    public void IsReadOnly_IsTrue()
    {
        _collection.IsReadOnly.Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]   // Checked and owned
    [InlineData(false, true, false)] // Not checked but owned
    [InlineData(true, false, false)] // Checked but not owned
    public void Contains_ReturnsExpected_BasedOnCheckedAndOwnership(bool isChecked, bool isOwned, bool expected)
    {
        using ListView listView2 = new();
        _listView.CheckBoxes = true;
        listView2.CheckBoxes = true;

        ListViewItem item = new() { Checked = isChecked };
        if (isOwned)
        {
            _listView.Items.Add(item);
        }
        else
        {
            listView2.Items.Add(item);
        }

        _collection.Contains(item).Should().Be(expected);
    }

    [WinFormsFact]
    public void Contains_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        _listView.VirtualMode = true;
        ListViewItem item = new();

        Action act = () => _collection.Contains(item);

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void IList_Contains_ReturnsTrue_IfListViewItemIsCheckedAndOwned()
    {
        _listView.CheckBoxes = true;
        ListViewItem item = new() { Checked = true };
        _listView.Items.Add(item);
        IList iList = _collection;

        iList.Contains(item).Should().BeTrue();
    }

    [WinFormsFact]
    public void IList_Contains_ReturnsFalse_IfNotListViewItem()
    {
        IList iList = _collection;

        iList.Contains("not an item").Should().BeFalse();
    }

    [WinFormsFact]
    public void IList_Contains_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        _listView.VirtualMode = true;
        IList iList = _collection;

        Action act = () => iList.Contains(new ListViewItem());

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsTheory]
    [InlineData("foo", true)]
    [InlineData("bar", false)]
    [InlineData(null, false)]
    [InlineData("", false)]
    public void ContainsKey_ReturnsExpected(string? key, bool expected)
    {
        _listView.CheckBoxes = true;
        ListViewItem item = new() { Name = "foo", Checked = true };
        _listView.Items.Add(item);

        _collection.ContainsKey(key).Should().Be(expected);
    }

    [WinFormsFact]
    public void ContainsKey_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        _listView.VirtualMode = true;

        Action act = () => _collection.ContainsKey("foo");

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void IndexOf_ReturnsIndex_IfItemIsCheckedAndOwned()
    {
        _listView.CheckBoxes = true;
        ListViewItem item1 = new() { Checked = true };
        ListViewItem item2 = new() { Checked = true };
        _listView.Items.AddRange([item1, item2]);

        _collection.IndexOf(item2).Should().Be(1);
    }

    [WinFormsFact]
    public void IndexOf_ReturnsMinusOne_IfItemIsNotChecked()
    {
        _listView.CheckBoxes = true;
        ListViewItem item = new() { Checked = false };
        _listView.Items.Add(item);

        _collection.IndexOf(item).Should().Be(-1);
    }

    [WinFormsFact]
    public void IndexOf_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        _listView.VirtualMode = true;

        Action act = () => _collection.IndexOf(new ListViewItem());

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void IndexOfKey_ReturnsIndex_IfKeyExists()
    {
        _listView.CheckBoxes = true;
        ListViewItem item1 = new() { Name = "foo", Checked = true };
        ListViewItem item2 = new() { Name = "bar", Checked = true };
        _listView.Items.AddRange([item1, item2]);

        _collection.IndexOfKey("bar").Should().Be(1);
    }

    [WinFormsFact]
    public void IndexOfKey_ReturnsMinusOne_IfKeyIsNullOrEmpty()
    {
        _listView.CheckBoxes = true;

        _collection.IndexOfKey(null).Should().Be(-1);
        _collection.IndexOfKey(string.Empty).Should().Be(-1);
    }

    [WinFormsFact]
    public void IndexOfKey_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        _listView.VirtualMode = true;

        Action act = () => _collection.IndexOfKey("foo");

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void IList_IndexOf_ReturnsIndex_IfListViewItemIsCheckedAndOwned()
    {
        _listView.CheckBoxes = true;
        ListViewItem item1 = new() { Checked = true };
        ListViewItem item2 = new() { Checked = true };
        _listView.Items.AddRange([item1, item2]);
        IList iList = _collection;

        iList.IndexOf(item2).Should().Be(1);
    }

    [WinFormsFact]
    public void IList_IndexOf_ReturnsMinusOne_IfNotListViewItem()
    {
        IList iList = _collection;

        iList.IndexOf("not an item").Should().Be(-1);
    }

    [WinFormsFact]
    public void IList_IndexOf_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        _listView.VirtualMode = true;
        IList iList = _collection;

        Action act = () => iList.IndexOf(new ListViewItem());

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
        IList iList = _collection;

        Action act = methodName switch
        {
            "Add" => () => iList.Add(new ListViewItem()),
            "Clear" => iList.Clear,
            "Insert" => () => iList.Insert(0, new ListViewItem()),
            "Remove" => () => iList.Remove(new ListViewItem()),
            "RemoveAt" => () => iList.RemoveAt(0),
            _ => throw new ArgumentException($"Invalid method name: {methodName}", nameof(methodName))
        };

        act.Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void CopyTo_CopiesCheckedItems()
    {
        _listView.CheckBoxes = true;
        ListViewItem item1 = new() { Checked = true };
        ListViewItem item2 = new() { Checked = false };
        ListViewItem item3 = new() { Checked = true };
        _listView.Items.AddRange([item1, item2, item3]);
        ListViewItem[] array = new ListViewItem[2];

        _collection.CopyTo(array, 0);

        array.Should().Equal(item1, item3);
    }

    [WinFormsFact]
    public void CopyTo_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        _listView.VirtualMode = true;
        ListViewItem[] array = new ListViewItem[1];

        Action act = () => _collection.CopyTo(array, 0);

        act.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void GetEnumerator_EnumeratesCheckedItems()
    {
        _listView.CheckBoxes = true;
        ListViewItem item1 = new() { Checked = true };
        ListViewItem item2 = new() { Checked = false };
        ListViewItem item3 = new() { Checked = true };
        _listView.Items.AddRange([item1, item2, item3]);

        ListViewItem[] items = _collection.Cast<ListViewItem>().ToArray();

        items.Should().Equal(item1, item3);
    }

    [WinFormsFact]
    public void GetEnumerator_ThrowsInvalidOperationException_WhenVirtualMode()
    {
        _listView.VirtualMode = true;

        Action act = () => _collection.GetEnumerator();

        act.Should().Throw<InvalidOperationException>();
    }
}
