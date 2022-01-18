// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripMenuItem
    {
        /// <summary>
        ///  An implementation of AccessibleChild for use with ToolStripItems
        /// </summary>
        internal class ToolStripMenuItemAccessibleObject : ToolStripDropDownItemAccessibleObject
        {
            private readonly ToolStripMenuItem _owningToolStripMenuItem;

            public ToolStripMenuItemAccessibleObject(ToolStripMenuItem ownerItem) : base(ownerItem)
            {
                _owningToolStripMenuItem = ownerItem;
            }

            public override AccessibleStates State
            {
                get
                {
                    if (_owningToolStripMenuItem.Enabled)
                    {
                        AccessibleStates state = base.State;

                        if ((state & AccessibleStates.Pressed) == AccessibleStates.Pressed)
                        {
                            // for some reason menu items are never "pressed".
                            state &= ~AccessibleStates.Pressed;
                        }

                        if (_owningToolStripMenuItem.Checked)
                        {
                            state |= AccessibleStates.Checked;
                        }

                        return state;
                    }

                    return base.State;
                }
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.AcceleratorKeyPropertyId)
                {
                    return _owningToolStripMenuItem.GetShortcutText();
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
