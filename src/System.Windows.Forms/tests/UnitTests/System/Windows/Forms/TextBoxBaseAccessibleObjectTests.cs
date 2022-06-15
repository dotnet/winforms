// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class TextBoxBaseAccessibleObjectTests
    {
        [WinFormsFact]
        public void TextBoxBaseAccessibleObject_ctor_default()
        {
            using TextBoxBase textBoxBase = new SubTextBoxBase();

            AccessibleObject textBoxAccessibleObject = textBoxBase.AccessibilityObject;
            Assert.NotNull(textBoxAccessibleObject);

            TextBoxBase.TextBoxBaseAccessibleObject textBoxBaseAccessibleObject = new TextBoxBase.TextBoxBaseAccessibleObject(textBoxBase);
            Assert.NotNull(textBoxBaseAccessibleObject);
            Assert.False(textBoxBase.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)Interop.UiaCore.UIA.IsTextPatternAvailablePropertyId)]
        [InlineData((int)Interop.UiaCore.UIA.IsTextPattern2AvailablePropertyId)]
        public void TextBoxBaseAccessibleObject_TextPatternAvailable(int propertyId)
        {
            using TextBoxBase textBoxBase = new SubTextBoxBase();
            AccessibleObject textBoxAccessibleObject = textBoxBase.AccessibilityObject;

            // Interop.UiaCore.UIA accessible level (internal) is less than the test level (public) so it needs boxing and unboxing
            Assert.True((bool)textBoxAccessibleObject.GetPropertyValue((Interop.UiaCore.UIA)propertyId));
            Assert.False(textBoxBase.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)Interop.UiaCore.UIA.TextPatternId)]
        [InlineData((int)Interop.UiaCore.UIA.TextPattern2Id)]
        public void TextBoxBaseAccessibleObject_TextPatternSupported(int patternId)
        {
            using TextBoxBase textBoxBase = new SubTextBoxBase();
            AccessibleObject textBoxAccessibleObject = textBoxBase.AccessibilityObject;

            // Interop.UiaCore.UIA accessible level (internal) is less than the test level (public) so it needs boxing and unboxing
            Assert.True(textBoxAccessibleObject.IsPatternSupported((Interop.UiaCore.UIA)patternId));
            Assert.False(textBoxBase.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void TextBoxBaseAccessibleObject_IsReadOnly_ReturnsCorrectValue(bool readOnly)
        {
            using TextBoxBase textBoxBase = new SubTextBoxBase();
            AccessibleObject accessibleObject = textBoxBase.AccessibilityObject;

            textBoxBase.ReadOnly = readOnly;
            Assert.Equal(textBoxBase.ReadOnly, accessibleObject.IsReadOnly);
            Assert.False(textBoxBase.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBoxBaseAccessibleObject_Value_ReturnsEmpty_WithoutHandle()
        {
            using TextBoxBase textBoxBase = new SubTextBoxBase();
            textBoxBase.Text = "Some test text";
            AccessibleObject accessibleObject = textBoxBase.AccessibilityObject;
            Assert.Equal(string.Empty, accessibleObject.Value);
            Assert.False(textBoxBase.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBoxBaseAccessibleObject_Value_EqualsText()
        {
            using TextBoxBase textBoxBase = new SubTextBoxBase();
            textBoxBase.CreateControl();
            textBoxBase.Text = "Some test text";
            AccessibleObject accessibleObject = textBoxBase.AccessibilityObject;
            Assert.Equal(textBoxBase.Text, accessibleObject.Value);
            Assert.True(textBoxBase.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(50, 20)]
        [InlineData(100, 10)]
        public void TextBoxBaseAccessibleObject_BoundingRectangle_IsCorrect(int width, int height)
        {
            using TextBoxBase textBoxBase = new SubTextBoxBase { Size = new Size(width, height) };
            textBoxBase.CreateControl();
            AccessibleObject accessibleObject = textBoxBase.AccessibilityObject;
            Rectangle expected = textBoxBase.RectangleToScreen(textBoxBase.ClientRectangle); // Forces Handle creating
            Rectangle actual = accessibleObject.BoundingRectangle;
            Assert.Equal(expected, actual);
            Assert.True(textBoxBase.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Text, (int)UiaCore.UIA.EditControlTypeId)]
        [InlineData(false, AccessibleRole.None, (int)UiaCore.UIA.PaneControlTypeId)]
        public void TextBoxBaseAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(bool createControl, AccessibleRole expectedRole, int expectedType)
        {
            using TextBoxBase textBoxBase = new SubTextBoxBase();
            // AccessibleRole is not set = Default

            if (createControl)
            {
                textBoxBase.CreateControl();
            }

            AccessibleObject accessibleObject = textBoxBase.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(expectedRole, accessibleObject.Role);
            Assert.Equal((UiaCore.UIA)expectedType, actual);
            Assert.Equal(createControl, textBoxBase.IsHandleCreated);
        }

        public static IEnumerable<object[]> TextBoxBaseAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(TextBoxBaseAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void TextBoxBaseAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using TextBoxBase textBoxBase = new SubTextBoxBase();
            textBoxBase.AccessibleRole = role;

            object actual = textBoxBase.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(textBoxBase.IsHandleCreated);
        }

        private class SubTextBoxBase : TextBoxBase
        { }
    }
}
