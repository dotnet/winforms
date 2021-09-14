// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class PropertyGridToolStrip_PropertyGridToolStripAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void PropertyGridToolStripAccessibleObject_Ctor_Default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using PropertyGridToolStrip propertyGridToolStrip = new PropertyGridToolStrip(propertyGrid);
            PropertyGridToolStrip.PropertyGridToolStripAccessibleObject accessibleObject =
                new PropertyGridToolStrip.PropertyGridToolStripAccessibleObject(propertyGridToolStrip, propertyGrid);

            Assert.Equal(propertyGridToolStrip, accessibleObject.Owner);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(propertyGridToolStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void PropertyGridToolStripAccessibleObject_ControlType_IsToolBar_IfAccessibleRoleIsDefault()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using PropertyGridToolStrip propertyGridToolStrip = new PropertyGridToolStrip(propertyGrid);
            // AccessibleRole is not set = Default

            object actual = propertyGridToolStrip.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ToolBarControlTypeId, actual);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(propertyGridToolStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void PropertyGridToolStripAccessibleObject_Role_IsToolBar_ByDefault()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using PropertyGridToolStrip propertyGridToolStrip = new PropertyGridToolStrip(propertyGrid);
            // AccessibleRole is not set = Default

            AccessibleRole actual = propertyGridToolStrip.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.ToolBar, actual);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(propertyGridToolStrip.IsHandleCreated);
        }
    }
}
