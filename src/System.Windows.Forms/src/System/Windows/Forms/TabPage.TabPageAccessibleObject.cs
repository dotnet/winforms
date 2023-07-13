// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms;

public partial class TabPage
{
    internal sealed class TabPageAccessibleObject : ControlAccessibleObject
    {
        public TabPageAccessibleObject(TabPage owningTabPage) : base(owningTabPage) { }

        public override Rectangle Bounds
        {
            get
            {
                if (!this.IsHandleCreated(out TabPage? owningTabPage))
                {
                    return Rectangle.Empty;
                }

                // The CHILDID_SELF constant returns to the id of the TabPage, which allows to use the native
                // "accLocation" method to get the "Bounds" property
                return SystemIAccessible.TryGetLocation(CHILDID_SELF);
            }
        }

        public override AccessibleStates State => SystemIAccessible.TryGetState(GetChildId());

        internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot => OwningTabControl?.AccessibilityObject;

        private TabControl? OwningTabControl =>
            this.TryGetOwnerAs(out TabPage? owningTabPage) ? owningTabPage.ParentInternal as TabControl : null;

        public override AccessibleObject? GetChild(int index)
        {
            if (!this.IsHandleCreated(out TabPage? owningTabPage))
            {
                return null;
            }

            if (index < 0 || index > owningTabPage.Controls.Count - 1)
            {
                return null;
            }

            return owningTabPage.Controls[index].AccessibilityObject;
        }

        public override int GetChildCount()
            => this.IsHandleCreated(out TabPage? owningTabPage) ? owningTabPage.Controls.Count : -1;

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (!this.IsHandleCreated(out TabPage? owningTabPage) || OwningTabControl is null)
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
                UiaCore.UIA.HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out TabPage? owningTabPage) && owningTabPage.Focused,
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
            if (!this.TryGetOwnerAs(out TabPage? owningTabPage) || OwningTabControl is null || owningTabPage != OwningTabControl.SelectedTab)
            {
                return null;
            }

            return OwningTabControl.TabPages.Count > 0 ? OwningTabControl.TabPages[0].TabAccessibilityObject : null;
        }
    }
}
