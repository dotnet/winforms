// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class GridTablesFactoryTests
    {
        public static IEnumerable<object[]> CreateGridTables_TestData()
        {
            yield return new object[] { null, null, null, null, new DataGridTableStyle[] { null } };
            var style = new DataGridTableStyle();
            yield return new object[] { style, new object(), string.Empty, new BindingContext(), new DataGridTableStyle[] { style } };
            yield return new object[] { style, new object(), "dataMember", new BindingContext(), new DataGridTableStyle[] { style } };
        }

        [Theory]
        [MemberData(nameof(CreateGridTables_TestData))]
        public void GridTablesFactory_CreateGridTables_Invoke_ReturnsExpected(DataGridTableStyle gridTable, object dataSource, string dataMember, BindingContext bindingManager, DataGridTableStyle[] expected)
        {
            Assert.Equal(expected, GridTablesFactory.CreateGridTables(gridTable, dataSource, dataMember, bindingManager));
        }
    }
}
