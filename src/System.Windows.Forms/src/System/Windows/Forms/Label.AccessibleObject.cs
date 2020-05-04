// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Label
    {
        [ComVisible(true)]
        internal class LabelAccessibleObject : ControlAccessibleObject
        {
            public LabelAccessibleObject(Label owner) : base(owner)
            {
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = OwningLabel.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }

                    return AccessibleRole.StaticText;
                }
            }

            private Label OwningLabel => Owner as Label;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.AutomationIdPropertyId
                        => OwningLabel.IsHandleCreated ? OwningLabel.Name : string.Empty,
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.TextControlTypeId,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
