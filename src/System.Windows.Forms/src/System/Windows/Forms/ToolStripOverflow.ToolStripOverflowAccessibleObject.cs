// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.UiaCore;

namespace System.Windows.Forms
{
    public partial class ToolStripOverflow
    {
        internal class ToolStripOverflowAccessibleObject : ToolStripDropDownAccessibleObject
        {
            public ToolStripOverflowAccessibleObject(ToolStripOverflow owner) : base(owner)
            { }

            public override AccessibleObject? GetChild(int index)
                => Owner is ToolStripOverflow overflow && (index >= 0 || index < overflow.DisplayedItems.Count)
                    ? overflow.DisplayedItems[index].AccessibilityObject
                    : null;

            public override int GetChildCount()
                => Owner is ToolStripOverflow overflow
                    ? overflow.DisplayedItems.Count
                    : 0;

            internal override IRawElementProviderFragment? FragmentNavigate(NavigateDirection direction)
                => direction switch
                {
                    NavigateDirection.Parent when Owner is ToolStripOverflow menu
                        => menu.OwnerItem?.AccessibilityObject,
                    _ => base.FragmentNavigate(direction),
                };
        }
    }
}
