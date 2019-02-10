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

        [
            ComVisible(false),
            SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")    // Consider adding an IList<DataGridViewControlCollection> implementation
        ]
        public class DataGridViewControlCollection : Control.ControlCollection
        {
            DataGridView owner;


            public DataGridViewControlCollection(DataGridView owner)
                : base(owner)
            {
                this.owner = owner;
            }


            public void CopyTo(Control[] array, int index)
            {
                base.CopyTo(array, index);
            }


            public void Insert(int index, Control value)
            {
                ((IList)this).Insert(index, (object)value);
            }


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
