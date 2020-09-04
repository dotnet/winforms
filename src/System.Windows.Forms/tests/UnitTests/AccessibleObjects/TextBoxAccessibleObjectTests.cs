// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    }
}
