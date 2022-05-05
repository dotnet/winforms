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
            private readonly ToolStripOverflow _owningToolStripOverflow;

            public ToolStripOverflowAccessibleObject(ToolStripOverflow owner) : base(owner)
            {
                _owningToolStripOverflow = owner;
            }

            public override AccessibleObject? GetChild(int index)
                => index >= 0 || index < _owningToolStripOverflow.DisplayedItems.Count
                    ? _owningToolStripOverflow.DisplayedItems[index].AccessibilityObject
                    : null;

            public override int GetChildCount()
                => _owningToolStripOverflow.DisplayedItems.Count;

            internal override Interop.UiaCore.IRawElementProviderFragment? FragmentNavigate(NavigateDirection direction)
                => direction switch
                {
                    NavigateDirection.Parent => _owningToolStripOverflow.ownerItem.AccessibilityObject,
                    NavigateDirection.FirstChild => GetChildCount() > 0 ? _owningToolStripOverflow.DisplayedItems[0].AccessibilityObject : null,
                    NavigateDirection.LastChild => GetChildCount() > 0 ? _owningToolStripOverflow.DisplayedItems[^1].AccessibilityObject : null,
                    _ => base.FragmentNavigate(direction),
                };
        }
    }
}
