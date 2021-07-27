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
            private readonly TabControl _owningTabControl;

            private readonly TabPage _owningTabPage;

            public TabAccessibleObject(TabPage owningTabPage)
            {
                _owningTabPage = owningTabPage ?? throw new ArgumentNullException(nameof(owningTabPage));
                _owningTabControl = owningTabPage.ParentInternal as TabControl
                                    ?? throw new ArgumentNullException(nameof(owningTabPage.ParentInternal));
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (!_owningTabControl.IsHandleCreated)
                    {
                        return Rectangle.Empty;
                    }

                    int index = CurrentIndex;

                    if (index == -1 || (State & AccessibleStates.Invisible) == AccessibleStates.Invisible)
                    {
                        return Rectangle.Empty;
                    }

                    int left = 0;
                    int top = 0;
                    int width = 0;
                    int height = 0;

                    // The "NativeMethods.CHILDID_SELF" constant returns to the id of the trackbar,
                    // which allows to use the native "accLocation" method to get the "Bounds" property
                    SystemIAccessibleInternal?.accLocation(out left, out top, out width, out height, GetChildId());

                    return new(left, top, width, height);
                }
            }

            public override string? DefaultAction => SystemIAccessibleInternal?.get_accDefaultAction(GetChildId());

            public override string? Name => _owningTabPage.Text;

            public override AccessibleRole Role
                => SystemIAccessibleInternal?.get_accRole(GetChildId()) is object accRole
                    ? (AccessibleRole)accRole
                    : AccessibleRole.None;

            public override AccessibleStates State
                => SystemIAccessibleInternal?.get_accState(GetChildId()) is object accState
                    ? (AccessibleStates)accState
                    : AccessibleStates.None;

            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot => _owningTabControl.AccessibilityObject;

            internal override bool IsItemSelected => _owningTabControl.SelectedTab == _owningTabPage;

            internal override UiaCore.IRawElementProviderSimple? ItemSelectionContainer => _owningTabControl.AccessibilityObject;

            internal override int[]? RuntimeId
                => new int[] { RuntimeIDFirstItem, PARAM.ToInt(_owningTabControl.InternalHandle), GetChildId() };

            private int CurrentIndex => _owningTabControl.TabPages.IndexOf(_owningTabPage);

            private IAccessible? SystemIAccessibleInternal
                => _owningTabControl.AccessibilityObject.GetSystemIAccessibleInternal();

            public override void DoDefaultAction()
            {
                if (_owningTabControl.IsHandleCreated && _owningTabControl.Enabled)
                {
                    _owningTabControl.SelectedTab = _owningTabPage;
                }
            }

            internal override void AddToSelection() => DoDefaultAction();

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (!_owningTabControl.IsHandleCreated)
                {
                    return null;
                }

                return direction switch
                {
                    UiaCore.NavigateDirection.Parent => _owningTabControl.AccessibilityObject,
                    UiaCore.NavigateDirection.NextSibling => _owningTabControl.AccessibilityObject.GetChild(GetChildId() + 1),
                    UiaCore.NavigateDirection.PreviousSibling => _owningTabControl.AccessibilityObject.GetChild(GetChildId() - 1),
                    _ => null
                };
            }

            // +1 is needed because 0 is the Pane id of the selected tab
            internal override int GetChildId() => CurrentIndex + 1;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.TabItemControlTypeId,
                    UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                    UiaCore.UIA.AutomationIdPropertyId => _owningTabPage.Name,
                    UiaCore.UIA.AccessKeyPropertyId => string.Empty,
                    UiaCore.UIA.IsPasswordPropertyId => false,
                    UiaCore.UIA.HelpTextPropertyId => string.Empty,
                    UiaCore.UIA.IsEnabledPropertyId => _owningTabControl.Enabled,
                    UiaCore.UIA.IsOffscreenPropertyId => (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => (State & AccessibleStates.Focused) == AccessibleStates.Focused,
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.SelectionItemPatternId),
                    UiaCore.UIA.IsInvokePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.InvokePatternId),
                    UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId),
                    UiaCore.UIA.IsKeyboardFocusablePropertyId
                        // This is necessary for compatibility with MSAA proxy:
                        // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                        => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.LegacyIAccessiblePatternId => true,
                    UiaCore.UIA.InvokePatternId => false,
                    UiaCore.UIA.SelectionItemPatternId => true,
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
