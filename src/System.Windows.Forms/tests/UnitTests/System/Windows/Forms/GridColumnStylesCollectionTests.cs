// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class GridColumnStylesCollectionTests
    {
        [Fact]
        public void GridColumnStylesCollection_Add_DataGridColumnStyle_Success()
        {
            var tableStyle = new DataGridTableStyle();
            GridColumnStylesCollection collection = tableStyle.GridColumnStyles;
            var style = new SubDataGridColumnStyle();
            Assert.Equal(0, collection.Add(style));
            Assert.Same(style, Assert.Single(collection));
            Assert.Same(tableStyle, style.DataGridTableStyle);
        }

        private class SubDataGridColumnStyle : DataGridColumnStyle
        {
            protected internal override void Abort(int rowNum)
            {
                throw new NotImplementedException();
            }

            protected internal override bool Commit(CurrencyManager dataSource, int rowNum)
            {
                throw new NotImplementedException();
            }

            protected internal override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText, bool cellIsVisible)
            {
                throw new NotImplementedException();
            }

            protected internal override Size GetPreferredSize(Graphics g, object value)
            {
                throw new NotImplementedException();
            }

            protected internal override int GetMinimumHeight()
            {
                throw new NotImplementedException();
            }

            protected internal override int GetPreferredHeight(Graphics g, object value)
            {
                throw new NotImplementedException();
            }

            protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
            {
                throw new NotImplementedException();
            }

            protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
            {
                throw new NotImplementedException();
            }
        }
    }
}
