// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

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
            _itemAccessibleObjects = new Dictionary<ItemArray.Entry, ListBoxItemAccessibleObject>();
        }

        internal override Rectangle BoundingRectangle => this.TryGetOwnerAs(out ListBox? owner) && owner.IsHandleCreated ?
                                                         owner.GetToolNativeScreenRectangle() : Rectangle.Empty;

        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

        internal override bool IsSelectionRequired => true;

        // We need to provide a unique ID. Others are implementing this in the same manner. First item is static - 0x2a (RuntimeIDFirstItem).
        // Second item can be anything, but it's good to supply HWND.
        internal override int[] RuntimeId
        {
            get
            {
                return !this.TryGetOwnerAs(out ListBox? owner)
                    ? base.RuntimeId
                    : (new int[]
                {
                    RuntimeIDFirstItem,
                    PARAM.ToInt(owner.InternalHandle),
                    owner.GetHashCode()
                });
            }
        }

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

        /// <summary>
        ///  Return the child object at the given screen coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>The accessible object of corresponding element in the provided coordinates.</returns>
        internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
        {
            if (!this.TryGetOwnerAs(out ListBox? owner) || !owner.IsHandleCreated)
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

        /// <summary>
        ///  Returns the element in the specified direction.
        /// </summary>
        /// <param name="direction">Indicates the direction in which to navigate.</param>
        /// <returns>Returns the element in the specified direction.</returns>
        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            int childCount = this.TryGetOwnerAs(out ListBox? owner) ? owner.Items.Count : 0;

            if (childCount == 0)
            {
                return base.FragmentNavigate(direction);
            }

            return direction switch
            {
                UiaCore.NavigateDirection.FirstChild => GetChild(0),
                UiaCore.NavigateDirection.LastChild => GetChild(childCount - 1),
                _ => base.FragmentNavigate(direction),
            };
        }

        internal override UiaCore.IRawElementProviderFragment? GetFocus() => this.TryGetOwnerAs(out ListBox? owner) && owner.IsHandleCreated ? GetFocused() : null;

        /// <summary>
        ///  Gets the accessible property value.
        /// </summary>
        /// <param name="propertyID">The accessible property ID.</param>
        /// <returns>The accessible property value.</returns>
        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
        {
            switch (propertyID)
            {
                case UiaCore.UIA.BoundingRectanglePropertyId:
                    return BoundingRectangle;
                case UiaCore.UIA.ControlTypePropertyId:
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    return this.GetOwnerAccessibleRole() == AccessibleRole.Default
                           ? UiaCore.UIA.ListControlTypeId
                           : base.GetPropertyValue(propertyID);
                case UiaCore.UIA.HasKeyboardFocusPropertyId:
                    return this.TryGetOwnerAs(out ListBox? owner) ? GetChildCount() == 0 && owner.Focused : null;
                default:
                    return base.GetPropertyValue(propertyID);
            }
        }

        internal override UiaCore.IRawElementProviderSimple[] GetSelection()
        {
            AccessibleObject? itemAccessibleObject = GetSelected();
            if (itemAccessibleObject is not null)
            {
                return new UiaCore.IRawElementProviderSimple[]
                {
                    itemAccessibleObject
                };
            }

            return Array.Empty<UiaCore.IRawElementProviderSimple>();
        }

        internal override bool IsIAccessibleExSupported()
        {
            if (this.TryGetOwnerAs(out ListBox? owner))
            {
                return true;
            }

            return base.IsIAccessibleExSupported();
        }

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
        {
            if (patternId == UiaCore.UIA.ScrollPatternId ||
                patternId == UiaCore.UIA.SelectionPatternId)
            {
                return true;
            }

            return base.IsPatternSupported(patternId);
        }

        internal void ResetListItemAccessibleObjects()
        {
            if (OsVersion.IsWindows8OrGreater())
            {
                foreach (ListBoxItemAccessibleObject itemAccessibleObject in _itemAccessibleObjects.Values)
                {
                    UiaCore.UiaDisconnectProvider(itemAccessibleObject);
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

            if (OsVersion.IsWindows8OrGreater())
            {
                UiaCore.UiaDisconnectProvider(value);
            }

            _itemAccessibleObjects.Remove(item);
        }

        internal override void SelectItem()
        {
            if (this.TryGetOwnerAs(out ListBox? owner) && owner.IsHandleCreated)
            {
                GetChild(owner.FocusedIndex)?.SelectItem();
            }
        }

        internal override void SetFocus()
        {
            if (!this.TryGetOwnerAs(out ListBox? owner) || !owner.IsHandleCreated)
            {
                return;
            }

            AccessibleObject? focusedItem = GetFocused();
            focusedItem?.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
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
            if (!this.TryGetOwnerAs(out ListBox? owner) || !owner.IsHandleCreated)
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
                    owner.HasKeyboardFocus = false;
                    return child;
                }
            }

            // Within the ListBox bounds?
            if (Bounds.Contains(x, y))
            {
                owner.HasKeyboardFocus = true;
                return this;
            }

            return null;
        }
    }
}
