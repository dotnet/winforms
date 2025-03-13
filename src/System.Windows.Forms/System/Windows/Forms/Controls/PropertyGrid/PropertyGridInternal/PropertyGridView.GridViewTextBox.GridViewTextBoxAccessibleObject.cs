// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.PropertyGridInternal.PropertyDescriptorGridEntry;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyGridView
{
    private partial class GridViewTextBox
    {
        private class GridViewTextBoxAccessibleObject : TextBoxBaseAccessibleObject
        {
            private int[]? _runtimeId;

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

            internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
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
                    NavigateDirection.NavigateDirection_Parent => parent,
                    NavigateDirection.NavigateDirection_NextSibling => parent.GetNextChild(this),
                    NavigateDirection.NavigateDirection_PreviousSibling => parent.GetPreviousChild(this),
                    _ => base.FragmentNavigate(direction),
                };
            }

            internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot =>
                this.TryGetOwnerAs(out GridViewTextBox? owner)
                    ? owner.PropertyGridView.OwnerGrid?.AccessibilityObject
                    : null;

            internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ClassNamePropertyId when this.TryGetOwnerAs(out object? owner) => (VARIANT)owner.GetType().ToString(),
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_EditControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(this.TryGetOwnerAs(out Control? owner) && owner.Focused),
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)!IsReadOnly,
                _ => base.GetPropertyValue(propertyID)
            };

            internal override unsafe IRawElementProviderSimple* HostRawElementProvider
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

            private protected override bool IsInternal => true;

            internal override int[] RuntimeId => _runtimeId ??= base.RuntimeId;

            internal override bool IsReadOnly
                => !this.TryGetOwnerAs(out GridViewTextBox? owner)
                    || owner.PropertyGridView.SelectedGridEntry is not PropertyDescriptorGridEntry propertyDescriptorGridEntry
                    || propertyDescriptorGridEntry.IsPropertyReadOnly;
        }
    }
}
