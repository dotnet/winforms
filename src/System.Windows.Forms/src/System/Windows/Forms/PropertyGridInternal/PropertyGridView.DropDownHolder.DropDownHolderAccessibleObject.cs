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
                    => direction switch
                    {
                        UiaCore.NavigateDirection.Parent => ExistsInAccessibleTree
                            ? _owningDropDownHolder._gridView?.SelectedGridEntry?.AccessibilityObject
                            : null,
                        UiaCore.NavigateDirection.NextSibling => ExistsInAccessibleTree
                            ? _owningDropDownHolder._gridView?.EditAccessibleObject
                            : null,
                        UiaCore.NavigateDirection.PreviousSibling => null,
                        _ => base.FragmentNavigate(direction),
                    };

                internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot =>
                    _owningDropDownHolder._gridView?.OwnerGrid?.AccessibilityObject;

                public override string? Name => SR.PropertyGridViewDropDownControlHolderAccessibleName;

                private bool ExistsInAccessibleTree
                    => _owningDropDownHolder.IsHandleCreated && _owningDropDownHolder.Visible;
            }
        }
    }
}
