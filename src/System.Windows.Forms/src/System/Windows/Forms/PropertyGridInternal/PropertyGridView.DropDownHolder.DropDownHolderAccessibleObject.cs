// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        internal partial class DropDownHolder
        {
            internal class DropDownHolderAccessibleObject : ControlAccessibleObject
            {
                private readonly DropDownHolder _owningDropDownHolder;

                public DropDownHolderAccessibleObject(DropDownHolder dropDownHolder) : base(dropDownHolder)
                {
                    _owningDropDownHolder = dropDownHolder;
                }

                internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                {
                    switch (direction)
                    {
                        case UiaCore.NavigateDirection.Parent:
                            return ExistsInAccessibleTree
                                ? _owningDropDownHolder.gridView?.SelectedGridEntry?.AccessibilityObject
                                : null;
                        case UiaCore.NavigateDirection.NextSibling:
                            return ExistsInAccessibleTree
                                ? _owningDropDownHolder.gridView?.EditAccessibleObject
                                : null;
                        case UiaCore.NavigateDirection.PreviousSibling:
                            return null;
                    }

                    return base.FragmentNavigate(direction);
                }

                internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot =>
                    _owningDropDownHolder.gridView?.OwnerGrid?.AccessibilityObject;

                internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                {
                    if (propertyID == UiaCore.UIA.NamePropertyId)
                    {
                        return SR.PropertyGridViewDropDownControlHolderAccessibleName;
                    }

                    return base.GetPropertyValue(propertyID);
                }

                private bool ExistsInAccessibleTree =>
                    _owningDropDownHolder.IsHandleCreated && _owningDropDownHolder.Visible;
            }
        }
    }
}
