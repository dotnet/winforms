// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TableLayoutControlCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TableLayoutControlCollection_Ctor_TableLayoutPanel()
        {
            using var container = new TableLayoutPanel();
            var collection = new TableLayoutControlCollection(container);
            Assert.Equal(container, collection.Container);
            Assert.Equal(container, collection.Owner);
            Assert.Empty(collection);
        }

        [WinFormsFact]
        public void TableLayoutControlCollection_NullContainer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("container", () => new TableLayoutControlCollection(null));
        }

        [WinFormsTheory]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        public void TableLayoutControlCollection_Add_ValidControl_Success(int column, int row)
        {
            using var container = new TableLayoutPanel();
            var collection = new TableLayoutControlCollection(container);
            using var child = new Control();
            collection.Add(child, column, row);
            Assert.Equal(child, Assert.Single(collection));
            Assert.Equal(column, container.GetColumn(child));
            Assert.Equal(row, container.GetRow(child));
        }

        [WinFormsFact]
        public void TableLayoutControlCollection_Add_NegativeColumn_ThrowsArgumentOutOfRangeException()
        {
            using var container = new TableLayoutPanel();
            var collection = new TableLayoutControlCollection(container);
            using var child = new Control();
            Assert.Throws<ArgumentOutOfRangeException>("column", () => collection.Add(child, -2, 2));
            Assert.Equal(child, Assert.Single(collection));
            Assert.Equal(-1, container.GetColumn(child));
            Assert.Equal(-1, container.GetRow(child));
        }

        [WinFormsFact]
        public void TableLayoutControlCollection_Add_NegativeRow_ThrowsArgumentOutOfRangeException()
        {
            using var container = new TableLayoutPanel();
            var collection = new TableLayoutControlCollection(container);
            using var child = new Control();
            Assert.Throws<ArgumentOutOfRangeException>("row", () => collection.Add(child, 1, -2));
            Assert.Equal(child, Assert.Single(collection));
            Assert.Equal(1, container.GetColumn(child));
            Assert.Equal(-1, container.GetRow(child));
        }

        [WinFormsFact]
        public void TableLayoutControlCollection_Add_NullControl_ThrowsArgumentNullException()
        {
            using var container = new TableLayoutPanel();
            var collection = new TableLayoutControlCollection(container);
            Assert.Throws<ArgumentNullException>("control", () => collection.Add(null, 1, 2));
            Assert.Empty(collection);
        }
    }
}
