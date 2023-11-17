﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

/// <summary>
///  Represents a linked list of <see cref="DataGridViewCell"/> objects
/// </summary>
internal class DataGridViewCellLinkedList : IEnumerable
{
    private DataGridViewCellLinkedListElement? _lastAccessedElement;
    private DataGridViewCellLinkedListElement? _headElement;
    private int _count;
    private int _lastAccessedIndex;

    IEnumerator IEnumerable.GetEnumerator() => new DataGridViewCellLinkedListEnumerator(_headElement);

    public DataGridViewCellLinkedList()
    {
        _lastAccessedIndex = -1;
    }

    public DataGridViewCell this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
            if (_lastAccessedIndex == -1 || index < _lastAccessedIndex)
            {
                // Since we have checked the index value, targetElement should not be null here.
                DataGridViewCellLinkedListElement targetElement = _headElement!;
                int tmpIndex = index;
                while (tmpIndex > 0)
                {
                    targetElement = targetElement.Next!;
                    tmpIndex--;
                }

                _lastAccessedElement = targetElement;
                _lastAccessedIndex = index;
                return targetElement.DataGridViewCell;
            }
            else
            {
                while (_lastAccessedIndex < index)
                {
                    _lastAccessedElement = _lastAccessedElement!.Next;
                    _lastAccessedIndex++;
                }

                return _lastAccessedElement!.DataGridViewCell;
            }
        }
    }

    public int Count => _count;

    public DataGridViewCell HeadCell
    {
        get
        {
            Debug.Assert(_headElement is not null);
            return _headElement.DataGridViewCell;
        }
    }

    public void Add(DataGridViewCell dataGridViewCell)
    {
        Debug.Assert(dataGridViewCell is not null);
        Debug.Assert(dataGridViewCell.DataGridView!.SelectionMode == DataGridViewSelectionMode.CellSelect ||
                     dataGridViewCell.DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect ||
                     dataGridViewCell.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect);
        DataGridViewCellLinkedListElement newHead = new DataGridViewCellLinkedListElement(dataGridViewCell);
        if (_headElement is not null)
        {
            newHead.Next = _headElement;
        }

        _headElement = newHead;
        _count++;
        _lastAccessedElement = null;
        _lastAccessedIndex = -1;
    }

    public void Clear()
    {
        _lastAccessedElement = null;
        _lastAccessedIndex = -1;
        _headElement = null;
        _count = 0;
    }

    public bool Contains(DataGridViewCell dataGridViewCell)
    {
        Debug.Assert(dataGridViewCell is not null);
        int index = 0;
        DataGridViewCellLinkedListElement? tmp = _headElement;
        while (tmp is not null)
        {
            if (tmp.DataGridViewCell == dataGridViewCell)
            {
                _lastAccessedElement = tmp;
                _lastAccessedIndex = index;
                return true;
            }

            tmp = tmp.Next;
            index++;
        }

        return false;
    }

    public bool Remove(DataGridViewCell dataGridViewCell)
    {
        Debug.Assert(dataGridViewCell is not null);
        DataGridViewCellLinkedListElement? removeTargetPrevious = null;
        DataGridViewCellLinkedListElement? tmp2 = _headElement;
        while (tmp2 is not null)
        {
            if (tmp2.DataGridViewCell == dataGridViewCell)
            {
                break;
            }

            removeTargetPrevious = tmp2;
            tmp2 = tmp2.Next;
        }

        if (tmp2 is not null && tmp2.DataGridViewCell == dataGridViewCell)
        {
            DataGridViewCellLinkedListElement? tmp3 = tmp2.Next;
            if (removeTargetPrevious is null)
            {
                _headElement = tmp3;
            }
            else
            {
                removeTargetPrevious.Next = tmp3;
            }

            _count--;
            _lastAccessedElement = null;
            _lastAccessedIndex = -1;
            return true;
        }

        return false;
    }

    public int RemoveAllCellsAtBand(bool column, int bandIndex)
    {
        int removedCount = 0;
        DataGridViewCellLinkedListElement? removeTargetPrevious = null;
        DataGridViewCellLinkedListElement? tmp2 = _headElement;
        while (tmp2 is not null)
        {
            if ((column && tmp2.DataGridViewCell.ColumnIndex == bandIndex) ||
                (!column && tmp2.DataGridViewCell.RowIndex == bandIndex))
            {
                DataGridViewCellLinkedListElement? tmp3 = tmp2.Next;
                if (removeTargetPrevious is null)
                {
                    _headElement = tmp3;
                }
                else
                {
                    removeTargetPrevious.Next = tmp3;
                }

                tmp2 = tmp3;
                _count--;
                _lastAccessedElement = null;
                _lastAccessedIndex = -1;
                removedCount++;
            }
            else
            {
                removeTargetPrevious = tmp2;
                tmp2 = tmp2.Next;
            }
        }

        return removedCount;
    }
}
