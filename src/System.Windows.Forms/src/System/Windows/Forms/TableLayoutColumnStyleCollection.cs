// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Design;    
    using System.Globalization;
    using System.Windows.Forms.Layout;
    using System.Reflection;
    /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="ColumnStyleCollection"]/*' />
    public class TableLayoutColumnStyleCollection : TableLayoutStyleCollection {


        internal TableLayoutColumnStyleCollection(IArrangedElement Owner) : base(Owner) {}
        internal TableLayoutColumnStyleCollection() : base(null) {}
        
        internal override string PropertyName {
            get { return PropertyNames.ColumnStyles; }
        }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="ColumnStyleCollection.Add"]/*' />
        public int Add(ColumnStyle columnStyle) { return ((IList)this).Add(columnStyle); }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="ColumnStyleCollection.Insert"]/*' />
        public void Insert(int index, ColumnStyle columnStyle) { ((IList)this).Insert(index, columnStyle); }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="ColumnStyleCollection.this"]/*' />
        public new ColumnStyle this[int index] {
            get { return (ColumnStyle)((IList)this)[index]; }
            set { ((IList)this)[index] = value; }
        }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="ColumnStyleCollection.Remove"]/*' />
        public void Remove(ColumnStyle columnStyle) { ((IList)this).Remove(columnStyle); }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="ColumnStyleCollection.Contains"]/*' />
        public bool Contains(ColumnStyle columnStyle) { return ((IList)this).Contains(columnStyle); }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="ColumnStyleCollection.IndexOf"]/*' />
        public int IndexOf(ColumnStyle columnStyle) { return ((IList)this).IndexOf(columnStyle); }
    }
}

