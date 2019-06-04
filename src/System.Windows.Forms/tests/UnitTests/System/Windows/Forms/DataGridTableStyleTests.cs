// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridTableStyleTests
    {
        [Fact]
        public void DataGridTableStyle_Ctor_Default()
        {
            var style = new SubDataGridTableStyle();
            Assert.True(style.AllowSorting);
            Assert.Equal(SystemColors.Window, style.AlternatingBackColor);
            Assert.Equal(SystemColors.Window, style.BackColor);
            Assert.True(style.CanRaiseEvents);
            Assert.True(style.ColumnHeadersVisible);
            Assert.Null(style.Container);
            Assert.Null(style.DataGrid);
            Assert.False(style.DesignMode);
            Assert.NotNull(style.Events);
            Assert.Same(style.Events, style.Events);
            Assert.Equal(SystemColors.WindowText, style.ForeColor);
            Assert.Empty(style.GridColumnStyles);
            Assert.Same(style.GridColumnStyles, style.GridColumnStyles);
            Assert.Equal(SystemColors.Control, style.GridLineColor);
            Assert.Equal(DataGridLineStyle.Solid, style.GridLineStyle);
            Assert.Equal(SystemColors.Control, style.HeaderBackColor);
            Assert.Same(Control.DefaultFont, style.HeaderFont);
            Assert.Equal(SystemColors.ControlText, style.HeaderForeColor);
            Assert.Equal(SystemColors.HotTrack, style.LinkColor);
            Assert.Equal(SystemColors.HotTrack, style.LinkHoverColor);
            Assert.Empty(style.MappingName);
            Assert.Equal(75, style.PreferredColumnWidth);
            Assert.Equal(Control.DefaultFont.Height + 3, style.PreferredRowHeight);
            Assert.False(style.ReadOnly);
            Assert.True(style.RowHeadersVisible);
            Assert.Equal(35, style.RowHeaderWidth);
            Assert.Equal(SystemColors.ActiveCaption, style.SelectionBackColor);
            Assert.Equal(SystemColors.ActiveCaptionText, style.SelectionForeColor);
            Assert.Null(style.Site);
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
    }
}
