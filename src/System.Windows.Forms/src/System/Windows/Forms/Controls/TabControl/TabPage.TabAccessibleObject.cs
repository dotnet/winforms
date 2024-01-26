// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class TabPage
{
    internal class TabAccessibleObject : AccessibleObject
    {
        private readonly TabPage _owningTabPage;

        public TabAccessibleObject(TabPage owningTabPage)
        {
            _owningTabPage = owningTabPage.OrThrowIfNull();
        }

        private protected override string AutomationId => _owningTabPage.Name;

        public override Rectangle Bounds
        {
            get
            {
                if (OwningTabControl is null || !OwningTabControl.IsHandleCreated || SystemIAccessibleInternal is null)
                {
                    return Rectangle.Empty;
                }

                int index = CurrentIndex;

                if (index == -1 || (State & AccessibleStates.Invisible) == AccessibleStates.Invisible)
                {
                    return Rectangle.Empty;
                }

                // The "GetChildId" method returns to the id of the TabControl element,
                // which allows to use the native "accLocation" method to get the "Bounds" property
                return SystemIAccessibleInternal.TryGetLocation(GetChildId());
            }
        }

        public override string? DefaultAction => GetDefaultActionInternal().ToNullableStringAndFree();

        private protected override bool IsInternal => true;

        internal override BSTR GetDefaultActionInternal() => SystemIAccessibleInternal.TryGetDefaultAction(GetChildId());

        public override string? Name => _owningTabPage.Text;

        internal override bool CanGetNameInternal => false;

        private TabControl? OwningTabControl => _owningTabPage.ParentInternal as TabControl;

        public override AccessibleRole Role => SystemIAccessibleInternal.TryGetRole(GetChildId());

        public override AccessibleStates State => SystemIAccessibleInternal.TryGetState(GetChildId());

        internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot => OwningTabControl?.AccessibilityObject;

        internal override bool IsItemSelected => OwningTabControl?.SelectedTab == _owningTabPage;

        internal override IRawElementProviderSimple.Interface? ItemSelectionContainer => OwningTabControl?.AccessibilityObject;

        internal override int[] RuntimeId =>
        [
            RuntimeIDFirstItem,
            OwningTabControl is null
                ? (int)IntPtr.Zero
                : (int)OwningTabControl.InternalHandle,
            GetHashCode()
        ];

        private int CurrentIndex => OwningTabControl?.TabPages.IndexOf(_owningTabPage) ?? -1;

        private AgileComPointer<IAccessible>? SystemIAccessibleInternal
            => OwningTabControl?.AccessibilityObject.SystemIAccessible;

        public override void DoDefaultAction()
        {
            if (OwningTabControl is not null && OwningTabControl.IsHandleCreated && OwningTabControl.Enabled)
            {
                OwningTabControl.SelectedTab = _owningTabPage;
            }
        }

        internal override void AddToSelection() => DoDefaultAction();

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (OwningTabControl is null || !OwningTabControl.IsHandleCreated)
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_Parent => OwningTabControl.AccessibilityObject,
                NavigateDirection.NavigateDirection_NextSibling => OwningTabControl.AccessibilityObject.GetChild(GetChildId() + 1),
                NavigateDirection.NavigateDirection_PreviousSibling => OwningTabControl.AccessibilityObject.GetChild(GetChildId() - 1),
                _ => null
            };
        }

        // +1 is needed because 0 is the Pane id of the selected tab
        internal override int GetChildId() => CurrentIndex + 1;

        public override string? Help => GetHelpInternal().ToNullableStringAndFree();

        internal override BSTR GetHelpInternal() => SystemIAccessibleInternal.TryGetHelp(GetChildId());

        public override string? KeyboardShortcut => GetKeyboardShortcutInternal((VARIANT)GetChildId()).ToNullableStringAndFree();

        internal override BSTR GetKeyboardShortcutInternal(VARIANT childID) => SystemIAccessibleInternal.TryGetKeyboardShortcut(childID);

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_TabItemControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focused),
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)(OwningTabControl?.Enabled ?? false),
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId
                    // This is necessary for compatibility with MSAA proxy:
                    // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                    => VARIANT.True,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                // The "Enabled" property of the TabControl does not affect the behavior of that property,
                // so it is always true
                UIA_PATTERN_ID.UIA_SelectionItemPatternId => true,
                UIA_PATTERN_ID.UIA_InvokePatternId => false,
                UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };

        internal override void RemoveFromSelection()
        {
            // Do nothing, C++ implementation returns UIA_E_INVALIDOPERATION 0x80131509
        }

        internal override unsafe void SelectItem() => DoDefaultAction();
    }
}
