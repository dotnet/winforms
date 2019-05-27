// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridTests
    {
        [Fact]
        public void DataGrid_Ctor_Default()
        {
            var dataGrid = new SubDataGrid();
            Assert.True(dataGrid.AllowNavigation);
            Assert.True(dataGrid.AllowSorting);
            Assert.Equal(SystemColors.Window, dataGrid.AlternatingBackColor);
            Assert.Equal(SystemColors.Window, dataGrid.BackColor);
            Assert.Equal(SystemColors.AppWorkspace, dataGrid.BackgroundColor);
            Assert.Null(dataGrid.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, dataGrid.BackgroundImageLayout);
            Assert.Equal(BorderStyle.Fixed3D, dataGrid.BorderStyle);
            Assert.Equal(new Rectangle(0, 0, 130, 80), dataGrid.Bounds);
            Assert.Equal(80, dataGrid.Bottom);
            Assert.Equal(SystemColors.ActiveCaption, dataGrid.CaptionBackColor);
            Assert.Equal(Control.DefaultFont.Name, dataGrid.CaptionFont.Name);
            Assert.Equal(FontStyle.Bold, dataGrid.CaptionFont.Style);
            Assert.Equal(SystemColors.ActiveCaptionText, dataGrid.CaptionForeColor);
            Assert.Empty(dataGrid.CaptionText);
            Assert.True(dataGrid.CaptionVisible);
            Assert.Equal(new Rectangle(0, 0, 130, 80), dataGrid.ClientRectangle);
            Assert.Equal(new Size(130, 80), dataGrid.ClientSize);
            Assert.True(dataGrid.ColumnHeadersVisible);
            Assert.Equal(0, dataGrid.CurrentCell.RowNumber);
            Assert.Equal(0, dataGrid.CurrentCell.ColumnNumber);
            Assert.Equal(-1, dataGrid.CurrentRowIndex);
            Assert.Same(Cursors.Default, dataGrid.Cursor);
            Assert.Empty(dataGrid.DataMember);
            Assert.Null(dataGrid.DataSource);
            Assert.Equal(new Rectangle(0, 0, 130, 80), dataGrid.DisplayRectangle);
            Assert.Equal(Size.Empty, dataGrid.DefaultMaximumSize);
            Assert.Equal(Size.Empty, dataGrid.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, dataGrid.DefaultPadding);
            Assert.Equal(new Size(130, 80), dataGrid.DefaultSize);
            Assert.Equal(0, dataGrid.FirstVisibleColumn);
            Assert.False(dataGrid.FlatMode);
            Assert.Equal(SystemColors.WindowText, dataGrid.ForeColor);
            Assert.Equal(SystemColors.Control, dataGrid.GridLineColor);
            Assert.Equal(DataGridLineStyle.Solid, dataGrid.GridLineStyle);
            Assert.Equal(SystemColors.Control, dataGrid.HeaderBackColor);
            Assert.Same(Control.DefaultFont, dataGrid.HeaderFont);
            Assert.Equal(SystemColors.ControlText, dataGrid.HeaderForeColor);
            Assert.Equal(SystemColors.ControlText, dataGrid.HeaderForeColor);
            Assert.Equal(80, dataGrid.Height);
            Assert.NotNull(dataGrid.HorizScrollBar);
            Assert.Same(dataGrid.HorizScrollBar, dataGrid.HorizScrollBar);
            Assert.Equal(0, dataGrid.Left);
            Assert.Equal(SystemColors.HotTrack, dataGrid.LinkColor);
            Assert.Equal(SystemColors.HotTrack, dataGrid.LinkHoverColor);
            Assert.Null(dataGrid.ListManager);
            Assert.Equal(Point.Empty, dataGrid.Location);
            Assert.Equal(new Padding(3, 3, 3, 3), dataGrid.Margin);
            Assert.Equal(Size.Empty, dataGrid.MaximumSize);
            Assert.Equal(Size.Empty, dataGrid.MinimumSize);
            Assert.Equal(Padding.Empty, dataGrid.Padding);
            Assert.Equal(SystemColors.Control, dataGrid.ParentRowsBackColor);
            Assert.Equal(SystemColors.WindowText, dataGrid.ParentRowsForeColor);
            Assert.Equal(DataGridParentRowsLabelStyle.Both, dataGrid.ParentRowsLabelStyle);
            Assert.True(dataGrid.ParentRowsVisible);
            Assert.Equal(75, dataGrid.PreferredColumnWidth);
            Assert.Equal(Control.DefaultFont.Height + 3, dataGrid.PreferredRowHeight);
            Assert.Equal(new Size(130, 80), dataGrid.PreferredSize);
            Assert.False(dataGrid.ReadOnly);
            Assert.True(dataGrid.RowHeadersVisible);
            Assert.Equal(35, dataGrid.RowHeaderWidth);
            Assert.Equal(SystemColors.ActiveCaption, dataGrid.SelectionBackColor);
            Assert.Equal(SystemColors.ActiveCaptionText, dataGrid.SelectionForeColor);
            Assert.Null(dataGrid.Site);
            Assert.Equal(new Size(130, 80), dataGrid.Size);
            Assert.Empty(dataGrid.TableStyles);
            Assert.Same(dataGrid.TableStyles, dataGrid.TableStyles);
            Assert.Empty(dataGrid.Text);
            Assert.Equal(0, dataGrid.Top);
            Assert.NotNull(dataGrid.VertScrollBar);
            Assert.Same(dataGrid.VertScrollBar, dataGrid.VertScrollBar);
            Assert.Equal(0, dataGrid.VisibleColumnCount);
            Assert.Equal(0, dataGrid.VisibleRowCount);
            Assert.Equal(130, dataGrid.Width);
        }

        [Fact]
        public void DataGrid_ColumnStartedEditing_ValidControl_Success()
        {
            var dataGrid = new DataGrid();
            var control = new Control();
            dataGrid.ColumnStartedEditing(control);
        }

        [Fact]
        public void DataGrid_ColumnStartedEditing_NullControl_Nop()
        {
            var dataGrid = new DataGrid();
            dataGrid.ColumnStartedEditing(null);
        }

        private class SubDataGridTableStyle : DataGridTableStyle
        {
            public SubDataGridTableStyle() : base()
            {
            }

            public SubDataGridTableStyle(bool isDefaultTableStyle) : base(isDefaultTableStyle)
            {
            }

            public SubDataGridTableStyle(CurrencyManager listManager) : base(listManager)
            {
            }

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new bool DesignMode => base.DesignMode;

            public new EventHandlerList Events => base.Events;
        }

        private class SubDataGrid : DataGrid
        {
            public new Size DefaultMaximumSize => base.DefaultMaximumSize;

            public new Size DefaultMinimumSize => base.DefaultMinimumSize;

            public new Padding DefaultPadding => base.DefaultPadding;

            public new Size DefaultSize => base.DefaultSize;

            public new ScrollBar HorizScrollBar => base.HorizScrollBar;

            public new ScrollBar VertScrollBar => base.VertScrollBar;
        }
    }
}
