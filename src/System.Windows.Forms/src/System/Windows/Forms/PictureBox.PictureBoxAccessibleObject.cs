﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class PictureBox
    {
        internal class PictureBoxAccessibleObject : ControlAccessibleObject
        {
            public PictureBoxAccessibleObject(PictureBox owner) : base(owner)
            {
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.NamePropertyId
                        => Name,
                    UiaCore.UIA.ControlTypePropertyId
                        // If we don't set a default role for the accessible object
                        // it will be retrieved from Windows.
                        // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                        => Owner.AccessibleRole == AccessibleRole.Default
                           ? UiaCore.UIA.PaneControlTypeId
                           : base.GetPropertyValue(propertyID),
                    UiaCore.UIA.AutomationIdPropertyId
                        => Owner.Name,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId
                        => true,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
