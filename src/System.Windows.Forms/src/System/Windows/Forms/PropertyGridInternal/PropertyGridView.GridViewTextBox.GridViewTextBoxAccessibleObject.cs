// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static System.Windows.Forms.PropertyGridInternal.PropertyDescriptorGridEntry;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyGridView
{
    private partial class GridViewTextBox
    {
        private class GridViewTextBoxAccessibleObject : TextBoxBaseAccessibleObject
        {
            public GridViewTextBoxAccessibleObject(GridViewTextBox owner) : base(owner)
            {
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates states = base.State;
                    if (IsReadOnly)
                    {
                        states |= AccessibleStates.ReadOnly;
                    }
                    else
                    {
                        states &= ~AccessibleStates.ReadOnly;
                    }

                    return states;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (!this.TryGetOwnerAs(out GridViewTextBox? owner)
                    || !owner.PropertyGridView.IsEditTextBoxCreated
                    // Created is set to false in WM_DESTROY, but the window Handle is released on NCDESTROY, which comes after DESTROY.
                    // But between these calls, AccessibleObject can be recreated and might cause memory leaks.
                    || !owner.PropertyGridView.OwnerGrid.Created
                    || owner.PropertyGridView.SelectedGridEntry?.AccessibilityObject is not PropertyDescriptorGridEntryAccessibleObject parent)
                {
                    return null;
                }

                return direction switch
                {
                    UiaCore.NavigateDirection.Parent => parent,
                    UiaCore.NavigateDirection.NextSibling => parent.GetNextChild(this),
                    UiaCore.NavigateDirection.PreviousSibling => parent.GetPreviousChild(this),
                    _ => base.FragmentNavigate(direction),
                };
            }

            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot
                => this.TryGetOwnerAs(out GridViewTextBox? owner)
                    ? owner.PropertyGridView.OwnerGrid?.AccessibilityObject
                    : null;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID) => propertyID switch
            {
                UiaCore.UIA.ClassNamePropertyId when this.TryGetOwnerAs(out object? owner) => owner.GetType().ToString(),
                UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.EditControlTypeId,
                UiaCore.UIA.HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out Control? owner) && owner.Focused,
                UiaCore.UIA.IsEnabledPropertyId => !IsReadOnly,
                _ => base.GetPropertyValue(propertyID)
            };

            internal override UiaCore.IRawElementProviderSimple? HostRawElementProvider
            {
                get
                {
                    // Prevent sending same runtime ID for all edit boxes. Individual edit in
                    // each row should have unique runtime ID to prevent incorrect announcement.
                    // For instance screen reader may announce row 2 for the third row edit
                    // as the same TextBox control is used both in row 2 and row 3.
                    return null;
                }
            }

            public override string? Name
            {
                get
                {
                    if (!this.TryGetOwnerAs(out GridViewTextBox? owner))
                    {
                        return base.Name;
                    }

                    return owner.AccessibleName is { } name
                        ? name
                        : owner.PropertyGridView.SelectedGridEntry?.AccessibilityObject.Name ?? base.Name;
                }
            }

            internal override int[] RuntimeId
                => !this.TryGetOwnerAs(out Control? owner) ? base.RuntimeId : new int[]
                {
                    RuntimeIDFirstItem,
                    PARAM.ToInt(owner.InternalHandle),
                    GetHashCode()
                };

            internal override bool IsReadOnly
                => !this.TryGetOwnerAs(out GridViewTextBox? owner)
                    || owner.PropertyGridView.SelectedGridEntry is not PropertyDescriptorGridEntry propertyDescriptorGridEntry
                    || propertyDescriptorGridEntry.IsPropertyReadOnly;
        }
    }
}
