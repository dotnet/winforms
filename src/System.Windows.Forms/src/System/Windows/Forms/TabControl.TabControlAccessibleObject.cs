// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop.UiaCore;

namespace System.Windows.Forms;

public partial class TabControl
{
    internal sealed class TabControlAccessibleObject : ControlAccessibleObject
    {
        public TabControlAccessibleObject(TabControl owningTabControl) : base(owningTabControl)
        {
        }

        public override Rectangle Bounds
        {
            get
            {
                if (!this.IsHandleCreated(out TabControl? owner))
                {
                    return Rectangle.Empty;
                }

                // The CHILDID_SELF constant returns to the id of the TabPage, which allows to use the native
                // "accLocation" method to get the "Bounds" property
                return SystemIAccessible.TryGetLocation(CHILDID_SELF);
            }
        }

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.PageTabList);

        public override AccessibleStates State
            // The CHILDID_SELF constant returns to the id of the trackbar, which allows to use the native
            // "get_accState" method to get the "State" property
            => SystemIAccessible.TryGetState(CHILDID_SELF);

        internal override IRawElementProviderFragmentRoot FragmentRoot => this;

        internal override bool IsSelectionRequired => true;

        public override AccessibleObject? GetChild(int index)
        {
            if (!this.IsHandleCreated(out TabControl? owner)
                || owner.TabPages.Count == 0
                || index < 0
                || index > owner.TabPages.Count)
            {
                return null;
            }

            return index == 0
                ? owner.SelectedTab?.AccessibilityObject
                : owner.TabPages[index - 1].TabAccessibilityObject;
        }

        public override int GetChildCount()
        {
            if (!this.IsHandleCreated(out TabControl? owner))
            {
                // We return -1 instead of 0 when the Handle has not been created,
                // so that the user can distinguish between the situation
                // when something went wrong (in this case, the Handle was not created)
                // and the situation when the Handle was created, but the TabControl,
                // for example, does not contain TabPages.
                return -1;
            }

            if (owner.TabPages.Count == 0)
            {
                return 0;
            }

            // We add 1 to the number of TabPages, since the TabControl, in addition to the elements
            // for the TabPages,contains an element for the Panel of the selected TabPage.
            return owner.TabPages.Count + 1;
        }

        public override AccessibleObject? HitTest(int x, int y)
        {
            if (!this.IsHandleCreated(out TabControl? owner))
            {
                return null;
            }

            Point point = new(x, y);
            if (owner.SelectedTab is not null
                && owner.SelectedTab.AccessibilityObject.Bounds.Contains(point))
            {
                return owner.SelectedTab.AccessibilityObject;
            }

            foreach (TabPage tabPage in owner.TabPages)
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
            if (!this.IsHandleCreated(out TabControl? owner))
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.FirstChild => owner.SelectedTab?.AccessibilityObject,
                NavigateDirection.LastChild => owner.TabPages.Count > 0
                    ? owner.TabPages[^1].TabAccessibilityObject
                    : null,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override object? GetPropertyValue(UIA propertyID)
            => propertyID switch
            {
                UIA.HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out TabControl? owner) && owner.Focused,
                UIA.IsKeyboardFocusablePropertyId
                    // This is necessary for compatibility with MSAA proxy:
                    // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                    => true,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override IRawElementProviderSimple[]? GetSelection()
            => !this.IsHandleCreated(out TabControl? owner) || owner.SelectedTab is null
                ? Array.Empty<IRawElementProviderSimple>()
                : new IRawElementProviderSimple[] { owner.SelectedTab.TabAccessibilityObject };

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
