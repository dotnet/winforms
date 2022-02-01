// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class DomainUpDown
    {
        public class DomainItemAccessibleObject : AccessibleObject
        {
            private string? _name;
            private readonly DomainItemListAccessibleObject _parent;

            public DomainItemAccessibleObject(string? name, AccessibleObject parent)
            {
                _name = name;
                _parent = (DomainItemListAccessibleObject)parent.OrThrowIfNull();
            }

            public override string? Name
            {
                get
                {
                    return _name;
                }
                set
                {
                    _name = value;
                }
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return _parent;
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

            public override string? Value
            {
                get
                {
                    return _name;
                }
            }

            internal override int[] RuntimeId => new int[] { RuntimeIDFirstItem, Parent.GetHashCode(), GetHashCode() };
        }
    }
}
