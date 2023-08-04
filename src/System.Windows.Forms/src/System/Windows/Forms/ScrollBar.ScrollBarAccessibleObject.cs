﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static Interop;

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

        internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot => this;

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

        internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
        {
            AccessibleObject? element = HitTest((int)x, (int)y);

            return element ?? base.ElementProviderFromPoint(x, y);
        }

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            => direction switch
            {
                UiaCore.NavigateDirection.FirstChild => FirstLineButtonAccessibleObject,
                UiaCore.NavigateDirection.LastChild => LastLineButtonAccessibleObject,
                _ => base.FragmentNavigate(direction)
            };

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                // If we don't set a default role for the accessible object
                // it will be retrieved from Windows.
                // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                UiaCore.UIA.ControlTypePropertyId when
                    this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    => UiaCore.UIA.ScrollBarControlTypeId,
                UiaCore.UIA.HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out ScrollBar? owner) ? owner.Focused : false,
                UiaCore.UIA.RangeValueValuePropertyId => RangeValue,
                UiaCore.UIA.RangeValueIsReadOnlyPropertyId => IsReadOnly,
                UiaCore.UIA.RangeValueLargeChangePropertyId => LargeChange,
                UiaCore.UIA.RangeValueSmallChangePropertyId => SmallChange,
                UiaCore.UIA.RangeValueMaximumPropertyId => Maximum,
                UiaCore.UIA.RangeValueMinimumPropertyId => Minimum,
                UiaCore.UIA.IsRangeValuePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.RangeValuePatternId),
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

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
            => patternId switch
            {
                UiaCore.UIA.ValuePatternId => true,
                UiaCore.UIA.RangeValuePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };
    }
}
