﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripPanel
    {
        internal class ToolStripPanelAccessibleObject : ControlAccessibleObject
        {
            private readonly ToolStripPanel _owningLabel;

            public ToolStripPanelAccessibleObject(ToolStripPanel owner) : base(owner)
            {
                _owningLabel = owner;
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = _owningLabel.AccessibleRole;

                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }

                    return AccessibleRole.Client;
                }
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyId)
                => propertyId switch
                {
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => Owner.CanFocus,
                    _ => base.GetPropertyValue(propertyId)
                };
        }
    }
}
