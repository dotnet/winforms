// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        public class DataGridViewControlCollection : ControlCollection
        {
            private readonly DataGridView _owner;

            public DataGridViewControlCollection(DataGridView owner)
                : base(owner)
            {
                _owner = owner;
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
                if (value != _owner._horizScrollBar && value != _owner._vertScrollBar && value != _owner._editingPanel)
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
                    if (this[i] == _owner._horizScrollBar || this[i] == _owner._vertScrollBar || this[i] == _owner._editingPanel)
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
