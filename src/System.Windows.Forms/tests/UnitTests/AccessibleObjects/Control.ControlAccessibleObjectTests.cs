// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class Control_ControlAccessibleObjectTests
    {
        public static IEnumerable<object[]> ControlAccessibleObject_IsPatternSupported_LegacyIAccessible_TestData()
        {
            var supportedLegacyIAccessiblePatternClasses = new List<Type> {
                typeof(Form),
                typeof(ListBox), 
                typeof(MonthCalendar),
                typeof(PrintPreviewDialog) 
            };

            return ReflectionHelper.GetPublicNotAbstractClasses<Control>()
                .Select(type => new object[] { type, supportedLegacyIAccessiblePatternClasses.Contains(type) });
        }

        [StaTheory]
        [MemberData(nameof(ControlAccessibleObject_IsPatternSupported_LegacyIAccessible_TestData))]
        public void ControlAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsExpected(Type type, bool legacyIAccessiblePatternSupported)
        {
            using Control control = ReflectionHelper.InvokePublicConstructor<Control>(type);

            if (control == null)
            {
                return;
            }

            AccessibleObject controlAccessibleObject = control.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = controlAccessibleObject.IsPatternSupported(NativeMethods.UIA_LegacyIAccessiblePatternId);

            Assert.Equal(legacyIAccessiblePatternSupported, supportsLegacyIAccessiblePatternId);
        }

        public static IEnumerable<object[]> ControlAccessibleObject_TestData()
        {
            return ReflectionHelper.GetPublicNotAbstractClasses<Control>().Select(type => new object[] { type });
        }

        [StaTheory]
        [MemberData(nameof(ControlAccessibleObject_TestData))]
        public void ControlAccessibleObject_Custom_Role_ReturnsExpected(Type type)
        {
            using Control control = ReflectionHelper.InvokePublicConstructor<Control>(type);

            if (control == null)
            {
                return;
            }

            control.AccessibleRole = AccessibleRole.Link;
            AccessibleObject controlAccessibleObject = control.AccessibilityObject;

            var accessibleObjectRole = controlAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [StaTheory]
        [MemberData(nameof(ControlAccessibleObject_TestData))]
        public void ControlAccessibleObject_Custom_Description_ReturnsExpected(Type type)
        {
            using Control control = ReflectionHelper.InvokePublicConstructor<Control>(type);

            if (control == null)
            {
                return;
            }

            control.AccessibleDescription = "Test Accessible Description";
            AccessibleObject controlAccessibleObject = control.AccessibilityObject;

            var accessibleObjectDescription = controlAccessibleObject.Description;

            Assert.Equal("Test Accessible Description", accessibleObjectDescription);
        }

        [StaTheory]
        [MemberData(nameof(ControlAccessibleObject_TestData))]
        public void ControlAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected(Type type)
        {
            using Control control = ReflectionHelper.InvokePublicConstructor<Control>(type);

            if (control == null || !control.SupportsUiaProviders)
            {
                return;
            }

            control.Name = "Name1";
            control.AccessibleName = "Test Name";
            AccessibleObject controlAccessibleObject = control.AccessibilityObject;

            var accessibleName = controlAccessibleObject.GetPropertyValue(NativeMethods.UIA_NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }
    }
}
