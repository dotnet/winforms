// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewAccessibleObjectTests
    {
        [Fact]
        public void PropertyGridAccessibleObject_Ctor_Default()
        {
            DataGridView dataGridView = new DataGridView();

            var accessibleObject = dataGridView.AccessibilityObject;
            Assert.NotNull(accessibleObject);
            Assert.Equal(AccessibleRole.Table, accessibleObject.Role);
        }
    }
}
