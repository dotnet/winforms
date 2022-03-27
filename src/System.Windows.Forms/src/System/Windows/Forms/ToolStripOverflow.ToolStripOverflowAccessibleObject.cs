// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class ToolStripOverflow
    {
        internal class ToolStripOverflowAccessibleObject : ToolStripAccessibleObject
        {
            public ToolStripOverflowAccessibleObject(ToolStripOverflow owner)
                : base(owner)
            {
            }

            public override AccessibleObject GetChild(int index)
            {
                return ((ToolStripOverflow)Owner).DisplayedItems[index].AccessibilityObject;
            }

            public override int GetChildCount()
            {
                return ((ToolStripOverflow)Owner).DisplayedItems.Count;
            }
        }
    }
}
