// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Automation;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ListBox
{
    /// <summary>
    ///  ListBox control accessible object with UI Automation provider functionality.
    /// </summary>
    internal class ListBoxAccessibleObject : ControlAccessibleObject
    {
        private readonly Dictionary<ItemArray.Entry, ListBoxItemAccessibleObject> _itemAccessibleObjects;

        /// <summary>
        ///  Initializes new instance of ListBoxAccessibleObject.
        /// </summary>
        /// <param name="owningListBox">The owning ListBox control.</param>
        public ListBoxAccessibleObject(ListBox owningListBox) : base(owningListBox)
        {
            _itemAccessibleObjects = [];
        }

        private protected override bool IsInternal => true;

        internal override Rectangle BoundingRectangle => this.IsOwnerHandleCreated(out ListBox? owner) ?
            owner.GetToolNativeScreenRectangle() : Rectangle.Empty;

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => this;

        internal override bool IsSelectionRequired
            => this.IsOwnerHandleCreated(out ListBox? owner) && owner.SelectionMode != SelectionMode.None;

        internal override bool CanSelectMultiple
            => this.IsOwnerHandleCreated(out ListBox? owner)
                && owner.SelectionMode != SelectionMode.One
                && owner.SelectionMode != SelectionMode.None;

        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = AccessibleStates.Focusable;
                if (this.TryGetOwnerAs(out ListBox? owner) && owner.Focused)
                {
                    state |= AccessibleStates.Focused;
                }

                return state;
            }
        }

        private protected virtual ListBoxItemAccessibleObject CreateItemAccessibleObject(ListBox listBox, ItemArray.Entry item)
            => new(listBox, item, this);

        internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
        {
            if (!this.IsOwnerHandleCreated(out ListBox? _))
            {
                return base.ElementProviderFromPoint(x, y);
            }

            AccessibleObject? element = HitTest((int)x, (int)y);

            if (element is not null)
            {
                return element;
            }

            return base.ElementProviderFromPoint(x, y);
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            int childCount = this.TryGetOwnerAs(out ListBox? owner) ? owner.Items.Count : 0;

            if (childCount == 0)
            {
                return base.FragmentNavigate(direction);
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild => GetChild(0),
                NavigateDirection.NavigateDirection_LastChild => GetChild(childCount - 1),
                _ => base.FragmentNavigate(direction),
            };
        }

        internal override IRawElementProviderFragment.Interface? GetFocus() => this.IsOwnerHandleCreated(out ListBox? _) ? GetFocused() : null;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) => propertyID switch
        {
            UIA_PROPERTY_ID.UIA_BoundingRectanglePropertyId => UiaTextProvider.BoundingRectangleAsVariant(BoundingRectangle),

            // If we don't set a default role for the accessible object it will be retrieved from Windows.
            // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
            UIA_PROPERTY_ID.UIA_ControlTypePropertyId => this.GetOwnerAccessibleRole() == AccessibleRole.Default
                ? (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ListControlTypeId
                : base.GetPropertyValue(propertyID),
            UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out ListBox? owner)
                ? (VARIANT)(GetChildCount() == 0 && owner.Focused)
                : base.GetPropertyValue(propertyID),
            _ => base.GetPropertyValue(propertyID),
        };

        internal override IRawElementProviderSimple.Interface[] GetSelection()
        {
            AccessibleObject? itemAccessibleObject = GetSelected();
            return itemAccessibleObject is not null
                ? [itemAccessibleObject]
                : [];
        }

        internal override bool IsIAccessibleExSupported()
            => this.TryGetOwnerAs(out ListBox? _) || base.IsIAccessibleExSupported();

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId == UIA_PATTERN_ID.UIA_ScrollPatternId ||
            patternId == UIA_PATTERN_ID.UIA_SelectionPatternId ||
            base.IsPatternSupported(patternId);

        internal void ResetListItemAccessibleObjects()
        {
            if (OsVersion.IsWindows8OrGreater())
            {
                foreach (ListBoxItemAccessibleObject itemAccessibleObject in _itemAccessibleObjects.Values)
                {
                    PInvoke.UiaDisconnectProvider(itemAccessibleObject, skipOSCheck: true);
                }
            }

            _itemAccessibleObjects.Clear();
        }

        internal void RemoveListItemAccessibleObjectAt(int index)
        {
            if (!this.TryGetOwnerAs(out ListBox? owner))
            {
                return;
            }

            IReadOnlyList<ItemArray.Entry?> entries = owner.Items.InnerArray.Entries;
            if (index >= entries.Count)
            {
                return;
            }

            ItemArray.Entry? item = entries[index];

            if (item is null || !_itemAccessibleObjects.TryGetValue(item, out ListBoxItemAccessibleObject? value))
            {
                return;
            }

            PInvoke.UiaDisconnectProvider(value);

            _itemAccessibleObjects.Remove(item);
        }

        internal override void SelectItem()
        {
            if (this.IsOwnerHandleCreated(out ListBox? owner))
            {
                GetChild(owner.FocusedIndex)?.SelectItem();
            }
        }

        internal override void SetFocus()
        {
            if (!this.IsOwnerHandleCreated(out ListBox? _))
            {
                return;
            }

            AccessibleObject? focusedItem = GetFocused();
            focusedItem?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            focusedItem?.SetFocus();
        }

        public override AccessibleObject? GetChild(int index)
        {
            if (!this.TryGetOwnerAs(out ListBox? owner))
            {
                return null;
            }

            if (index < 0 || index >= owner.Items.Count || owner.Items.InnerArray.Count == 0)
            {
                return null;
            }

            ItemArray.Entry? item = owner.Items.InnerArray.Entries[index];

            if (item is null)
            {
                return null;
            }

            if (!_itemAccessibleObjects.TryGetValue(item, out ListBoxItemAccessibleObject? value))
            {
                value = CreateItemAccessibleObject(owner, item);
                _itemAccessibleObjects.Add(item, value);
            }

            return value;
        }

        public override int GetChildCount()
        {
            return this.TryGetOwnerAs(out ListBox? owner) ? owner.Items.Count : 0;
        }

        public override AccessibleObject? GetFocused()
        {
            if (this.TryGetOwnerAs(out ListBox? owner))
            {
                int index = owner.FocusedIndex;
                if (index >= 0)
                {
                    return GetChild(index);
                }
            }

            return null;
        }

        public override AccessibleObject? GetSelected()
        {
            if (this.TryGetOwnerAs(out ListBox? owner))
            {
                int index = owner.SelectedIndex;

                if (index >= 0)
                {
                    return GetChild(index);
                }
            }

            return null;
        }

        public override AccessibleObject? HitTest(int x, int y)
        {
            if (!this.IsOwnerHandleCreated(out ListBox? _))
            {
                return null;
            }

            // Within a child element?
            int count = GetChildCount();
            for (int index = 0; index < count; ++index)
            {
                AccessibleObject? child = GetChild(index);
                Debug.Assert(child is not null, $"GetChild({index}) returned null");
                if (child is not null && child.Bounds.Contains(x, y))
                {
                    return child;
                }
            }

            // Within the ListBox bounds?
            if (Bounds.Contains(x, y))
            {
                return this;
            }

            return null;
        }
    }
}
