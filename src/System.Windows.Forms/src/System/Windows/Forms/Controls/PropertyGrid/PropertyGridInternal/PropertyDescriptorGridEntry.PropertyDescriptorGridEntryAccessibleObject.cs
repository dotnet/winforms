// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyDescriptorGridEntry
{
    internal class PropertyDescriptorGridEntryAccessibleObject : GridEntryAccessibleObject
    {
        public PropertyDescriptorGridEntryAccessibleObject(PropertyDescriptorGridEntry owner) : base(owner)
        {
        }

        internal override ExpandCollapseState ExpandCollapseState
            => GetPropertyGridView() is { } propertyGridView
                && this.TryGetOwnerAs(out PropertyDescriptorGridEntry? owner)
                && owner == propertyGridView.SelectedGridEntry
                && (owner.InternalExpanded || propertyGridView.DropDownVisible)
                    ? ExpandCollapseState.ExpandCollapseState_Expanded
                    : ExpandCollapseState.ExpandCollapseState_Collapsed;

        public override AccessibleObject? GetChild(int index)
        {
            Debug.Assert(index >= 0);

            if (!this.TryGetOwnerAs(out PropertyDescriptorGridEntry? owner))
            {
                return null;
            }

            // Child controls exist in tree only when the entry is selected.
            if (GetPropertyGridView() is { } propertyGridView && propertyGridView.SelectedGridEntry == owner)
            {
                // DropDownControlHolder exists in the tree if the drop-down holder is visible.
                if (propertyGridView.DropDownVisible)
                {
                    if (index == 0)
                    {
                        return propertyGridView.DropDownControlHolder.AccessibilityObject;
                    }

                    index--;
                }

                // TextBox exists in the tree if it's created.
                if (propertyGridView.IsEditTextBoxCreated)
                {
                    if (index == 0)
                    {
                        return propertyGridView.EditAccessibleObject;
                    }

                    index--;
                }

                if (propertyGridView.DropDownButton is { Visible: true } dropDownButton)
                {
                    // DropDownButton exists in the tree if the drop-down button is visible.
                    if (index == 0)
                    {
                        return dropDownButton.AccessibilityObject;
                    }

                    index--;
                }
                else if (propertyGridView.DialogButton is { Visible: true } dialogButton)
                {
                    // DialogButton exists in the tree if the ellipsis button is visible.
                    if (index == 0)
                    {
                        return dialogButton.AccessibilityObject;
                    }

                    index--;
                }
            }

            // Child entries exist in the tree if the entry has child entries and is expanded.
            if (owner is { ChildCount: > 0, Expanded: true })
            {
                if (index < owner.ChildCount)
                {
                    return owner.Children[index].AccessibilityObject;
                }

                // Uncomment the following line in case there will be another children going after the child entries:
                // index -= _owningPropertyDescriptorGridEntry.ChildCount;
            }

            return null;
        }

        public override int GetChildCount()
        {
            int count = 0;

            if (!this.TryGetOwnerAs(out PropertyDescriptorGridEntry? owner))
            {
                return 0;
            }

            // Child controls exist in tree only when the entry is selected.
            if (GetPropertyGridView() is { } propertyGridView && propertyGridView.SelectedGridEntry == owner)
            {
                // DropDownControlHolder exists in the tree if the drop-down holder is visible.
                if (propertyGridView.DropDownVisible)
                {
                    count++;
                }

                // TextBox exists in the tree if it's created.
                if (propertyGridView.IsEditTextBoxCreated)
                {
                    count++;
                }

                if (propertyGridView.DropDownButton.Visible || propertyGridView.DialogButton.Visible)
                {
                    count++;
                }
            }

            // Child entries exist in the tree if the entry has child entries and is expanded.
            if (owner.Expanded)
            {
                count += owner.ChildCount;
            }

            return count;
        }

        internal override void Collapse()
        {
            if (ExpandCollapseState == ExpandCollapseState.ExpandCollapseState_Expanded)
            {
                ExpandOrCollapse();
            }
        }

        internal override void Expand()
        {
            if (ExpandCollapseState == ExpandCollapseState.ExpandCollapseState_Collapsed)
            {
                ExpandOrCollapse();
            }
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_NextSibling => GetNextSibling(),
                NavigateDirection.NavigateDirection_PreviousSibling => GetPreviousSibling(),
                NavigateDirection.NavigateDirection_FirstChild => FirstChild,
                NavigateDirection.NavigateDirection_LastChild => LastChild,
                _ => base.FragmentNavigate(direction),
            };

        internal override int GetChildIndex(AccessibleObject? child)
        {
            Debug.Assert(child is not null);

            if (!this.TryGetOwnerAs(out PropertyDescriptorGridEntry? owner))
            {
                return -1;
            }

            int index = 0;

            // Child controls exist in tree only when the entry is selected.
            if (GetPropertyGridView() is { } propertyGridView && propertyGridView.SelectedGridEntry == owner)
            {
                // DropDownControlHolder exists in the tree if the drop-down holder is visible.
                if (propertyGridView.DropDownVisible)
                {
                    if (child == propertyGridView.DropDownControlHolder.AccessibilityObject)
                    {
                        return index;
                    }

                    index++;
                }

                // TextBox exists in the tree if it's created.
                if (propertyGridView.IsEditTextBoxCreated)
                {
                    if (child == propertyGridView.EditAccessibleObject)
                    {
                        return index;
                    }

                    index++;
                }

                if (propertyGridView.DropDownButton is { Visible: true } dropDownButton)
                {
                    // DropDownButton exists in the tree if the drop-down button is visible.
                    if (child == dropDownButton.AccessibilityObject)
                    {
                        return index;
                    }

                    index++;
                }
                else if (propertyGridView.DialogButton is { Visible: true } dialogButton)
                {
                    // DialogButton exists in the tree if the ellipsis button is visible.
                    if (child == dialogButton.AccessibilityObject)
                    {
                        return index;
                    }

                    index++;
                }
            }

            // Child entries exist in the tree if the entry has child entries and is expanded.
            if (owner is { ChildCount: > 0, Expanded: true })
            {
                foreach (GridEntry childEntry in owner.Children)
                {
                    if (child == childEntry.AccessibilityObject)
                    {
                        return index;
                    }

                    index++;
                }
            }

            return -1;
        }

        internal AccessibleObject? GetNextChild(AccessibleObject child)
        {
            Debug.Assert(child is not null);

            int index = GetChildIndex(child);
            int lastChildIndex = GetChildCount() - 1;

            Debug.Assert(index <= lastChildIndex);

            // Ensure that it is a valid child and not the last child.
            return index == -1 || index == lastChildIndex ? null : GetChild(index + 1);
        }

        internal AccessibleObject? GetPreviousChild(AccessibleObject child)
        {
            Debug.Assert(child is not null);

            int index = GetChildIndex(child);

            // Ensure that it is a valid child and not the first child.
            return index <= 0 ? null : GetChild(index - 1);
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId
                    => (VARIANT)(this.TryGetOwnerAs(out PropertyDescriptorGridEntry? owner) && !owner.IsPropertyReadOnly),
                UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId => VARIANT.True,
                UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId => (VARIANT)string.Empty,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsIAccessibleExSupported() => true;

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_ValuePatternId => true,
                UIA_PATTERN_ID.UIA_ExpandCollapsePatternId when
                    this.TryGetOwnerAs(out PropertyDescriptorGridEntry? owner)
                    && (owner.Enumerable || owner.NeedsDropDownButton) => true,
                _ => base.IsPatternSupported(patternId)
            };

        private void ExpandOrCollapse()
        {
            if (GetPropertyGridView() is not { } propertyGridView
                || !propertyGridView.IsHandleCreated
                || !this.TryGetOwnerAs(out PropertyDescriptorGridEntry? owner))
            {
                return;
            }

            int row = propertyGridView.GetRowFromGridEntry(owner);

            if (row != -1)
            {
                propertyGridView.PopupEditor(row);
            }
        }

        private IRawElementProviderFragment.Interface? FirstChild
            => GetChildCount() > 0 ? GetChild(0) : null;

        private IRawElementProviderFragment.Interface? LastChild
            => GetChildCount() is int count and > 0
                ? GetChild(count - 1)
                : null;

        private AccessibleObject? GetNextSibling()
        {
            if (!this.TryGetOwnerAs(out PropertyDescriptorGridEntry? owner))
            {
                return null;
            }

            if (owner.ParentGridEntry?.AccessibilityObject is PropertyDescriptorGridEntryAccessibleObject parent)
            {
                return parent.GetNextChild(this);
            }

            return Parent is PropertyGridView.PropertyGridViewAccessibleObject propertyGridViewAccessibleObject
                && propertyGridViewAccessibleObject.TryGetOwnerAs(out PropertyGridView? gridViewOwner)
                    ? PropertyGridView.PropertyGridViewAccessibleObject.GetNextGridEntry(
                        owner,
                        gridViewOwner.TopLevelGridEntries,
                        out _)
                    : null;
        }

        private AccessibleObject? GetPreviousSibling()
        {
            if (!this.TryGetOwnerAs(out PropertyDescriptorGridEntry? owner))
            {
                return null;
            }

            if (owner.ParentGridEntry?.AccessibilityObject is PropertyDescriptorGridEntryAccessibleObject parent)
            {
                return parent.GetPreviousChild(this);
            }

            return Parent is PropertyGridView.PropertyGridViewAccessibleObject propertyGridViewAccessibleObject
                && propertyGridViewAccessibleObject.TryGetOwnerAs(out PropertyGridView? gridViewOwner)
                    ? PropertyGridView.PropertyGridViewAccessibleObject.GetPreviousGridEntry(owner, gridViewOwner.TopLevelGridEntries, out _)
                    : null;
        }

        private PropertyGridView? GetPropertyGridView()
            => Parent is PropertyGridView.PropertyGridViewAccessibleObject propertyGridViewAccessibleObject
                ? propertyGridViewAccessibleObject.TryGetOwnerAs(out PropertyGridView? owner) ? owner : null
                : null;
    }
}
