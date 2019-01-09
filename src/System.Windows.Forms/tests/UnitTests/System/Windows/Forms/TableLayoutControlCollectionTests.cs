// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TableLayoutControlCollectionTests
    {
        public static IEnumerable<object[]> Ctor_TableLayoutPanel_TestData()
        {
            yield return new object[] { new TableLayoutPanel() };
            yield return new object[] { null };
        }

        [Theory]
        [MemberData(nameof(Ctor_TableLayoutPanel_TestData))]
        public void TableLayoutControlCollection_Ctor_TableLayoutPanel(TableLayoutPanel container)
        {
            var collection = new TableLayoutControlCollection(container);
            Assert.Equal(container, collection.Container);
            Assert.Equal(container, collection.Owner);
            Assert.Empty(collection);
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
        public void TableLayoutControlCollection_Add_NegativeColumn_ThrowsArgumentException()
        {
            var container = new TableLayoutPanel();
            var collection = new TableLayoutControlCollection(container);
            var control = new Control();
            Assert.Throws<ArgumentException>(null, () => collection.Add(control, -2, 2));
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
        public void TableLayoutControlCollection_Add_NullControl_ThrowsNullReferenceException()
        {
            var container = new TableLayoutPanel();
            var collection = new TableLayoutControlCollection(container);
            Assert.Throws<NullReferenceException>(() => collection.Add(null, 1, 2));
            Assert.Empty(collection);
        }

        [Fact]
        public void TableLayoutControlCollection_Add_NullContainer_ThrowsNullReferenceExceptio()
        {
            var collection = new TableLayoutControlCollection(null);
            var control = new Control();
            Assert.Throws<NullReferenceException>(() => collection.Add(control, -2, 2));
        }
    }
}
