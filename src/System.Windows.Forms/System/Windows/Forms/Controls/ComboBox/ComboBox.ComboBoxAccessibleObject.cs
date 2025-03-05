// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ComboBox.ObjectCollection;

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

        /// <summary>
        ///  Initializes new instance of ComboBoxAccessibleObject.
        /// </summary>
        /// <param name="owningComboBox">The owning ComboBox control.</param>
        public ComboBoxAccessibleObject(ComboBox owningComboBox) : base(owningComboBox)
        {
            ItemAccessibleObjects = new ComboBoxItemAccessibleObjectCollection(this);
        }

        private void ComboBoxDefaultAction(bool expand)
        {
            if (this.IsOwnerHandleCreated(out ComboBox? owner) && owner.DroppedDown != expand)
            {
                owner.DroppedDown = expand;
            }
        }

        internal override bool IsIAccessibleExSupported()
            => this.TryGetOwnerAs(out ComboBox? _) || base.IsIAccessibleExSupported();

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId == UIA_PATTERN_ID.UIA_ExpandCollapsePatternId && this.TryGetOwnerAs(out ComboBox? owner)
                ? owner.DropDownStyle != ComboBoxStyle.Simple
                : patternId == UIA_PATTERN_ID.UIA_ValuePatternId || base.IsPatternSupported(patternId);

        internal override void Expand() => ComboBoxDefaultAction(true);

        internal override void Collapse() => ComboBoxDefaultAction(false);

        internal override ExpandCollapseState ExpandCollapseState
            => this.IsOwnerHandleCreated(out ComboBox? owner) && owner.DroppedDown
                ? ExpandCollapseState.ExpandCollapseState_Expanded
                : ExpandCollapseState.ExpandCollapseState_Collapsed;

        internal override bool IsValidSelfChildIDAdditionalCheck(VARIANT childId) =>
            childId.vt is VARENUM.VT_I4 or VARENUM.VT_INT && childId.data.intVal == COMBOBOX_ACC_ITEM_INDEX;

        /// <summary>
        ///  Gets the collection of item accessible objects.
        /// </summary>
        public ComboBoxItemAccessibleObjectCollection ItemAccessibleObjects { get; }

        /// <summary>
        ///  Gets the DropDown button accessible object. (UI Automation provider)
        /// </summary>
        public ComboBoxChildDropDownButtonUiaProvider? DropDownButtonUiaProvider
            => _dropDownButtonUiaProvider ??= this.TryGetOwnerAs(out ComboBox? owner) ? new ComboBoxChildDropDownButtonUiaProvider(owner) : null;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!this.IsOwnerHandleCreated(out ComboBox? _))
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild => GetFirstChild(),
                NavigateDirection.NavigateDirection_LastChild => GetLastChild(),
                _ => base.FragmentNavigate(direction),
            };
        }

        internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot => this;

        public override string DefaultAction
        {
            get
            {
                if (!this.TryGetOwnerAs(out ComboBox? owner))
                {
                    return string.Empty;
                }

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
        }

        internal override bool CanGetDefaultActionInternal => false;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId =>
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    this.GetOwnerAccessibleRole() == AccessibleRole.Default
                        ? (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ComboBoxControlTypeId
                        : base.GetPropertyValue(propertyID),
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(this.TryGetOwnerAs(out ComboBox? owner) && owner.Focused),
                _ => base.GetPropertyValue(propertyID)
            };

        internal void RemoveComboBoxItemAccessibleObjectAt(int index)
        {
            if (!this.TryGetOwnerAs(out ComboBox? owner))
            {
                return;
            }

            List<Entry> entries = owner.Items.InnerList;
            Debug.Assert(index < entries.Count);

            Entry item = entries[index];
            if (!ItemAccessibleObjects.TryGetValue(item, out ComboBoxItemAccessibleObject? value))
            {
                return;
            }

            PInvoke.UiaDisconnectProvider(value);

            ItemAccessibleObjects.Remove(item);
        }

        internal void ReleaseDropDownButtonUiaProvider()
        {
            PInvoke.UiaDisconnectProvider(_dropDownButtonUiaProvider);
            _dropDownButtonUiaProvider = null;
        }

        internal void ResetListItemAccessibleObjects()
        {
            if (OsVersion.IsWindows8OrGreater())
            {
                foreach (ComboBoxItemAccessibleObject itemAccessibleObject in ItemAccessibleObjects.Values)
                {
                    PInvoke.UiaDisconnectProvider(itemAccessibleObject, skipOSCheck: true);
                }
            }

            ItemAccessibleObjects.Clear();
        }

        internal void SetComboBoxItemFocus()
        {
            if (!this.IsOwnerHandleCreated(out ComboBox? _))
            {
                return;
            }

            GetSelectedComboBoxItemAccessibleObject()?.SetFocus();
        }

        internal void SetComboBoxItemSelection()
        {
            if (!this.IsOwnerHandleCreated(out ComboBox? _))
            {
                return;
            }

            GetSelectedComboBoxItemAccessibleObject()?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_SelectionItem_ElementSelectedEventId);
        }

        internal override void SetFocus()
        {
            if (!this.IsOwnerHandleCreated(out ComboBox? _))
            {
                return;
            }

            base.SetFocus();

            RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }

        private ComboBoxItemAccessibleObject? GetSelectedComboBoxItemAccessibleObject()
        {
            if (!this.TryGetOwnerAs(out ComboBox? owner))
            {
                return null;
            }

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
            if (!this.IsOwnerHandleCreated(out ComboBox? owner) || owner.DropDownStyle == ComboBoxStyle.Simple)
            {
                return;
            }

            owner.DroppedDown = !owner.DroppedDown;
        }
    }
}
