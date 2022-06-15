// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class CommandsPane_CommandsPaneAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CommandsPaneAccessibleObject_Ctor_Default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using CommandsPane commandsPane = new CommandsPane(propertyGrid);
            CommandsPane.CommandsPaneAccessibleObject accessibleObject =
                new CommandsPane.CommandsPaneAccessibleObject(commandsPane, propertyGrid);

            Assert.Equal(commandsPane, accessibleObject.Owner);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(commandsPane.IsHandleCreated);
        }

        [WinFormsFact]
        public void CommandsPaneAccessibleObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using CommandsPane commandsPane = new CommandsPane(propertyGrid);
            // AccessibleRole is not set = Default

            object actual = commandsPane.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.PaneControlTypeId, actual);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(commandsPane.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Client)]
        [InlineData(false, AccessibleRole.None)]
        public void CommandsPaneAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using CommandsPane commandsPane = new CommandsPane(propertyGrid);
            // AccessibleRole is not set = Default

            if (createControl)
            {
                commandsPane.CreateControl();
            }

            AccessibleRole actual = commandsPane.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.Equal(createControl, commandsPane.IsHandleCreated);
        }
    }
}
