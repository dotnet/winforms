// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class PrintPreviewControl
{
    internal class PrintPreviewControlAccessibleObject : ControlAccessibleObject
    {
        public PrintPreviewControlAccessibleObject(PrintPreviewControl owner) : base(owner)
        {
        }

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => !this.TryGetOwnerAs(out PrintPreviewControl? owner) ? null : propertyID switch
            {
                UiaCore.UIA.AutomationIdPropertyId => owner.Name,
                UiaCore.UIA.HasKeyboardFocusPropertyId => owner.Focused,
                UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                _ => base.GetPropertyValue(propertyID)
            };

        public override int GetChildCount()
        {
            if (!this.TryGetOwnerAs(out PrintPreviewControl? owner))
            {
                return 0;
            }

            return owner._vScrollBar.Visible
                ? owner._hScrollBar.Visible ? 2 : 1
                : owner._hScrollBar.Visible ? 1 : 0;
        }

        public override AccessibleObject? GetChild(int index)
        {
            if (!this.TryGetOwnerAs(out PrintPreviewControl? owner))
            {
                return null;
            }

            return index switch
            {
                0 => owner._vScrollBar.Visible ? owner._vScrollBar.AccessibilityObject
                    : owner._hScrollBar.Visible ? owner._hScrollBar.AccessibilityObject : null,

                1 => owner._vScrollBar.Visible && owner._hScrollBar.Visible ? owner._hScrollBar.AccessibilityObject : null,

                _ => null
            };
        }

        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            => this;

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (!this.TryGetOwnerAs(out PrintPreviewControl? owner))
            {
                return base.FragmentNavigate(direction);
            }

            switch (direction)
            {
                case UiaCore.NavigateDirection.FirstChild:
                    return owner._vScrollBar.Visible ? owner._vScrollBar.AccessibilityObject
                        : owner._hScrollBar.Visible ? owner._hScrollBar.AccessibilityObject
                        : null;

                case UiaCore.NavigateDirection.LastChild:
                    return owner._hScrollBar.Visible ? owner._hScrollBar.AccessibilityObject
                        : owner._vScrollBar.Visible ? owner._vScrollBar.AccessibilityObject
                        : null;

                default:
                    return base.FragmentNavigate(direction);
            }
        }
    }
}
