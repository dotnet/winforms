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

            // We need to provide a unique ID. Others are implementing this in the same manner. First item is static - 0x2a (RuntimeIDFirstItem).
            // Second item can be anything, but it's good to supply HWND.
            internal override int[] RuntimeId
                => new int[]
                {
                    RuntimeIDFirstItem,
                    PARAM.ToInt(_owningDomainUpDown.InternalHandle),
                    _owningDomainUpDown.GetHashCode()
                };
        }
    }
}
