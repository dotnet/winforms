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
    public void ObjectCollection_AddRange_AddsObjectCollectionCorrectly()
    {
        DataGridViewComboBoxCell.ObjectCollection items = new(_comboBoxCell) { "Item1", "Item2", "Item3" };

        _collection.AddRange(items);

        _collection.InnerArray.Count.Should().Be(3);
        _collection[0].Should().Be("Item1");
        _collection[1].Should().Be("Item2");
        _collection[2].Should().Be("Item3");
    }
}
