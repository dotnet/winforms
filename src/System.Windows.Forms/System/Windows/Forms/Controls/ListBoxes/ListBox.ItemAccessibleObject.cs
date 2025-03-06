// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ListBox
{
    /// <summary>
    ///  ListBox item control accessible object with UI Automation provider functionality.
    /// </summary>
    internal class ListBoxItemAccessibleObject : AccessibleObject
    {
        private readonly ItemArray.Entry _itemEntry;
        private readonly ListBoxAccessibleObject _owningAccessibleObject;
        private readonly ListBox _owningListBox;

        public ListBoxItemAccessibleObject(ListBox owningListBox, ItemArray.Entry itemEntry, ListBoxAccessibleObject owningAccessibleObject)
        {
            _owningListBox = owningListBox.OrThrowIfNull();
            _itemEntry = itemEntry.OrThrowIfNull();
            _owningAccessibleObject = owningAccessibleObject.OrThrowIfNull();
        }

        protected int CurrentIndex => _owningListBox.Items.InnerArray.IndexOf(_itemEntry);

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => _owningAccessibleObject;

        internal override bool IsItemSelected => State.HasFlag(AccessibleStates.Selected);

        internal override IRawElementProviderSimple.Interface ItemSelectionContainer
            => _owningAccessibleObject;

        public override AccessibleObject Parent => _owningAccessibleObject;

        private protected override bool IsInternal => true;

        internal override int[] RuntimeId
        {
            get
            {
                int[] id = _owningAccessibleObject.RuntimeId;

                Debug.Assert(id.Length >= 3);

                return [id[0], id[1], id[2], _itemEntry.GetHashCode()];
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                if (!_owningListBox.IsHandleCreated)
                {
                    return Rectangle.Empty;
                }

                Rectangle bounds = _owningListBox.GetItemRectangle(CurrentIndex);

                if (bounds.IsEmpty)
                {
                    return bounds;
                }

                bounds = _owningListBox.RectangleToScreen(bounds);
                Rectangle visibleArea = _owningListBox.RectangleToScreen(_owningListBox.ClientRectangle);

                if (visibleArea.Bottom < bounds.Bottom)
                {
                    bounds.Height = visibleArea.Bottom - bounds.Top;
                }

                bounds.Width = visibleArea.Width;

                return bounds;
            }
        }

        public override string? DefaultAction => GetDefaultActionInternal().ToNullableStringAndFree();

        internal override BSTR GetDefaultActionInternal() =>
            _owningAccessibleObject.SystemIAccessible.TryGetDefaultAction(GetChildId());

        public override string? Help => GetHelpInternal().ToNullableStringAndFree();

        internal override BSTR GetHelpInternal() => _owningAccessibleObject.SystemIAccessible.TryGetHelp(GetChildId());

        public override string? Name => _owningListBox.GetItemText(_itemEntry.Item);

        internal override bool CanGetNameInternal => false;

        public override AccessibleRole Role => _owningAccessibleObject.SystemIAccessible.TryGetRole(GetChildId());

        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;

                if (_owningListBox.SelectedIndex == CurrentIndex)
                {
                    return state |= AccessibleStates.Selected | AccessibleStates.Focused;
                }

                return state |= _owningAccessibleObject.SystemIAccessible.TryGetState(GetChildId());
            }
        }

        internal override void AddToSelection()
        {
            if (_owningListBox.IsHandleCreated)
            {
                SelectItem();
            }
        }

        public override void DoDefaultAction()
        {
            if (_owningListBox.IsHandleCreated)
            {
                SetFocus();
            }
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            int firstItemIndex = 0;
            int lastItemIndex = _owningListBox.Items.Count - 1;
            int currentIndex = CurrentIndex;

            switch (direction)
            {
                case NavigateDirection.NavigateDirection_Parent:
                    return _owningListBox.AccessibilityObject;
                case NavigateDirection.NavigateDirection_PreviousSibling:
                    if (currentIndex > firstItemIndex && currentIndex <= lastItemIndex)
                    {
                        return _owningAccessibleObject.GetChild(currentIndex - 1);
                    }

                    return null;
                case NavigateDirection.NavigateDirection_NextSibling:
                    if (currentIndex >= firstItemIndex && currentIndex < lastItemIndex)
                    {
                        return _owningAccessibleObject.GetChild(currentIndex + 1);
                    }

                    return null;
            }

            return base.FragmentNavigate(direction);
        }

        internal override int GetChildId()
        {
            // Index is zero-based, Child ID is 1-based.
            return CurrentIndex + 1;
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
             => propertyID switch
             {
                 UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ListItemControlTypeId,
                 UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(_owningListBox.Focused && _owningListBox.FocusedIndex == CurrentIndex),
                 UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)_owningListBox.Enabled,
                 UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
                 UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId => UIAHelper.WindowHandleToVariant(HWND.Null),
                 _ => base.GetPropertyValue(propertyID)
             };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) => patternId switch
        {
            UIA_PATTERN_ID.UIA_ScrollItemPatternId
                or UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId
                or UIA_PATTERN_ID.UIA_SelectionItemPatternId => true,
            _ => base.IsPatternSupported(patternId),
        };

        internal override void RemoveFromSelection()
        {
            if (!_owningListBox.IsHandleCreated
                || _owningListBox.SelectionMode == SelectionMode.None
                || _owningListBox.SelectionMode == SelectionMode.One
                || !IsItemSelected)
            {
                return;
            }

            _owningListBox.SetSelected(CurrentIndex, value: false);
        }

        internal override void ScrollIntoView()
        {
            if (!_owningListBox.IsHandleCreated)
            {
                return;
            }

            int currentIndex = CurrentIndex;

            if (_owningListBox.SelectedIndex == -1) // no item selected
            {
                PInvokeCore.SendMessage(_owningListBox, PInvoke.LB_SETCARETINDEX, (WPARAM)currentIndex);
                return;
            }

            int firstVisibleIndex = (int)PInvokeCore.SendMessage(_owningListBox, PInvoke.LB_GETTOPINDEX);
            if (currentIndex < firstVisibleIndex)
            {
                PInvokeCore.SendMessage(_owningListBox, PInvoke.LB_SETTOPINDEX, (WPARAM)currentIndex);
                return;
            }

            int itemsHeightSum = 0;
            int listBoxHeight = _owningListBox.ClientRectangle.Height;
            int itemsCount = _owningListBox.Items.Count;

            for (int i = firstVisibleIndex; i < itemsCount; i++)
            {
                int itemHeight = (int)PInvokeCore.SendMessage(_owningListBox, PInvoke.LB_GETITEMHEIGHT, (WPARAM)i);

                if ((itemsHeightSum += itemHeight) <= listBoxHeight)
                {
                    continue;
                }

                int lastVisibleIndex = i - 1; // - 1 because last "i" index is invisible
                int visibleItemsCount = lastVisibleIndex - firstVisibleIndex + 1; // + 1 because array indexes begin with 0

                if (currentIndex > lastVisibleIndex)
                {
                    PInvokeCore.SendMessage(_owningListBox, PInvoke.LB_SETTOPINDEX, (WPARAM)(currentIndex - visibleItemsCount + 1));
                }

                break;
            }
        }

        internal override unsafe void SelectItem()
        {
            if (!_owningListBox.IsHandleCreated)
            {
                return;
            }

            _owningListBox.SelectedIndex = CurrentIndex;

            PInvoke.InvalidateRect(_owningListBox, lpRect: null, bErase: false);
            RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            RaiseAutomationEvent(UIA_EVENT_ID.UIA_SelectionItem_ElementSelectedEventId);
        }

        internal override void SetFocus()
        {
            if (!_owningListBox.IsHandleCreated)
            {
                return;
            }

            RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            SelectItem();
        }

        public override void Select(AccessibleSelection flags)
            => _owningAccessibleObject.SystemIAccessible.TrySelect(flags, GetChildId());

        // In .NET Framework 1.1, the ListBox accessible children did not have any selection capability.
        // In .NET Framework 2.0, they delegate the selection capability to OLEACC.
        // However, OLEACC does not deal with several selection flags:
        // ExtendSelection, AddSelection, RemoveSelection.
        // OLEACC instead throws an ArgumentException.
        // Since .NET Framework 2.0 API's should not throw an exception in places where
        // .NET Framework 1.1 API's did not, we catch the ArgumentException and fail silently.
    }
}
