// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ToolStripContainer
{
    internal class ToolStripContainerAccessibleObject : ControlAccessibleObject
    {
        public ToolStripContainerAccessibleObject(ToolStripContainer owner) : base(owner)
        {
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
           => propertyID switch
           {
               UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(this.TryGetOwnerAs(out ToolStripContainer? owner) && owner.Focused),
               UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
               _ => base.GetPropertyValue(propertyID)
           };
    }
}
