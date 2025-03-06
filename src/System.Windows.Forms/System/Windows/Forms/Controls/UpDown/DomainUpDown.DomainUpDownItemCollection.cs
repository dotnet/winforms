// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public partial class DomainUpDown
{
    /// <summary>
    ///  Encapsulates a collection of objects for use by the <see cref="DomainUpDown"/>
    ///  class.
    /// </summary>
    public class DomainUpDownItemCollection : ArrayList
    {
        private readonly DomainUpDown _owner;

        internal DomainUpDownItemCollection(DomainUpDown owner)
            : base()
        {
            _owner = owner;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override object? this[int index]
        {
            get
            {
                return base[index];
            }

            set
            {
                base[index] = value;

                if (_owner.SelectedIndex == index)
                {
                    _owner.SelectIndex(index);
                }

                if (_owner.Sorted)
                {
                    _owner.SortDomainItems();
                }
            }
        }

        /// <summary>
        /// </summary>
        public override int Add(object? item)
        {
            // Overridden to perform sorting after adding an item

            int ret = base.Add(item);
            if (_owner.Sorted)
            {
                _owner.SortDomainItems();
            }

            return ret;
        }

        /// <summary>
        /// </summary>
        public override void Remove(object? item)
        {
            int index = IndexOf(item);

            if (index == -1)
            {
                throw new ArgumentOutOfRangeException(nameof(item), item, string.Format(SR.InvalidArgument, nameof(item), item));
            }
            else
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        /// </summary>
        public override void RemoveAt(int item)
        {
            // Overridden to update the domain index if necessary
            base.RemoveAt(item);

            if (item < _owner._domainIndex)
            {
                // The item removed was before the currently selected item
                _owner.SelectIndex(_owner._domainIndex - 1);
            }
            else if (item == _owner._domainIndex)
            {
                // The currently selected item was removed
                _owner.SelectIndex(-1);
            }
        }

        /// <summary>
        /// </summary>
        public override void Insert(int index, object? item)
        {
            base.Insert(index, item);
            if (_owner.Sorted)
            {
                _owner.SortDomainItems();
            }
        }
    }
}
