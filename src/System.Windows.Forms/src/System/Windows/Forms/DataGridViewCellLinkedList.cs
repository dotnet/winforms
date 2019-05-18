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

    /// <summary>
    /// <para>Represents a linked list of <see cref='System.Windows.Forms.DataGridViewCell'/> objects</para>
    /// </devdoc>
    internal class DataGridViewCellLinkedList : IEnumerable
    {
        private DataGridViewCellLinkedListElement lastAccessedElement;
        private DataGridViewCellLinkedListElement headElement;
        private int count, lastAccessedIndex;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DataGridViewCellLinkedListEnumerator(this.headElement);
        }

        public DataGridViewCellLinkedList()
        {
            this.lastAccessedIndex = -1;
        }

        public DataGridViewCell this[int index]
        {
            get
            {
                Debug.Assert(index >= 0);
                Debug.Assert(index < this.count);
                if (this.lastAccessedIndex == -1 || index < this.lastAccessedIndex)
                {
                    DataGridViewCellLinkedListElement tmp = this.headElement;
                    int tmpIndex = index;
                    while (tmpIndex > 0)
                    {
                        tmp = tmp.Next;
                        tmpIndex--;
                    }
                    this.lastAccessedElement = tmp;
                    this.lastAccessedIndex = index;
                    return tmp.DataGridViewCell;
                }
                else
                {
                    while (this.lastAccessedIndex < index)
                    {
                        this.lastAccessedElement = this.lastAccessedElement.Next;
                        this.lastAccessedIndex++;
                    }
                    return this.lastAccessedElement.DataGridViewCell;
                }
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }

        public DataGridViewCell HeadCell
        {
            get
            {
                Debug.Assert(this.headElement != null);
                return this.headElement.DataGridViewCell;
            }
        }

        public void Add(DataGridViewCell dataGridViewCell)
        {
            Debug.Assert(dataGridViewCell != null);
            Debug.Assert(dataGridViewCell.DataGridView.SelectionMode == DataGridViewSelectionMode.CellSelect ||
                         dataGridViewCell.DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect ||
                         dataGridViewCell.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect);
            DataGridViewCellLinkedListElement newHead = new DataGridViewCellLinkedListElement(dataGridViewCell);
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

        public bool Contains(DataGridViewCell dataGridViewCell)
        {
            Debug.Assert(dataGridViewCell != null);
            int index = 0;
            DataGridViewCellLinkedListElement tmp = this.headElement;
            while (tmp != null)
            {
                if (tmp.DataGridViewCell == dataGridViewCell)
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

        public bool Remove(DataGridViewCell dataGridViewCell)
        {
            Debug.Assert(dataGridViewCell != null);
            DataGridViewCellLinkedListElement tmp1 = null, tmp2 = this.headElement;
            while (tmp2 != null)
            {
                if (tmp2.DataGridViewCell == dataGridViewCell)
                {
                    break;
                }
                tmp1 = tmp2;
                tmp2 = tmp2.Next;
            }
            if (tmp2.DataGridViewCell == dataGridViewCell)
            {
                DataGridViewCellLinkedListElement tmp3 = tmp2.Next;
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

        /* Unused for now
        public DataGridViewCell RemoveHead()
        {
            if (this.headElement == null)
            {
                return null;
            }
            DataGridViewCellLinkedListElement tmp = this.headElement;
            this.headElement = tmp.Next;
            this.count--;
            this.lastAccessedElement = null;
            this.lastAccessedIndex = -1;
            return tmp.DataGridViewCell;
        }
        */

        public int RemoveAllCellsAtBand(bool column, int bandIndex)
        {
            int removedCount = 0;
            DataGridViewCellLinkedListElement tmp1 = null, tmp2 = this.headElement;
            while (tmp2 != null)
            {
                if ((column && tmp2.DataGridViewCell.ColumnIndex == bandIndex) ||
                    (!column && tmp2.DataGridViewCell.RowIndex == bandIndex))
                {
                    DataGridViewCellLinkedListElement tmp3 = tmp2.Next;
                    if (tmp1 == null)
                    {
                        this.headElement = tmp3;
                    }
                    else
                    {
                        tmp1.Next = tmp3;
                    }
                    tmp2 = tmp3;
                    this.count--;
                    this.lastAccessedElement = null;
                    this.lastAccessedIndex = -1;
                    removedCount++;
                }
                else
                {
                    tmp1 = tmp2;
                    tmp2 = tmp2.Next;
                }
            }
            return removedCount;
        }
    }

    /// <summary>
    /// <para>Represents an emunerator of elements in a <see cref='System.Windows.Forms.DataGridViewCellLinkedList'/>  linked list.</para>
    /// </devdoc>
    internal class DataGridViewCellLinkedListEnumerator : IEnumerator
    {
        private DataGridViewCellLinkedListElement headElement;
        private DataGridViewCellLinkedListElement current;
        private bool reset;

        public DataGridViewCellLinkedListEnumerator(DataGridViewCellLinkedListElement headElement)
        {
            this.headElement = headElement;
            this.reset = true;
        }

        object IEnumerator.Current
        {
            get
            {
                Debug.Assert(this.current != null); // Since this is for internal use only.
                return this.current.DataGridViewCell;
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

    /// <summary>
    /// <para>Represents an element in a <see cref='System.Windows.Forms.DataGridViewCellLinkedList'/> linked list.</para>
    /// </devdoc>
    internal class DataGridViewCellLinkedListElement
    {
        private DataGridViewCell dataGridViewCell;
        private DataGridViewCellLinkedListElement next;

        public DataGridViewCellLinkedListElement(DataGridViewCell dataGridViewCell)
        {
            Debug.Assert(dataGridViewCell != null);
            this.dataGridViewCell = dataGridViewCell;
        }

        public DataGridViewCell DataGridViewCell
        {
            get
            {
                return this.dataGridViewCell;
            }
        }

        public DataGridViewCellLinkedListElement Next
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
