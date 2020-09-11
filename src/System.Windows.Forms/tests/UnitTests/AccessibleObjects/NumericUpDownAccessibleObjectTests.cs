// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class NumericUpDownAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void NumericUpDownAccessibleObject_Ctor_Default()
        {
            using NumericUpDown numericUpDown = new NumericUpDown();
            AccessibleObject accessibleObject = numericUpDown.AccessibilityObject;
            Assert.NotNull(accessibleObject);
        }
    }
}
