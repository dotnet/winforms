// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Globalization;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewRowTests
    {
        [Fact]
        public void DataGridViewRow_Ctor_Default()
        {
            var row = new SubDataGridViewRow();
            Assert.Equal(DataGridViewElementStates.Visible, row.State);
            Assert.Null(row.DataGridView);

            Assert.False(row.HasDefaultCellStyle);
            Assert.Equal(-1, row.Index);
            Assert.True(row.IsRow);
        }

        public static IEnumerable<object[]> SharedRow_TestData()
        {
            var dataGridView = new DataGridView();
            dataGridView.Columns.Add("Column", "Text");
            dataGridView.Rows.Add(new SubDataGridViewRow());
            yield return new object[] { dataGridView.Rows.SharedRow(1) };
        }

        public static IEnumerable<object[]> ContextMenu_Get_TestData()
        {
            var menu1 = new ContextMenuStrip();
            yield return new object[] { new DataGridViewRow(), null };
            yield return new object[] { new DataGridViewRow { ContextMenuStrip = menu1 }, menu1 };

            var menu2 = new ContextMenuStrip();
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow secondRow = dataGridView.Rows[1];
            secondRow.ContextMenuStrip = menu2;
            yield return new object[] { dataGridView.Rows[0], null };
            yield return new object[] { secondRow, menu2 };
        }

        [Theory]
        [MemberData(nameof(ContextMenu_Get_TestData))]
        public void DataGridViewRow_ContextMenuStrip_Get_ReturnsExpected(DataGridViewRow row, ContextMenuStrip expected)
        {
            Assert.Equal(expected, row.ContextMenuStrip);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_ContextMenuStrip_GetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.ContextMenuStrip);
        }

        public static IEnumerable<object[]> ContextMenuStrip_Set_TestData()
        {
            yield return new object[] { new DataGridViewRow() };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[1] };
        }

        [Theory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void DataGridViewRow_ContextMenuStrip_Set_GetReturnsExpected(DataGridViewRow row)
        {
            // Set non-null.
            var menu1 = new ContextMenuStrip();
            row.ContextMenuStrip = menu1;
            Assert.Same(menu1, row.ContextMenuStrip);

            // Set same.
            row.ContextMenuStrip = menu1;
            Assert.Same(menu1, row.ContextMenuStrip);

            // Set different.
            var menu2 = new ContextMenuStrip();
            row.ContextMenuStrip = menu2;
            Assert.Same(menu2, row.ContextMenuStrip);

            // Set null.
            row.ContextMenuStrip = null;
            Assert.Null(row.ContextMenuStrip);
        }

        [Fact]
        public void DataGridViewRow_ContextMenuStrip_SetWithDataGridView_CallsRowContextMenuStripChanged()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Same(row, e.Row);
            };
            dataGridView.RowContextMenuStripChanged += handler;

            // Set non-null.
            var menu1 = new ContextMenuStrip();
            row.ContextMenuStrip = menu1;
            Assert.Same(menu1, row.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Set same.
            row.ContextMenuStrip = menu1;
            Assert.Same(menu1, row.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Set different.
            var menu2 = new ContextMenuStrip();
            row.ContextMenuStrip = menu2;
            Assert.Same(menu2, row.ContextMenuStrip);
            Assert.Equal(2, callCount);

            // Set null.
            row.ContextMenuStrip = null;
            Assert.Null(row.ContextMenuStrip);
            Assert.Equal(3, callCount);

            // Remove handler.
            dataGridView.RowContextMenuStripChanged -= handler;
            row.ContextMenuStrip = menu1;
            Assert.Equal(menu1, row.ContextMenuStrip);
            Assert.Equal(3, callCount);
        }

        [Fact]
        public void DataGridViewRow_ContextMenuStrip_SetShared_Success()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows.SharedRow(1);

            // Set non-null.
            var menu1 = new ContextMenuStrip();
            row.ContextMenuStrip = menu1;
            Assert.Throws<InvalidOperationException>(() => row.ContextMenuStrip);

            // Set same.
            row.ContextMenuStrip = menu1;
            Assert.Throws<InvalidOperationException>(() => row.ContextMenuStrip);

            // Set different.
            var menu2 = new ContextMenuStrip();
            row.ContextMenuStrip = menu2;
            Assert.Throws<InvalidOperationException>(() => row.ContextMenuStrip);

            // Set null.
            row.ContextMenuStrip = null;
            Assert.Throws<InvalidOperationException>(() => row.ContextMenuStrip);
        }

        [Fact]
        public void DataGridViewRow_ContextMenuStrip_Dispose_SetsToNull()
        {
            var row = new SubDataGridViewRow();
            var menu = new ContextMenuStrip();
            row.ContextMenuStrip = menu;
            Assert.Same(menu, row.ContextMenuStrip);

            menu.Dispose();
            Assert.Null(row.ContextMenuStrip);
        }

        [Fact]
        public void DataGridViewRow_ContextMenuStrip_ResetThenDispose_Nop()
        {
            var row = new SubDataGridViewRow();
            var menu1 = new ContextMenuStrip();
            row.ContextMenuStrip = menu1;
            Assert.Same(menu1, row.ContextMenuStrip);

            var menu2 = new ContextMenuStrip();
            row.ContextMenuStrip = menu2;

            menu1.Dispose();
            Assert.Same(menu2, row.ContextMenuStrip);
        }

        [Fact]
        public void DataGridViewRow_Cells_Get_ReturnsSameInstance()
        {
            var row = new DataGridViewRow();
            Assert.Same(row.Cells, row.Cells);
            Assert.Empty(row.Cells);
        }

        public static IEnumerable<object[]> DataBoundItem_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), null };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], null };
            yield return new object[] { dataGridView.Rows.SharedRow(1), null };

            var boundObject = new { Name = "Name" };
            var bound = new DataGridView { DataSource = new[] { boundObject } };
            new Form().Controls.Add(bound);
            Assert.NotNull(bound.BindingContext);
            yield return new object[] { bound.Rows.SharedRow(0), boundObject };
            yield return new object[] { bound.Rows[0], boundObject };
        }

        [Theory]
        [MemberData(nameof(DataBoundItem_Get_TestData))]
        public void DataGridViewRow_DataBoundItem_Get_ReturnsExpected(DataGridViewRow row, object expected)
        {
            Assert.Equal(expected, row.DataBoundItem);
        }

        public static IEnumerable<object[]> DefaultCellStyle_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow() };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0] };
            yield return new object[] { dataGridView.Rows.SharedRow(1) };
        }

        [Theory]
        [MemberData(nameof(DefaultCellStyle_Get_TestData))]
        public void DataGridViewRow_DefaultCellStyle_Get_ReturnsSameInstance(DataGridViewRow row)
        {
            Assert.Same(row.DefaultCellStyle, row.DefaultCellStyle);
            Assert.Equal(DataGridViewCellStyleScopes.Row, row.DefaultCellStyle.Scope);
        }

        public static IEnumerable<object[]> DefaultCellStyle_Set_TestData()
        {
            var style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomRight };

            yield return new object[] { new DataGridViewRow(), null, new DataGridViewCellStyle() };
            yield return new object[] { new DataGridViewRow(), style, style };

            var dataGridView1 = new DataGridView { ColumnCount = 1 };
            dataGridView1.Rows.Add(new DataGridViewRow());
            var dataGridView2 = new DataGridView { ColumnCount = 1 };
            dataGridView2.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView1.Rows[0], null, new DataGridViewCellStyle() };
            yield return new object[] { dataGridView2.Rows[0], style, style };

            var templateDataGridView1 = new DataGridView();
            var templateDataGridView2 = new DataGridView();
            yield return new object[] { templateDataGridView1.RowTemplate, null, new DataGridViewCellStyle() };
            yield return new object[] { templateDataGridView2.RowTemplate, style, style };
        }

        [Theory]
        [MemberData(nameof(DefaultCellStyle_Set_TestData))]
        public void DataGridViewRow_DefaultCellStyle_SetWithNullOldValue_GetReturnsExpected(DataGridViewRow row, DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            row.DefaultCellStyle = value;
            Assert.Equal(expected, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);

            // Set same.
            row.DefaultCellStyle = value;
            Assert.Equal(expected, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
        }

        [Theory]
        [MemberData(nameof(DefaultCellStyle_Set_TestData))]
        public void DataGridViewRow_DefaultCellStyle_SetWithNonNullOldValue_GetReturnsExpected(DataGridViewRow row, DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            row.DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomCenter };
            row.DefaultCellStyle = value;
            Assert.Equal(expected, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);

            // Set same.
            row.DefaultCellStyle = value;
            Assert.Equal(expected, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
        }

        [Fact]
        public void DataGridViewRow_DefaultCellStyle_SetWithDataGridView_CallsRowDefaultCellStyleChanged()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Same(row, e.Row);
            };
            dataGridView.RowDefaultCellStyleChanged += handler;

            var style1 = new DataGridViewCellStyle();

            // Set non-null.
            row.DefaultCellStyle = style1;
            Assert.Equal(style1, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.Equal(1, callCount);

            // Set same.
            row.DefaultCellStyle = style1;
            Assert.Equal(style1, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.Equal(1, callCount);

            // Set different.
            var style2 = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomCenter };
            row.DefaultCellStyle = style2;
            Assert.Same(style2, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.Equal(2, callCount);

            // Set null.
            row.DefaultCellStyle = null;
            Assert.NotNull(row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.Equal(3, callCount);

            // Remove handler.
            dataGridView.RowDefaultCellStyleChanged -= handler;
            row.DefaultCellStyle = style1;
            Assert.Equal(style1, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.Equal(3, callCount);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_DefaultCellStyle_SetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.DefaultCellStyle = new DataGridViewCellStyle());
        }

        public static IEnumerable<object[]> DefaultHeaderCellType_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), typeof(DataGridViewRowHeaderCell) };

            var style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomRight };
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], typeof(DataGridViewRowHeaderCell) };
            yield return new object[] { dataGridView.Rows.SharedRow(1), typeof(DataGridViewRowHeaderCell) };
        }

        [Theory]
        [MemberData(nameof(DefaultHeaderCellType_Get_TestData))]
        public void DataGridViewRow_DefaultHeaderCellType_Get_ReturnsExpected(DataGridViewRow row, Type expected)
        {
            Assert.Equal(expected, row.DefaultHeaderCellType);
        }

        public static IEnumerable<object[]> DefaultHeaderCellType_Set_TestData()
        {
            yield return new object[] { new DataGridViewRow(), null, typeof(DataGridViewRowHeaderCell) };

            var style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomRight };
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], typeof(DataGridViewRowHeaderCell), typeof(DataGridViewRowHeaderCell) };
            yield return new object[] { dataGridView.Rows.SharedRow(1), typeof(DataGridViewColumnHeaderCell), typeof(DataGridViewColumnHeaderCell) };
            yield return new object[] { new DataGridViewRow(), typeof(DataGridViewHeaderCell), typeof(DataGridViewHeaderCell) };
        }

        [Theory]
        [MemberData(nameof(DefaultHeaderCellType_Set_TestData))]
        public void DataGridViewRow_DefaultHeaderCellType_Set_GetReturnsExpected(DataGridViewRow row, Type value, Type expected)
        {
            row.DefaultHeaderCellType = value;
            Assert.Equal(expected, row.DefaultHeaderCellType);

            // Set same.
            row.DefaultHeaderCellType = value;
            Assert.Equal(expected, row.DefaultHeaderCellType);
        }

        [Theory]
        [InlineData(typeof(DataGridViewRowHeaderCell))]
        [InlineData(typeof(DataGridViewColumnHeaderCell))]
        [InlineData(typeof(DataGridViewHeaderCell))]
        public void DataGridViewRow_DefaultHeaderCellType_SetWithNonNullOldValue_GetReturnsExpected(Type value)
        {
            var row = new SubDataGridViewRow
            {
                DefaultHeaderCellType = typeof(DataGridViewRowHeaderCell)
            };
            row.DefaultHeaderCellType = value;
            Assert.Equal(value, row.DefaultHeaderCellType);

            // Set same.
            row.DefaultHeaderCellType = value;
            Assert.Equal(value, row.DefaultHeaderCellType);
        }

        [Theory]
        [InlineData(typeof(int))]
        public void DataGridViewRow_DefaultHeaderCellType_SetInvalidWithNullOldValue_GetReturnsExpected(Type value)
        {
            var row = new SubDataGridViewRow();
            Assert.Throws<ArgumentException>("value", () => row.DefaultHeaderCellType = value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(typeof(int))]
        public void DataGridViewRow_DefaultHeaderCellType_SetInvalidWithNonNullOldValue_GetReturnsExpected(Type value)
        {
            var row = new SubDataGridViewRow
            {
                DefaultHeaderCellType = typeof(DataGridViewRowHeaderCell)
            };
            Assert.Throws<ArgumentException>("value", () => row.DefaultHeaderCellType = value);
        }

        public static IEnumerable<object[]> Displayed_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), false };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], false };
        }

        [Theory]
        [MemberData(nameof(Displayed_Get_TestData))]
        public void DataGridViewRow_Displayed_Get_ReturnsExpected(DataGridViewRow row, bool expected)
        {
            Assert.Equal(expected, row.Displayed);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_Displayed_GetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.Displayed);
        }

        public static IEnumerable<object[]> DividerHeight_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), 0 };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], 0 };
            yield return new object[] { dataGridView.Rows.SharedRow(1), 0 };
        }

        [Theory]
        [MemberData(nameof(DividerHeight_Get_TestData))]
        public void DataGridViewRow_DividerHeight_Get_ReturnsExpected(DataGridViewRow row, int expected)
        {
            Assert.Equal(expected, row.DividerHeight);
        }

        public static IEnumerable<object[]> DividerHeight_Set_TestData()
        {
            foreach (int value in new int[] { 0, 1, 65536 })
            {
                yield return new object[] { new DataGridViewRow(), value };

                var dataGridView = new DataGridView { ColumnCount = 1 };
                dataGridView.Rows.Add(new DataGridViewRow());
                yield return new object[] { dataGridView.Rows[0], value };
            }
        }

        [Theory]
        [MemberData(nameof(DividerHeight_Set_TestData))]
        public void DataGridViewRow_DividerHeight_Set_GetReturnsExpected(DataGridViewRow row, int value)
        {
            row.DividerHeight = value;
            Assert.Equal(value, row.DividerHeight);

            // Set same.
            row.DividerHeight = value;
            Assert.Equal(value, row.DividerHeight);
        }

        [Fact]
        public void DataGridViewRow_DividerHeight_SetWithDataGridView_CallsRowDividerHeightChanged()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Same(row, e.Row);
            };
            dataGridView.RowDividerHeightChanged += handler;

            // Set non-zero.
            row.DividerHeight = 4;
            Assert.Equal(4, row.DividerHeight);
            Assert.Equal(1, callCount);

            // Set same.
            row.DividerHeight = 4;
            Assert.Equal(4, row.DividerHeight);
            Assert.Equal(1, callCount);

            // Set different.
            row.DividerHeight = 3;
            Assert.Equal(3, row.DividerHeight);
            Assert.Equal(2, callCount);

            // Remove handler.
            dataGridView.RowDividerHeightChanged -= handler;
            row.DividerHeight = 4;
            Assert.Equal(4, row.DividerHeight);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(65537)]
        public void DataGridViewRow_DividerHeight_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            var row = new DataGridViewRow();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => row.DividerHeight = value);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_DividerHeight_SetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.DividerHeight = -1);
        }

        public static IEnumerable<object[]> ErrorText_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), string.Empty };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], string.Empty };
        }

        [Theory]
        [MemberData(nameof(ErrorText_Get_TestData))]
        public void DataGridViewRow_ErrorText_Get_ReturnsExpected(DataGridViewRow row, string expected)
        {
            Assert.Equal(expected, row.ErrorText);
        }

        public static IEnumerable<object[]> ErrorText_GetNeedsErrorText_TestData()
        {
            yield return new object[] { new DataGridView { ColumnCount = 1, VirtualMode = true } };

            var bound = new DataGridView { DataSource = new[] { new { Name = "Name" } } };
            new Form().Controls.Add(bound);
            Assert.NotNull(bound.BindingContext);
            yield return new object[] { bound };
        }

        [Theory]
        [MemberData(nameof(ErrorText_GetNeedsErrorText_TestData))]
        public void DataGridViewRow_ErrorText_GetNeedsErrorText_CallsRowErrorTextNeeded(DataGridView dataGridView)
        {
            DataGridViewRow row = dataGridView.Rows[0];
            row.ErrorText = "errorText1";

            int callCount = 0;
            DataGridViewRowErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal("errorText1", e.ErrorText);
                e.ErrorText = "errorText2";
            };
            dataGridView.RowErrorTextNeeded += handler;

            Assert.Same("errorText2", row.ErrorText);
            Assert.Equal(1, callCount);

            // Remove the handler.
            dataGridView.RowErrorTextNeeded -= handler;
            Assert.Same("errorText1", row.ErrorText);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_ErrorText_GetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.ErrorText);
        }

        public static IEnumerable<object[]> ErrorText_Set_TestData()
        {
            foreach (string value in new string[] { null, string.Empty, "reasonable" })
            {
                yield return new object[] { new DataGridViewRow(), value, value ?? string.Empty };

                var dataGridView = new DataGridView { ColumnCount = 1 };
                dataGridView.Rows.Add(new DataGridViewRow());
                yield return new object[] { dataGridView.Rows[0], value, value ?? string.Empty };
            }
        }

        [Theory]
        [MemberData(nameof(ErrorText_Set_TestData))]
        public void DataGridViewRow_ErrorText_Set_GetReturnsExpected(DataGridViewRow row, string value, string expected)
        {
            row.ErrorText = value;
            Assert.Same(expected, row.ErrorText);

            // Set same.
            row.ErrorText = value;
            Assert.Same(expected, row.ErrorText);
        }

        [Theory]
        [MemberData(nameof(ErrorText_Set_TestData))]
        public void DataGridViewRow_ErrorText_SetWithNonNullOldValue_GetReturnsExpected(DataGridViewRow row, string value, string expected)
        {
            row.ErrorText = "value";
            row.ErrorText = value;
            Assert.Same(expected, row.ErrorText);

            // Set same.
            row.ErrorText = value;
            Assert.Same(expected, row.ErrorText);
        }

        [Fact]
        public void DataGridViewRow_ErrorText_SetWithDataGridView_CallsRowErrorTextChanged()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Same(row, e.Row);
            };
            dataGridView.RowErrorTextChanged += handler;

            // Set non-null.
            row.ErrorText = "errorText";
            Assert.Equal("errorText", row.ErrorText);
            Assert.Equal(1, callCount);

            // Set same.
            row.ErrorText = "errorText";
            Assert.Equal("errorText", row.ErrorText);
            Assert.Equal(1, callCount);

            // Set null.
            row.ErrorText = null;
            Assert.Empty(row.ErrorText);
            Assert.Equal(2, callCount);

            // Set different.
            row.ErrorText = "other";
            Assert.Equal("other", row.ErrorText);
            Assert.Equal(3, callCount);

            // Remove handler.
            dataGridView.RowErrorTextChanged -= handler;
            row.ErrorText = "errorText";
            Assert.Equal("errorText", row.ErrorText);
            Assert.Equal(3, callCount);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_ErrorText_SetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => row.ErrorText = "value");
        }

        public static IEnumerable<object[]> Frozen_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), false };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], false };
        }

        [Theory]
        [MemberData(nameof(Frozen_Get_TestData))]
        public void DataGridViewRow_Frozen_Get_ReturnsExpected(DataGridViewRow row, bool expected)
        {
            Assert.Equal(expected, row.Frozen);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_Frozen_GetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.Frozen);
        }

        public static IEnumerable<object[]> Frozen_Set_TestData()
        {
            yield return new object[] { new DataGridViewRow() };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0] };
        }

        [Theory]
        [MemberData(nameof(Frozen_Set_TestData))]
        public void DataGridViewRow_Frozen_SetGetReturnsExpected(DataGridViewRow row)
        {
            // Set true.
            row.Frozen = true;
            Assert.True(row.Frozen);

            // Set same.
            row.Frozen = true;
            Assert.True(row.Frozen);

            // Set different.
            row.Frozen = false;
            Assert.False(row.Frozen);
        }

        [Fact]
        public void DataGridViewRow_Frozen_SetWithDataGridView_CallsRowStateChanged()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowStateChangedEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Same(row, e.Row);
            };
            dataGridView.RowStateChanged += handler;

            // Set true.
            row.Frozen = true;
            Assert.True(row.Frozen);
            Assert.Equal(1, callCount);

            // Set same.
            row.Frozen = true;
            Assert.True(row.Frozen);
            Assert.Equal(1, callCount);

            // Set different.
            row.Frozen = false;
            Assert.False(row.Frozen);
            Assert.Equal(2, callCount);

            // Remove handler.
            dataGridView.RowStateChanged -= handler;
            row.Frozen = true;
            Assert.True(row.Frozen);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_Frozen_SetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.Frozen = false);
        }

        public static IEnumerable<object[]> HeaderCell_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow() };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0] };
            yield return new object[] { dataGridView.Rows.SharedRow(1) };
        }

        [Theory]
        [MemberData(nameof(HeaderCell_Get_TestData))]
        public void DataGridViewRow_HeaderCell_Get_ReturnsExpected(DataGridViewRow row)
        {
            Assert.Same(row.HeaderCell, row.HeaderCell);
            Assert.Equal(row, row.HeaderCell.OwningRow);
        }

        public static IEnumerable<object[]> HeaderCell_Set_TestData()
        {
            var cell1 = new DataGridViewRowHeaderCell();
            yield return new object[] { new DataGridViewRow(), cell1 };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            var cellWithParent = new DataGridViewRowHeaderCell();
            var parentRow = new DataGridViewRow { HeaderCell = cellWithParent };
            var cell2 = new DataGridViewRowHeaderCell();
            yield return new object[] { dataGridView.Rows[0], cell2 };
            yield return new object[] { dataGridView.Rows.SharedRow(0), cellWithParent };
        }

        [Theory]
        [MemberData(nameof(HeaderCell_Set_TestData))]
        public void DataGridViewRow_HeaderCell_SetWithoutDataGridView_GetReturnsExpected(DataGridViewRow row, DataGridViewRowHeaderCell value)
        {
            row.HeaderCell = value;
            if (value != null)
            {
                Assert.Same(value, row.HeaderCell);
            }
            else
            {
                Assert.NotNull(row.HeaderCell);
            }
            Assert.Equal(row, row.HeaderCell.OwningRow);

            // Set same.
            row.HeaderCell = value;
            if (value != null)
            {
                Assert.Same(value, row.HeaderCell);
            }
            else
            {
                Assert.NotNull(row.HeaderCell);
            }
            Assert.Equal(row, row.HeaderCell.OwningRow);
        }

        [Theory]
        [MemberData(nameof(HeaderCell_Set_TestData))]
        public void DataGridViewRow_HeaderCell_SetWithNonNullOldValue_GetReturnsExpected(DataGridViewRow row, DataGridViewRowHeaderCell value)
        {
            row.HeaderCell = new DataGridViewRowHeaderCell();
            row.HeaderCell = value;
            if (value != null)
            {
                Assert.Same(value, row.HeaderCell);
            }
            else
            {
                Assert.NotNull(row.HeaderCell);
            }

            // Set same.
            row.HeaderCell = value;
            if (value != null)
            {
                Assert.Same(value, row.HeaderCell);
            }
            else
            {
                Assert.NotNull(row.HeaderCell);
            }
        }

        [Fact]
        public void DataGridViewRow_HeaderCell_SetWithDataGridView_CallsRowHeaderCellChanged()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Same(row, e.Row);
            };
            dataGridView.RowHeaderCellChanged += handler;

            // Set non-null.
            var cell1 = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell1;
            Assert.Same(cell1, row.HeaderCell);
            Assert.Equal(1, callCount);

            // Set same.
            row.HeaderCell = cell1;
            Assert.Same(cell1, row.HeaderCell);
            Assert.Equal(1, callCount);

            // Set different.
            var cell2 = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell2;
            Assert.Same(cell2, row.HeaderCell);
            Assert.Equal(2, callCount);

            // Set null.
            row.HeaderCell = null;
            Assert.NotNull(row.HeaderCell);
            Assert.Equal(3, callCount);

            // Remove handler.
            dataGridView.RowHeaderCellChanged -= handler;
            row.HeaderCell = cell1;
            Assert.Equal(cell1, row.HeaderCell);
            Assert.Equal(3, callCount);
        }

        public static IEnumerable<object[]> Height_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), Control.DefaultFont.Height + 9 };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], Control.DefaultFont.Height + 9 };
            yield return new object[] { dataGridView.Rows.SharedRow(1), Control.DefaultFont.Height + 9 };
        }

        [Theory]
        [MemberData(nameof(Height_Get_TestData))]
        public void DataGridViewRow_Height_Get_ReturnsExpected(DataGridViewRow row, int expected)
        {
            Assert.Equal(expected, row.Height);
        }

        public static IEnumerable<object[]> Height_NeedsHeightInfo_TestData()
        {
            yield return new object[] { new DataGridView { ColumnCount = 1 }, 5, 0, 5 };
            yield return new object[]
            {
                new DataGridView
                {
                    ColumnCount = 1,
                    AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
                },
                25, 0, 25
            };
            yield return new object[] { new DataGridView { ColumnCount = 1, VirtualMode = true }, 6, 1, 5 };

            var bound = new DataGridView { DataSource = new[] { new { Name = "Name" } } };
            new Form().Controls.Add(bound);
            Assert.NotNull(bound.BindingContext);
            yield return new object[] { bound, 6, 1, 5 };
        }

        [Theory]
        [MemberData(nameof(Height_NeedsHeightInfo_TestData))]
        public void DataGridViewRow_Height_NeedsHeightInfo_CallsRowHeightInfoNeeded(DataGridView dataGridView, int expectedHeight, int expectedCallCount, int expectedOriginalHeight)
        {
            DataGridViewRow row = dataGridView.Rows[0];
            row.Height = 5;

            int callCount = 0;
            DataGridViewRowHeightInfoNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal(5, e.Height);
                Assert.Equal(3, e.MinimumHeight);
                e.Height = 6;
            };
            dataGridView.RowHeightInfoNeeded += handler;

            Assert.Equal(expectedHeight, row.Height);
            Assert.Equal(expectedCallCount, callCount);

            // Remove the handler.
            dataGridView.RowHeightInfoNeeded -= handler;
            Assert.Equal(expectedOriginalHeight, row.Height);
            Assert.Equal(expectedCallCount, callCount);
        }

        public static IEnumerable<object[]> Height_Set_TestData()
        {
            yield return new object[] { new DataGridViewRow(), -1, 3 };
            yield return new object[] { new DataGridViewRow(), 0, 3 };
            yield return new object[] { new DataGridViewRow(), 1, 3 };
            yield return new object[] { new DataGridViewRow(), 3, 3 };
            yield return new object[] { new DataGridViewRow(), 4, 4 };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], 65536, 65536 };

            var customAutoSizeRowsModeDataGridView = new DataGridView
            {
                ColumnCount = 1,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            };
            customAutoSizeRowsModeDataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { customAutoSizeRowsModeDataGridView.Rows[0], 5, Control.DefaultFont.Height + 9 };
        }

        [Theory]
        [MemberData(nameof(Height_Set_TestData))]
        public void DataGridViewRow_Height_Set_GetReturnsExpected(DataGridViewRow row, int value, int expected)
        {
            row.Height = value;
            Assert.Equal(expected, row.Height);

            // Set same.
            row.Height = value;
            Assert.Equal(expected, row.Height);
        }

        [Fact]
        public void DataGridViewRow_Height_SetWithDataGridView_CallsRowHeightChanged()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Same(row, e.Row);
            };
            dataGridView.RowHeightChanged += handler;

            // Set non-zero.
            row.Height = 4;
            Assert.Equal(4, row.Height);
            Assert.Equal(1, callCount);

            // Set same.
            row.Height = 4;
            Assert.Equal(4, row.Height);
            Assert.Equal(1, callCount);

            // Set different.
            row.Height = 3;
            Assert.Equal(3, row.Height);
            Assert.Equal(2, callCount);

            // Remove handler.
            dataGridView.RowHeightChanged -= handler;
            row.Height = 4;
            Assert.Equal(4, row.Height);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [InlineData(65537)]
        public void DataGridViewRow_Height_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            var row = new DataGridViewRow();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => row.Height = value);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_Height_SetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.Height = -1);
        }

        public static IEnumerable<object[]> InheritedStyle_Get_TestData()
        {
            DataGridViewRow CreateRow(int index, DataGridViewCellStyle rowDefaultCellStyle, DataGridViewCellStyle rowsDefaultCellStyle, DataGridViewCellStyle alternatingRowsDefaultCellStyle, DataGridViewCellStyle gridDefaultCellStyle)
            {
                var dataGridView = new DataGridView
                {
                    ColumnCount = 2
                };

                if (rowsDefaultCellStyle != null)
                {
                    dataGridView.RowsDefaultCellStyle = rowsDefaultCellStyle;
                }
                if (alternatingRowsDefaultCellStyle != null)
                {
                    dataGridView.AlternatingRowsDefaultCellStyle = alternatingRowsDefaultCellStyle;
                }
                if (gridDefaultCellStyle != null)
                {
                    dataGridView.DefaultCellStyle = gridDefaultCellStyle;
                };

                dataGridView.Rows.Add(new SubDataGridViewRow());
                dataGridView.Rows.Add(new SubDataGridViewRow());

                DataGridViewRow row = dataGridView.Rows[index];
                if (rowDefaultCellStyle != null)
                {
                    row.DefaultCellStyle = rowDefaultCellStyle;
                }

                return row;
            }
            Font font1 = SystemFonts.DefaultFont;
            Font font2 = SystemFonts.MenuFont;
            var provider1 = new NumberFormatInfo();
            var provider2 = new NumberFormatInfo();

            var complete1 = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.BottomCenter,
                BackColor = Color.Green,
                DataSourceNullValue = "dbNull",
                Font = font1,
                ForeColor = Color.Blue,
                Format = "format",
                FormatProvider = provider1,
                Padding = new Padding(1, 2, 3, 4),
                NullValue = "null",
                SelectionForeColor = Color.Red,
                SelectionBackColor = Color.Yellow,
                Tag = "tag",
                WrapMode = DataGridViewTriState.True,
            };
            var complete2 = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.BottomRight,
                BackColor = Color.Blue,
                DataSourceNullValue = "dbNull2",
                Font = font2,
                ForeColor = Color.Green,
                Format = "forma2t",
                FormatProvider = provider2,
                Padding = new Padding(2, 3, 4, 5),
                NullValue = "null2",
                SelectionForeColor = Color.Yellow,
                SelectionBackColor = Color.Red,
                Tag = "tag2",
                WrapMode = DataGridViewTriState.False,
            };
            yield return new object[]
            {
                CreateRow(1, complete1, null, null, null),
                complete1
            };
            yield return new object[]
            {
                CreateRow(1, new DataGridViewCellStyle(), complete1, null, null),
                complete1
            };
            yield return new object[]
            {
                CreateRow(1, null, null, complete1, null),
                complete1
            };
            yield return new object[]
            {
                CreateRow(1, null, new DataGridViewCellStyle(), complete1, null),
                complete1
            };
            yield return new object[]
            {
                CreateRow(1, null, null, null, complete1),
                complete1
            };
            yield return new object[]
            {
                CreateRow(1, null, null, new DataGridViewCellStyle(), complete1),
                complete1
            };
            yield return new object[]
            {
                CreateRow(2, null, null, null, complete1),
                complete1
            };
            yield return new object[]
            {
                CreateRow(2, null, null, new DataGridViewCellStyle(), complete1),
                complete1
            };
            yield return new object[]
            {
                CreateRow(1, null, complete1, complete2, null),
                complete2
            };
            yield return new object[]
            {
                CreateRow(2, null, complete1, complete2, null),
                complete1
            };
        }

        [Theory]
        [MemberData(nameof(InheritedStyle_Get_TestData))]
        public void DataGridViewRow_InheritedStyle_Get_ReturnsExpected(DataGridViewRow row, DataGridViewCellStyle expected)
        {
            Assert.Equal(expected, row.InheritedStyle);
        }

        [Fact]
        public void DataGridViewRow_InheritedStyle_GetWithoutDataGridView_ThrowsInvalidOperationException()
        {
            var row = new DataGridViewRow();
            Assert.Throws<InvalidOperationException>(() => row.InheritedStyle);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_InheritedStyle_GetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.InheritedStyle);
        }

        public static IEnumerable<object[]> IsNewRow_TestData()
        {
            yield return new object[] { new DataGridViewRow(), false };
            var dataGridView = new DataGridView();
            dataGridView.Columns.Add("Column", "Text");
            dataGridView.Rows.Add(new SubDataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], false };
            yield return new object[] { dataGridView.Rows.SharedRow(1), false };
        }

        [Theory]
        [MemberData(nameof(IsNewRow_TestData))]
        public void DataGridViewRow_IsNewRow_Get_ReturnsExpected(DataGridViewRow row, bool expected)
        {
            Assert.Equal(expected, row.IsNewRow);
        }

        public static IEnumerable<object[]> MinimumHeight_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), 3 };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], 3 };
            yield return new object[] { dataGridView.Rows.SharedRow(1), 3 };
        }

        [Theory]
        [MemberData(nameof(MinimumHeight_Get_TestData))]
        public void DataGridViewRow_MinimumHeight_Get_ReturnsExpected(DataGridViewRow row, int expected)
        {
            Assert.Equal(expected, row.MinimumHeight);
        }

        public static IEnumerable<object[]> MinimumHeight_NeedsHeightInfo_TestData()
        {
            yield return new object[] { new DataGridView { ColumnCount = 1 }, 5, 0 };
            yield return new object[]
            {
                new DataGridView
                {
                    ColumnCount = 1,
                    AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
                },
                5, 0
            };
            yield return new object[] { new DataGridView { ColumnCount = 1, VirtualMode = true }, 6, 1 };

            var bound = new DataGridView { DataSource = new[] { new { Name = "Name" } } };
            new Form().Controls.Add(bound);
            Assert.NotNull(bound.BindingContext);
            yield return new object[] { bound, 6, 1 };
        }

        [Theory]
        [MemberData(nameof(MinimumHeight_NeedsHeightInfo_TestData))]
        public void DataGridViewRow_MinimumHeight_NeedsHeightInfo_CallsRowHeightInfoNeeded(DataGridView dataGridView, int expectedMinimumHeight, int expectedCallCount)
        {
            DataGridViewRow row = dataGridView.Rows[0];
            row.MinimumHeight = 5;

            int callCount = 0;
            DataGridViewRowHeightInfoNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal(Control.DefaultFont.Height + 9, e.Height);
                Assert.Equal(5, e.MinimumHeight);
                e.MinimumHeight = 6;
            };
            dataGridView.RowHeightInfoNeeded += handler;

            Assert.Equal(expectedMinimumHeight, row.MinimumHeight);
            Assert.Equal(expectedCallCount, callCount);

            // Remove the handler.
            dataGridView.RowHeightInfoNeeded -= handler;
            Assert.Equal(5, row.MinimumHeight);
            Assert.Equal(expectedCallCount, callCount);
        }

        public static IEnumerable<object[]> MinimumHeight_Set_TestData()
        {
            yield return new object[] { new DataGridViewRow(), 2, 10 };
            yield return new object[] { new DataGridViewRow(), 3, 10 };
            yield return new object[] { new DataGridViewRow(), 10, 10 };
            yield return new object[] { new DataGridViewRow(), 11, 11 };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], 65536, 65536 };
        }

        [Theory]
        [MemberData(nameof(MinimumHeight_Set_TestData))]
        public void DataGridViewRow_MinimumHeight_Set_GetReturnsExpected(DataGridViewRow row, int value, int expectedHeight)
        {
            row.Height = 10;
            row.MinimumHeight = value;
            Assert.Equal(value, row.MinimumHeight);
            Assert.Equal(expectedHeight, row.Height);

            // Set same.
            row.MinimumHeight = value;
            Assert.Equal(value, row.MinimumHeight);
            Assert.Equal(expectedHeight, row.Height);
        }

        [Fact]
        public void DataGridViewRow_MinimumHeight_SetWithDataGridView_CallsRowMinimumHeightChanged()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Same(row, e.Row);
            };
            dataGridView.RowMinimumHeightChanged += handler;

            // Set non-zero.
            row.MinimumHeight = 4;
            Assert.Equal(4, row.MinimumHeight);
            Assert.Equal(1, callCount);

            // Set same.
            row.MinimumHeight = 4;
            Assert.Equal(4, row.MinimumHeight);
            Assert.Equal(1, callCount);

            // Set different.
            row.MinimumHeight = 3;
            Assert.Equal(3, row.MinimumHeight);
            Assert.Equal(2, callCount);

            // Remove handler.
            dataGridView.RowMinimumHeightChanged -= handler;
            row.MinimumHeight = 4;
            Assert.Equal(4, row.MinimumHeight);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(65537)]
        public void DataGridViewRow_MinimumHeight_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            var row = new DataGridViewRow();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => row.MinimumHeight = value);
            Assert.Equal(3, row.MinimumHeight);
            Assert.Equal(Control.DefaultFont.Height + 9, row.Height);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_MinimumHeight_SetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.MinimumHeight = -1);
        }

        public static IEnumerable<object[]> ReadOnly_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), false };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], false };

            var readOnlyDataGridView = new DataGridView { ColumnCount = 1, ReadOnly = true };
            readOnlyDataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { readOnlyDataGridView.Rows[0], true };
        }

        [Theory]
        [MemberData(nameof(ReadOnly_Get_TestData))]
        public void DataGridViewRow_ReadOnly_Get_ReturnsExpected(DataGridViewRow row, bool expected)
        {
            Assert.Equal(expected, row.ReadOnly);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_ReadOnly_GetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.ReadOnly);
        }

        public static IEnumerable<object[]> ReadOnly_Set_TestData()
        {
            yield return new object[] { new DataGridViewRow() };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0] };

            DataGridViewRow secondRow = dataGridView.Rows[1];
            secondRow.Cells[0].ReadOnly = true;
            yield return new object[] { secondRow };
        }

        [Theory]
        [MemberData(nameof(ReadOnly_Set_TestData))]
        public void DataGridViewRow_ReadOnly_Set_GetReturnsExpected(DataGridViewRow row)
        {
            // Set true.
            row.ReadOnly = true;
            Assert.True(row.ReadOnly);

            // Set same.
            row.ReadOnly = true;
            Assert.True(row.ReadOnly);

            // Set different.
            row.ReadOnly = false;
            Assert.False(row.ReadOnly);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewRow_ReadOnly_SetParentReadOnly_Nop(bool value)
        {
            var dataGridView = new DataGridView { ColumnCount = 1, ReadOnly = true };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];
            row.ReadOnly = value;
            Assert.True(dataGridView.ReadOnly);
        }

        [Fact]
        public void DataGridViewRow_ReadOnly_SetWithDataGridView_CallsRowStateChanged()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowStateChangedEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Same(row, e.Row);
            };
            dataGridView.RowStateChanged += handler;

            // Set true.
            row.ReadOnly = true;
            Assert.True(row.ReadOnly);
            Assert.Equal(1, callCount);

            // Set same.
            row.ReadOnly = true;
            Assert.True(row.ReadOnly);
            Assert.Equal(1, callCount);

            // Set different.
            row.ReadOnly = false;
            Assert.False(row.ReadOnly);
            Assert.Equal(2, callCount);

            // Remove handler.
            dataGridView.RowStateChanged -= handler;
            row.ReadOnly = true;
            Assert.True(row.ReadOnly);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_ReadOnly_SetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.ReadOnly = true);
        }

        public static IEnumerable<object[]> Resizable_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), DataGridViewTriState.NotSet };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], DataGridViewTriState.True };

            var nonResizableDataGridView = new DataGridView { ColumnCount = 1, AllowUserToResizeRows = false };
            nonResizableDataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { nonResizableDataGridView.Rows[0], DataGridViewTriState.False };
        }

        [Theory]
        [MemberData(nameof(Resizable_Get_TestData))]
        public void DataGridViewRow_Resizable_Get_ReturnsExpected(DataGridViewRow row, DataGridViewTriState expected)
        {
            Assert.Equal(expected, row.Resizable);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_Resizable_GetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.Resizable);
        }

        public static IEnumerable<object[]> Resizable_Set_TestData()
        {
            yield return new object[] { new DataGridViewRow(), DataGridViewTriState.NotSet, DataGridViewTriState.NotSet };
            yield return new object[] { new DataGridViewRow(), DataGridViewTriState.True, DataGridViewTriState.True };
            yield return new object[] { new DataGridViewRow(), DataGridViewTriState.False, DataGridViewTriState.False };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], DataGridViewTriState.True, DataGridViewTriState.True };
        }

        [Theory]
        [MemberData(nameof(Resizable_Set_TestData))]
        public void DataGridViewRow_Resizable_Set_GetReturnsExpected(DataGridViewRow row, DataGridViewTriState value, DataGridViewTriState expected)
        {
            row.Resizable = value;
            Assert.Equal(expected, row.Resizable);

            // Set same.
            row.Resizable = value;
            Assert.Equal(expected, row.Resizable);
        }

        [Theory]
        [MemberData(nameof(Resizable_Set_TestData))]
        public void DataGridViewRow_Resizable_SetWithNonEmptyOldValue_Success(DataGridViewRow row, DataGridViewTriState value, DataGridViewTriState expected)
        {
            row.Resizable = DataGridViewTriState.True;
            row.Resizable = value;
            Assert.Equal(expected, row.Resizable);
        }

        [Fact]
        public void DataGridViewRow_Resizable_SetWithDataGridView_CallsRowStateChanged()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowStateChangedEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Same(row, e.Row);
            };
            dataGridView.RowStateChanged += handler;

            // Set false.
            row.Resizable = DataGridViewTriState.False;
            Assert.Equal(DataGridViewTriState.False, row.Resizable);
            Assert.Equal(1, callCount);

            // Set same.
            row.Resizable = DataGridViewTriState.False;
            Assert.Equal(DataGridViewTriState.False, row.Resizable);
            Assert.Equal(1, callCount);

            // Set different.
            row.Resizable = DataGridViewTriState.True;
            Assert.Equal(DataGridViewTriState.True, row.Resizable);
            Assert.Equal(2, callCount);

            // Remove handler.
            dataGridView.RowStateChanged -= handler;
            row.Resizable = DataGridViewTriState.False;
            Assert.Equal(DataGridViewTriState.False, row.Resizable);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DataGridViewTriState))]
        public void DataGridViewRow_Resizable_SetInvalid_ThrowsInvalidEnumArgumentException(DataGridViewTriState value)
        {
            var row = new DataGridViewRow();
            Assert.Throws<InvalidEnumArgumentException>(() => row.Resizable = value);
        }

        public static IEnumerable<object[]> Selected_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), false };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], false };
        }

        [Theory]
        [MemberData(nameof(Selected_Get_TestData))]
        public void DataGridViewRow_Selected_Get_ReturnsExpected(DataGridViewRow row, bool expected)
        {
            Assert.Equal(expected, row.Selected);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_Selected_GetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.Selected);
        }

        [Fact]
        public void DataGridViewRow_Selected_SetWithoutDataGridView_ThrowsInvalidOperationException()
        {
            var row = new DataGridViewRow();
            Assert.Throws<InvalidOperationException>(() => row.Selected = true);
            Assert.False(row.Selected);

            row.Selected = false;
            Assert.False(row.Selected);
        }

        [Theory]
        [InlineData(DataGridViewSelectionMode.CellSelect, false)]
        [InlineData(DataGridViewSelectionMode.FullRowSelect, true)]
        [InlineData(DataGridViewSelectionMode.FullColumnSelect, false)]
        [InlineData(DataGridViewSelectionMode.RowHeaderSelect, true)]
        [InlineData(DataGridViewSelectionMode.ColumnHeaderSelect, false)]
        public void DataGridViewRow_Selected_SetWithDataGridView_GetReturnsExpected(DataGridViewSelectionMode selectionMode, bool selected)
        {
            var dataGridView = new DataGridView
            {
                ColumnCount = 1
            };
            dataGridView.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView.SelectionMode = selectionMode;
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];

            // Set true.
            row.Selected = true;
            Assert.Equal(selected, (dataGridView.SelectedRows).Contains(row));
            Assert.Equal(selected, row.Selected);

            // Set same.
            row.Selected = true;
            Assert.Equal(selected, (dataGridView.SelectedRows).Contains(row));
            Assert.Equal(selected, row.Selected);

            // Set different.
            row.Selected = false;
            Assert.False((dataGridView.SelectedRows).Contains(row));
            Assert.False(row.Selected);
        }

        [Fact]
        public void DataGridViewRow_Selected_SetMultipleNotMultiSelect_Success()
        {
            var dataGridView = new DataGridView
            {
                ColumnCount = 1,
                MultiSelect = false
            };
            dataGridView.Rows.Add(new DataGridViewRow());
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row1 = dataGridView.Rows[0];
            DataGridViewRow row2 = dataGridView.Rows[1];

            row1.Selected = true;
            Assert.Same(row1, Assert.Single(dataGridView.SelectedRows));
            Assert.True(row1.Selected);
            Assert.False(row2.Selected);

            row2.Selected = true;
            Assert.Same(row2, Assert.Single(dataGridView.SelectedRows));
            Assert.False(row1.Selected);
            Assert.True(row2.Selected);

            row1.Selected = false;
            Assert.Empty(dataGridView.SelectedRows);
            Assert.False(row1.Selected);
            Assert.False(row2.Selected);

            row2.Selected = false;
            Assert.Empty(dataGridView.SelectedRows);
            Assert.False(row1.Selected);
            Assert.False(row2.Selected);
        }

        [Fact]
        public void DataGridViewRow_Selected_SetMultipleMultiSelect_Success()
        {
            var dataGridView = new DataGridView
            {
                ColumnCount = 1,
                MultiSelect = true
            };
            dataGridView.Rows.Add(new DataGridViewRow());
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row1 = dataGridView.Rows[0];
            DataGridViewRow row2 = dataGridView.Rows[1];

            row1.Selected = true;
            Assert.Same(row1, Assert.Single(dataGridView.SelectedRows));
            Assert.True(row1.Selected);
            Assert.False(row2.Selected);

            row2.Selected = true;
            Assert.Equal(new DataGridViewRow[] { row2, row1 }, dataGridView.SelectedRows.Cast<DataGridViewRow>());
            Assert.True(row1.Selected);
            Assert.True(row2.Selected);

            row1.Selected = false;
            Assert.Same(row2, Assert.Single(dataGridView.SelectedRows));
            Assert.False(row1.Selected);
            Assert.True(row2.Selected);

            row2.Selected = false;
            Assert.Empty(dataGridView.SelectedRows);
            Assert.False(row1.Selected);
            Assert.False(row2.Selected);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_Selected_SetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.Selected = true);
            Assert.Throws<InvalidOperationException>(() => row.Selected = false);
        }

        public static IEnumerable<object[]> State_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), DataGridViewElementStates.Visible };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], DataGridViewElementStates.Visible };
        }

        [Theory]
        [MemberData(nameof(State_Get_TestData))]
        public void DataGridViewRow_State_Get_ReturnsExpected(DataGridViewRow row, DataGridViewElementStates expected)
        {
            Assert.Equal(expected, row.State);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_State_GetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.State);
        }

        public static IEnumerable<object[]> Tag_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), null };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], null };
            yield return new object[] { dataGridView.Rows.SharedRow(1), null };
        }

        [Theory]
        [MemberData(nameof(Tag_Get_TestData))]
        public void DataGridViewRow_Tag_Get_ReturnsExpected(DataGridViewRow row, object expected)
        {
            Assert.Equal(expected, row.Tag);
        }

        public static IEnumerable<object[]> Tag_Set_TestData()
        {
            yield return new object[] { new DataGridViewRow(), null };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], "" };
            yield return new object[] { dataGridView.Rows.SharedRow(1), "value" };
        }

        [Theory]
        [MemberData(nameof(Tag_Set_TestData))]
        public void DataGridViewRow_Tag_Set_GetReturnsExpected(DataGridViewRow row, object value)
        {
            row.Tag = value;
            Assert.Equal(value, row.Tag);

            // Set same.
            row.Tag = value;
            Assert.Equal(value, row.Tag);
        }

        [Theory]
        [MemberData(nameof(Tag_Set_TestData))]
        public void DataGridViewRow_Tag_SetWithNonNullOldValue_GetReturnsExpected(DataGridViewRow row, object value)
        {
            row.Tag = "tag";
            row.Tag = value;
            Assert.Equal(value, row.Tag);

            // Set same.
            row.Tag = value;
            Assert.Equal(value, row.Tag);
        }

        public static IEnumerable<object[]> Visible_Get_TestData()
        {
            yield return new object[] { new DataGridViewRow(), true };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], true };
        }

        [Theory]
        [MemberData(nameof(Visible_Get_TestData))]
        public void DataGridViewRow_Visible_Get_ReturnsExpected(DataGridViewRow row, bool expected)
        {
            Assert.Equal(expected, row.Visible);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_Visible_GetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.Visible);
        }

        public static IEnumerable<object[]> Visible_Set_TestData()
        {
            yield return new object[] { new DataGridViewRow() };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0] };
        }

        [Theory]
        [MemberData(nameof(Visible_Set_TestData))]
        public void DataGridViewRow_Visible_Set_GetReturnsExpected(DataGridViewRow row)
        {
            // Set false.
            row.Visible = false;
            Assert.False(row.Visible);

            // Set same.
            row.Visible = false;
            Assert.False(row.Visible);

            // Set different.
            row.Visible = true;
            Assert.True(row.Visible);
        }

        [Fact]
        public void DataGridViewRow_Visible_SetWithDataGridView_CallsRowStateChanged()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowStateChangedEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Same(row, e.Row);
            };
            dataGridView.RowStateChanged += handler;

            // Set false.
            row.Visible = false;
            Assert.False(row.Visible);
            Assert.Equal(1, callCount);

            // Set same.
            row.Visible = false;
            Assert.False(row.Visible);
            Assert.Equal(1, callCount);

            // Set different.
            row.Visible = true;
            Assert.True(row.Visible);
            Assert.Equal(2, callCount);

            // Remove handler.
            dataGridView.RowStateChanged -= handler;
            row.Visible = true;
            Assert.True(row.Visible);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DataGridViewRow_Visible_SetNewRowIndexDifferent_ThrowsInvalidOperationException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];

            Assert.Throws<InvalidOperationException>(() => row.Visible = false);
            Assert.True(row.Visible);

            row.Visible = true;
            Assert.True(row.Visible);
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_Visible_SetShared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.Visible = true);
            Assert.Throws<InvalidOperationException>(() => row.Visible = false);
        }

        public static IEnumerable<object[]> AdjustRowHeaderBorderStyle_TestData()
        {
            DataGridViewRow GetRow(bool enableHeadersVisualStyles, bool columnHeadersVisible, RightToLeft rightToLeft = RightToLeft.No)
            {
                var dataGridView = new DataGridView
                {
                    ColumnCount = 1,
                    EnableHeadersVisualStyles = enableHeadersVisualStyles,
                    ColumnHeadersVisible = columnHeadersVisible,
                    RightToLeft = rightToLeft
                };
                dataGridView.Rows.Add(new SubDataGridViewRow());
                return dataGridView.Rows[0];
            }

            // Inset.
            foreach (bool isFirstDisplayedRow in new bool[] { true, false })
            {
                yield return new object[]
                {
                    GetRow(true, true), DataGridViewAdvancedCellBorderStyle.Inset, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
            }
            yield return new object[]
            {
                GetRow(false, true), DataGridViewAdvancedCellBorderStyle.Inset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                GetRow(false, true), DataGridViewAdvancedCellBorderStyle.Inset, true, true, false, true,
                false, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                GetRow(true, false), DataGridViewAdvancedCellBorderStyle.Inset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                GetRow(true, false), DataGridViewAdvancedCellBorderStyle.Inset, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                GetRow(false, false), DataGridViewAdvancedCellBorderStyle.Inset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                GetRow(false, false), DataGridViewAdvancedCellBorderStyle.Inset, true, true, false, true,
                false, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.Inset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.Inset, true, true, false, true,
                false, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.Inset, true, false, true, true,
                false, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };

            // InsetDouble.
            foreach (bool isFirstDisplayedRow in new bool[] { true, false })
            {
                yield return new object[]
                {
                    GetRow(true, true), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
                yield return new object[]
                {
                    GetRow(true, true, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
                yield return new object[]
                {
                    GetRow(false, true), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
                };
                yield return new object[]
                {
                    GetRow(false, true, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
                };
            }
            yield return new object[]
            {
                GetRow(true, false), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                GetRow(true, false, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                GetRow(false, false), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                GetRow(false, false, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                GetRow(false, false), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                GetRow(false, false, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, false, false, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };

            // Outset.
            foreach (bool isFirstDisplayedRow in new bool[] { true, false })
            {
                yield return new object[]
                {
                    GetRow(true, true), DataGridViewAdvancedCellBorderStyle.Outset, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
            }
            yield return new object[]
            {
                GetRow(false, true), DataGridViewAdvancedCellBorderStyle.Outset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                GetRow(false, true), DataGridViewAdvancedCellBorderStyle.Outset, true, true, false, true,
                false, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                GetRow(true, false), DataGridViewAdvancedCellBorderStyle.Outset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                GetRow(true, false), DataGridViewAdvancedCellBorderStyle.Outset, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                GetRow(false, false), DataGridViewAdvancedCellBorderStyle.Outset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                GetRow(false, false), DataGridViewAdvancedCellBorderStyle.Outset, true, true, false, true,
                false, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.Outset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.Outset, true, true, false, true,
                false, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.Outset, true, false, true, true,
                false, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };

            // OutsetDouble.
            foreach (bool isFirstDisplayedRow in new bool[] { true, false })
            {
                yield return new object[]
                {
                    GetRow(true, true), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
                yield return new object[]
                {
                    GetRow(true, true, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
                yield return new object[]
                {
                    GetRow(false, true), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
                };
                yield return new object[]
                {
                    GetRow(false, true, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
                };
            }
            yield return new object[]
            {
                GetRow(true, false), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                GetRow(true, false, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                GetRow(false, false), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                GetRow(false, false, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                GetRow(false, false), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                GetRow(false, false, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, false, false, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };

            // OutsetPartial.
            foreach (bool isFirstDisplayedRow in new bool[] { true, false })
            {
                foreach (bool isLastVisibleRow in new bool[] { true, false })
                {
                    yield return new object[]
                    {
                        GetRow(true, true), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, isFirstDisplayedRow, isLastVisibleRow,
                        true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                    };
                    yield return new object[]
                    {
                        GetRow(true, true, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, isFirstDisplayedRow, isLastVisibleRow,
                        true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                    };
                }
            }
            foreach (bool isLastVisibleRow in new bool[] { true, false })
            {
                yield return new object[]
                {
                    GetRow(true, false), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, isLastVisibleRow,
                    true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
                yield return new object[]
                {
                    GetRow(true, false, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, isLastVisibleRow,
                    true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.None
                };
            }
            yield return new object[]
            {
                GetRow(false, true), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                GetRow(false, true, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                GetRow(false, true), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, false,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };
            yield return new object[]
            {
                GetRow(false, true, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, false,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };
            yield return new object[]
            {
                GetRow(false, true), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                GetRow(false, true, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                GetRow(false, true), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, false,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };
            yield return new object[]
            {
                GetRow(false, true, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, false,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };
            yield return new object[]
            {
                GetRow(false, false), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                GetRow(false, false, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                GetRow(false, false), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, false,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };
            yield return new object[]
            {
                GetRow(false, false, RightToLeft.Yes), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, false,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, false, false, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, false, false, false,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };

            // Single.
            foreach (bool isFirstDisplayedRow in new bool[] { true, false })
            {
                yield return new object[]
                {
                    GetRow(true, true), DataGridViewAdvancedCellBorderStyle.Single, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
                yield return new object[]
                {
                    GetRow(false, true), DataGridViewAdvancedCellBorderStyle.Single, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single
                };
            }
            yield return new object[]
            {
                GetRow(true, false), DataGridViewAdvancedCellBorderStyle.Single, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                GetRow(true, false), DataGridViewAdvancedCellBorderStyle.Single, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                GetRow(false, false), DataGridViewAdvancedCellBorderStyle.Single, true, true, true, true,
                false, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single
            };
            yield return new object[]
            {
                GetRow(false, false), DataGridViewAdvancedCellBorderStyle.Single, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.Single, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single
            };

            // None.
            yield return new object[]
            {
                GetRow(true, true), DataGridViewAdvancedCellBorderStyle.None, true, true, true, true,
                false, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                GetRow(true, false), DataGridViewAdvancedCellBorderStyle.None, true, true, true, true,
                false, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };

            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.None, true, true, true, true,
                false, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.Single, true, true, true, true,
                false, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.Single, true, false, true, true,
                false, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.InsetDouble, true, false, true, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, false, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                new DataGridViewRow(), DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, false, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
        }

        [Theory]
        [MemberData(nameof(AdjustRowHeaderBorderStyle_TestData))]
        public void DataGridViewRow_AdjustRowHeaderBorderStyle_Invoke_ReturnsExpected(DataGridViewRow row, DataGridViewAdvancedCellBorderStyle all, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow, bool returnsAdvanced, DataGridViewAdvancedCellBorderStyle expectedLeft, DataGridViewAdvancedCellBorderStyle expectedRight, DataGridViewAdvancedCellBorderStyle expectedTop, DataGridViewAdvancedCellBorderStyle expectedBottom)
        {
            if (!Application.RenderWithVisualStyles && row.DataGridView != null && row.DataGridView.EnableHeadersVisualStyles)
            {
                // Not supported.
                return;
            }

            var dataGridViewAdvancedBorderStyleInput = new DataGridViewAdvancedBorderStyle
            {
                All = all
            };
            var dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle();
            DataGridViewAdvancedBorderStyle result = row.AdjustRowHeaderBorderStyle(dataGridViewAdvancedBorderStyleInput, dataGridViewAdvancedBorderStylePlaceholder, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
            Assert.Same(returnsAdvanced ? dataGridViewAdvancedBorderStylePlaceholder : dataGridViewAdvancedBorderStyleInput, result);

            Assert.Equal(expectedLeft, result.Left);
            Assert.Equal(expectedRight, result.Right);
            Assert.Equal(expectedTop, result.Top);
            Assert.Equal(expectedBottom, result.Bottom);
        }

        [Fact]
        public void DataGridViewRow_Clone_Empty_Success()
        {
            var source = new DataGridViewRow();
            DataGridViewRow row = Assert.IsType<DataGridViewRow>(source.Clone());
            Assert.Null(row.ContextMenuStrip);
            Assert.Null(row.DataGridView);
            Assert.NotNull(row.DefaultCellStyle);
            Assert.Equal(typeof(DataGridViewRowHeaderCell), row.DefaultHeaderCellType);
            Assert.Equal(0, row.DividerHeight);
            Assert.Empty(row.ErrorText);
            Assert.False(row.Frozen);
            Assert.NotNull(row.HeaderCell);
            Assert.Equal(Control.DefaultFont.Height + 9, row.Height);
            Assert.Equal(3, row.MinimumHeight);
            Assert.False(row.ReadOnly);
            Assert.Equal(DataGridViewTriState.NotSet, row.Resizable);
            Assert.False(row.Selected);
            Assert.True(row.Visible);
        }

        [Fact]
        public void DataGridViewRow_Clone_NoDataGridView_Success()
        {
            var menu = new ContextMenuStrip();
            var style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomRight };
            var cell = new DataGridViewRowHeaderCell();
            var source = new DataGridViewRow
            {
                ContextMenuStrip = menu,
                DefaultCellStyle = style,
                DefaultHeaderCellType = typeof(DataGridViewRowHeaderCell),
                DividerHeight = 10,
                ErrorText = "errorText",
                Frozen = true,
                HeaderCell = cell,
                Height = 5,
                MinimumHeight = 4,
                Resizable = DataGridViewTriState.True,
                Visible = false
            };
            DataGridViewRow row = Assert.IsType<DataGridViewRow>(source.Clone());
            Assert.NotNull(row.ContextMenuStrip);
            Assert.NotSame(menu, row.ContextMenuStrip);
            Assert.Null(row.DataGridView);
            Assert.Equal(style, row.DefaultCellStyle);
            Assert.NotSame(style, row.DefaultCellStyle);
            Assert.Equal(typeof(DataGridViewRowHeaderCell), row.DefaultHeaderCellType);
            Assert.Equal(10, row.DividerHeight);
            Assert.Equal("errorText", row.ErrorText);
            Assert.True(row.Frozen);
            Assert.NotNull(row.HeaderCell);
            Assert.NotSame(cell, row.HeaderCell);
            Assert.Equal(5, row.Height);
            Assert.Equal(4, row.MinimumHeight);
            Assert.False(row.ReadOnly);
            Assert.Equal(DataGridViewTriState.True, row.Resizable);
            Assert.False(row.Selected);
            Assert.False(row.Visible);
        }

        [Fact]
        public void DataGridViewRow_Clone_Subclass_Success()
        {
            var menu = new ContextMenuStrip();
            var style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomRight };
            var cell = new DataGridViewRowHeaderCell();
            var source = new SubDataGridViewRow
            {
                ContextMenuStrip = menu,
                DefaultCellStyle = style,
                DefaultHeaderCellType = typeof(DataGridViewRowHeaderCell),
                DividerHeight = 10,
                ErrorText = "errorText",
                Frozen = true,
                HeaderCell = cell,
                Height = 5,
                MinimumHeight = 4,
                Resizable = DataGridViewTriState.True,
                Visible = false
            };
            SubDataGridViewRow row = Assert.IsType<SubDataGridViewRow>(source.Clone());
            Assert.NotNull(row.ContextMenuStrip);
            Assert.NotSame(menu, row.ContextMenuStrip);
            Assert.Null(row.DataGridView);
            Assert.Equal(style, row.DefaultCellStyle);
            Assert.NotSame(style, row.DefaultCellStyle);
            Assert.Equal(typeof(DataGridViewRowHeaderCell), row.DefaultHeaderCellType);
            Assert.Equal(10, row.DividerHeight);
            Assert.Equal("errorText", row.ErrorText);
            Assert.True(row.Frozen);
            Assert.NotNull(row.HeaderCell);
            Assert.NotSame(cell, row.HeaderCell);
            Assert.Equal(5, row.Height);
            Assert.Equal(4, row.MinimumHeight);
            Assert.False(row.ReadOnly);
            Assert.Equal(DataGridViewTriState.True, row.Resizable);
            Assert.False(row.Selected);
            Assert.False(row.Visible);
        }

        [Fact]
        public void DataGridViewRow_Clone_WithDataGridView_Success()
        {
            var menu = new ContextMenuStrip();
            var style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomRight };
            var cell = new DataGridViewRowHeaderCell();

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow source = dataGridView.Rows[0];
            source.ContextMenuStrip = menu;
            source.DefaultCellStyle = style;
            source.DefaultHeaderCellType = typeof(DataGridViewRowHeaderCell);
            source.DividerHeight = 10;
            source.ErrorText = "errorText";
            source.Frozen = true;
            source.HeaderCell = cell;
            source.Height = 5;
            source.MinimumHeight = 4;
            source.ReadOnly = true;
            source.Resizable = DataGridViewTriState.True;
            source.Selected = true;
            source.Visible = false;

            DataGridViewRow row = Assert.IsType<DataGridViewRow>(source.Clone());
            Assert.NotNull(row.ContextMenuStrip);
            Assert.NotSame(menu, row.ContextMenuStrip);
            Assert.Null(row.DataGridView);
            Assert.Equal(style, row.DefaultCellStyle);
            Assert.NotSame(style, row.DefaultCellStyle);
            Assert.Equal(typeof(DataGridViewRowHeaderCell), row.DefaultHeaderCellType);
            Assert.Equal(10, row.DividerHeight);
            Assert.Equal("errorText", row.ErrorText);
            Assert.True(row.Frozen);
            Assert.NotNull(row.HeaderCell);
            Assert.NotSame(cell, row.HeaderCell);
            Assert.Equal(5, row.Height);
            Assert.Equal(4, row.MinimumHeight);
            Assert.True(row.ReadOnly);
            Assert.Equal(DataGridViewTriState.True, row.Resizable);
            Assert.False(row.Selected);
            Assert.False(row.Visible);
        }

        [Fact]
        public void DataGridViewRow_CreateCells_InvokeNoColumns_Success()
        {
            var dataGridView = new DataGridView();
            var row = new DataGridViewRow();
            row.Cells.Add(new SubDataGridViewCell());
            row.CreateCells(dataGridView);
            Assert.Empty(row.Cells);
        }

        [Fact]
        public void DataGridViewRow_CreateCells_InvokeWithColumns_Success()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            var row = new DataGridViewRow();
            row.Cells.Add(new SubDataGridViewCell());
            row.CreateCells(dataGridView);
            Assert.Null(Assert.Single(row.Cells.Cast<DataGridViewCell>()).Value);
        }

        [Theory]
        [InlineData(new object[] { new object[0] })]
        [InlineData(new object[] { new object[] { 1, 2, 3 } })]
        public void DataGridViewRow_CreateCells_InvokeNoColumnsWithValues_Success(object[] values)
        {
            var dataGridView = new DataGridView();
            var row = new DataGridViewRow();
            row.Cells.Add(new SubDataGridViewCell());
            row.CreateCells(dataGridView, values);
            Assert.Empty(row.Cells);
        }

        [Theory]
        [InlineData(new object[] { new object[0], null })]
        [InlineData(new object[] { new object[] { 1 }, 1 })]
        [InlineData(new object[] { new object[] { 1, 2, 3 }, 1 })]
        public void DataGridViewRow_CreateCells_InvokeWithValues_Success(object[] values, object expectedValue)
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            var row = new DataGridViewRow();
            row.Cells.Add(new SubDataGridViewCell());
            row.CreateCells(dataGridView, values);
            Assert.Equal(expectedValue, Assert.Single(row.Cells.Cast<DataGridViewCell>()).Value);
        }

        [Fact]
        public void DataGridViewRow_CreateCells_NullDataGridView_ThrowsArgumentNullException()
        {
            var row = new DataGridViewRow();
            Assert.Throws<ArgumentNullException>("dataGridView", () => row.CreateCells(null));
            Assert.Throws<ArgumentNullException>("dataGridView", () => row.CreateCells(null, Array.Empty<object>()));
        }

        [Fact]
        public void DataGridViewRow_CreateCells_HasDataGridView_ThrowsInvalidOperationException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];
            Assert.Throws<InvalidOperationException>(() => row.CreateCells(new DataGridView()));
            Assert.Throws<InvalidOperationException>(() => row.CreateCells(new DataGridView(), Array.Empty<object>()));
        }

        [Fact]
        public void DataGridViewRow_CreateCells_ColumnHasNoCellTemplate_ThrowsInvalidOperationException()
        {
            var dataGridView = new DataGridView();
            dataGridView.Columns.Add("Name", "Text");
            dataGridView.Columns[0].CellTemplate = null;
            var row = new DataGridViewRow();
            Assert.Throws<InvalidOperationException>(() => row.CreateCells(dataGridView));
            Assert.Throws<InvalidOperationException>(() => row.CreateCells(dataGridView, Array.Empty<object>()));
        }

        [Fact]
        public void DataGridViewRow_CreateCells_NullValues_ThrowsArgumentNullException()
        {
            var row = new DataGridViewRow();
            Assert.Throws<ArgumentNullException>("values", () => row.CreateCells(new DataGridView(), null));
        }

        [Fact]
        public void DataGridViewRow_CreateCellsInstance_Invoke_ReturnsEmpty()
        {
            var row = new SubDataGridViewRow();
            Assert.Empty(row.CreateCellsInstance());
        }

        [Fact]
        public void DataGridViewRow_Dispose_WithoutContextMenuStrip_Nop()
        {
            var row = new DataGridViewRow();
            row.Dispose();
            Assert.Null(row.ContextMenuStrip);

            // Call multiple times.
            row.Dispose();
            Assert.Null(row.ContextMenuStrip);
        }

        [Fact]
        public void DataGridViewRow_Dispose_WithContextMenuStrip_Success()
        {
            var row = new DataGridViewRow
            {
                ContextMenuStrip = new ContextMenuStrip()
            };
            row.Dispose();
            Assert.NotNull(row.ContextMenuStrip);

            // Call multiple times.
            row.Dispose();
            Assert.NotNull(row.ContextMenuStrip);
        }

        public static IEnumerable<object[]> DrawFocus_TestData()
        {
            yield return new object[] { Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, new DataGridViewCellStyle(), false };
            yield return new object[] { Rectangle.Empty, Rectangle.Empty, 0, DataGridViewElementStates.None, new DataGridViewCellStyle(), false };
            yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), 1, DataGridViewElementStates.Selected, new DataGridViewCellStyle { BackColor = Color.Blue, SelectionBackColor = Color.Red }, true };
            yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), 1, DataGridViewElementStates.Selected, new DataGridViewCellStyle { BackColor = Color.Blue, SelectionBackColor = Color.Red }, true };
            yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), 1, DataGridViewElementStates.None, new DataGridViewCellStyle { BackColor = Color.Blue, SelectionBackColor = Color.Red }, true };
            yield return new object[] { new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), 1, DataGridViewElementStates.None, new DataGridViewCellStyle { BackColor = Color.Blue, SelectionBackColor = Color.Red }, false };
        }

        [Theory]
        [MemberData(nameof(DrawFocus_TestData))]
        public void DataGridViewRow_DrawFocus_Invoke_Success(Rectangle clipBounds, Rectangle bounds, int rowIndex, DataGridViewElementStates rowState, DataGridViewCellStyle cellStyle, bool cellsPaintSelectionBackground)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView { ColumnCount = 1 };
                dataGridView.Rows.Add(new DataGridViewRow());
                DataGridViewRow row = dataGridView.Rows[0];
                row.DrawFocus(graphics, clipBounds, bounds, rowIndex, rowState, cellStyle, cellsPaintSelectionBackground);
            }
        }

        [Fact]
        public void DataGridViewRow_DrawFocus_NoDataGridView_ThrowsInvalidOperationException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var row = new DataGridViewRow();
                Assert.Throws<InvalidOperationException>(() => row.DrawFocus(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), -1, DataGridViewElementStates.None, new DataGridViewCellStyle(), true));
            }
        }

        [Fact]
        public void DataGridViewRow_DrawFocus_NullGraphics_ThrowsArgumentNullException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];
            Assert.Throws<ArgumentNullException>("graphics", () => row.DrawFocus(null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), -1, DataGridViewElementStates.None, new DataGridViewCellStyle(), true));
        }

        [Fact]
        public void DataGridViewRow_DrawFocus_NullCellStyle_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView { ColumnCount = 1 };
                dataGridView.Rows.Add(new DataGridViewRow());
                DataGridViewRow row = dataGridView.Rows[0];
                Assert.Throws<ArgumentNullException>("cellStyle", () => row.DrawFocus(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), -1, DataGridViewElementStates.None, null, true));
            }
        }

        public static IEnumerable<object[]> GetContextMenuStrip_TestData()
        {
            foreach (int rowIndex in new int[] { -2, -1, 0 })
            {
                var menu1 = new ContextMenuStrip();
                yield return new object[] { new DataGridViewRow(), rowIndex, null };
                yield return new object[] { new DataGridViewRow { ContextMenuStrip = menu1 }, rowIndex, menu1 };
            }

            var menu2 = new ContextMenuStrip();
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow secondRow = dataGridView.Rows[1];
            secondRow.ContextMenuStrip = menu2;
            yield return new object[] { dataGridView.Rows[0], 0, null };
            yield return new object[] { secondRow, 1, menu2 };
        }

        [Theory]
        [MemberData(nameof(GetContextMenuStrip_TestData))]
        public void DataGridViewRow_GetContextMenuStrip_Invoke_ReturnsExpected(DataGridViewRow row, int rowIndex, ContextMenuStrip expected)
        {
            Assert.Equal(expected, row.GetContextMenuStrip(rowIndex));
        }

        public static IEnumerable<object[]> GetContextMenuStrip_NeedsContextMenuStrip_TestData()
        {
            yield return new object[] { new DataGridView { ColumnCount = 1, VirtualMode = true } };

            var bound = new DataGridView { DataSource = new[] { new { Name = "Name" } } };
            new Form().Controls.Add(bound);
            Assert.NotNull(bound.BindingContext);
            yield return new object[] { bound };
        }

        [Theory]
        [MemberData(nameof(GetContextMenuStrip_NeedsContextMenuStrip_TestData))]
        public void DataGridViewRow_GetContextMenuStrip_VirtualMode_CallsRowContextMenuStripNeeded(DataGridView dataGridView)
        {
            var menu1 = new ContextMenuStrip();
            var menu2 = new ContextMenuStrip();
            DataGridViewRow row = dataGridView.Rows[0];
            row.ContextMenuStrip = menu1;

            int callCount = 0;
            DataGridViewRowContextMenuStripNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Same(menu1, e.ContextMenuStrip);
                e.ContextMenuStrip = menu2;
            };
            dataGridView.RowContextMenuStripNeeded += handler;

            Assert.Same(menu2, row.GetContextMenuStrip(0));
            Assert.Equal(1, callCount);

            // Remove the handler.
            dataGridView.RowContextMenuStripNeeded -= handler;
            Assert.Same(menu1, row.GetContextMenuStrip(0));
            Assert.Equal(1, callCount);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewRow_GetContextMenuStrip_InvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            DataGridViewRow row = dataGridView.Rows[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => row.GetContextMenuStrip(rowIndex));
        }

        [Fact]
        public void DataGridViewRow_GetContextMenuStrip_SharedRow_ThrowsInvalidOperationException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];
            Assert.Throws<InvalidOperationException>(() => row.GetContextMenuStrip(-1));
        }

        public static IEnumerable<object[]> GetErrorText_TestData()
        {
            foreach (int rowIndex in new int[] { -2, -1, 0 })
            {
                yield return new object[] { new DataGridViewRow(), rowIndex, "" };
                yield return new object[] { new DataGridViewRow { ErrorText = "errorText" }, rowIndex, "errorText" };
            }

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow secondRow = dataGridView.Rows[1];
            secondRow.ErrorText = "errorText";
            yield return new object[] { dataGridView.Rows[0], 0, "" };
            yield return new object[] { secondRow, 1, "errorText" };

            var bound = new DataGridView { DataSource = new[] { new { Name = "Name" } } };
            new Form().Controls.Add(bound);
            Assert.NotNull(bound.BindingContext);
            yield return new object[] { bound.Rows[0], 0, "" };
        }

        [Theory]
        [MemberData(nameof(GetErrorText_TestData))]
        public void DataGridViewRow_GetErrorText_Invoke_ReturnsExpected(DataGridViewRow row, int rowIndex, string expected)
        {
            Assert.Equal(expected, row.GetErrorText(rowIndex));
        }

        public static IEnumerable<object[]> GetErrorText_NeedsErrorText_TestData()
        {
            yield return new object[] { new DataGridView { ColumnCount = 1, VirtualMode = true } };

            var bound = new DataGridView { DataSource = new[] { new { Name = "Name" } } };
            new Form().Controls.Add(bound);
            Assert.NotNull(bound.BindingContext);
            yield return new object[] { bound };
        }

        [Theory]
        [MemberData(nameof(GetErrorText_NeedsErrorText_TestData))]
        public void DataGridViewRow_GetErrorText_NeedsErrorText_CallsRowErrorTextNeeded(DataGridView dataGridView)
        {
            DataGridViewRow row = dataGridView.Rows[0];
            row.ErrorText = "errorText1";

            int callCount = 0;
            DataGridViewRowErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal("errorText1", e.ErrorText);
                e.ErrorText = "errorText2";
            };
            dataGridView.RowErrorTextNeeded += handler;

            Assert.Same("errorText2", row.GetErrorText(0));
            Assert.Equal(1, callCount);

            // Remove the handler.
            dataGridView.RowErrorTextNeeded -= handler;
            Assert.Same("errorText1", row.GetErrorText(0));
            Assert.Equal(1, callCount);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewRow_GetErrorText_InvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            DataGridViewRow row = dataGridView.Rows[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => row.GetErrorText(rowIndex));
        }

        [Fact]
        public void DataGridViewRow_GetErrorText_SharedRow_ThrowsInvalidOperationException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];
            Assert.Throws<InvalidOperationException>(() => row.GetErrorText(-1));
        }

        public static IEnumerable<object[]> GetState_TestData()
        {
            yield return new object[] { new DataGridViewRow(), -1, DataGridViewElementStates.Visible };

            var dataGridView = new DataGridView();
            dataGridView.Columns.Add("Column", "Text");
            dataGridView.Rows.Add(new SubDataGridViewRow());
            yield return new object[] { dataGridView.Rows[0], 0, DataGridViewElementStates.Visible };
            yield return new object[] { dataGridView.Rows.SharedRow(1), 1, DataGridViewElementStates.Visible };
        }

        [Theory]
        [MemberData(nameof(GetState_TestData))]
        public void DataGridViewRow_GetState_Invoke_ReturnsExpected(DataGridViewRow row, int rowIndex, DataGridViewElementStates expected)
        {
            Assert.Equal(expected, row.GetState(rowIndex));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewRow_GetState_NoDataGridViewInvalidRowIndex_ThrowsArgumentException(int rowIndex)
        {
            var row = new DataGridViewRow();
            Assert.Throws<ArgumentException>("rowIndex", () => row.GetState(rowIndex));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewRow_GetState_InvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            DataGridViewRow row = dataGridView.Rows[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => row.GetState(rowIndex));
        }

        public static IEnumerable<object[]> Paint_TestData()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[1], Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All };
            yield return new object[] { dataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { dataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, false, true, DataGridViewPaintParts.None };
            yield return new object[] { dataGridView.Rows[1], new Rectangle(1000, 2000, 100, 100), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };

            var headersInvisibleDataGridView = new DataGridView { ColumnCount = 1, RowHeadersVisible = false };
            headersInvisibleDataGridView.Rows.Add(new DataGridViewRow());

            yield return new object[] { headersInvisibleDataGridView.Rows[1], Rectangle.Empty, new Rectangle(1, 2, 100, 100), -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All };

            var frozenDataGridView = new DataGridView { ColumnCount = 1 };
            frozenDataGridView.Rows.Add(new DataGridViewRow());
            frozenDataGridView.Columns[0].Frozen = true;
            frozenDataGridView.Columns[0].Visible = true;
            yield return new object[] { frozenDataGridView.Rows[1], Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All };
            yield return new object[] { frozenDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { frozenDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, false, true, DataGridViewPaintParts.None };
            yield return new object[] { frozenDataGridView.Rows[1], new Rectangle(1000, 2000, 100, 100), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { frozenDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 1000, 1000), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };

            var leftToRightDataGridView = new DataGridView { ColumnCount = 1, RightToLeft = RightToLeft.Yes };
            leftToRightDataGridView.Rows.Add(new DataGridViewRow());
            leftToRightDataGridView.Columns[0].Frozen = true;
            leftToRightDataGridView.Columns[0].Visible = true;
            yield return new object[] { leftToRightDataGridView.Rows[1], Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All };
            yield return new object[] { leftToRightDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { leftToRightDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, false, true, DataGridViewPaintParts.None };
            yield return new object[] { leftToRightDataGridView.Rows[1], new Rectangle(1000, 2000, 100, 100), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { leftToRightDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 1000, 1000), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };

            var singleVerticalBorderAddedDataGridView = new DataGridView { ColumnCount = 1, RowHeadersVisible = false };
            singleVerticalBorderAddedDataGridView.Rows.Add(new DataGridViewRow());
            singleVerticalBorderAddedDataGridView.Columns[0].Frozen = true;
            singleVerticalBorderAddedDataGridView.Columns[0].Visible = true;
            yield return new object[] { singleVerticalBorderAddedDataGridView.Rows[1], Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All };
            yield return new object[] { singleVerticalBorderAddedDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { singleVerticalBorderAddedDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, false, true, DataGridViewPaintParts.None };
            yield return new object[] { singleVerticalBorderAddedDataGridView.Rows[1], new Rectangle(1000, 2000, 100, 100), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { singleVerticalBorderAddedDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 1000, 1000), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };

            var singleHorizontalBorderAddedDataGridView = new DataGridView { ColumnCount = 1, ColumnHeadersVisible = false };
            singleHorizontalBorderAddedDataGridView.Rows.Add(new DataGridViewRow());
            singleHorizontalBorderAddedDataGridView.Columns[0].Frozen = true;
            singleHorizontalBorderAddedDataGridView.Columns[0].Visible = true;
            yield return new object[] { singleHorizontalBorderAddedDataGridView.Rows[1], Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All };
            yield return new object[] { singleHorizontalBorderAddedDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { singleHorizontalBorderAddedDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, false, true, DataGridViewPaintParts.None };
            yield return new object[] { singleHorizontalBorderAddedDataGridView.Rows[1], new Rectangle(1000, 2000, 100, 100), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { singleHorizontalBorderAddedDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 1000, 1000), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };

            var firstDisplayedScrollingColumnIndexDataGridView = new DataGridView { ColumnCount = 1, FirstDisplayedScrollingColumnIndex = 0 };
            firstDisplayedScrollingColumnIndexDataGridView.Rows.Add(new DataGridViewRow());
            firstDisplayedScrollingColumnIndexDataGridView.FirstDisplayedScrollingColumnIndex = 0;
            firstDisplayedScrollingColumnIndexDataGridView.Columns[0].Visible = true;
            yield return new object[] { firstDisplayedScrollingColumnIndexDataGridView.Rows[1], Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All };
            yield return new object[] { firstDisplayedScrollingColumnIndexDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { firstDisplayedScrollingColumnIndexDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, false, true, DataGridViewPaintParts.None };
            yield return new object[] { firstDisplayedScrollingColumnIndexDataGridView.Rows[1], new Rectangle(1000, 2000, 100, 100), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { firstDisplayedScrollingColumnIndexDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 1000, 1000), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };

            var firstDisplayedScrollingColumnIndexRightToLeftDataGridView = new DataGridView { ColumnCount = 1, FirstDisplayedScrollingColumnIndex = 0, RightToLeft = RightToLeft.Yes };
            firstDisplayedScrollingColumnIndexRightToLeftDataGridView.Rows.Add(new DataGridViewRow());
            firstDisplayedScrollingColumnIndexRightToLeftDataGridView.FirstDisplayedScrollingColumnIndex = 0;
            firstDisplayedScrollingColumnIndexRightToLeftDataGridView.Columns[0].Visible = true;
            yield return new object[] { firstDisplayedScrollingColumnIndexRightToLeftDataGridView.Rows[1], Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All };
            yield return new object[] { firstDisplayedScrollingColumnIndexRightToLeftDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { firstDisplayedScrollingColumnIndexRightToLeftDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, false, true, DataGridViewPaintParts.None };
            yield return new object[] { firstDisplayedScrollingColumnIndexRightToLeftDataGridView.Rows[1], new Rectangle(1000, 2000, 100, 100), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { firstDisplayedScrollingColumnIndexRightToLeftDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 1000, 1000), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };

            var singleHorizontalBorderAddedDisplayedDataGridView = new DataGridView { ColumnCount = 1, ColumnHeadersVisible = false };
            singleHorizontalBorderAddedDisplayedDataGridView.Rows.Add(new DataGridViewRow());
            singleHorizontalBorderAddedDisplayedDataGridView.FirstDisplayedScrollingColumnIndex = 0;
            singleHorizontalBorderAddedDisplayedDataGridView.Columns[0].Visible = true;
            yield return new object[] { singleHorizontalBorderAddedDisplayedDataGridView.Rows[1], Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All };
            yield return new object[] { singleHorizontalBorderAddedDisplayedDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { singleHorizontalBorderAddedDisplayedDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, false, true, DataGridViewPaintParts.None };
            yield return new object[] { singleHorizontalBorderAddedDisplayedDataGridView.Rows[1], new Rectangle(1000, 2000, 100, 100), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { singleHorizontalBorderAddedDisplayedDataGridView.Rows[1], new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 1000, 1000), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
        }

        [Theory]
        [MemberData(nameof(Paint_TestData))]
        public void DataGridViewRow_Paint_Invoke_Success(DataGridViewRow row, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts unused)
        {
            if (rowIndex == -1)
            {
                return;
            }

            Assert.NotNull((object)unused);
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                row.Paint(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow);
            }
        }

        [Fact]
        public void DataGridViewRow_Paint_Invoke_CallsRowPrePaint()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView { ColumnCount = 1 };
                dataGridView.Rows.Add(new DataGridViewRow());
                DataGridViewRow row = dataGridView.Rows[1];

                int callCount = 0;
                DataGridViewRowPrePaintEventHandler handler = (sender, e) =>
                {
                    callCount++;
                    Assert.Same(dataGridView, sender);
                    Assert.Same(graphics, e.Graphics);
                    Assert.Equal(new Rectangle(1, 2, 3, 4), e.ClipBounds);
                    Assert.Equal(new Rectangle(2, 3, 4, 5), e.RowBounds);
                    Assert.Equal(1, e.RowIndex);
                    Assert.Equal(DataGridViewElementStates.Frozen, e.State);
                    Assert.Empty(e.ErrorText);
                    Assert.True(e.IsFirstDisplayedRow);
                    Assert.False(e.IsLastVisibleRow);
                    Assert.Equal(DataGridViewPaintParts.All, e.PaintParts);
                    Assert.False(e.Handled);

                    if (callCount > 1)
                    {
                        e.Handled = true;
                    }
                };
                dataGridView.RowPrePaint += handler;
                row.Paint(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false);
                Assert.Equal(1, callCount);

                // Call again, handled.
                row.Paint(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false);
                Assert.Equal(2, callCount);

                // Remove handler.
                dataGridView.RowPrePaint -= handler;
                row.Paint(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false);
                Assert.Equal(2, callCount);
            }
        }

        [Fact]
        public void DataGridViewRow_Paint_InvokeNullGraphics_DoesNotCallRowPrePaint()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];

            int callCount = 0;
            DataGridViewRowPrePaintEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Null(e.Graphics);
                Assert.Equal(new Rectangle(1, 2, 3, 4), e.ClipBounds);
                Assert.Equal(new Rectangle(2, 3, 4, 5), e.RowBounds);
                Assert.Equal(1, e.RowIndex);
                Assert.Equal(DataGridViewElementStates.Frozen, e.State);
                Assert.Empty(e.ErrorText);
                Assert.True(e.IsFirstDisplayedRow);
                Assert.False(e.IsLastVisibleRow);
                Assert.Equal(DataGridViewPaintParts.All, e.PaintParts);
                Assert.False(e.Handled);

                if (callCount > 1)
                {
                    e.Handled = true;
                }
            };
            dataGridView.RowPrePaint += handler;
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);

            // Call again, handled.
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);

            // Remove handler.
            dataGridView.RowPrePaint -= handler;
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);
        }

        [Fact]
        public void DataGridViewRow_Paint_InvokeWithGraphics_CallsRowPostPaint()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView { ColumnCount = 1 };
                dataGridView.Rows.Add(new DataGridViewRow());
                DataGridViewRow row = dataGridView.Rows[1];

                int callCount = 0;
                DataGridViewRowPostPaintEventHandler handler = (sender, e) =>
                {
                    callCount++;
                    Assert.Same(dataGridView, sender);
                    Assert.Same(graphics, e.Graphics);
                    Assert.Equal(new Rectangle(1, 2, 3, 4), e.ClipBounds);
                    Assert.Equal(new Rectangle(2, 3, 4, 5), e.RowBounds);
                    Assert.Equal(1, e.RowIndex);
                    Assert.Equal(DataGridViewElementStates.Frozen, e.State);
                    Assert.Empty(e.ErrorText);
                    Assert.True(e.IsFirstDisplayedRow);
                    Assert.False(e.IsLastVisibleRow);
                };
                dataGridView.RowPostPaint += handler;
                row.Paint(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false);
                Assert.Equal(1, callCount);

                // Call again.
                row.Paint(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false);
                Assert.Equal(2, callCount);

                // Remove handler.
                dataGridView.RowPostPaint -= handler;
                row.Paint(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false);
                Assert.Equal(2, callCount);
            }
        }

        [Fact]
        public void DataGridViewRow_Paint_InvokeNullGraphics_DoesNotCallRowPostPaint()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];

            int callCount = 0;
            DataGridViewRowPostPaintEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Null(e.Graphics);
                Assert.Equal(Rectangle.Empty, e.ClipBounds);
                Assert.Equal(new Rectangle(2, 3, 4, 5), e.RowBounds);
                Assert.Equal(1, e.RowIndex);
                Assert.Equal(DataGridViewElementStates.Frozen, e.State);
                Assert.Empty(e.ErrorText);
                Assert.True(e.IsFirstDisplayedRow);
                Assert.False(e.IsLastVisibleRow);
            };
            dataGridView.RowPostPaint += handler;
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);

            // Call again.
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);

            // Call with nothing to do.
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, Rectangle.Empty, new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);

            // Remove handler.
            dataGridView.RowPostPaint -= handler;
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, Rectangle.Empty, new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);
        }

        [Fact]
        public void DataGridViewRow_Paint_NullGraphicsEmptyRectangle_ThrowsArgumentNullException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, Rectangle.Empty, Rectangle.Empty, 1, DataGridViewElementStates.None, true, true));
        }

        [Fact]
        public void DataGridViewRow_Paint_NullGraphicsNonEmptyRectangle_ThrowsArgumentNullException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, Rectangle.Empty, new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.None, true, true));
        }

        [Fact]
        public void DataGridViewRow_Paint_NullGraphicsNonEmptyClipRectangle_ThrowsArgumentNullException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.None, true, true));
        }

        [Fact]
        public void DataGridViewRow_Paint_NoDataGridView_ThrowsInvalidOperationException()
        {
            var row = new DataGridViewRow();
            Assert.Throws<InvalidOperationException>(() => row.Paint(null, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(2)]
        public void DataGridViewRow_Paint_InvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView { ColumnCount = 1 };
                dataGridView.Rows.Add(new DataGridViewRow());
                dataGridView.Columns[0].Frozen = true;
                dataGridView.Columns[0].Visible = true;
                DataGridViewRow row = dataGridView.Rows[1];
                Assert.Throws<ArgumentOutOfRangeException>("index", () => row.Paint(graphics, new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), rowIndex, DataGridViewElementStates.None, true, true));
            }
        }

        [Theory]
        [MemberData(nameof(Paint_TestData))]
        public void DataGridViewRow_PaintCells_Invoke_Success(DataGridViewRow row, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                row.PaintCells(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
            }
        }

        [Fact]
        public void DataGridViewRow_PaintCells_NullGraphicsEmptyRectangle_Nop()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintCells(null, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [Fact]
        public void DataGridViewRow_PaintCells_NullGraphicsNonEmptyRectangle_ThrowsArgumentNullException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintCells(null, Rectangle.Empty, new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [Fact]
        public void DataGridViewRow_PaintCells_NullGraphicsNoVisibleColumns_ThrowsArgumentNullException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintCells(null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 100, 100), -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [Fact]
        public void DataGridViewRow_PaintCells_NullGraphicsNonEmptyClipRectangle_ThrowsArgumentNullException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];
            dataGridView.Columns[0].Frozen = true;
            dataGridView.Columns[0].Visible = true;
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintCells(null, new Rectangle(1, 2, 1000, 10000), new Rectangle(1, 2, 1000, 1000), 1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [Fact]
        public void DataGridViewRow_PaintCells_NoDataGridView_ThrowsInvalidOperationException()
        {
            var row = new DataGridViewRow();
            Assert.Throws<InvalidOperationException>(() => row.PaintCells(null, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(2)]
        public void DataGridViewRow_PaintCells_InvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView { ColumnCount = 1 };
                dataGridView.Rows.Add(new DataGridViewRow());
                dataGridView.Columns[0].Frozen = true;
                dataGridView.Columns[0].Visible = true;
                DataGridViewRow row = dataGridView.Rows[1];
                Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => row.PaintCells(graphics, new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), rowIndex, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
            }
        }

        [Theory]
        [InlineData((DataGridViewPaintParts)(DataGridViewPaintParts.None - 1))]
        [InlineData((DataGridViewPaintParts)(DataGridViewPaintParts.All + 1))]
        public void DataGridViewRow_PaintCells_InvalidPaintParts_ThrowsInvalidEnumArgumentException(DataGridViewPaintParts paintParts)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView { ColumnCount = 1 };
                dataGridView.Rows.Add(new DataGridViewRow());
                DataGridViewRow row = dataGridView.Rows[1];
                Assert.Throws<InvalidEnumArgumentException>("paintParts", () => row.PaintHeader(graphics, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, paintParts));
            }
        }

        [Theory]
        [MemberData(nameof(Paint_TestData))]
        public void DataGridViewRow_PaintHeader_Invoke_Success(DataGridViewRow row, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                row.PaintHeader(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
            }
        }

        [Fact]
        public void DataGridViewRow_PaintHeader_NullGraphicsEmptyRectangle_ThrowsArgumentNullException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintHeader(null, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [Fact]
        public void DataGridViewRow_PaintHeader_NullGraphicsNonEmptyRectangle_ThrowsArgumentNullException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintHeader(null, Rectangle.Empty, new Rectangle(1, 2, 100, 100), -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [Fact]
        public void DataGridViewRow_PaintHeader_NullGraphicsNonEmptyClipRectangle_ThrowsArgumentNullException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintHeader(null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [Fact]
        public void DataGridViewRow_PaintHeader_NullGraphicsRowHeadersNotVisible_ThrowsArgumentNullException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1, RowHeadersVisible = false };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintHeader(null, new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [Fact]
        public void DataGridViewRow_PaintHeader_NoDataGridView_ThrowsInvalidOperationException()
        {
            var row = new DataGridViewRow();
            Assert.Throws<InvalidOperationException>(() => row.PaintHeader(null, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(2)]
        public void DataGridViewRow_PaintHeader_InvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView { ColumnCount = 1 };
                dataGridView.Rows.Add(new DataGridViewRow());
                DataGridViewRow row = dataGridView.Rows[1];
                Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => row.PaintHeader(graphics, new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), rowIndex, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
            }
        }

        [Fact]
        public void DataGridViewRow_PaintHeader_SharedRowIndex_ThrowsInvalidOperationException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView { ColumnCount = 1 };
                dataGridView.Rows.Add(new DataGridViewRow());
                DataGridViewRow row = dataGridView.Rows[1];
                Assert.Throws<InvalidOperationException>(() => row.PaintHeader(graphics, new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
            }
        }

        [Theory]
        [InlineData((DataGridViewPaintParts)(DataGridViewPaintParts.None - 1))]
        [InlineData((DataGridViewPaintParts)(DataGridViewPaintParts.All + 1))]
        public void DataGridViewRow_PaintHeader_InvalidPaintParts_ThrowsInvalidEnumArgumentException(DataGridViewPaintParts paintParts)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var dataGridView = new DataGridView { ColumnCount = 1 };
                dataGridView.Rows.Add(new DataGridViewRow());
                DataGridViewRow row = dataGridView.Rows[1];
                Assert.Throws<InvalidEnumArgumentException>("paintParts", () => row.PaintHeader(graphics, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, paintParts));
            }
        }

        [Theory]
        [InlineData(new object[0], true, null)]
        [InlineData(new object[] { 1 }, true, 1)]
        [InlineData(new object[] { 1, 2 }, false, 1)]
        public void DataGridViewRow_SetValues_HasCellsWithoutDataGridView_Success(object[] values, bool expectedResult, object expectedValue)
        {
            var row = new DataGridViewRow();
            row.Cells.Add(new SubDataGridViewCell());
            Assert.Equal(expectedResult, row.SetValues(values));
            Assert.Equal(expectedValue, Assert.Single(row.Cells.Cast<DataGridViewCell>()).Value);
        }

        [Theory]
        [InlineData(new object[0], true, null)]
        [InlineData(new object[] { 1 }, true, 1)]
        [InlineData(new object[] { 1, 2 }, false, 1)]
        public void DataGridViewRow_SetValues_HasCellsWithDataGridView_Success(object[] values, bool expectedResult, object expectedValue)
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];
            Assert.Equal(expectedResult, row.SetValues(values));
            Assert.Equal(expectedValue, Assert.Single(row.Cells.Cast<DataGridViewCell>()).Value);
        }

        [Fact]
        public void DataGridViewRow_SetValues_CantSet_ReturnsFalse()
        {
            var row = new DataGridViewRow();
            row.Cells.Add(new CantSetDataGridViewCell());
            Assert.False(row.SetValues(new object[] { 1 }));
            Assert.Null(Assert.Single(row.Cells.Cast<DataGridViewCell>()).Value);

        }

        private class CantSetDataGridViewCell : DataGridViewCell
        {
            protected override bool SetValue(int rowIndex, object value) => false;
        }

        [Fact]
        public void DataGridViewRow_SetValues_NullValue_ThrowsArgumentNullException()
        {
            var row = new DataGridViewRow();
            Assert.Throws<ArgumentNullException>("values", () => row.SetValues(null));
        }

        [Fact]
        public void DataGridViewRow_SetValues_VirtualDataGridView_ThrowsInvalidOperationException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1, VirtualMode = true };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = dataGridView.Rows[0];
            Assert.Throws<InvalidOperationException>(() => row.SetValues(Array.Empty<object>()));
        }

        [Theory]
        [MemberData(nameof(SharedRow_TestData))]
        public void DataGridViewRow_SetValues_Shared_ThrowsInvalidOperationException(DataGridViewRow row)
        {
            Assert.Throws<InvalidOperationException>(() => row.SetValues(Array.Empty<object>()));
        }

        private class SubDataGridViewCell : DataGridViewCell
        {
        }

        public class SubDataGridViewRow : DataGridViewRow
        {
            public new DataGridViewCellCollection CreateCellsInstance() => base.CreateCellsInstance();

#pragma warning disable xUnit1013

            public new void DrawFocus(Graphics graphics, Rectangle clipBounds, Rectangle bounds, int rowIndex, DataGridViewElementStates rowState, DataGridViewCellStyle cellStyle, bool cellsPaintSelectionBackground)
            {
                base.DrawFocus(graphics, clipBounds, bounds, rowIndex, rowState, cellStyle, cellsPaintSelectionBackground);
            }

#pragma warning restore xUnit1013

            public new bool IsRow => base.IsRow;

            [Fact]
            public void DataGridViewRow_AccessibilityObject_Get_ReturnsSameInstance()
            {
                Assert.Same(AccessibilityObject, AccessibilityObject);
                DataGridViewRowAccessibleObject accessibilityObject = Assert.IsType<DataGridViewRowAccessibleObject>(AccessibilityObject);
                Assert.Equal(this, accessibilityObject.Owner);
            }

            [Fact]
            public void DataGridViewRow_CreateAccessibilityInstance_Invoke_ReturnsExpected()
            {
                DataGridViewRowAccessibleObject accessibilityObject = Assert.IsType<DataGridViewRowAccessibleObject>(CreateAccessibilityInstance());
                Assert.Equal(this, accessibilityObject.Owner);
            }
        }
    }
}
