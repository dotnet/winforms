// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public partial class DomainUpDown
    {
        public class DomainItemAccessibleObject : AccessibleObject
        {
            private string name;
            private readonly DomainItemListAccessibleObject parent;

            public DomainItemAccessibleObject(string name, AccessibleObject parent) : base()
            {
                this.name = name;
                this.parent = (DomainItemListAccessibleObject)parent;
            }

            public override string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
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
                    return AccessibleRole.ListItem;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    return AccessibleStates.Selectable;
                }
            }

            public override string Value
            {
                get
                {
                    return name;
                }
            }
        }
    }
}
