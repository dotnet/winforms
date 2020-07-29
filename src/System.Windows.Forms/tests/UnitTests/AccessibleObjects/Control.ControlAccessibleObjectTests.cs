// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using InternalUtilitiesForTests.src;
using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class Control_ControlAccessibleObjectTests
    {
        public static IEnumerable<object[]> ControlObject_TestData()
        {
            return ReflectionHelper.GetDerivedPublicNotAbstractClasses<Control>();
        }

        [StaTheory]
        [MemberData(nameof(ControlObject_TestData))]
        public void ControlAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected(Type type)
        {
            using Control control = ReflectionHelper.InvokePublicConstructor<Control>(type);

            if (control == null || !control.SupportsUiaProviders)
            {
                return;
            }

            control.AccessibleRole = AccessibleRole.Link;
            AccessibleObject controlAccessibleObject = control.AccessibilityObject;

            var accessibleObjectRole = controlAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [StaTheory]
        [MemberData(nameof(ControlObject_TestData))]
        public void ControlAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue(Type type)
        {
            using Control control = ReflectionHelper.InvokePublicConstructor<Control>(type);

            if (control == null || !control.SupportsUiaProviders)
            {
                return;
            }

            AccessibleObject controlAccessibleObject = control.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = controlAccessibleObject.IsPatternSupported(NativeMethods.UIA_LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [StaTheory]
        [MemberData(nameof(ControlObject_TestData))]
        public void ControlAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected(Type type)
        {
            using Control control = ReflectionHelper.InvokePublicConstructor<Control>(type);

            if (control == null || !control.SupportsUiaProviders)
            {
                return;
            }

            control.AccessibleDescription = "Test Accessible Description";
            AccessibleObject controlAccessibleObject = control.AccessibilityObject;

            var accessibleObjectDescription = controlAccessibleObject.Description;

            Assert.Equal("Test Accessible Description", accessibleObjectDescription);
        }

        [StaTheory]
        [MemberData(nameof(ControlObject_TestData))]
        public void ToolStripItemAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected(Type type)
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
