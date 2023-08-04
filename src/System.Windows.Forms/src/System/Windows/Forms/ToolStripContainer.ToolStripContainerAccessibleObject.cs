﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class ToolStripContainer
{
    internal class ToolStripContainerAccessibleObject : ControlAccessibleObject
    {
        public ToolStripContainerAccessibleObject(ToolStripContainer owner) : base(owner)
        {
        }

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
           => propertyID switch
           {
               UiaCore.UIA.HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out ToolStripContainer? owner) && owner.Focused,
               UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
               _ => base.GetPropertyValue(propertyID)
           };
    }
}
