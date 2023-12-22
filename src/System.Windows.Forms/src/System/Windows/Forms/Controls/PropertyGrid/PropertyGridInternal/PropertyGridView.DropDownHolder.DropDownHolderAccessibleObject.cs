// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.PropertyGridInternal.PropertyDescriptorGridEntry;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyGridView
{
    internal partial class DropDownHolder
    {
        internal sealed class DropDownHolderAccessibleObject : ControlAccessibleObject
        {
            private readonly DropDownHolder _owningDropDownHolder;

            public DropDownHolderAccessibleObject(DropDownHolder dropDownHolder) : base(dropDownHolder)
            {
                _owningDropDownHolder = dropDownHolder;
            }

            internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
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
                    NavigateDirection.NavigateDirection_Parent => parent,
                    NavigateDirection.NavigateDirection_NextSibling => parent.GetNextChild(this),
                    NavigateDirection.NavigateDirection_PreviousSibling => parent.GetPreviousChild(this),
                    NavigateDirection.NavigateDirection_FirstChild or NavigateDirection.NavigateDirection_LastChild
                        when selectedEntry.Enumerable && _owningDropDownHolder.Component == gridView!.DropDownListBox
                        => gridView.DropDownListBoxAccessibleObject,
                    _ => base.FragmentNavigate(direction),
                };
            }

            internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot =>
                _owningDropDownHolder._gridView?.AccessibilityObject;

            public override string? Name => SR.PropertyGridViewDropDownControlHolderAccessibleName;

            private protected override bool IsInternal => true;

            internal override bool CanGetNameInternal => false;

            private bool ExistsInAccessibleTree => _owningDropDownHolder.IsHandleCreated && _owningDropDownHolder.Visible;
        }
    }
}
