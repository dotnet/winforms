// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class DropDownButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DropDownButtonAccessibleObject_Ctor_Default()
        {
            using DropDownButton dropDownButton = new DropDownButton();
            DropDownButtonAccessibleObject accessibleObject = new DropDownButtonAccessibleObject(dropDownButton);

            Assert.Equal(dropDownButton, accessibleObject.Owner);
            Assert.False(dropDownButton.IsHandleCreated);
        }

        [WinFormsFact]
        public void DropDownButtonAccessibleObject_ControlType_IsButton_IfAccessibleRoleIsDefault()
        {
            using DropDownButton dropDownButton = new DropDownButton();
            // AccessibleRole is not set = Default

            object actual = dropDownButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ButtonControlTypeId, actual);
            Assert.False(dropDownButton.IsHandleCreated);
        }

        [WinFormsFact]
        public void DropDownButtonAccessibleObject_Role_IsPushButton_ByDefault()
        {
            using DropDownButton dropDownButton = new DropDownButton();
            // AccessibleRole is not set = Default

            AccessibleRole actual = dropDownButton.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.PushButton, actual);
            Assert.False(dropDownButton.IsHandleCreated);
        }
    }
}
