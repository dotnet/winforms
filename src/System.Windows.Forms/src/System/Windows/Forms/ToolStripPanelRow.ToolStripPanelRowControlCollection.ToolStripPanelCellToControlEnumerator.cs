// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;

namespace System.Windows.Forms
{
    public partial class ToolStripPanelRow
    {
internal partial class ToolStripPanelRowControlCollection
        {
            ///  We want to pretend like we're only holding controls... so everywhere we've returned controls.
            ///  but the problem is if you do a foreach, you'll get the cells not the controls.  So we've got
            ///  to sort of write a wrapper class around the ArrayList enumerator.
            private class ToolStripPanelCellToControlEnumerator : IEnumerator, ICloneable
            {
                private readonly IEnumerator _arrayListEnumerator;

                internal ToolStripPanelCellToControlEnumerator(ArrayList list)
                {
                    _arrayListEnumerator = ((IEnumerable)list).GetEnumerator();
                }

                public virtual object Current
                {
                    get
                    {
                        ToolStripPanelCell cell = _arrayListEnumerator.Current as ToolStripPanelCell;
                        Debug.Assert(cell is not null, "Expected ToolStripPanel cells only!!!" + _arrayListEnumerator.Current.GetType().ToString());
                        return cell?.Control;
                    }
                }

                public object Clone()
                {
                    return MemberwiseClone();
                }

                public virtual bool MoveNext()
                {
                    return _arrayListEnumerator.MoveNext();
                }

                public virtual void Reset()
                {
                    _arrayListEnumerator.Reset();
                }
            }
        }
    }
}
