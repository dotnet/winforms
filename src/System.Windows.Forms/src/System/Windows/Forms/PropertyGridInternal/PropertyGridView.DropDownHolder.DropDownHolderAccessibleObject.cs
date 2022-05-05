// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static System.Windows.Forms.PropertyGridInternal.PropertyDescriptorGridEntry;
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
                    if (!ExistsInAccessibleTree)
                    {
                        return null;
                    }

                    PropertyGridView? gridView = _owningDropDownHolder._gridView;
                    GridEntry? selectedEntry = gridView?.SelectedGridEntry;
                    if (selectedEntry?.AccessibilityObject is not PropertyDescriptorGridEntryAccessibleObject parent)
                    {
                        return null;
                    }

                    return direction switch
                    {
                        UiaCore.NavigateDirection.Parent => parent,
                        UiaCore.NavigateDirection.NextSibling => parent.GetNextChild(this),
                        UiaCore.NavigateDirection.PreviousSibling => parent.GetPreviousChild(this),
                        UiaCore.NavigateDirection.FirstChild or UiaCore.NavigateDirection.LastChild
                            when selectedEntry.Enumerable && _owningDropDownHolder.Component == gridView!.DropDownListBox
                            => gridView.DropDownListBoxAccessibleObject,
                        _ => base.FragmentNavigate(direction),
                    };
                }

                internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot =>
                    _owningDropDownHolder._gridView?.AccessibilityObject;

                public override string? Name => SR.PropertyGridViewDropDownControlHolderAccessibleName;

                private bool ExistsInAccessibleTree
                    => _owningDropDownHolder.IsHandleCreated && _owningDropDownHolder.Visible;
            }
        }
    }
}
