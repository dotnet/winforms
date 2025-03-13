// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ToolStripOverflow
{
    internal sealed class ToolStripOverflowAccessibleObject : ToolStripDropDownAccessibleObject
    {
        public ToolStripOverflowAccessibleObject(ToolStripOverflow owner) : base(owner)
        { }

        public override AccessibleObject? GetChild(int index)
            => this.TryGetOwnerAs(out ToolStripOverflow? owner) && (index >= 0 || index < owner.DisplayedItems.Count)
                ? owner.DisplayedItems[index].AccessibilityObject
                : null;

        private protected override bool IsInternal => true;

        public override int GetChildCount()
            => this.TryGetOwnerAs(out ToolStripOverflow? owner)
                ? owner.DisplayedItems.Count
                : 0;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_Parent when this.TryGetOwnerAs(out ToolStripOverflow? owner)
                    => owner.OwnerItem?.AccessibilityObject,
                _ => base.FragmentNavigate(direction),
            };

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_IsControlElementPropertyId => VARIANT.True,
                UIA_PROPERTY_ID.UIA_IsContentElementPropertyId => VARIANT.False,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
