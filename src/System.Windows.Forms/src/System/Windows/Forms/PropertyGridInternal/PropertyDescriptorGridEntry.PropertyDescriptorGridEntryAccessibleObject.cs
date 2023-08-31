// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyDescriptorGridEntry
{
    internal class PropertyDescriptorGridEntryAccessibleObject : GridEntryAccessibleObject
    {
        public PropertyDescriptorGridEntryAccessibleObject(PropertyDescriptorGridEntry owner) : base(owner)
        {
        }

        internal override UiaCore.ExpandCollapseState ExpandCollapseState
            => GetPropertyGridView() is { } propertyGridView
                && this.TryGetOwnerAs(out PropertyDescriptorGridEntry? owner)
                && owner == propertyGridView.SelectedGridEntry
                && (owner.InternalExpanded || propertyGridView.DropDownVisible)
                    ? UiaCore.ExpandCollapseState.Expanded
                    : UiaCore.ExpandCollapseState.Collapsed;

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
            if (ExpandCollapseState == UiaCore.ExpandCollapseState.Expanded)
            {
                ExpandOrCollapse();
            }
        }

        internal override void Expand()
        {
            if (ExpandCollapseState == UiaCore.ExpandCollapseState.Collapsed)
            {
                ExpandOrCollapse();
            }
        }

        /// <summary>
        ///  Returns the element in the specified direction.
        /// </summary>
        /// <param name="direction">Indicates the direction in which to navigate.</param>
        /// <returns>Returns the element in the specified direction.</returns>
        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            => direction switch
            {
                UiaCore.NavigateDirection.NextSibling => GetNextSibling(),
                UiaCore.NavigateDirection.PreviousSibling => GetPreviousSibling(),
                UiaCore.NavigateDirection.FirstChild => GetFirstChild(),
                UiaCore.NavigateDirection.LastChild => GetLastChild(),
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

        internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
            propertyID switch
            {
                UiaCore.UIA.IsEnabledPropertyId
                    => this.TryGetOwnerAs(out PropertyDescriptorGridEntry? owner) && !owner.IsPropertyReadOnly,
                UiaCore.UIA.IsValuePatternAvailablePropertyId => true,
                UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId => string.Empty,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsIAccessibleExSupported() => true;

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
            => patternId switch
            {
                UiaCore.UIA.ValuePatternId => true,
                UiaCore.UIA.ExpandCollapsePatternId when
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

        private UiaCore.IRawElementProviderFragment? GetFirstChild() => GetChildCount() > 0 ? GetChild(0) : null;

        private UiaCore.IRawElementProviderFragment? GetLastChild() => GetChildCount() is int count and > 0
            ? GetChild(count - 1)
            : null;

        private UiaCore.IRawElementProviderFragment? GetNextSibling()
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
                    ? (UiaCore.IRawElementProviderFragment?)PropertyGridView.PropertyGridViewAccessibleObject.GetNextGridEntry(
                        owner,
                        gridViewOwner.TopLevelGridEntries,
                        out _)
                    : null;
        }

        private UiaCore.IRawElementProviderFragment? GetPreviousSibling()
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
