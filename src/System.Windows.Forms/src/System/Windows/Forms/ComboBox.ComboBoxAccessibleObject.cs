// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static System.Windows.Forms.ComboBox.ObjectCollection;
using static Interop;

namespace System.Windows.Forms;

public partial class ComboBox
{
    /// <summary>
    ///  ComboBox control accessible object with UI Automation provider functionality.
    /// </summary>
    internal class ComboBoxAccessibleObject : ControlAccessibleObject
    {
        private const int COMBOBOX_ACC_ITEM_INDEX = 1;

        private ComboBoxChildDropDownButtonUiaProvider? _dropDownButtonUiaProvider;
        private WeakReference<ComboBox> owningComboBox;

        /// <summary>
        ///  Initializes new instance of ComboBoxAccessibleObject.
        /// </summary>
        /// <param name="owningComboBox">The owning ComboBox control.</param>
        public ComboBoxAccessibleObject(ComboBox owningComboBox): base(owningComboBox)
        {
            this.owningComboBox = new WeakReference<ComboBox>(owningComboBox);
            ItemAccessibleObjects = new ComboBoxItemAccessibleObjectCollection(owningComboBox);
        }

        public ComboBox? OwningComboBox
        {
            get
            {
                return owningComboBox.TryGetTarget(out ComboBox? comboBox) ? comboBox : null;
            }
        }

        private void ComboBoxDefaultAction(bool expand)
        {
            if (this.TryGetOwnerAs(out ComboBox? owner) && owner.IsHandleCreated && owner.DroppedDown != expand)
            {
                owner.DroppedDown = expand;
            }
        }

        internal override bool IsIAccessibleExSupported()
            => this.TryGetOwnerAs(out ComboBox? owner) || base.IsIAccessibleExSupported();

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
        {
            if (patternId == UiaCore.UIA.ExpandCollapsePatternId && this.TryGetOwnerAs(out ComboBox? owner))
            {
                return owner.DropDownStyle != ComboBoxStyle.Simple;
            }

            return patternId == UiaCore.UIA.ValuePatternId ? true : base.IsPatternSupported(patternId);
        }

        internal override int[] RuntimeId
            => this.TryGetOwnerAs(out ComboBox? owner)
                ? new int[]
                {
                   // We need to provide a unique ID. Others are implementing this in the same manner. First item is
                   // static - 0x2a (RuntimeIDFirstItem). Second item can be anything, but it's good to supply HWND.
                   RuntimeIDFirstItem,
                   PARAM.ToInt(owner.InternalHandle),
                   owner.GetHashCode()
                }
                : new int[] { RuntimeIDFirstItem, 0, 0 };

        internal override void Expand() => ComboBoxDefaultAction(true);

        internal override void Collapse() => ComboBoxDefaultAction(false);

        internal override UiaCore.ExpandCollapseState ExpandCollapseState
            => this.TryGetOwnerAs(out ComboBox? owner) && owner.IsHandleCreated && owner.DroppedDown
                ? UiaCore.ExpandCollapseState.Expanded
                : UiaCore.ExpandCollapseState.Collapsed;

        internal override string? get_accNameInternal(object childID)
        {
            ValidateChildID(ref childID);

            if ((int)childID == COMBOBOX_ACC_ITEM_INDEX)
            {
                return Name;
            }

            return base.get_accNameInternal(childID);
        }

        internal override string? get_accKeyboardShortcutInternal(object childID)
        {
            ValidateChildID(ref childID);
            if ((int)childID == COMBOBOX_ACC_ITEM_INDEX)
            {
                return KeyboardShortcut;
            }

            return base.get_accKeyboardShortcutInternal(childID);
        }

        /// <summary>
        ///  Gets the collection of item accessible objects.
        /// </summary>
        public ComboBoxItemAccessibleObjectCollection ItemAccessibleObjects { get; }

        /// <summary>
        ///  Gets the DropDown button accessible object. (UI Automation provider)
        /// </summary>
        public ComboBoxChildDropDownButtonUiaProvider? DropDownButtonUiaProvider
            => _dropDownButtonUiaProvider ??= this.TryGetOwnerAs(out ComboBox? owner) ? new ComboBoxChildDropDownButtonUiaProvider(owner) : null;

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (!this.TryGetOwnerAs(out ComboBox? owner) || !owner.IsHandleCreated)
            {
                return null;
            }

            switch (direction)
            {
                case UiaCore.NavigateDirection.FirstChild:
                    return GetFirstChild();
                case UiaCore.NavigateDirection.LastChild:
                    return GetLastChild();
                default:
                    return base.FragmentNavigate(direction);
            }
        }

        internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot => this;

