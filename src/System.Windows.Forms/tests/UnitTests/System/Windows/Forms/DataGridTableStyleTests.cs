// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_Ctor_Bool(bool isDefaultTableStyle)
        {
            var style = new SubDataGridTableStyle(isDefaultTableStyle);
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

        [Fact]
        public void DataGridTableStyle_DefaultTableStyle_Get_ReturnsExpected()
        {
            DataGridTableStyle style = DataGridTableStyle.DefaultTableStyle;
            Assert.True(style.AllowSorting);
            Assert.Equal(SystemColors.Window, style.AlternatingBackColor);
            Assert.Equal(SystemColors.Window, style.BackColor);
            Assert.True(style.ColumnHeadersVisible);
            Assert.Null(style.Container);
            Assert.Null(style.DataGrid);
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
            Assert.Same(style, DataGridTableStyle.DefaultTableStyle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_AllowSorting_Set_GetReturnsExpected(bool value)
        {
            var style = new DataGridTableStyle
            {
                AllowSorting = value
            };
            Assert.Equal(value, style.AllowSorting);
            
            // Set same.
            style.AllowSorting = value;
            Assert.Equal(value, style.AllowSorting);
            
            // Set different
            style.AllowSorting = !value;
            Assert.Equal(!value, style.AllowSorting);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_AllowSorting_SetWithDataGrid_DoesNotCallInvalidate(bool value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.AllowSorting = value;
            Assert.Equal(value, style.AllowSorting);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set same.
            style.AllowSorting = value;
            Assert.Equal(value, style.AllowSorting);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set different
            style.AllowSorting = !value;
            Assert.Equal(!value, style.AllowSorting);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_AllowSorting_SetWithHandler_CallsAllowSortingChanged()
        {
            var style = new DataGridTableStyle
            {
                AllowSorting = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.AllowSortingChanged += handler;
        
            // Set different.
            style.AllowSorting = false;
            Assert.False(style.AllowSorting);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.AllowSorting = false;
            Assert.False(style.AllowSorting);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.AllowSorting = true;
            Assert.True(style.AllowSorting);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.AllowSortingChanged -= handler;
            style.AllowSorting = false;
            Assert.False(style.AllowSorting);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_AllowSorting_SetDefaultTableStyle_ThrowsArgumentException(bool value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true);
            Assert.Throws<ArgumentException>(null, () => style.AllowSorting = value);
            Assert.True(style.AllowSorting);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void DataGridTableStyle_AlternatingBackColor_Set_GetReturnsExpected(Color value)
        {
            var style = new DataGridTableStyle
            {
                AlternatingBackColor = value
            };
            Assert.Equal(value, style.AlternatingBackColor);
            
            // Set same.
            style.AlternatingBackColor = value;
            Assert.Equal(value, style.AlternatingBackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void DataGridTableStyle_AlternatingBackColor_SetWithDataGrid_CallsInvalidate(Color value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.AlternatingBackColor = value;
            Assert.Equal(value, style.AlternatingBackColor);
            Assert.Equal(1, invalidatedCallCount);
            
            // Set same.
            style.AlternatingBackColor = value;
            Assert.Equal(value, style.AlternatingBackColor);
            Assert.Equal(1, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_AlternatingBackColor_SetWithHandler_CallsAlternatingBackColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.AlternatingBackColorChanged += handler;
        
            // Set different.
            style.AlternatingBackColor = Color.Red;
            Assert.Equal(Color.Red, style.AlternatingBackColor);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.AlternatingBackColor = Color.Red;
            Assert.Equal(Color.Red, style.AlternatingBackColor);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.AlternatingBackColor = Color.Blue;
            Assert.Equal(Color.Blue, style.AlternatingBackColor);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.AlternatingBackColorChanged -= handler;
            style.AlternatingBackColor = Color.Red;
            Assert.Equal(Color.Red, style.AlternatingBackColor);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> InvalidColor_TestData()
        {
            yield return new object[] { Color.Empty };
            yield return new object[] { Color.FromArgb(244, 1, 2, 3) };
        }

        [Theory]
        [MemberData(nameof(InvalidColor_TestData))]
        public void DataGridTableStyle_AlternatingBackColor_SetInvalid_ThrowsArgumentException(Color value)
        {
            var style = new DataGridTableStyle();
            Assert.Throws<ArgumentException>("value", () => style.AlternatingBackColor = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void DataGridTableStyle_AlternatingBackColor_SetDefaultTableStyle_ThrowsArgumentException(Color value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true);
            Assert.Throws<ArgumentException>(null, () => style.AlternatingBackColor = value);
            Assert.Equal(SystemColors.Window, style.AlternatingBackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void DataGridTableStyle_BackColor_Set_GetReturnsExpected(Color value)
        {
            var style = new DataGridTableStyle
            {
                BackColor = value
            };
            Assert.Equal(value, style.BackColor);
            
            // Set same.
            style.BackColor = value;
            Assert.Equal(value, style.BackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void DataGridTableStyle_BackColor_SetWithDataGrid_CallsInvalidate(Color value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.BackColor = value;
            Assert.Equal(value, style.BackColor);
            Assert.Equal(1, invalidatedCallCount);
            
            // Set same.
            style.BackColor = value;
            Assert.Equal(value, style.BackColor);
            Assert.Equal(1, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.BackColorChanged += handler;
        
            // Set different.
            style.BackColor = Color.Red;
            Assert.Equal(Color.Red, style.BackColor);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.BackColor = Color.Red;
            Assert.Equal(Color.Red, style.BackColor);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.BackColor = Color.Blue;
            Assert.Equal(Color.Blue, style.BackColor);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.BackColorChanged -= handler;
            style.BackColor = Color.Red;
            Assert.Equal(Color.Red, style.BackColor);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [MemberData(nameof(InvalidColor_TestData))]
        public void DataGridTableStyle_BackColor_SetInvalid_ThrowsArgumentException(Color value)
        {
            var style = new DataGridTableStyle();
            Assert.Throws<ArgumentException>("value", () => style.BackColor = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void DataGridTableStyle_BackColor_SetDefaultTableStyle_ThrowsArgumentException(Color value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true);
            Assert.Throws<ArgumentException>(null, () => style.BackColor = value);
            Assert.Equal(SystemColors.Window, style.BackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_ColumnHeadersVisible_Set_GetReturnsExpected(bool value)
        {
            var style = new DataGridTableStyle
            {
                ColumnHeadersVisible = value
            };
            Assert.Equal(value, style.ColumnHeadersVisible);
            
            // Set same.
            style.ColumnHeadersVisible = value;
            Assert.Equal(value, style.ColumnHeadersVisible);
            
            // Set different
            style.ColumnHeadersVisible = !value;
            Assert.Equal(!value, style.ColumnHeadersVisible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_ColumnHeadersVisible_SetDefaultTableStyle_GetReturnsExpected(bool value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true)
            {
                ColumnHeadersVisible = value
            };
            Assert.Equal(value, style.ColumnHeadersVisible);
            
            // Set same.
            style.ColumnHeadersVisible = value;
            Assert.Equal(value, style.ColumnHeadersVisible);
            
            // Set different
            style.ColumnHeadersVisible = !value;
            Assert.Equal(!value, style.ColumnHeadersVisible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_ColumnHeadersVisible_SetWithDataGrid_DoesNotCallInvalidate(bool value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.ColumnHeadersVisible = value;
            Assert.Equal(value, style.ColumnHeadersVisible);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set same.
            style.ColumnHeadersVisible = value;
            Assert.Equal(value, style.ColumnHeadersVisible);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set different
            style.ColumnHeadersVisible = !value;
            Assert.Equal(!value, style.ColumnHeadersVisible);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_ColumnHeadersVisible_SetWithHandler_CallsColumnHeadersVisibleChanged()
        {
            var style = new DataGridTableStyle
            {
                ColumnHeadersVisible = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.ColumnHeadersVisibleChanged += handler;
        
            // Set different.
            style.ColumnHeadersVisible = false;
            Assert.False(style.ColumnHeadersVisible);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.ColumnHeadersVisible = false;
            Assert.False(style.ColumnHeadersVisible);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.ColumnHeadersVisible = true;
            Assert.True(style.ColumnHeadersVisible);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.ColumnHeadersVisibleChanged -= handler;
            style.ColumnHeadersVisible = false;
            Assert.False(style.ColumnHeadersVisible);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> DataGrid_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DataGrid() };
        }

        [Theory]
        [MemberData(nameof(DataGrid_TestData))]
        public void DataGridTableStyle_DataGrid_Set_GetReturnsExpected(DataGrid value)
        {
            var style = new DataGridTableStyle
            {
                DataGrid = value
            };
            Assert.Same(value, style.DataGrid);
            Assert.Same(value?.Font ?? Control.DefaultFont, style.HeaderFont);
            
            // Set same.
            style.DataGrid = value;
            Assert.Same(value, style.DataGrid);
            Assert.Same(value?.Font ?? Control.DefaultFont, style.HeaderFont);
        }

        [Theory]
        [MemberData(nameof(DataGrid_TestData))]
        public void DataGridTableStyle_DataGrid_SetDefaultTableStyle_GetReturnsExpected(DataGrid value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true)
            {
                DataGrid = value
            };
            Assert.Same(value, style.DataGrid);
            Assert.Same(value?.Font ?? Control.DefaultFont, style.HeaderFont);
            
            // Set same.
            style.DataGrid = value;
            Assert.Same(value, style.DataGrid);
            Assert.Same(value?.Font ?? Control.DefaultFont, style.HeaderFont);
        }

        [Fact]
        public void DataGridTableStyle_DataGrid_SetWithGridColumnStyles_SetsDataGridOnGridColumnStyles()
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle();
            var columnStyle = new SubDataGridColumnStyle();
            int callCount = 0;
            columnStyle.SetDataGridInColumnAction = actualDataGrid =>
            {
                Assert.Same(dataGrid, actualDataGrid);
                callCount++;
            };
            style.GridColumnStyles.Add(columnStyle);

            style.DataGrid = dataGrid;
            Assert.Same(dataGrid, style.DataGrid);
            Assert.Equal(1, callCount);

            // Set same.
            style.DataGrid = dataGrid;
            Assert.Same(dataGrid, style.DataGrid);
            Assert.Equal(2, callCount);

            // Set null.
            style.DataGrid = null;
            Assert.Null(style.DataGrid);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_DataGrid_SetWithGridColumnStylesInitializing_DoesNotSetDataGridOnGridColumnStyles()
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle();
            var columnStyle = new SubDataGridColumnStyle();
            int callCount = 0;
            columnStyle.SetDataGridInColumnAction = actualDataGrid =>
            {
                Assert.Same(dataGrid, actualDataGrid);
                callCount++;
            };
            style.GridColumnStyles.Add(columnStyle);
            dataGrid.BeginInit();

            style.DataGrid = dataGrid;
            Assert.Same(dataGrid, style.DataGrid);
            Assert.Equal(0, callCount);

            // Set same.
            style.DataGrid = dataGrid;
            Assert.Same(dataGrid, style.DataGrid);
            Assert.Equal(0, callCount);

            // Set null.
            style.DataGrid = null;
            Assert.Null(style.DataGrid);
            Assert.Equal(0, callCount);
        }

        public static IEnumerable<object[]> ForeColor_Set_TestData()
        {
            yield return new object[] { Color.FromArgb(254, 1, 2, 3) };
            yield return new object[] { Color.Red };
        }

        [Theory]
        [MemberData(nameof(ForeColor_Set_TestData))]
        public void DataGridTableStyle_ForeColor_Set_GetReturnsExpected(Color value)
        {
            var style = new DataGridTableStyle
            {
                ForeColor = value
            };
            Assert.Equal(value, style.ForeColor);
            
            // Set same.
            style.ForeColor = value;
            Assert.Equal(value, style.ForeColor);
        }

        [Theory]
        [MemberData(nameof(ForeColor_Set_TestData))]
        public void DataGridTableStyle_ForeColor_SetWithDataGrid_CallsInvalidate(Color value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.ForeColor = value;
            Assert.Equal(value, style.ForeColor);
            Assert.Equal(1, invalidatedCallCount);
            
            // Set same.
            style.ForeColor = value;
            Assert.Equal(value, style.ForeColor);
            Assert.Equal(1, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.ForeColorChanged += handler;
        
            // Set different.
            style.ForeColor = Color.Red;
            Assert.Equal(Color.Red, style.ForeColor);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.ForeColor = Color.Red;
            Assert.Equal(Color.Red, style.ForeColor);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.ForeColor = Color.Blue;
            Assert.Equal(Color.Blue, style.ForeColor);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.ForeColorChanged -= handler;
            style.ForeColor = Color.Red;
            Assert.Equal(Color.Red, style.ForeColor);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_ForeColor_SetEmpty_ThrowsArgumentException()
        {
            var style = new DataGridTableStyle();
            Assert.Throws<ArgumentException>("value", () => style.ForeColor = Color.Empty);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void DataGridTableStyle_ForeColor_SetDefaultTableStyle_ThrowsArgumentException(Color value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true);
            Assert.Throws<ArgumentException>(null, () => style.ForeColor = value);
            Assert.Equal(SystemColors.WindowText, style.ForeColor);
        }

        public static IEnumerable<object[]> GridLineColor_Set_TestData()
        {
            yield return new object[] { Color.FromArgb(254, 1, 2, 3) };
            yield return new object[] { Color.Red };
        }

        [Theory]
        [MemberData(nameof(GridLineColor_Set_TestData))]
        public void DataGridTableStyle_GridLineColor_Set_GetReturnsExpected(Color value)
        {
            var style = new DataGridTableStyle
            {
                GridLineColor = value
            };
            Assert.Equal(value, style.GridLineColor);
            
            // Set same.
            style.GridLineColor = value;
            Assert.Equal(value, style.GridLineColor);
        }

        [Theory]
        [MemberData(nameof(GridLineColor_Set_TestData))]
        public void DataGridTableStyle_GridLineColor_SetWithDataGrid_DoesNotCallInvalidate(Color value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.GridLineColor = value;
            Assert.Equal(value, style.GridLineColor);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set same.
            style.GridLineColor = value;
            Assert.Equal(value, style.GridLineColor);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_GridLineColor_SetWithHandler_CallsGridLineColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.GridLineColorChanged += handler;
        
            // Set different.
            style.GridLineColor = Color.Red;
            Assert.Equal(Color.Red, style.GridLineColor);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.GridLineColor = Color.Red;
            Assert.Equal(Color.Red, style.GridLineColor);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.GridLineColor = Color.Blue;
            Assert.Equal(Color.Blue, style.GridLineColor);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.GridLineColorChanged -= handler;
            style.GridLineColor = Color.Red;
            Assert.Equal(Color.Red, style.GridLineColor);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_GridLineColor_SetEmpty_ThrowsArgumentException()
        {
            var style = new DataGridTableStyle();
            Assert.Throws<ArgumentException>("value", () => style.GridLineColor = Color.Empty);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void DataGridTableStyle_GridLineColor_SetDefaultTableStyle_ThrowsArgumentException(Color value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true);
            Assert.Throws<ArgumentException>(null, () => style.GridLineColor = value);
            Assert.Equal(SystemColors.Control, style.GridLineColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridLineStyle))]
        public void DataGridTableStyle_GridLineStyle_Set_GetReturnsExpected(DataGridLineStyle value)
        {
            var style = new DataGridTableStyle
            {
                GridLineStyle = value
            };
            Assert.Equal(value, style.GridLineStyle);
            
            // Set same.
            style.GridLineStyle = value;
            Assert.Equal(value, style.GridLineStyle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridLineStyle))]
        public void DataGridTableStyle_GridLineStyle_SetWithDataGrid_DoesNotCallInvalidate(DataGridLineStyle value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.GridLineStyle = value;
            Assert.Equal(value, style.GridLineStyle);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set same.
            style.GridLineStyle = value;
            Assert.Equal(value, style.GridLineStyle);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DataGridLineStyle))]
        public void DataGridTableStyle_GridLineStyle_SetInvalidValue_ThrowsInvalidEnumArgumentException(DataGridLineStyle value)
        {
            var style = new DataGridTableStyle();
            Assert.Throws<InvalidEnumArgumentException>("value", () => style.GridLineStyle = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridLineStyle))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DataGridLineStyle))]
        public void DataGridTableStyle_GridLineStyle_SetDefaultTableStyle_ThrowsArgumentException(DataGridLineStyle value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true);
            Assert.Throws<ArgumentException>(null, () => style.GridLineStyle = value);
            Assert.Equal(DataGridLineStyle.Solid, style.GridLineStyle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void DataGridTableStyle_HeaderBackColor_Set_GetReturnsExpected(Color value)
        {
            var style = new DataGridTableStyle
            {
                HeaderBackColor = value
            };
            Assert.Equal(value, style.HeaderBackColor);
            
            // Set same.
            style.HeaderBackColor = value;
            Assert.Equal(value, style.HeaderBackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void DataGridTableStyle_HeaderBackColor_SetWithDataGrid_DoesNotCallInvalidate(Color value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.HeaderBackColor = value;
            Assert.Equal(value, style.HeaderBackColor);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set same.
            style.HeaderBackColor = value;
            Assert.Equal(value, style.HeaderBackColor);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_HeaderBackColor_SetWithHandler_CallsHeaderBackColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.HeaderBackColorChanged += handler;
        
            // Set different.
            style.HeaderBackColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderBackColor);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.HeaderBackColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderBackColor);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.HeaderBackColor = Color.Blue;
            Assert.Equal(Color.Blue, style.HeaderBackColor);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.HeaderBackColorChanged -= handler;
            style.HeaderBackColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderBackColor);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [MemberData(nameof(InvalidColor_TestData))]
        public void DataGridTableStyle_HeaderBackColor_SetInvalid_ThrowsArgumentException(Color value)
        {
            var style = new DataGridTableStyle();
            Assert.Throws<ArgumentException>("value", () => style.HeaderBackColor = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void DataGridTableStyle_HeaderBackColor_SetDefaultTableStyle_ThrowsArgumentException(Color value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true);
            Assert.Throws<ArgumentException>(null, () => style.HeaderBackColor = value);
            Assert.Equal(SystemColors.Control, style.HeaderBackColor);
        }

        [Fact]
        public void DataGridTableStyle_HeaderFont_Set_GetReturnsExpected()
        {
            var font1 = new Font(SystemFonts.MessageBoxFont.FontFamily, 10);
            var font2 = new Font(SystemFonts.MessageBoxFont.FontFamily, 11);
            var style = new DataGridTableStyle
            {
                HeaderFont = font1
            };
            Assert.Same(font1, style.HeaderFont);

            // Set same.
            style.HeaderFont = font1;
            Assert.Same(font1, style.HeaderFont);

            // Set null.
            style.HeaderFont = null;
            Assert.Same(Control.DefaultFont, style.HeaderFont);

            // Set null again.
            style.HeaderFont = null;
            Assert.Same(Control.DefaultFont, style.HeaderFont);

            // Set different.
            style.HeaderFont = font1;
            Assert.Same(font1, style.HeaderFont);

            // Set different.
            style.HeaderFont = font2;
            Assert.Same(font2, style.HeaderFont);
        }

        [Fact]
        public void DataGridTableStyle_HeaderFont_SetWithDataGrid_DoesNotCallInvalidate()
        {
            var font1 = new Font(SystemFonts.MessageBoxFont.FontFamily, 10);
            var font2 = new Font(SystemFonts.MessageBoxFont.FontFamily, 11);
            var dataGrid = new DataGrid
            {
                Font = SystemFonts.StatusFont
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.HeaderFont = font1;
            Assert.Same(font1, style.HeaderFont);
            Assert.Equal(0, invalidatedCallCount);

            // Set same.
            style.HeaderFont = font1;
            Assert.Same(font1, style.HeaderFont);
            Assert.Equal(0, invalidatedCallCount);

            // Set null.
            style.HeaderFont = null;
            Assert.Same(dataGrid.Font, style.HeaderFont);
            Assert.Equal(0, invalidatedCallCount);

            // Set null again.
            style.HeaderFont = null;
            Assert.Same(dataGrid.Font, style.HeaderFont);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            style.HeaderFont = font1;
            Assert.Same(font1, style.HeaderFont);
            Assert.Equal(0, invalidatedCallCount);

            // Set different.
            style.HeaderFont = font2;
            Assert.Same(font2, style.HeaderFont);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_HeaderFont_SetWithHandler_CallsHeaderFontChanged()
        {
            var font1 = new Font(SystemFonts.MessageBoxFont.FontFamily, 10);
            var font2 = new Font(SystemFonts.MessageBoxFont.FontFamily, 11);
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.HeaderFontChanged += handler;

            // Set different.
            style.HeaderFont = font1;
            Assert.Same(font1, style.HeaderFont);
            Assert.Equal(1, callCount);

            // Set same.
            style.HeaderFont = font1;
            Assert.Same(font1, style.HeaderFont);
            Assert.Equal(1, callCount);

            // Set null.
            style.HeaderFont = null;
            Assert.Same(Control.DefaultFont, style.HeaderFont);
            Assert.Equal(2, callCount);

            // Set null again.
            style.HeaderFont = null;
            Assert.Same(Control.DefaultFont, style.HeaderFont);
            Assert.Equal(2, callCount);

            // Set different.
            style.HeaderFont = font1;
            Assert.Same(font1, style.HeaderFont);
            Assert.Equal(3, callCount);

            // Set different.
            style.HeaderFont = font2;
            Assert.Same(font2, style.HeaderFont);
            Assert.Equal(4, callCount);

            // Remove handler.
            style.HeaderFontChanged -= handler;
            style.HeaderFont = font1;
            Assert.Same(font1, style.HeaderFont);
            Assert.Equal(4, callCount);
        }

        public static IEnumerable<object[]> HeaderForeColor_Set_TestData()
        {
            yield return new object[] { Color.FromArgb(254, 1, 2, 3) };
            yield return new object[] { Color.Red };
        }

        [Theory]
        [MemberData(nameof(HeaderForeColor_Set_TestData))]
        public void DataGridTableStyle_HeaderForeColor_Set_GetReturnsExpected(Color value)
        {
            var style = new DataGridTableStyle
            {
                HeaderForeColor = value
            };
            Assert.Equal(value, style.HeaderForeColor);
            
            // Set same.
            style.HeaderForeColor = value;
            Assert.Equal(value, style.HeaderForeColor);
        }

        [Theory]
        [MemberData(nameof(HeaderForeColor_Set_TestData))]
        public void DataGridTableStyle_HeaderForeColor_SetWithDataGrid_DoesNotCallInvalidate(Color value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.HeaderForeColor = value;
            Assert.Equal(value, style.HeaderForeColor);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set same.
            style.HeaderForeColor = value;
            Assert.Equal(value, style.HeaderForeColor);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_HeaderForeColor_SetWithHandler_CallsHeaderForeColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.HeaderForeColorChanged += handler;
        
            // Set different.
            style.HeaderForeColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderForeColor);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.HeaderForeColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderForeColor);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.HeaderForeColor = Color.Blue;
            Assert.Equal(Color.Blue, style.HeaderForeColor);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.HeaderForeColorChanged -= handler;
            style.HeaderForeColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderForeColor);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_HeaderForeColor_SetEmpty_ThrowsArgumentException()
        {
            var style = new DataGridTableStyle();
            Assert.Throws<ArgumentException>("value", () => style.HeaderForeColor = Color.Empty);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void DataGridTableStyle_HeaderForeColor_SetDefaultTableStyle_ThrowsArgumentException(Color value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true);
            Assert.Throws<ArgumentException>(null, () => style.HeaderForeColor = value);
            Assert.Equal(SystemColors.ControlText, style.HeaderForeColor);
        }

        public static IEnumerable<object[]> LinkColor_Set_TestData()
        {
            yield return new object[] { Color.FromArgb(254, 1, 2, 3) };
            yield return new object[] { Color.Red };
        }

        [Theory]
        [MemberData(nameof(LinkColor_Set_TestData))]
        public void DataGridTableStyle_LinkColor_Set_GetReturnsExpected(Color value)
        {
            var style = new DataGridTableStyle
            {
                LinkColor = value
            };
            Assert.Equal(value, style.LinkColor);
            Assert.Equal(value, style.LinkHoverColor);
            
            // Set same.
            style.LinkColor = value;
            Assert.Equal(value, style.LinkColor);
            Assert.Equal(value, style.LinkHoverColor);
        }

        [Theory]
        [MemberData(nameof(LinkColor_Set_TestData))]
        public void DataGridTableStyle_LinkColor_SetWithDataGrid_DoesNotCallInvalidate(Color value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.LinkColor = value;
            Assert.Equal(value, style.LinkColor);
            Assert.Equal(value, style.LinkHoverColor);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set same.
            style.LinkColor = value;
            Assert.Equal(value, style.LinkColor);
            Assert.Equal(value, style.LinkHoverColor);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_LinkColor_SetWithHandler_CallsLinkColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.LinkColorChanged += handler;

            // Set different.
            style.LinkColor = Color.Red;
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(Color.Red, style.LinkHoverColor);
            Assert.Equal(1, callCount);

            // Set same.
            style.LinkColor = Color.Red;
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(Color.Red, style.LinkHoverColor);
            Assert.Equal(1, callCount);

            // Set different.
            style.LinkColor = Color.Blue;
            Assert.Equal(Color.Blue, style.LinkColor);
            Assert.Equal(Color.Blue, style.LinkHoverColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            style.LinkColorChanged -= handler;
            style.LinkColor = Color.Red;
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(Color.Red, style.LinkHoverColor);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_LinkColor_SetEmpty_ThrowsArgumentException()
        {
            var style = new DataGridTableStyle();
            Assert.Throws<ArgumentException>("value", () => style.LinkColor = Color.Empty);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void DataGridTableStyle_LinkColor_SetDefaultTableStyle_ThrowsArgumentException(Color value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true);
            Assert.Throws<ArgumentException>(null, () => style.LinkColor = value);
            Assert.Equal(SystemColors.HotTrack, style.LinkColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void DataGridTableStyle_LinkHoverColor_Set_Nop(Color value)
        {
            var style = new DataGridTableStyle();
            style.LinkHoverColor = value;
            Assert.Equal(SystemColors.HotTrack, style.LinkColor);
            Assert.Equal(SystemColors.HotTrack, style.LinkHoverColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridTableStyle_MappingName_Set_GetReturnsExpected(string value, string expected)
        {
            var style = new DataGridTableStyle
            {
                MappingName = value
            };
            Assert.Equal(expected, style.MappingName);
            
            // Set same.
            style.MappingName = value;
            Assert.Equal(expected, style.MappingName);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridTableStyle_MappingName_SetWithDataGridView_GetReturnsExpected(string value, string expected)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.MappingName = value;
            Assert.Equal(expected, style.MappingName);
            Assert.Equal(0, invalidatedCallCount);
            
            style.MappingName = value;
            Assert.Equal(expected, style.MappingName);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridTableStyle_MappingName_SetDefaultTableStyle_GetReturnsExpected(string value, string expected)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true)
            {
                MappingName = value
            };
            Assert.Equal(expected, style.MappingName);
            
            style.MappingName = value;
            Assert.Equal(expected, style.MappingName);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("mappingname2", "mappingname2")]
        public void DataGridTableStyle_MappingName_SetNonDuplicate_GetReturnsExpected(string value, string expected)
        {
            var dataGrid = new DataGrid();
            var style1 = new DataGridTableStyle
            {
                MappingName = "MappingName1"
            };
            var style2 = new DataGridTableStyle
            {
                MappingName = "MappingName2"
            };
            dataGrid.TableStyles.Add(style1);
            dataGrid.TableStyles.Add(style2);

            style1.MappingName = value;
            Assert.Equal(expected, style1.MappingName);

            // Set same.
            style1.MappingName = value;
            Assert.Equal(expected, style1.MappingName);
        }

        [Fact]
        public void DataGridTableStyle_MappingName_SetDuplicate_Throws()
        {
            var dataGrid = new DataGrid();
            var style1 = new DataGridTableStyle
            {
                MappingName = "MappingName1"
            };
            var style2 = new DataGridTableStyle
            {
                MappingName = "MappingName2"
            };
            dataGrid.TableStyles.Add(style1);
            dataGrid.TableStyles.Add(style2);

            Assert.Throws<ArgumentException>("table", () => style1.MappingName = "MappingName2");
            Assert.Equal("MappingName1", style1.MappingName);
        }

        [Fact]
        public void DataGridTableStyle_MappingName_SetWithHandler_CallsMappingNameChanged()
        {
            var control = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.MappingNameChanged += handler;

            // Set different.
            control.MappingName = "mappingName";
            Assert.Same("mappingName", control.MappingName);
            Assert.Equal(1, callCount);

            // Set same.
            control.MappingName = "mappingName";
            Assert.Same("mappingName", control.MappingName);
            Assert.Equal(1, callCount);

            // Set empty.
            control.MappingName = string.Empty;
            Assert.Empty(control.MappingName);
            Assert.Equal(2, callCount);

            // Set null.
            control.MappingName = null;
            Assert.Empty(control.MappingName);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.MappingNameChanged -= handler;
            control.MappingName = "mappingName";
            Assert.Same("mappingName", control.MappingName);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(75)]
        [InlineData(76)]
        public void DataGridTableStyle_PreferredColumnWidth_Set_GetReturnsExpected(int value)
        {
            var style = new DataGridTableStyle
            {
                PreferredColumnWidth = value
            };
            Assert.Equal(value, style.PreferredColumnWidth);

            // Set same.
            style.PreferredColumnWidth = value;
            Assert.Equal(value, style.PreferredColumnWidth);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(75)]
        [InlineData(76)]
        public void DataGridTableStyle_PreferredColumnWidth_SetWithDataGrid_DoesNotCallInvalidate(int value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.PreferredColumnWidth = value;
            Assert.Equal(value, style.PreferredColumnWidth);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set same.
            style.PreferredColumnWidth = value;
            Assert.Equal(value, style.PreferredColumnWidth);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_PreferredColumnWidth_SetWithHandler_CallsPreferredColumnWidthChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.PreferredColumnWidthChanged += handler;
        
            // Set different.
            style.PreferredColumnWidth = 1;
            Assert.Equal(1, style.PreferredColumnWidth);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.PreferredColumnWidth = 1;
            Assert.Equal(1, style.PreferredColumnWidth);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.PreferredColumnWidth = 2;
            Assert.Equal(2, style.PreferredColumnWidth);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.PreferredColumnWidthChanged -= handler;
            style.PreferredColumnWidth = 1;
            Assert.Equal(1, style.PreferredColumnWidth);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void DataGridTableStyle_PreferredColumnWidth_SetDefaultTableStyle_ThrowsArgumentException(int value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true);
            Assert.Throws<ArgumentException>(null, () => style.PreferredColumnWidth = value);
            Assert.Equal(75, style.PreferredColumnWidth);
        }

        [Fact]
        public void DataGridTableStyle_PreferredColumnWidth_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var style = new DataGridTableStyle();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => style.PreferredColumnWidth = -1);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(75)]
        [InlineData(76)]
        public void DataGridTableStyle_PreferredRowHeight_Set_GetReturnsExpected(int value)
        {
            var style = new DataGridTableStyle
            {
                PreferredRowHeight = value
            };
            Assert.Equal(value, style.PreferredRowHeight);

            // Set same.
            style.PreferredRowHeight = value;
            Assert.Equal(value, style.PreferredRowHeight);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(75)]
        [InlineData(76)]
        public void DataGridTableStyle_PreferredRowHeight_SetWithDataGrid_DoesNotCallInvalidate(int value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.PreferredRowHeight = value;
            Assert.Equal(value, style.PreferredRowHeight);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set same.
            style.PreferredRowHeight = value;
            Assert.Equal(value, style.PreferredRowHeight);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_PreferredRowHeight_SetWithHandler_CallsPreferredRowHeightChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.PreferredRowHeightChanged += handler;
        
            // Set different.
            style.PreferredRowHeight = 1;
            Assert.Equal(1, style.PreferredRowHeight);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.PreferredRowHeight = 1;
            Assert.Equal(1, style.PreferredRowHeight);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.PreferredRowHeight = 2;
            Assert.Equal(2, style.PreferredRowHeight);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.PreferredRowHeightChanged -= handler;
            style.PreferredRowHeight = 1;
            Assert.Equal(1, style.PreferredRowHeight);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void DataGridTableStyle_PreferredRowHeight_SetDefaultTableStyle_ThrowsArgumentException(int value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true);
            Assert.Throws<ArgumentException>(null, () => style.PreferredRowHeight = value);
            Assert.Equal(Control.DefaultFont.Height + 3, style.PreferredRowHeight);
        }

        [Fact]
        public void DataGridTableStyle_PreferredRowHeight_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var style = new DataGridTableStyle();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => style.PreferredRowHeight = -1);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_ReadOnly_Set_GetReturnsExpected(bool value)
        {
            var style = new DataGridTableStyle
            {
                ReadOnly = value
            };
            Assert.Equal(value, style.ReadOnly);
            
            // Set same.
            style.ReadOnly = value;
            Assert.Equal(value, style.ReadOnly);
            
            // Set different
            style.ReadOnly = !value;
            Assert.Equal(!value, style.ReadOnly);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_ReadOnly_SetWithDataGrid_DoesNotCallInvalidate(bool value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.ReadOnly = value;
            Assert.Equal(value, style.ReadOnly);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set same.
            style.ReadOnly = value;
            Assert.Equal(value, style.ReadOnly);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set different
            style.ReadOnly = !value;
            Assert.Equal(!value, style.ReadOnly);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_ReadOnly_SetDefaultTableStyle_GetReturnsExpected(bool value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true)
            {
                ReadOnly = value
            };
            Assert.Equal(value, style.ReadOnly);
            
            // Set same.
            style.ReadOnly = value;
            Assert.Equal(value, style.ReadOnly);
            
            // Set different
            style.ReadOnly = !value;
            Assert.Equal(!value, style.ReadOnly);
        }

        [Fact]
        public void DataGridTableStyle_ReadOnly_SetWithHandler_CallsReadOnlyChanged()
        {
            var style = new DataGridTableStyle
            {
                ReadOnly = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.ReadOnlyChanged += handler;
        
            // Set different.
            style.ReadOnly = false;
            Assert.False(style.ReadOnly);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.ReadOnly = false;
            Assert.False(style.ReadOnly);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.ReadOnly = true;
            Assert.True(style.ReadOnly);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.ReadOnlyChanged -= handler;
            style.ReadOnly = false;
            Assert.False(style.ReadOnly);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_RowHeadersVisible_Set_GetReturnsExpected(bool value)
        {
            var style = new DataGridTableStyle
            {
                RowHeadersVisible = value
            };
            Assert.Equal(value, style.RowHeadersVisible);
            
            // Set same.
            style.RowHeadersVisible = value;
            Assert.Equal(value, style.RowHeadersVisible);
            
            // Set different
            style.RowHeadersVisible = !value;
            Assert.Equal(!value, style.RowHeadersVisible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_RowHeadersVisible_SetWithDataGrid_DoesNotCallInvalidate(bool value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.RowHeadersVisible = value;
            Assert.Equal(value, style.RowHeadersVisible);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set same.
            style.RowHeadersVisible = value;
            Assert.Equal(value, style.RowHeadersVisible);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set different
            style.RowHeadersVisible = !value;
            Assert.Equal(!value, style.RowHeadersVisible);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_RowHeadersVisible_SetDefaultTableStyle_GetReturnsExpected(bool value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true)
            {
                RowHeadersVisible = value
            };
            Assert.Equal(value, style.RowHeadersVisible);
            
            // Set same.
            style.RowHeadersVisible = value;
            Assert.Equal(value, style.RowHeadersVisible);
            
            // Set different
            style.RowHeadersVisible = !value;
            Assert.Equal(!value, style.RowHeadersVisible);
        }

        [Fact]
        public void DataGridTableStyle_RowHeadersVisible_SetWithHandler_CallsRowHeadersVisibleChanged()
        {
            var style = new DataGridTableStyle
            {
                RowHeadersVisible = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.RowHeadersVisibleChanged += handler;
        
            // Set different.
            style.RowHeadersVisible = false;
            Assert.False(style.RowHeadersVisible);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.RowHeadersVisible = false;
            Assert.False(style.RowHeadersVisible);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.RowHeadersVisible = true;
            Assert.True(style.RowHeadersVisible);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.RowHeadersVisibleChanged -= handler;
            style.RowHeadersVisible = false;
            Assert.False(style.RowHeadersVisible);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void DataGridTableStyle_RowHeaderWidth_Set_GetReturnsExpected(int value)
        {
            var style = new DataGridTableStyle
            {
                RowHeaderWidth = value
            };
            Assert.Equal(value, style.RowHeaderWidth);

            // Set same.
            style.RowHeaderWidth = value;
            Assert.Equal(value, style.RowHeaderWidth);
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(35, 35)]
        [InlineData(36, 36)]
        public void DataGridTableStyle_RowHeaderWidth_SetWithDataGrid_DoesNotCallInvalidate(int value, int expected)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.RowHeaderWidth = value;
            Assert.Equal(expected, style.RowHeaderWidth);
            Assert.Equal(0, invalidatedCallCount);
            
            // Set same.
            style.RowHeaderWidth = value;
            Assert.Equal(expected, style.RowHeaderWidth);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_RowHeaderWidth_SetWithHandler_CallsRowHeaderWidthChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.RowHeaderWidthChanged += handler;
        
            // Set different.
            style.RowHeaderWidth = 1;
            Assert.Equal(1, style.RowHeaderWidth);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.RowHeaderWidth = 1;
            Assert.Equal(1, style.RowHeaderWidth);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.RowHeaderWidth = 2;
            Assert.Equal(2, style.RowHeaderWidth);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.RowHeaderWidthChanged -= handler;
            style.RowHeaderWidth = 1;
            Assert.Equal(1, style.RowHeaderWidth);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void DataGridTableStyle_SelectionBackColor_Set_GetReturnsExpected(Color value)
        {
            var style = new DataGridTableStyle
            {
                SelectionBackColor = value
            };
            Assert.Equal(value, style.SelectionBackColor);
            
            // Set same.
            style.SelectionBackColor = value;
            Assert.Equal(value, style.SelectionBackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void DataGridTableStyle_SelectionBackColor_SetWithDataGrid_CallsInvalidate(Color value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.SelectionBackColor = value;
            Assert.Equal(value, style.SelectionBackColor);
            Assert.Equal(1, invalidatedCallCount);
            
            // Set same.
            style.SelectionBackColor = value;
            Assert.Equal(value, style.SelectionBackColor);
            Assert.Equal(1, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_SelectionBackColor_SetWithHandler_CallsSelectionBackColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.SelectionBackColorChanged += handler;
        
            // Set different.
            style.SelectionBackColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionBackColor);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.SelectionBackColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionBackColor);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.SelectionBackColor = Color.Blue;
            Assert.Equal(Color.Blue, style.SelectionBackColor);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.SelectionBackColorChanged -= handler;
            style.SelectionBackColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionBackColor);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [MemberData(nameof(InvalidColor_TestData))]
        public void DataGridTableStyle_SelectionBackColor_SetInvalid_ThrowsArgumentException(Color value)
        {
            var style = new DataGridTableStyle();
            Assert.Throws<ArgumentException>("value", () => style.SelectionBackColor = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void DataGridTableStyle_SelectionBackColor_SetDefaultTableStyle_ThrowsArgumentException(Color value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true);
            Assert.Throws<ArgumentException>(null, () => style.SelectionBackColor = value);
            Assert.Equal(SystemColors.ActiveCaption, style.SelectionBackColor);
        }

        public static IEnumerable<object[]> SelectionForeColor_Set_TestData()
        {
            yield return new object[] { Color.FromArgb(254, 1, 2, 3) };
            yield return new object[] { Color.Red };
        }

        [Theory]
        [MemberData(nameof(SelectionForeColor_Set_TestData))]
        public void DataGridTableStyle_SelectionForeColor_Set_GetReturnsExpected(Color value)
        {
            var style = new DataGridTableStyle
            {
                SelectionForeColor = value
            };
            Assert.Equal(value, style.SelectionForeColor);
            
            // Set same.
            style.SelectionForeColor = value;
            Assert.Equal(value, style.SelectionForeColor);
        }

        [Theory]
        [MemberData(nameof(SelectionForeColor_Set_TestData))]
        public void DataGridTableStyle_SelectionForeColor_SetWithDataGrid_CallsInvalidate(Color value)
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            style.SelectionForeColor = value;
            Assert.Equal(value, style.SelectionForeColor);
            Assert.Equal(1, invalidatedCallCount);
            
            // Set same.
            style.SelectionForeColor = value;
            Assert.Equal(value, style.SelectionForeColor);
            Assert.Equal(1, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_SelectionForeColor_SetWithHandler_CallsSelectionForeColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.SelectionForeColorChanged += handler;
        
            // Set different.
            style.SelectionForeColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionForeColor);
            Assert.Equal(1, callCount);
        
            // Set same.
            style.SelectionForeColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionForeColor);
            Assert.Equal(1, callCount);
        
            // Set different.
            style.SelectionForeColor = Color.Blue;
            Assert.Equal(Color.Blue, style.SelectionForeColor);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            style.SelectionForeColorChanged -= handler;
            style.SelectionForeColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionForeColor);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_SelectionForeColor_SetEmpty_ThrowsArgumentException()
        {
            var style = new DataGridTableStyle();
            Assert.Throws<ArgumentException>("value", () => style.SelectionForeColor = Color.Empty);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void DataGridTableStyle_SelectionForeColor_SetDefaultTableStyle_ThrowsArgumentException(Color value)
        {
            var style = new DataGridTableStyle(isDefaultTableStyle: true);
            Assert.Throws<ArgumentException>(null, () => style.SelectionForeColor = value);
            Assert.Equal(SystemColors.ActiveCaptionText, style.SelectionForeColor);
        }

        public static IEnumerable<object[]> BeginEdit_WithDataSource_TestData()
        {
            yield return new object[] { 0, true, true, 0, 1, 0, 0, 5 };
            yield return new object[] { 1, true, true, 1, 2, 1, 0, 5 };
            yield return new object[] { -1, true, true, 0, 1, 0, 0, 5 };
            yield return new object[] { 2, true, true, 1, 2, 1, 0, 5 };

            yield return new object[] { 0, false, true, 0, 1, 0, 0, 2 };
            yield return new object[] { 1, false, true, 1, 2, 1, 1, 3 };
            yield return new object[] { -1, false, true, 0, 1, 0, 0, 2 };
            yield return new object[] { 2, false, true, 1, 2, 1, 1, 3 };
        }

        [Theory]
        [MemberData(nameof(BeginEdit_WithDataSource_TestData))]
        public void DataGridTableStyle_BeginEdit_InvokeWithDataSource_ReturnsExpected(int rowNumber, bool commitResult1, bool commitResult2, int expectedRowNumber, int expectedEditCallCount, int expectedCommitCallCount, int expectedAbortCallCount, int expectedCommitCallCount2)
        {
            var dataSource = new DataClass[]
            {
                new DataClass { Value1 = "Value1_1", Value2 = "Value2_1" },
                new DataClass { Value1 = "Value1_2", Value2 = "Value2_2" }
            };
            var bindingContext = new BindingContext();
            CurrencyManager currencyManager = (CurrencyManager)bindingContext[dataSource, null];

            var style = new DataGridTableStyle
            {
                MappingName = "DataClass[]"
            };

            var gridColumn1 = new SubDataGridColumnStyle
            {
                MappingName = "Value1"
            };
            int editCallCount1 = 0;
            gridColumn1.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                Assert.Same(currencyManager, source);
                Assert.Equal(expectedRowNumber, rowNum);
                Assert.NotEqual(Rectangle.Empty, bounds);
                Assert.False(readOnly);
                Assert.Null(displayText);
                Assert.True(cellIsVisible);
                editCallCount1++;
            };
            int commitCallCount1 = 0;
            gridColumn1.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount1++;
                return commitResult1;
            };
            int abortCallCount1 = 0;
            gridColumn1.AbortAction = (rowNum) =>
            {
                Assert.Equal(expectedRowNumber, rowNum);
                abortCallCount1++;
            };
            style.GridColumnStyles.Add(gridColumn1);

            var gridColumn2 = new SubDataGridColumnStyle
            {
                MappingName = "Value2"
            };
            int editCallCount2 = 0;
            gridColumn2.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                Assert.Same(currencyManager, source);
                Assert.Equal(expectedRowNumber, rowNum);
                Assert.NotEqual(Rectangle.Empty, bounds);
                Assert.False(readOnly);
                Assert.Null(displayText);
                Assert.True(cellIsVisible);
                editCallCount2++;
            };
            int commitCallCount2 = 0;
            gridColumn2.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount2++;
                return commitResult2;
            };
            int abortCallCount2 = 0;
            gridColumn2.AbortAction = (rowNum) =>
            {
                Assert.Equal(expectedRowNumber, rowNum);
                abortCallCount2++;
            };
            style.GridColumnStyles.Add(gridColumn2);

            var dataGrid = new SubDataGrid
            {
                BindingContext = bindingContext
            };
            dataGrid.TableStyles.Add(style);
            dataGrid.SetDataBinding(dataSource, null);

            // Simulate layout on the DataGrid.
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                dataGrid.OnPaint(new PaintEventArgs(graphics, new Rectangle(0, 0, 1, 2)));
            }
            
            // Edit cell.
            Assert.True(style.BeginEdit(gridColumn1, rowNumber));
            Assert.Equal(new DataGridCell(expectedRowNumber, 0), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(expectedCommitCallCount, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            Assert.Equal(expectedAbortCallCount, abortCallCount1);
            Assert.Equal(0, abortCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));

            // Edit same.
            Assert.True(style.BeginEdit(gridColumn1, rowNumber));
            Assert.Equal(new DataGridCell(expectedRowNumber, 0), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount + 1, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(expectedCommitCallCount + 1, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));

            // Edit another.
            Assert.True(style.BeginEdit(gridColumn2, rowNumber));
            Assert.Equal(new DataGridCell(expectedRowNumber, 1), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount + 1, editCallCount1);
            Assert.Equal(2, editCallCount2);
            Assert.Equal(expectedCommitCallCount2, commitCallCount1);
            Assert.Equal(1, commitCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));

            // Edit invalid.
            Assert.False(style.BeginEdit(new SubDataGridColumnStyle(), rowNumber));
            Assert.Equal(new DataGridCell(expectedRowNumber, 1), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount + 1, editCallCount1);
            Assert.Equal(2, editCallCount2);
            Assert.Equal(expectedCommitCallCount2, commitCallCount1);
            Assert.Equal(1, commitCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));
            
            // Edit null.
            Assert.False(style.BeginEdit(null, rowNumber));
            Assert.Equal(new DataGridCell(expectedRowNumber, 1), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount + 1, editCallCount1);
            Assert.Equal(2, editCallCount2);
            Assert.Equal(expectedCommitCallCount2, commitCallCount1);
            Assert.Equal(1, commitCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(2)]
        public void DataGridTableStyle_BeginEdit_InvokeWithEmptyDataSource_ReturnsExpected(int rowNumber)
        {
            var dataSource = new DataClass[0];
            var bindingContext = new BindingContext();
            CurrencyManager currencyManager = (CurrencyManager)bindingContext[dataSource, null];

            var style = new DataGridTableStyle
            {
                MappingName = "DataClass[]"
            };

            var gridColumn1 = new SubDataGridColumnStyle
            {
                MappingName = "Value1"
            };
            int editCallCount1 = 0;
            gridColumn1.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                editCallCount1++;
            };
            int commitCallCount1 = 0;
            gridColumn1.CommitAction = (dataSource, rowNum) =>
            {
                commitCallCount1++;
                return true;
            };
            style.GridColumnStyles.Add(gridColumn1);

            var gridColumn2 = new SubDataGridColumnStyle
            {
                MappingName = "Value2"
            };
            int editCallCount2 = 0;
            gridColumn2.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                editCallCount2++;
            };
            int commitCallCount2 = 0;
            gridColumn2.CommitAction = (dataSource, rowNum) =>
            {
                commitCallCount2++;
                return true;
            };
            style.GridColumnStyles.Add(gridColumn2);

            var dataGrid = new SubDataGrid
            {
                BindingContext = bindingContext
            };
            dataGrid.TableStyles.Add(style);
            dataGrid.SetDataBinding(dataSource, null);

            // Simulate layout on the DataGrid.
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                dataGrid.OnPaint(new PaintEventArgs(graphics, new Rectangle(0, 0, 1, 2)));
            }
            
            // Edit cell.
            Assert.True(style.BeginEdit(gridColumn1, rowNumber));
            Assert.Equal(new DataGridCell(0, 0), dataGrid.CurrentCell);
            Assert.Equal(0, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(0, commitCallCount1);
            Assert.Equal(0, commitCallCount2);

            // Edit same.
            Assert.True(style.BeginEdit(gridColumn1, rowNumber));
            Assert.Equal(new DataGridCell(0, 0), dataGrid.CurrentCell);
            Assert.Equal(0, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(0, commitCallCount1);
            Assert.Equal(0, commitCallCount2);

            // Edit another.
            Assert.True(style.BeginEdit(gridColumn2, rowNumber));
            Assert.Equal(new DataGridCell(0, 0), dataGrid.CurrentCell);
            Assert.Equal(0, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(0, commitCallCount1);
            Assert.Equal(0, commitCallCount2);

            // Edit invalid.
            Assert.False(style.BeginEdit(new SubDataGridColumnStyle(), rowNumber));
            Assert.Equal(new DataGridCell(0, 0), dataGrid.CurrentCell);
            Assert.Equal(0, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(0, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            
            // Edit null.
            Assert.False(style.BeginEdit(null, rowNumber));
            Assert.Equal(new DataGridCell(0, 0), dataGrid.CurrentCell);
            Assert.Equal(0, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(0, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
        }

        [Theory]
        [InlineData(0, 0, 1, 0)]
        [InlineData(1, 1, 2, 1)]
        [InlineData(-1, 0, 1, 0)]
        [InlineData(2, 1, 2, 1)]
        public void DataGridTableStyle_BeginEdit_InvokeWithDataSource_ResetsSelection(int rowNumber, int expectedRowNumber, int expectedEditCallCount, int expectedCommitCallCount)
        {
            var dataSource = new DataClass[]
            {
                new DataClass { Value1 = "Value1_1", Value2 = "Value2_1" },
                new DataClass { Value1 = "Value1_2", Value2 = "Value2_2" }
            };
            var bindingContext = new BindingContext();
            CurrencyManager currencyManager = (CurrencyManager)bindingContext[dataSource, null];

            var style = new DataGridTableStyle
            {
                MappingName = "DataClass[]"
            };

            var gridColumn1 = new SubDataGridColumnStyle
            {
                MappingName = "Value1"
            };
            int editCallCount1 = 0;
            gridColumn1.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                Assert.Same(currencyManager, source);
                Assert.Equal(expectedRowNumber, rowNum);
                Assert.NotEqual(Rectangle.Empty, bounds);
                Assert.False(readOnly);
                Assert.Null(displayText);
                Assert.True(cellIsVisible);
                editCallCount1++;
            };
            int commitCallCount1 = 0;
            gridColumn1.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount1++;
                return true;
            };
            style.GridColumnStyles.Add(gridColumn1);

            var gridColumn2 = new SubDataGridColumnStyle
            {
                MappingName = "Value2"
            };
            int editCallCount2 = 0;
            gridColumn2.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                Assert.Same(currencyManager, source);
                Assert.Equal(expectedRowNumber, rowNum);
                Assert.NotEqual(Rectangle.Empty, bounds);
                Assert.False(readOnly);
                Assert.Null(displayText);
                Assert.True(cellIsVisible);
                editCallCount2++;
            };
            int commitCallCount2 = 0;
            gridColumn2.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount2++;
                return true;
            };
            style.GridColumnStyles.Add(gridColumn2);

            var dataGrid = new SubDataGrid
            {
                BindingContext = bindingContext
            };
            dataGrid.TableStyles.Add(style);
            dataGrid.SetDataBinding(dataSource, null);

            // Simulate layout on the DataGrid.
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                dataGrid.OnPaint(new PaintEventArgs(graphics, new Rectangle(0, 0, 1, 2)));
            }

            dataGrid.Select(0);
            Assert.True(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));

            // Edit cell.
            Assert.True(style.BeginEdit(gridColumn1, rowNumber));
            Assert.Equal(new DataGridCell(expectedRowNumber, 0), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(expectedCommitCallCount, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));
        }

        [Theory]
        [InlineData(0, 0, 1, 0)]
        [InlineData(1, 1, 2, 1)]
        [InlineData(-1, 0, 1, 0)]
        [InlineData(2, 1, 2, 1)]
        public void DataGridTableStyle_BeginEdit_InvokeWithDataSourceEditing_ReturnsFalse(int rowNumber, int expectedRowNumber, int expectedEditCallCount, int expectedCommitCallCount)
        {
            var dataSource = new DataClass[]
            {
                new DataClass { Value1 = "Value1_1", Value2 = "Value2_1" },
                new DataClass { Value1 = "Value1_2", Value2 = "Value2_2" }
            };
            var bindingContext = new BindingContext();
            CurrencyManager currencyManager = (CurrencyManager)bindingContext[dataSource, null];

            var style = new DataGridTableStyle
            {
                MappingName = "DataClass[]"
            };

            var gridColumn1 = new SubDataGridColumnStyle
            {
                MappingName = "Value1"
            };
            int editCallCount1 = 0;
            gridColumn1.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                Assert.Same(currencyManager, source);
                Assert.Equal(expectedRowNumber, rowNum);
                Assert.NotEqual(Rectangle.Empty, bounds);
                Assert.False(readOnly);
                Assert.Null(displayText);
                Assert.True(cellIsVisible);
                editCallCount1++;
            };
            int commitCallCount1 = 0;
            gridColumn1.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount1++;
                return true;
            };
            style.GridColumnStyles.Add(gridColumn1);

            var gridColumn2 = new SubDataGridColumnStyle
            {
                MappingName = "Value2"
            };
            int editCallCount2 = 0;
            gridColumn2.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                Assert.Same(currencyManager, source);
                Assert.Equal(expectedRowNumber, rowNum);
                Assert.NotEqual(Rectangle.Empty, bounds);
                Assert.False(readOnly);
                Assert.Null(displayText);
                Assert.True(cellIsVisible);
                editCallCount2++;
            };
            int commitCallCount2 = 0;
            gridColumn2.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount2++;
                return true;
            };
            style.GridColumnStyles.Add(gridColumn2);

            var dataGrid = new SubDataGrid
            {
                BindingContext = bindingContext
            };
            dataGrid.TableStyles.Add(style);
            dataGrid.SetDataBinding(dataSource, null);

            // Simulate layout on the DataGrid.
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                dataGrid.OnPaint(new PaintEventArgs(graphics, new Rectangle(0, 0, 1, 2)));
            }

            // Edit cell.
            Assert.True(style.BeginEdit(gridColumn1, rowNumber));
            Assert.Equal(new DataGridCell(expectedRowNumber, 0), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(expectedCommitCallCount, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));

            // Actual edit.
            dataGrid.ColumnStartedEditing(Rectangle.Empty);
            Assert.False(style.BeginEdit(gridColumn1, rowNumber));
            Assert.Equal(new DataGridCell(expectedRowNumber, 0), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(expectedCommitCallCount, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));
        }

        public static IEnumerable<object[]> BeginEdit_WithoutDataSource_TestData()
        {
            yield return new object[] { null, null, -1, null };
            yield return new object[] { null, null, 0, null };
            yield return new object[] { null, null, 1, null };
            yield return new object[] { null, new SubDataGridColumnStyle(), -1, null };
            yield return new object[] { null, new SubDataGridColumnStyle(), 0, null };
            yield return new object[] { null, new SubDataGridColumnStyle(), 1, null };

            yield return new object[] { new DataGrid(), null, -1, new DataGridCell(0, 0) };
            yield return new object[] { new DataGrid(), null, 0, new DataGridCell(0, 0) };
            yield return new object[] { new DataGrid(), null, 1, new DataGridCell(0, 0) };
            yield return new object[] { new DataGrid(), new SubDataGridColumnStyle(), -1, new DataGridCell(0, 0) };
            yield return new object[] { new DataGrid(), new SubDataGridColumnStyle(), 0, new DataGridCell(0, 0) };
            yield return new object[] { new DataGrid(), new SubDataGridColumnStyle(), 1, new DataGridCell(0, 0) };
        }

        [Theory]
        [MemberData(nameof(BeginEdit_WithoutDataSource_TestData))]
        public void DataGridTableStyle_BeginEdit_InvokeWithoutDataSource_ReturnsFalse(DataGrid dataGrid, DataGridColumnStyle gridColumn, int rowNumber, DataGridCell? expectedCurrentCell)
        {
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            Assert.False(style.BeginEdit(gridColumn, rowNumber));
            Assert.Equal(expectedCurrentCell, dataGrid?.CurrentCell);
        }

        [Fact]
        public void DataGridTableStyle_CreateGridColumn_InvokePropertyDescriptorBoolProperty_ReturnsExpected()
        {
            PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(ClassWithProperties))[nameof(ClassWithProperties.BoolProperty)];
            var style = new SubDataGridTableStyle();
            DataGridBoolColumn columnStyle = Assert.IsType<DataGridBoolColumn>(style.CreateGridColumn(prop));
            Assert.Same(prop, columnStyle.PropertyDescriptor);
            Assert.Empty(columnStyle.MappingName);
        }

        [Theory]
        [InlineData(nameof(ClassWithProperties.SByteProperty), "G")]
        [InlineData(nameof(ClassWithProperties.ShortProperty), "G")]
        [InlineData(nameof(ClassWithProperties.IntProperty), "G")]
        [InlineData(nameof(ClassWithProperties.LongProperty), "G")]
        [InlineData(nameof(ClassWithProperties.ByteProperty), "G")]
        [InlineData(nameof(ClassWithProperties.UShortProperty), "G")]
        [InlineData(nameof(ClassWithProperties.UIntProperty), "G")]
        [InlineData(nameof(ClassWithProperties.ULongProperty), "G")]
        [InlineData(nameof(ClassWithProperties.CharProperty), "")]
        [InlineData(nameof(ClassWithProperties.StringProperty), "")]
        [InlineData(nameof(ClassWithProperties.DateTimeProperty), "d")]
        [InlineData(nameof(ClassWithProperties.DecimalProperty), "G")]
        [InlineData(nameof(ClassWithProperties.DoubleProperty), "G")]
        [InlineData(nameof(ClassWithProperties.FloatProperty), "G")]
        [InlineData(nameof(ClassWithProperties.ObjectProperty), "")]
        public void DataGridTableStyle_CreateGridColumn_InvokePropertyDescriptorTextBoxProperty_ReturnsExpected(string propertyName, string expectedFormat)
        {
            PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(ClassWithProperties))[propertyName];
            var style = new SubDataGridTableStyle();
            DataGridTextBoxColumn columnStyle = Assert.IsType<DataGridTextBoxColumn>(style.CreateGridColumn(prop));
            Assert.Same(prop, columnStyle.PropertyDescriptor);
            Assert.Equal(expectedFormat, columnStyle.Format);
            Assert.Empty(columnStyle.MappingName);
        }

        [Theory]
        [InlineData(true, nameof(ClassWithProperties.BoolProperty))]
        [InlineData(false, "")]
        public void DataGridTableStyle_CreateGridColumn_InvokePropertyDescriptorBoolBoolProperty_ReturnsExpected(bool isDefault, string expectedMappingName)
        {
            PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(ClassWithProperties))[nameof(ClassWithProperties.BoolProperty)];
            var style = new SubDataGridTableStyle();
            DataGridBoolColumn columnStyle = Assert.IsType<DataGridBoolColumn>(style.CreateGridColumn(prop, isDefault));
            Assert.Same(prop, columnStyle.PropertyDescriptor);
            Assert.Equal(expectedMappingName, columnStyle.MappingName);
        }

        public static IEnumerable<object[]> CreateGridColumn_TextBoxProperty_TestData()
        {
            yield return new object[] { nameof(ClassWithProperties.SByteProperty), false, "G", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.ShortProperty), false, "G", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.IntProperty), false, "G", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.LongProperty), false, "G", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.ByteProperty), false, "G", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.UShortProperty), false, "G", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.UIntProperty), false, "G", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.ULongProperty), false, "G", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.CharProperty), false, "", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.StringProperty), false, "", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.DateTimeProperty), false, "d", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.DecimalProperty), false, "G", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.DoubleProperty), false, "G", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.FloatProperty), false, "G", string.Empty };
            yield return new object[] { nameof(ClassWithProperties.ObjectProperty), false, "", string.Empty };
            
            yield return new object[] { nameof(ClassWithProperties.SByteProperty), true, "G", nameof(ClassWithProperties.SByteProperty) };
            yield return new object[] { nameof(ClassWithProperties.ShortProperty), true, "G", nameof(ClassWithProperties.ShortProperty) };
            yield return new object[] { nameof(ClassWithProperties.IntProperty), true, "G", nameof(ClassWithProperties.IntProperty) };
            yield return new object[] { nameof(ClassWithProperties.LongProperty), true, "G", nameof(ClassWithProperties.LongProperty) };
            yield return new object[] { nameof(ClassWithProperties.ByteProperty), true, "G", nameof(ClassWithProperties.ByteProperty) };
            yield return new object[] { nameof(ClassWithProperties.UShortProperty), true, "G", nameof(ClassWithProperties.UShortProperty) };
            yield return new object[] { nameof(ClassWithProperties.UIntProperty), true, "G", nameof(ClassWithProperties.UIntProperty) };
            yield return new object[] { nameof(ClassWithProperties.ULongProperty), true, "G", nameof(ClassWithProperties.ULongProperty) };
            yield return new object[] { nameof(ClassWithProperties.CharProperty), true, "", nameof(ClassWithProperties.CharProperty) };
            yield return new object[] { nameof(ClassWithProperties.StringProperty), true, "", nameof(ClassWithProperties.StringProperty) };
            yield return new object[] { nameof(ClassWithProperties.DateTimeProperty), true, "d", nameof(ClassWithProperties.DateTimeProperty) };
            yield return new object[] { nameof(ClassWithProperties.DecimalProperty), true, "G", nameof(ClassWithProperties.DecimalProperty) };
            yield return new object[] { nameof(ClassWithProperties.DoubleProperty), true, "G", nameof(ClassWithProperties.DoubleProperty) };
            yield return new object[] { nameof(ClassWithProperties.FloatProperty), true, "G", nameof(ClassWithProperties.FloatProperty) };
            yield return new object[] { nameof(ClassWithProperties.ObjectProperty), true, "", nameof(ClassWithProperties.ObjectProperty) };
        }

        [Theory]
        [MemberData(nameof(CreateGridColumn_TextBoxProperty_TestData))]
        public void DataGridTableStyle_CreateGridColumn_InvokePropertyDescriptorBoolTextBoxProperty_ReturnsExpected(string propertyName, bool isDefault, string expectedFormat, string expectedMappingName)
        {
            PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(ClassWithProperties))[propertyName];
            var style = new SubDataGridTableStyle();
            DataGridTextBoxColumn columnStyle = Assert.IsType<DataGridTextBoxColumn>(style.CreateGridColumn(prop, isDefault));
            Assert.Same(prop, columnStyle.PropertyDescriptor);
            Assert.Equal(expectedFormat, columnStyle.Format);
            Assert.Equal(expectedMappingName, columnStyle.MappingName);
        }

        [Fact]
        public void DataGridTableStyle_CreateGridColumn_NullProp_ThrowsArgumentNullException()
        {
            var style = new SubDataGridTableStyle();
            Assert.Throws<ArgumentNullException>("prop", () => style.CreateGridColumn(null));
            Assert.Throws<ArgumentNullException>("prop", () => style.CreateGridColumn(null, isDefault: true));
            Assert.Throws<ArgumentNullException>("prop", () => style.CreateGridColumn(null, isDefault: false));
        }

        [Fact]
        public void DataGridTableStyle_Dispose_InvokeWithEmptyGridColumnStyles_Nop()
        {
            var style = new DataGridTableStyle();
            style.Dispose();
            style.Dispose();
        }

        [Fact]
        public void DataGridTableStyle_Dispose_InvokeWithNullGridColumnStyles_Nop()
        {
            var style = new NullGridColumnStylesDataGridTableStyle();
            style.Dispose();
            style.Dispose();
        }

        [Fact]
        public void DataGridTableStyle_Dispose_Invoke_CallsDisposeOnChildren()
        {
            var columnStyle = new SubDataGridColumnStyle();
            int disposeCallCount = 0;
            columnStyle.DisposeAction = (disposing) =>
            {
                Assert.True(disposing);
                disposeCallCount++;
            };
            var style = new DataGridTableStyle();
            style.GridColumnStyles.Add(columnStyle);

            style.Dispose();
            Assert.Equal(1, disposeCallCount);
            Assert.Same(columnStyle, Assert.Single(style.GridColumnStyles));

            // Call again.
            style.Dispose();
            Assert.Equal(2, disposeCallCount);
            Assert.Same(columnStyle, Assert.Single(style.GridColumnStyles));

            columnStyle.DisposeAction = null;
        }

        [Fact]
        public void DataGridTableStyle_Dispose_InvokeWithHandler_CallsDisposed()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.Disposed += handler;

            // Call with handler.
            style.Dispose();
            Assert.Equal(1, callCount);

            // Call again.
            style.Dispose();
            Assert.Equal(2, callCount);

            // Remove handler.
            style.Disposed -= handler;
            style.Dispose();
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_Dispose_InvokeBoolWithEmptyGridColumnStyles_Nop(bool disposing)
        {
            var style = new SubDataGridTableStyle();
            style.Dispose(disposing);
            style.Dispose(disposing);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridTableStyle_Dispose_InvokeBoolWithNullGridColumnStyles_Nop(bool disposing)
        {
            var style = new NullGridColumnStylesDataGridTableStyle();
            style.Dispose(disposing);
            style.Dispose(disposing);
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void DataGridTableStyle_Dispose_InvokeBool_CallsDisposeOnChildren(bool disposing, int expectedCallCount)
        {
            var columnStyle = new SubDataGridColumnStyle();
            int disposeCallCount = 0;
            columnStyle.DisposeAction += (disposing) =>
            {
                Assert.True(disposing);
                disposeCallCount++;
            };
            var style = new SubDataGridTableStyle();
            style.GridColumnStyles.Add(columnStyle);

            style.Dispose(disposing);
            Assert.Equal(expectedCallCount, disposeCallCount);
            Assert.Same(columnStyle, Assert.Single(style.GridColumnStyles));

            // Call again.
            style.Dispose(disposing);
            Assert.Equal(expectedCallCount * 2, disposeCallCount);
            Assert.Same(columnStyle, Assert.Single(style.GridColumnStyles));

            columnStyle.DisposeAction = null;
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void DataGridTableStyle_Dispose_InvokeBoolWithHandler_CallsDisposed(bool disposing, int expectedCallCount)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.Disposed += handler;

            // Call with handler.
            style.Dispose(disposing);
            Assert.Equal(expectedCallCount, callCount);

            // Call again.
            style.Dispose(disposing);
            Assert.Equal(expectedCallCount * 2, callCount);

            // Remove handler.
            style.Disposed -= handler;
            style.Dispose(disposing);
            Assert.Equal(expectedCallCount * 2, callCount);
        }

        public static IEnumerable<object[]> EndEdit_DataSource_TestData()
        {
            foreach (bool commitResult in new bool[] { true, false })
            {
                yield return new object[] { 0, true, 0, 1, 0, 1, 0, commitResult, true };
                yield return new object[] { 1, true, 1, 2, 1, 1, 1, commitResult, true };
                yield return new object[] { -1, true, 0, 1, 0, 1, 0, commitResult, true };
                yield return new object[] { 2, true, 1, 2, 1, 1, 1, commitResult, true };
                yield return new object[] { 0, false, 0, 1, 0, 0, 1, commitResult, commitResult };
                yield return new object[] { 1, false, 1, 2, 1, 0, 2, commitResult, commitResult };
                yield return new object[] { -1, false, 0, 1, 0, 0, 1, commitResult, commitResult };
                yield return new object[] { 2, false, 1, 2, 1, 0, 2, commitResult, commitResult };
            }
        }

        [Theory]
        [MemberData(nameof(EndEdit_DataSource_TestData))]
        public void DataGridTableStyle_EndEdit_InvokeWithDataSourceAbort_ReturnsExpected(int rowNumber, bool shouldAbort, int expectedRowNumber, int expectedEditCallCount, int expectedCommitCallCount, int expectedAbortCallCount, int expectedCommitCallCount2, bool commitResult, bool expected)
        {
            var dataSource = new DataClass[]
            {
                new DataClass { Value1 = "Value1_1", Value2 = "Value2_1" },
                new DataClass { Value1 = "Value1_2", Value2 = "Value2_2" }
            };
            var bindingContext = new BindingContext();
            CurrencyManager currencyManager = (CurrencyManager)bindingContext[dataSource, null];

            var style = new DataGridTableStyle
            {
                MappingName = "DataClass[]"
            };

            var gridColumn1 = new SubDataGridColumnStyle
            {
                MappingName = "Value1"
            };
            int editCallCount1 = 0;
            gridColumn1.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                Assert.Same(currencyManager, source);
                Assert.Equal(expectedRowNumber, rowNum);
                Assert.NotEqual(Rectangle.Empty, bounds);
                Assert.False(readOnly);
                Assert.Null(displayText);
                Assert.True(cellIsVisible);
                editCallCount1++;
            };
            int commitCallCount1 = 0;
            gridColumn1.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount1++;
                return true;
            };
            int abortCallCount1 = 0;
            gridColumn1.AbortAction = (rowNum) =>
            {
                Assert.Equal(expectedRowNumber, rowNum);
                abortCallCount1++;
            };
            style.GridColumnStyles.Add(gridColumn1);

            var gridColumn2 = new SubDataGridColumnStyle
            {
                MappingName = "Value2"
            };
            int editCallCount2 = 0;
            gridColumn2.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                Assert.Same(currencyManager, source);
                Assert.Equal(expectedRowNumber, rowNum);
                Assert.NotEqual(Rectangle.Empty, bounds);
                Assert.False(readOnly);
                Assert.Null(displayText);
                Assert.True(cellIsVisible);
                editCallCount2++;
            };
            int commitCallCount2 = 0;
            gridColumn2.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount2++;
                return true;
            };
            int abortCallCount2 = 0;
            gridColumn2.AbortAction = (rowNum) =>
            {
                Assert.Equal(expectedRowNumber, rowNum);
                abortCallCount2++;
            };
            style.GridColumnStyles.Add(gridColumn2);

            var dataGrid = new SubDataGrid
            {
                BindingContext = bindingContext
            };
            dataGrid.TableStyles.Add(style);
            dataGrid.SetDataBinding(dataSource, null);

            // Simulate layout on the DataGrid.
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                dataGrid.OnPaint(new PaintEventArgs(graphics, new Rectangle(0, 0, 1, 2)));
            }

            // Edit cell.
            Assert.True(style.BeginEdit(gridColumn1, rowNumber));
            Assert.Equal(new DataGridCell(expectedRowNumber, 0), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(expectedCommitCallCount, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            Assert.Equal(0, abortCallCount1);
            Assert.Equal(0, abortCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));

            // End edit.
            gridColumn1.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount1++;
                return commitResult;
            };
            gridColumn2.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount2++;
                return commitResult;
            };
            dataGrid.ColumnStartedEditing(Rectangle.Empty);
            Assert.Equal(expected, style.EndEdit(gridColumn1, rowNumber, shouldAbort));
            Assert.Equal(new DataGridCell(expectedRowNumber, 0), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(expectedCommitCallCount2, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            Assert.Equal(expectedAbortCallCount, abortCallCount1);
            Assert.Equal(0, abortCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));

            // End edit again.
            Assert.False(style.EndEdit(gridColumn1, rowNumber, shouldAbort));
        }

        [Theory]
        [MemberData(nameof(EndEdit_DataSource_TestData))]
        public void DataGridTableStyle_EndEdit_InvokeWithDataSourceDifferentColumn_ReturnsExpected(int rowNumber, bool shouldAbort, int expectedRowNumber, int expectedEditCallCount, int expectedCommitCallCount, int expectedAbortCallCount, int expectedCommitCallCount2, bool commitResult, bool expected)
        {
            var dataSource = new DataClass[]
            {
                new DataClass { Value1 = "Value1_1", Value2 = "Value2_1" },
                new DataClass { Value1 = "Value1_2", Value2 = "Value2_2" }
            };
            var bindingContext = new BindingContext();
            CurrencyManager currencyManager = (CurrencyManager)bindingContext[dataSource, null];

            var style = new DataGridTableStyle
            {
                MappingName = "DataClass[]"
            };

            var gridColumn1 = new SubDataGridColumnStyle
            {
                MappingName = "Value1"
            };
            int editCallCount1 = 0;
            gridColumn1.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                Assert.Same(currencyManager, source);
                Assert.Equal(expectedRowNumber, rowNum);
                Assert.NotEqual(Rectangle.Empty, bounds);
                Assert.False(readOnly);
                Assert.Null(displayText);
                Assert.True(cellIsVisible);
                editCallCount1++;
            };
            int commitCallCount1 = 0;
            gridColumn1.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount1++;
                return true;
            };
            int abortCallCount1 = 0;
            gridColumn1.AbortAction = (rowNum) =>
            {
                Assert.Equal(expectedRowNumber, rowNum);
                abortCallCount1++;
            };
            style.GridColumnStyles.Add(gridColumn1);

            var gridColumn2 = new SubDataGridColumnStyle
            {
                MappingName = "Value2"
            };
            int editCallCount2 = 0;
            gridColumn2.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                Assert.Same(currencyManager, source);
                Assert.Equal(expectedRowNumber, rowNum);
                Assert.NotEqual(Rectangle.Empty, bounds);
                Assert.False(readOnly);
                Assert.Null(displayText);
                Assert.True(cellIsVisible);
                editCallCount2++;
            };
            int commitCallCount2 = 0;
            gridColumn2.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount2++;
                return true;
            };
            int abortCallCount2 = 0;
            gridColumn2.AbortAction = (rowNum) =>
            {
                Assert.Equal(expectedRowNumber, rowNum);
                abortCallCount2++;
            };
            style.GridColumnStyles.Add(gridColumn2);

            var dataGrid = new SubDataGrid
            {
                BindingContext = bindingContext
            };
            dataGrid.TableStyles.Add(style);
            dataGrid.SetDataBinding(dataSource, null);

            // Simulate layout on the DataGrid.
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                dataGrid.OnPaint(new PaintEventArgs(graphics, new Rectangle(0, 0, 1, 2)));
            }

            // Edit cell.
            Assert.True(style.BeginEdit(gridColumn1, rowNumber));
            Assert.Equal(new DataGridCell(expectedRowNumber, 0), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(expectedCommitCallCount, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            Assert.Equal(0, abortCallCount1);
            Assert.Equal(0, abortCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));

            // End edit.
            gridColumn1.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount1++;
                return commitResult;
            };
            gridColumn2.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount2++;
                return commitResult;
            };
            dataGrid.ColumnStartedEditing(Rectangle.Empty);
            Assert.Equal(expected, style.EndEdit(gridColumn2, rowNumber, shouldAbort));
            Assert.Equal(new DataGridCell(expectedRowNumber, 0), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(expectedCommitCallCount2, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            Assert.Equal(expectedAbortCallCount, abortCallCount1);
            Assert.Equal(0, abortCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));

            // End edit again.
            Assert.False(style.EndEdit(gridColumn2, rowNumber, shouldAbort));
        }

        public static IEnumerable<object[]> EndEdit_DataSourceInvalidColumn_TestData()
        {
            foreach (object[] testData in EndEdit_DataSource_TestData())
            {
                yield return testData.Prepend(null).ToArray();
                yield return testData.Prepend(new SubDataGridColumnStyle()).ToArray();
            }
        }

        [Theory]
        [MemberData(nameof(EndEdit_DataSourceInvalidColumn_TestData))]
        public void DataGridTableStyle_EndEdit_InvokeWithDataSourceInvalidColumn_ReturnsExpected(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort, int expectedRowNumber, int expectedEditCallCount, int expectedCommitCallCount, int expectedAbortCallCount, int expectedCommitCallCount2, bool commitResult, bool expected)
        {
            var dataSource = new DataClass[]
            {
                new DataClass { Value1 = "Value1_1", Value2 = "Value2_1" },
                new DataClass { Value1 = "Value1_2", Value2 = "Value2_2" }
            };
            var bindingContext = new BindingContext();
            CurrencyManager currencyManager = (CurrencyManager)bindingContext[dataSource, null];

            var style = new DataGridTableStyle
            {
                MappingName = "DataClass[]"
            };

            var gridColumn1 = new SubDataGridColumnStyle
            {
                MappingName = "Value1"
            };
            int editCallCount1 = 0;
            gridColumn1.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                Assert.Same(currencyManager, source);
                Assert.Equal(expectedRowNumber, rowNum);
                Assert.NotEqual(Rectangle.Empty, bounds);
                Assert.False(readOnly);
                Assert.Null(displayText);
                Assert.True(cellIsVisible);
                editCallCount1++;
            };
            int commitCallCount1 = 0;
            gridColumn1.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount1++;
                return true;
            };
            int abortCallCount1 = 0;
            gridColumn1.AbortAction = (rowNum) =>
            {
                Assert.Equal(expectedRowNumber, rowNum);
                abortCallCount1++;
            };
            style.GridColumnStyles.Add(gridColumn1);

            var gridColumn2 = new SubDataGridColumnStyle
            {
                MappingName = "Value2"
            };
            int editCallCount2 = 0;
            gridColumn2.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                Assert.Same(currencyManager, source);
                Assert.Equal(expectedRowNumber, rowNum);
                Assert.NotEqual(Rectangle.Empty, bounds);
                Assert.False(readOnly);
                Assert.Null(displayText);
                Assert.True(cellIsVisible);
                editCallCount2++;
            };
            int commitCallCount2 = 0;
            gridColumn2.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount2++;
                return true;
            };
            int abortCallCount2 = 0;
            gridColumn2.AbortAction = (rowNum) =>
            {
                Assert.Equal(expectedRowNumber, rowNum);
                abortCallCount2++;
            };
            style.GridColumnStyles.Add(gridColumn2);

            var dataGrid = new SubDataGrid
            {
                BindingContext = bindingContext
            };
            dataGrid.TableStyles.Add(style);
            dataGrid.SetDataBinding(dataSource, null);

            // Simulate layout on the DataGrid.
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                dataGrid.OnPaint(new PaintEventArgs(graphics, new Rectangle(0, 0, 1, 2)));
            }

            // Edit cell.
            Assert.True(style.BeginEdit(gridColumn1, rowNumber));
            Assert.Equal(new DataGridCell(expectedRowNumber, 0), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(expectedCommitCallCount, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            Assert.Equal(0, abortCallCount1);
            Assert.Equal(0, abortCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));

            // End edit.
            gridColumn1.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount1++;
                return commitResult;
            };
            gridColumn2.CommitAction = (dataSource, rowNum) =>
            {
                Assert.Same(currencyManager, dataSource);
                Assert.Equal(expectedRowNumber, rowNum);
                commitCallCount2++;
                return commitResult;
            };
            dataGrid.ColumnStartedEditing(Rectangle.Empty);
            Assert.Equal(expected, style.EndEdit(gridColumn, rowNumber, shouldAbort));
            Assert.Equal(new DataGridCell(expectedRowNumber, 0), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(expectedCommitCallCount2, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            Assert.Equal(expectedAbortCallCount, abortCallCount1);
            Assert.Equal(0, abortCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));

            // End edit again.
            Assert.False(style.EndEdit(gridColumn, rowNumber, shouldAbort));
        }

        [Theory]
        [InlineData(0, true, 0, 1, 0)]
        [InlineData(1, true, 1, 2, 1)]
        [InlineData(-1, true, 0, 1, 0)]
        [InlineData(2, true, 1, 2, 1)]
        [InlineData(0, false, 0, 1, 0)]
        [InlineData(1, false, 1, 2, 1)]
        [InlineData(-1, false, 0, 1, 0)]
        [InlineData(2, false, 1, 2, 1)]
        public void DataGridTableStyle_EndEdit_InvokeWithDataSourceNotEditing_ReturnsFalse(int rowNumber, bool shouldAbort, int expectedRowNumber, int expectedEditCallCount, int expectedCommitCallCount)
        {
            var dataSource = new DataClass[]
            {
                new DataClass { Value1 = "Value1_1", Value2 = "Value2_1" },
                new DataClass { Value1 = "Value1_2", Value2 = "Value2_2" }
            };
            var bindingContext = new BindingContext();
            CurrencyManager currencyManager = (CurrencyManager)bindingContext[dataSource, null];

            var style = new DataGridTableStyle
            {
                MappingName = "DataClass[]"
            };

            var gridColumn1 = new SubDataGridColumnStyle
            {
                MappingName = "Value1"
            };
            int editCallCount1 = 0;
            gridColumn1.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                editCallCount1++;
            };
            int commitCallCount1 = 0;
            gridColumn1.CommitAction = (dataSource, rowNum) =>
            {
                commitCallCount1++;
                return true;
            };
            style.GridColumnStyles.Add(gridColumn1);

            var gridColumn2 = new SubDataGridColumnStyle
            {
                MappingName = "Value2"
            };
            int editCallCount2 = 0;
            gridColumn2.EditAction = (source, rowNum, bounds, readOnly, displayText, cellIsVisible) =>
            {
                editCallCount2++;
            };
            int commitCallCount2 = 0;
            gridColumn2.CommitAction = (dataSource, rowNum) =>
            {
                commitCallCount2++;
                return true;
            };
            style.GridColumnStyles.Add(gridColumn2);

            var dataGrid = new SubDataGrid
            {
                BindingContext = bindingContext
            };
            dataGrid.TableStyles.Add(style);
            dataGrid.SetDataBinding(dataSource, null);

            // Simulate layout on the DataGrid.
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                dataGrid.OnPaint(new PaintEventArgs(graphics, new Rectangle(0, 0, 1, 2)));
            }

            // Edit cell.
            Assert.True(style.BeginEdit(gridColumn1, rowNumber));
            Assert.Equal(new DataGridCell(expectedRowNumber, 0), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(expectedCommitCallCount, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));

            // End edit.
            Assert.False(style.EndEdit(gridColumn1, rowNumber, shouldAbort));
            Assert.Equal(new DataGridCell(expectedRowNumber, 0), dataGrid.CurrentCell);
            Assert.Equal(expectedEditCallCount, editCallCount1);
            Assert.Equal(0, editCallCount2);
            Assert.Equal(expectedCommitCallCount, commitCallCount1);
            Assert.Equal(0, commitCallCount2);
            Assert.False(dataGrid.IsSelected(0));
            Assert.False(dataGrid.IsSelected(1));
        }

        public static IEnumerable<object[]> EndEdit_TestData()
        {
            foreach (bool shouldAbort in new bool[] { true, false })
            {
                yield return new object[] { null, null, -1, shouldAbort, null };
                yield return new object[] { null, null, 0, shouldAbort, null };
                yield return new object[] { null, null, 1, shouldAbort, null };
                yield return new object[] { null, new SubDataGridColumnStyle(), -1, shouldAbort, null };
                yield return new object[] { null, new SubDataGridColumnStyle(), 0, shouldAbort, null };
                yield return new object[] { null, new SubDataGridColumnStyle(), 1, shouldAbort, null };

                yield return new object[] { new DataGrid(), null, -1, shouldAbort, new DataGridCell(0, 0) };
                yield return new object[] { new DataGrid(), null, 0, shouldAbort, new DataGridCell(0, 0) };
                yield return new object[] { new DataGrid(), null, 1, shouldAbort, new DataGridCell(0, 0) };
                yield return new object[] { new DataGrid(), new SubDataGridColumnStyle(), -1, shouldAbort, new DataGridCell(0, 0) };
                yield return new object[] { new DataGrid(), new SubDataGridColumnStyle(), 0, shouldAbort, new DataGridCell(0, 0) };
                yield return new object[] { new DataGrid(), new SubDataGridColumnStyle(), 1, shouldAbort, new DataGridCell(0, 0) };
            }
        }

        [Theory]
        [MemberData(nameof(EndEdit_TestData))]
        public void DataGridTableStyle_EndEdit_InvokeWithoutDataSource_ReturnsFalse(DataGrid dataGrid, DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort, DataGridCell? expectedCurrentCell)
        {
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            Assert.False(style.EndEdit(gridColumn, rowNumber, shouldAbort));
            Assert.Equal(expectedCurrentCell, dataGrid?.CurrentCell);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnAllowSortingChanged_Invoke_CallsAllowSortingChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.AllowSortingChanged += handler;
            style.OnAllowSortingChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.AllowSortingChanged -= handler;
           style.OnAllowSortingChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnAlternatingBackColorChanged_Invoke_CallsAlternatingBackColorChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.AlternatingBackColorChanged += handler;
            style.OnAlternatingBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.AlternatingBackColorChanged -= handler;
           style.OnAlternatingBackColorChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnBackColorChanged_Invoke_CallsBackColorChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.BackColorChanged += handler;
            style.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.BackColorChanged -= handler;
           style.OnBackColorChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnColumnHeadersVisibleChanged_Invoke_CallsColumnHeadersVisibleChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.ColumnHeadersVisibleChanged += handler;
            style.OnColumnHeadersVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.ColumnHeadersVisibleChanged -= handler;
           style.OnColumnHeadersVisibleChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnForeColorChanged_Invoke_CallsForeColorChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.ForeColorChanged += handler;
            style.OnForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.ForeColorChanged -= handler;
           style.OnForeColorChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnGridLineColorChanged_Invoke_CallsGridLineColorChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.GridLineColorChanged += handler;
            style.OnGridLineColorChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.GridLineColorChanged -= handler;
           style.OnGridLineColorChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnGridLineStyleChanged_Invoke_CallsGridLineStyleChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.GridLineStyleChanged += handler;
            style.OnGridLineStyleChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.GridLineStyleChanged -= handler;
           style.OnGridLineStyleChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnHeaderBackColorChanged_Invoke_CallsHeaderBackColorChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.HeaderBackColorChanged += handler;
            style.OnHeaderBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.HeaderBackColorChanged -= handler;
           style.OnHeaderBackColorChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnHeaderFontChanged_Invoke_CallsHeaderFontChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.HeaderFontChanged += handler;
            style.OnHeaderFontChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.HeaderFontChanged -= handler;
           style.OnHeaderFontChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnHeaderForeColorChanged_Invoke_CallsHeaderForeColorChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle ();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.HeaderForeColorChanged += handler;
            style.OnHeaderForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.HeaderForeColorChanged -= handler;
           style.OnHeaderForeColorChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnLinkColorChanged_Invoke_CallsLineColorChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.LinkColorChanged += handler;
            style.OnLinkColorChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.LinkColorChanged -= handler;
           style.OnLinkColorChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnLinkHoverColorChanged_Invoke_CallsLinkHoverColorChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.LinkHoverColorChanged += handler;
            style.OnLinkHoverColorChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.LinkHoverColorChanged -= handler;
           style.OnLinkHoverColorChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnMappingNameChanged_Invoke_CallsMappingNameChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.MappingNameChanged += handler;
            style.OnMappingNameChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.MappingNameChanged -= handler;
           style.OnMappingNameChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnPreferredColumnWidthChanged_Invoke_CallsPreferredColumnWidthChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.PreferredColumnWidthChanged += handler;
            style.OnPreferredColumnWidthChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.PreferredColumnWidthChanged -= handler;
           style.OnPreferredColumnWidthChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnPreferredRowHeightChanged_Invoke_CallsPreferredRowHeightChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.PreferredRowHeightChanged += handler;
            style.OnPreferredRowHeightChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.PreferredRowHeightChanged -= handler;
           style.OnPreferredRowHeightChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnReadOnlyChanged_Invoke_CallsReadOnlyChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.ReadOnlyChanged += handler;
            style.OnReadOnlyChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.ReadOnlyChanged -= handler;
           style.OnReadOnlyChanged(eventArgs);
           Assert.Equal(1, callCount);
        }
        
        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnRowHeadersVisibleChanged_Invoke_CallsRowHeadersVisibleChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.RowHeadersVisibleChanged += handler;
            style.OnRowHeadersVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.RowHeadersVisibleChanged -= handler;
           style.OnRowHeadersVisibleChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnRowHeaderWidthChanged_Invoke_CallsRowHeaderWidthChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.RowHeaderWidthChanged += handler;
            style.OnRowHeaderWidthChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.RowHeaderWidthChanged -= handler;
           style.OnRowHeaderWidthChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnSelectionBackColorChanged_Invoke_CallsSelectionBackColorChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.SelectionBackColorChanged += handler;
            style.OnSelectionBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.SelectionBackColorChanged -= handler;
           style.OnSelectionBackColorChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DataGridTableStyle_OnSelectionForeColorChanged_Invoke_CallsSelectionForeColorChanged(EventArgs eventArgs)
        {
            var style = new SubDataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(style, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            style.SelectionForeColorChanged += handler;
            style.OnSelectionForeColorChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           style.SelectionForeColorChanged -= handler;
           style.OnSelectionForeColorChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetAlternatingBackColor_Invoke_GetReturnsExpected()
        {
            var style = new DataGridTableStyle();

            // Reset default.
            style.ResetAlternatingBackColor();
            Assert.Equal(SystemColors.Window, style.AlternatingBackColor);

            // Reset custom.
            style.AlternatingBackColor = Color.Red;
            Assert.Equal(Color.Red, style.AlternatingBackColor);
            style.ResetAlternatingBackColor();
            Assert.Equal(SystemColors.Window, style.AlternatingBackColor);
        }

        [Fact]
        public void DataGridTableStyle_ResetAlternatingBackColor_InvokeWithDataGrid_CallsInvalidate()
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            // Reset default.
            style.ResetAlternatingBackColor();
            Assert.Equal(SystemColors.Window, style.AlternatingBackColor);
            Assert.Equal(0, invalidatedCallCount);

            // Reset custom.
            style.AlternatingBackColor = Color.Red;
            Assert.Equal(Color.Red, style.AlternatingBackColor);
            Assert.Equal(1, invalidatedCallCount);
            style.ResetAlternatingBackColor();
            Assert.Equal(SystemColors.Window, style.AlternatingBackColor);
            Assert.Equal(3, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetAlternatingBackColor_InvokeWithHandler_CallsAlternatingBackColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(sender, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.AlternatingBackColorChanged += handler;

            // Reset default.
            style.ResetAlternatingBackColor();
            Assert.Equal(SystemColors.Window, style.AlternatingBackColor);
            Assert.Equal(0, callCount);

            // Reset custom.
            style.AlternatingBackColor = Color.Red;
            Assert.Equal(Color.Red, style.AlternatingBackColor);
            Assert.Equal(1, callCount);
            style.ResetAlternatingBackColor();
            Assert.Equal(SystemColors.Window, style.AlternatingBackColor);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetBackColor_Invoke_GetReturnsExpected()
        {
            var style = new DataGridTableStyle();

            // Reset default.
            style.ResetBackColor();
            Assert.Equal(SystemColors.Window, style.BackColor);

            // Reset custom.
            style.BackColor = Color.Red;
            Assert.Equal(Color.Red, style.BackColor);
            style.ResetBackColor();
            Assert.Equal(SystemColors.Window, style.BackColor);
        }

        [Fact]
        public void DataGridTableStyle_ResetBackColor_InvokeWithDataGrid_CallsInvalidate()
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            // Reset default.
            style.ResetBackColor();
            Assert.Equal(SystemColors.Window, style.BackColor);
            Assert.Equal(0, invalidatedCallCount);

            // Reset custom.
            style.BackColor = Color.Red;
            Assert.Equal(Color.Red, style.BackColor);
            Assert.Equal(1, invalidatedCallCount);
            style.ResetBackColor();
            Assert.Equal(SystemColors.Window, style.BackColor);
            Assert.Equal(2, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetBackColor_InvokeWithHandler_CallsBackColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(sender, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.BackColorChanged += handler;

            // Reset default.
            style.ResetBackColor();
            Assert.Equal(SystemColors.Window, style.BackColor);
            Assert.Equal(0, callCount);

            // Reset custom.
            style.BackColor = Color.Red;
            Assert.Equal(Color.Red, style.BackColor);
            Assert.Equal(1, callCount);
            style.ResetBackColor();
            Assert.Equal(SystemColors.Window, style.BackColor);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetForeColor_Invoke_GetReturnsExpected()
        {
            var style = new DataGridTableStyle();

            // Reset default.
            style.ResetForeColor();
            Assert.Equal(SystemColors.WindowText, style.ForeColor);

            // Reset custom.
            style.ForeColor = Color.Red;
            Assert.Equal(Color.Red, style.ForeColor);
            style.ResetForeColor();
            Assert.Equal(SystemColors.WindowText, style.ForeColor);
        }

        [Fact]
        public void DataGridTableStyle_ResetForeColor_InvokeWithDataGrid_CallsInvalidate()
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            // Reset default.
            style.ResetForeColor();
            Assert.Equal(SystemColors.WindowText, style.ForeColor);
            Assert.Equal(0, invalidatedCallCount);

            // Reset custom.
            style.ForeColor = Color.Red;
            Assert.Equal(Color.Red, style.ForeColor);
            Assert.Equal(1, invalidatedCallCount);
            style.ResetForeColor();
            Assert.Equal(SystemColors.WindowText, style.ForeColor);
            Assert.Equal(2, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetForeColor_InvokeWithHandler_CallsForeColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(sender, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.ForeColorChanged += handler;

            // Reset default.
            style.ResetForeColor();
            Assert.Equal(SystemColors.WindowText, style.ForeColor);
            Assert.Equal(0, callCount);

            // Reset custom.
            style.ForeColor = Color.Red;
            Assert.Equal(Color.Red, style.ForeColor);
            Assert.Equal(1, callCount);
            style.ResetForeColor();
            Assert.Equal(SystemColors.WindowText, style.ForeColor);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetHeaderBackColor_Invoke_GetReturnsExpected()
        {
            var style = new DataGridTableStyle();

            // Reset default.
            style.ResetHeaderBackColor();
            Assert.Equal(SystemColors.Control, style.HeaderBackColor);

            // Reset custom.
            style.HeaderBackColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderBackColor);
            style.ResetHeaderBackColor();
            Assert.Equal(SystemColors.Control, style.HeaderBackColor);
        }

        [Fact]
        public void DataGridTableStyle_ResetHeaderBackColor_InvokeWithDataGrid_DoesNotCallInvalidate()
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            // Reset default.
            style.ResetHeaderBackColor();
            Assert.Equal(SystemColors.Control, style.HeaderBackColor);
            Assert.Equal(0, invalidatedCallCount);

            // Reset custom.
            style.HeaderBackColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderBackColor);
            Assert.Equal(0, invalidatedCallCount);
            style.ResetHeaderBackColor();
            Assert.Equal(SystemColors.Control, style.HeaderBackColor);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetHeaderBackColor_InvokeWithHandler_CallsHeaderBackColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(sender, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.HeaderBackColorChanged += handler;

            // Reset default.
            style.ResetHeaderBackColor();
            Assert.Equal(SystemColors.Control, style.HeaderBackColor);
            Assert.Equal(0, callCount);

            // Reset custom.
            style.HeaderBackColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderBackColor);
            Assert.Equal(1, callCount);
            style.ResetHeaderBackColor();
            Assert.Equal(SystemColors.Control, style.HeaderBackColor);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetHeaderFont_Invoke_GetReturnsExpected()
        {
            var style = new DataGridTableStyle();
            var font = new Font(SystemFonts.MessageBoxFont.FontFamily, 10);

            // Reset default.
            style.ResetHeaderFont();
            Assert.Same(Control.DefaultFont, style.HeaderFont);

            // Reset custom.
            style.HeaderFont = font;
            Assert.Same(font, style.HeaderFont);
            style.ResetHeaderFont();
            Assert.Same(Control.DefaultFont, style.HeaderFont);
        }

        [Fact]
        public void DataGridTableStyle_ResetHeaderFont_InvokeWithDataGrid_DoesNotCallInvalidate()
        {
            var font1 = new Font(SystemFonts.MessageBoxFont.FontFamily, 10);
            var font2 = new Font(SystemFonts.MessageBoxFont.FontFamily, 11);
            var dataGrid = new DataGrid
            {
                Font = font1
            };
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            // Reset default.
            style.ResetHeaderFont();
            Assert.Same(font1, style.HeaderFont);
            Assert.Equal(0, invalidatedCallCount);

            // Reset custom.
            style.HeaderFont = font2;
            Assert.Same(font2, style.HeaderFont);
            Assert.Equal(0, invalidatedCallCount);
            style.ResetHeaderFont();
            Assert.Same(font1, style.HeaderFont);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetHeaderFont_InvokeWithHandler_CallsHeaderFontChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(sender, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.HeaderFontChanged += handler;
            var font = new Font(SystemFonts.MessageBoxFont.FontFamily, 10);

            // Reset default.
            style.ResetHeaderFont();
            Assert.Same(Control.DefaultFont, style.HeaderFont);
            Assert.Equal(0, callCount);

            // Reset custom.
            style.HeaderFont = font;
            Assert.Same(font, style.HeaderFont);
            Assert.Equal(1, callCount);
            style.ResetHeaderFont();
            Assert.Same(Control.DefaultFont, style.HeaderFont);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetHeaderForeColor_Invoke_GetReturnsExpected()
        {
            var style = new DataGridTableStyle();

            // Reset default.
            style.ResetHeaderForeColor();
            Assert.Equal(SystemColors.ControlText, style.HeaderForeColor);

            // Reset custom.
            style.HeaderForeColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderForeColor);
            style.ResetHeaderForeColor();
            Assert.Equal(SystemColors.ControlText, style.HeaderForeColor);
        }

        [Fact]
        public void DataGridTableStyle_ResetHeaderForeColor_InvokeWithDataGrid_DoesNotCallInvalidate()
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            // Reset default.
            style.ResetHeaderForeColor();
            Assert.Equal(SystemColors.ControlText, style.HeaderForeColor);
            Assert.Equal(0, invalidatedCallCount);

            // Reset custom.
            style.HeaderForeColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderForeColor);
            Assert.Equal(0, invalidatedCallCount);
            style.ResetHeaderForeColor();
            Assert.Equal(SystemColors.ControlText, style.HeaderForeColor);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetHeaderForeColor_InvokeWithHandler_CallsHeaderForeColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(sender, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.HeaderForeColorChanged += handler;

            // Reset default.
            style.ResetHeaderForeColor();
            Assert.Equal(SystemColors.ControlText, style.HeaderForeColor);
            Assert.Equal(0, callCount);

            // Reset custom.
            style.HeaderForeColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderForeColor);
            Assert.Equal(1, callCount);
            style.ResetHeaderForeColor();
            Assert.Equal(SystemColors.ControlText, style.HeaderForeColor);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetLinkColor_Invoke_GetReturnsExpected()
        {
            var style = new DataGridTableStyle();

            // Reset default.
            style.ResetLinkColor();
            Assert.Equal(SystemColors.HotTrack, style.LinkColor);
            Assert.Equal(SystemColors.HotTrack, style.LinkHoverColor);

            // Reset custom.
            style.LinkColor = Color.Red;
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(Color.Red, style.LinkHoverColor);
            style.ResetLinkColor();
            Assert.Equal(SystemColors.HotTrack, style.LinkColor);
            Assert.Equal(SystemColors.HotTrack, style.LinkHoverColor);
        }

        [Fact]
        public void DataGridTableStyle_ResetLinkColor_InvokeWithDataGrid_DoesNotCallInvalidate()
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            // Reset default.
            style.ResetLinkColor();
            Assert.Equal(SystemColors.HotTrack, style.LinkColor);
            Assert.Equal(SystemColors.HotTrack, style.LinkHoverColor);
            Assert.Equal(0, invalidatedCallCount);

            // Reset custom.
            style.LinkColor = Color.Red;
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(Color.Red, style.LinkHoverColor);
            Assert.Equal(0, invalidatedCallCount);
            style.ResetLinkColor();
            Assert.Equal(SystemColors.HotTrack, style.LinkColor);
            Assert.Equal(SystemColors.HotTrack, style.LinkHoverColor);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetLinkColor_InvokeWithHandler_CallsLinkColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(sender, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.LinkColorChanged += handler;

            // Reset default.
            style.ResetLinkColor();
            Assert.Equal(SystemColors.HotTrack, style.LinkColor);
            Assert.Equal(SystemColors.HotTrack, style.LinkHoverColor);
            Assert.Equal(0, callCount);

            // Reset custom.
            style.LinkColor = Color.Red;
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(1, callCount);
            style.ResetLinkColor();
            Assert.Equal(SystemColors.HotTrack, style.LinkColor);
            Assert.Equal(SystemColors.HotTrack, style.LinkHoverColor);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetLinkHoverColor_Invoke_Nop()
        {
            var style = new DataGridTableStyle();

            // Reset default.
            style.ResetLinkHoverColor();
            Assert.Equal(SystemColors.HotTrack, style.LinkColor);
            Assert.Equal(SystemColors.HotTrack, style.LinkHoverColor);

            // Reset custom.
            style.LinkColor = Color.Red;
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(Color.Red, style.LinkHoverColor);
            style.ResetLinkHoverColor();
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(Color.Red, style.LinkHoverColor);
        }

        [Fact]
        public void DataGridTableStyle_ResetLinkHoverColor_InvokeWithDataGrid_Nop()
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            // Reset default.
            style.ResetLinkHoverColor();
            Assert.Equal(SystemColors.HotTrack, style.LinkColor);
            Assert.Equal(SystemColors.HotTrack, style.LinkHoverColor);
            Assert.Equal(0, invalidatedCallCount);

            // Reset custom.
            style.LinkColor = Color.Red;
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(Color.Red, style.LinkHoverColor);
            Assert.Equal(0, invalidatedCallCount);
            style.ResetLinkHoverColor();
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(Color.Red, style.LinkHoverColor);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetLinkHoverColor_InvokeWithHandler_DoesNotCallLinkColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(sender, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.LinkColorChanged += handler;

            // Reset default.
            style.ResetLinkHoverColor();
            Assert.Equal(SystemColors.HotTrack, style.LinkColor);
            Assert.Equal(SystemColors.HotTrack, style.LinkHoverColor);
            Assert.Equal(0, callCount);

            // Reset custom.
            style.LinkColor = Color.Red;
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(Color.Red, style.LinkHoverColor);
            Assert.Equal(1, callCount);
            style.ResetLinkHoverColor();
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(Color.Red, style.LinkHoverColor);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetPreferredRowHeight_Invoke_GetReturnsExpected()
        {
            var style = new DataGridTableStyle();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(DataGridTableStyle))[nameof(DataGridTableStyle.PreferredRowHeight)];

            // Reset default.
            Assert.False(property.CanResetValue(style));
            property.ResetValue(style);
            Assert.Equal(Control.DefaultFont.Height + 3, style.PreferredRowHeight);

            // Reset custom.
            style.PreferredRowHeight = 1;
            Assert.Equal(1, style.PreferredRowHeight);
            Assert.True(property.CanResetValue(style));
            property.ResetValue(style);
            Assert.Equal(Control.DefaultFont.Height + 3, style.PreferredRowHeight);
        }

        [Fact]
        public void DataGridTableStyle_ResetPreferredRowHeight_InvokeWithDataGrid_DoesNotCallInvalidate()
        {
            var dataGrid = new DataGrid();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(DataGridTableStyle))[nameof(DataGridTableStyle.PreferredRowHeight)];
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            // Reset default.
            Assert.False(property.CanResetValue(style));
            property.ResetValue(style);
            Assert.Equal(Control.DefaultFont.Height + 3, style.PreferredRowHeight);
            Assert.Equal(0, invalidatedCallCount);

            // Reset custom.
            style.PreferredRowHeight = 1;
            Assert.Equal(1, style.PreferredRowHeight);
            Assert.Equal(0, invalidatedCallCount);
            Assert.True(property.CanResetValue(style));
            property.ResetValue(style);
            Assert.Equal(Control.DefaultFont.Height + 3, style.PreferredRowHeight);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetPreferredRowHeight_InvokeWithHandler_CallsPreferredRowHeightChanged()
        {
            var style = new DataGridTableStyle();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(DataGridTableStyle))[nameof(DataGridTableStyle.PreferredRowHeight)];
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(sender, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.PreferredRowHeightChanged += handler;

            // Reset default.
            Assert.False(property.CanResetValue(style));
            property.ResetValue(style);
            Assert.Equal(Control.DefaultFont.Height + 3, style.PreferredRowHeight);
            Assert.Equal(0, callCount);

            // Reset custom.
            style.PreferredRowHeight = 1;
            Assert.Equal(1, style.PreferredRowHeight);
            Assert.Equal(1, callCount);
            Assert.True(property.CanResetValue(style));
            property.ResetValue(style);
            Assert.Equal(Control.DefaultFont.Height + 3, style.PreferredRowHeight);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetSelectionBackColor_Invoke_GetReturnsExpected()
        {
            var style = new DataGridTableStyle();

            // Reset default.
            style.ResetSelectionBackColor();
            Assert.Equal(SystemColors.ActiveCaption, style.SelectionBackColor);

            // Reset custom.
            style.SelectionBackColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionBackColor);
            style.ResetSelectionBackColor();
            Assert.Equal(SystemColors.ActiveCaption, style.SelectionBackColor);
        }

        [Fact]
        public void DataGridTableStyle_ResetSelectionBackColor_InvokeWithDataGrid_CallsInvalidate()
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            // Reset default.
            style.ResetSelectionBackColor();
            Assert.Equal(SystemColors.ActiveCaption, style.SelectionBackColor);
            Assert.Equal(0, invalidatedCallCount);

            // Reset custom.
            style.SelectionBackColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionBackColor);
            Assert.Equal(1, invalidatedCallCount);
            style.ResetSelectionBackColor();
            Assert.Equal(SystemColors.ActiveCaption, style.SelectionBackColor);
            Assert.Equal(2, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetSelectionBackColor_InvokeWithHandler_CallsSelectionBackColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(sender, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.SelectionBackColorChanged += handler;

            // Reset default.
            style.ResetSelectionBackColor();
            Assert.Equal(SystemColors.ActiveCaption, style.SelectionBackColor);
            Assert.Equal(0, callCount);

            // Reset custom.
            style.SelectionBackColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionBackColor);
            Assert.Equal(1, callCount);
            style.ResetSelectionBackColor();
            Assert.Equal(SystemColors.ActiveCaption, style.SelectionBackColor);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetSelectionForeColor_Invoke_GetReturnsExpected()
        {
            var style = new DataGridTableStyle();

            // Reset default.
            style.ResetSelectionForeColor();
            Assert.Equal(SystemColors.ActiveCaptionText, style.SelectionForeColor);

            // Reset custom.
            style.SelectionForeColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionForeColor);
            style.ResetSelectionForeColor();
            Assert.Equal(SystemColors.ActiveCaptionText, style.SelectionForeColor);
        }

        [Fact]
        public void DataGridTableStyle_ResetSelectionForeColor_InvokeWithDataGrid_CallsInvalidate()
        {
            var dataGrid = new DataGrid();
            var style = new DataGridTableStyle
            {
                DataGrid = dataGrid
            };
            int invalidatedCallCount = 0;
            dataGrid.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, dataGrid.Handle);

            // Reset default.
            style.ResetSelectionForeColor();
            Assert.Equal(SystemColors.ActiveCaptionText, style.SelectionForeColor);
            Assert.Equal(0, invalidatedCallCount);

            // Reset custom.
            style.SelectionForeColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionForeColor);
            Assert.Equal(1, invalidatedCallCount);
            style.ResetSelectionForeColor();
            Assert.Equal(SystemColors.ActiveCaptionText, style.SelectionForeColor);
            Assert.Equal(2, invalidatedCallCount);
        }

        [Fact]
        public void DataGridTableStyle_ResetSelectionForeColor_InvokeWithHandler_CallsSelectionForeColorChanged()
        {
            var style = new DataGridTableStyle();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(sender, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            style.SelectionForeColorChanged += handler;

            // Reset default.
            style.ResetSelectionForeColor();
            Assert.Equal(SystemColors.ActiveCaptionText, style.SelectionForeColor);
            Assert.Equal(0, callCount);

            // Reset custom.
            style.SelectionForeColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionForeColor);
            Assert.Equal(1, callCount);
            style.ResetSelectionForeColor();
            Assert.Equal(SystemColors.ActiveCaptionText, style.SelectionForeColor);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridTableStyle_ShouldSerializeAlternatingBackColor_Invoke_ReturnsExpected()
        {
            var style = new SubDataGridTableStyle();
            Assert.False(style.ShouldSerializeAlternatingBackColor());

            style.AlternatingBackColor = Color.Red;
            Assert.Equal(Color.Red, style.AlternatingBackColor);
            Assert.True(style.ShouldSerializeAlternatingBackColor());
        }

        [Fact]
        public void DataGridTableStyle_ShouldSerializeBackColor_Invoke_ReturnsExpected()
        {
            var style = new SubDataGridTableStyle();
            Assert.False(style.ShouldSerializeBackColor());

            style.BackColor = Color.Red;
            Assert.Equal(Color.Red, style.BackColor);
            Assert.True(style.ShouldSerializeBackColor());
        }

        [Fact]
        public void DataGridTableStyle_ShouldSerializeForeColor_Invoke_ReturnsExpected()
        {
            var style = new SubDataGridTableStyle();
            Assert.False(style.ShouldSerializeForeColor());

            style.ForeColor = Color.Red;
            Assert.Equal(Color.Red, style.ForeColor);
            Assert.True(style.ShouldSerializeForeColor());
        }

        [Fact]
        public void DataGridTableStyle_ShouldSerializeGridLineColor_Invoke_ReturnsExpected()
        {
            var style = new SubDataGridTableStyle();
            Assert.False(style.ShouldSerializeGridLineColor());

            style.GridLineColor = Color.Red;
            Assert.Equal(Color.Red, style.GridLineColor);
            Assert.True(style.ShouldSerializeGridLineColor());
        }

        [Fact]
        public void DataGridTableStyle_ShouldSerializeHeaderBackColor_Invoke_ReturnsExpected()
        {
            var style = new SubDataGridTableStyle();
            Assert.False(style.ShouldSerializeHeaderBackColor());

            style.HeaderBackColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderBackColor);
            Assert.True(style.ShouldSerializeHeaderBackColor());
        }

        [Fact]
        public void DataGridTableStyle_ShouldSerializeHeaderFont_Invoke_ReturnsExpected()
        {
            var style = new DataGridTableStyle();
            var font = new Font(SystemFonts.MessageBoxFont.FontFamily, 10);
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(DataGridTableStyle))[nameof(DataGridTableStyle.HeaderFont)];
            Assert.False(property.ShouldSerializeValue(style));

            style.HeaderFont = font;
            Assert.Same(font, style.HeaderFont);
            Assert.True(property.ShouldSerializeValue(style));
        }

        [Fact]
        public void DataGridTableStyle_ShouldSerializeHeaderForeColor_Invoke_ReturnsExpected()
        {
            var style = new SubDataGridTableStyle();
            Assert.True(style.ShouldSerializeHeaderForeColor());

            style.HeaderForeColor = Color.Red;
            Assert.Equal(Color.Red, style.HeaderForeColor);
            Assert.True(style.ShouldSerializeHeaderForeColor());
        }

        [Fact]
        public void DataGridTableStyle_ShouldSerializeLinkColor_Invoke_ReturnsExpected()
        {
            var style = new SubDataGridTableStyle();
            Assert.False(style.ShouldSerializeLinkColor());

            style.LinkColor = Color.Red;
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(Color.Red, style.LinkHoverColor);
            Assert.True(style.ShouldSerializeLinkColor());
        }

        [Fact]
        public void DataGridTableStyle_ShouldSerializeLinkHoverColor_Invoke_ReturnsFalse()
        {
            var style = new SubDataGridTableStyle();
            Assert.False(style.ShouldSerializeLinkHoverColor());

            style.LinkColor = Color.Red;
            Assert.Equal(Color.Red, style.LinkColor);
            Assert.Equal(Color.Red, style.LinkHoverColor);
            Assert.False(style.ShouldSerializeLinkHoverColor());
        }

        [Fact]
        public void DataGridTableStyle_ShouldShouldSerializePreferredRowHeight_Invoke_ReturnsExpected()
        {
            var style = new SubDataGridTableStyle();
            Assert.False(style.ShouldSerializePreferredRowHeight());

            style.PreferredRowHeight = 1;
            Assert.Equal(1, style.PreferredRowHeight);
            Assert.True(style.ShouldSerializePreferredRowHeight());
        }

        [Fact]
        public void DataGridTableStyle_ShouldSerializeSelectionBackColor_Invoke_ReturnsExpected()
        {
            var style = new SubDataGridTableStyle();
            Assert.False(style.ShouldSerializeSelectionBackColor());

            style.SelectionBackColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionBackColor);
            Assert.True(style.ShouldSerializeSelectionBackColor());
        }

        [Fact]
        public void DataGridTableStyle_ShouldSerializeSelectionForeColor_Invoke_ReturnsExpected()
        {
            var style = new SubDataGridTableStyle();
            Assert.False(style.ShouldSerializeSelectionForeColor());

            style.SelectionForeColor = Color.Red;
            Assert.Equal(Color.Red, style.SelectionForeColor);
            Assert.True(style.ShouldSerializeSelectionForeColor());
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

            public new DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop) => base.CreateGridColumn(prop);

            public new DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop, bool isDefault) => base.CreateGridColumn(prop, isDefault);

            public new void Dispose(bool disposing) => base.Dispose(disposing);

            public new void OnAllowSortingChanged(EventArgs e) => base.OnAllowSortingChanged(e);

            public new void OnAlternatingBackColorChanged(EventArgs e) => base.OnAlternatingBackColorChanged(e);

            public new void OnBackColorChanged(EventArgs e) => base.OnBackColorChanged(e);

            public new void OnColumnHeadersVisibleChanged(EventArgs e) => base.OnColumnHeadersVisibleChanged(e);

            public new void OnForeColorChanged(EventArgs e) => base.OnForeColorChanged(e);

            public new void OnGridLineColorChanged(EventArgs e) => base.OnGridLineColorChanged(e);

            public new void OnGridLineStyleChanged(EventArgs e) => base.OnGridLineStyleChanged(e);

            public new void OnHeaderBackColorChanged(EventArgs e) => base.OnHeaderBackColorChanged(e);

            public new void OnHeaderFontChanged(EventArgs e) => base.OnHeaderFontChanged(e);

            public new void OnHeaderForeColorChanged(EventArgs e) => base.OnHeaderForeColorChanged(e);

            public new void OnLinkColorChanged(EventArgs e) => base.OnLinkColorChanged(e);

            public new void OnLinkHoverColorChanged(EventArgs e) => base.OnLinkHoverColorChanged(e);

            public new void OnMappingNameChanged(EventArgs e) => base.OnMappingNameChanged(e);

            public new void OnPreferredColumnWidthChanged(EventArgs e) => base.OnPreferredColumnWidthChanged(e);

            public new void OnPreferredRowHeightChanged(EventArgs e) => base.OnPreferredRowHeightChanged(e);

            public new void OnReadOnlyChanged(EventArgs e) => base.OnReadOnlyChanged(e);

            public new void OnRowHeadersVisibleChanged(EventArgs e) => base.OnRowHeadersVisibleChanged(e);

            public new void OnRowHeaderWidthChanged(EventArgs e) => base.OnRowHeaderWidthChanged(e);

            public new void OnSelectionBackColorChanged(EventArgs e) => base.OnSelectionBackColorChanged(e);

            public new void OnSelectionForeColorChanged(EventArgs e) => base.OnSelectionForeColorChanged(e);

            public new bool ShouldSerializeAlternatingBackColor() => base.ShouldSerializeAlternatingBackColor();

            public new bool ShouldSerializeBackColor() => base.ShouldSerializeBackColor();

            public new bool ShouldSerializeForeColor() => base.ShouldSerializeForeColor();

            public new bool ShouldSerializeGridLineColor() => base.ShouldSerializeGridLineColor();

            public new bool ShouldSerializeHeaderBackColor() => base.ShouldSerializeHeaderBackColor();

            public new bool ShouldSerializeHeaderForeColor() => base.ShouldSerializeHeaderForeColor();

            public new bool ShouldSerializeLinkColor() => base.ShouldSerializeLinkColor();

            public new bool ShouldSerializeLinkHoverColor() => base.ShouldSerializeLinkHoverColor();

            public new bool ShouldSerializePreferredRowHeight() => base.ShouldSerializePreferredRowHeight();

            public new bool ShouldSerializeSelectionBackColor() => base.ShouldSerializeSelectionBackColor();

            public new bool ShouldSerializeSelectionForeColor() => base.ShouldSerializeSelectionForeColor();
        }

        private class SubDataGridColumnStyle : DataGridColumnStyle
        {
            public Action<int> AbortAction { get; set; }

            protected internal override void Abort(int rowNum)
            {
                AbortAction(rowNum);
            }

            public Func<CurrencyManager, int, bool> CommitAction { get; set; }

            protected internal override bool Commit(CurrencyManager dataSource, int rowNum)
            {
                return CommitAction(dataSource, rowNum);
            }

            public Action<CurrencyManager, int, Rectangle, bool, string, bool> EditAction { get; set; }

            protected internal override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText, bool cellIsVisible)
            {
                EditAction(source, rowNum, bounds, readOnly, displayText, cellIsVisible);
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

            public Action<DataGrid> SetDataGridInColumnAction { get; set; }

            protected override void SetDataGridInColumn(DataGrid value)
            {
                if (SetDataGridInColumnAction == null)
                {
                    base.SetDataGridInColumn(value);
                }
                else
                {
                    SetDataGridInColumnAction(value);
                }
            }

            public Action<bool> DisposeAction { get; set; }

            protected override void Dispose(bool disposing)
            {
                DisposeAction?.Invoke(disposing);
            }
        }

        private class NullGridColumnStylesDataGridTableStyle : DataGridTableStyle
        {
            public override GridColumnStylesCollection GridColumnStyles => null;

            public new void Dispose(bool disposing) => base.Dispose(disposing);
        }

        private class SubDataGrid : DataGrid
        {
            public new void ColumnStartedEditing(Rectangle bounds) => base.ColumnStartedEditing(bounds);

            public new void OnPaint(PaintEventArgs pe) => base.OnPaint(pe);
        }

        private class DataClass
        {
            public string Value1 { get; set; }

            public string Value2 { get; set; }
        }


        private class ClassWithProperties
        {
            public bool BoolProperty { get; set; }

            public string StringProperty { get; set; }

            public DateTime DateTimeProperty { get; set; }

            public sbyte SByteProperty { get; set; }

            public short ShortProperty { get; set; }

            public int IntProperty { get; set; }

            public long LongProperty { get; set; }

            public byte ByteProperty { get; set; }

            public ushort UShortProperty { get; set; }

            public uint UIntProperty { get; set; }

            public ulong ULongProperty { get; set; }

            public char CharProperty { get; set; }

            public decimal DecimalProperty { get; set; }

            public double DoubleProperty { get; set; }

            public float FloatProperty { get; set; }

            public object ObjectProperty { get; set; }
        }
    }
}
