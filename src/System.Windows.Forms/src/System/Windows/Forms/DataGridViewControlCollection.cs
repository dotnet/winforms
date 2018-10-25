// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        /// <include file='doc\DataGridView.uex' path='docs/doc[@for="DataGridView.DataGridViewControlCollection"]/*' />
        [
            ComVisible(false),
            SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")    // Consider adding an IList<DataGridViewControlCollection> implementation
        ]
        public class DataGridViewControlCollection : Control.ControlCollection
        {
            DataGridView owner;

            /// <include file='doc\DataGridViewControlCollection.uex' path='docs/doc[@for="DataGridView.DataGridViewControlCollection.DataGridViewControlCollection"]/*' />
            public DataGridViewControlCollection(DataGridView owner)
                : base(owner)
            {
                this.owner = owner;
            }

            /// <include file='doc\DataGridViewControlCollection.uex' path='docs/doc[@for="DataGridView.DataGridViewControlCollection.CopyTo"]/*' />
            public void CopyTo(Control[] array, int index)
            {
                base.CopyTo(array, index);
            }

            /// <include file='doc\DataGridViewControlCollection.uex' path='docs/doc[@for="DataGridView.DataGridViewControlCollection.Insert"]/*' />
            public void Insert(int index, Control value)
            {
                ((IList)this).Insert(index, (object)value);
            }

            /// <include file='doc\DataGridViewControlCollection.uex' path='docs/doc[@for="DataGridView.DataGridViewControlCollection.Remove"]/*' />
            public override void Remove(Control value)
            {
                if (value != owner.horizScrollBar && value != owner.vertScrollBar && value != this.owner.editingPanel)
                {
                    base.Remove(value);
                }
            }

            internal void RemoveInternal(Control value)
            {
                base.Remove(value);
            }

            /// <include file='doc\DataGridViewControlCollection.uex' path='docs/doc[@for="DataGridView.DataGridViewControlCollection.Clear"]/*' />
            public override void Clear()
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i] == this.owner.horizScrollBar || this[i] == this.owner.vertScrollBar || this[i] == this.owner.editingPanel)
                    {
                        continue;
                    }
                    else
                    {
                        Remove(this[i]);
                    }
                }
            }
        }
    }
}