        public override string DefaultAction
        {
            get
            {
                if (this.TryGetOwnerAs(out ComboBox? owner))
                {
                    string? defaultAction = owner.AccessibleDefaultActionDescription;
                    if (defaultAction is not null)
                    {
                        return defaultAction;
                    }

                    if (!owner.IsHandleCreated || owner.DropDownStyle == ComboBoxStyle.Simple)
                    {
                        return string.Empty;
                    }

                    return owner.DroppedDown ? SR.AccessibleActionCollapse : SR.AccessibleActionExpand;
                }

                return string.Empty;
            }
        }

        internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
            propertyID switch
            {
                UiaCore.UIA.ControlTypePropertyId =>
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    this.GetOwnerAccessibleRole() == AccessibleRole.Default
                        ? UiaCore.UIA.ComboBoxControlTypeId
                        : base.GetPropertyValue(propertyID),
                UiaCore.UIA.HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out ComboBox? owner) && owner.Focused,
                _ => base.GetPropertyValue(propertyID)
            };

        internal void RemoveListItemAccessibleObjectAt(int index)
        {
            if (this.TryGetOwnerAs(out ComboBox? owner))
            {
                IReadOnlyList<Entry> entries = owner.Items.InnerList;
                Debug.Assert(index < entries.Count);

                Entry item = entries[index];
                if (!ItemAccessibleObjects.ContainsKey(item))
                {
                    return;
                }

                if (OsVersion.IsWindows8OrGreater())
                {
                    UiaCore.UiaDisconnectProvider(ItemAccessibleObjects[item]);
                }

                ItemAccessibleObjects.Remove(item);
            }
        }

        internal void ReleaseDropDownButtonUiaProvider()
        {
            if (OsVersion.IsWindows8OrGreater())
            {
                UiaCore.UiaDisconnectProvider(_dropDownButtonUiaProvider);
            }

            _dropDownButtonUiaProvider = null;
        }

        internal void ResetListItemAccessibleObjects()
        {
            if (OsVersion.IsWindows8OrGreater())
            {
                foreach (ComboBoxItemAccessibleObject itemAccessibleObject in ItemAccessibleObjects.Values)
                {
                    UiaCore.UiaDisconnectProvider(itemAccessibleObject);
                }
            }

            ItemAccessibleObjects.Clear();
        }

        internal void SetComboBoxItemFocus()
        {
            if (!this.TryGetOwnerAs(out ComboBox? owner) || !owner.IsHandleCreated)
            {
                return;
            }

            GetSelectedComboBoxItemAccessibleObject()?.SetFocus();
        }

        internal void SetComboBoxItemSelection()
        {
            if (!this.TryGetOwnerAs(out ComboBox? owner) || !owner.IsHandleCreated)
            {
                return;
            }

            GetSelectedComboBoxItemAccessibleObject()?.RaiseAutomationEvent(UiaCore.UIA.SelectionItem_ElementSelectedEventId);
        }

        internal override void SetFocus()
        {
            if (!this.TryGetOwnerAs(out ComboBox? owner) || !owner.IsHandleCreated)
            {
                return;
            }

            base.SetFocus();

            RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
        }

        private ComboBoxItemAccessibleObject? GetSelectedComboBoxItemAccessibleObject()
        {
            if (this.TryGetOwnerAs(out ComboBox? owner))
            {
                // We should use the SelectedIndex property instead of the SelectedItem to avoid the problem of getting
                // the incorrect item when the list contains duplicate items https://github.com/dotnet/winforms/issues/3590
                int selectedIndex = owner.SelectedIndex;

                if (selectedIndex < 0 || selectedIndex > owner.Items.Count - 1)
                {
                    return null;
                }

                Entry selectedItem = owner.Entries[selectedIndex];
                return ItemAccessibleObjects.GetComboBoxItemAccessibleObject(selectedItem);
            }
            else
            {
                return null;
            }
        }

        private AccessibleObject? GetFirstChild()
        {
            if (this.TryGetOwnerAs(out ComboBox? owner) && owner.DroppedDown)
            {
                return owner.ChildListAccessibleObject;
            }

            return owner?.DropDownStyle switch
            {
                ComboBoxStyle.DropDown => owner.ChildEditAccessibleObject,
                ComboBoxStyle.DropDownList => owner.ChildTextAccessibleObject,
                ComboBoxStyle.Simple => null,
                _ => null
            };
        }

        private AccessibleObject? GetLastChild() =>
             this.TryGetOwnerAs(out ComboBox? owner) && owner.DropDownStyle == ComboBoxStyle.Simple
                   ? owner.ChildEditAccessibleObject
                   : DropDownButtonUiaProvider;

        public override void DoDefaultAction()
        {
            if (!this.TryGetOwnerAs(out ComboBox? owner) || !owner.IsHandleCreated || owner.DropDownStyle == ComboBoxStyle.Simple)
            {
                return;
            }

            owner.DroppedDown = !owner.DroppedDown;
        }
    }
}
