// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TableLayoutControlCollectionTests
    {
        [Fact]
        public void TableLayoutControlCollection_Ctor_TableLayoutPanel()
        {
            var container = new TableLayoutPanel();
            var collection = new TableLayoutControlCollection(container);
            Assert.Equal(container, collection.Container);
            Assert.Equal(container, collection.Owner);
            Assert.Empty(collection);
        }

        [Fact]
        public void TableLayoutControlCollection_NullContainer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("container", () => new TableLayoutControlCollection(null));
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        public void TableLayoutControlCollection_Add_ValidControl_Success(int column, int row)
        {
            var container = new TableLayoutPanel();
            var collection = new TableLayoutControlCollection(container);
            var control = new Control();
            collection.Add(control, column, row);
            Assert.Equal(control, Assert.Single(collection));
            Assert.Equal(column, container.GetColumn(control));
            Assert.Equal(row, container.GetRow(control));
        }

        [Fact]
        public void TableLayoutControlCollection_Add_NegativeColumn_ThrowsArgumentOutOfRangeException()
        {
            var container = new TableLayoutPanel();
            var collection = new TableLayoutControlCollection(container);
            var control = new Control();
            Assert.Throws<ArgumentOutOfRangeException>("column", () => collection.Add(control, -2, 2));
            Assert.Equal(control, Assert.Single(collection));
            Assert.Equal(-1, container.GetColumn(control));
            Assert.Equal(-1, container.GetRow(control));
        }

        [Fact]
        public void TableLayoutControlCollection_Add_NegativeRow_ThrowsArgumentOutOfRangeException()
        {
            var container = new TableLayoutPanel();
            var collection = new TableLayoutControlCollection(container);
            var control = new Control();
            Assert.Throws<ArgumentOutOfRangeException>("row", () => collection.Add(control, 1, -2));
            Assert.Equal(control, Assert.Single(collection));
            Assert.Equal(1, container.GetColumn(control));
            Assert.Equal(-1, container.GetRow(control));
        }

        [Fact]
        public void TableLayoutControlCollection_Add_NullControl_ThrowsArgumentNullException()
        {
            var container = new TableLayoutPanel();
            var collection = new TableLayoutControlCollection(container);
            Assert.Throws<ArgumentNullException>("control", () => collection.Add(null, 1, 2));
            Assert.Empty(collection);
        }
    }
}
