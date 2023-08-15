// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;
using IScrollProvider = Windows.Win32.UI.Accessibility.IScrollProvider;
using ScrollAmount = Windows.Win32.UI.Accessibility.ScrollAmount;

namespace System.Windows.Forms;

public partial class PrintPreviewControl
{
    internal class PrintPreviewControlAccessibleObject : ControlAccessibleObject, IScrollProvider.Interface
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

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
            => patternId switch
            {
                UiaCore.UIA.ScrollPatternId => this.TryGetOwnerAs(out PrintPreviewControl? owner)
                    && (owner._vScrollBar.Visible || owner._hScrollBar.Visible) ? true : false,
                _ => base.IsPatternSupported(patternId)
            };

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

        HRESULT IScrollProvider.Interface.Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
        {
            if (!this.TryGetOwnerAs(out PrintPreviewControl? owner))
            {
                return HRESULT.E_FAIL;
            }

            int scrollValue = 0;
            if (owner._hScrollBar.Visible && horizontalAmount != ScrollAmount.ScrollAmount_NoAmount)
            {
                switch (horizontalAmount)
                {
                    case ScrollAmount.ScrollAmount_LargeIncrement:
                        scrollValue = owner._hScrollBar.Value + owner._hScrollBar.LargeChange;
                        owner._hScrollBar.Value = (scrollValue > owner._hScrollBar.Maximum ? owner._hScrollBar.Maximum : scrollValue);
                        break;
                    case ScrollAmount.ScrollAmount_SmallIncrement:
                        scrollValue = owner._hScrollBar.Value + owner._hScrollBar.SmallChange;
                        owner._hScrollBar.Value = (scrollValue > owner._hScrollBar.Maximum ? owner._hScrollBar.Maximum : scrollValue);
                        break;
                    case ScrollAmount.ScrollAmount_LargeDecrement:
                        scrollValue = owner._hScrollBar.Value - owner._hScrollBar.LargeChange;
                        owner._hScrollBar.Value = (scrollValue < owner._hScrollBar.Minimum ? owner._hScrollBar.Minimum : scrollValue);
                        break;
                    case ScrollAmount.ScrollAmount_SmallDecrement:
                        scrollValue = owner._hScrollBar.Value - owner._hScrollBar.SmallChange;
                        owner._hScrollBar.Value = (scrollValue < owner._hScrollBar.Minimum ? owner._hScrollBar.Minimum : scrollValue);
                        break;
                    case ScrollAmount.ScrollAmount_NoAmount:
                        return HRESULT.E_FAIL;
                }

                return HRESULT.S_OK;
            }

            if (owner._vScrollBar.Visible && horizontalAmount != ScrollAmount.ScrollAmount_NoAmount)
            {
                switch (horizontalAmount)
                {
                    case ScrollAmount.ScrollAmount_LargeIncrement:
                        scrollValue = owner._vScrollBar.Value + owner._vScrollBar.LargeChange;
                        owner._vScrollBar.Value = (scrollValue > owner._vScrollBar.Maximum ? owner._vScrollBar.Maximum : scrollValue);
                        break;
                    case ScrollAmount.ScrollAmount_SmallIncrement:
                        scrollValue = owner._vScrollBar.Value + owner._vScrollBar.SmallChange;
                        owner._vScrollBar.Value = (scrollValue > owner._vScrollBar.Maximum ? owner._vScrollBar.Maximum : scrollValue);
                        break;
                    case ScrollAmount.ScrollAmount_LargeDecrement:
                        scrollValue = owner._vScrollBar.Value - owner._vScrollBar.LargeChange;
                        owner._vScrollBar.Value = (scrollValue < owner._vScrollBar.Minimum ? owner._vScrollBar.Minimum : scrollValue);
                        break;
                    case ScrollAmount.ScrollAmount_SmallDecrement:
                        scrollValue = owner._vScrollBar.Value - owner._vScrollBar.SmallChange;
                        owner._vScrollBar.Value = (scrollValue < owner._vScrollBar.Minimum ? owner._vScrollBar.Minimum : scrollValue);
                        break;
                    case ScrollAmount.ScrollAmount_NoAmount:
                        return HRESULT.E_FAIL;
                }

                return HRESULT.S_OK;
            }

            return HRESULT.E_FAIL;
        }

        HRESULT IScrollProvider.Interface.SetScrollPercent(double horizontalPercent, double verticalPercent)
        {
            if (!this.TryGetOwnerAs(out PrintPreviewControl? owner))
            {
                return HRESULT.E_FAIL;
            }

            int scrollValue = 0;
            if (owner._hScrollBar.Visible && horizontalPercent >= 0 && horizontalPercent <= 100)
            {
                scrollValue = owner._hScrollBar.Minimum + (int)((owner._hScrollBar.Maximum - owner._hScrollBar.Minimum) * horizontalPercent);
                owner._hScrollBar.Value = scrollValue;
                return HRESULT.S_OK;
            }

            if (owner._vScrollBar.Visible && verticalPercent >= 0 && verticalPercent <= 100)
            {
                scrollValue = owner._vScrollBar.Minimum + (int)((owner._vScrollBar.Maximum - owner._vScrollBar.Minimum) * verticalPercent);
                owner._vScrollBar.Value = scrollValue;
                return HRESULT.S_OK;
            }

            return HRESULT.E_FAIL;
        }

        public double HorizontalScrollPercent
        {
            get
            {
                if (this.TryGetOwnerAs(out PrintPreviewControl? owner) && owner._hScrollBar.Visible)
                {
                    double percent = owner._hScrollBar.Value * 100.0 / (owner._hScrollBar.Maximum - owner._hScrollBar.LargeChange);
                    return percent > 100 ? 100 : percent;
                }

                return 0;
            }
        }

        public double VerticalScrollPercent
        {
            get
            {
                if (this.TryGetOwnerAs(out PrintPreviewControl? owner) && owner._vScrollBar.Visible)
                {
                    double percent = owner._vScrollBar.Value * 100.0 / (owner._vScrollBar.Maximum - owner._vScrollBar.LargeChange);
                    return percent > 100 ? 100 : percent;
                }

                return 0;
            }
        }

        public double HorizontalViewSize => this.TryGetOwnerAs(out PrintPreviewControl? owner)
            && owner._hScrollBar.Visible ? owner.HorizontalViewSize : 100;

        public double VerticalViewSize => this.TryGetOwnerAs(out PrintPreviewControl? owner)
            && owner._vScrollBar.Visible ? owner.VerticalViewSize : 100;

        BOOL IScrollProvider.Interface.HorizontallyScrollable =>
            this.TryGetOwnerAs(out PrintPreviewControl? owner) ? owner._hScrollBar.Visible : false;

        BOOL IScrollProvider.Interface.VerticallyScrollable =>
            this.TryGetOwnerAs(out PrintPreviewControl? owner) ? owner._vScrollBar.Visible : false;
    }
}
