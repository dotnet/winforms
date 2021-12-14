// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyDescriptorGridEntry
    {
        internal class PropertyDescriptorGridEntryAccessibleObject : GridEntryAccessibleObject
        {
            private readonly PropertyDescriptorGridEntry _owningPropertyDescriptorGridEntry;

            public PropertyDescriptorGridEntryAccessibleObject(PropertyDescriptorGridEntry owner) : base(owner)
            {
                _owningPropertyDescriptorGridEntry = owner;
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    PropertyGridView? propertyGridView = GetPropertyGridView();
                    if (propertyGridView is null)
                    {
                        return UiaCore.ExpandCollapseState.Collapsed;
                    }

                    if (_owningPropertyDescriptorGridEntry == propertyGridView.SelectedGridEntry &&
                        ((_owningPropertyDescriptorGridEntry is not null && _owningPropertyDescriptorGridEntry.InternalExpanded)
                         || propertyGridView.DropDownVisible))
                    {
                        return UiaCore.ExpandCollapseState.Expanded;
                    }

                    return UiaCore.ExpandCollapseState.Collapsed;
                }
            }

            public override AccessibleObject? GetChild(int index)
            {
                Debug.Assert(index >= 0);

                // Child controls exist in tree only when the entry is selected.
                if (GetPropertyGridView() is { } propertyGridView && propertyGridView.SelectedGridEntry == _owningPropertyDescriptorGridEntry)
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

                    // DropDownButton exists in the tree if the drop-down button is visible.
                    if (propertyGridView.DropDownButton is { Visible: true } dropDownButton)
                    {
                        if (index == 0)
                        {
                            return dropDownButton.AccessibilityObject;
                        }

                        index--;
                    } // DialogButton exists in the tree if the ellipsis button is visible.
                    else if (propertyGridView.DialogButton is { Visible: true } dialogButton)
                    {
                        if (index == 0)
                        {
                            return dialogButton.AccessibilityObject;
                        }

                        index--;
                    }
                }

                // Child entries exist in the tree if the entry has child entries and is expanded.
                if (_owningPropertyDescriptorGridEntry is { ChildCount: > 0, Expanded: true })
                {
                    if (index < _owningPropertyDescriptorGridEntry.ChildCount)
                    {
                        return _owningPropertyDescriptorGridEntry.Children[index].AccessibilityObject;
                    }

                    // Uncomment the following line in case there will be another children going after the child entries:
                    // index -= _owningPropertyDescriptorGridEntry.ChildCount;
                }

                return null;
            }

            public override int GetChildCount()
            {
                int count = 0;
                // Child controls exist in tree only when the entry is selected.
                if (GetPropertyGridView() is { } propertyGridView && propertyGridView.SelectedGridEntry == _owningPropertyDescriptorGridEntry)
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

                    // DropDownButton exists in the tree if the drop-down button is visible.
                    if (propertyGridView.DropDownButton.Visible)
                    {
                        count++;
                    } // DialogButton exists in the tree if the ellipsis button is visible.
                    else if (propertyGridView.DialogButton.Visible)
                    {
                        count++;
                    }
                }

                // Child entries exist in the tree if the entry has child entries and is expanded.
                if (_owningPropertyDescriptorGridEntry.Expanded)
                {
                    count += _owningPropertyDescriptorGridEntry.ChildCount;
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

                int index = 0;
                // Child controls exist in tree only when the entry is selected.
                if (GetPropertyGridView() is { } propertyGridView && propertyGridView.SelectedGridEntry == _owningPropertyDescriptorGridEntry)
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

                    // DropDownButton exists in the tree if the drop-down button is visible.
                    if (propertyGridView.DropDownButton is { Visible: true } dropDownButton)
                    {
                        if (child == dropDownButton.AccessibilityObject)
                        {
                            return index;
                        }

                        index++;
                    } // DialogButton exists in the tree if the ellipsis button is visible.
                    else if (propertyGridView.DialogButton is { Visible: true } dialogButton)
                    {
                        if (child == dialogButton.AccessibilityObject)
                        {
                            return index;
                        }

                        index++;
                    }
                }

                // Child entries exist in the tree if the entry has child entries and is expanded.
                if (_owningPropertyDescriptorGridEntry is { ChildCount: > 0, Expanded: true })
                {
                    foreach (GridEntry childEntry in _owningPropertyDescriptorGridEntry.Children)
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
                if (index == -1 || index == lastChildIndex)
                {
                    return null;
                }

                return GetChild(index + 1);
            }

            internal AccessibleObject? GetPreviousChild(AccessibleObject child)
            {
                Debug.Assert(child is not null);

                int index = GetChildIndex(child);

                // Ensure that it is a valid child and not the first child.
                if (index == -1 || index == 0)
                {
                    return null;
                }

                return GetChild(index - 1);
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.IsEnabledPropertyId)
                {
                    return !_owningPropertyDescriptorGridEntry.IsPropertyReadOnly;
                }
                else if (propertyID == UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId)
                {
                    return string.Empty;
                }
                else if (propertyID == UiaCore.UIA.IsValuePatternAvailablePropertyId)
                {
                    return true;
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.ValuePatternId => true,
                    UiaCore.UIA.ExpandCollapsePatternId when
                        _owningPropertyDescriptorGridEntry.Enumerable || _owningPropertyDescriptorGridEntry.NeedsDropDownButton => true,
                    _ => base.IsPatternSupported(patternId)
                };

            private void ExpandOrCollapse()
            {
                PropertyGridView? propertyGridView = GetPropertyGridView();
                if (propertyGridView is null)
                {
                    return;
                }

                if (!propertyGridView.IsHandleCreated)
                {
                    return;
                }

                int row = propertyGridView.GetRowFromGridEntry(_owningPropertyDescriptorGridEntry);

                if (row != -1)
                {
                    propertyGridView.PopupEditor(row);
                }
            }

            private UiaCore.IRawElementProviderFragment? GetFirstChild() => GetChildCount() > 0 ? GetChild(0) : null;

            private UiaCore.IRawElementProviderFragment? GetLastChild() => GetChildCount() is int count and > 0 ? GetChild(count - 1) : null;

            private UiaCore.IRawElementProviderFragment? GetNextSibling()
            {
                if (_owningPropertyDescriptorGridEntry.ParentGridEntry?.AccessibilityObject is PropertyDescriptorGridEntryAccessibleObject parent)
                {
                    return parent.GetNextChild(this);
                }

                var propertyGridViewAccessibleObject = (PropertyGridView.PropertyGridViewAccessibleObject?)Parent;
                var propertyGridView = propertyGridViewAccessibleObject?.Owner as PropertyGridView;
                return propertyGridViewAccessibleObject?.GetNextGridEntry(_owningPropertyDescriptorGridEntry, propertyGridView?.TopLevelGridEntries, out _);
            }

            private UiaCore.IRawElementProviderFragment? GetPreviousSibling()
            {
                if (_owningPropertyDescriptorGridEntry.ParentGridEntry?.AccessibilityObject is PropertyDescriptorGridEntryAccessibleObject parent)
                {
                    return parent.GetPreviousChild(this);
                }

                var propertyGridViewAccessibleObject = (PropertyGridView.PropertyGridViewAccessibleObject?)Parent;
                var propertyGridView = propertyGridViewAccessibleObject?.Owner as PropertyGridView;
                return propertyGridViewAccessibleObject?.GetPreviousGridEntry(_owningPropertyDescriptorGridEntry, propertyGridView?.TopLevelGridEntries, out _);
            }

            private PropertyGridView? GetPropertyGridView()
            {
                if (Parent is not PropertyGridView.PropertyGridViewAccessibleObject propertyGridViewAccessibleObject)
                {
                    return null;
                }

                return propertyGridViewAccessibleObject.Owner as PropertyGridView;
            }
        }
    }
}
