// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class HelpPane_HelpPaneAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void HelpPaneAccessibleObject_Ctor_Default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using HelpPane helpPane = new HelpPane(propertyGrid);
            HelpPane.HelpPaneAccessibleObject accessibleObject =
                new HelpPane.HelpPaneAccessibleObject(helpPane, propertyGrid);

            Assert.Equal(helpPane, accessibleObject.Owner);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(helpPane.IsHandleCreated);
        }

        [WinFormsFact]
        public void HelpPaneAccessibleObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using HelpPane helpPane = new HelpPane(propertyGrid);
            // AccessibleRole is not set = Default

            object actual = helpPane.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.PaneControlTypeId, actual);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(helpPane.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Client)]
        [InlineData(false, AccessibleRole.None)]
        public void HelpPaneAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using HelpPane helpPane = new HelpPane(propertyGrid);
            // AccessibleRole is not set = Default

            if (createControl)
            {
                helpPane.CreateControl();
            }

            AccessibleRole actual = helpPane.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.Equal(createControl, helpPane.IsHandleCreated);
        }
    }
}
