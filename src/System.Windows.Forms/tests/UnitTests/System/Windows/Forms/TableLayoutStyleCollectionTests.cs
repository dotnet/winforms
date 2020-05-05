// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TableLayoutStyleCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TableLayoutStyleCollection_Properties_GetDefault_ReturnsExpected()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            Assert.Equal(0, collection.Count);
            Assert.False(collection.IsFixedSize);
            Assert.False(collection.IsReadOnly);
            Assert.False(collection.IsSynchronized);
            Assert.NotNull(collection.SyncRoot);
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Add_TableLayoutStyle_Success()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;

            var style = new RowStyle();
            collection.Add(style);
            Assert.Equal(style, Assert.Single(collection));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Add_Object_Success()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;

            var style = new RowStyle();
            collection.Add(style);
            Assert.Equal(style, Assert.Single(collection));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Add_Null_ThrowsArgumentNullException()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            Assert.Throws<ArgumentNullException>("style", () => collection.Add(null));
            Assert.Throws<ArgumentNullException>("style", () => ((IList)collection).Add(null));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Add_ObjectNotTableLayoutStyle_ThrowsInvalidCastException()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            Assert.Throws<InvalidCastException>(() => collection.Add(new object()));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Add_StyleAlreadyAdded_ThrowsArgumentException()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            Assert.Throws<ArgumentException>("style", () => collection.Add(style));
            Assert.Throws<ArgumentException>("style", () => ((IList)collection).Add(style));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Insert_Object_Success()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;

            var style = new RowStyle();
            collection.Insert(0, style);
            Assert.Equal(style, Assert.Single(collection));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        public void TableLayoutStyleCollection_Insert_Null_ThrowsArgumentNullException(int index)
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            Assert.Throws<ArgumentNullException>("style", () => ((IList)collection).Insert(index, null));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Insert_ObjectNotTableLayoutStyle_ThrowsInvalidCastException()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            Assert.Throws<InvalidCastException>(() => collection.Insert(0, new object()));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Insert_StyleAlreadyAdded_ThrowsArgumentException()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            Assert.Throws<ArgumentException>("style", () => ((IList)collection).Insert(0, style));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Item_SetTableLayoutStyle_GetReturnsExpected()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            collection.Add(new RowStyle());

            var style = new RowStyle();
            collection[0] = style;
            Assert.Single(collection);
            Assert.Equal(style, collection[0]);
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Item_SetObject_GetReturnsExpected()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            collection.Add(new RowStyle());

            var style = new RowStyle();
            collection[0] = style;
            Assert.Single(collection);
            Assert.Equal(style, collection[0]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        public void TableLayoutStyleCollection_Item_SetNull_ThrowsArgumentNullException(int index)
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            collection.Add(new RowStyle());
            Assert.Throws<ArgumentNullException>("value", () => collection[index] = null);
            Assert.Throws<ArgumentNullException>("value", () => ((IList)collection)[index] = null);
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Item_ObjectNotTableLayoutStyle_ThrowsInvalidCastException()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            collection.Add(new RowStyle());
            Assert.Throws<InvalidCastException>(() => collection[0] = new object());
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Item_StyleAlreadyAdded_ThrowsArgumentException()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            Assert.Throws<ArgumentException>("style", () => collection[0] = style);
            Assert.Throws<ArgumentException>("style", () => ((IList)collection)[0] = style);
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Remove_Object_Success()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            collection.Remove(style);
            Assert.Empty(collection);

            collection.Add(style);
            Assert.Equal(style, Assert.Single(collection));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Remove_Null_Nop()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            collection.Remove(null);
            Assert.Same(style, Assert.Single(collection));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Remove_ObjectNotTableLayoutStyle_ThrowsInvalidCastException()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            Assert.Throws<InvalidCastException>(() => collection.Remove(new object()));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_RemoveAt_Invoke_Success()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            TableLayoutStyleCollection collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            collection.RemoveAt(0);
            Assert.Empty(collection);

            collection.Add(style);
            Assert.Equal(style, Assert.Single(collection));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_Clear_Invoke_Success()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
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

        [WinFormsFact]
        public void TableLayoutStyleCollection_Contains_Object_ReturnsExpected()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            Assert.True(collection.Contains(style));
            Assert.False(collection.Contains(new ColumnStyle()));
            Assert.False(collection.Contains(new RowStyle()));
            Assert.False(collection.Contains(null));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_IndexOf_Object_ReturnsExpected()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
            TableLayoutSettings settings = Assert.IsType<TableLayoutSettings>(toolStrip.LayoutSettings);
            IList collection = settings.RowStyles;
            var style = new RowStyle();
            collection.Add(style);
            Assert.Equal(0, collection.IndexOf(style));
            Assert.Equal(-1, collection.IndexOf(new ColumnStyle()));
            Assert.Equal(-1, collection.IndexOf(new RowStyle()));
            Assert.Equal(-1, collection.IndexOf(null));
        }

        [WinFormsFact]
        public void TableLayoutStyleCollection_CopyTo_Invoke_Success()
        {
            using var toolStrip = new ToolStrip { LayoutStyle = ToolStripLayoutStyle.Table };
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
