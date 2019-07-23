// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    public class TableLayoutColumnStyleCollection : TableLayoutStyleCollection
    {
        internal TableLayoutColumnStyleCollection(IArrangedElement Owner) : base(Owner) { }
        internal TableLayoutColumnStyleCollection() : base(null) { }

        internal override string PropertyName
        {
            get { return PropertyNames.ColumnStyles; }
        }

        public int Add(ColumnStyle columnStyle) { return ((IList)this).Add(columnStyle); }

        public void Insert(int index, ColumnStyle columnStyle) { ((IList)this).Insert(index, columnStyle); }

        public new ColumnStyle this[int index]
        {
            get { return (ColumnStyle)((IList)this)[index]; }
            set { ((IList)this)[index] = value; }
        }

        public void Remove(ColumnStyle columnStyle) { ((IList)this).Remove(columnStyle); }

        public bool Contains(ColumnStyle columnStyle) { return ((IList)this).Contains(columnStyle); }

        public int IndexOf(ColumnStyle columnStyle) { return ((IList)this).IndexOf(columnStyle); }
    }
}

