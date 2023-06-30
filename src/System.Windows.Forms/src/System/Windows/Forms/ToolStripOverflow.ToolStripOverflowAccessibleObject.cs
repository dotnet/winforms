// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.UiaCore;

namespace System.Windows.Forms;

public partial class ToolStripOverflow
{
    internal class ToolStripOverflowAccessibleObject : ToolStripDropDownAccessibleObject
    {
        public ToolStripOverflowAccessibleObject(ToolStripOverflow owner) : base(owner)
        { }

        public override AccessibleObject? GetChild(int index)
            => this.TryGetOwnerAs(out ToolStripOverflow? owner) && (index >= 0 || index < owner.DisplayedItems.Count)
                ? owner.DisplayedItems[index].AccessibilityObject
                : null;

        public override int GetChildCount()
            => this.TryGetOwnerAs(out ToolStripOverflow? owner)
                ? owner.DisplayedItems.Count
                : 0;

        internal override IRawElementProviderFragment? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.Parent when this.TryGetOwnerAs(out ToolStripOverflow? owner)
                    => owner.OwnerItem?.AccessibilityObject,
                _ => base.FragmentNavigate(direction),
            };

        internal override object? GetPropertyValue(UIA propertyID)
            => propertyID switch
            {
                UIA.IsControlElementPropertyId => true,
                UIA.IsContentElementPropertyId => false,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
