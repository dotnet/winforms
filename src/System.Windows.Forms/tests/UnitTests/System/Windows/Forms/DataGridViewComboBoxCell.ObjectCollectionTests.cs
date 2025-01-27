// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Tests;

public class DataGridViewComboBoxCell_ObjectCollectionTests : IDisposable
{
    private readonly DataGridViewComboBoxCell _comboBoxCell;
    private readonly DataGridViewComboBoxCell.ObjectCollection _collection;

    public void Dispose() => _comboBoxCell.Dispose();

    public DataGridViewComboBoxCell_ObjectCollectionTests()
    {
        _comboBoxCell = new();
        _collection = new(_comboBoxCell);
    }

    [WinFormsFact]
    public void ObjectCollection_Add_Remove_Operations()
    {
        _collection.Count.Should().Be(0);

        int index1 = _collection.Add("Item1");
        int index2 = _collection.Add("Item2");
        _collection.Count.Should().Be(2);
        index1.Should().Be(0);
        index2.Should().Be(1);

        _collection.Remove("Item1");
        _collection.Count.Should().Be(1);
        _collection[0].Should().Be("Item2");

        _collection.Add("Item1");
        _collection.Remove("Item2");
        _collection.Count.Should().Be(1);
        _collection[0].Should().Be("Item1");

        _collection.Clear();
        _collection.Count.Should().Be(0);
    }

    [WinFormsFact]
    public void ObjectCollection_IsReadOnly_ReturnsCorrectValue() =>
        _collection.IsReadOnly.Should().BeFalse();

    [WinFormsFact]
    public void ObjectCollection_Add_AddsItemInSortedOrder()
    {
        _comboBoxCell.Sorted = true;
        var item1 = "B";
        var item2 = "A";

        _collection.Add(item1);
        int index = _collection.Add(item2);

        index.Should().Be(0);
        _collection.Count.Should().Be(2);
        _collection[0].Should().Be(item2);
        _collection[1].Should().Be(item1);
    }

