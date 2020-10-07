// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class HotCommandsAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void HotCommandsAccessibleObject_Ctor_Default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using HotCommands hotCommands = new HotCommands(propertyGrid);
            HotCommandsAccessibleObject accessibleObject = new HotCommandsAccessibleObject(hotCommands, propertyGrid);

            Assert.Equal(hotCommands, accessibleObject.Owner);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(hotCommands.IsHandleCreated);
        }

        [WinFormsFact]
        public void HotCommandsAccessibleObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using HotCommands hotCommands = new HotCommands(propertyGrid);
            // AccessibleRole is not set = Default

            object actual = hotCommands.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.PaneControlTypeId, actual);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(hotCommands.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Client)]
        [InlineData(false, AccessibleRole.None)]
        public void HotCommandsAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using HotCommands hotCommands = new HotCommands(propertyGrid);
            // AccessibleRole is not set = Default

            if (createControl)
            {
                hotCommands.CreateControl();
            }

            AccessibleRole actual = hotCommands.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.Equal(createControl, hotCommands.IsHandleCreated);
        }
    }
}
