// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ScrollBar
{
    internal class ScrollBarAccessibleObject : ControlAccessibleObject
    {
        private ScrollBarFirstLineButtonAccessibleObject? _firstLineButtonAccessibleObject;
        private ScrollBarFirstPageButtonAccessibleObject? _firstPageButtonAccessibleObject;
        private ScrollBarLastLineButtonAccessibleObject? _lastLineButtonAccessibleObject;
        private ScrollBarLastPageButtonAccessibleObject? _lastPageButtonAccessibleObject;
        private ScrollBarThumbAccessibleObject? _thumbAccessibleObject;

        internal ScrollBarAccessibleObject(ScrollBar owningScrollBar) : base(owningScrollBar)
        {
        }

        internal ScrollBarFirstLineButtonAccessibleObject? FirstLineButtonAccessibleObject
            => _firstLineButtonAccessibleObject ??= this.TryGetOwnerAs(out ScrollBar? owner) ? new(owner) : null;

        internal ScrollBarFirstPageButtonAccessibleObject? FirstPageButtonAccessibleObject
            => _firstPageButtonAccessibleObject ??= this.TryGetOwnerAs(out ScrollBar? owner) ? new(owner) : null;

        internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot => this;

        internal ScrollBarLastLineButtonAccessibleObject? LastLineButtonAccessibleObject
            => _lastLineButtonAccessibleObject ??= this.TryGetOwnerAs(out ScrollBar? owner) ? new(owner) : null;

        internal ScrollBarLastPageButtonAccessibleObject? LastPageButtonAccessibleObject
            => _lastPageButtonAccessibleObject ??= this.TryGetOwnerAs(out ScrollBar? owner) ? new(owner) : null;

        internal ScrollBarThumbAccessibleObject? ThumbAccessibleObject
            => _thumbAccessibleObject ??= this.TryGetOwnerAs(out ScrollBar? owner) ? new(owner) : null;

        // The maximum value can only be reached programmatically. The value of a scroll bar cannot reach its maximum
        // value through user interaction at run time. The maximum value that can be reached through user interaction
        // is equal to 1 plus the Maximum property value minus the LargeChange property value.
        internal int UIMaximum => this.TryGetOwnerAs(out ScrollBar? owner) ? owner.Maximum - owner.LargeChange + 1 : 0;

        private bool ArePageButtonsDisplayed
            => (FirstPageButtonAccessibleObject?.IsDisplayed ?? false)
               && (LastPageButtonAccessibleObject?.IsDisplayed ?? false);

        private bool ArePageButtonsHidden
            => !(FirstPageButtonAccessibleObject?.IsDisplayed ?? true)
               && !(LastPageButtonAccessibleObject?.IsDisplayed ?? true);

        public override AccessibleObject? GetChild(int index)
        {
            if (!this.IsOwnerHandleCreated(out ScrollBar? _))
            {
                return null;
            }

            return index switch
            {
                0 => FirstLineButtonAccessibleObject,
                1 => (FirstPageButtonAccessibleObject?.IsDisplayed ?? false) ? FirstPageButtonAccessibleObject : ThumbAccessibleObject,
                2 => (FirstPageButtonAccessibleObject?.IsDisplayed ?? false)
                    ? ThumbAccessibleObject
                    : ArePageButtonsHidden ? LastLineButtonAccessibleObject : LastPageButtonAccessibleObject,
                3 => ArePageButtonsDisplayed
                    ? LastPageButtonAccessibleObject
                    : ArePageButtonsHidden ? null : LastLineButtonAccessibleObject,
                4 => ArePageButtonsDisplayed ? LastLineButtonAccessibleObject : null,
                _ => null
            };
        }

        private protected override bool IsInternal => true;

        public override int GetChildCount()
            => this.IsOwnerHandleCreated(out ScrollBar? _)
                ? ArePageButtonsDisplayed
                    ? 5
                    : ArePageButtonsHidden ? 3 : 4
                : -1;

        public override AccessibleObject? HitTest(int x, int y)
        {
            if (!this.IsOwnerHandleCreated(out ScrollBar? _))
            {
                return null;
            }

            Point point = new(x, y);

            if (ThumbAccessibleObject?.Bounds.Contains(point) == true)
            {
                return ThumbAccessibleObject;
            }

            if (FirstLineButtonAccessibleObject?.Bounds.Contains(point) == true)
            {
                return FirstLineButtonAccessibleObject;
            }

            if (FirstPageButtonAccessibleObject?.Bounds.Contains(point) == true)
            {
                return FirstPageButtonAccessibleObject;
            }

            if (LastPageButtonAccessibleObject?.Bounds.Contains(point) == true)
            {
                return LastPageButtonAccessibleObject;
            }

            if (LastLineButtonAccessibleObject?.Bounds.Contains(point) == true)
            {
                return LastLineButtonAccessibleObject;
            }

            return null;
        }

        internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
        {
            AccessibleObject? element = HitTest((int)x, (int)y);

            return element ?? base.ElementProviderFromPoint(x, y);
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild => FirstLineButtonAccessibleObject,
                NavigateDirection.NavigateDirection_LastChild => LastLineButtonAccessibleObject,
                _ => base.FragmentNavigate(direction)
            };

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                // If we don't set a default role for the accessible object
                // it will be retrieved from Windows.
                // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId when
                    this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ScrollBarControlTypeId,
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(this.TryGetOwnerAs(out ScrollBar? owner) && owner.Focused),
                UIA_PROPERTY_ID.UIA_RangeValueValuePropertyId => (VARIANT)RangeValue,
                UIA_PROPERTY_ID.UIA_RangeValueIsReadOnlyPropertyId => (VARIANT)IsReadOnly,
                UIA_PROPERTY_ID.UIA_RangeValueLargeChangePropertyId => (VARIANT)LargeChange,
                UIA_PROPERTY_ID.UIA_RangeValueSmallChangePropertyId => (VARIANT)SmallChange,
                UIA_PROPERTY_ID.UIA_RangeValueMaximumPropertyId => (VARIANT)Maximum,
                UIA_PROPERTY_ID.UIA_RangeValueMinimumPropertyId => (VARIANT)Minimum,
                UIA_PROPERTY_ID.UIA_IsRangeValuePatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_RangeValuePatternId),
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsIAccessibleExSupported() => true;

        internal override double RangeValue => this.TryGetOwnerAs(out ScrollBar? owner) ? owner.Value : base.RangeValue;

        internal override double LargeChange => this.TryGetOwnerAs(out ScrollBar? owner) ? owner.LargeChange : base.LargeChange;

        internal override double SmallChange => this.TryGetOwnerAs(out ScrollBar? owner) ? owner.SmallChange : base.SmallChange;

        internal override double Maximum => this.TryGetOwnerAs(out ScrollBar? owner) ? owner.Maximum : base.Maximum;

        internal override double Minimum => this.TryGetOwnerAs(out ScrollBar? owner) ? owner.Minimum : base.Minimum;

        internal override bool IsReadOnly => false;

        internal override void SetValue(double newValue)
        {
            if (!this.IsOwnerHandleCreated(out ScrollBar? owner))
            {
                return;
            }

            owner.Value = (int)newValue;
        }

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_ValuePatternId => true,
                UIA_PATTERN_ID.UIA_RangeValuePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };
    }
}
