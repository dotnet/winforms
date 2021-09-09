// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewLinkCellAccessibleObjectTests : DataGridViewLinkCell
    {
        [WinFormsFact]
        public void DataGridViewLinkCellAccessibleObject_Ctor_Default()
        {
            var accessibleObject = new DataGridViewLinkCellAccessibleObject(null);
            Assert.Null(accessibleObject.Owner);
            Assert.Equal(AccessibleRole.Cell, accessibleObject.Role);
        }
    }
}
