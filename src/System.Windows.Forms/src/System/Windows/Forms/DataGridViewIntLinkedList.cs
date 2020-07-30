// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a linked list of integers
    /// </summary>
    internal class DataGridViewIntLinkedList : IEnumerable
    {
        private DataGridViewIntLinkedListElement _lastAccessedElement;
        private DataGridViewIntLinkedListElement _headElement;
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
            Debug.Assert(source != null);
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
                Debug.Assert(index >= 0);
                Debug.Assert(index < Count);
                if (_lastAccessedIndex == -1 || index < _lastAccessedIndex)
                {
                    DataGridViewIntLinkedListElement tmp = _headElement;
                    int tmpIndex = index;
                    while (tmpIndex > 0)
                    {
                        tmp = tmp.Next;
                        tmpIndex--;
                    }
                    _lastAccessedElement = tmp;
                    _lastAccessedIndex = index;
                    return tmp.Int;
                }
                else
                {
                    while (_lastAccessedIndex < index)
                    {
                        _lastAccessedElement = _lastAccessedElement.Next;
                        _lastAccessedIndex++;
                    }
                    return _lastAccessedElement.Int;
                }
            }
            set
            {
                Debug.Assert(index >= 0);
                if (index != _lastAccessedIndex)
                {
                    int currentInt = this[index];
                    Debug.Assert(index == _lastAccessedIndex);
                }
                _lastAccessedElement.Int = value;
            }
        }

        public int Count { get; private set; }

        public int HeadInt
        {
            get
            {
                Debug.Assert(_headElement != null);
                return _headElement.Int;
            }
        }

        public void Add(int integer)
        {
            DataGridViewIntLinkedListElement newHead = new DataGridViewIntLinkedListElement(integer);
            if (_headElement != null)
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
            DataGridViewIntLinkedListElement tmp = _headElement;
            while (tmp != null)
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
            DataGridViewIntLinkedListElement tmp1 = null, tmp2 = _headElement;
            while (tmp2 != null)
            {
                if (tmp2.Int == integer)
                {
                    break;
                }
                tmp1 = tmp2;
                tmp2 = tmp2.Next;
            }
            if (tmp2.Int == integer)
            {
                DataGridViewIntLinkedListElement tmp3 = tmp2.Next;
                if (tmp1 is null)
                {
                    _headElement = tmp3;
                }
                else
                {
                    tmp1.Next = tmp3;
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
            DataGridViewIntLinkedListElement tmp1 = null, tmp2 = _headElement;
            while (index > 0)
            {
                tmp1 = tmp2;
                tmp2 = tmp2.Next;
                index--;
            }
            DataGridViewIntLinkedListElement tmp3 = tmp2.Next;
            if (tmp1 is null)
            {
                _headElement = tmp3;
            }
            else
            {
                tmp1.Next = tmp3;
            }
            Count--;
            _lastAccessedElement = null;
            _lastAccessedIndex = -1;
        }
    }

    /// <summary>
    ///  Represents an emunerator of elements in a <see cref='DataGridViewIntLinkedList'/>  linked list.
    /// </summary>
    internal class DataGridViewIntLinkedListEnumerator : IEnumerator
    {
        private readonly DataGridViewIntLinkedListElement _headElement;
        private DataGridViewIntLinkedListElement _current;
        private bool _reset;

        public DataGridViewIntLinkedListEnumerator(DataGridViewIntLinkedListElement headElement)
        {
            _headElement = headElement;
            _reset = true;
        }

        object IEnumerator.Current
        {
            get
            {
                Debug.Assert(_current != null); // Since this is for internal use only.
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
                Debug.Assert(_current != null); // Since this is for internal use only.
                _current = _current.Next;
            }
            return (_current != null);
        }

        void IEnumerator.Reset()
        {
            _reset = true;
            _current = null;
        }
    }

    /// <summary>
    ///  Represents an element in a <see cref='DataGridViewIntLinkedList'/> linked list.
    /// </summary>
    internal class DataGridViewIntLinkedListElement
    {
        public DataGridViewIntLinkedListElement(int integer)
        {
            Int = integer;
        }

        public int Int { get; set; }

        public DataGridViewIntLinkedListElement Next { get; set; }
    }
}
