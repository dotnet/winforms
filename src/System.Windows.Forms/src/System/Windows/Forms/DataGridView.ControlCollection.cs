// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        [ComVisible(false)]
        public class DataGridViewControlCollection : ControlCollection
        {
            readonly DataGridView owner;

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
                if (value != owner.horizScrollBar && value != owner.vertScrollBar && value != owner.editingPanel)
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
                for (int i = 0; i < Count; i++)
                {
                    if (this[i] == owner.horizScrollBar || this[i] == owner.vertScrollBar || this[i] == owner.editingPanel)
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
