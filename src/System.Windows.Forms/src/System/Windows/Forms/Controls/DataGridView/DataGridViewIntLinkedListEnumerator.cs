// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

/// <summary>
///  Represents an enumerator of elements in a <see cref="DataGridViewIntLinkedList"/>  linked list.
/// </summary>
internal class DataGridViewIntLinkedListEnumerator : IEnumerator
{
    private readonly DataGridViewIntLinkedListElement? _headElement;
    private DataGridViewIntLinkedListElement? _current;
    private bool _reset;

    public DataGridViewIntLinkedListEnumerator(DataGridViewIntLinkedListElement? headElement)
    {
        _headElement = headElement;
        _reset = true;
    }

    object IEnumerator.Current
    {
        get
        {
            Debug.Assert(_current is not null); // Since this is for internal use only.
            return _current.Int;
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

        return (_current is not null);
    }

    void IEnumerator.Reset()
    {
        _reset = true;
        _current = null;
    }
}
