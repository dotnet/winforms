// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class ProgressBar
{
    internal class ProgressBarAccessibleObject : ControlAccessibleObject
    {
        internal ProgressBarAccessibleObject(ProgressBar owner) : base(owner)
        {
        }

        internal override bool IsIAccessibleExSupported() => true;

        internal override bool IsPatternSupported(UiaCore.UIA patternId) => patternId switch
        {
            UiaCore.UIA.ValuePatternId or UiaCore.UIA.RangeValuePatternId => true,
            _ => base.IsPatternSupported(patternId),
        };

        internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
            propertyID switch
            {
                UiaCore.UIA.ControlTypePropertyId when this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    => UiaCore.UIA.ProgressBarControlTypeId,
                UiaCore.UIA.IsKeyboardFocusablePropertyId =>
                    // This is necessary for compatibility with MSAA proxy:
                    // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                    true,
                UiaCore.UIA.IsRangeValuePatternAvailablePropertyId => true,
                UiaCore.UIA.IsValuePatternAvailablePropertyId => true,
                UiaCore.UIA.RangeValueIsReadOnlyPropertyId => true,
                UiaCore.UIA.RangeValueLargeChangePropertyId => double.NaN,
                UiaCore.UIA.RangeValueSmallChangePropertyId => double.NaN,
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
