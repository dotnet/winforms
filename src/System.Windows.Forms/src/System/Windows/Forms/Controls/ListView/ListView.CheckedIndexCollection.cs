// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public partial class ListView
{
    [ListBindable(false)]
    public class CheckedIndexCollection : IList
    {
        private readonly ListView _owner;

        /* C#r: protected */
        public CheckedIndexCollection(ListView owner)
        {
            _owner = owner.OrThrowIfNull();
        }

        /// <summary>
        ///  Number of currently selected items.
        /// </summary>
        [Browsable(false)]
        public int Count
        {
            get
            {
                if (!_owner.CheckBoxes)
                {
                    return 0;
                }

                // Count the number of checked items
                int count = 0;
                foreach (ListViewItem item in _owner.Items)
                {
                    if (item is not null && item.Checked)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        private int[] IndicesArray
        {
            get
            {
                int[] indices = new int[Count];
                int index = 0;
                for (int i = 0; i < _owner.Items.Count && index < indices.Length; ++i)
                {
                    if (_owner.Items[i].Checked)
                    {
                        indices[index++] = i;
                    }
                }

                return indices;
            }
        }

        /// <summary>
        ///  Selected item in the list.
        /// </summary>
        public int this[int index]
        {
            get
            {
                ArgumentOutOfRangeException.ThrowIfNegative(index);

                // Loop through the main collection until we find the right index.
                int cnt = _owner.Items.Count;
                int nChecked = 0;
                for (int i = 0; i < cnt; i++)
                {
                    ListViewItem item = _owner.Items[i];

                    if (item.Checked)
                    {
                        if (nChecked == index)
                        {
                            return i;
                        }

                        nChecked++;
                    }
                }

                // Should never get to this point.
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }
        }

        object? IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return true;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool Contains(int checkedIndex)
        {
            if (_owner.Items[checkedIndex].Checked)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool IList.Contains(object? checkedIndex)
        {
            if (checkedIndex is int checkedIndexAsInt)
            {
                return Contains(checkedIndexAsInt);
            }
            else
            {
                return false;
            }
        }

        public int IndexOf(int checkedIndex)
        {
            int[] indices = IndicesArray;
            for (int index = 0; index < indices.Length; ++index)
            {
                if (indices[index] == checkedIndex)
                {
                    return index;
                }
            }

            return -1;
        }

        int IList.IndexOf(object? checkedIndex)
        {
            if (checkedIndex is int checkedIndexAsInt)
            {
                return IndexOf(checkedIndexAsInt);
            }
            else
            {
                return -1;
            }
        }

        int IList.Add(object? value)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object? value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object? value)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array dest, int index)
        {
            if (Count > 0)
            {
                Array.Copy(IndicesArray, 0, dest, index, Count);
            }
        }

        public IEnumerator GetEnumerator()
        {
            int[] indices = IndicesArray;
            if (indices is not null)
            {
                return indices.GetEnumerator();
            }
            else
            {
                return Array.Empty<int>().GetEnumerator();
            }
        }
    }
}
