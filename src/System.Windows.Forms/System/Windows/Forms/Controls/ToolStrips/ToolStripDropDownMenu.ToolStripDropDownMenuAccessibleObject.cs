// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ToolStripDropDownMenu : ToolStripDropDown
{
    internal class ToolStripDropDownMenuAccessibleObject : ToolStripDropDownAccessibleObject
    {
        public ToolStripDropDownMenuAccessibleObject(ToolStripDropDownMenu owner) : base(owner)
        { }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_Parent when this.TryGetOwnerAs(out ToolStripDropDownMenu? owner)
                    => owner.OwnerItem?.AccessibilityObject,
                _ => base.FragmentNavigate(direction)
            };

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_IsControlElementPropertyId => VARIANT.True,
                UIA_PROPERTY_ID.UIA_IsContentElementPropertyId => (VARIANT)this.TryGetOwnerAs(out ContextMenuStrip? _),
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
