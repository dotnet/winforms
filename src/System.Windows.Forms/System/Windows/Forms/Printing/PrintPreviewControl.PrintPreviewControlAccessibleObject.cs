// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using IScrollProvider = Windows.Win32.UI.Accessibility.IScrollProvider;
using ScrollAmount = Windows.Win32.UI.Accessibility.ScrollAmount;

namespace System.Windows.Forms;

public partial class PrintPreviewControl
{
    internal sealed class PrintPreviewControlAccessibleObject : ControlAccessibleObject, IScrollProvider.Interface
    {
        public PrintPreviewControlAccessibleObject(PrintPreviewControl owner) : base(owner)
        {
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => !this.TryGetOwnerAs(out PrintPreviewControl? owner) ? default : propertyID switch
            {
                UIA_PROPERTY_ID.UIA_AutomationIdPropertyId => (VARIANT)owner.Name,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)owner.Focused,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focusable),
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

        private protected override bool IsInternal => true;

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_ScrollPatternId => this.TryGetOwnerAs(out PrintPreviewControl? owner)
                    && (owner._vScrollBar.Visible || owner._hScrollBar.Visible),
                _ => base.IsPatternSupported(patternId)
            };

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => this;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!this.TryGetOwnerAs(out PrintPreviewControl? owner))
            {
                return base.FragmentNavigate(direction);
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild
                    => owner._vScrollBar.Visible
                        ? owner._vScrollBar.AccessibilityObject
                        : owner._hScrollBar.Visible
                            ? owner._hScrollBar.AccessibilityObject
                            : null,
                NavigateDirection.NavigateDirection_LastChild
                    => owner._hScrollBar.Visible
                        ? owner._hScrollBar.AccessibilityObject
                        : owner._vScrollBar.Visible
                            ? owner._vScrollBar.AccessibilityObject
                            : null,
                _ => base.FragmentNavigate(direction),
            };
        }

        HRESULT IScrollProvider.Interface.Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
        {
            if (!this.TryGetOwnerAs(out PrintPreviewControl? owner))
            {
                return HRESULT.E_FAIL;
            }

            int scrollValue;
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
                }
            }

            if (owner._vScrollBar.Visible && verticalAmount != ScrollAmount.ScrollAmount_NoAmount)
            {
                switch (verticalAmount)
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
                }
            }

            return HRESULT.S_OK;
        }

        HRESULT IScrollProvider.Interface.SetScrollPercent(double horizontalPercent, double verticalPercent)
        {
            if (!this.TryGetOwnerAs(out PrintPreviewControl? owner))
            {
                return HRESULT.E_FAIL;
            }

            int scrollValue;
            if (owner._hScrollBar.Visible && horizontalPercent >= 0 && horizontalPercent <= 100)
            {
                scrollValue = owner._hScrollBar.Minimum + (int)((owner._hScrollBar.Maximum - owner._hScrollBar.Minimum) * horizontalPercent);
                owner._hScrollBar.Value = scrollValue;
            }

            if (owner._vScrollBar.Visible && verticalPercent >= 0 && verticalPercent <= 100)
            {
                scrollValue = owner._vScrollBar.Minimum + (int)((owner._vScrollBar.Maximum - owner._vScrollBar.Minimum) * verticalPercent);
                owner._vScrollBar.Value = scrollValue;
            }

            return HRESULT.S_OK;
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
            this.TryGetOwnerAs(out PrintPreviewControl? owner) && owner._hScrollBar.Visible;

        BOOL IScrollProvider.Interface.VerticallyScrollable =>
            this.TryGetOwnerAs(out PrintPreviewControl? owner) && owner._vScrollBar.Visible;
    }
}
