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
        private readonly ComboBox _owningComboBox;

        /// <summary>
        ///  Initializes new instance of ComboBoxAccessibleObject.
        /// </summary>
        /// <param name="owningComboBox">The owning ComboBox control.</param>
        public ComboBoxAccessibleObject(ComboBox owningComboBox) : base(owningComboBox)
        {
            _owningComboBox = owningComboBox;
            ItemAccessibleObjects = new ComboBoxItemAccessibleObjectCollection(owningComboBox);
        }

        private void ComboBoxDefaultAction(bool expand)
        {
            if (_owningComboBox.IsHandleCreated && _owningComboBox.DroppedDown != expand)
            {
                _owningComboBox.DroppedDown = expand;
            }
        }

        internal override bool IsIAccessibleExSupported()
            => _owningComboBox is not null || base.IsIAccessibleExSupported();

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
        {
            if (patternId == UiaCore.UIA.ExpandCollapsePatternId)
            {
                return _owningComboBox.DropDownStyle != ComboBoxStyle.Simple;
            }

            return patternId == UiaCore.UIA.ValuePatternId ? true : base.IsPatternSupported(patternId);
        }

        internal override int[] RuntimeId
            => new int[]
            {
                // We need to provide a unique ID. Others are implementing this in the same manner. First item is
                // static - 0x2a (RuntimeIDFirstItem). Second item can be anything, but it's good to supply HWND.
                RuntimeIDFirstItem,
                PARAM.ToInt(_owningComboBox.InternalHandle),
                _owningComboBox.GetHashCode()
            };

        internal override void Expand() => ComboBoxDefaultAction(true);

        internal override void Collapse() => ComboBoxDefaultAction(false);

        internal override UiaCore.ExpandCollapseState ExpandCollapseState
            => _owningComboBox.IsHandleCreated && _owningComboBox.DroppedDown
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
        public ComboBoxChildDropDownButtonUiaProvider DropDownButtonUiaProvider
            => _dropDownButtonUiaProvider ??= new ComboBoxChildDropDownButtonUiaProvider(_owningComboBox);

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (!_owningComboBox.IsHandleCreated)
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
                string? defaultAction = _owningComboBox.AccessibleDefaultActionDescription;
                if (defaultAction is not null)
                {
                    return defaultAction;
                }

                if (!_owningComboBox.IsHandleCreated || _owningComboBox.DropDownStyle == ComboBoxStyle.Simple)
                {
                    return string.Empty;
                }

                return _owningComboBox.DroppedDown ? SR.AccessibleActionCollapse : SR.AccessibleActionExpand;
            }
        }

        internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
            propertyID switch
            {
                UiaCore.UIA.ControlTypePropertyId =>
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    _owningComboBox.AccessibleRole == AccessibleRole.Default
                        ? UiaCore.UIA.ComboBoxControlTypeId
                        : base.GetPropertyValue(propertyID),
                UiaCore.UIA.HasKeyboardFocusPropertyId => _owningComboBox.Focused,
                _ => base.GetPropertyValue(propertyID)
            };

        internal void RemoveListItemAccessibleObjectAt(int index)
        {
            IReadOnlyList<Entry> entries = _owningComboBox.Items.InnerList;
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
            if (!_owningComboBox.IsHandleCreated)
            {
                return;
            }

            GetSelectedComboBoxItemAccessibleObject()?.SetFocus();
        }

        internal void SetComboBoxItemSelection()
        {
            if (!_owningComboBox.IsHandleCreated)
            {
                return;
            }

            GetSelectedComboBoxItemAccessibleObject()?.RaiseAutomationEvent(UiaCore.UIA.SelectionItem_ElementSelectedEventId);
        }

        internal override void SetFocus()
        {
            if (!_owningComboBox.IsHandleCreated)
            {
                return;
            }

            base.SetFocus();

            RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
        }

        private ComboBoxItemAccessibleObject? GetSelectedComboBoxItemAccessibleObject()
        {
            // We should use the SelectedIndex property instead of the SelectedItem to avoid the problem of getting
            // the incorrect item when the list contains duplicate items https://github.com/dotnet/winforms/issues/3590
            int selectedIndex = _owningComboBox.SelectedIndex;

            if (selectedIndex < 0 || selectedIndex > _owningComboBox.Items.Count - 1)
            {
                return null;
            }

            Entry selectedItem = _owningComboBox.Entries[selectedIndex];
            return ItemAccessibleObjects.GetComboBoxItemAccessibleObject(selectedItem);
        }

        private AccessibleObject? GetFirstChild()
        {
            if (_owningComboBox.DroppedDown)
            {
                return _owningComboBox.ChildListAccessibleObject;
            }

            return _owningComboBox.DropDownStyle switch
            {
                ComboBoxStyle.DropDown => _owningComboBox.ChildEditAccessibleObject,
                ComboBoxStyle.DropDownList => _owningComboBox.ChildTextAccessibleObject,
                ComboBoxStyle.Simple => null,
                _ => null
            };
        }

        private AccessibleObject? GetLastChild() =>
            _owningComboBox.DropDownStyle == ComboBoxStyle.Simple
                ? _owningComboBox.ChildEditAccessibleObject
                : DropDownButtonUiaProvider;

        public override void DoDefaultAction()
        {
            if (!_owningComboBox.IsHandleCreated || _owningComboBox.DropDownStyle == ComboBoxStyle.Simple)
            {
                return;
            }

            _owningComboBox.DroppedDown = !_owningComboBox.DroppedDown;
        }
    }
}
