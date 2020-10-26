// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class DomainUpDownAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DomainUpDownAccessibleObject_Ctor_Default()
        {
            using DomainUpDown domainUpDown = new DomainUpDown();
            AccessibleObject accessibleObject = domainUpDown.AccessibilityObject;
            Assert.NotNull(accessibleObject);
        }

        [WinFormsFact]
        public void DomainUpDownAccessibleObject_GetPropertyValue_IsKeyboardFocusable_ReturnsTrue()
        {
            using DomainUpDown domainUpDown = new DomainUpDown();
            AccessibleObject accessibleObject = domainUpDown.AccessibilityObject;

            bool isKeyboardFocusable = (bool)accessibleObject.GetPropertyValue(Interop.UiaCore.UIA.IsKeyboardFocusablePropertyId);
            Assert.True(isKeyboardFocusable);
        }

        [WinFormsFact]
        public void DomainUpDownAccessibleObject_GetPropertyValue_IsKeyboardFocusable_WhenDisabled_ReturnsFalse()
        {
            using DomainUpDown domainUpDown = new DomainUpDown();
            AccessibleObject accessibleObject = domainUpDown.AccessibilityObject;

            domainUpDown.Enabled = false;

            bool isKeyboardFocusable = (bool)accessibleObject.GetPropertyValue(Interop.UiaCore.UIA.IsKeyboardFocusablePropertyId);
            Assert.False(isKeyboardFocusable);
        }
    }
}
