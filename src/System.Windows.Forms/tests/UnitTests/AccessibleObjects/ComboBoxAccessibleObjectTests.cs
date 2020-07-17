﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class ComboBoxAccessibleObjectTests
    {
        public static IEnumerable<object[]> Ctor_ComboBox_TestData()
        {
            yield return new object[] { new ComboBox() };
        }

        [Theory]
        [MemberData(nameof(Ctor_ComboBox_TestData))]
        public void ComboBoxAccessibleObject_Ctor_Default(ComboBox owner)
        {
            var accessibleObject = new ComboBox.ComboBoxAccessibleObject(owner);
            Assert.NotNull(accessibleObject.Owner);
            Assert.Equal(AccessibleRole.ComboBox, accessibleObject.Role);
        }

        public static IEnumerable<object[]> ComboBoxAccessibleObject_TestData()
        {
            ComboBox dropDownComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDown
            };
            yield return new object[] { dropDownComboBox.AccessibilityObject };

            ComboBox dropDownListComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            yield return new object[] { dropDownListComboBox.AccessibilityObject };
        }

        [Theory]
        [MemberData(nameof(ComboBoxAccessibleObject_TestData))]
        public void ComboBoxAccessibleObject_ExpandCollapse_Set_CollapsedState(AccessibleObject accessibleObject)
        {
            accessibleObject.Expand();
            Assert.NotEqual(AccessibleStates.Collapsed, accessibleObject.State & AccessibleStates.Collapsed);
            Assert.Equal(AccessibleStates.Expanded, accessibleObject.State & AccessibleStates.Expanded);

            accessibleObject.Collapse();
            Assert.Equal(AccessibleStates.Collapsed, accessibleObject.State & AccessibleStates.Collapsed);
            Assert.NotEqual(AccessibleStates.Expanded, accessibleObject.State & AccessibleStates.Expanded);
        }

        [Theory]
        [MemberData(nameof(ComboBoxAccessibleObject_TestData))]
        public void ComboBoxAccessibleObject_FragmentNavigate_FirstChild_NotNull(AccessibleObject accessibleObject)
        {
            UnsafeNativeMethods.IRawElementProviderFragment firstChild = accessibleObject.FragmentNavigate(UnsafeNativeMethods.NavigateDirection.FirstChild);
            Assert.NotNull(firstChild);
        }

        [Theory]
        [InlineData("Test text")]
        [InlineData(null)]
        public void ComboBoxEditAccessibleObject_NameNotNull(string name)
        {
            ComboBox comboBox = new ComboBox();
            comboBox.AccessibleName = name;
            comboBox.CreateControl(false);
            object editAccessibleName = comboBox.ChildEditAccessibleObject.GetPropertyValue(NativeMethods.UIA_NamePropertyId);
            Assert.NotNull(editAccessibleName);
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
        {
            using var comboBox = new ComboBox()
            {
                Name = "Name1",
                AccessibleName = "Test Name"
            };

            AccessibleObject comboBoxAccessibleObject = comboBox.AccessibilityObject;
            var accessibleName = comboBoxAccessibleObject.GetPropertyValue(NativeMethods.UIA_NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var comboBox = new ComboBox();
            AccessibleObject comboBoxAccessibleObject = comboBox.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = comboBoxAccessibleObject.IsPatternSupported(NativeMethods.UIA_LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_LegacyIAccessible_Role_ReturnsExpected()
        {
            using var comboBox = new ComboBox()
            {
                AccessibleRole = AccessibleRole.Link
            };

            AccessibleObject comboBoxAccessibleObject = comboBox.AccessibilityObject;
            var accessibleObjectRole = comboBoxAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [WinFormsFact]
        public void ComboBoxAccessibleObject_LegacyIAccessible_Description_ReturnsExpected()
        {
            using var comboBox = new ComboBox()
            {
                AccessibleDescription = "Test Description"
            };

            AccessibleObject comboBoxAccessibleObject = comboBox.AccessibilityObject;
            var accessibleObjectDescription = comboBoxAccessibleObject.Description;

            Assert.Equal("Test Description", accessibleObjectDescription);
        }
    }
}
