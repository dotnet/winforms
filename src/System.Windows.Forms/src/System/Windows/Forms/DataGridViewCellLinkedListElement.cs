// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents an element in a <see cref="DataGridViewCellLinkedList"/> linked list.
/// </summary>
internal class DataGridViewCellLinkedListElement
{
    private readonly DataGridViewCell dataGridViewCell;
    private DataGridViewCellLinkedListElement? next;

    public DataGridViewCellLinkedListElement(DataGridViewCell dataGridViewCell)
    {
        Debug.Assert(dataGridViewCell is not null);
        this.dataGridViewCell = dataGridViewCell;
    }

    public DataGridViewCell DataGridViewCell
    {
        get
        {
            return dataGridViewCell;
        }
    }

    public DataGridViewCellLinkedListElement? Next
    {
        get
        {
            return next;
        }
        set
        {
            next = value;
        }
    }
}
