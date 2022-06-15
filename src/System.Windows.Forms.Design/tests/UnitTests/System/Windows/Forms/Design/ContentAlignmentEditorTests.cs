// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing.Design;
using Xunit;
using System.Reflection;

namespace System.Windows.Forms.Design.Tests
{
    public class ContentAlignmentEditorTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData("_topLeft")]
        [InlineData("_topCenter")]
        [InlineData("_topRight")]
        [InlineData("_middleLeft")]
        [InlineData("_middleCenter")]
        [InlineData("_middleRight")]
        [InlineData("_bottomLeft")]
        [InlineData("_bottomCenter")]
        [InlineData("_bottomRight")]
        public void ContentAlignmentEditor_ContentAlignmentEditor_ContentUI_IsRadioButton(string fieldName)
        {
            ContentAlignmentEditor editor = new();
            Type type = editor.GetType()
                .GetNestedType("ContentUI", BindingFlags.NonPublic | BindingFlags.Instance);
            var contentUI = (Control)Activator.CreateInstance(type);
            var item = (Control)contentUI.GetType()
                .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(contentUI);

            object actual = item.AccessibilityObject.TestAccessor().Dynamic
                .GetPropertyValue(Interop.UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(Interop.UiaCore.UIA.RadioButtonControlTypeId, actual);
        }
    }
}
