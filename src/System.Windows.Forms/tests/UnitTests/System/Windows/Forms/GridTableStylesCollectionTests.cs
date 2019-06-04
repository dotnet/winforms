// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class GridTableStylesCollectionTests
    {
        [Fact]
        public void GridTableStylesCollection_Add_DataGridTableStyle_Success()
        {
            var dataGrid = new DataGrid();
            GridTableStylesCollection collection = dataGrid.TableStyles;
            var style = new DataGridTableStyle();
            Assert.Equal(0, collection.Add(style));
            Assert.Same(style, Assert.Single(collection));
            Assert.Same(dataGrid, style.DataGrid);
        }
    }
}
