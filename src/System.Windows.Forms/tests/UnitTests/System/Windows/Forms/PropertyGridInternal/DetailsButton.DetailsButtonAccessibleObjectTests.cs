// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class DetailsButton_DetailsButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DetailsButtonAccessibleObject_Ctor_Default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using GridErrorDialog gridErrorDlg = new GridErrorDialog(propertyGrid);
            using DetailsButton detailsButton = new DetailsButton(gridErrorDlg);
            DetailsButton.DetailsButtonAccessibleObject accessibleObject = new DetailsButton.DetailsButtonAccessibleObject(detailsButton);

            Assert.Equal(detailsButton, accessibleObject.Owner);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(gridErrorDlg.IsHandleCreated);
            Assert.False(detailsButton.IsHandleCreated);
        }

        [WinFormsFact]
        public void DetailsButtonAccessibleObject_ControlType_IsButton_IfAccessibleRoleIsDefault()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using GridErrorDialog gridErrorDlg = new GridErrorDialog(propertyGrid);
            using DetailsButton detailsButton = new DetailsButton(gridErrorDlg);
            // AccessibleRole is not set = Default

            object actual = detailsButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ButtonControlTypeId, actual);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(gridErrorDlg.IsHandleCreated);
            Assert.False(detailsButton.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.PushButton)]
        [InlineData(false, AccessibleRole.None)]
        public void DetailsButtonAccessibleObject_Role_IsPushButton_ByDefault(bool createControl, AccessibleRole expectedRole)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using GridErrorDialog gridErrorDlg = new GridErrorDialog(propertyGrid);
            using DetailsButton detailsButton = new DetailsButton(gridErrorDlg);
            // AccessibleRole is not set = Default

            if (createControl)
            {
                detailsButton.CreateControl();
            }

            AccessibleRole actual = detailsButton.AccessibilityObject.Role;

            Assert.Equal(expectedRole, actual);
            Assert.False(propertyGrid.IsHandleCreated);
            Assert.False(gridErrorDlg.IsHandleCreated);
            Assert.Equal(createControl, detailsButton.IsHandleCreated);
        }
    }
}
