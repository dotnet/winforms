// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Accessibility;
using static Interop;

namespace System.Windows.Forms
{
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
                    SystemIAccessibleInternal.accLocation(out int left, out int top, out int width, out int height, GetChildId());
                    return new(left, top, width, height);
                }
            }

            public override string? DefaultAction => SystemIAccessibleInternal?.get_accDefaultAction(GetChildId());

            public override string? Name => _owningTabPage.Text;

            private TabControl? OwningTabControl => _owningTabPage.ParentInternal as TabControl;

            public override AccessibleRole Role
                => SystemIAccessibleInternal?.get_accRole(GetChildId()) is object accRole
                    ? (AccessibleRole)accRole
                    : AccessibleRole.None;

            public override AccessibleStates State
                => SystemIAccessibleInternal?.get_accState(GetChildId()) is object accState
                    ? (AccessibleStates)accState
                    : AccessibleStates.None;

            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot => OwningTabControl?.AccessibilityObject;

            internal override bool IsItemSelected => OwningTabControl?.SelectedTab == _owningTabPage;

            internal override UiaCore.IRawElementProviderSimple? ItemSelectionContainer => OwningTabControl?.AccessibilityObject;

            internal override int[] RuntimeId
                => new int[]
                {
                    RuntimeIDFirstItem,
                    OwningTabControl is null
                        ? PARAM.ToInt(IntPtr.Zero)
                        : PARAM.ToInt(OwningTabControl.InternalHandle),
                    GetChildId()
                };

            private int CurrentIndex => OwningTabControl?.TabPages.IndexOf(_owningTabPage) ?? -1;

            private IAccessible? SystemIAccessibleInternal
                => OwningTabControl?.AccessibilityObject.GetSystemIAccessibleInternal();

            public override void DoDefaultAction()
            {
                if (OwningTabControl is not null && OwningTabControl.IsHandleCreated && OwningTabControl.Enabled)
                {
                    OwningTabControl.SelectedTab = _owningTabPage;
                }
            }

            internal override void AddToSelection() => DoDefaultAction();

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (OwningTabControl is null || !OwningTabControl.IsHandleCreated)
                {
                    return null;
                }

                return direction switch
                {
                    UiaCore.NavigateDirection.Parent => OwningTabControl.AccessibilityObject,
                    UiaCore.NavigateDirection.NextSibling => OwningTabControl.AccessibilityObject.GetChild(GetChildId() + 1),
                    UiaCore.NavigateDirection.PreviousSibling => OwningTabControl.AccessibilityObject.GetChild(GetChildId() - 1),
                    _ => null
                };
            }

            // +1 is needed because 0 is the Pane id of the selected tab
            internal override int GetChildId() => CurrentIndex + 1;

            public override string? Help => SystemIAccessibleInternal?.get_accHelp(GetChildId());

            public override string? KeyboardShortcut => SystemIAccessibleInternal?.get_accKeyboardShortcut(GetChildId());

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.TabItemControlTypeId,
                    UiaCore.UIA.IsEnabledPropertyId => OwningTabControl?.Enabled ?? false,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => (State & AccessibleStates.Focused) == AccessibleStates.Focused,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId
                        // This is necessary for compatibility with MSAA proxy:
                        // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                        => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    // The "Enabled" property of the TabControl does not affect the behavior of that property,
                    // so it is always true
                    UiaCore.UIA.SelectionItemPatternId => true,
                    UiaCore.UIA.InvokePatternId => false,
                    UiaCore.UIA.LegacyIAccessiblePatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

            internal override void RemoveFromSelection()
            {
                // Do nothing, C++ implementation returns UIA_E_INVALIDOPERATION 0x80131509
            }

            internal unsafe override void SelectItem() => DoDefaultAction();
        }
    }
}