    [WinFormsFact]
    public void ObjectCollection_Add_ThrowsException_WhenItemIsNull()
    {
        Action action = () => _collection.Add(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [WinFormsFact]
    public void ObjectCollection_AddRange_AddsItemsCorrectly()
    {
        var items = new object[] { "Item1", "Item2", "Item3" };

        _collection.AddRange(items);

        _collection.Count.Should().Be(3);
        _collection[0].Should().Be("Item1");
        _collection[1].Should().Be("Item2");
        _collection[2].Should().Be("Item3");
    }

    [WinFormsFact]
    public void ObjectCollection_AddRange_AddsItemsInSortedOrder()
    {
        _comboBoxCell.Sorted = true;
        var items = new object[] { "B", "A", "C" };

        _collection.AddRange(items);

        _collection.Count.Should().Be(3);
        _collection[0].Should().Be("A");
        _collection[1].Should().Be("B");
        _collection[2].Should().Be("C");
    }

    [WinFormsFact]
    public void ObjectCollection_AddRange_AddsObjectCollectionCorrectly()
    {
        DataGridViewComboBoxCell.ObjectCollection items = new(_comboBoxCell) { "Item1", "Item2", "Item3" };

        _collection.AddRange(items);

        _collection.InnerArray.Count.Should().Be(3);
        _collection[0].Should().Be("Item1");
        _collection[1].Should().Be("Item2");
        _collection[2].Should().Be("Item3");
    }

    [WinFormsFact]
    public void ObjectCollection_AddRange_DoesNotAddItems_WhenExceptionIsThrown()
    {
        var items = new object[] { "Item1", null!, "Item3" };

        Action action = () => _collection.AddRange(items);

        action.Should().Throw<InvalidOperationException>();
        _collection.Count.Should().Be(0);
    }

    [WinFormsFact]
    public void ObjectCollection_Indexer_Get_ReturnsCorrectItem()
    {
        var item1 = "Item1";
        var item2 = "Item2";
        _collection.Add(item1);
        _collection.Add(item2);

        _collection[0].Should().Be(item1);
        _collection[1].Should().Be(item2);
    }

    [WinFormsFact]
    public void ObjectCollection_Indexer_Set_SetsItemCorrectly()
    {
        var item1 = "Item1";
        var item2 = "Item2";
        _collection.Add(item1);

        _collection[0] = item2;

        _collection[0].Should().Be(item2);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ObjectCollection_Indexer_Set_ThrowsException_WhenIndexIsInvalid(int index)
    {
        Action action = () => _collection[index] = "Item";

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [WinFormsFact]
    public void ObjectCollection_Indexer_Set_ThrowsException_WhenValueIsNull()
    {
        _collection.Add("Item");

        Action action = () => _collection[0] = null!;

        action.Should().Throw<ArgumentNullException>();
    }

    [WinFormsTheory]
    [BoolData]
    public void ObjectCollection_Clear_RemovesAllItems(bool sorted)
    {
        _comboBoxCell.Sorted = sorted;
        _collection.Add("Item1");
        _collection.Add("Item2");

        _collection.Clear();

        _collection.Count.Should().Be(0);
    }

    [WinFormsTheory]
    [BoolData]
    public void ObjectCollection_Contains_ReturnsCorrectValue(bool sorted)
    {
        _comboBoxCell.Sorted = sorted;
        var item = "Item1";
        _collection.Add(item);

        _collection.Contains(item).Should().BeTrue();
        _collection.Contains("NonExistentItem").Should().BeFalse();
    }

    [WinFormsFact]
    public void ObjectCollection_Contains_ThrowsException_WhenItemIsNull()
    {
        Action action = () => _collection.Contains(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [WinFormsFact]
    public void ObjectCollection_CopyTo_CopiesItemsCorrectly()
    {
        var items = new object[] { "Item1", "Item2", "Item3" };
        _collection.AddRange(items);
        var destination = new object[3];

        _collection.CopyTo(destination, 0);

        destination[0].Should().Be("Item1");
        destination[1].Should().Be("Item2");
        destination[2].Should().Be("Item3");
    }

    [WinFormsFact]
    public void ObjectCollection_CopyTo_ThrowsException_WhenInvalidConditions()
    {
        Action actionNullDestination = () => _collection.CopyTo(null!, 0);
        actionNullDestination.Should().Throw<ArgumentNullException>();

        var destination = new object[3];
        Action actionNegativeIndex = () => _collection.CopyTo(destination, -1);
        actionNegativeIndex.Should().Throw<ArgumentOutOfRangeException>();

        Action actionOutOfRangeIndex = () => _collection.CopyTo(destination, 4);
        actionOutOfRangeIndex.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void ObjectCollection_CopyTo_ThrowsException_WhenDestinationArrayIsTooSmall()
    {
        var items = new object[] { "Item1", "Item2", "Item3" };
        _collection.AddRange(items);
        var destination = new object[2];

        Action action = () => _collection.CopyTo(destination, 0);

        action.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void ObjectCollection_GetEnumerator_ReturnsEnumerator()
    {
        var items = new object[] { "Item1", "Item2", "Item3" };
        _collection.AddRange(items);

        var enumerator = _collection.GetEnumerator();

        enumerator.Should().NotBeNull();
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("Item1");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("Item2");
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().Be("Item3");
        enumerator.MoveNext().Should().BeFalse();
    }

    [WinFormsFact]
    public void ObjectCollection_IndexOf_ReturnsCorrectIndex_WhenItemExists()
    {
        var item1 = "Item1";
        var item2 = "Item2";
        _collection.Add(item1);
        _collection.Add(item2);

        _collection.IndexOf(item1).Should().Be(0);
        _collection.IndexOf(item2).Should().Be(1);
        _collection.IndexOf("NonExistentItem").Should().Be(-1);
    }

    [WinFormsFact]
    public void ObjectCollection_IndexOf_ThrowsException_WhenItemIsNull()
    {
        Action action = () => _collection.IndexOf(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [WinFormsTheory]
    [InlineData("Item1", "Item2", false)]
    [InlineData("Item2", "Item1", true)]
    public void ObjectCollection_Insert_InsertsItemCorrectly_OrInSortedOrder(string item1, string item2, bool sorted)
    {
        _comboBoxCell.Sorted = sorted;
        _collection.Add(item1);
        _collection.Insert(0, item2);

        if (sorted)
        {
            _collection[0].Should().Be(item2);
            _collection[1].Should().Be(item1);
        }
        else
        {
            _collection[0].Should().Be(item2);
            _collection[1].Should().Be(item1);
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void ObjectCollection_Insert_ThrowsException_WhenIndexIsInvalidOrItemIsNull(bool sorted)
    {
        _comboBoxCell.Sorted = sorted;

        Action actionNegativeIndex = () => _collection.Insert(-1, "Item");
        Action actionOutOfRangeIndex = () => _collection.Insert(1, "Item");
        Action actionNullItem = () => _collection.Insert(0, null!);

        actionNegativeIndex.Should().Throw<ArgumentOutOfRangeException>();
        actionOutOfRangeIndex.Should().Throw<ArgumentOutOfRangeException>();
        actionNullItem.Should().Throw<ArgumentNullException>();
    }

    [WinFormsFact]
    public void ObjectCollection_Remove_DoesNothing_WhenItemDoesNotExist()
    {
        var item1 = "Item1";
        var item2 = "Item2";
        _collection.Add(item1);

        _collection.Remove(item2);

        _collection.Count.Should().Be(1);
        _collection[0].Should().Be(item1);
    }

    [WinFormsFact]
    public void ObjectCollection_RemoveAt_RemovesItemCorrectly()
    {
        var item1 = "Item1";
        var item2 = "Item2";
        _collection.Add(item1);
        _collection.Add(item2);

        _collection.RemoveAt(0);

        _collection.Count.Should().Be(1);
        _collection[0].Should().Be(item2);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ObjectCollection_RemoveAt_ThrowsException_WhenIndexIsInvalid(int index)
    {
        Action action = () => _collection.RemoveAt(index);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}
