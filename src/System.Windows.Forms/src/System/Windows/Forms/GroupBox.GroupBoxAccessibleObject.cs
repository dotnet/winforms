﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class GroupBox
{
    internal class GroupBoxAccessibleObject : ControlAccessibleObject
    {
        internal GroupBoxAccessibleObject(GroupBox owner) : base(owner)
        {
        }

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.Grouping);

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.IsKeyboardFocusablePropertyId => true,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
