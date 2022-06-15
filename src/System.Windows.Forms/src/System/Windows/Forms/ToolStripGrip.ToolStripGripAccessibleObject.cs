// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using static Interop;

namespace System.Windows.Forms
{
    internal partial class ToolStripGrip
    {
        internal class ToolStripGripAccessibleObject : ToolStripButtonAccessibleObject
        {
            public ToolStripGripAccessibleObject(ToolStripGrip owner) : base(owner)
            {
            }

            [AllowNull]
            public override string Name
            {
                get => Owner.AccessibleName ?? SR.ToolStripGripAccessibleName;
                set => base.Name = value;
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }

                    return AccessibleRole.Grip;
                }
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return false;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
