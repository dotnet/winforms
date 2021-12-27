// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class TabPage
    {
        internal class TabPageAccessibleObject : ControlAccessibleObject
        {
            private readonly TabPage _owningTabPage;

            public TabPageAccessibleObject(TabPage owningTabPage) : base(owningTabPage)
            {
                _owningTabPage = owningTabPage;
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (!_owningTabPage.IsHandleCreated || GetSystemIAccessibleInternal() is null)
                    {
                        return Rectangle.Empty;
                    }

                    // The "NativeMethods.CHILDID_SELF" constant returns to the id of the TabPage,
                    // which allows to use the native "accLocation" method to get the "Bounds" property
                    GetSystemIAccessibleInternal()!.accLocation(out int left, out int top, out int width, out int height, NativeMethods.CHILDID_SELF);
                    return new(left, top, width, height);
                }
            }

            public override AccessibleStates State
                => GetSystemIAccessibleInternal()?.get_accState(GetChildId()) is object accState
                    ? (AccessibleStates)accState
                    : AccessibleStates.None;

            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot => OwningTabControl?.AccessibilityObject;

            private TabControl? OwningTabControl => _owningTabPage.ParentInternal as TabControl;

            public override AccessibleObject? GetChild(int index)
            {
                if (!_owningTabPage.IsHandleCreated)
                {
                    return null;
                }

                if (index < 0 || index > _owningTabPage.Controls.Count - 1)
                {
                    return null;
                }

                return _owningTabPage.Controls[index].AccessibilityObject;
            }

            public override int GetChildCount() => _owningTabPage.IsHandleCreated
                                                    ? _owningTabPage.Controls.Count
                                                    : -1;

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (!_owningTabPage.IsHandleCreated || OwningTabControl is null)
                {
                    return null;
                }

                return direction switch
                {
                    UiaCore.NavigateDirection.Parent => OwningTabControl?.AccessibilityObject,
                    UiaCore.NavigateDirection.NextSibling => GetNextSibling(),
                    UiaCore.NavigateDirection.PreviousSibling => null,
                    _ => base.FragmentNavigate(direction)
                };
            }

            internal override int GetChildId() => 0;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.AutomationIdPropertyId => _owningTabPage.Name,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => _owningTabPage.Focused,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId
                        // This is necessary for compatibility with MSAA proxy:
                        // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                        => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.ValuePatternId => false,
                    _ => base.IsPatternSupported(patternId)
                };

            private UiaCore.IRawElementProviderFragment? GetNextSibling()
            {
                if (OwningTabControl is null || _owningTabPage != OwningTabControl.SelectedTab)
                {
                    return null;
                }

                return OwningTabControl.TabPages.Count > 0 ? OwningTabControl.TabPages[0].TabAccessibilityObject : null;
            }
        }
    }
}
