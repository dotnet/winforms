// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class ProgressBar
    {
        internal class ProgressBarAccessibleObject : ControlAccessibleObject
        {
            internal ProgressBarAccessibleObject(ProgressBar owner) : base(owner)
            {
            }

            private ProgressBar OwningProgressBar => (ProgressBar)Owner;

            internal override bool IsIAccessibleExSupported() => true;

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ValuePatternId ||
                    patternId == UiaCore.UIA.RangeValuePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.ControlTypePropertyId:
                        // If we don't set a default role for the accessible object
                        // it will be retrieved from Windows.
                        // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                        return Owner.AccessibleRole == AccessibleRole.Default
                               ? UiaCore.UIA.ProgressBarControlTypeId
                               : base.GetPropertyValue(propertyID);
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        // This is necessary for compatibility with MSAA proxy:
                        // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                        return true;
                    case UiaCore.UIA.IsRangeValuePatternAvailablePropertyId:
                    case UiaCore.UIA.IsValuePatternAvailablePropertyId:
                    case UiaCore.UIA.RangeValueIsReadOnlyPropertyId:
                        return true;
                    case UiaCore.UIA.RangeValueLargeChangePropertyId:
                    case UiaCore.UIA.RangeValueSmallChangePropertyId:
                        return double.NaN;
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override void SetValue(double newValue)
            {
                throw new InvalidOperationException("Progress Bar is read-only.");
            }

            internal override double LargeChange => double.NaN;

            internal override double Maximum => OwningProgressBar.Maximum;

            internal override double Minimum => OwningProgressBar.Minimum;

            internal override double SmallChange => double.NaN;

            internal override double RangeValue => OwningProgressBar.Value;

            internal override bool IsReadOnly => true;
        }
    }
}
