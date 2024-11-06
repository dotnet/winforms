// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class CheckedListBox_CheckedItemCollectionTests : IDisposable
{
    private readonly CheckedListBox _checkedListBox;
    private readonly CheckedListBox.CheckedItemCollection _collection;

    public CheckedListBox_CheckedItemCollectionTests()
    {
        _checkedListBox = new CheckedListBox();
        _collection = new CheckedListBox.CheckedItemCollection(_checkedListBox);
    }

    public void Dispose()
    {
        _checkedListBox.Items.Clear();
        _checkedListBox.Dispose();
    }

    [WinFormsFact]
    public void CheckedItemCollection_Properties_ReturnExpected()
    {
        // Test properties: Count, SyncRoot, IsSynchronized, IsFixedSize, IsReadOnly
        _collection.Count.Should().Be(0);
        ((ICollection)_collection).SyncRoot.Should().BeSameAs(_collection);
        ((ICollection)_collection).IsSynchronized.Should().BeFalse();
        ((IList)_collection).IsFixedSize.Should().BeTrue();
        _collection.IsReadOnly.Should().BeTrue();
    }

    [WinFormsFact]
    public void CheckedItemCollection_Methods_ThrowNotSupportedException()
    {
        // Test methods: Add, Clear, Insert, Remove, RemoveAt that should throw NotSupportedException
        ((Action)(() => ((IList)_collection)[0] = 1)).Should().Throw<NotSupportedException>();
        ((IList)_collection).Invoking(c => c.Add(1)).Should().Throw<NotSupportedException>();
        ((IList)_collection).Invoking(c => c.Clear()).Should().Throw<NotSupportedException>();
        ((IList)_collection).Invoking(c => c.Insert(0, 1)).Should().Throw<NotSupportedException>();
        ((IList)_collection).Invoking(c => c.Remove(1)).Should().Throw<NotSupportedException>();
        ((IList)_collection).Invoking(c => c.RemoveAt(0)).Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void CheckedItemCollection_ItemGet_ReturnsExpected()
    {
        _checkedListBox.Items.Add("item1", true);
        _checkedListBox.Items.Add("item2", false);

        _collection.Count.Should().Be(1);
        _collection[0].Should().Be("item1");
    }

    private void AddItemsToCheckedListBox(string[] items, bool[] checkedStates)
    {
        for (int i = 0; i < items.Length; i++)
        {
            _checkedListBox.Items.Add(items[i], checkedStates[i]);
        }
    }

    [WinFormsTheory]
    [InlineData(new string[] { "item1", "item2" }, new bool[] { true, false }, "item1", true)]
    [InlineData(new string[] { "item1", "item2" }, new bool[] { false, true }, "item2", true)]
    [InlineData(new string[] { "item1", "item2" }, new bool[] { false, false }, "item1", false)]
    public void CheckedItemCollection_Contains_ReturnsExpected(string[] items, bool[] checkedStates, string item, bool expected)
    {
        AddItemsToCheckedListBox(items, checkedStates);

        _collection.Contains(item).Should().Be(expected);
    }

    [WinFormsTheory]
    [InlineData(new string[] { "item1", "item2" }, new bool[] { true, false }, "item1", 0)]
    [InlineData(new string[] { "item1", "item2" }, new bool[] { false, true }, "item2", 0)]
    [InlineData(new string[] { "item1", "item2" }, new bool[] { false, false }, "item1", -1)]
    public void CheckedItemCollection_IndexOf_ReturnsExpected(string[] items, bool[] checkedStates, string item, int expected)
    {
        AddItemsToCheckedListBox(items, checkedStates);

        _collection.IndexOf(item).Should().Be(expected);
    }

    [WinFormsTheory]
    [InlineData(new string[] { "item1", "item2" }, new bool[] { true, false }, new object[] { "item1", null })]
    [InlineData(new string[] { "item1", "item2" }, new bool[] { false, true }, new object[] { "item2", null })]
    [InlineData(new string[] { "item1", "item2" }, new bool[] { false, false }, new object[] { null, null })]
    public void CheckedItemCollection_CopyTo_CopiesExpectedValues(string[] items, bool[] checkedStates, object[] expected)
    {
        AddItemsToCheckedListBox(items, checkedStates);

        object[] array = new object[items.Length];
        ((ICollection)_collection).CopyTo(array, 0);

        array.Should().Equal(expected);
    }

    [WinFormsTheory]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { true, false, true }, new object[] { "item1", "item3" })]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { false, true, false }, new object[] { "item2" })]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { false, false, false }, new object[] { })]
    public void CheckedItemCollection_GetEnumerator_ReturnsExpected(string[] items, bool[] checkedStates, object[] expected)
    {
        AddItemsToCheckedListBox(items, checkedStates);

        IEnumerator enumerator = _collection.GetEnumerator();
        List<object> result = [];
        while (enumerator.MoveNext())
        {
            result.Add(enumerator.Current);
        }

        result.Should().Equal(expected);
    }
}
