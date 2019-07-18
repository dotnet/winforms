// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    public class TableLayoutRowStyleCollection : TableLayoutStyleCollection
    {
        internal TableLayoutRowStyleCollection(IArrangedElement Owner) : base(Owner) { }
        internal TableLayoutRowStyleCollection() : base(null) { }

        internal override string PropertyName
        {
            get { return PropertyNames.RowStyles; }
        }

        public int Add(RowStyle rowStyle) { return ((IList)this).Add(rowStyle); }

        public void Insert(int index, RowStyle rowStyle) { ((IList)this).Insert(index, rowStyle); }

        public new RowStyle this[int index]
        {
            get { return (RowStyle)((IList)this)[index]; }
            set { ((IList)this)[index] = value; }
        }

        public void Remove(RowStyle rowStyle) { ((IList)this).Remove(rowStyle); }

        public bool Contains(RowStyle rowStyle) { return ((IList)this).Contains(rowStyle); }

        public int IndexOf(RowStyle rowStyle) { return ((IList)this).IndexOf(rowStyle); }
    }
}

