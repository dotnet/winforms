// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Globalization;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using PInvoke = Windows.Win32.PInvoke;

namespace System.Windows.Forms;

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

        internal override bool CanGetDefaultActionInternal => false;

        public override string? Description =>
            _ownerItem.AccessibleDescription is { } description ? description : base.Description;

        internal override bool CanGetDescriptionInternal => IsInternal && _ownerItem.AccessibleDescription is null;

        public override string? Help
        {
            get
            {
                QueryAccessibilityHelpEventHandler? handler = (QueryAccessibilityHelpEventHandler?)Owner.Events[s_queryAccessibilityHelpEvent];
                if (handler is not null)
                {
                    QueryAccessibilityHelpEventArgs args = new();
                    handler(Owner, args);
                    return args.HelpString;
                }

                return base.Help;
            }
        }

        internal override bool CanGetHelpInternal
            => IsInternal && (QueryAccessibilityHelpEventHandler?)Owner.Events[s_queryAccessibilityHelpEvent] is null;

        public override string KeyboardShortcut
        {
            get
            {
                // This really is the Mnemonic - NOT the shortcut. E.g. in notepad Edit->Replace is Control+H
                // but the KeyboardShortcut comes up as the mnemonic 'r'.
                char mnemonic = WindowsFormsUtils.GetMnemonic(_ownerItem.Text, false);
                if (_ownerItem.IsOnDropDown)
                {
                    // no ALT on dropdown
                    return mnemonic == '\0' ? string.Empty : mnemonic.ToString();
                }

                return mnemonic == '\0' ? string.Empty : $"Alt+{mnemonic}";
            }
        }

        internal override bool CanGetKeyboardShortcutInternal => false;

        /// <remarks>
        ///  <para>
        ///    First item should be <see cref="PInvoke.UiaAppendRuntimeId" /> since this is not a top-level
        ///    element of the fragment. Second item can be anything, but here it is the owner hash code.
        ///    For <see cref="ToolStrip" /> hash code is unique even with child controls.
        ///    <see cref="Control.Handle" /> is not.
        ///  </para>
        /// </remarks>
        /// <inheritdoc cref="AccessibleObject.RuntimeId" />
        internal override int[] RuntimeId => _runtimeId ??=
        [
            (int)PInvoke.UiaAppendRuntimeId,
            _ownerItem.GetHashCode()
        ];

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                // "ControlType" value depends on owner's AccessibleRole value.
                // See: docs/accessibility/accessible-role-controltype.md
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)AccessibleRoleControlTypeMap.GetControlType(Role),
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)_ownerItem.Selected,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)_ownerItem.Enabled,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)_ownerItem.CanSelect,
                UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId => (VARIANT)GetIsOffscreenPropertyValue(_ownerItem.Placement, Bounds),
                UIA_PROPERTY_ID.UIA_IsControlElementPropertyId => VARIANT.True,
                UIA_PROPERTY_ID.UIA_IsContentElementPropertyId => VARIANT.True,
                _ => base.GetPropertyValue(propertyID)
            };

        public override string? Name
        {
            get
            {
                string? name = _ownerItem.AccessibleName;
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

        internal override bool CanGetNameInternal => false;

        internal override bool CanSetNameInternal => false;

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
            Owner?.PerformClick();
        }

        public override int GetHelpTopic(out string? fileName)
        {
            int topic = 0;

            QueryAccessibilityHelpEventHandler? handler = (QueryAccessibilityHelpEventHandler?)Owner.Events[s_queryAccessibilityHelpEvent];

            if (handler is not null)
            {
                QueryAccessibilityHelpEventArgs args = new();
                handler(Owner, args);

                fileName = args.HelpNamespace;

                int.TryParse(args.HelpKeyword, NumberStyles.Integer, CultureInfo.InvariantCulture, out topic);

                return topic;
            }

            return base.GetHelpTopic(out fileName);
        }

        internal override bool CanGetHelpTopicInternal => IsInternal && Owner.Events[s_queryAccessibilityHelpEvent] is null;

        public override AccessibleObject? Navigate(AccessibleNavigation navigationDirection)
        {
            ToolStripItem? nextItem = null;

            if (Owner is not null)
            {
                ToolStrip? parent = Owner.ParentInternal;
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
                return $"ToolStripItemAccessibleObject: Owner = {Owner}";
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
                    ToolStripDropDown dropDown = Owner.GetCurrentParentDropDown()!;
                    return dropDown.AccessibilityObject;
                }

                return (Owner.Parent is not null) ? Owner.Parent.AccessibilityObject : base.Parent;
            }
        }

        internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot =>
            _ownerItem.RootToolStrip?.AccessibilityObject;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            switch (direction)
            {
                case NavigateDirection.NavigateDirection_Parent:
                    return Parent;
                case NavigateDirection.NavigateDirection_NextSibling:
                case NavigateDirection.NavigateDirection_PreviousSibling:
                    int index = GetChildFragmentIndex();
                    if (index == -1)
                    {
                        Debug.Fail("No item matched the index?");
                        return null;
                    }

                    AccessibleObject? sibling = null;
                    index += direction == NavigateDirection.NavigateDirection_NextSibling ? 1 : -1;
                    int itemsCount = GetChildFragmentCount();
                    if (index >= 0 && index < itemsCount)
                    {
                        sibling = GetChildFragment(index, direction);
                    }

                    return sibling;
            }

            return base.FragmentNavigate(direction);
        }

        private AccessibleObject? GetChildFragment(int index, NavigateDirection direction)
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

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
        {
            if (patternId == UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)
            {
                return true;
            }

            return base.IsPatternSupported(patternId);
        }

        internal override void SetFocus() => Owner.Select();

        internal void RaiseFocusChanged()
        {
            ToolStrip? root = _ownerItem.RootToolStrip;
            if (root is not null && root.IsHandleCreated && root.SupportsUiaProviders)
            {
                RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            }
        }
    }
}
