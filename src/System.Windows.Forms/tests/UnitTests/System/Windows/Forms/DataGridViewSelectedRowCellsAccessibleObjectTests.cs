// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewSelectedRowCellsAccessibleObjectTests
    {
        [WinFormsFact]
        public void DataGridViewSelectedRowCellsAccessibleObject_Ctor_Default()
        {
            Type type = typeof(DataGridViewRow)
                .GetNestedType("DataGridViewSelectedRowCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
            var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { null }, null);

            Assert.Null(accessibleObject.TestAccessor().Dynamic.owner);
            Assert.Equal(AccessibleRole.Grouping, accessibleObject.Role);
        }
    }
}
