// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class Label
    {
        internal class LabelAccessibleObject : ControlAccessibleObject
        {
            private readonly Label _owningLabel;

            public LabelAccessibleObject(Label owner) : base(owner)
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

                    return AccessibleRole.StaticText;
                }
            }
        }
    }
}
