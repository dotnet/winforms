// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class HScrollBar_HScrollBarAccessibleObjectTests :
        IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void HScrollBarAccessibleObject_ctor_ThrowsException_IfHScrollBarAccessibleObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HScrollBar.HScrollBarAccessibleObject(null));
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.ScrollBar)]
        [InlineData(false, AccessibleRole.None)]
        public void HScrollBarAccessibleObject_Ctor_Default(bool createControl, AccessibleRole accessibleRole)
        {
            using var scrollBar = new HScrollBar();

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
        public void HScrollBarAccessibleObject_Name_Get_ReturnsExpected()
        {
            using var scrollBar = new HScrollBar();
            HScrollBar.HScrollBarAccessibleObject accessibleObject =
                Assert.IsType<HScrollBar.HScrollBarAccessibleObject>(scrollBar.AccessibilityObject);
            Assert.Equal("Horizontal", accessibleObject.Name);
            Assert.False(scrollBar.IsHandleCreated);
        }

        [WinFormsFact]
        public void HScrollBarAccessibleObject_ControlType_IsScrollBar_IfAccessibleRoleIsDefault()
        {
            using HScrollBar scrollBar = new HScrollBar();
            // AccessibleRole is not set = Default

            object actual = scrollBar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ScrollBarControlTypeId, actual);
            Assert.False(scrollBar.IsHandleCreated);
        }

        public static IEnumerable<object[]> HScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(HScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void HScrollBarAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using HScrollBar scrollBar = new HScrollBar();
            scrollBar.AccessibleRole = role;

            object actual = scrollBar.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(scrollBar.IsHandleCreated);
        }
    }
}
