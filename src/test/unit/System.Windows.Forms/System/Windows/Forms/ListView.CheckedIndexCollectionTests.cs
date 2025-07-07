// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class ListView_CheckedIndexCollectionTests
{
    [WinFormsFact]
    public void CheckedIndexCollection_Ctor_OwnerIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new ListView.CheckedIndexCollection(null!));
    }

    [WinFormsFact]
    public void Count_ReturnsZero_WhenCheckBoxesIsFalse()
    {
        using ListView listView = new();
        listView.CheckBoxes = false;
        listView.Items.Add(new ListViewItem { Checked = true });
        ListView.CheckedIndexCollection collection = new(listView);

        collection.Count.Should().Be(0);
    }

    [WinFormsFact]
    public void Count_ReturnsCheckedItemCount_WhenCheckBoxesIsTrue()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        collection.Count.Should().Be(2);
    }

    [WinFormsFact]
    public void Indexer_ReturnsCorrectIndex_ForCheckedItems()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        collection[0].Should().Be(1);
        collection[1].Should().Be(2);
    }

    [WinFormsFact]
    public void Indexer_ThrowsArgumentOutOfRangeException_ForInvalidIndex()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.Add(new ListViewItem { Checked = true });
        ListView.CheckedIndexCollection collection = new(listView);

        Action act = () => _ = collection[1];
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [WinFormsFact]
    public void Contains_ReturnsTrue_IfIndexIsChecked()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        collection.Contains(1).Should().BeTrue();
    }

    [WinFormsFact]
    public void Contains_ReturnsFalse_IfIndexIsNotChecked()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        collection.Contains(0).Should().BeFalse();
    }

    [WinFormsFact]
    public void IndexOf_ReturnsIndexInCheckedCollection_IfChecked()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        collection.IndexOf(1).Should().Be(0);
        collection.IndexOf(2).Should().Be(1);
    }

    [WinFormsFact]
    public void IndexOf_ReturnsMinusOne_IfNotChecked()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        collection.IndexOf(0).Should().Be(-1);
    }

    [WinFormsFact]
    public void IsReadOnly_IsTrue()
    {
        using ListView listView = new();
        ListView.CheckedIndexCollection collection = new(listView);

        collection.IsReadOnly.Should().BeTrue();
    }

    [WinFormsFact]
    public void GetEnumerator_EnumeratesCheckedIndices()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        var indices = collection.Cast<int>().ToArray();

        indices.Should().Equal(0, 2);
    }

    [WinFormsFact]
    public void CopyTo_CopiesCheckedIndices()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);
        int[] array = new int[2];

        ((ICollection)collection).CopyTo(array, 0);

        array.Should().Equal(0, 2);
    }

    [WinFormsFact]
    public void IList_Add_ThrowsNotSupportedException()
    {
        using ListView listView = new();
        ListView.CheckedIndexCollection collection = new(listView);

        IList ilist = collection;

        ilist.Invoking(i => i.Add(0)).Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void IList_Clear_ThrowsNotSupportedException()
    {
        using ListView listView = new();
        ListView.CheckedIndexCollection collection = new(listView);

        IList ilist = collection;

        ilist.Invoking(i => i.Clear()).Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void IList_Insert_ThrowsNotSupportedException()
    {
        using ListView listView = new();
        ListView.CheckedIndexCollection collection = new(listView);

        IList ilist = collection;

        ilist.Invoking(i => i.Insert(0, 0)).Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void IList_Remove_ThrowsNotSupportedException()
    {
        using ListView listView = new();
        ListView.CheckedIndexCollection collection = new(listView);

        IList ilist = collection;

        ilist.Invoking(i => i.Remove(0)).Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void IList_RemoveAt_ThrowsNotSupportedException()
    {
        using ListView listView = new();
        ListView.CheckedIndexCollection collection = new(listView);

        IList ilist = collection;

        ilist.Invoking(i => i.RemoveAt(0)).Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void IList_Indexer_Get_ReturnsCheckedIndex()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        IList ilist = collection;
        ilist[0].Should().Be(1);
    }

    [WinFormsFact]
    public void IList_Indexer_Set_ThrowsNotSupportedException()
    {
        using ListView listView = new();
        ListView.CheckedIndexCollection collection = new(listView);

        IList ilist = collection;
        Action act = () => ilist[0] = 1;

        act.Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void ICollection_SyncRoot_ReturnsSelf()
    {
        using ListView listView = new();
        ListView.CheckedIndexCollection collection = new(listView);

        ListView.CheckedIndexCollection syncRoot = (ListView.CheckedIndexCollection)((ICollection)collection).SyncRoot;
        syncRoot.Should().BeSameAs(collection);
    }

    [WinFormsFact]
    public void ICollection_IsSynchronized_IsFalse()
    {
        using ListView listView = new();
        ListView.CheckedIndexCollection collection = new(listView);

        bool isSynchronized = ((ICollection)collection).IsSynchronized;
        isSynchronized.Should().BeFalse();
    }

    [WinFormsFact]
    public void IList_IsFixedSize_IsTrue()
    {
        using ListView listView = new();
        ListView.CheckedIndexCollection collection = new(listView);

        bool isFixedSize = ((IList)collection).IsFixedSize;
        isFixedSize.Should().BeTrue();
    }

    [WinFormsFact]
    public void ICollection_CopyTo_CopiesCheckedIndicesToArray()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);
        int[] array = new int[2];

        ((ICollection)collection).CopyTo(array, 0);

        array.Should().Equal(0, 2);
    }

    [WinFormsTheory]
    [InlineData(null, false)]
    [InlineData("string", false)]
    [InlineData(1.5, false)]
    [InlineData(true, false)]
    public void IList_Contains_ReturnsFalse_ForNonIntOrNonCheckedIndex(object? value, bool expected)
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        IList ilist = collection;

        ilist.Contains(value).Should().Be(expected);
    }

    [WinFormsFact]
    public void IList_Contains_ReturnsTrue_IfIntIndexIsChecked()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        IList ilist = collection;

        ilist.Contains(1).Should().BeTrue();
    }

    [WinFormsFact]
    public void IList_Contains_ReturnsFalse_IfIntIndexIsNotChecked()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        IList ilist = collection;

        ilist.Contains(0).Should().BeFalse();
    }

    [WinFormsTheory]
    [InlineData(null, -1)]
    [InlineData("string", -1)]
    [InlineData(1.5, -1)]
    [InlineData(true, -1)]
    public void IList_IndexOf_ReturnsMinusOne_ForNonIntValues(object? value, int expected)
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        IList ilist = collection;

        ilist.IndexOf(value).Should().Be(expected);
    }

    [WinFormsFact]
    public void IList_IndexOf_ReturnsIndexInCheckedCollection_IfChecked()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        IList ilist = collection;

        ilist.IndexOf(1).Should().Be(0);
        ilist.IndexOf(2).Should().Be(1);
    }

    [WinFormsFact]
    public void IList_IndexOf_ReturnsMinusOne_IfIntIndexIsNotChecked()
    {
        using ListView listView = new();
        listView.CheckBoxes = true;
        listView.Items.AddRange([
            new ListViewItem { Checked = false },
            new ListViewItem { Checked = true }
        ]);
        ListView.CheckedIndexCollection collection = new(listView);

        IList ilist = collection;

        ilist.IndexOf(0).Should().Be(-1);
    }
}
