// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public partial class DomainUpDown
    {
        public class DomainUpDownAccessibleObject : ControlAccessibleObject
        {
            private DomainItemListAccessibleObject _domainItemList;
            private readonly UpDownBase _owningDomainUpDown;

            /// <summary>
            /// </summary>
            public DomainUpDownAccessibleObject(DomainUpDown owner) : base(owner)
            {
                _owningDomainUpDown = owner;
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.RuntimeIdPropertyId:
                        return RuntimeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.BoundingRectanglePropertyId:
                        return Bounds;
                    case UiaCore.UIA.LegacyIAccessibleStatePropertyId:
                        return State;
                    case UiaCore.UIA.LegacyIAccessibleRolePropertyId:
                        return Role;
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            private DomainItemListAccessibleObject ItemList
            {
                get
                {
                    if (_domainItemList is null)
                    {
                        _domainItemList = new DomainItemListAccessibleObject(this);
                    }

                    return _domainItemList;
                }
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

                    return AccessibleRole.SpinButton;
                }
            }

            /// <summary>
            /// </summary>
            public override AccessibleObject GetChild(int index)
            {
                switch (index)
                {
                    // TextBox child
                    case 0:
                        return _owningDomainUpDown.TextBox.AccessibilityObject.Parent;
                    // Up/down buttons
                    case 1:
                        return _owningDomainUpDown.UpDownButtonsInternal.AccessibilityObject.Parent;
                    case 2:
                        return ItemList;
                    default:
                        return null;
                }
            }

            public override int GetChildCount()
            {
                return 3;
            }

            internal override int[] RuntimeId
            {
                get
                {
                    if (_owningDomainUpDown is null)
                    {
                        return base.RuntimeId;
                    }

                    // we need to provide a unique ID
                    // others are implementing this in the same manner
                    // first item is static - 0x2a (RuntimeIDFirstItem)
                    // second item can be anything, but here it is a hash

                    var runtimeId = new int[3];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = (int)(long)_owningDomainUpDown.InternalHandle;
                    runtimeId[2] = _owningDomainUpDown.GetHashCode();

                    return runtimeId;
                }
            }
        }
    }
}
