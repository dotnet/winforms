// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class LabelAccessibleObjectTests
    {
        [WinFormsFact]
        public void LabelAccessibleObject_Constructor_InitializesOwner()
        {
            using var label = new Label();
            var labelAccessibleObject = new Label.LabelAccessibleObject(label);

            Assert.NotNull(labelAccessibleObject.Owner);
            Assert.Same(label, labelAccessibleObject.Owner);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_AccessibleRole_Default_is_StaticText()
        {
            using var label = new Label();
            label.AccessibleRole = AccessibleRole.Default;
            var labelAccessibleObject = new Label.LabelAccessibleObject(label);

            Assert.Equal(AccessibleRole.StaticText, labelAccessibleObject.Role);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_AccessibleRole_Custom_is_correct()
        {
            using var label = new Label();
            label.AccessibleRole = AccessibleRole.Link;
            var labelAccessibleObject = new Label.LabelAccessibleObject(label);

            Assert.Equal(AccessibleRole.Link, labelAccessibleObject.Role);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_GetPropertyValue_returns_correct_values()
        {
            using var label = new Label();
            label.Name = "Label1";
            label.AccessibleName = "Address";
            var labelAccessibleObject = new Label.LabelAccessibleObject(label);

            var accessibleName = labelAccessibleObject.GetPropertyValue(Interop.UiaCore.UIA.NamePropertyId);
            Assert.Equal("Address", accessibleName);

            var automationId = labelAccessibleObject.GetPropertyValue(Interop.UiaCore.UIA.AutomationIdPropertyId);
            Assert.Equal("Label1", automationId);

            var accessibleControlType = labelAccessibleObject.GetPropertyValue(Interop.UiaCore.UIA.ControlTypePropertyId);
            Assert.Equal(Interop.UiaCore.UIA.TextControlTypeId, accessibleControlType);
        }
    }
}
