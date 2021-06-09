// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
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
                _monthCalendarAccessibleObject = calendarAccessibleObject ?? throw new ArgumentNullException(nameof(calendarAccessibleObject));
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.HasKeyboardFocusPropertyId => HasKeyboardFocus,
                    UiaCore.UIA.IsEnabledPropertyId => IsEnabled,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => false,
                    UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId),
                    UiaCore.UIA.LegacyIAccessibleRolePropertyId => Role,
                    UiaCore.UIA.LegacyIAccessibleStatePropertyId => State,
                    UiaCore.UIA.NamePropertyId => Name,
                    _ => base.GetPropertyValue(propertyID)
                };

            private protected virtual bool HasKeyboardFocus => false;

            private protected virtual bool IsEnabled => _monthCalendarAccessibleObject.IsEnabled;

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.LegacyIAccessiblePatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => _monthCalendarAccessibleObject;

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.Parent => Parent,
                    _ => base.FragmentNavigate(direction)
                };

            // This value wasn't saved to _initRuntimeId as in the rest calendar accessible objects
            // because GetChildId requires _monthCalendarAccessibleObject existing
            // but it will be null because an inherited constructor is not called yet.
            internal override int[] RuntimeId
                => new int[]
                {
                    RuntimeIDFirstItem,
                    _monthCalendarAccessibleObject.Owner.InternalHandle.ToInt32(),
                    GetChildId()
                };

            public override AccessibleStates State => AccessibleStates.None;
        }
    }
}
