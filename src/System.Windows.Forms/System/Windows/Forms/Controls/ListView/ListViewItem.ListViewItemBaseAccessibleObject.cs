// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ListViewItem
{
    /// <summary>
    ///  This class contains the base implementation of properties and methods for ListViewItem accessibility objects.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The implementation of this class fully corresponds to the behavior of the ListViewItem accessibility
    ///   object when the ListView is in "LargeIcon" or "SmallIcon" view.
    ///  </para>
    /// </remarks>
    internal abstract class ListViewItemBaseAccessibleObject : AccessibleObject
    {
        private protected readonly ListView _owningListView;
        private protected readonly ListViewItem _owningItem;

        public ListViewItemBaseAccessibleObject(ListViewItem owningItem)
        {
            _owningItem = owningItem.OrThrowIfNull();
            _owningListView = owningItem.ListView ?? owningItem.Group?.ListView ?? throw new InvalidOperationException(nameof(owningItem.ListView));
        }

        private protected ListViewGroup? OwningGroup => _owningListView.GroupsDisplayed
            ? _owningItem.Group ?? _owningListView.DefaultGroup
            : null;

        private protected override string AutomationId
            => $"{nameof(ListViewItem)}-{CurrentIndex}";

        public override Rectangle Bounds
            => !_owningListView.IsHandleCreated || OwningGroup?.CollapsedState == ListViewGroupCollapsedState.Collapsed
                ? Rectangle.Empty
                : new Rectangle(
                    _owningListView.AccessibilityObject.Bounds.X + _owningItem.Bounds.X,
                    _owningListView.AccessibilityObject.Bounds.Y + _owningItem.Bounds.Y,
                    _owningItem.Bounds.Width,
                    _owningItem.Bounds.Height);

        internal int CurrentIndex
            => _owningItem.Index;

        internal virtual int FirstSubItemIndex => 0;

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => _owningListView.AccessibilityObject;

        internal bool HasImage => _owningItem.ImageList is not null && _owningItem.ImageList.Images.Count > 0
            && _owningItem.ImageIndex != ImageList.Indexer.DefaultIndex;

        internal override bool IsItemSelected
            => (State & AccessibleStates.Selected) != 0;

        public override string? Name => _owningItem.Text;

        internal override bool CanGetNameInternal => false;

        private bool OwningListItemFocused
        {
            get
            {
                bool owningListViewFocused = _owningListView.Focused;
                bool owningListItemFocused = _owningListView.FocusedItem == _owningItem;
                return owningListViewFocused && owningListItemFocused;
            }
        }

        public override AccessibleRole Role
            => _owningListView.CheckBoxes
                ? AccessibleRole.CheckButton
                : AccessibleRole.ListItem;

        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable | AccessibleStates.MultiSelectable;

                if (_owningListView.SelectedIndices.Contains(_owningItem.Index))
                {
                    return state |= AccessibleStates.Selected | AccessibleStates.Focused;
                }

                return state |= _owningListView.AccessibilityObject.SystemIAccessible.TryGetState(GetChildId());
            }
        }

        protected abstract View View { get; }

        internal override void AddToSelection() => SelectItem();

        public override string DefaultAction
        {
            get
            {
                if (_owningListView.CheckBoxes)
                {
                    return _owningItem.Checked
                        ? SR.AccessibleActionUncheck
                        : SR.AccessibleActionCheck;
                }

                return SR.AccessibleActionDoubleClick;
            }
        }

        private protected override bool IsInternal => true;

        internal override bool CanGetDefaultActionInternal => false;

        public override void DoDefaultAction()
        {
            if (_owningListView.CheckBoxes)
            {
                Toggle();
            }

            SetFocus();
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            AccessibleObject parentInternal = OwningGroup?.AccessibilityObject ?? _owningListView.AccessibilityObject;

            switch (direction)
            {
                case NavigateDirection.NavigateDirection_Parent:
                    return parentInternal;
                case NavigateDirection.NavigateDirection_NextSibling:
                    int childIndex = parentInternal.GetChildIndex(this);
                    return childIndex == InvalidIndex ? null : parentInternal.GetChild(childIndex + 1);
                case NavigateDirection.NavigateDirection_PreviousSibling:
                    return parentInternal.GetChild(parentInternal.GetChildIndex(this) - 1);
                case NavigateDirection.NavigateDirection_FirstChild:
                case NavigateDirection.NavigateDirection_LastChild:
                    return null;
            }

            return base.FragmentNavigate(direction);
        }

        public override AccessibleObject? GetChild(int index)
        {
            if (_owningListView.View != View)
            {
                throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewException, View.ToString()));
            }

            return null;
        }

        internal virtual AccessibleObject? GetChildInternal(int index) => GetChild(index);

        public override int GetChildCount()
        {
            if (_owningListView.View != View)
            {
                throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewException, View.ToString()));
            }

            return InvalidIndex;
        }

        internal override int GetChildIndex(AccessibleObject? child) => InvalidIndex;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
        {
            switch (propertyID)
            {
                case UIA_PROPERTY_ID.UIA_ControlTypePropertyId:
                    return (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ListItemControlTypeId;
                case UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId:
                    return (VARIANT)OwningListItemFocused;
                case UIA_PROPERTY_ID.UIA_IsEnabledPropertyId:
                    return (VARIANT)_owningListView.Enabled;
                case UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId:
                    return (VARIANT)State.HasFlag(AccessibleStates.Focusable);
                case UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId:
                    if (OwningGroup?.CollapsedState == ListViewGroupCollapsedState.Collapsed)
                    {
                        return VARIANT.True;
                    }

                    VARIANT result = base.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId);
                    return result.IsEmpty ? VARIANT.False : result;
                case UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId:
                    return UIAHelper.WindowHandleToVariant(HWND.Null);
                default:
                    return base.GetPropertyValue(propertyID);
            }
        }

        internal virtual Rectangle GetSubItemBounds(int index) => Rectangle.Empty;

        internal override int[] RuntimeId
        {
            get
            {
                int[] id = _owningListView.AccessibilityObject.RuntimeId;

                Debug.Assert(id.Length >= 2);

                return
                [
                    id[0],
                    id[1],
                    // Win32-control specific RuntimeID constant.
                    4,
                    // RuntimeId uses hash code instead of item's index. When items are removed,
                    // indexes of below items shift. But when UiaDisconnectProvider is called for item
                    // with updated index, it in fact disconnects the item which had the index initially,
                    // apparently because of lack of synchronization with RuntimeId updates.
                    // Similar applies for items within a group, where adding the group's index
                    // was preventing from correct disconnection of items on removal.
                    _owningItem.GetHashCode()
                ];
            }
        }

        internal override ToggleState ToggleState
            => _owningItem.Checked
                ? ToggleState.ToggleState_On
                : ToggleState.ToggleState_Off;

        /// <summary>
        ///  Indicates whether specified pattern is supported.
        /// </summary>
        /// <param name="patternId">The pattern ID.</param>
        /// <returns>True if specified </returns>
        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_ScrollItemPatternId => true,
                UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId => true,
                UIA_PATTERN_ID.UIA_SelectionItemPatternId => true,
                UIA_PATTERN_ID.UIA_InvokePatternId => true,
                UIA_PATTERN_ID.UIA_TogglePatternId => _owningListView.CheckBoxes,
                _ => base.IsPatternSupported(patternId)
            };

        internal virtual void ReleaseChildUiaProviders()
        {
            foreach (ListViewSubItem subItem in _owningItem.SubItems)
            {
                subItem.ReleaseUiaProvider();
            }
        }

        internal override void RemoveFromSelection()
        {
            // Do nothing, C++ implementation returns UIA_E_INVALIDOPERATION 0x80131509
        }

        internal override IRawElementProviderSimple.Interface ItemSelectionContainer
            => _owningListView.AccessibilityObject;

        internal override void ScrollIntoView() => _owningItem.EnsureVisible();

        internal override unsafe void SelectItem()
        {
            if (_owningListView.IsHandleCreated)
            {
                _owningListView.SelectedIndices.Add(CurrentIndex);
                PInvoke.InvalidateRect(_owningListView, lpRect: null, bErase: false);
            }

            RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            RaiseAutomationEvent(UIA_EVENT_ID.UIA_SelectionItem_ElementSelectedEventId);
        }

        internal override void SetFocus()
        {
            RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            SelectItem();
        }

        public override void Select(AccessibleSelection flags)
        {
            if (!_owningListView.IsHandleCreated)
            {
                return;
            }

            _owningListView.AccessibilityObject.SystemIAccessible.TrySelect(flags, GetChildId());

            // In Everett, the ListBox accessible children did not have any selection capability.
            // In Whidbey, they delegate the selection capability to OLEACC.
            // However, OLEACC does not deal w/ several Selection flags: ExtendSelection, AddSelection, RemoveSelection.
            // OLEACC instead throws an ArgumentException.
            // Since Whidbey API's should not throw an exception in places where Everett API's did not, we catch
            // the ArgumentException and fail silently.
        }

        internal override void Toggle() => _owningItem.Checked = !_owningItem.Checked;
    }
}
