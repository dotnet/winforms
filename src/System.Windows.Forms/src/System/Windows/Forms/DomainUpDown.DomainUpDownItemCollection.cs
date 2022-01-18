// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
    public partial class DomainUpDown
    {
        /// <summary>
        ///  Encapsulates a collection of objects for use by the <see cref="DomainUpDown"/>
        ///  class.
        /// </summary>
        public class DomainUpDownItemCollection : ArrayList
        {
            readonly DomainUpDown owner;

            internal DomainUpDownItemCollection(DomainUpDown owner)
            : base()
            {
                this.owner = owner;
            }

            [Browsable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override object this[int index]
            {
                get
                {
                    return base[index];
                }

                set
                {
                    base[index] = value;

                    if (owner.SelectedIndex == index)
                    {
                        owner.SelectIndex(index);
                    }

                    if (owner.Sorted)
                    {
                        owner.SortDomainItems();
                    }
                }
            }

            /// <summary>
            /// </summary>
            public override int Add(object item)
            {
                // Overridden to perform sorting after adding an item

                int ret = base.Add(item);
                if (owner.Sorted)
                {
                    owner.SortDomainItems();
                }

                return ret;
            }

            /// <summary>
            /// </summary>
            public override void Remove(object item)
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

                if (item < owner._domainIndex)
                {
                    // The item removed was before the currently selected item
                    owner.SelectIndex(owner._domainIndex - 1);
                }
                else if (item == owner._domainIndex)
                {
                    // The currently selected item was removed
                    //
                    owner.SelectIndex(-1);
                }
            }

            /// <summary>
            /// </summary>
            public override void Insert(int index, object item)
            {
                base.Insert(index, item);
                if (owner.Sorted)
                {
                    owner.SortDomainItems();
                }
            }
        }
    }
}
