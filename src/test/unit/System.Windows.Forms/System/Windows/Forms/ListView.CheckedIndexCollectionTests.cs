// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class ListView_CheckedIndexCollectionTests : IDisposable
{
    private readonly ListView _listView;
    private readonly ListView.CheckedIndexCollection _collection;

    public ListView_CheckedIndexCollectionTests()
    {
        _listView = new ListView();
        _collection = new ListView.CheckedIndexCollection(_listView);
    }

    public void Dispose() => _listView.Dispose();

    [WinFormsFact]
    public void CheckedIndexCollection_Ctor_OwnerIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new ListView.CheckedIndexCollection(null!));
    }

    [WinFormsFact]
    public void Count_ReturnsZero_WhenCheckBoxesIsFalse()
    {
        _listView.CheckBoxes = false;
        _listView.Items.Add(new ListViewItem { Checked = true });

        _collection.Count.Should().Be(0);
    }

    [WinFormsFact]
    public void Count_ReturnsCheckedItemCount_WhenCheckBoxesIsTrue()
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
    public void Indexer_ReturnsCorrectIndex_ForCheckedItems()
    {
        _listView.CheckBoxes = true;
        _listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = true }
        ]);

        _collection[0].Should().Be(1);
        _collection[1].Should().Be(2);
    }

    [WinFormsFact]
    public void Indexer_ThrowsArgumentOutOfRangeException_ForInvalidIndex()
    {
        _listView.CheckBoxes = true;
        _listView.Items.Add(new ListViewItem { Checked = true });

        Action act = () => _ = _collection[1];

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [WinFormsTheory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public void Contains_ReturnsExpected(int index, bool expected)
    {
        _listView.CheckBoxes = true;
        _listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);

        _collection.Contains(index).Should().Be(expected);
    }

    [WinFormsFact]
    public void IndexOf_ReturnsIndexInCheckedCollection_IfChecked()
    {
        _listView.CheckBoxes = true;
        _listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = true }
        ]);

        _collection.IndexOf(1).Should().Be(0);
        _collection.IndexOf(2).Should().Be(1);
    }

    [WinFormsFact]
    public void IndexOf_ReturnsMinusOne_IfNotChecked()
    {
        _listView.CheckBoxes = true;
        _listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);

        _collection.IndexOf(0).Should().Be(-1);
    }

    [WinFormsFact]
    public void IsReadOnly_IsTrue()
    {
        _collection.IsReadOnly.Should().BeTrue();
    }

    [WinFormsFact]
    public void GetEnumerator_EnumeratesCheckedIndices()
    {
        _listView.CheckBoxes = true;
        _listView.Items.AddRange([
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);

        var indices = _collection.Cast<int>().ToArray();

        indices.Should().Equal(0, 2);
    }

    [WinFormsFact]
    public void CopyTo_CopiesCheckedIndices()
    {
        _listView.CheckBoxes = true;
        _listView.Items.AddRange([
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        int[] array = new int[2];

        ((ICollection)_collection).CopyTo(array, 0);

        array.Should().Equal(0, 2);
    }

    [WinFormsTheory]
    [InlineData("Add")]
    [InlineData("Clear")]
    [InlineData("Insert")]
    [InlineData("Remove")]
    [InlineData("RemoveAt")]
    public void IList_ModificationMethods_ThrowNotSupportedException(string method)
    {
        IList ilist = _collection;

        Action act = method switch
        {
            "Add" => () => ilist.Add(0),
            "Clear" => ilist.Clear,
            "Insert" => () => ilist.Insert(0, 0),
            "Remove" => () => ilist.Remove(0),
            "RemoveAt" => () => ilist.RemoveAt(0),
            _ => throw new ArgumentOutOfRangeException(nameof(method))
        };

        act.Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void IList_Indexer_Get_ReturnsCheckedIndex()
    {
        _listView.CheckBoxes = true;
        _listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);

        IList ilist = _collection;

        ilist[0].Should().Be(1);
    }

    [WinFormsFact]
    public void IList_Indexer_Set_ThrowsNotSupportedException()
    {
        IList ilist = _collection;
        Action act = () => ilist[0] = 1;

        act.Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void ICollection_SyncRoot_ReturnsSelf()
    {
        ListView.CheckedIndexCollection syncRoot = (ListView.CheckedIndexCollection)((ICollection)_collection).SyncRoot;
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

    [WinFormsTheory]
    [InlineData(null, false)]
    [InlineData("string", false)]
    [InlineData(1.5, false)]
    [InlineData(true, false)]
    public void IList_Contains_ReturnsFalse_ForNonIntOrNonCheckedIndex(object? value, bool expected)
    {
        _listView.CheckBoxes = true;
        _listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);

        IList ilist = _collection;

        ilist.Contains(value).Should().Be(expected);
    }

    [WinFormsTheory]
    [InlineData(null, -1)]
    [InlineData("string", -1)]
    [InlineData(1.5, -1)]
    [InlineData(true, -1)]
    public void IList_IndexOf_ReturnsMinusOne_ForNonIntValues(object? value, int expected)
    {
        _listView.CheckBoxes = true;
        _listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);

        IList ilist = _collection;

        ilist.IndexOf(value).Should().Be(expected);
    }

    [WinFormsFact]
    public void IList_IndexOf_ReturnsIndexInCheckedCollection_IfChecked()
    {
        _listView.CheckBoxes = true;
        _listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = true }
        ]);

        IList ilist = _collection;

        ilist.IndexOf(1).Should().Be(0);
        ilist.IndexOf(2).Should().Be(1);
    }

    [WinFormsFact]
    public void IList_IndexOf_ReturnsMinusOne_IfIntIndexIsNotChecked()
    {
        _listView.CheckBoxes = true;
        _listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);

        IList ilist = _collection;

        ilist.IndexOf(0).Should().Be(-1);
    }
}
