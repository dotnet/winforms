// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents an element in a <see cref="DataGridViewCellLinkedList"/> linked list.
/// </summary>
internal class DataGridViewCellLinkedListElement
{
    private readonly DataGridViewCell _dataGridViewCell;

    public DataGridViewCellLinkedListElement(DataGridViewCell dataGridViewCell)
    {
        Debug.Assert(dataGridViewCell is not null);
        _dataGridViewCell = dataGridViewCell;
    }

    public DataGridViewCell DataGridViewCell => _dataGridViewCell;

    public DataGridViewCellLinkedListElement? Next { get; set; }
}
