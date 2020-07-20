// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class PropertyGridAccessibleObjectTests
    {
        [Fact]
        public void PropertyGridAccessibleObject_Ctor_Default()
        {
            PropertyGrid propertyGrid = new PropertyGrid();

            var accessibleObject = new PropertyGridAccessibleObject(propertyGrid);
            Assert.NotNull(accessibleObject.Owner);
            Assert.Equal(propertyGrid, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void PropertyGridAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected()
        {
            using var propertyGrid = new PropertyGrid()
            {
                Name = "Name1",
                AccessibleName = "Test Name"
            };

            AccessibleObject propertyGridAccessibleObject = propertyGrid.AccessibilityObject;
            var accessibleName = propertyGridAccessibleObject.GetPropertyValue(NativeMethods.UIA_NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        [WinFormsFact]
        public void PropertyGridAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var propertyGrid = new PropertyGrid();
            AccessibleObject propertyGridAccessibleObject = propertyGrid.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = propertyGridAccessibleObject.IsPatternSupported(NativeMethods.UIA_LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void PropertyGridAccessibleObject_LegacyIAccessible_Custom_Role_ReturnsExpected()
        {
            using var propertyGrid = new PropertyGrid()
            {
                AccessibleRole = AccessibleRole.Link
            };

            AccessibleObject propertyGridAccessibleObject = propertyGrid.AccessibilityObject;
            var accessibleObjectRole = propertyGridAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [WinFormsFact]
        public void PropertyGridAccessibleObject_LegacyIAccessible_Custom_Description_ReturnsExpected()
        {
            using var propertyGrid = new PropertyGrid()
            {
                AccessibleDescription = "Test Description"
            };

            AccessibleObject propertyGridAccessibleObject = propertyGrid.AccessibilityObject;
            var accessibleObjectDescription = propertyGridAccessibleObject.Description;

            Assert.Equal("Test Description", accessibleObjectDescription);
        }
    }
}
