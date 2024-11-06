// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class CheckedListBox_CheckedIndexCollectionTests : IDisposable
{
    private readonly CheckedListBox _checkedListBox;
    private readonly CheckedListBox.CheckedIndexCollection _collection;

    public CheckedListBox_CheckedIndexCollectionTests()
    {
        _checkedListBox = new();
        _collection = new(_checkedListBox);
    }

    public void Dispose()
    {
        _checkedListBox.Items.Clear();
        _checkedListBox.Dispose();
    }

    [WinFormsFact]
    public void CheckedIndexCollection_Ctor_OwnerIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => { new CheckedListBox.CheckedIndexCollection(null); });
    }

    [WinFormsFact]
    public void CheckedIndexCollection_Properties_ReturnExpected()
    {
        // Test properties: Count, SyncRoot, IsSynchronized, IsFixedSize, IsReadOnly
        _collection.Count.Should().Be(0);
        ((ICollection)_collection).SyncRoot.Should().BeSameAs(_collection);
        ((ICollection)_collection).IsSynchronized.Should().BeFalse();
        ((IList)_collection).IsFixedSize.Should().BeTrue();
        _collection.IsReadOnly.Should().BeTrue();
    }

    [WinFormsFact]
    public void CheckedIndexCollection_Methods_ThrowNotSupportedException()
    {
        // Test methods: Add, Clear, Insert, Remove, RemoveAt that should throw NotSupportedException
        ((Action)(() => ((IList)_collection)[0] = 1)).Should().Throw<NotSupportedException>();
        ((IList)_collection).Invoking(c => c.Add(1)).Should().Throw<NotSupportedException>();
        ((IList)_collection).Invoking(c => c.Clear()).Should().Throw<NotSupportedException>();
        ((IList)_collection).Invoking(c => c.Insert(0, 1)).Should().Throw<NotSupportedException>();
        ((IList)_collection).Invoking(c => c.Remove(1)).Should().Throw<NotSupportedException>();
        ((IList)_collection).Invoking(c => c.RemoveAt(0)).Should().Throw<NotSupportedException>();
    }

    [WinFormsTheory]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { true, false, true }, 0, true)]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { true, false, true }, 1, false)]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { false, true, false }, 1, true)]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { false, false, false }, 0, false)]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { true, false, true }, "invalid", false)]
    public void CheckedIndexCollection_ContainsIntAndObject_ReturnsExpected(string[] items, bool[] checkedStates, object index, bool expected)
    {
        for (int i = 0; i < items.Length; i++)
        {
            _checkedListBox.Items.Add(items[i], checkedStates[i]);
        }

        // Test Contains(int index) method
        if (index is int @int)
        {
            _collection.Contains(@int).Should().Be(expected);
        }

        // Test IList.Contains(object? index) method
        ((IList)_collection).Contains(index).Should().Be(expected);
    }

    [WinFormsTheory]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { true, false, true }, 0, 0)]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { true, false, true }, 1, -1)]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { false, true, false }, 1, 0)]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { false, false, false }, 0, -1)]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { true, false, true }, "invalid", -1)]
    public void CheckedIndexCollection_IndexOfIntAndObject_ReturnsExpected(string[] items, bool[] checkedStates, object index, int expected)
    {
        for (int i = 0; i < items.Length; i++)
        {
            _checkedListBox.Items.Add(items[i], checkedStates[i]);
        }

        // Test IndexOf(int index) method
        if (index is int @int)
        {
            _collection.IndexOf(@int).Should().Be(expected);
        }

        // Test IList.IndexOf(object? index) method
        ((IList)_collection).IndexOf(index).Should().Be(expected);
    }

    [WinFormsTheory]
    [InlineData([new string[] { "item1", "item2", "item3" }, new bool[] { true, false, true }, new int[] { 0, 2, 0 }])]
    [InlineData([new string[] { "item1", "item2" }, new bool[] { true, true }, new int[] { 0, 1 }])]
    [InlineData([new string[] { "item1", "item2", "item3" }, new bool[] { false, false, false }, new int[] { 0, 0, 0 }])]
    public void CheckedIndexCollection_CopyTo_CopiesExpectedValues(string[] items, bool[] checkedStatus, int[] expected)
    {
        for (int i = 0; i < items.Length; i++)
        {
            _checkedListBox.Items.Add(items[i], checkedStatus[i]);
        }

        // Create an array to copy to
        int[] array = new int[items.Length];

        // Copy the collection to the array
        _collection.CopyTo(array, 0);

        // Test that the copied values are correct
        array.Should().Equal(expected);
    }

    [WinFormsTheory]
    [InlineData([new string[] { "item1", "item2", "item3" }, new bool[] { true, false, true }, new int[] { 0, 2 }])]
    [InlineData([new string[] { "item1", "item2" }, new bool[] { true, true }, new int[] { 0, 1 }])]
    [InlineData([new string[] { "item1", "item2", "item3" }, new bool[] { false, false, false }, new int[0]])]
    public void CheckedIndexCollection_GetEnumerator_ReturnsExpected(string[] items, bool[] checkedStatus, int[] expected)
    {
        for (int i = 0; i < items.Length; i++)
        {
            _checkedListBox.Items.Add(items[i], checkedStatus[i]);
        }

        // Test GetEnumerator method
        IEnumerator enumerator = _collection.GetEnumerator();
        foreach (int expectedIndex in expected)
        {
            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().Be(expectedIndex);
        }

        enumerator.MoveNext().Should().BeFalse();
    }

    [WinFormsTheory]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { true, false, true }, 0, 0)]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { true, false, true }, 1, 2)]
    [InlineData(new string[] { "item1", "item2", "item3" }, new bool[] { false, true, false }, 0, 1)]
    public void CheckedIndexCollection_ItemGet_ReturnsExpected(string[] items, bool[] checkedStates, int index, int expected)
    {
        for (int i = 0; i < items.Length; i++)
        {
            _checkedListBox.Items.Add(items[i], checkedStates[i]);
        }

        int result = (int)((IList)_collection)[index];
        result.Should().Be(expected);
    }
}
