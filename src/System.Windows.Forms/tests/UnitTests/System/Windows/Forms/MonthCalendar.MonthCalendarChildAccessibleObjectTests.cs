// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.MonthCalendar;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class MonthCalendar_MonthCalendarChildAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void MonthCalendarChildAccessibleObject_ctor_ThrowsException_IfMonthCalendarAccessibleObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SubObject(null));
        }

        [WinFormsFact]
        public void MonthCalendarChildAccessibleObject_ctor_default()
        {
            using MonthCalendar control = new MonthCalendar();

            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);

            Assert.Equal(controlAccessibleObject, accessibleObject.FragmentRoot);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.HasKeyboardFocusPropertyId, false)]
        [InlineData((int)UiaCore.UIA.IsEnabledPropertyId, true)]
        [InlineData((int)UiaCore.UIA.IsKeyboardFocusablePropertyId, false)]
        [InlineData((int)UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId, true)]
        [InlineData((int)UiaCore.UIA.LegacyIAccessibleRolePropertyId, AccessibleRole.None)]
        [InlineData((int)UiaCore.UIA.LegacyIAccessibleStatePropertyId, AccessibleStates.None)]
        [InlineData((int)UiaCore.UIA.NamePropertyId, null)]
        public void MonthCalendarChildAccessibleObject_GetPropertyValue_ReturnsExpected(int property, object expected)
        {
            using MonthCalendar control = new MonthCalendar();

            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);
            object actual = accessibleObject.GetPropertyValue((UiaCore.UIA)property);

            Assert.Equal(expected, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void MonthCalendarChildAccessibleObject_LegacyIAccessiblePattern_IsSupported()
        {
            using MonthCalendar control = new MonthCalendar();

            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);

            Assert.True(accessibleObject.IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void MonthCalendarChildAccessibleObject_FragmentRoot_IsControlAccessibleObject()
        {
            using MonthCalendar control = new MonthCalendar();

            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);

            Assert.Equal(controlAccessibleObject, accessibleObject.FragmentRoot);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.NavigateDirection.FirstChild)]
        [InlineData((int)UiaCore.NavigateDirection.LastChild)]
        [InlineData((int)UiaCore.NavigateDirection.NextSibling)]
        [InlineData((int)UiaCore.NavigateDirection.PreviousSibling)]
        public void MonthCalendarChildAccessibleObject_FragmentNavigate_DoesntHaveChildrenAndSiblings(int direction)
        {
            using MonthCalendar control = new MonthCalendar();

            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);

            Assert.Null(accessibleObject.FragmentNavigate((UiaCore.NavigateDirection)direction));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void MonthCalendarChildAccessibleObject_FragmentNavigate_Parent_IsNull()
        {
            using MonthCalendar control = new MonthCalendar();

            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void MonthCalendarChildAccessibleObject_RuntimeId_HasThreeExpectedItems()
        {
            using MonthCalendar control = new MonthCalendar();

            control.CreateControl();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            MonthCalendarChildAccessibleObject accessibleObject = new SubObject(controlAccessibleObject);

            Assert.Equal(3, accessibleObject.RuntimeId.Length);
            Assert.Equal(AccessibleObject.RuntimeIDFirstItem, accessibleObject.RuntimeId[0]);
            Assert.Equal(PARAM.ToInt(control.Handle), accessibleObject.RuntimeId[1]);
            Assert.Equal(accessibleObject.GetChildId(), accessibleObject.RuntimeId[2]);
            Assert.True(control.IsHandleCreated);
        }

        private class SubObject : MonthCalendarChildAccessibleObject
        {
            public SubObject(MonthCalendarAccessibleObject calendarAccessibleObject)
                : base(calendarAccessibleObject)
            { }
        }
    }
}
