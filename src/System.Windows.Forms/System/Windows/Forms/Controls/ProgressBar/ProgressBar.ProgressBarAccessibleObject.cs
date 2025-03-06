// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ProgressBar
{
    internal class ProgressBarAccessibleObject : ControlAccessibleObject
    {
        internal ProgressBarAccessibleObject(ProgressBar owner) : base(owner)
        {
        }

        internal override bool IsIAccessibleExSupported() => true;

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) => patternId switch
        {
            UIA_PATTERN_ID.UIA_ValuePatternId or UIA_PATTERN_ID.UIA_RangeValuePatternId => true,
            _ => base.IsPatternSupported(patternId),
        };

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId when this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ProgressBarControlTypeId,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId =>
                    // This is necessary for compatibility with MSAA proxy:
                    // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                    VARIANT.True,
                UIA_PROPERTY_ID.UIA_IsRangeValuePatternAvailablePropertyId => VARIANT.True,
                UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId => VARIANT.True,
                UIA_PROPERTY_ID.UIA_RangeValueIsReadOnlyPropertyId => VARIANT.True,
                UIA_PROPERTY_ID.UIA_RangeValueLargeChangePropertyId => (VARIANT)double.NaN,
                UIA_PROPERTY_ID.UIA_RangeValueSmallChangePropertyId => (VARIANT)double.NaN,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override void SetValue(double newValue)
            => throw new InvalidOperationException("Progress Bar is read-only.");

        internal override double LargeChange => double.NaN;

        internal override double Maximum => this.TryGetOwnerAs(out ProgressBar? owner) ? owner.Maximum : 0;

        internal override double Minimum => this.TryGetOwnerAs(out ProgressBar? owner) ? owner.Minimum : 0;

        internal override double SmallChange => double.NaN;

        internal override double RangeValue => this.TryGetOwnerAs(out ProgressBar? owner) ? owner.Value : 0;

        internal override bool IsReadOnly => true;
    }
}
