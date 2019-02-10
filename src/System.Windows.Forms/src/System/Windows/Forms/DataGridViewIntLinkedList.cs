// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.Collections;
    using System.Windows.Forms;
    using System.ComponentModel;

    /// <devdoc>
    /// <para>Represents a linked list of integers</para>
    /// </devdoc>
    internal class DataGridViewIntLinkedList : IEnumerable
    {
        private DataGridViewIntLinkedListElement lastAccessedElement;
        private DataGridViewIntLinkedListElement headElement;
        private int count, lastAccessedIndex;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DataGridViewIntLinkedListEnumerator(this.headElement);
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
                Debug.Assert(index < this.count);
                if (this.lastAccessedIndex == -1 || index < this.lastAccessedIndex)
                {
                    DataGridViewIntLinkedListElement tmp = this.headElement;
                    int tmpIndex = index;
                    while (tmpIndex > 0)
                    {
                        tmp = tmp.Next;
                        tmpIndex--;
                    }
                    this.lastAccessedElement = tmp;
                    this.lastAccessedIndex = index;
                    return tmp.Int;
                }
                else
                {
                    while (this.lastAccessedIndex < index)
                    {
                        this.lastAccessedElement = this.lastAccessedElement.Next;
                        this.lastAccessedIndex++;
                    }
                    return this.lastAccessedElement.Int;
                }
            }
            set
            {
                Debug.Assert(index >= 0);
                if (index != this.lastAccessedIndex)
                {
                    int currentInt = this[index];
                    Debug.Assert(index == this.lastAccessedIndex);
                }
                this.lastAccessedElement.Int = value;
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }

        public int HeadInt
        {
            get
            {
                Debug.Assert(this.headElement != null);
                return this.headElement.Int;
            }
        }

        public void Add(int integer)
        {
            DataGridViewIntLinkedListElement newHead = new DataGridViewIntLinkedListElement(integer);
            if (this.headElement != null)
            {
                newHead.Next = this.headElement;
            }
            this.headElement = newHead;
            this.count++;
            this.lastAccessedElement = null;
            this.lastAccessedIndex = -1;
        }

        public void Clear()
        {
            this.lastAccessedElement = null;
            this.lastAccessedIndex = -1;
            this.headElement = null;
            this.count = 0;
        }

        public bool Contains(int integer)
        {
            int index = 0;
            DataGridViewIntLinkedListElement tmp = this.headElement;
            while (tmp != null)
            {
                if (tmp.Int == integer)
                {
                    this.lastAccessedElement = tmp;
                    this.lastAccessedIndex = index;
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
                return this.lastAccessedIndex;
            }
            else
            {
                return -1;
            }
        }

        public bool Remove(int integer)
        {
            DataGridViewIntLinkedListElement tmp1 = null, tmp2 = this.headElement;
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
                    this.headElement = tmp3;
                }
                else
                {
                    tmp1.Next = tmp3;
                }
                this.count--;
                this.lastAccessedElement = null;
                this.lastAccessedIndex = -1;
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            DataGridViewIntLinkedListElement tmp1 = null, tmp2 = this.headElement;
            while (index > 0)
            {
                tmp1 = tmp2;
                tmp2 = tmp2.Next;
                index--;
            }
            DataGridViewIntLinkedListElement tmp3 = tmp2.Next;
            if (tmp1 == null)
            {
                this.headElement = tmp3;
            }
            else
            {
                tmp1.Next = tmp3;
            }
            this.count--;
            this.lastAccessedElement = null;
            this.lastAccessedIndex = -1;
        }
    }

    /// <devdoc>
    /// <para>Represents an emunerator of elements in a <see cref='System.Windows.Forms.DataGridViewIntLinkedList'/>  linked list.</para>
    /// </devdoc>
    internal class DataGridViewIntLinkedListEnumerator : IEnumerator
    {
        private DataGridViewIntLinkedListElement headElement;
        private DataGridViewIntLinkedListElement current;
        private bool reset;

        public DataGridViewIntLinkedListEnumerator(DataGridViewIntLinkedListElement headElement)
        {
            this.headElement = headElement;
            this.reset = true;
        }

        object IEnumerator.Current
        {
            get
            {
                Debug.Assert(this.current != null); // Since this is for internal use only.
                return this.current.Int;
            }
        }

        bool IEnumerator.MoveNext()
        {
            if (this.reset)
            {
                Debug.Assert(this.current == null);
                this.current = this.headElement;
                this.reset = false;
            }
            else
            {
                Debug.Assert(this.current != null); // Since this is for internal use only.
                this.current = this.current.Next;
            }
            return (this.current != null);
        }

        void IEnumerator.Reset()
        {
            this.reset = true;
            this.current = null;
        }
    }

    /// <devdoc>
    /// <para>Represents an element in a <see cref='System.Windows.Forms.DataGridViewIntLinkedList'/> linked list.</para>
    /// </devdoc>
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
                return this.integer;
            }
            set
            {
                this.integer = value;
            }
        }

        public DataGridViewIntLinkedListElement Next
        {
            get
            {
                return this.next;
            }
            set
            {
                this.next = value;
            }
        }
    }
}
