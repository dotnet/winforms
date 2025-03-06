// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public partial class TabControl
{
    public class TabPageCollection : IList
    {
        private readonly TabControl _owner;

        /// <summary>
        ///  A caching mechanism for key accessor. We use an index here rather than control so
        ///  that we don't have lifetime issues by holding on to extra references.
        /// </summary>
        private int _lastAccessedIndex = -1;

        public TabPageCollection(TabControl owner)
        {
            _owner = owner.OrThrowIfNull();
        }

        public virtual TabPage this[int index]
        {
            get => _owner.GetTabPage(index);
            set => _owner.SetTabPage(index, value);
        }

        object? IList.this[int index]
        {
            get => this[index];
            set
            {
                if (value is not TabPage tabPage)
                {
                    throw new ArgumentException(null, nameof(value));
                }

                this[index] = tabPage;
            }
        }

        /// <summary>
        ///  Retrieves the child control with the specified key.
        /// </summary>
        public virtual TabPage? this[string? key]
        {
            get
            {
                // We do not support null and empty string as valid keys.
                if (string.IsNullOrEmpty(key))
                {
                    return null;
                }

                // Search for the key in our collection
                int index = IndexOfKey(key);
                if (!IsValidIndex(index))
                {
                    return null;
                }

                return this[index];
            }
        }

        [Browsable(false)]
        public int Count => _owner.TabCount;

        object ICollection.SyncRoot => this;

        bool ICollection.IsSynchronized => false;

        bool IList.IsFixedSize => false;

        public bool IsReadOnly => false;

        public void Add(TabPage value)
        {
            ArgumentNullException.ThrowIfNull(value);

            _owner.Controls.Add(value);
        }

        int IList.Add(object? value)
        {
            if (value is not TabPage tabPage)
            {
                throw new ArgumentException(null, nameof(value));
            }

            Add(tabPage);
            return IndexOf(tabPage);
        }

        public void Add(string? text) => Add(new TabPage
        {
            Text = text
        });

        public void Add(string? key, string? text) => Add(new TabPage
        {
            Name = key,
            Text = text
        });

        public void Add(string? key, string? text, int imageIndex) => Add(new TabPage
        {
            Name = key,
            Text = text,
            ImageIndex = imageIndex
        });

        public void Add(string? key, string? text, string imageKey) => Add(new TabPage
        {
            Name = key,
            Text = text,
            ImageKey = imageKey
        });

        public void AddRange(params TabPage[] pages)
        {
            ArgumentNullException.ThrowIfNull(pages);

            foreach (TabPage page in pages)
            {
                Add(page);
            }
        }

        public bool Contains(TabPage page)
        {
            ArgumentNullException.ThrowIfNull(page);

            return IndexOf(page) != -1;
        }

        bool IList.Contains(object? page)
        {
            if (page is not TabPage tabPage)
            {
                return false;
            }

            return Contains(tabPage);
        }

        /// <summary>
        ///  Returns true if the collection contains an item with the specified key, false otherwise.
        /// </summary>
        public virtual bool ContainsKey(string? key)
        {
            return IsValidIndex(IndexOfKey(key));
        }

        public int IndexOf(TabPage page)
        {
            ArgumentNullException.ThrowIfNull(page);

            for (int index = 0; index < Count; ++index)
            {
                if (this[index] == page)
                {
                    return index;
                }
            }

            return -1;
        }

        int IList.IndexOf(object? page)
        {
            if (page is not TabPage tabPage)
            {
                return -1;
            }

            return IndexOf(tabPage);
        }

        /// <summary>
        ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
        /// </summary>
        public virtual int IndexOfKey(string? key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return -1;
            }

            // Check the last cached item
            if (IsValidIndex(_lastAccessedIndex))
            {
                if (WindowsFormsUtils.SafeCompareStrings(this[_lastAccessedIndex].Name, key, ignoreCase: true))
                {
                    return _lastAccessedIndex;
                }
            }

            // Search for the item
            for (int i = 0; i < Count; i++)
            {
                if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, ignoreCase: true))
                {
                    _lastAccessedIndex = i;
                    return i;
                }
            }

            // We didn't find it. Invalidate the last accessed index and return -1.
            _lastAccessedIndex = -1;
            return -1;
        }

        /// <summary>
        ///  Inserts the supplied Tabpage at the given index.
        /// </summary>
        public void Insert(int index, TabPage tabPage)
        {
            _owner.InsertItem(index, tabPage);
            try
            {
                // See InsertingItem property
                _owner.InsertingItem = true;
                _owner.Controls.Add(tabPage);
            }
            finally
            {
                _owner.InsertingItem = false;
            }

            _owner.Controls.SetChildIndex(tabPage, index);
        }

        void IList.Insert(int index, object? tabPage)
        {
            if (tabPage is not TabPage actualTabPage)
            {
                throw new ArgumentException(null, nameof(tabPage));
            }

            Insert(index, actualTabPage);
        }

        public void Insert(int index, string? text) => Insert(index, new TabPage
        {
            Text = text
        });

        public void Insert(int index, string? key, string? text) => Insert(index, new TabPage
        {
            Name = key,
            Text = text
        });

        public void Insert(int index, string? key, string? text, int imageIndex)
        {
            TabPage page = new()
            {
                Name = key,
                Text = text
            };

            Insert(index, page);

            // ImageKey and ImageIndex require parenting.
            page.ImageIndex = imageIndex;
        }

        public void Insert(int index, string? key, string? text, string imageKey)
        {
            TabPage page = new()
            {
                Name = key,
                Text = text
            };

            Insert(index, page);

            // ImageKey and ImageIndex require parenting.
            page.ImageKey = imageKey;
        }

        /// <summary>
        ///  Determines if the index is valid for the collection.
        /// </summary>
        private bool IsValidIndex(int index)
        {
            return ((index >= 0) && (index < Count));
        }

        public virtual void Clear() => _owner.RemoveAll();
        void ICollection.CopyTo(Array dest, int index)
        {
            if (Count > 0)
            {
                Array.Copy(_owner.GetTabPages(), 0, dest, index, Count);
            }
        }

        public IEnumerator GetEnumerator()
        {
            TabPage[] tabPages = _owner.GetTabPages();
            if (tabPages is null)
            {
                return Array.Empty<TabPage>().GetEnumerator();
            }

            return tabPages.GetEnumerator();
        }

        public void Remove(TabPage value)
        {
            ArgumentNullException.ThrowIfNull(value);

            _owner.Controls.Remove(value);
            if (value.IsHandleCreated)
            {
                value.ReleaseUiaProvider(value.HWNDInternal);
            }
        }

        void IList.Remove(object? value)
        {
            if (value is TabPage tabPage)
            {
                Remove(tabPage);
            }
        }

        public void RemoveAt(int index) => _owner.Controls.RemoveAt(index);

        /// <summary>
        ///  Removes the child control with the specified key.
        /// </summary>
        public virtual void RemoveByKey(string? key)
        {
            int index = IndexOfKey(key);
            if (IsValidIndex(index))
            {
                RemoveAt(index);
            }
        }
    }
}
