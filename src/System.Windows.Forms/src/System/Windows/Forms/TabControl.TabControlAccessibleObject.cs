// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop.UiaCore;

namespace System.Windows.Forms
{
    public partial class TabControl
    {
        internal class TabControlAccessibleObject : ControlAccessibleObject
        {
            private readonly TabControl _owningTabControl;

            public TabControlAccessibleObject(TabControl owningTabControl) : base(owningTabControl)
            {
                _owningTabControl = owningTabControl;
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (!_owningTabControl.IsHandleCreated || GetSystemIAccessibleInternal() is null)
                    {
                        return Rectangle.Empty;
                    }

                    // The "NativeMethods.CHILDID_SELF" constant returns to the id of the TabPage,
                    // which allows to use the native "accLocation" method to get the "Bounds" property
                    GetSystemIAccessibleInternal()!.accLocation(out int left, out int top, out int width, out int height, NativeMethods.CHILDID_SELF);
                    return new(left, top, width, height);
                }
            }

            public override AccessibleRole Role
                => Owner.AccessibleRole != AccessibleRole.Default
                    ? Owner.AccessibleRole
                    : AccessibleRole.PageTabList;

            public override AccessibleStates State
                // The "NativeMethods.CHILDID_SELF" constant returns to the id of the trackbar,
                // which allows to use the native "get_accState" method to get the "State" property
                => GetSystemIAccessibleInternal()?.get_accState(NativeMethods.CHILDID_SELF) is object accState
                    ? (AccessibleStates)accState
                    : AccessibleStates.None;

            internal override IRawElementProviderFragmentRoot FragmentRoot => this;

            internal override bool IsSelectionRequired => true;

            public override AccessibleObject? GetChild(int index)
            {
                if (!_owningTabControl.IsHandleCreated
                    || _owningTabControl.TabPages.Count == 0
                    || index < 0
                    || index > _owningTabControl.TabPages.Count)
                {
                    return null;
                }

                return index == 0
                    ? _owningTabControl.SelectedTab?.AccessibilityObject
                    : _owningTabControl.TabPages[index - 1].TabAccessibilityObject;
            }

            public override int GetChildCount()
            {
                if (!_owningTabControl.IsHandleCreated)
                {
                    // We return -1 instead of 0 when the Handle has not been created,
                    // so that the user can distinguish between the situation
                    // when something went wrong (in this case, the Handle was not created)
                    // and the situation when the Handle was created, but the TabControl,
                    // for example, does not contain TabPages.
                    return -1;
                }

                if (_owningTabControl.TabPages.Count == 0)
                {
                    return 0;
                }

                // We add 1 to the number of TabPages, since the TabControl, in addition to the elements
                // for the TabPages,contains an element for the Panel of the selected TabPage.
                return _owningTabControl.TabPages.Count + 1;
            }

            public override AccessibleObject? HitTest(int x, int y)
            {
                if (!_owningTabControl.IsHandleCreated)
                {
                    return null;
                }

                Point point = new(x, y);
                if (_owningTabControl.SelectedTab is not null
                    && _owningTabControl.SelectedTab.AccessibilityObject.Bounds.Contains(point))
                {
                    return _owningTabControl.SelectedTab.AccessibilityObject;
                }

                foreach (TabPage tabPage in _owningTabControl.TabPages)
                {
                    if (tabPage.TabAccessibilityObject.Bounds.Contains(point))
                    {
                        return tabPage.TabAccessibilityObject;
                    }
                }

                return this;
            }

            internal override IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
                => HitTest((int)x, (int)y) ?? base.ElementProviderFromPoint(x, y);

            internal override IRawElementProviderFragment? FragmentNavigate(NavigateDirection direction)
            {
                if (!_owningTabControl.IsHandleCreated)
                {
                    return null;
                }

                return direction switch
                {
                    NavigateDirection.FirstChild => _owningTabControl.SelectedTab?.AccessibilityObject,
                    NavigateDirection.LastChild => _owningTabControl.TabPages.Count > 0
                                                            ? _owningTabControl.TabPages[^1].TabAccessibilityObject
                                                            : null,
                    _ => base.FragmentNavigate(direction)
                };
            }

            internal override object? GetPropertyValue(UIA propertyID)
                => propertyID switch
                {
                    UIA.HasKeyboardFocusPropertyId => _owningTabControl.Focused,
                    UIA.IsKeyboardFocusablePropertyId
                        // This is necessary for compatibility with MSAA proxy:
                        // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                        => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override IRawElementProviderSimple[]? GetSelection()
                => !_owningTabControl.IsHandleCreated
                    || _owningTabControl.SelectedTab is null
                        ? Array.Empty<IRawElementProviderSimple>()
                        : new IRawElementProviderSimple[] { _owningTabControl.SelectedTab.TabAccessibilityObject };

            internal override bool IsPatternSupported(UIA patternId)
                => patternId switch
                {
                    // The "Enabled" property of the TabControl does not affect the behavior of that property,
                    // so it is always true
                    UIA.SelectionPatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };
        }
    }
}
