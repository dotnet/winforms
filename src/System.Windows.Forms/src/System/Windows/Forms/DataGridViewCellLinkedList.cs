// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Collections;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a linked list of <see cref='DataGridViewCell'/> objects
    /// </summary>
    internal class DataGridViewCellLinkedList : IEnumerable
    {
        private DataGridViewCellLinkedListElement lastAccessedElement;
        private DataGridViewCellLinkedListElement headElement;
        private int count, lastAccessedIndex;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DataGridViewCellLinkedListEnumerator(headElement);
        }

        public DataGridViewCellLinkedList()
        {
            lastAccessedIndex = -1;
        }

        public DataGridViewCell this[int index]
        {
            get
            {
                Debug.Assert(index >= 0);
                Debug.Assert(index < count);
                if (lastAccessedIndex == -1 || index < lastAccessedIndex)
                {
                    DataGridViewCellLinkedListElement tmp = headElement;
                    int tmpIndex = index;
                    while (tmpIndex > 0)
                    {
                        tmp = tmp.Next;
                        tmpIndex--;
                    }
                    lastAccessedElement = tmp;
                    lastAccessedIndex = index;
                    return tmp.DataGridViewCell;
                }
                else
                {
                    while (lastAccessedIndex < index)
                    {
                        lastAccessedElement = lastAccessedElement.Next;
                        lastAccessedIndex++;
                    }
                    return lastAccessedElement.DataGridViewCell;
                }
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public DataGridViewCell HeadCell
        {
            get
            {
                Debug.Assert(headElement != null);
                return headElement.DataGridViewCell;
            }
        }

        public void Add(DataGridViewCell dataGridViewCell)
        {
            Debug.Assert(dataGridViewCell != null);
            Debug.Assert(dataGridViewCell.DataGridView.SelectionMode == DataGridViewSelectionMode.CellSelect ||
                         dataGridViewCell.DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect ||
                         dataGridViewCell.DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect);
            DataGridViewCellLinkedListElement newHead = new DataGridViewCellLinkedListElement(dataGridViewCell);
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

        public bool Contains(DataGridViewCell dataGridViewCell)
        {
            Debug.Assert(dataGridViewCell != null);
            int index = 0;
            DataGridViewCellLinkedListElement tmp = headElement;
            while (tmp != null)
            {
                if (tmp.DataGridViewCell == dataGridViewCell)
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

        public bool Remove(DataGridViewCell dataGridViewCell)
        {
            Debug.Assert(dataGridViewCell != null);
            DataGridViewCellLinkedListElement tmp1 = null, tmp2 = headElement;
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
            DataGridViewCellLinkedListElement tmp1 = null, tmp2 = headElement;
            while (tmp2 != null)
            {
                if ((column && tmp2.DataGridViewCell.ColumnIndex == bandIndex) ||
                    (!column && tmp2.DataGridViewCell.RowIndex == bandIndex))
                {
                    DataGridViewCellLinkedListElement tmp3 = tmp2.Next;
                    if (tmp1 == null)
                    {
                        headElement = tmp3;
                    }
                    else
                    {
                        tmp1.Next = tmp3;
                    }
                    tmp2 = tmp3;
                    count--;
                    lastAccessedElement = null;
                    lastAccessedIndex = -1;
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
    ///  Represents an emunerator of elements in a <see cref='DataGridViewCellLinkedList'/>  linked list.
    /// </summary>
    internal class DataGridViewCellLinkedListEnumerator : IEnumerator
    {
        private readonly DataGridViewCellLinkedListElement headElement;
        private DataGridViewCellLinkedListElement current;
        private bool reset;

        public DataGridViewCellLinkedListEnumerator(DataGridViewCellLinkedListElement headElement)
        {
            this.headElement = headElement;
            reset = true;
        }

        object IEnumerator.Current
        {
            get
            {
                Debug.Assert(current != null); // Since this is for internal use only.
                return current.DataGridViewCell;
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
    ///  Represents an element in a <see cref='DataGridViewCellLinkedList'/> linked list.
    /// </summary>
    internal class DataGridViewCellLinkedListElement
    {
        private readonly DataGridViewCell dataGridViewCell;
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
                return dataGridViewCell;
            }
        }

        public DataGridViewCellLinkedListElement Next
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
