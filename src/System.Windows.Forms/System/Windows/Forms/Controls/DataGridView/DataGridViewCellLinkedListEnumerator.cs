// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

/// <summary>
///  Represents an enumerator of elements in a <see cref="DataGridViewCellLinkedList"/>  linked list.
/// </summary>
internal class DataGridViewCellLinkedListEnumerator : IEnumerator
{
    private readonly DataGridViewCellLinkedListElement? _headElement;
    private DataGridViewCellLinkedListElement? _current;
    private bool _reset;

    public DataGridViewCellLinkedListEnumerator(DataGridViewCellLinkedListElement? headElement)
    {
        _headElement = headElement;
        _reset = true;
    }

    object IEnumerator.Current
    {
        get
        {
            Debug.Assert(_current is not null); // Since this is for internal use only.
            return _current.DataGridViewCell;
        }
    }

    bool IEnumerator.MoveNext()
    {
        if (_reset)
        {
            Debug.Assert(_current is null);
            _current = _headElement;
            _reset = false;
        }
        else
        {
            Debug.Assert(_current is not null); // Since this is for internal use only.
            _current = _current.Next;
        }

        return _current is not null;
    }

    void IEnumerator.Reset()
    {
        _reset = true;
        _current = null;
    }
}
