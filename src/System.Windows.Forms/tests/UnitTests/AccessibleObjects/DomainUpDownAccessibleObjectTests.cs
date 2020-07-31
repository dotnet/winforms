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
            using DomainUpDown numericUpDown = new DomainUpDown();
            AccessibleObject accessibleObject = numericUpDown.AccessibilityObject;
            Assert.NotNull(accessibleObject);
        }

        [WinFormsFact]
        public void DomainUpDownAccessibleObject_NameIsNotEqualControlType()
        {
            using DomainUpDown numericUpDown = new DomainUpDown();
            string accessibleName = numericUpDown.AccessibilityObject.Name;

            // Control should have null AccessibleName by default
            Assert.Null(accessibleName);
        }
    }
}
