// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

/// <summary>
///  Represents a linked list of integers
/// </summary>
internal class DataGridViewIntLinkedList : IEnumerable
{
    private DataGridViewIntLinkedListElement? _lastAccessedElement;
    private DataGridViewIntLinkedListElement? _headElement;
    private int _lastAccessedIndex;

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new DataGridViewIntLinkedListEnumerator(_headElement);
    }

    public DataGridViewIntLinkedList()
    {
        _lastAccessedIndex = -1;
    }

    public DataGridViewIntLinkedList(DataGridViewIntLinkedList source)
    {
        Debug.Assert(source is not null);
        int elements = source.Count;
        for (int element = 0; element < elements; element++)
        {
            Add(source[element]);
        }
    }

    public int this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
            if (_lastAccessedIndex == -1 || index < _lastAccessedIndex)
            {
                // Since we have checked the index value, targetElement should not be null here.
                DataGridViewIntLinkedListElement targetElement = _headElement!;
                int tmpIndex = index;
                while (tmpIndex > 0)
                {
                    targetElement = targetElement.Next!;
                    tmpIndex--;
                }

                _lastAccessedElement = targetElement;
                _lastAccessedIndex = index;
                return targetElement.Int;
            }
            else
            {
                while (_lastAccessedIndex < index)
                {
                    _lastAccessedElement = _lastAccessedElement!.Next;
                    _lastAccessedIndex++;
                }

                return _lastAccessedElement!.Int;
            }
        }
        set
        {
            Debug.Assert(index >= 0);
            if (index != _lastAccessedIndex)
            {
                _ = this[index];
                Debug.Assert(index == _lastAccessedIndex);
            }

            _lastAccessedElement!.Int = value;
        }
    }

    public int Count { get; private set; }

    public int HeadInt
    {
        get
        {
            Debug.Assert(_headElement is not null);
            return _headElement.Int;
        }
    }

    public void Add(int integer)
    {
        DataGridViewIntLinkedListElement newHead = new(integer);
        if (_headElement is not null)
        {
            newHead.Next = _headElement;
        }

        _headElement = newHead;
        Count++;
        _lastAccessedElement = null;
        _lastAccessedIndex = -1;
    }

    public void Clear()
    {
        _lastAccessedElement = null;
        _lastAccessedIndex = -1;
        _headElement = null;
        Count = 0;
    }

    public bool Contains(int integer)
    {
        int index = 0;
        DataGridViewIntLinkedListElement? tmp = _headElement;
        while (tmp is not null)
        {
            if (tmp.Int == integer)
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

    public int IndexOf(int integer)
    {
        if (Contains(integer))
        {
            return _lastAccessedIndex;
        }
        else
        {
            return -1;
        }
    }

    public bool Remove(int integer)
    {
        DataGridViewIntLinkedListElement? removeTargetPrevious = null;
        DataGridViewIntLinkedListElement? removeTarget = _headElement;
        while (removeTarget is not null)
        {
            if (removeTarget.Int == integer)
            {
                break;
            }

            removeTargetPrevious = removeTarget;
            removeTarget = removeTarget.Next;
        }

        if (removeTarget is not null && removeTarget.Int == integer)
        {
            DataGridViewIntLinkedListElement? removeTargetNext = removeTarget.Next;
            if (removeTargetPrevious is null)
            {
                _headElement = removeTargetNext;
            }
            else
            {
                removeTargetPrevious.Next = removeTargetNext;
            }

            Count--;
            _lastAccessedElement = null;
            _lastAccessedIndex = -1;
            return true;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);

        DataGridViewIntLinkedListElement? removeTargetPrevious = null;

        // Since we have checked the index value, removeTarget should not be null here.
        DataGridViewIntLinkedListElement removeTarget = _headElement!;
        while (index > 0)
        {
            removeTargetPrevious = removeTarget;
            removeTarget = removeTarget.Next!;
            index--;
        }

        DataGridViewIntLinkedListElement? removeTargetNext = removeTarget!.Next;
        if (removeTargetPrevious is null)
        {
            _headElement = removeTargetNext;
        }
        else
        {
            removeTargetPrevious.Next = removeTargetNext;
        }

        Count--;
        _lastAccessedElement = null;
        _lastAccessedIndex = -1;
    }
}
