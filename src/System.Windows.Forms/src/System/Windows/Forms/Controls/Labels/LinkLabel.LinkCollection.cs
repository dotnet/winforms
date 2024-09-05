// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

public partial class LinkLabel
{
    public class LinkCollection : IList
    {
        private readonly LinkLabel _owner;

        ///  A caching mechanism for key accessor
        ///  We use an index here rather than control so that we don't have lifetime
        ///  issues by holding on to extra references.
        ///  Note this is not Thread Safe - but WinForms has to be run in a STA anyways.
        private int _lastAccessedIndex = -1;

        public LinkCollection(LinkLabel owner) => _owner = owner.OrThrowIfNull();

        public virtual Link this[int index]
        {
            get
            {
                return _owner._links[index];
            }
            set
            {
                _owner._links[index] = value;

                _owner._links.Sort(s_linkComparer);

                _owner.InvalidateTextLayout();
                _owner.Invalidate();
            }
        }

        object? IList.this[int index]
        {
            get => this[index];
            set
            {
                if (value is Link link)
                {
                    this[index] = link;
                }
                else
                {
                    throw new ArgumentException(SR.LinkLabelBadLink, nameof(value));
                }
            }
        }

        /// <summary>
        ///  Retrieves the child control with the specified key.
        /// </summary>
        public virtual Link? this[string key]
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
                if (IsValidIndex(index))
                {
                    return this[index];
                }
                else
                {
                    return null;
                }
            }
        }

        [Browsable(false)]
        public int Count => _owner._links.Count;

        /// <summary>
        ///  whether we have added a non-trivial link to the collection
        /// </summary>
        public bool LinksAdded { get; private set; }

        object ICollection.SyncRoot => this;

        bool ICollection.IsSynchronized => false;

        bool IList.IsFixedSize => false;

        public bool IsReadOnly => false;

        public Link Add(int start, int length)
        {
            if (length != 0)
            {
                LinksAdded = true;
            }

            return Add(start, length, null);
        }

        public Link Add(int start, int length, object? linkData)
        {
            if (length != 0)
            {
                LinksAdded = true;
            }

            // check for the special case where the list is in the "magic"
            // state of having only the default link in it. In that case
            // we want to clear the list before adding this link.

            if (_owner._links.Count == 1
                && this[0].Start == 0
                && this[0]._length == -1)
            {
                _owner._links.Clear();
                _owner.FocusLink = null;
            }

            Link l = new(_owner)
            {
                Start = start,
                Length = length,
                LinkData = linkData
            };
            Add(l);
            return l;
        }

        public int Add(Link value)
        {
            if (value is not null && value.Length != 0)
            {
                LinksAdded = true;
            }

            // check for the special case where the list is in the "magic"
            // state of having only the default link in it. In that case
            // we want to clear the list before adding this link.
            if (_owner._links.Count == 1
                && this[0].Start == 0
                && this[0]._length == -1)
            {
                _owner._links.Clear();
                _owner.FocusLink = null;
            }

            // Set the owner control for this link
            value!.Owner = _owner;

            _owner._links.Add(value);

            if (_owner.AutoSize)
            {
                LayoutTransaction.DoLayout(_owner.ParentInternal, _owner, PropertyNames.Links);
                _owner.AdjustSize();
                _owner.Invalidate();
            }

            if (_owner.Links.Count > 1)
            {
                _owner._links.Sort(s_linkComparer);
            }

            _owner.ValidateNoOverlappingLinks();
            _owner.UpdateSelectability();
            _owner.InvalidateTextLayout();
            _owner.Invalidate();

            if (_owner.Links.Count > 1)
            {
                return IndexOf(value);
            }
            else
            {
                return 0;
            }
        }

        int IList.Add(object? value)
        {
            if (value is Link link)
            {
                return Add(link);
            }
            else
            {
                throw new ArgumentException(SR.LinkLabelBadLink, nameof(value));
            }
        }

        void IList.Insert(int index, object? value)
        {
            if (value is Link link)
            {
                Add(link);
            }
            else
            {
                throw new ArgumentException(SR.LinkLabelBadLink, nameof(value));
            }
        }

        public bool Contains(Link link)
        {
            return _owner._links.Contains(link);
        }

        /// <summary>
        ///  Returns true if the collection contains an item with the specified key, false otherwise.
        /// </summary>
        public virtual bool ContainsKey(string? key)
        {
            return IsValidIndex(IndexOfKey(key));
        }

        bool IList.Contains(object? value)
        {
            if (value is Link link)
            {
                return Contains(link);
            }
            else
            {
                return false;
            }
        }

        public int IndexOf(Link link)
        {
            return _owner._links.IndexOf(link);
        }

        int IList.IndexOf(object? value)
        {
            if (value is Link link)
            {
                return IndexOf(link);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
        /// </summary>
        public virtual int IndexOfKey(string? key)
        {
            // Step 0 - Arg validation
            if (string.IsNullOrEmpty(key))
            {
                return -1; // we don't support empty or null keys.
            }

            // step 1 - check the last cached item
            if (IsValidIndex(_lastAccessedIndex))
            {
                if (WindowsFormsUtils.SafeCompareStrings(this[_lastAccessedIndex].Name, key, /* ignoreCase = */ true))
                {
                    return _lastAccessedIndex;
                }
            }

            // step 2 - search for the item
            for (int i = 0; i < Count; i++)
            {
                if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true))
                {
                    _lastAccessedIndex = i;
                    return i;
                }
            }

            // step 3 - we didn't find it. Invalidate the last accessed index and return -1.
            _lastAccessedIndex = -1;
            return -1;
        }

        /// <summary>
        ///  Determines if the index is valid for the collection.
        /// </summary>
        private bool IsValidIndex(int index)
        {
            return ((index >= 0) && (index < Count));
        }

        /// <summary>
        ///  Remove all links from the linkLabel.
        /// </summary>
        public virtual void Clear()
        {
            bool doLayout = _owner._links.Count > 0 && _owner.AutoSize;
            _owner._links.Clear();

            if (doLayout)
            {
                LayoutTransaction.DoLayout(_owner.ParentInternal, _owner, PropertyNames.Links);
                _owner.AdjustSize();
                _owner.Invalidate();
            }

            _owner.UpdateSelectability();
            _owner.InvalidateTextLayout();
            _owner.Invalidate();
        }

        void ICollection.CopyTo(Array dest, int index)
        {
            ((ICollection)_owner._links).CopyTo(dest, index);
        }

        public IEnumerator GetEnumerator()
        {
            if (_owner._links is not null)
            {
                return _owner._links.GetEnumerator();
            }
            else
            {
                return Array.Empty<Link>().GetEnumerator();
            }
        }

        public void Remove(Link value)
        {
            if (value.Owner != _owner)
            {
                return;
            }

            _owner._links.Remove(value);

            if (_owner.AutoSize)
            {
                LayoutTransaction.DoLayout(_owner.ParentInternal, _owner, PropertyNames.Links);
                _owner.AdjustSize();
                _owner.Invalidate();
            }

            _owner._links.Sort(s_linkComparer);

            _owner.ValidateNoOverlappingLinks();
            _owner.UpdateSelectability();
            _owner.InvalidateTextLayout();
            _owner.Invalidate();

            if (_owner.FocusLink is null && _owner._links.Count > 0)
            {
                _owner.FocusLink = _owner._links[0];
            }
        }

        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

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

        void IList.Remove(object? value)
        {
            if (value is Link link)
            {
                Remove(link);
            }
        }
    }
}
