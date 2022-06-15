// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class ToolStripContainer_ToolStripContainerAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripContainerAccessibleObject_Ctor_Default()
        {
            using ToolStripContainer toolStripContainer = new();
            ToolStripContainer.ToolStripContainerAccessibleObject accessibleObject = new(toolStripContainer);

            Assert.Equal(toolStripContainer, accessibleObject.Owner);
            Assert.False(toolStripContainer.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.AutomationIdPropertyId, "ToolStripContainer1")]
        public void ToolStripContainerAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using var control = new ToolStripContainer
            {
                Name = "ToolStripContainer1",
                AccessibleName = "TestName"
            };

            var accessibleObject = new ToolStripContainer.ToolStripContainerAccessibleObject(control);
            object value = accessibleObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, value);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripContainerAccessibleObject_GetPropertyValue_HasKeyboardFocus_ReturnsTrue_IfControlHasFocus()
        {
            using var control = new ToolStripContainer();

            var accessibleObject = new ToolStripContainer.ToolStripContainerAccessibleObject(control);
            Assert.False(control.IsHandleCreated);
            control.FocusActiveControlInternal();
            bool value = (bool)accessibleObject.GetPropertyValue(UIA.HasKeyboardFocusPropertyId);

            Assert.True(value);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripContainerAccessibleObject_GetPropertyValue_HasKeyboardFocus_ReturnsFalse_IfControlHasNoFocus()
        {
            using var control = new ToolStripContainer();

            var accessibleObject = new ToolStripContainer.ToolStripContainerAccessibleObject(control);
            bool value = (bool)accessibleObject.GetPropertyValue(UIA.HasKeyboardFocusPropertyId);

            Assert.False(value);
            Assert.False(control.IsHandleCreated);
        }
    }
}
