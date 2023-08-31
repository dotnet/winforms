// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class ToolStripDropDownMenu : ToolStripDropDown
{
    internal class ToolStripDropDownMenuAccessibleObject : ToolStripDropDownAccessibleObject
    {
        public ToolStripDropDownMenuAccessibleObject(ToolStripDropDownMenu owner) : base(owner)
        { }

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            => direction switch
            {
                UiaCore.NavigateDirection.Parent when this.TryGetOwnerAs(out ToolStripDropDownMenu? owner)
                    => owner.OwnerItem?.AccessibilityObject,
                _ => base.FragmentNavigate(direction)
            };

        internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
            propertyID switch
            {
                UiaCore.UIA.IsControlElementPropertyId => true,
                UiaCore.UIA.IsContentElementPropertyId => this.TryGetOwnerAs(out ContextMenuStrip? _),
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
