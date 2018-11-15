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
    
    /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="RowStyleCollection"]/*' />
    public class TableLayoutRowStyleCollection : TableLayoutStyleCollection {

        internal TableLayoutRowStyleCollection(IArrangedElement Owner) : base(Owner) {}
        internal TableLayoutRowStyleCollection() : base(null) {}

        internal override string PropertyName {
            get { return PropertyNames.RowStyles; }
        }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="RowStyleCollection.Add"]/*' />
        public int Add(RowStyle rowStyle) { return ((IList)this).Add(rowStyle); }
        
        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="RowStyleCollection.Insert"]/*' />
        public void Insert(int index, RowStyle rowStyle) { ((IList)this).Insert(index, rowStyle); }
        
        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="RowStyleCollection.this"]/*' />
        public new RowStyle this[int index] {
            get { return (RowStyle)((IList)this)[index]; }
            set { ((IList)this)[index] = value; }
        }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="RowStyleCollection.Remove"]/*' />
        public void Remove(RowStyle rowStyle) { ((IList)this).Remove(rowStyle); }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="RowStyleCollection.Contains"]/*' />
        public bool Contains(RowStyle rowStyle) { return ((IList)this).Contains(rowStyle); }

        /// <include file='doc\TableLayoutSettings.uex' path='docs/doc[@for="RowStyleCollection.IndexOf"]/*' />
        public int IndexOf(RowStyle rowStyle) { return ((IList)this).IndexOf(rowStyle); }
    }
}

