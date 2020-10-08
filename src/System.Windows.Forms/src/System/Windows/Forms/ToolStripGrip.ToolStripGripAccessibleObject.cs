// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    internal partial class ToolStripGrip
    {
        internal class ToolStripGripAccessibleObject : ToolStripButtonAccessibleObject
        {
            private string stockName;

            public ToolStripGripAccessibleObject(ToolStripGrip owner) : base(owner)
            {
            }

            public override string Name
            {
                get
                {
                    string name = Owner.AccessibleName;
                    if (name != null)
                    {
                        return name;
                    }
                    if (string.IsNullOrEmpty(stockName))
                    {
                        stockName = SR.ToolStripGripAccessibleName;
                    }
                    return stockName;
                }
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

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return false;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.ThumbControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
