// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public partial class DomainUpDown
    {
        internal class DomainItemListAccessibleObject : AccessibleObject
        {
            private readonly DomainUpDownAccessibleObject parent;

            public DomainItemListAccessibleObject(DomainUpDownAccessibleObject parent) : base()
            {
                this.parent = parent;
            }

            public override string Name
            {
                get
                {
                    string baseName = base.Name;
                    if (baseName is null || baseName.Length == 0)
                    {
                        return "Items";
                    }

                    return baseName;
                }
                set => base.Name = value;
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return parent;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.List;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    return AccessibleStates.Invisible | AccessibleStates.Offscreen;
                }
            }

            public override AccessibleObject GetChild(int index)
            {
                if (index >= 0 && index < GetChildCount())
                {
                    return new DomainItemAccessibleObject(((DomainUpDown)parent.Owner).Items[index].ToString(), this);
                }

                return null;
            }

            public override int GetChildCount()
            {
                return ((DomainUpDown)parent.Owner).Items.Count;
            }
        }
    }
}
