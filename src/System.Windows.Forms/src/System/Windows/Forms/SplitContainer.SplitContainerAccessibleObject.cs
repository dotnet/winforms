// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class SplitContainer
{
    internal class SplitContainerAccessibleObject : ControlAccessibleObject
    {
        public SplitContainerAccessibleObject(SplitContainer owner) : base(owner)
        {
        }

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
           => propertyID switch
           {
               UiaCore.UIA.AutomationIdPropertyId when this.TryGetOwnerAs(out SplitContainer? owner) => owner.Name,
               UiaCore.UIA.HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out SplitContainer? owner) && owner.Focused,
               UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
               _ => base.GetPropertyValue(propertyID)
           };
    }
}
