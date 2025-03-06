// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class MonthCalendar
{
    /// <summary>
    ///  Represents an accessible object for a calendar child in <see cref="MonthCalendar"/> control.
    /// </summary>
    internal abstract class MonthCalendarChildAccessibleObject : AccessibleObject
    {
        private readonly MonthCalendarAccessibleObject _monthCalendarAccessibleObject;

        public MonthCalendarChildAccessibleObject(MonthCalendarAccessibleObject calendarAccessibleObject)
        {
            _monthCalendarAccessibleObject = calendarAccessibleObject.OrThrowIfNull();
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)HasKeyboardFocus,
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId => (VARIANT)IsEnabled,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => VARIANT.False,
                _ => base.GetPropertyValue(propertyID)
            };

        private protected virtual bool HasKeyboardFocus => false;

        private protected virtual bool IsEnabled => _monthCalendarAccessibleObject.IsEnabled;

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => _monthCalendarAccessibleObject;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_Parent => Parent,
                _ => base.FragmentNavigate(direction)
            };

        /// <remarks>
        ///  <para>
        ///    This value wasn't saved to a field as in the rest calendar accessible objects
        ///    because GetChildId requires <see cref="_monthCalendarAccessibleObject" /> existing
        ///    but it will be <see langword="null"/> because an inherited constructor is not called yet.
        ///  </para>
        /// </remarks>
        /// <inheritdoc cref="AccessibleObject.RuntimeId" />
        internal override int[] RuntimeId =>
            !_monthCalendarAccessibleObject.TryGetOwnerAs(out Control? owner)
                ? base.RuntimeId
                : [RuntimeIDFirstItem, (int)owner.InternalHandle, GetChildId()];

        public override AccessibleStates State => AccessibleStates.None;
    }
}
