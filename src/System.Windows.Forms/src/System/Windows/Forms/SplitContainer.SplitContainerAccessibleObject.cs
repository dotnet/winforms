﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class SplitContainer
{
    internal class SplitContainerAccessibleObject : ControlAccessibleObject
    {
        public SplitContainerAccessibleObject(SplitContainer owner) : base(owner)
        {
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
           => propertyID switch
           {
               UIA_PROPERTY_ID.UIA_AutomationIdPropertyId when this.TryGetOwnerAs(out SplitContainer? owner) => (VARIANT)owner.Name,
               UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(this.TryGetOwnerAs(out SplitContainer? owner) && owner.Focused),
               UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
               _ => base.GetPropertyValue(propertyID)
           };
    }
}
