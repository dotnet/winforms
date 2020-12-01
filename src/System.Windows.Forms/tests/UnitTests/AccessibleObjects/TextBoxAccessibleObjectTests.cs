﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class TextBoxAccessibleObjectTests
    {
        [WinFormsTheory]
        [InlineData((int)Interop.UiaCore.UIA.IsTextPatternAvailablePropertyId)]
        [InlineData((int)Interop.UiaCore.UIA.IsTextPattern2AvailablePropertyId)]
        public void TextBoxAccessibleObject_TextPatternAvailable(int propertyId)
        {
            using TextBox textBox = new TextBox();
            AccessibleObject textBoxAccessibleObject = textBox.AccessibilityObject;

            // Interop.UiaCore.UIA accessible level (internal) is less than the test level (public) so it needs boxing and unboxing
            Assert.True((bool)textBoxAccessibleObject.GetPropertyValue((Interop.UiaCore.UIA)propertyId));
            Assert.False(textBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)Interop.UiaCore.UIA.TextPatternId)]
        [InlineData((int)Interop.UiaCore.UIA.TextPattern2Id)]
        public void TextBoxAccessibleObject_TextPatternSupported(int patternId)
        {
            using TextBox textBox = new TextBox();
            AccessibleObject textBoxAccessibleObject = textBox.AccessibilityObject;

            // Interop.UiaCore.UIA accessible level (internal) is less than the test level (public) so it needs boxing and unboxing
            Assert.True(textBoxAccessibleObject.IsPatternSupported((Interop.UiaCore.UIA)patternId));
            Assert.False(textBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBoxAccessibilityObject_ControlType_IsEdit_IfAccessibleRoleIsDefault()
        {
            using TextBox textBox = new TextBox();
            textBox.CreateControl();
            // AccessibleRole is not set = Default

            object actual = textBox.AccessibilityObject.GetPropertyValue(Interop.UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(Interop.UiaCore.UIA.EditControlTypeId, actual);
            Assert.True(textBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBoxAccessibilityObject_Role_IsText_ByDefault()
        {
            using TextBox textBox = new TextBox();
            textBox.CreateControl();
            // AccessibleRole is not set = Default

            AccessibleRole actual = textBox.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Text, actual);
            Assert.True(textBox.IsHandleCreated);
        }

        public static IEnumerable<object[]> TextBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(TextBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void TextBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using TextBox textBox = new TextBox();
            textBox.AccessibleRole = role;

            object actual = textBox.AccessibilityObject.GetPropertyValue(Interop.UiaCore.UIA.ControlTypePropertyId);
            Interop.UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(textBox.IsHandleCreated);
        }
    }
}
