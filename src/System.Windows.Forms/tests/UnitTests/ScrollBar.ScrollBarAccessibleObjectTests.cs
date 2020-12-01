﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static Interop;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class ScrollBar_ScrollBarAccessibleObjectTests :
        IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ScrollBarAccessibleObject_ctor_ThrowsException_IfScrollBarAccessibleObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ScrollBar.ScrollBarAccessibleObject(null));
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.ScrollBar)]
        [InlineData(false, AccessibleRole.None)]
        public void ScrollBarAccessibleObject_Ctor_Default(bool createControl, AccessibleRole accessibleRole)
        {
            using var scrollBar = new SubScrollBar();

            if (createControl)
            {
                scrollBar.CreateControl();
            }

            AccessibleObject accessibleObject = scrollBar.AccessibilityObject;

            Assert.NotNull(accessibleObject);
            Assert.Equal(accessibleRole, accessibleObject.Role);
            Assert.Equal(createControl, scrollBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void ScrollBarAccessibleObject_IsPatternSupported_Invoke_ReturnsExpected()
        {
            using var scrollBar = new SubScrollBar();
            scrollBar.CreateControl();
            AccessibleObject accessibleObject = scrollBar.AccessibilityObject;

            Assert.True(accessibleObject.IsPatternSupported(UIA.ValuePatternId));
            Assert.True(scrollBar.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.ControlTypePropertyId, UIA.ScrollBarControlTypeId)] // If AccessibleRole is Default
        [InlineData((int)UIA.IsKeyboardFocusablePropertyId, true)]
        [InlineData((int)UIA.IsValuePatternAvailablePropertyId, true)]
        [InlineData((int)UIA.AutomationIdPropertyId, "AutomId")]
        public void ScrollBarAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using var scrollBar = new SubScrollBar
            {
                AccessibleName = "TestName",
                Name = "AutomId"
            };

            Assert.False(scrollBar.IsHandleCreated);
            var scrollBarAccessibleObject = new ScrollBar.ScrollBarAccessibleObject(scrollBar);
            object value = scrollBarAccessibleObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, value);
            Assert.False(scrollBar.IsHandleCreated);
        }

        public static IEnumerable<object[]> ScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ScrollBar scrollBar = new SubScrollBar();
            scrollBar.AccessibleRole = role;

            object actual = scrollBar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(scrollBar.IsHandleCreated);
        }

        private class SubScrollBar : ScrollBar
        {
            public SubScrollBar() : base()
            {
            }
        }
    }
}
