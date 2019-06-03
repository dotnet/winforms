// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TableLayoutStyleCollectionTests
    {
        [Fact]
        public void TableLayoutStyleCollection_Properties_GetDefault_ReturnsExpected()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            Assert.Equal(0, collection.Count);
            Assert.False(collection.IsFixedSize);
            Assert.False(collection.IsReadOnly);
            Assert.False(collection.IsSynchronized);
            Assert.NotNull(collection.SyncRoot);
        }

        [Fact]
        public void TableLayoutStyleCollection_Add_TableLayoutStyle_Success()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;

            var style = new RowStyle();
            collection.Add(style);
            Assert.Equal(style, Assert.Single(collection));
        }

        [Fact]
        public void TableLayoutStyleCollection_Add_Object_Success()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;

            var style = new RowStyle();
            collection.Add(style);
            Assert.Equal(style, Assert.Single(collection));
        }

        [Fact]
        public void TableLayoutStyleCollection_Add_Null_ThrowsArgumentNullException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            Assert.Throws<ArgumentNullException>("style", () => collection.Add(null));
            Assert.Throws<ArgumentNullException>("style", () => ((IList)collection).Add(null));
        }

        [Fact]
        public void TableLayoutStyleCollection_Add_ObjectNotTableLayoutStyle_ThrowsInvalidCastException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            Assert.Throws<InvalidCastException>(() => collection.Add(new object()));
        }

        [Fact]
        public void TableLayoutStyleCollection_Add_StyleAlreadyAdded_ThrowsArgumentException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            Assert.Throws<ArgumentException>("style", () => collection.Add(style));
            Assert.Throws<ArgumentException>("style", () => ((IList)collection).Add(style));
        }

        [Fact]
        public void TableLayoutStyleCollection_Insert_Object_Success()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;

            var style = new RowStyle();
            collection.Insert(0, style);
            Assert.Equal(style, Assert.Single(collection));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void TableLayoutStyleCollection_Insert_Null_ThrowsArgumentNullException(int index)
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            Assert.Throws<ArgumentNullException>("style", () => ((IList)collection).Insert(index, null));
        }

        [Fact]
        public void TableLayoutStyleCollection_Insert_ObjectNotTableLayoutStyle_ThrowsInvalidCastException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            Assert.Throws<InvalidCastException>(() => collection.Insert(0, new object()));
        }

        [Fact]
        public void TableLayoutStyleCollection_Insert_StyleAlreadyAdded_ThrowsArgumentException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            Assert.Throws<ArgumentException>("style", () => ((IList)collection).Insert(0, style));
        }

        [Fact]
        public void TableLayoutStyleCollection_Item_SetTableLayoutStyle_GetReturnsExpected()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            collection.Add(new RowStyle());

            var style = new RowStyle();
            collection[0] = style;
            Assert.Single(collection);
            Assert.Equal(style, collection[0]);
        }

        [Fact]
        public void TableLayoutStyleCollection_Item_SetObject_GetReturnsExpected()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            collection.Add(new RowStyle());

            var style = new RowStyle();
            collection[0] = style;
            Assert.Single(collection);
            Assert.Equal(style, collection[0]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void TableLayoutStyleCollection_Item_SetNull_ThrowsArgumentNullException(int index)
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            collection.Add(new RowStyle());
            Assert.Throws<ArgumentNullException>("value", () => collection[index] = null);
            Assert.Throws<ArgumentNullException>("value", () => ((IList)collection)[index] = null);
        }

        [Fact]
        public void TableLayoutStyleCollection_Item_ObjectNotTableLayoutStyle_ThrowsInvalidCastException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            collection.Add(new RowStyle());
            Assert.Throws<InvalidCastException>(() => collection[0] = new object());
        }

        [Fact]
        public void TableLayoutStyleCollection_Item_StyleAlreadyAdded_ThrowsArgumentException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            Assert.Throws<ArgumentException>("style", () => collection[0] = style);
            Assert.Throws<ArgumentException>("style", () => ((IList)collection)[0] = style);
        }

        [Fact]
        public void TableLayoutStyleCollection_Remove_Object_Success()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            collection.Remove(style);
            Assert.Empty(collection);

            collection.Add(style);
            Assert.Equal(style, Assert.Single(collection));
        }

        [Fact]
        public void TableLayoutStyleCollection_Remove_Null_Nop()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            collection.Remove(null);
            Assert.Same(style, Assert.Single(collection));
        }

        [Fact]
        public void TableLayoutStyleCollection_Remove_ObjectNotTableLayoutStyle_ThrowsInvalidCastException()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            Assert.Throws<InvalidCastException>(() => collection.Remove(new object()));
        }

        [Fact]
        public void TableLayoutStyleCollection_RemoveAt_Invoke_Success()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            collection.RemoveAt(0);
            Assert.Empty(collection);

            collection.Add(style);
            Assert.Equal(style, Assert.Single(collection));
        }

        [Fact]
        public void TableLayoutStyleCollection_Clear_Invoke_Success()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            collection.Clear();
            Assert.Empty(collection);

            collection.Clear();
            Assert.Empty(collection);

            collection.Add(style);
            Assert.Equal(style, Assert.Single(collection));
        }

        [Fact]
        public void TableLayoutStyleCollection_Contains_Object_ReturnsExpected()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            Assert.True(collection.Contains(style));
            Assert.False(collection.Contains(new ColumnStyle()));
            Assert.False(collection.Contains(new RowStyle()));
            Assert.False(collection.Contains(null));
        }

        [Fact]
        public void TableLayoutStyleCollection_IndexOf_Object_ReturnsExpected()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            Assert.Equal(0, collection.IndexOf(style));
            Assert.Equal(-1, collection.IndexOf(new ColumnStyle()));
            Assert.Equal(-1, collection.IndexOf(new RowStyle()));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [Fact]
        public void TableLayoutStyleCollection_CopyTo_Invoke_Success()
        {
            var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);

            var array = new object[] { 1, 2, 3 };
            collection.CopyTo(array, 1);
            Assert.Equal(new object[] { 1, style, 3 }, array);
        }
    }
}
