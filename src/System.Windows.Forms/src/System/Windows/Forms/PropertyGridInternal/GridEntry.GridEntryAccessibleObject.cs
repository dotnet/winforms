// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal abstract partial class GridEntry
    {
        public class GridEntryAccessibleObject : AccessibleObject
        {
            private readonly GridEntry _owningGridEntry;

            // Used by UIAutomation
            private int[]? _runtimeId;

            private delegate void SelectDelegate(AccessibleSelection flags);

            public GridEntryAccessibleObject(GridEntry owner) : base()
            {
                Debug.Assert(owner is not null, "GridEntryAccessibleObject must have a valid owner GridEntry");
                _owningGridEntry = owner;
            }

            public override Rectangle Bounds
            {
                get => PropertyGridView is not null && PropertyGridView.IsHandleCreated
                    ? PropertyGridView.AccessibilityGetGridEntryBounds(_owningGridEntry)
                    : Rectangle.Empty;
            }

            public override string? DefaultAction
            {
                get
                {
                    if (!_owningGridEntry.Expandable)
                    {
                        return base.DefaultAction;
                    }
                    else if (_owningGridEntry.Expanded)
                    {
                        return SR.AccessibleActionCollapse;
                    }
                    else
                    {
                        return SR.AccessibleActionExpand;
                    }
                }
            }

            public override string Description
            {
                get
                {
                    return _owningGridEntry.PropertyDescription;
                }
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    if (_owningGridEntry.Expandable)
                    {
                        return _owningGridEntry.Expanded ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
                    }
                    else
                    {
                        return UiaCore.ExpandCollapseState.LeafNode;
                    }
                }
            }

            public override string Help
            {
                get
                {
                    return _owningGridEntry.PropertyDescription;
                }
            }

            public override string? Name
            {
                get
                {
                    return _owningGridEntry?.PropertyLabel;
                }
            }

            public override AccessibleObject? Parent
            {
                get
                {
                    return _owningGridEntry?.GridEntryHost?.AccessibilityObject;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.Cell;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (PropertyGridView is null || !PropertyGridView.IsHandleCreated)
                    {
                        return AccessibleStates.None;
                    }

                    AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;

                    // Determine focus
                    //
                    if (_owningGridEntry.Focus)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    // Determine selected
                    //
                    Debug.Assert(Parent is not null, "GridEntry AO does not have a parent AO");
                    PropertyGridView.PropertyGridViewAccessibleObject parent = (PropertyGridView.PropertyGridViewAccessibleObject)Parent;
                    if (parent.GetSelected() == this)
                    {
                        state |= AccessibleStates.Selected;
                    }

                    // Determine expanded/collapsed state
                    //
                    if (_owningGridEntry.Expandable)
                    {
                        if (_owningGridEntry.Expanded)
                        {
                            state |= AccessibleStates.Expanded;
                        }
                        else
                        {
                            state |= AccessibleStates.Collapsed;
                        }
                    }

                    // Determine readonly/editable state
                    //
                    if (_owningGridEntry.ShouldRenderReadOnly)
                    {
                        state |= AccessibleStates.ReadOnly;
                    }

                    // Determine password state
                    //
                    if (_owningGridEntry.ShouldRenderPassword)
                    {
                        state |= AccessibleStates.Protected;
                    }

                    Rectangle entryBounds = BoundingRectangle;
                    Rectangle propertyGridViewBounds = PropertyGridView.GetToolNativeScreenRectangle();

                    if (!entryBounds.IntersectsWith(propertyGridViewBounds))
                    {
                        state |= AccessibleStates.Offscreen;
                    }

                    return state;
                }
            }

            public override string? Value
            {
                get
                {
                    return _owningGridEntry.GetPropertyTextValue();
                }

                set
                {
                    _owningGridEntry.SetPropertyTextValue(value);
                }
            }

            internal override int Column => 0;

            internal override UiaCore.IRawElementProviderSimple? ContainingGrid
            {
                get => PropertyGridView?.AccessibilityObject;
            }

            /// <summary>
            ///  Return the element that is the root node of this fragment of UI.
            /// </summary>
            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot
            {
                get
                {
                    return Parent as PropertyGridView.PropertyGridViewAccessibleObject;
                }
            }

            internal override int Row
            {
                get
                {
                    if (Parent is not PropertyGridView.PropertyGridViewAccessibleObject parent)
                    {
                        return -1;
                    }

                    if (parent.Owner is not PropertyGridView gridView)
                    {
                        return -1;
                    }

                    GridEntryCollection? topLevelGridEntries = gridView.TopLevelGridEntries;
                    if (topLevelGridEntries is null)
                    {
                        return -1;
                    }

                    for (int i = 0; i < topLevelGridEntries.Count; i++)
                    {
                        GridItem? topLevelGridEntry = topLevelGridEntries[i];
                        if (_owningGridEntry == topLevelGridEntry)
                        {
                            return i;
                        }
                    }

                    return -1;
                }
            }

            internal override int[]? RuntimeId
            {
                get
                {
                    if (_owningGridEntry.GridEntryHost is null || !_owningGridEntry.GridEntryHost.IsHandleCreated)
                    {
                        return base.RuntimeId;
                    }

                    if (_runtimeId is null)
                    {
                        // we need to provide a unique ID
                        // others are implementing this in the same manner
                        // first item is static - 0x2a
                        // second item can be anything, but it's good to supply HWND
                        // third and others are optional, but in case of GridItem we need it, to make it unique
                        // grid items are not controls, they don't have hwnd - we use hwnd of PropertyGridView

                        _runtimeId = new int[3];
                        _runtimeId[0] = 0x2a;
                        _runtimeId[1] = (int)(long)_owningGridEntry.GridEntryHost.InternalHandle;
                        _runtimeId[2] = GetHashCode();
                    }

                    return _runtimeId;
                }
            }

            private PropertyGridView? PropertyGridView
            {
                get
                {
                    var propertyGridViewAccessibleObject = Parent as PropertyGridView.PropertyGridViewAccessibleObject;
                    if (propertyGridViewAccessibleObject is not null)
                    {
                        return propertyGridViewAccessibleObject.Owner as PropertyGridView;
                    }

                    return null;
                }
            }

            public override void DoDefaultAction()
            {
                if (PropertyGridView is not null && PropertyGridView.IsHandleCreated)
                {
                    _owningGridEntry.OnOutlineClick(EventArgs.Empty);
                }
            }

            /// <summary>
            ///  Returns the currently focused child, if any.
            ///  Returns this if the object itself is focused.
            /// </summary>
            public override AccessibleObject? GetFocused()
            {
                if (_owningGridEntry.Focus)
                {
                    return this;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            ///  Navigate to the next or previous grid entry.
            /// </summary>
            public override AccessibleObject? Navigate(AccessibleNavigation navdir)
            {
                if (Parent is not PropertyGridView.PropertyGridViewAccessibleObject parent)
                {
                    return null;
                }

                switch (navdir)
                {
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Right:
                    case AccessibleNavigation.Next:
                        return parent.Next(_owningGridEntry);

                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Left:
                    case AccessibleNavigation.Previous:
                        return parent.Previous(_owningGridEntry);

                    case AccessibleNavigation.FirstChild:
                    case AccessibleNavigation.LastChild:
                        // Fall through and return null,
                        // as this object has no children.
                        break;
                }

                return null;
            }

            public override void Select(AccessibleSelection flags)
            {
                if (PropertyGridView is null || !PropertyGridView.IsHandleCreated)
                {
                    return;
                }

                // make sure we're on the right thread.
                //
                if (PropertyGridView.InvokeRequired)
                {
                    PropertyGridView.Invoke(new SelectDelegate(Select), new object[] { flags });
                    return;
                }

                // Focus the PropertyGridView window
                //
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    bool focused = PropertyGridView.Focus();
                }

                // Select the grid entry
                //
                if ((flags & AccessibleSelection.TakeSelection) == AccessibleSelection.TakeSelection)
                {
                    PropertyGridView.AccessibilitySelect(_owningGridEntry);
                }
            }

            internal override void Collapse()
            {
                if (_owningGridEntry.Expandable && _owningGridEntry.Expanded == true)
                {
                    _owningGridEntry.Expanded = false;
                }
            }

            internal override void Expand()
            {
                if (_owningGridEntry.Expandable && _owningGridEntry.Expanded == false)
                {
                    _owningGridEntry.Expanded = true;
                }
            }

            /// <summary>
            ///  Request to return the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        GridEntry parentGridEntry = _owningGridEntry.ParentGridEntry;
                        if (parentGridEntry is not null)
                        {
                            if (parentGridEntry is SingleSelectRootGridEntry)
                            {
                                return _owningGridEntry.OwnerGrid.GridViewAccessibleObject;
                            }
                            else
                            {
                                return parentGridEntry.AccessibilityObject;
                            }
                        }

                        return Parent;
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return Navigate(AccessibleNavigation.Previous);
                    case UiaCore.NavigateDirection.NextSibling:
                        return Navigate(AccessibleNavigation.Next);
                }

                return base.FragmentNavigate(direction);
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.ControlTypePropertyId:

                        // The accessible hierarchy is changed so we cannot use Button type
                        // for the grid items to not break automation logic that searches for the first
                        // button in the PropertyGridView to show dialog/drop-down. In Level < 3 action
                        // button is one of the first children of PropertyGridView.
                        return UiaCore.UIA.TreeItemControlTypeId;
                    case UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId:
                        return (Object)IsPatternSupported(UiaCore.UIA.ExpandCollapsePatternId);
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return string.Empty;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return _owningGridEntry.hasFocus;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return true;
                    case UiaCore.UIA.AutomationIdPropertyId:
                        return GetHashCode().ToString();
                    case UiaCore.UIA.HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    case UiaCore.UIA.IsGridItemPatternAvailablePropertyId:
                    case UiaCore.UIA.IsTableItemPatternAvailablePropertyId:
                        return true;
                    case UiaCore.UIA.LegacyIAccessibleRolePropertyId:
                        return Role;
                    case UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId:
                        return DefaultAction;

                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (_owningGridEntry.Expandable)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                switch (patternId)
                {
                    case UiaCore.UIA.InvokePatternId:
                    case UiaCore.UIA.LegacyIAccessiblePatternId:
                        return true;

                    case UiaCore.UIA.ExpandCollapsePatternId:
                        if (_owningGridEntry is not null && _owningGridEntry.Expandable)
                        {
                            return true;
                        }

                        break;

                    case UiaCore.UIA.GridItemPatternId:
                    case UiaCore.UIA.TableItemPatternId:
                        if (_owningGridEntry is null || _owningGridEntry.OwnerGrid is null || _owningGridEntry.OwnerGrid.SortedByCategories)
                        {
                            break;
                        }

                        // Only top level rows are grid items.
                        // Sub-items (for instance height in size is not a grid item)
                        GridEntry parentGridEntry = _owningGridEntry.ParentGridEntry;
                        if (parentGridEntry is not null && parentGridEntry is SingleSelectRootGridEntry)
                        {
                            return true;
                        }

                        break;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override void SetFocus()
            {
                if (PropertyGridView is null || !PropertyGridView.IsHandleCreated)
                {
                    return;
                }

                base.SetFocus();

                RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
            }
        }
    }
}
