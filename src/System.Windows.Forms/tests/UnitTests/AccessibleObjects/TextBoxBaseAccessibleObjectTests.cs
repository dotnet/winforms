// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
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

        private class SubTextBoxBase : TextBoxBase
        { }
    }
}
