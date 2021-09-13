// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewSelectedCellsAccessibleObjectTests
    {
        [WinFormsFact]
        public void DataGridViewSelectedCellsAccessibleObject_Ctor_Default()
        {
            Type type = typeof(DataGridView)
                .GetNestedType("DataGridViewSelectedCellsAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
            var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null });
            Assert.Null(accessibleObject.TestAccessor().Dynamic._ownerDataGridView);
            Assert.Equal(AccessibleRole.Grouping, accessibleObject.Role);
        }
    }
}
