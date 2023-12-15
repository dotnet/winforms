// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

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
                if (!this.IsOwnerHandleCreated(out TabControl? _))
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

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => this;

        internal override bool IsSelectionRequired => true;

        public override AccessibleObject? GetChild(int index)
        {
            if (!this.IsOwnerHandleCreated(out TabControl? owner)
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

        private protected override bool IsInternal => true;

        public override int GetChildCount()
        {
            if (!this.IsOwnerHandleCreated(out TabControl? owner))
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
            if (!this.IsOwnerHandleCreated(out TabControl? owner))
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

        internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
            => HitTest((int)x, (int)y) ?? base.ElementProviderFromPoint(x, y);

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!this.IsOwnerHandleCreated(out TabControl? owner))
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild => owner.SelectedTab?.AccessibilityObject,
                NavigateDirection.NavigateDirection_LastChild => owner.TabPages.Count > 0
                    ? owner.TabPages[^1].TabAccessibilityObject
                    : null,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(this.TryGetOwnerAs(out TabControl? owner) && owner.Focused),
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId
                    // This is necessary for compatibility with MSAA proxy:
                    // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                    => VARIANT.True,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override IRawElementProviderSimple.Interface[]? GetSelection()
            => !this.IsOwnerHandleCreated(out TabControl? owner) || owner.SelectedTab is null
                ? []
                : [owner.SelectedTab.TabAccessibilityObject];

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                // The "Enabled" property of the TabControl does not affect the behavior of that property,
                // so it is always true
                UIA_PATTERN_ID.UIA_SelectionPatternId => true,
                _ => base.IsPatternSupported(patternId)
            };
    }
}
