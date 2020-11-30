// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using Xunit;
using static System.Windows.Forms.MonthCalendar;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class MonthCalendarAccessibleObjectTests
    {
        [WinFormsFact]
        public void MonthCalendarAccessibleObject_ctor_ThrowsException_IfOwnerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new MonthCalendarAccessibleObject(null));
        }

        [WinFormsFact]
        public void MonthCalendarAccessibleObject_GetCalendarCell_DoesntThrowException_If_ParentAccessibleObject_IsNull()
        {
            using MonthCalendar monthCalendar = new MonthCalendar();
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;
            Type type = typeof(MonthCalendarAccessibleObject);
            MethodInfo method = type.GetMethod("GetCalendarCell", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.Null(method.Invoke(accessibleObject, new object[] { 0, /*parentAccessibleObject*/ null, 0 }));
        }

        [WinFormsFact]
        public void MonthCalendarAccessibleObject_GetCalendarRow_DoesntThrowException_If_ParentAccessibleObject_IsNull()
        {
            using MonthCalendar monthCalendar = new MonthCalendar();
            MonthCalendarAccessibleObject accessibleObject = (MonthCalendarAccessibleObject)monthCalendar.AccessibilityObject;
            Type type = typeof(MonthCalendarAccessibleObject);
            MethodInfo method = type.GetMethod("GetCalendarCell", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.Null(method.Invoke(accessibleObject, new object[] { 0, /*parentAccessibleObject*/ null, 0 }));
        }

        [WinFormsTheory]
        [InlineData("Test name", (int)UiaCore.UIA.TableControlTypeId)]
        [InlineData(null, (int)UiaCore.UIA.CalendarControlTypeId)]
        public void MonthCalendarAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(string name, int expected)
        {
            // UIA is less accessible than the test
            // so we have to use "int" type here for "expected" argument
            using MonthCalendar monthCalendar = new MonthCalendar()
            {
                AccessibleName = name
            };
            // AccessibleRole is not set = Default

            object actual = monthCalendar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal((UiaCore.UIA)expected, actual);
            Assert.False(monthCalendar.IsHandleCreated);
        }

        [WinFormsFact]
        public void MonthCalendarAccessibleObject_Role_IsExpected_ByDefault()
        {
            using MonthCalendar monthCalendar = new MonthCalendar();
            // AccessibleRole is not set = Default

            AccessibleRole actual = monthCalendar.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Table, actual);
            Assert.False(monthCalendar.IsHandleCreated);
        }

        public static IEnumerable<object[]> MonthCalendarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
        {
            Array roles = Enum.GetValues(typeof(AccessibleRole));

            foreach (AccessibleRole role in roles)
            {
                if (role == AccessibleRole.Default)
                {
                    continue; // The test checks custom roles
                }

                yield return new object[] { role };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(MonthCalendarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void MonthCalendarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using MonthCalendar monthCalendar = new MonthCalendar();
            monthCalendar.AccessibleRole = role;

            object actual = monthCalendar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(monthCalendar.IsHandleCreated);
        }
    }
}
