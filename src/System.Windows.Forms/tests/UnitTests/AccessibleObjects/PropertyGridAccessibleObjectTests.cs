﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows.Forms.PropertyGridInternal;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class PropertyGridAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void PropertyGridAccessibleObject_Ctor_Default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridAccessibleObject accessibleObject = new PropertyGridAccessibleObject(propertyGrid);
            Assert.NotNull(accessibleObject.Owner);
            Assert.Equal(propertyGrid, accessibleObject.Owner);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.TableItemPatternId)]
        [InlineData((int)UiaCore.UIA.GridItemPatternId)]
        public void GridEntryAccessibleObject_SupportsPattern(int pattern)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using ComboBox comboBox = new ComboBox();
            propertyGrid.SelectedObject = comboBox;
            GridEntry defaultGridEntry = propertyGrid.GetDefaultGridEntry();
            GridEntry parentGridEntry = defaultGridEntry.ParentGridEntry; // Category which has item pattern.
            AccessibleObject accessibleObject = parentGridEntry.AccessibilityObject;
            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)pattern));
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.GridPatternId)]
        [InlineData((int)UiaCore.UIA.TablePatternId)]
        public void PropertyGridAccessibleObject_SupportsPattern(int pattern)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            propertyGrid.CreateControl();
            using ComboBox comboBox = new ComboBox();
            propertyGrid.SelectedObject = comboBox;
            PropertyGridAccessibleObject propertyGridAccessibleObject = new PropertyGridAccessibleObject(propertyGrid);

            // First child should be PropertyGrid toolbox.
            AccessibleObject firstChild = (AccessibleObject)propertyGridAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);

            // Second child entry should be PropertyGridView.
            AccessibleObject gridViewChild = (AccessibleObject)firstChild.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);

            Assert.True(gridViewChild.IsPatternSupported((UiaCore.UIA)pattern));
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Client)]
        [InlineData(false, AccessibleRole.None)]
        public void PropertyGridAccessibleObject_ControlType_IsPane_IfAccessibleRoleIsDefault(bool createControl, AccessibleRole expectedRole)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            // AccessibleRole is not set = Default

            if (createControl)
            {
                propertyGrid.CreateControl();
            }

            AccessibleObject accessibleObject = propertyGrid.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(expectedRole, accessibleObject.Role);
            Assert.Equal(UiaCore.UIA.PaneControlTypeId, actual);
            Assert.Equal(createControl, propertyGrid.IsHandleCreated);
        }

        public static IEnumerable<object[]> PropertyGridAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(PropertyGridAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void PropertyGridAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            propertyGrid.AccessibleRole = role;

            object actual = propertyGrid.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(propertyGrid.IsHandleCreated);
        }
    }
}
