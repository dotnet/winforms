﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class ToolStripItem
    {
        /// <summary>
        ///  An implementation of AccessibleChild for use with ToolStripItems
        /// </summary>
        public class ToolStripItemAccessibleObject : AccessibleObject
        {
            private readonly ToolStripItem _ownerItem; // The associated ToolStripItem for this AccessibleChild (if any)

            private AccessibleStates _additionalState = AccessibleStates.None; // Test hook for the designer

            private int[]? _runtimeId;

            public ToolStripItemAccessibleObject(ToolStripItem ownerItem)
            {
                _ownerItem = ownerItem.OrThrowIfNull();
            }

            public override string DefaultAction
            {
                get
                {
                    string? defaultAction = _ownerItem.AccessibleDefaultActionDescription;
                    if (defaultAction is not null)
                    {
                        return defaultAction;
                    }

                    return SR.AccessibleActionPress;
                }
            }

            public override string? Description
            {
                get
                {
                    string? description = _ownerItem.AccessibleDescription;
                    if (description is not null)
                    {
                        return description;
                    }

                    return base.Description;
                }
            }

            public override string? Help
            {
                get
                {
                    QueryAccessibilityHelpEventHandler? handler = (QueryAccessibilityHelpEventHandler?)Owner.Events[s_queryAccessibilityHelpEvent];
                    if (handler is not null)
                    {
                        QueryAccessibilityHelpEventArgs args = new QueryAccessibilityHelpEventArgs();
                        handler(Owner, args);
                        return args.HelpString;
                    }

                    return base.Help;
                }
            }

            public override string KeyboardShortcut
            {
                get
                {
                    // This really is the Mnemonic - NOT the shortcut.  E.g. in notepad Edit->Replace is Control+H
                    // but the KeyboardShortcut comes up as the mnemonic 'r'.
                    char mnemonic = WindowsFormsUtils.GetMnemonic(_ownerItem.Text, false);
                    if (_ownerItem.IsOnDropDown)
                    {
                        // no ALT on dropdown
                        return mnemonic == '\0' ? string.Empty : mnemonic.ToString();
                    }

                    return mnemonic == '\0' ? string.Empty : ("Alt+" + mnemonic);
                }
            }

            // We need to provide a unique ID. Others are implementing this in the same manner. First item should be UiaAppendRuntimeId
            // since this is not a top-level element of the fragment. Second item can be anything, but here it is a hash.
            // For toolstrip hash is unique even with child controls. Hwnd  is not.
            internal override int[] RuntimeId
                => _runtimeId ??= new int[]
                {
                    NativeMethods.UiaAppendRuntimeId,
                    _ownerItem.GetHashCode()
                };

            /// <summary>
            ///  Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    // "ControlType" value depends on owner's AccessibleRole value.
                    // See: docs/accessibility/accessible-role-controltype.md
                    UiaCore.UIA.ControlTypePropertyId => AccessibleRoleControlTypeMap.GetControlType(Role),
                    UiaCore.UIA.HasKeyboardFocusPropertyId => _ownerItem.Selected,
                    UiaCore.UIA.IsEnabledPropertyId => _ownerItem.Enabled,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => _ownerItem.CanSelect,
                    UiaCore.UIA.IsOffscreenPropertyId => GetIsOffscreenPropertyValue(_ownerItem.Placement, Bounds),
                    _ => base.GetPropertyValue(propertyID)
                };

            public override string? Name
            {
                get
                {
                    string name = _ownerItem.AccessibleName;
                    if (name is not null)
                    {
                        return name;
                    }

                    string? baseName = base.Name;
                    if (string.IsNullOrEmpty(baseName))
                    {
                        return WindowsFormsUtils.TextWithoutMnemonics(_ownerItem.Text)!;
                    }

                    return baseName;
                }
                set => _ownerItem.AccessibleName = value;
            }

            internal ToolStripItem Owner => _ownerItem;

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = _ownerItem.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }

                    return AccessibleRole.PushButton;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (!_ownerItem.CanSelect)
                    {
                        return base.State | _additionalState;
                    }

                    if (!_ownerItem.Enabled)
                    {
                        if (_ownerItem.Selected && _ownerItem is ToolStripMenuItem)
                        {
                            return AccessibleStates.Unavailable | _additionalState | AccessibleStates.Focused;
                        }

                        // Disabled menu items that are selected must have focus
                        // state so that Narrator can announce them.
                        if (_ownerItem.Selected && _ownerItem is ToolStripMenuItem)
                        {
                            return AccessibleStates.Focused;
                        }

                        return AccessibleStates.Unavailable | _additionalState;
                    }

                    AccessibleStates accState = AccessibleStates.Focusable | _additionalState;
                    if (_ownerItem.Selected || _ownerItem.Pressed)
                    {
                        accState |= AccessibleStates.Focused | AccessibleStates.HotTracked;
                    }

                    if (_ownerItem.Pressed)
                    {
                        accState |= AccessibleStates.Pressed;
                    }

                    return accState;
                }
            }

            public override void DoDefaultAction()
            {
                if (Owner is not null)
                {
                    Owner.PerformClick();
                }
            }

            public override int GetHelpTopic(out string? fileName)
            {
                int topic = 0;

                QueryAccessibilityHelpEventHandler? handler = (QueryAccessibilityHelpEventHandler?)Owner.Events[s_queryAccessibilityHelpEvent];

                if (handler is not null)
                {
                    QueryAccessibilityHelpEventArgs args = new QueryAccessibilityHelpEventArgs();
                    handler(Owner, args);

                    fileName = args.HelpNamespace;

                    int.TryParse(args.HelpKeyword, NumberStyles.Integer, CultureInfo.InvariantCulture, out topic);

                    return topic;
                }

                return base.GetHelpTopic(out fileName);
            }

            public override AccessibleObject? Navigate(AccessibleNavigation navigationDirection)
            {
                ToolStripItem? nextItem = null;

                if (Owner is not null)
                {
                    ToolStrip parent = Owner.ParentInternal;
                    if (parent is null)
                    {
                        return null;
                    }

                    switch (navigationDirection)
                    {
                        case AccessibleNavigation.FirstChild:
                            nextItem = parent.GetNextItem(null, ArrowDirection.Right, /*RTLAware=*/true);
                            break;
                        case AccessibleNavigation.LastChild:
                            nextItem = parent.GetNextItem(null, ArrowDirection.Left, /*RTLAware=*/true);
                            break;
                        case AccessibleNavigation.Previous:
                        case AccessibleNavigation.Left:
                            nextItem = parent.GetNextItem(Owner, ArrowDirection.Left, /*RTLAware=*/true);
                            break;
                        case AccessibleNavigation.Next:
                        case AccessibleNavigation.Right:
                            nextItem = parent.GetNextItem(Owner, ArrowDirection.Right, /*RTLAware=*/true);
                            break;
                        case AccessibleNavigation.Up:
                            nextItem = (Owner.IsOnDropDown) ? parent.GetNextItem(Owner, ArrowDirection.Up) :
                                                               parent.GetNextItem(Owner, ArrowDirection.Left, /*RTLAware=*/true);
                            break;
                        case AccessibleNavigation.Down:
                            nextItem = (Owner.IsOnDropDown) ? parent.GetNextItem(Owner, ArrowDirection.Down) :
                                                               parent.GetNextItem(Owner, ArrowDirection.Right, /*RTLAware=*/true);
                            break;
                    }
                }

                return nextItem?.AccessibilityObject;
            }

            public void AddState(AccessibleStates state)
            {
                if (state == AccessibleStates.None)
                {
                    _additionalState = state;
                }
                else
                {
                    _additionalState |= state;
                }
            }

            public override string ToString()
            {
                if (Owner is not null)
                {
                    return "ToolStripItemAccessibleObject: Owner = " + Owner.ToString();
                }

                return "ToolStripItemAccessibleObject: Owner = null";
            }

            /// <summary>
            ///  Gets the bounds of the accessible object, in screen coordinates.
            /// </summary>
            public override Rectangle Bounds
            {
                get
                {
                    Rectangle bounds = Owner.Bounds;

                    if (Owner.ParentInternal is not null && Owner.ParentInternal.Visible)
                    {
                        return new Rectangle(Owner.ParentInternal.PointToScreen(bounds.Location), bounds.Size);
                    }

                    return Rectangle.Empty;
                }
            }

            /// <summary>
            ///  When overridden in a derived class, gets or sets the parent of an accessible object.
            /// </summary>
            public override AccessibleObject? Parent
            {
                get
                {
                    if (Owner.IsOnDropDown)
                    {
                        // Return the owner item as the accessible parent.
                        ToolStripDropDown dropDown = Owner.GetCurrentParentDropDown();
                        return dropDown.AccessibilityObject;
                    }

                    return (Owner.Parent is not null) ? Owner.Parent.AccessibilityObject : base.Parent;
                }
            }

            /// <summary>
            ///  Gets the top level element.
            /// </summary>
            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot
                => _ownerItem.RootToolStrip?.AccessibilityObject;

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return Parent;
                    case UiaCore.NavigateDirection.NextSibling:
                    case UiaCore.NavigateDirection.PreviousSibling:
                        int index = GetChildFragmentIndex();
                        if (index == -1)
                        {
                            Debug.Fail("No item matched the index?");
                            return null;
                        }

                        AccessibleObject? sibling = null;
                        index += direction == UiaCore.NavigateDirection.NextSibling ? 1 : -1;
                        int itemsCount = GetChildFragmentCount();
                        if (index >= 0 && index < itemsCount)
                        {
                            sibling = GetChildFragment(index, direction);
                        }

                        return sibling;
                }

                return base.FragmentNavigate(direction);
            }

            private AccessibleObject? GetChildFragment(int index, UiaCore.NavigateDirection direction)
            {
                if (Parent is ToolStrip.ToolStripAccessibleObject toolStripParent)
                {
                    return toolStripParent.GetChildFragment(index, direction);
                }

                // ToolStripOverflowButtonAccessibleObject is derived from ToolStripDropDownItemAccessibleObject
                // and we should not process ToolStripOverflowButton as a ToolStripDropDownItem here so check for
                // the ToolStripOverflowButton firstly as more specific condition.
                if (Parent is ToolStripOverflowButton.ToolStripOverflowButtonAccessibleObject toolStripOverflowButtonParent)
                {
                    if (toolStripOverflowButtonParent.Parent is ToolStrip.ToolStripAccessibleObject toolStripGrandParent)
                    {
                        return toolStripGrandParent.GetChildFragment(index, direction, true);
                    }
                }

                if (Parent is ToolStripDropDownItemAccessibleObject dropDownItemParent)
                {
                    return dropDownItemParent.GetChildFragment(index, direction);
                }

                return null;
            }

            private int GetChildFragmentCount()
            {
                if (Parent is ToolStrip.ToolStripAccessibleObject toolStripParent)
                {
                    return toolStripParent.GetChildFragmentCount();
                }

                if (Parent is ToolStripOverflowButton.ToolStripOverflowButtonAccessibleObject toolStripOverflowButtonParent)
                {
                    if (toolStripOverflowButtonParent.Parent is ToolStrip.ToolStripAccessibleObject toolStripGrandParent)
                    {
                        return toolStripGrandParent.GetChildOverflowFragmentCount();
                    }
                }

                if (Parent is ToolStripDropDownItemAccessibleObject dropDownItemParent)
                {
                    return dropDownItemParent.GetChildCount();
                }

                return -1;
            }

            private int GetChildFragmentIndex()
            {
                if (Parent is ToolStrip.ToolStripAccessibleObject toolStripParent)
                {
                    return toolStripParent.GetChildFragmentIndex(this);
                }

                if (Parent is ToolStripOverflowButton.ToolStripOverflowButtonAccessibleObject toolStripOverflowButtonParent)
                {
                    if (toolStripOverflowButtonParent.Parent is ToolStrip.ToolStripAccessibleObject toolStripGrandParent)
                    {
                        return toolStripGrandParent.GetChildFragmentIndex(this);
                    }
                }

                if (Parent is ToolStripDropDownItemAccessibleObject dropDownItemParent)
                {
                    return dropDownItemParent.GetChildFragmentIndex(this);
                }

                return -1;
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.LegacyIAccessiblePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override void SetFocus() => Owner.Select();

            internal void RaiseFocusChanged()
            {
                ToolStrip root = _ownerItem.RootToolStrip;
                if (root is not null && root.IsHandleCreated && root.SupportsUiaProviders)
                {
                    RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                }
            }
        }
    }
}
