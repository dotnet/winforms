// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.Control;
using static System.Windows.Forms.PropertyGridInternal.PropertyGridView;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class PropertyGridView_GridViewListBoxAccessibleObjectTest : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void GridViewListBoxAccessibleObject_Ctor_Default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
            using GridViewListBox gridViewListBox = new GridViewListBox(propertyGridView);

            Type type = gridViewListBox.AccessibilityObject.GetType();
            ControlAccessibleObject accessibleObject = (ControlAccessibleObject)Activator.CreateInstance(type, gridViewListBox);

            Assert.Equal(gridViewListBox, accessibleObject.Owner);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(gridViewListBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void GridViewListBoxAccessibleObject_ControlType_IsList_IfAccessibleRoleIsDefault()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
            AccessibleObject accessibleObject = propertyGridView.DropDownListBoxAccessibleObject;
            // AccessibleRole is not set = Default

            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ListControlTypeId, actual);
            Assert.False(propertyGrid.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.List)]
        [InlineData(false, AccessibleRole.None)]
        public void GridViewListBoxAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
            using GridViewListBox gridViewListBox = new GridViewListBox(propertyGridView);
            // AccessibleRole is not set = Default

            if (createControl)
            {
                gridViewListBox.CreateControl();
            }

            AccessibleRole actual = gridViewListBox.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.Equal(createControl, gridViewListBox.IsHandleCreated);
        }
    }
}
