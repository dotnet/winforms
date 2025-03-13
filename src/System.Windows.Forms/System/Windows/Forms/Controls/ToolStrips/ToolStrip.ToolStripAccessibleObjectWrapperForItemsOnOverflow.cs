// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ToolStrip
{
    private class ToolStripAccessibleObjectWrapperForItemsOnOverflow : ToolStripItem.ToolStripItemAccessibleObject
    {
        public ToolStripAccessibleObjectWrapperForItemsOnOverflow(ToolStripItem item)
            : base(item)
        {
        }

        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = base.State;
                state |= AccessibleStates.Offscreen;
                state |= AccessibleStates.Invisible;
                return state;
            }
        }
    }
}
