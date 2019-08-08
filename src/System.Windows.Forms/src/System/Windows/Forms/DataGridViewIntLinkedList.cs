// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a linked list of integers
    /// </summary>
    internal class DataGridViewIntLinkedList : IEnumerable
    {
        private DataGridViewIntLinkedListElement lastAccessedElement;
        private DataGridViewIntLinkedListElement headElement;
        private int count, lastAccessedIndex;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DataGridViewIntLinkedListEnumerator(headElement);
        }

        public DataGridViewIntLinkedList()
        {
            lastAccessedIndex = -1;
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
                Debug.Assert(index < count);
                if (lastAccessedIndex == -1 || index < lastAccessedIndex)
                {
                    DataGridViewIntLinkedListElement tmp = headElement;
                    int tmpIndex = index;
                    while (tmpIndex > 0)
                    {
                        tmp = tmp.Next;
                        tmpIndex--;
                    }
                    lastAccessedElement = tmp;
                    lastAccessedIndex = index;
                    return tmp.Int;
                }
                else
                {
                    while (lastAccessedIndex < index)
                    {
                        lastAccessedElement = lastAccessedElement.Next;
                        lastAccessedIndex++;
                    }
                    return lastAccessedElement.Int;
                }
            }
            set
            {
                Debug.Assert(index >= 0);
                if (index != lastAccessedIndex)
                {
                    int currentInt = this[index];
                    Debug.Assert(index == lastAccessedIndex);
                }
                lastAccessedElement.Int = value;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public int HeadInt
        {
            get
            {
                Debug.Assert(headElement != null);
                return headElement.Int;
            }
        }

        public void Add(int integer)
        {
            DataGridViewIntLinkedListElement newHead = new DataGridViewIntLinkedListElement(integer);
            if (headElement != null)
            {
                newHead.Next = headElement;
            }
            headElement = newHead;
            count++;
            lastAccessedElement = null;
            lastAccessedIndex = -1;
        }

        public void Clear()
        {
            lastAccessedElement = null;
            lastAccessedIndex = -1;
            headElement = null;
            count = 0;
        }

        public bool Contains(int integer)
        {
            int index = 0;
            DataGridViewIntLinkedListElement tmp = headElement;
            while (tmp != null)
            {
                if (tmp.Int == integer)
                {
                    lastAccessedElement = tmp;
                    lastAccessedIndex = index;
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
                return lastAccessedIndex;
            }
            else
            {
                return -1;
            }
        }

        public bool Remove(int integer)
        {
            DataGridViewIntLinkedListElement tmp1 = null, tmp2 = headElement;
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
                if (tmp1 == null)
                {
                    headElement = tmp3;
                }
                else
                {
                    tmp1.Next = tmp3;
                }
                count--;
                lastAccessedElement = null;
                lastAccessedIndex = -1;
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            DataGridViewIntLinkedListElement tmp1 = null, tmp2 = headElement;
            while (index > 0)
            {
                tmp1 = tmp2;
                tmp2 = tmp2.Next;
                index--;
            }
            DataGridViewIntLinkedListElement tmp3 = tmp2.Next;
            if (tmp1 == null)
            {
                headElement = tmp3;
            }
            else
            {
                tmp1.Next = tmp3;
            }
            count--;
            lastAccessedElement = null;
            lastAccessedIndex = -1;
        }
    }

    /// <summary>
    ///  Represents an emunerator of elements in a <see cref='DataGridViewIntLinkedList'/>  linked list.
    /// </summary>
    internal class DataGridViewIntLinkedListEnumerator : IEnumerator
    {
        private readonly DataGridViewIntLinkedListElement headElement;
        private DataGridViewIntLinkedListElement current;
        private bool reset;

        public DataGridViewIntLinkedListEnumerator(DataGridViewIntLinkedListElement headElement)
        {
            this.headElement = headElement;
            reset = true;
        }

        object IEnumerator.Current
        {
            get
            {
                Debug.Assert(current != null); // Since this is for internal use only.
                return current.Int;
            }
        }

        bool IEnumerator.MoveNext()
        {
            if (reset)
            {
                Debug.Assert(current == null);
                current = headElement;
                reset = false;
            }
            else
            {
                Debug.Assert(current != null); // Since this is for internal use only.
                current = current.Next;
            }
            return (current != null);
        }

        void IEnumerator.Reset()
        {
            reset = true;
            current = null;
        }
    }

    /// <summary>
    ///  Represents an element in a <see cref='DataGridViewIntLinkedList'/> linked list.
    /// </summary>
    internal class DataGridViewIntLinkedListElement
    {
        private int integer;
        private DataGridViewIntLinkedListElement next;

        public DataGridViewIntLinkedListElement(int integer)
        {
            this.integer = integer;
        }

        public int Int
        {
            get
            {
                return integer;
            }
            set
            {
                integer = value;
            }
        }

        public DataGridViewIntLinkedListElement Next
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
}
