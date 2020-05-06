// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Threading;
using Microsoft.DotNet.RemoteExecutor;
using Xunit;
using static System.Windows.Forms.MonthCalendar;

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
    }
}
