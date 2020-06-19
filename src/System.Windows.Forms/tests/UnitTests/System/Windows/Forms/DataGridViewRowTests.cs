// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Globalization;
using WinForms.Common.Tests;
using Xunit;
using System.ComponentModel;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewRowTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DataGridViewRow_Ctor_Default()
        {
            using var row = new SubDataGridViewRow();
            Assert.Same(row.Cells, row.Cells);
            Assert.Empty(row.Cells);
            Assert.Null(row.ContextMenuStrip);
            Assert.Null(row.DataBoundItem);
            Assert.Null(row.DataGridView);
            Assert.NotNull(row.DefaultCellStyle);
            Assert.Same(row.DefaultCellStyle, row.DefaultCellStyle);
            Assert.Same(typeof(DataGridViewRowHeaderCell), row.DefaultHeaderCellType);
            Assert.False(row.Displayed);
            Assert.Equal(0, row.DividerHeight);
            Assert.Empty(row.ErrorText);
            Assert.False(row.Frozen);
            Assert.True(row.HasDefaultCellStyle);
            Assert.IsType<DataGridViewRowHeaderCell>(row.HeaderCell);
            Assert.Same(row.HeaderCell, row.HeaderCell);
            Assert.IsType<DataGridViewRowHeaderCell>(row.HeaderCellCore);
            Assert.Same(row.HeaderCellCore, row.HeaderCellCore);
            Assert.Equal(Control.DefaultFont.Height + 9, row.Height);
            Assert.Equal(row, row.HeaderCell.OwningRow);
            Assert.Equal(-1, row.Index);
            Assert.Throws<InvalidOperationException>(() => row.InheritedStyle);
            Assert.True(row.IsRow);
            Assert.False(row.IsNewRow);
            Assert.Equal(3, row.MinimumHeight);
            Assert.False(row.ReadOnly);
            Assert.Equal(DataGridViewTriState.NotSet, row.Resizable);
            Assert.False(row.Selected);
            Assert.Equal(DataGridViewElementStates.Visible, row.State);
            Assert.Null(row.Tag);
            Assert.True(row.Visible);
        }

        [WinFormsFact]
        public void DataGridViewRow_ContextMenuStrip_GetWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            int callCount = 0;
            control.RowContextMenuStripNeeded += (sender, e) => callCount++;
            DataGridViewRow row = control.Rows[0];
            Assert.Null(row.ContextMenuStrip);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_ContextMenuStrip_GetWithDataGridViewVirtualMode_CallsRowContextMenuStripNeeded()
        {
            using var menu1 = new ContextMenuStrip();
            using var menu2 = new ContextMenuStrip();
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                VirtualMode = true
            };
            DataGridViewRow row = control.Rows[0];
            row.ContextMenuStrip = menu1;

            int callCount = 0;
            DataGridViewRowContextMenuStripNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Same(menu1, e.ContextMenuStrip);
                e.ContextMenuStrip = menu2;
            };
            control.RowContextMenuStripNeeded += handler;

            Assert.Same(menu2, row.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.RowContextMenuStripNeeded -= handler;
            Assert.Same(menu1, row.ContextMenuStrip);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_ContextMenuStrip_GetWithDataGridViewDataSource_CallsRowContextMenuStripNeeded()
        {
            using var menu1 = new ContextMenuStrip();
            using var menu2 = new ContextMenuStrip();
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                DataSource = new[] { new { Name = "Name" } }
            };
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);
            DataGridViewRow row = control.Rows[0];
            row.ContextMenuStrip = menu1;

            int callCount = 0;
            DataGridViewRowContextMenuStripNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Same(menu1, e.ContextMenuStrip);
                e.ContextMenuStrip = menu2;
            };
            control.RowContextMenuStripNeeded += handler;

            Assert.Same(menu2, row.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.RowContextMenuStripNeeded -= handler;
            Assert.Same(menu1, row.ContextMenuStrip);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_ContextMenuStrip_GetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.ContextMenuStrip);
        }

        public static IEnumerable<object[]> ContextMenuStrip_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ContextMenuStrip() };
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void DataGridViewRow_ContextMenuStrip_Set_GetReturnsExpected(ContextMenuStrip value)
        {
            using var row = new DataGridViewRow
            {
                ContextMenuStrip = value
            };
            Assert.Same(value, row.ContextMenuStrip);

            // Set same.
            row.ContextMenuStrip = value;
            Assert.Same(value, row.ContextMenuStrip);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void DataGridViewRow_ContextMenuStrip_SetWithCustomOldValue_GetReturnsExpected(ContextMenuStrip value)
        {
            using var oldValue = new ContextMenuStrip();
            using var row = new DataGridViewRow
            {
                ContextMenuStrip = oldValue
            };

            row.ContextMenuStrip = value;
            Assert.Same(value, row.ContextMenuStrip);

            // Set same.
            row.ContextMenuStrip = value;
            Assert.Same(value, row.ContextMenuStrip);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void DataGridViewRow_ContextMenuStrip_SetWithDataGridView_GetReturnsExpected(ContextMenuStrip value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            row.ContextMenuStrip = value;
            Assert.Same(value, row.ContextMenuStrip);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.ContextMenuStrip = value;
            Assert.Same(value, row.ContextMenuStrip);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void DataGridViewRow_ContextMenuStrip_SetShared_GetReturnsExpected(ContextMenuStrip value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);

            row.ContextMenuStrip = value;
            Assert.Throws<InvalidOperationException>(() => row.ContextMenuStrip);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.ContextMenuStrip = value;
            Assert.Throws<InvalidOperationException>(() => row.ContextMenuStrip);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRow_ContextMenuStrip_SetDisposeNew_RemovesContextMenuStrip()
        {
            using var menu = new ContextMenuStrip();
            using var row = new DataGridViewRow
            {
                ContextMenuStrip = menu
            };
            Assert.Same(menu, row.ContextMenuStrip);

            menu.Dispose();
            Assert.Null(row.ContextMenuStrip);
        }

        [WinFormsFact]
        public void DataGridViewRow_ContextMenuStrip_SetDisposeOld_RemovesContextMenuStrip()
        {
            using var menu1 = new ContextMenuStrip();
            using var menu2 = new ContextMenuStrip();
            using var row = new DataGridViewRow
            {
                ContextMenuStrip = menu1
            };
            Assert.Same(menu1, row.ContextMenuStrip);

            row.ContextMenuStrip = menu2;
            Assert.Same(menu2, row.ContextMenuStrip);

            menu1.Dispose();
            Assert.Same(menu2, row.ContextMenuStrip);
        }

        [WinFormsFact]
        public void DataGridViewRow_ContextMenuStrip_SetWithHandler_CallsContextMenuStripChanged()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(row, e.Row);
                callCount++;
            };
            control.RowContextMenuStripChanged += handler;

            // Set different.
            using var menu1 = new ContextMenuStrip();
            row.ContextMenuStrip = menu1;
            Assert.Same(menu1, row.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Set same.
            row.ContextMenuStrip = menu1;
            Assert.Same(menu1, row.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Set different.
            using var menu2 = new ContextMenuStrip();
            row.ContextMenuStrip = menu2;
            Assert.Same(menu2, row.ContextMenuStrip);
            Assert.Equal(2, callCount);

            // Set null.
            row.ContextMenuStrip = null;
            Assert.Null(row.ContextMenuStrip);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.RowContextMenuStripChanged -= handler;
            row.ContextMenuStrip = menu1;
            Assert.Same(menu1, row.ContextMenuStrip);
            Assert.Equal(3, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_DataBoundItem_GetWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Null(row.DataBoundItem);
        }

        [WinFormsFact]
        public void DataGridViewRow_DataBoundItem_GetWithDataGridViewWithDataSource_ReturnsExpected()
        {
            var boundObject = new { Name = "Name" };
            using var control = new DataGridView
            {
                DataSource = new[] { boundObject }
            };
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);
            DataGridViewRow row = control.Rows[0];
            Assert.Same(boundObject, row.DataBoundItem);
        }

        [WinFormsFact]
        public void DataGridViewRow_DataBoundItem_GetShared_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Null(row.DataBoundItem);
        }

        [WinFormsFact]
        public void DataGridViewRow_DataBoundItem_GetSharedWithDataSource_ReturnsExpected()
        {
            var boundObject = new { Name = "Name" };
            using var control = new DataGridView
            {
                DataSource = new[] { boundObject }
            };
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Null(row.DataBoundItem);
        }

        [WinFormsFact]
        public void DataGridViewRow_DefaultCellStyle_GetWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.NotNull(row.DefaultCellStyle);
            Assert.Same(row.DefaultCellStyle, row.DefaultCellStyle);
        }

        [WinFormsFact]
        public void DataGridViewRow_DefaultCellStyle_GetShared_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.NotNull(row.DefaultCellStyle);
            Assert.Same(row.DefaultCellStyle, row.DefaultCellStyle);
        }

        public static IEnumerable<object[]> DefaultCellStyle_Set_TestData()
        {
            yield return new object[] { null, new DataGridViewCellStyle() };

            var style1 = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter };
            var style2 = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomLeft };
            yield return new object[] { style1, style1 };
            yield return new object[] { style2, style2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultCellStyle_Set_TestData))]
        public void DataGridViewRow_DefaultCellStyle_Set_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            using var row = new DataGridViewRow
            {
                DefaultCellStyle = value
            };
            Assert.Equal(expected, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);

            // Set same.
            row.DefaultCellStyle = value;
            Assert.Equal(expected, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultCellStyle_Set_TestData))]
        public void DataGridViewRow_DefaultCellStyle_SetWithNonNullOldValue_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            var oldValue = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            using var row = new DataGridViewRow
            {
                DefaultCellStyle = oldValue
            };

            row.DefaultCellStyle = value;
            Assert.Equal(expected, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);

            // Set same.
            row.DefaultCellStyle = value;
            Assert.Equal(expected, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultCellStyle_Set_TestData))]
        public void DataGridViewRow_DefaultCellStyle_SetWithDataGridView_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            row.DefaultCellStyle = value;

            Assert.Equal(expected, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.DefaultCellStyle = value;
            Assert.Equal(expected, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultCellStyle_Set_TestData))]
        public void DataGridViewRow_DefaultCellStyle_SetWithDataGridViewWithNonNullOldValue_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            var oldValue = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            row.DefaultCellStyle = oldValue;

            row.DefaultCellStyle = value;
            Assert.Equal(expected, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.DefaultCellStyle = value;
            Assert.Equal(expected, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRow_DefaultCellStyle_SetWithDataGridView_CallsRowDefaultCellStyleChanged()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(row, e.Row);
                callCount++;
            };
            control.RowDefaultCellStyleChanged += handler;

            var style1 = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

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
            var style2 = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.BottomCenter
            };
            row.DefaultCellStyle = style2;
            Assert.Same(style2, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.Equal(2, callCount);

            // Set null.
            row.DefaultCellStyle = null;
            Assert.NotNull(row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.Equal(3, callCount);

            // Set null again.
            row.DefaultCellStyle = null;
            Assert.NotNull(row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.Equal(4, callCount);

            // Set non-null.
            row.DefaultCellStyle = style2;
            Assert.NotNull(row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.Equal(5, callCount);

            // Remove handler.
            control.RowDefaultCellStyleChanged -= handler;
            row.DefaultCellStyle = style1;
            Assert.Equal(style1, row.DefaultCellStyle);
            Assert.True(row.HasDefaultCellStyle);
            Assert.Equal(5, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_DefaultCellStyle_SetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.DefaultCellStyle = new DataGridViewCellStyle());
        }

        [WinFormsFact]
        public void DataGridViewRow_DefaultHeaderCellType_GetWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Equal(typeof(DataGridViewRowHeaderCell), row.DefaultHeaderCellType);
        }

        [WinFormsFact]
        public void DataGridViewRow_DefaultHeaderCellType_GetShared_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Equal(typeof(DataGridViewRowHeaderCell), row.DefaultHeaderCellType);
        }

        public static IEnumerable<object[]> DefaultHeaderCellType_Set_TestData()
        {
            yield return new object[] { null, typeof(DataGridViewRowHeaderCell) };
            yield return new object[] { typeof(DataGridViewRowHeaderCell), typeof(DataGridViewRowHeaderCell) };
            yield return new object[] { typeof(DataGridViewColumnHeaderCell), typeof(DataGridViewColumnHeaderCell) };
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultHeaderCellType_Set_TestData))]
        public void DataGridViewRow_DefaultHeaderCellType_Set_GetReturnsExpected(Type value, Type expected)
        {
            using var row = new DataGridViewRow
            {
                DefaultHeaderCellType = value
            };
            Assert.Equal(expected, row.DefaultHeaderCellType);

            // Set same.
            row.DefaultHeaderCellType = value;
            Assert.Equal(expected, row.DefaultHeaderCellType);
        }

        [WinFormsTheory]
        [InlineData(typeof(DataGridViewRowHeaderCell))]
        [InlineData(typeof(DataGridViewColumnHeaderCell))]
        [InlineData(typeof(DataGridViewHeaderCell))]
        public void DataGridViewRow_DefaultHeaderCellType_SetWithNonNullOldValue_GetReturnsExpected(Type value)
        {
            using var row = new SubDataGridViewRow
            {
                DefaultHeaderCellType = typeof(DataGridViewRowHeaderCell)
            };
            row.DefaultHeaderCellType = value;
            Assert.Equal(value, row.DefaultHeaderCellType);

            // Set same.
            row.DefaultHeaderCellType = value;
            Assert.Equal(value, row.DefaultHeaderCellType);
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultHeaderCellType_Set_TestData))]
        public void DataGridViewRow_DefaultHeaderCellType_SetWithDataGridView_GetReturnsExpected(Type value, Type expected)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            row.DefaultHeaderCellType = value;
            Assert.Equal(expected, row.DefaultHeaderCellType);

            // Set same.
            row.DefaultHeaderCellType = value;
            Assert.Equal(expected, row.DefaultHeaderCellType);
        }

        [WinFormsTheory]
        [InlineData(typeof(DataGridViewRowHeaderCell))]
        [InlineData(typeof(DataGridViewColumnHeaderCell))]
        [InlineData(typeof(DataGridViewHeaderCell))]
        public void DataGridViewRow_DefaultHeaderCellType_SetWithDataGridViewNonNullOldValue_GetReturnsExpected(Type value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            row.DefaultHeaderCellType = typeof(DataGridViewRowHeaderCell);

            row.DefaultHeaderCellType = value;
            Assert.Equal(value, row.DefaultHeaderCellType);

            // Set same.
            row.DefaultHeaderCellType = value;
            Assert.Equal(value, row.DefaultHeaderCellType);
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultHeaderCellType_Set_TestData))]
        public void DataGridViewRow_DefaultHeaderCellType_SetShared_GetReturnsExpected(Type value, Type expected)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);

            row.DefaultHeaderCellType = value;
            Assert.Equal(expected, row.DefaultHeaderCellType);

            // Set same.
            row.DefaultHeaderCellType = value;
            Assert.Equal(expected, row.DefaultHeaderCellType);
        }

        [WinFormsTheory]
        [InlineData(typeof(DataGridViewRowHeaderCell))]
        [InlineData(typeof(DataGridViewColumnHeaderCell))]
        [InlineData(typeof(DataGridViewHeaderCell))]
        public void DataGridViewRow_DefaultHeaderCellType_SetSharedNonNullOldValue_GetReturnsExpected(Type value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            row.DefaultHeaderCellType = typeof(DataGridViewRowHeaderCell);

            row.DefaultHeaderCellType = value;
            Assert.Equal(value, row.DefaultHeaderCellType);

            // Set same.
            row.DefaultHeaderCellType = value;
            Assert.Equal(value, row.DefaultHeaderCellType);
        }

        [WinFormsTheory]
        [InlineData(typeof(int))]
        public void DataGridViewRow_DefaultHeaderCellType_SetInvalidWithNullOldValue_GetReturnsExpected(Type value)
        {
            using var row = new DataGridViewRow();
            Assert.Throws<ArgumentException>("value", () => row.DefaultHeaderCellType = value);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData(typeof(int))]
        public void DataGridViewRow_DefaultHeaderCellType_SetInvalidWithNonNullOldValue_GetReturnsExpected(Type value)
        {
            using var row = new DataGridViewRow
            {
                DefaultHeaderCellType = typeof(DataGridViewRowHeaderCell)
            };
            Assert.Throws<ArgumentException>("value", () => row.DefaultHeaderCellType = value);
        }

        [WinFormsFact]
        public void DataGridViewRow_Displayed_GetWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.False(row.Displayed);
        }

        [WinFormsFact]
        public void DataGridViewRow_Displayed_GetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.Displayed);
        }

        [WinFormsFact]
        public void DataGridViewRow_DividerHeight_GetWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Equal(0, row.DividerHeight);
        }

        [WinFormsFact]
        public void DataGridViewRow_DividerHeight_GetShared_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Equal(0, row.DividerHeight);
        }

        public static IEnumerable<object[]> DividerHeight_Set_TestData()
        {
            yield return new object[] { 0 };
            yield return new object[] { 1 };
            yield return new object[] { 65536 };
        }

        [WinFormsTheory]
        [MemberData(nameof(DividerHeight_Set_TestData))]
        public void DataGridViewRow_DividerHeight_Set_GetReturnsExpected(int value)
        {
            using var row = new DataGridViewRow
            {
                DividerHeight = value
            };
            Assert.Equal(value, row.DividerHeight);

            // Set same.
            row.DividerHeight = value;
            Assert.Equal(value, row.DividerHeight);
        }

        [WinFormsTheory]
        [MemberData(nameof(DividerHeight_Set_TestData))]
        public void DataGridViewRow_DividerHeight_SetWithDataGridView_GetReturnsExpected(int value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            row.DividerHeight = value;
            Assert.Equal(value, row.DividerHeight);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.DividerHeight = value;
            Assert.Equal(value, row.DividerHeight);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRow_DividerHeight_SetWithDataGridView_CallsRowDividerHeightChanged()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Same(row, e.Row);
            };
            control.RowDividerHeightChanged += handler;

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
            control.RowDividerHeightChanged -= handler;
            row.DividerHeight = 4;
            Assert.Equal(4, row.DividerHeight);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(65537)]
        public void DataGridViewRow_DividerHeight_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            using var row = new DataGridViewRow();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => row.DividerHeight = value);
        }

        [WinFormsFact]
        public void DataGridViewRow_DividerHeight_SetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.DividerHeight = -1);
        }

        [WinFormsFact]
        public void DataGridViewRow_ErrorText_GetWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Empty(row.ErrorText);
        }

        [WinFormsFact]
        public void DataGridViewRow_ErrorText_GetNeedsErrorTextVirtualMode_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                VirtualMode = true
            };
            DataGridViewRow row = control.Rows[0];
            row.ErrorText = "errorText1";

            int callCount = 0;
            DataGridViewRowErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal("errorText1", e.ErrorText);
                e.ErrorText = "errorText2";
            };
            control.RowErrorTextNeeded += handler;
            Assert.Same("errorText2", row.ErrorText);
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.RowErrorTextNeeded -= handler;
            Assert.Same("errorText1", row.ErrorText);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_ErrorText_GetNeedsErrorTextDataSource_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                DataSource = new[] { new { Name = "Name" } }
            };
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);

            DataGridViewRow row = control.Rows[0];
            row.ErrorText = "errorText1";

            int callCount = 0;
            DataGridViewRowErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal("errorText1", e.ErrorText);
                e.ErrorText = "errorText2";
            };
            control.RowErrorTextNeeded += handler;
            Assert.Equal("errorText2", row.ErrorText);
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.RowErrorTextNeeded -= handler;
            Assert.Equal("errorText1", row.ErrorText);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_ErrorText_GetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.ErrorText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewRow_ErrorText_Set_GetReturnsExpected(string value, string expected)
        {
            using var row = new DataGridViewRow
            {
                ErrorText = value
            };
            Assert.Equal(expected, row.ErrorText);

            // Set same.
            row.ErrorText = value;
            Assert.Equal(expected, row.ErrorText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewRow_ErrorText_SetWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            using var row = new DataGridViewRow
            {
                ErrorText = "OldValue"
            };
            row.ErrorText = value;

            Assert.Equal(expected, row.ErrorText);

            // Set same.
            row.ErrorText = value;
            Assert.Equal(expected, row.ErrorText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewRow_ErrorText_SetWithDataGridView_GetReturnsExpected(string value, string expected)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            row.ErrorText = value;
            Assert.Equal(expected, row.ErrorText);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.ErrorText = value;
            Assert.Equal(expected, row.ErrorText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewRow_ErrorText_SetWithDataGridViewWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            row.ErrorText = "OldValue";

            row.ErrorText = value;
            Assert.Equal(expected, row.ErrorText);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.ErrorText = value;
            Assert.Equal(expected, row.ErrorText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRow_ErrorText_SetWithDataGridView_CallsRowErrorTextChanged()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Same(row, e.Row);
            };
            control.RowErrorTextChanged += handler;

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
            control.RowErrorTextChanged -= handler;
            row.ErrorText = "errorText";
            Assert.Equal("errorText", row.ErrorText);
            Assert.Equal(3, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_ErrorText_SetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => row.ErrorText = "value");
        }

        [WinFormsTheory]
        [InlineData(DataGridViewElementStates.None, false)]
        [InlineData(DataGridViewElementStates.Frozen, false)]
        [InlineData(DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly, false)]
        [InlineData(DataGridViewElementStates.Frozen | DataGridViewElementStates.Selected, false)]
        public void DataGridViewRow_Frozen_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state, bool expected)
        {
            using var row = new CustomStateDataGridViewRow
            {
                StateResult = state
            };
            Assert.Equal(expected, row.Frozen);
        }

        [WinFormsFact]
        public void DataGridViewRow_Frozen_GetWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView();
            using var cellTemplate = new SubDataGridViewCell();
            using var row = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            control.Columns.Add(row);
            control.Rows.Add();
            Assert.False(control.Rows[0].Frozen);
        }

        [WinFormsFact]
        public void DataGridViewRow_Frozen_GetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView();
            using var cellTemplate = new SubDataGridViewCell();
            using var row = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            control.Columns.Add(row);
            control.Rows.Add();
            Assert.Throws<InvalidOperationException>(() => control.Rows.SharedRow(0).Frozen);
        }

        public static IEnumerable<object[]> Frozen_Set_TestData()
        {
            foreach (bool visible in new bool[] { true, false })
            {
                yield return new object[] { visible, true };
                yield return new object[] { visible, false };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Frozen_Set_TestData))]
        public void DataGridViewRow_Frozen_Set_GetReturnsExpected(bool visible, bool value)
        {
            using var row = new DataGridViewRow
            {
                Visible = visible,
                Frozen = value
            };
            Assert.Equal(value, row.Frozen);
            Assert.Equal(value, (row.State & DataGridViewElementStates.Frozen) != 0);

            // Set same.
            row.Frozen = value;
            Assert.Equal(value, row.Frozen);
            Assert.Equal(value, (row.State & DataGridViewElementStates.Frozen) != 0);

            // Set different.
            row.Frozen = !value;
            Assert.Equal(!value, row.Frozen);
            Assert.Equal(!value, (row.State & DataGridViewElementStates.Frozen) != 0);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewRow_Frozen_SetWithDataGridView_GetReturnsExpected(bool value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1
            };
            control.Rows.Add();
            DataGridViewRow row = control.Rows[0];

            row.Frozen = value;
            Assert.Equal(value, row.Frozen);
            Assert.Equal(value, (row.State & DataGridViewElementStates.Frozen) != 0);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.Frozen = value;
            Assert.Equal(value, row.Frozen);
            Assert.Equal(value, (row.State & DataGridViewElementStates.Frozen) != 0);
            Assert.False(control.IsHandleCreated);

            // Set different.
            row.Frozen = !value;
            Assert.Equal(!value, row.Frozen);
            Assert.Equal(!value, (row.State & DataGridViewElementStates.Frozen) != 0);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void DataGridViewRow_Frozen_SetWithDataGridViewWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1
            };
            control.Rows.Add();
            DataGridViewRow row = control.Rows[0];
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            row.Frozen = value;
            Assert.Equal(value, row.Frozen);
            Assert.Equal(value, (row.State & DataGridViewElementStates.Frozen) != 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            row.Frozen = value;
            Assert.Equal(value, row.Frozen);
            Assert.Equal(value, (row.State & DataGridViewElementStates.Frozen) != 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            row.Frozen = !value;
            Assert.Equal(!value, row.Frozen);
            Assert.Equal(!value, (row.State & DataGridViewElementStates.Frozen) != 0);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewRow_Frozen_SetWithPreviousRows_SetsToFrozen(bool previousVisible)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1
            };
            control.Rows.Add();
            control.Rows.Add();
            control.Rows.Add();
            control.Rows.Add();
            DataGridViewRow row1 = control.Rows[0];
            DataGridViewRow row2 = control.Rows[1];
            row2.Visible = previousVisible;
            DataGridViewRow row3 = control.Rows[2];
            DataGridViewRow row4 = control.Rows[3];

            // Freeze middle.
            row3.Frozen = true;
            Assert.True(row1.Frozen);
            Assert.Equal(previousVisible, row2.Frozen);
            Assert.True(row3.Frozen);
            Assert.False(row4.Frozen);

            // Freeze again.
            row3.Frozen = true;
            Assert.True(row1.Frozen);
            Assert.Equal(previousVisible, row2.Frozen);
            Assert.True(row3.Frozen);
            Assert.False(row4.Frozen);

            // Freeze later.
            row4.Frozen = true;
            Assert.True(row1.Frozen);
            Assert.Equal(previousVisible, row2.Frozen);
            Assert.True(row3.Frozen);
            Assert.True(row4.Frozen);

            // Unfreeze middle.
            row3.Frozen = false;
            Assert.True(row1.Frozen);
            Assert.Equal(previousVisible, row2.Frozen);
            Assert.False(row3.Frozen);
            Assert.False(row4.Frozen);

            // Unfreeze first.
            row1.Frozen = false;
            Assert.False(row1.Frozen);
            Assert.False(row2.Frozen);
            Assert.False(row3.Frozen);
            Assert.False(row4.Frozen);
        }

        [WinFormsFact]
        public void DataGridViewRow_Frozen_SetWithDataGridView_CallsRowStateChanged()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            int callCount = 0;
            DataGridViewRowStateChangedEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Same(row, e.Row);
            };
            control.RowStateChanged += handler;

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
            control.RowStateChanged -= handler;
            row.Frozen = true;
            Assert.True(row.Frozen);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewRow_Frozen_SetShared_ThrowsInvalidOperationException(bool value)
        {
            using var control = new DataGridView();
            using var cellTemplate = new SubDataGridViewCell();
            using var row = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            control.Columns.Add(row);
            control.Rows.Add();
            Assert.Throws<InvalidOperationException>(() => control.Rows.SharedRow(0).Frozen = value);
        }

        [WinFormsFact]
        public void DataGridViewRow_HeaderCell_Get_ReturnsExpected()
        {
            using var row = new SubDataGridViewRow();
            Assert.IsType<DataGridViewRowHeaderCell>(row.HeaderCell);
            Assert.Same(row, row.HeaderCell.OwningRow);
            Assert.Null(row.HeaderCell.OwningColumn);
            Assert.Same(row.HeaderCell, row.HeaderCell);
            Assert.Same(row.HeaderCell, row.HeaderCellCore);
        }

        [WinFormsFact]
        public void DataGridViewRow_HeaderCell_GetWithDataGridView_ReturnsExpected()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 1,
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];
            Assert.IsType<DataGridViewRowHeaderCell>(row.HeaderCell);
            Assert.Same(row, row.HeaderCell.OwningRow);
            Assert.Null(row.HeaderCell.OwningColumn);
            Assert.Same(row.HeaderCell, row.HeaderCell);
            Assert.Same(row.HeaderCell, row.HeaderCellCore);
        }

        [WinFormsFact]
        public void DataGridViewRow_HeaderCell_GetShared_ReturnsExpected()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 1,
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows.SharedRow(0);
            Assert.IsType<DataGridViewRowHeaderCell>(row.HeaderCell);
            Assert.Same(row, row.HeaderCell.OwningRow);
            Assert.Null(row.HeaderCell.OwningColumn);
            Assert.Same(row.HeaderCell, row.HeaderCell);
            Assert.Same(row.HeaderCell, row.HeaderCellCore);
        }

        [WinFormsFact]
        public void DataGridViewRow_HeaderCellCore_Get_ReturnsExpected()
        {
            using var row = new SubDataGridViewRow();
            Assert.IsType<DataGridViewRowHeaderCell>(row.HeaderCellCore);
            Assert.Same(row, row.HeaderCellCore.OwningRow);
            Assert.Null(row.HeaderCellCore.OwningColumn);
            Assert.Same(row.HeaderCellCore, row.HeaderCellCore);
            Assert.Same(row.HeaderCell, row.HeaderCellCore);
        }

        [WinFormsFact]
        public void DataGridViewRow_HeaderCellCore_GetWithDataGridView_ReturnsExpected()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 1,
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];
            Assert.IsType<DataGridViewRowHeaderCell>(row.HeaderCellCore);
            Assert.Same(row, row.HeaderCellCore.OwningRow);
            Assert.Null(row.HeaderCellCore.OwningColumn);
            Assert.Same(row.HeaderCellCore, row.HeaderCellCore);
            Assert.Same(row.HeaderCell, row.HeaderCellCore);
        }

        [WinFormsFact]
        public void DataGridViewRow_HeaderCellCore_GetShared_ReturnsExpected()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 1,
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows.SharedRow(0);
            Assert.IsType<DataGridViewRowHeaderCell>(row.HeaderCellCore);
            Assert.Same(row, row.HeaderCellCore.OwningRow);
            Assert.Null(row.HeaderCellCore.OwningColumn);
            Assert.Same(row.HeaderCellCore, row.HeaderCellCore);
            Assert.Same(row.HeaderCell, row.HeaderCellCore);
        }

        public static IEnumerable<object[]> HeaderCell_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DataGridViewRowHeaderCell() };
        }

        [WinFormsTheory]
        [MemberData(nameof(HeaderCell_Set_TestData))]
        public void DataGridViewRow_HeaderCell_Set_GetReturnsExpected(DataGridViewRowHeaderCell value)
        {
            using var row = new DataGridViewRow
            {
                HeaderCell = value
            };
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

        [WinFormsTheory]
        [MemberData(nameof(HeaderCell_Set_TestData))]
        public void DataGridViewRow_HeaderCell_SetNonNullOldValue_GetReturnsExpected(DataGridViewRowHeaderCell value)
        {
            using var oldValue = new DataGridViewRowHeaderCell();
            using var row = new DataGridViewRow
            {
                HeaderCell = oldValue
            };
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

        [WinFormsFact]
        public void DataGridViewRow_HeaderCell_SetAlreadyHasBand_GetReturnsExpected()
        {
            using var value = new DataGridViewRowHeaderCell();
            using var otherRow = new DataGridViewRow
            {
                HeaderCell = value
            };
            using var row = new DataGridViewRow
            {
                HeaderCell = value
            };
            Assert.Same(value, row.HeaderCell);
            Assert.NotSame(value, otherRow.HeaderCell);
            Assert.Equal(row, row.HeaderCell.OwningRow);

            // Set same.
            row.HeaderCell = value;
            Assert.Same(value, row.HeaderCell);
            Assert.NotSame(value, otherRow.HeaderCell);
            Assert.Equal(row, row.HeaderCell.OwningRow);
        }

        [WinFormsTheory]
        [MemberData(nameof(HeaderCell_Set_TestData))]
        public void DataGridViewRow_HeaderCell_SetWithDataGridView_GetReturnsExpected(DataGridViewRowHeaderCell value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

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

        [WinFormsTheory]
        [MemberData(nameof(HeaderCell_Set_TestData))]
        public void DataGridViewRow_HeaderCell_SetWithDataGridViewNonNullOldValue_GetReturnsExpected(DataGridViewRowHeaderCell value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            using var oldValue = new DataGridViewRowHeaderCell();
            row.HeaderCell = oldValue;

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

        [WinFormsTheory]
        [MemberData(nameof(HeaderCell_Set_TestData))]
        public void DataGridViewRow_HeaderCell_SetShared_GetReturnsExpected(DataGridViewRowHeaderCell value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);

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

        [WinFormsTheory]
        [MemberData(nameof(HeaderCell_Set_TestData))]
        public void DataGridViewRow_HeaderCell_SetSharedNonNullOldValue_GetReturnsExpected(DataGridViewRowHeaderCell value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            using var oldValue = new DataGridViewRowHeaderCell();
            row.HeaderCell = oldValue;

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

        [WinFormsFact]
        public void DataGridViewRow_HeaderCell_SetWithDataGridView_CallsRowHeaderCellChanged()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Same(row, e.Row);
            };
            control.RowHeaderCellChanged += handler;

            // Set non-null.
            using var cell1 = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell1;
            Assert.Same(cell1, row.HeaderCell);
            Assert.Equal(1, callCount);

            // Set same.
            row.HeaderCell = cell1;
            Assert.Same(cell1, row.HeaderCell);
            Assert.Equal(1, callCount);

            // Set different.
            using var cell2 = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell2;
            Assert.Same(cell2, row.HeaderCell);
            Assert.Equal(2, callCount);

            // Set null.
            row.HeaderCell = null;
            Assert.NotNull(row.HeaderCell);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.RowHeaderCellChanged -= handler;
            row.HeaderCell = cell1;
            Assert.Equal(cell1, row.HeaderCell);
            Assert.Equal(3, callCount);
        }

        public static IEnumerable<object[]> HeaderCellCore_SetInvalid_TestData()
        {
            yield return new object[] { new DataGridViewColumnHeaderCell() };
            yield return new object[] { new DataGridViewHeaderCell() };
        }

        [WinFormsTheory]
        [MemberData(nameof(HeaderCellCore_SetInvalid_TestData))]
        public void DataGridViewRow_HeaderCellCore_SetInvalid_ThrowsArgumentException(DataGridViewHeaderCell value)
        {
            using var row = new SubDataGridViewRow();
            Assert.Throws<ArgumentException>("value", () => row.HeaderCellCore = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridViewAutoSizeRowsMode))]
        public void DataGridViewRow_Height_GetWithDataGridView_ReturnsExpected(DataGridViewAutoSizeRowsMode autoSizeRowsMode)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                AutoSizeRowsMode = autoSizeRowsMode
            };
            int callCount = 0;
            control.RowHeightInfoNeeded += (sender, e) => callCount++;
            DataGridViewRow row = control.Rows[0];
            Assert.Equal(Control.DefaultFont.Height + 9, row.Height);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridViewAutoSizeRowsMode))]
        public void DataGridViewRow_Height_GetShared_ReturnsExpected(DataGridViewAutoSizeRowsMode autoSizeRowsMode)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                AutoSizeRowsMode = autoSizeRowsMode
            };
            int callCount = 0;
            control.RowHeightInfoNeeded += (sender, e) => callCount++;
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Equal(Control.DefaultFont.Height + 9, row.Height);
            Assert.Equal(0, callCount);
        }

        public static IEnumerable<object[]> Height_NeedsHeightInfo_TestData()
        {
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 6, 2, 6, 1 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 6, 9, 9, 1 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 0, 2, 3, 1 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, -1, 2, 3, 1 };

            yield return new object[] { DataGridViewAutoSizeRowsMode.AllHeaders, 2, 1, Control.DefaultFont.Height + 9, 0 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders, 2, 1, Control.DefaultFont.Height + 9, 0 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.AllCells, 2, 1, Control.DefaultFont.Height + 9, 0 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.DisplayedHeaders, 2, 1, Control.DefaultFont.Height + 9, 0 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders, 2, 1, Control.DefaultFont.Height + 9, 0 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.DisplayedCells, 2, 1, Control.DefaultFont.Height + 9, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Height_NeedsHeightInfo_TestData))]
        public void DataGridViewRow_Height_GetWithDataGridViewVirtualMode_CallsHeightInfoNeeded(DataGridViewAutoSizeRowsMode autoSizeRowsMode, int heightResult, int minimumHeightResult, int expectedHeight, int expectedCallCount)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                AutoSizeRowsMode = autoSizeRowsMode,
                VirtualMode = true
            };
            DataGridViewRow row = control.Rows[0];
            int callCount = 0;
            DataGridViewRowHeightInfoNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal(Control.DefaultFont.Height + 9, e.Height);
                Assert.Equal(3, e.MinimumHeight);
                e.Height = heightResult;
                e.MinimumHeight = minimumHeightResult;
            };
            control.RowHeightInfoNeeded += handler;

            Assert.Equal(expectedHeight, row.Height);
            Assert.Equal(expectedCallCount, callCount);

            // Remove the handler.
            control.RowHeightInfoNeeded -= handler;
            Assert.Equal(Control.DefaultFont.Height + 9, row.Height);
            Assert.Equal(expectedCallCount, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Height_NeedsHeightInfo_TestData))]
        public void DataGridViewRow_Height_GetWithDataGridViewDataSource_CallsHeightInfoNeeded(DataGridViewAutoSizeRowsMode autoSizeRowsMode, int heightResult, int minimumHeightResult, int expectedHeight, int expectedCallCount)
        {
            using var control = new DataGridView
            {
                AutoSizeRowsMode = autoSizeRowsMode,
                DataSource = new[] { new { Name = "Name" } }
            };
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);
            DataGridViewRow row = control.Rows[0];
            int callCount = 0;
            DataGridViewRowHeightInfoNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal(Control.DefaultFont.Height + 9, e.Height);
                Assert.Equal(3, e.MinimumHeight);
                e.Height = heightResult;
                e.MinimumHeight = minimumHeightResult;
            };
            control.RowHeightInfoNeeded += handler;

            Assert.Equal(expectedHeight, row.Height);
            Assert.Equal(expectedCallCount, callCount);

            // Remove the handler.
            control.RowHeightInfoNeeded -= handler;
            Assert.Equal(Control.DefaultFont.Height + 9, row.Height);
            Assert.Equal(expectedCallCount, callCount);
        }

        public static IEnumerable<object[]> Height_Set_TestData()
        {
            yield return new object[] { -1, 3 };
            yield return new object[] { 0, 3 };
            yield return new object[] { 1, 3 };
            yield return new object[] { 3, 3 };
            yield return new object[] { 4, 4 };
            yield return new object[] { 65536, 65536 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Height_Set_TestData))]
        public void DataGridViewRow_Height_Set_GetReturnsExpected(int value, int expected)
        {
            using var row = new DataGridViewRow
            {
                Height = value
            };
            Assert.Equal(expected, row.Height);

            // Set same.
            row.Height = value;
            Assert.Equal(expected, row.Height);
        }

        public static IEnumerable<object[]> Height_SetWithDataGridView_TestData()
        {
            foreach (DataGridViewAutoSizeRowsMode autoSizeRowsMode  in Enum.GetValues(typeof(DataGridViewAutoSizeRowsMode)))
            {
                if (autoSizeRowsMode == DataGridViewAutoSizeRowsMode.None)
                {
                    continue;
                }

                yield return new object[] { autoSizeRowsMode, -1, Control.DefaultFont.Height + 9 };
                yield return new object[] { autoSizeRowsMode, 0, Control.DefaultFont.Height + 9 };
                yield return new object[] { autoSizeRowsMode, 1, Control.DefaultFont.Height + 9 };
                yield return new object[] { autoSizeRowsMode, 3, Control.DefaultFont.Height + 9 };
                yield return new object[] { autoSizeRowsMode, 4, Control.DefaultFont.Height + 9 };
                yield return new object[] { autoSizeRowsMode, 65536, Control.DefaultFont.Height + 9 };
            }

            yield return new object[] { DataGridViewAutoSizeRowsMode.None, -1, 3 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 0, 3 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 1, 3 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 3, 3 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 4, 4 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 65536, 65536 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Height_SetWithDataGridView_TestData))]
        public void DataGridViewRow_Height_SetWithDataGridView_GetReturnsExpected(DataGridViewAutoSizeRowsMode autoSizeRowsMode, int value, int expected)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                AutoSizeRowsMode = autoSizeRowsMode
            };
            DataGridViewRow row = control.Rows[0];

            row.Height = value;
            Assert.Equal(expected, row.Height);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.Height = value;
            Assert.Equal(expected, row.Height);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRow_Height_SetWithDataGridView_CallsRowHeightChanged()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Same(row, e.Row);
            };
            control.RowHeightChanged += handler;

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
            control.RowHeightChanged -= handler;
            row.Height = 4;
            Assert.Equal(4, row.Height);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [InlineData(65537)]
        public void DataGridViewRow_Height_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            using var row = new DataGridViewRow();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => row.Height = value);
        }

        [WinFormsFact]
        public void DataGridViewRow_Height_SetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.Height = -1);
        }

        public static IEnumerable<object[]> InheritedStyle_GetWithDataGridView_TestData()
        {
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
                1, complete1, null, null, null,
                complete1
            };
            yield return new object[]
            {
                1, new DataGridViewCellStyle(), complete1, null, null,
                complete1
            };
            yield return new object[]
            {
                1, null, null, complete1, null,
                complete1
            };
            yield return new object[]
            {
                1, null, new DataGridViewCellStyle(), complete1, null,
                complete1
            };
            yield return new object[]
            {
                1, null, null, null, complete1,
                complete1
            };
            yield return new object[]
            {
                1, null, null, new DataGridViewCellStyle(), complete1,
                complete1
            };
            yield return new object[]
            {
                2, null, null, null, complete1,
                complete1
            };
            yield return new object[]
            {
                2, null, null, new DataGridViewCellStyle(), complete1,
                complete1
            };
            yield return new object[]
            {
                1, null, complete1, complete2, null,
                complete2
            };
            yield return new object[]
            {
                2, null, complete1, complete2, null,
                complete1
            };
        }

        [WinFormsTheory]
        [MemberData(nameof(InheritedStyle_GetWithDataGridView_TestData))]
        public void DataGridViewRow_InheritedStyle_Get_ReturnsExpected(int index, DataGridViewCellStyle rowDefaultCellStyle, DataGridViewCellStyle rowsDefaultCellStyle, DataGridViewCellStyle alternatingRowsDefaultCellStyle, DataGridViewCellStyle gridDefaultCellStyle, DataGridViewCellStyle expected)
        {
            using var control = new DataGridView
            {
                ColumnCount = 2
            };

            if (rowsDefaultCellStyle != null)
            {
                control.RowsDefaultCellStyle = rowsDefaultCellStyle;
            }
            if (alternatingRowsDefaultCellStyle != null)
            {
                control.AlternatingRowsDefaultCellStyle = alternatingRowsDefaultCellStyle;
            }
            if (gridDefaultCellStyle != null)
            {
                control.DefaultCellStyle = gridDefaultCellStyle;
            };

            control.Rows.Add(new SubDataGridViewRow());
            control.Rows.Add(new SubDataGridViewRow());

            DataGridViewRow row = control.Rows[index];
            if (rowDefaultCellStyle != null)
            {
                row.DefaultCellStyle = rowDefaultCellStyle;
            }
            Assert.Equal(expected, row.InheritedStyle);
        }

        [WinFormsFact]
        public void DataGridViewRow_InheritedStyle_GetWithoutDataGridView_ThrowsInvalidOperationException()
        {
            using var row = new DataGridViewRow();
            Assert.Throws<InvalidOperationException>(() => row.InheritedStyle);
        }

        [WinFormsFact]
        public void DataGridViewRow_InheritedStyle_GetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.InheritedStyle);
        }

        [WinFormsFact]
        public void DataGridViewRow_IsNewRow_GetWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 2
            };
            DataGridViewRow row = control.Rows[0];
            Assert.False(row.IsNewRow);
        }

        [WinFormsFact]
        public void DataGridViewRow_IsNewRow_GetWithDataGridViewNewRow_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.True(row.IsNewRow);
        }

        [WinFormsFact]
        public void DataGridViewRow_IsNewRow_GetSharedNotNewRow_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 2
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.False(row.IsNewRow);
        }

        [WinFormsFact]
        public void DataGridViewRow_IsNewRow_GetSharedNewRow_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.False(row.IsNewRow);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridViewAutoSizeRowsMode))]
        public void DataGridViewRow_MinimumHeight_GetWithDataGridView_ReturnsExpected(DataGridViewAutoSizeRowsMode autoSizeRowsMode)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                AutoSizeRowsMode = autoSizeRowsMode
            };
            int callCount = 0;
            control.RowHeightInfoNeeded += (sender, e) => callCount++;
            DataGridViewRow row = control.Rows[0];
            Assert.Equal(3, row.MinimumHeight);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridViewAutoSizeRowsMode))]
        public void DataGridViewRow_MinimumHeight_GetShared_ReturnsExpected(DataGridViewAutoSizeRowsMode autoSizeRowsMode)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                AutoSizeRowsMode = autoSizeRowsMode
            };
            int callCount = 0;
            control.RowHeightInfoNeeded += (sender, e) => callCount++;
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Equal(3, row.MinimumHeight);
            Assert.Equal(0, callCount);
        }

        public static IEnumerable<object[]> MinimumHeight_NeedsHeightInfo_TestData()
        {
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 6, 2, 2, 1 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 6, 9, 9, 1 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 0, 2, 2, 1 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, -1, 2, 2, 1 };

            yield return new object[] { DataGridViewAutoSizeRowsMode.AllHeaders, 2, 1, 3, 0 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders, 2, 1, 3, 0 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.AllCells, 2, 1, 3, 0 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.DisplayedHeaders, 2, 1, 3, 0 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders, 2, 1, 3, 0 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.DisplayedCells, 2, 1, 3, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumHeight_NeedsHeightInfo_TestData))]
        public void DataGridViewRow_MinimumHeight_GetWithDataGridViewVirtualMode_CallsHeightInfoNeeded(DataGridViewAutoSizeRowsMode autoSizeRowsMode, int heightResult, int minimumHeightResult, int expectedMinimumHeight, int expectedCallCount)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                AutoSizeRowsMode = autoSizeRowsMode,
                VirtualMode = true
            };
            DataGridViewRow row = control.Rows[0];
            int callCount = 0;
            DataGridViewRowHeightInfoNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal(Control.DefaultFont.Height + 9, e.Height);
                Assert.Equal(3, e.MinimumHeight);
                e.Height = heightResult;
                e.MinimumHeight = minimumHeightResult;
            };
            control.RowHeightInfoNeeded += handler;

            Assert.Equal(expectedMinimumHeight, row.MinimumHeight);
            Assert.Equal(expectedCallCount, callCount);

            // Remove the handler.
            control.RowHeightInfoNeeded -= handler;
            Assert.Equal(3, row.MinimumHeight);
            Assert.Equal(expectedCallCount, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumHeight_NeedsHeightInfo_TestData))]
        public void DataGridViewRow_MinimumHeight_GetWithDataGridViewDataSource_CallsHeightInfoNeeded(DataGridViewAutoSizeRowsMode autoSizeRowsMode, int heightResult, int minimumHeightResult, int expectedMinimumHeight, int expectedCallCount)
        {
            using var control = new DataGridView
            {
                AutoSizeRowsMode = autoSizeRowsMode,
                DataSource = new[] { new { Name = "Name" } }
            };
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);
            DataGridViewRow row = control.Rows[0];
            int callCount = 0;
            DataGridViewRowHeightInfoNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal(Control.DefaultFont.Height + 9, e.Height);
                Assert.Equal(3, e.MinimumHeight);
                e.Height = heightResult;
                e.MinimumHeight = minimumHeightResult;
            };
            control.RowHeightInfoNeeded += handler;

            Assert.Equal(expectedMinimumHeight, row.MinimumHeight);
            Assert.Equal(expectedCallCount, callCount);

            // Remove the handler.
            control.RowHeightInfoNeeded -= handler;
            Assert.Equal(3, row.MinimumHeight);
            Assert.Equal(expectedCallCount, callCount);
        }

        public static IEnumerable<object[]> MinimumHeight_Set_TestData()
        {
            yield return new object[] { 2, 2, Control.DefaultFont.Height + 9 };
            yield return new object[] { 3, 3, Control.DefaultFont.Height + 9 };
            yield return new object[] { 4, 4, Control.DefaultFont.Height + 9 };
            yield return new object[] { 65536, 65536, 65536 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumHeight_Set_TestData))]
        public void DataGridViewRow_MinimumHeight_Set_GetReturnsExpected(int value, int expectedMinimumHeight, int expectedHeight)
        {
            using var row = new DataGridViewRow
            {
                MinimumHeight = value
            };
            Assert.Equal(expectedMinimumHeight, row.MinimumHeight);
            Assert.Equal(expectedHeight, row.Height);

            // Set same.
            row.MinimumHeight = value;
            Assert.Equal(expectedMinimumHeight, row.MinimumHeight);
            Assert.Equal(expectedHeight, row.Height);
        }

        public static IEnumerable<object[]> MinimumHeight_SetWithDataGridView_TestData()
        {
            foreach (DataGridViewAutoSizeRowsMode autoSizeRowsMode  in Enum.GetValues(typeof(DataGridViewAutoSizeRowsMode)))
            {
                if (autoSizeRowsMode == DataGridViewAutoSizeRowsMode.None)
                {
                    continue;
                }

                yield return new object[] { autoSizeRowsMode, 2, 2, Control.DefaultFont.Height + 9 };
                yield return new object[] { autoSizeRowsMode, 3, 3, Control.DefaultFont.Height + 9 };
                yield return new object[] { autoSizeRowsMode, 4, 4, Control.DefaultFont.Height + 9 };
                yield return new object[] { autoSizeRowsMode, 65536, 65536, Control.DefaultFont.Height + 9 };
            }

            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 2, 2, Control.DefaultFont.Height + 9 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 3, 3, Control.DefaultFont.Height + 9 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 4, 4, Control.DefaultFont.Height + 9 };
            yield return new object[] { DataGridViewAutoSizeRowsMode.None, 65536, 65536, 65536 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumHeight_SetWithDataGridView_TestData))]
        public void DataGridViewRow_MinimumHeight_SetWithDataGridView_GetReturnsExpected(DataGridViewAutoSizeRowsMode autoSizeRowsMode, int value, int expectedMinimumHeight, int expectedHeight)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                AutoSizeRowsMode = autoSizeRowsMode
            };
            DataGridViewRow row = control.Rows[0];

            row.MinimumHeight = value;
            Assert.Equal(expectedMinimumHeight, row.MinimumHeight);
            Assert.Equal(expectedHeight, row.Height);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.MinimumHeight = value;
            Assert.Equal(expectedMinimumHeight, row.MinimumHeight);
            Assert.Equal(expectedHeight, row.Height);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRow_MinimumHeight_SetWithDataGridView_CallsRowMinimumHeightChanged()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            int callCount = 0;
            DataGridViewRowEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Same(row, e.Row);
            };
            control.RowMinimumHeightChanged += handler;

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
            control.RowMinimumHeightChanged -= handler;
            row.MinimumHeight = 4;
            Assert.Equal(4, row.MinimumHeight);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(65537)]
        public void DataGridViewRow_MinimumHeight_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            using var row = new DataGridViewRow();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => row.MinimumHeight = value);
            Assert.Equal(3, row.MinimumHeight);
            Assert.Equal(Control.DefaultFont.Height + 9, row.Height);
        }

        [WinFormsFact]
        public void DataGridViewRow_MinimumHeight_SetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.MinimumHeight = -1);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewElementStates.None, false)]
        [InlineData(DataGridViewElementStates.ReadOnly, false)]
        [InlineData(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Visible, false)]
        [InlineData(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected, false)]
        public void DataGridViewRow_ReadOnly_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state, bool expected)
        {
            using var row = new CustomStateDataGridViewRow
            {
                StateResult = state
            };
            Assert.Equal(expected, row.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewRow_ReadOnly_GetWithDataGridView_ReturnsExpected(bool dataGridViewReadOnly)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                ReadOnly = dataGridViewReadOnly
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Equal(dataGridViewReadOnly, row.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewRow_ReadOnly_GetShared_ThrowsInvalidOperationException(bool dataGridViewReadOnly)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                ReadOnly = dataGridViewReadOnly
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewRow_ReadOnly_Set_GetReturnsExpected(bool value)
        {
            using var row = new DataGridViewRow
            {
                ReadOnly = value
            };
            Assert.Equal(value, row.ReadOnly);
            Assert.Equal(value, (row.State & DataGridViewElementStates.ReadOnly) != 0);

            // Set same.
            row.ReadOnly = value;
            Assert.Equal(value, row.ReadOnly);
            Assert.Equal(value, (row.State & DataGridViewElementStates.ReadOnly) != 0);

            // Set different.
            row.ReadOnly = !value;
            Assert.Equal(!value, row.ReadOnly);
            Assert.Equal(!value, (row.State & DataGridViewElementStates.ReadOnly) != 0);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void DataGridViewRow_ReadOnly_SetWithDataGridView_GetReturnsExpected(bool dataGridViewReadOnly, bool value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                ReadOnly = dataGridViewReadOnly
            };
            DataGridViewRow row = control.Rows[0];

            row.ReadOnly = value;
            Assert.Equal(dataGridViewReadOnly || value, row.ReadOnly);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.ReadOnly = value;
            Assert.Equal(dataGridViewReadOnly ||value, row.ReadOnly);
            Assert.False(control.IsHandleCreated);

            // Set different.
            row.ReadOnly = !value;
            Assert.Equal(dataGridViewReadOnly ||!value, row.ReadOnly);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRow_ReadOnly_SetWithDataGridViewReadOnly_Nop()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                ReadOnly = true
            };
            DataGridViewRow row = control.Rows[0];
            row.ReadOnly = true;
            Assert.True(row.ReadOnly);

            // Unset read only.
            control.ReadOnly = false;
            Assert.False(row.ReadOnly);
        }

        [WinFormsFact]
        public void DataGridViewRow_ReadOnly_SetWithCells_Success()
        {
            using var row = new DataGridViewRow();
            using var cell1 = new SubDataGridViewCell();
            using var cell2 = new SubDataGridViewCell();
            row.Cells.Add(cell1);
            row.Cells.Add(cell2);
            cell2.ReadOnly = true;

            // Set same.
            row.ReadOnly = false;
            Assert.False(row.ReadOnly);
            Assert.False(cell1.ReadOnly);
            Assert.True(cell2.ReadOnly);

            // Set true.
            row.ReadOnly = true;
            Assert.True(row.ReadOnly);
            Assert.True(cell1.ReadOnly);
            Assert.True(cell2.ReadOnly);

            // Set false.
            row.ReadOnly = false;
            Assert.False(row.ReadOnly);
            Assert.False(cell1.ReadOnly);
            Assert.False(cell2.ReadOnly);
        }

        [WinFormsFact]
        public void DataGridViewRow_ReadOnly_SetWithCellsWithDataGridView_Success()
        {
            using var control = new DataGridView
            {
                ColumnCount = 2,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            row.Cells[1].ReadOnly = true;

            // Set same.
            row.ReadOnly = false;
            Assert.False(row.ReadOnly);
            Assert.False(row.Cells[0].ReadOnly);
            Assert.False(row.Cells[1].ReadOnly);

            // Set true.
            row.ReadOnly = true;
            Assert.True(row.ReadOnly);
            Assert.True(row.Cells[0].ReadOnly);
            Assert.True(row.Cells[1].ReadOnly);

            // Set false.
            row.ReadOnly = false;
            Assert.False(row.ReadOnly);
            Assert.False(row.Cells[0].ReadOnly);
            Assert.False(row.Cells[1].ReadOnly);
        }

        [WinFormsFact]
        public void DataGridViewRow_ReadOnly_SetWithDataGridView_CallsRowStateChanged()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            int callCount = 0;
            DataGridViewRowStateChangedEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Same(row, e.Row);
            };
            control.RowStateChanged += handler;

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
            control.RowStateChanged -= handler;
            row.ReadOnly = true;
            Assert.True(row.ReadOnly);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_ReadOnly_SetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.ReadOnly = true);
        }

        [WinFormsTheory]
        [InlineData(true, DataGridViewTriState.True)]
        [InlineData(false, DataGridViewTriState.False)]
        public void DataGridViewRow_Resizable_GetWithDataGridView_ReturnsExpected(bool allowUserToResizeRows, DataGridViewTriState expected)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                AllowUserToResizeRows = allowUserToResizeRows
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Equal(expected, row.Resizable);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewRow_Resizable_GetShared_ReturnsExpected(bool allowUserToResizeRows)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                AllowUserToResizeRows = allowUserToResizeRows
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.Resizable);
        }

        [WinFormsFact]
        public void DataGridViewRow_Resizable_GetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.Resizable);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridViewTriState))]
        public void DataGridViewRow_Resizable_Set_GetReturnsExpected(DataGridViewTriState value)
        {
            using var row = new DataGridViewRow
            {
                Resizable = value
            };
            Assert.Equal(value, row.Resizable);

            // Set same.
            row.Resizable = value;
            Assert.Equal(value, row.Resizable);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridViewTriState))]
        public void DataGridViewRow_Resizable_SetWithCustomOldValue_GetReturnsExpected(DataGridViewTriState value)
        {
            using var row = new DataGridViewRow
            {
                Resizable = DataGridViewTriState.True
            };

            row.Resizable = value;
            Assert.Equal(value, row.Resizable);

            // Set same.
            row.Resizable = value;
            Assert.Equal(value, row.Resizable);
        }

        public static IEnumerable<object[]> Resizable_SetWithDataGridView_TestData()
        {
            yield return new object[] { true, DataGridViewTriState.NotSet, DataGridViewTriState.True };
            yield return new object[] { false, DataGridViewTriState.NotSet, DataGridViewTriState.False };
            yield return new object[] { true, DataGridViewTriState.True, DataGridViewTriState.True };
            yield return new object[] { false, DataGridViewTriState.True, DataGridViewTriState.True };
            yield return new object[] { true, DataGridViewTriState.False, DataGridViewTriState.False };
            yield return new object[] { false, DataGridViewTriState.False, DataGridViewTriState.False };
        }

        [WinFormsTheory]
        [MemberData(nameof(Resizable_SetWithDataGridView_TestData))]
        public void DataGridViewRow_Resizable_SetWithDataGridView_GetReturnsExpected(bool allowUserToResizeRows, DataGridViewTriState value, DataGridViewTriState expected)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                AllowUserToResizeRows = allowUserToResizeRows
            };
            DataGridViewRow row = control.Rows[0];

            row.Resizable = value;
            Assert.Equal(expected, row.Resizable);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.Resizable = value;
            Assert.Equal(expected, row.Resizable);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRow_Resizable_SetWithDataGridView_CallsRowStateChanged()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            int callCount = 0;
            DataGridViewRowStateChangedEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Same(row, e.Row);
            };
            control.RowStateChanged += handler;

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
            control.RowStateChanged -= handler;
            row.Resizable = DataGridViewTriState.False;
            Assert.Equal(DataGridViewTriState.False, row.Resizable);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Resizable_SetWithDataGridView_TestData))]
        public void DataGridViewRow_Resizable_SetWithDataGridViewWithCustomOldValue_GetReturnsExpected(bool allowUserToResizeRows, DataGridViewTriState value, DataGridViewTriState expected)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                AllowUserToResizeRows = allowUserToResizeRows
            };
            DataGridViewRow row = control.Rows[0];
            row.Resizable = DataGridViewTriState.True;

            row.Resizable = value;
            Assert.Equal(expected, row.Resizable);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.Resizable = value;
            Assert.Equal(expected, row.Resizable);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DataGridViewTriState))]
        public void DataGridViewRow_Resizable_SetInvalid_ThrowsInvalidEnumArgumentException(DataGridViewTriState value)
        {
            using var row = new DataGridViewRow();
            Assert.Throws<InvalidEnumArgumentException>("value", () => row.Resizable = value);
        }

        [WinFormsFact]
        public void DataGridViewRow_Selected_GetWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
            };
            DataGridViewRow row = control.Rows[0];
            Assert.False(row.Selected);
        }

        [WinFormsFact]
        public void DataGridViewRow_Selected_GetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.Selected);
        }

        [WinFormsFact]
        public void DataGridViewRow_Selected_SetWithoutDataGridView_ThrowsInvalidOperationException()
        {
            using var row = new DataGridViewRow();
            Assert.Throws<InvalidOperationException>(() => row.Selected = true);
            Assert.False(row.Selected);

            row.Selected = false;
            Assert.False(row.Selected);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewSelectionMode.CellSelect, false)]
        [InlineData(DataGridViewSelectionMode.FullRowSelect, true)]
        [InlineData(DataGridViewSelectionMode.FullColumnSelect, false)]
        [InlineData(DataGridViewSelectionMode.RowHeaderSelect, true)]
        [InlineData(DataGridViewSelectionMode.ColumnHeaderSelect, false)]
        public void DataGridViewRow_Selected_SetWithDataGridView_GetReturnsExpected(DataGridViewSelectionMode selectionMode, bool selected)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            control.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            control.SelectionMode = selectionMode;
            DataGridViewRow row = control.Rows[0];

            // Set true.
            row.Selected = true;
            Assert.Equal(selected, (control.SelectedRows).Contains(row));
            Assert.Equal(selected, row.Selected);

            // Set same.
            row.Selected = true;
            Assert.Equal(selected, (control.SelectedRows).Contains(row));
            Assert.Equal(selected, row.Selected);

            // Set different.
            row.Selected = false;
            Assert.False((control.SelectedRows).Contains(row));
            Assert.False(row.Selected);
        }

        [WinFormsFact]
        public void DataGridViewRow_Selected_SetMultipleNotMultiSelect_Success()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                MultiSelect = false
            };
            control.Rows.Add(new DataGridViewRow());
            control.Rows.Add(new DataGridViewRow());
            DataGridViewRow row1 = control.Rows[0];
            DataGridViewRow row2 = control.Rows[1];

            row1.Selected = true;
            Assert.Same(row1, Assert.Single(control.SelectedRows));
            Assert.True(row1.Selected);
            Assert.False(row2.Selected);

            row2.Selected = true;
            Assert.Same(row2, Assert.Single(control.SelectedRows));
            Assert.False(row1.Selected);
            Assert.True(row2.Selected);

            row1.Selected = false;
            Assert.Empty(control.SelectedRows);
            Assert.False(row1.Selected);
            Assert.False(row2.Selected);

            row2.Selected = false;
            Assert.Empty(control.SelectedRows);
            Assert.False(row1.Selected);
            Assert.False(row2.Selected);
        }

        [WinFormsFact]
        public void DataGridViewRow_Selected_SetMultipleMultiSelect_Success()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                MultiSelect = true
            };
            control.Rows.Add(new DataGridViewRow());
            control.Rows.Add(new DataGridViewRow());
            DataGridViewRow row1 = control.Rows[0];
            DataGridViewRow row2 = control.Rows[1];

            row1.Selected = true;
            Assert.Same(row1, Assert.Single(control.SelectedRows));
            Assert.True(row1.Selected);
            Assert.False(row2.Selected);

            row2.Selected = true;
            Assert.Equal(new DataGridViewRow[] { row2, row1 }, control.SelectedRows.Cast<DataGridViewRow>());
            Assert.True(row1.Selected);
            Assert.True(row2.Selected);

            row1.Selected = false;
            Assert.Same(row2, Assert.Single(control.SelectedRows));
            Assert.False(row1.Selected);
            Assert.True(row2.Selected);

            row2.Selected = false;
            Assert.Empty(control.SelectedRows);
            Assert.False(row1.Selected);
            Assert.False(row2.Selected);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewRow_Selected_SetShared_ThrowsInvalidOperationException(bool value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.Selected = value);
        }

        [WinFormsFact]
        public void DataGridViewRow_State_GetWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Equal(DataGridViewElementStates.Visible, row.State);
        }

        [WinFormsFact]
        public void DataGridViewRow_State_GetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.State);
        }

        [WinFormsFact]
        public void DataGridViewRow_Tag_GetWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Null(row.Tag);
        }

        [WinFormsFact]
        public void DataGridViewRow_Tag_GetShared_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Null(row.Tag);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewRow_Tag_Set_GetReturnsExpected(string value)
        {
            using var row = new DataGridViewRow
            {
                Tag = value
            };
            Assert.Equal(value, row.Tag);

            // Set same.
            row.Tag = value;
            Assert.Equal(value, row.Tag);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewRow_Tag_SetWithNonNullOldValue_GetReturnsExpected(string value)
        {
            using var row = new DataGridViewRow
            {
                Tag = "OldValue"
            };

            row.Tag = value;
            Assert.Equal(value, row.Tag);

            // Set same.
            row.Tag = value;
            Assert.Equal(value, row.Tag);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewRow_Tag_SetWithDataGridView_GetReturnsExpected(string value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            row.Tag = value;
            Assert.Equal(value, row.Tag);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.Tag = value;
            Assert.Equal(value, row.Tag);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewRow_Tag_SetWithDataGridViewWithNonNullOldValue_GetReturnsExpected(string value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            row.Tag = "OldValue";

            row.Tag = value;
            Assert.Equal(value, row.Tag);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.Tag = value;
            Assert.Equal(value, row.Tag);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewRow_Tag_SetShared_GetReturnsExpected(string value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);

            row.Tag = value;
            Assert.Equal(value, row.Tag);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.Tag = value;
            Assert.Equal(value, row.Tag);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewRow_Tag_SetSharedWithNonNullOldValue_GetReturnsExpected(string value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            row.Tag = "OldValue";

            row.Tag = value;
            Assert.Equal(value, row.Tag);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.Tag = value;
            Assert.Equal(value, row.Tag);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRow_Visible_GetWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.True(row.Visible);
        }

        [WinFormsFact]
        public void DataGridViewRow_Visible_GetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.Visible);
        }

        public static IEnumerable<object[]> Visible_Set_TestData()
        {
            foreach (bool frozen in new bool[] { true, false })
            {
                yield return new object[] { frozen, true };
                yield return new object[] { frozen, false };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Visible_Set_TestData))]
        public void DataGridViewRow_Visible_Set_GetReturnsExpected(bool frozen, bool value)
        {
            using var row = new DataGridViewRow
            {
                Frozen = frozen,
                Visible = value
            };
            Assert.Equal(value, row.Visible);
            Assert.Equal(value, (row.State & DataGridViewElementStates.Visible) != 0);

            // Set same.
            row.Visible = value;
            Assert.Equal(value, row.Visible);
            Assert.Equal(value, (row.State & DataGridViewElementStates.Visible) != 0);

            // Set different.
            row.Visible = !value;
            Assert.Equal(!value, row.Visible);
            Assert.Equal(!value, (row.State & DataGridViewElementStates.Visible) != 0);
        }

        [WinFormsTheory]
        [MemberData(nameof(Visible_Set_TestData))]
        public void DataGridViewRow_Visible_SetWithDataGridView_GetReturnsExpected(bool frozen, bool value)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 2
            };
            DataGridViewRow row = control.Rows[0];
            row.Frozen = frozen;
            row.Visible = value;
            Assert.Equal(value, row.Visible);
            Assert.Equal(value, (row.State & DataGridViewElementStates.Visible) != 0);
            Assert.False(control.IsHandleCreated);

            // Set same.
            row.Visible = value;
            Assert.Equal(value, row.Visible);
            Assert.Equal(value, (row.State & DataGridViewElementStates.Visible) != 0);
            Assert.False(control.IsHandleCreated);

            // Set different.
            row.Visible = !value;
            Assert.Equal(!value, row.Visible);
            Assert.Equal(!value, (row.State & DataGridViewElementStates.Visible) != 0);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRow_Visible_SetWithDataGridView_CallsRowStateChanged()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 2
            };
            DataGridViewRow row = control.Rows[0];

            int callCount = 0;
            DataGridViewRowStateChangedEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Same(row, e.Row);
            };
            control.RowStateChanged += handler;

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
            control.RowStateChanged -= handler;
            row.Visible = true;
            Assert.True(row.Visible);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_Visible_SetNewRowIndexDifferent_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];

            Assert.Throws<InvalidOperationException>(() => row.Visible = false);
            Assert.True(row.Visible);

            row.Visible = true;
            Assert.True(row.Visible);
        }

        [WinFormsFact]
        public void DataGridViewRow_Visible_SetShared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.Visible = true);
            Assert.Throws<InvalidOperationException>(() => row.Visible = false);
        }

        public static IEnumerable<object[]> AdjustRowHeaderBorderStyle_WithDataGridView_TestData()
        {
            // Inset.
            foreach (bool isFirstDisplayedRow in new bool[] { true, false })
            {
                yield return new object[]
                {
                    true, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Inset, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
            }
            yield return new object[]
            {
                false, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Inset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                false, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Inset, true, true, false, true,
                false, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                true, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Inset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                true, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Inset, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                false, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Inset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                false, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Inset, true, true, false, true,
                false, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };

            // InsetDouble.
            foreach (bool isFirstDisplayedRow in new bool[] { true, false })
            {
                yield return new object[]
                {
                    true, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
                yield return new object[]
                {
                    true, true, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
                yield return new object[]
                {
                    false, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
                };
                yield return new object[]
                {
                    false, true, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
                };
            }
            yield return new object[]
            {
                true, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                true, false, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                false, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                false, false, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                false, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                false, false, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };

            // Outset.
            foreach (bool isFirstDisplayedRow in new bool[] { true, false })
            {
                yield return new object[]
                {
                    true, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Outset, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
            }
            yield return new object[]
            {
                false, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Outset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                false, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Outset, true, true, false, true,
                false, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                true, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Outset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                true, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Outset, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                false, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Outset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                false, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Outset, true, true, false, true,
                false, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };

            // OutsetDouble.
            foreach (bool isFirstDisplayedRow in new bool[] { true, false })
            {
                yield return new object[]
                {
                    true, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
                yield return new object[]
                {
                    true, true, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
                yield return new object[]
                {
                    false, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
                };
                yield return new object[]
                {
                    false, true, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
                };
            }
            yield return new object[]
            {
                true, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                true, false, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                false, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                false, false, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                false, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                false, false, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };

            // OutsetPartial.
            foreach (bool isFirstDisplayedRow in new bool[] { true, false })
            {
                foreach (bool isLastVisibleRow in new bool[] { true, false })
                {
                    yield return new object[]
                    {
                        true, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, isFirstDisplayedRow, isLastVisibleRow,
                        true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                    };
                    yield return new object[]
                    {
                        true, true, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, isFirstDisplayedRow, isLastVisibleRow,
                        true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                    };
                }
            }
            foreach (bool isLastVisibleRow in new bool[] { true, false })
            {
                yield return new object[]
                {
                    true, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, isLastVisibleRow,
                    true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
                yield return new object[]
                {
                    true, false, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, isLastVisibleRow,
                    true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.None
                };
            }
            yield return new object[]
            {
                false, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                false, true, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                false, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, false,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };
            yield return new object[]
            {
                false, true, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, false,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };
            yield return new object[]
            {
                false, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                false, true, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                false, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, false,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };
            yield return new object[]
            {
                false, true, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, false,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };
            yield return new object[]
            {
                false, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                false, false, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                false, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, false,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };
            yield return new object[]
            {
                false, false, RightToLeft.Yes, DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, false,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };

            // Single.
            foreach (bool isFirstDisplayedRow in new bool[] { true, false })
            {
                yield return new object[]
                {
                    true, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Single, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
                };
                yield return new object[]
                {
                    false, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Single, true, true, isFirstDisplayedRow, true,
                    true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single
                };
            }
            yield return new object[]
            {
                true, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Single, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                true, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Single, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                false, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Single, true, true, true, true,
                false, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single
            };
            yield return new object[]
            {
                false, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.Single, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single
            };

            // None.
            yield return new object[]
            {
                true, true, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.None, true, true, true, true,
                false, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                true, false, RightToLeft.No, DataGridViewAdvancedCellBorderStyle.None, true, true, true, true,
                false, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
        }

        [WinFormsTheory]
        [MemberData(nameof(AdjustRowHeaderBorderStyle_WithDataGridView_TestData))]
        public void DataGridViewRow_AdjustRowHeaderBorderStyle_InvokeWithDataGridVew_ReturnsExpected(bool enableHeadersVisualStyles, bool rowHeadersVisible, RightToLeft rightToLeft, DataGridViewAdvancedCellBorderStyle all, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow, bool returnsAdvanced, DataGridViewAdvancedCellBorderStyle expectedLeft, DataGridViewAdvancedCellBorderStyle expectedRight, DataGridViewAdvancedCellBorderStyle expectedTop, DataGridViewAdvancedCellBorderStyle expectedBottom)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                EnableHeadersVisualStyles = enableHeadersVisualStyles,
                ColumnHeadersVisible = rowHeadersVisible,
                RightToLeft = rightToLeft
            };
            DataGridViewRow row = control.Rows[0];

            if (!Application.RenderWithVisualStyles && row.DataGridView.EnableHeadersVisualStyles)
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

        public static IEnumerable<object[]> AdjustRowHeaderBorderStyle_WithoutDataGridView_TestData()
        {
            // Inset.
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.Inset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.Inset, true, true, false, true,
                false, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.Inset, true, false, true, true,
                false, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };

            // InsetDouble.
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.InsetDouble, true, false, false, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.Inset
            };

            // Outset.
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.Outset, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.Outset, true, true, false, true,
                false, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.Outset, true, false, true, true,
                false, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };

            // OutsetDouble.
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, false, false, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.Outset
            };

            // OutsetPartial.
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, false, false, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, false, false, false,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetPartial, DataGridViewAdvancedCellBorderStyle.OutsetPartial
            };

            // Single.
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.Single, true, true, false, true,
                true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single
            };

            // None.

            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.None, true, true, true, true,
                false, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.None
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.Single, true, true, true, true,
                false, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.Single, true, false, true, true,
                false, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.InsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.InsetDouble, true, false, true, true,
                true, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset, DataGridViewAdvancedCellBorderStyle.InsetDouble, DataGridViewAdvancedCellBorderStyle.Inset
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.OutsetDouble, true, false, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, true, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
            yield return new object[]
            {
                DataGridViewAdvancedCellBorderStyle.OutsetPartial, true, false, true, true,
                true, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset, DataGridViewAdvancedCellBorderStyle.OutsetDouble, DataGridViewAdvancedCellBorderStyle.Outset
            };
        }

        [WinFormsTheory]
        [MemberData(nameof(AdjustRowHeaderBorderStyle_WithoutDataGridView_TestData))]
        public void DataGridViewRow_AdjustRowHeaderBorderStyle_InvokeWithoutDataGridView_ReturnsExpected(DataGridViewAdvancedCellBorderStyle all, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow, bool returnsAdvanced, DataGridViewAdvancedCellBorderStyle expectedLeft, DataGridViewAdvancedCellBorderStyle expectedRight, DataGridViewAdvancedCellBorderStyle expectedTop, DataGridViewAdvancedCellBorderStyle expectedBottom)
        {
            using var row = new DataGridViewRow();

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

        [WinFormsFact]
        public void DataGridViewRow_Clone_Empty_Success()
        {
            using var source = new DataGridViewRow();
            using DataGridViewRow row = Assert.IsType<DataGridViewRow>(source.Clone());
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

        [WinFormsFact]
        public void DataGridViewRow_Clone_NoDataGridView_Success()
        {
            using var menu = new ContextMenuStrip();
            var style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomRight };
            using var cell = new DataGridViewRowHeaderCell();
            using var source = new DataGridViewRow
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
            using DataGridViewRow row = Assert.IsType<DataGridViewRow>(source.Clone());
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

        [WinFormsFact]
        public void DataGridViewRow_Clone_Subclass_Success()
        {
            using var menu = new ContextMenuStrip();
            var style = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.BottomRight
            };
            using var cell = new DataGridViewRowHeaderCell();
            using var source = new SubDataGridViewRow
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
            using SubDataGridViewRow row = Assert.IsType<SubDataGridViewRow>(source.Clone());
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

        [WinFormsFact]
        public void DataGridViewRow_Clone_WithDataGridView_Success()
        {
            using var menu = new ContextMenuStrip();
            var style = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.BottomRight
            };
            using var cell = new DataGridViewRowHeaderCell();
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 2
            };
            DataGridViewRow source = control.Rows[0];
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

            using DataGridViewRow row = Assert.IsType<DataGridViewRow>(source.Clone());
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

        [WinFormsFact]
        public void DataGridViewRow_CreateCells_InvokeNoColumns_Success()
        {
            using var control = new DataGridView();
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            row.CreateCells(control);
            Assert.Empty(row.Cells);
        }

        [WinFormsFact]
        public void DataGridViewRow_CreateCells_InvokeWithColumns_Success()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1
            };
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            row.CreateCells(control);
            Assert.Null(Assert.Single(row.Cells.Cast<DataGridViewCell>()).Value);
        }

        [WinFormsTheory]
        [InlineData(new object[] { new object[0] })]
        [InlineData(new object[] { new object[] { 1, 2, 3 } })]
        public void DataGridViewRow_CreateCells_InvokeNoColumnsWithValues_Success(object[] values)
        {
            using var control = new DataGridView();
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            row.CreateCells(control, values);
            Assert.Empty(row.Cells);
        }

        [WinFormsTheory]
        [InlineData(new object[] { new object[0], null })]
        [InlineData(new object[] { new object[] { 1 }, 1 })]
        [InlineData(new object[] { new object[] { 1, 2, 3 }, 1 })]
        public void DataGridViewRow_CreateCells_InvokeWithValues_Success(object[] values, object expectedValue)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1
            };
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            row.CreateCells(control, values);
            Assert.Equal(expectedValue, Assert.Single(row.Cells.Cast<DataGridViewCell>()).Value);
        }

        [WinFormsFact]
        public void DataGridViewRow_CreateCells_NullDataGridView_ThrowsArgumentNullException()
        {
            using var row = new DataGridViewRow();
            Assert.Throws<ArgumentNullException>("dataGridView", () => row.CreateCells(null));
            Assert.Throws<ArgumentNullException>("dataGridView", () => row.CreateCells(null, Array.Empty<object>()));
        }

        [WinFormsFact]
        public void DataGridViewRow_CreateCells_HasDataGridView_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            using var newControl = new DataGridView();
            DataGridViewRow row = control.Rows[0];
            Assert.Throws<InvalidOperationException>(() => row.CreateCells(newControl));
            Assert.Throws<InvalidOperationException>(() => row.CreateCells(newControl, Array.Empty<object>()));
        }

        [WinFormsFact]
        public void DataGridViewRow_CreateCells_ColumnHasNoCellTemplate_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView();
            control.Columns.Add("Name", "Text");
            control.Columns[0].CellTemplate = null;
            using var row = new DataGridViewRow();
            Assert.Throws<InvalidOperationException>(() => row.CreateCells(control));
            Assert.Throws<InvalidOperationException>(() => row.CreateCells(control, Array.Empty<object>()));
        }

        [WinFormsFact]
        public void DataGridViewRow_CreateCells_NullValues_ThrowsArgumentNullException()
        {
            using var row = new DataGridViewRow();
            using var control = new DataGridView();
            Assert.Throws<ArgumentNullException>("values", () => row.CreateCells(control, null));
        }

        [WinFormsFact]
        public void DataGridViewRow_CreateCellsInstance_Invoke_ReturnsEmpty()
        {
            using var row = new SubDataGridViewRow();
            Assert.Empty(row.CreateCellsInstance());
        }

        [WinFormsFact]
        public void DataGridViewRow_Dispose_WithoutContextMenuStrip_Nop()
        {
            using var row = new DataGridViewRow();
            row.Dispose();
            Assert.Null(row.ContextMenuStrip);

            // Call multiple times.
            row.Dispose();
            Assert.Null(row.ContextMenuStrip);
        }

        [WinFormsFact]
        public void DataGridViewRow_Dispose_WithContextMenuStrip_Success()
        {
            using var row = new DataGridViewRow
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

        [WinFormsTheory]
        [MemberData(nameof(DrawFocus_TestData))]
        public void DataGridViewRow_DrawFocus_Invoke_Success(Rectangle clipBounds, Rectangle bounds, int rowIndex, DataGridViewElementStates rowState, DataGridViewCellStyle cellStyle, bool cellsPaintSelectionBackground)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 1
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];
            row.DrawFocus(graphics, clipBounds, bounds, rowIndex, rowState, cellStyle, cellsPaintSelectionBackground);
        }

        [WinFormsFact]
        public void DataGridViewRow_DrawFocus_NoDataGridView_ThrowsInvalidOperationException()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            using var row = new SubDataGridViewRow();
            Assert.Throws<InvalidOperationException>(() => row.DrawFocus(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), -1, DataGridViewElementStates.None, new DataGridViewCellStyle(), true));
        }

        [WinFormsFact]
        public void DataGridViewRow_DrawFocus_NullGraphics_ThrowsArgumentNullException()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 1
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];
            Assert.Throws<ArgumentNullException>("graphics", () => row.DrawFocus(null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), -1, DataGridViewElementStates.None, new DataGridViewCellStyle(), true));
        }

        [WinFormsFact]
        public void DataGridViewRow_DrawFocus_NullCellStyle_ThrowsArgumentNullException()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 1
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];
            Assert.Throws<ArgumentNullException>("cellStyle", () => row.DrawFocus(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), -1, DataGridViewElementStates.None, null, true));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        public void DataGridViewRow_GetContextMenuStrip_Invoke_ReturnsExpected(int rowIndex)
        {
            using var row = new DataGridViewRow();
            Assert.Null(row.GetContextMenuStrip(rowIndex));

            // Set context menu.
            using var menu = new ContextMenuStrip();
            row.ContextMenuStrip = menu;
            Assert.Same(menu, row.GetContextMenuStrip(rowIndex));
        }

        [WinFormsFact]
        public void DataGridViewRow_GetContextMenuStrip_InvokeWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 2
            };
            int callCount = 0;
            control.RowContextMenuStripNeeded += (sender, e) => callCount++;
            DataGridViewRow row1 = control.Rows[0];
            using var menu1 = new ContextMenuStrip();
            row1.ContextMenuStrip = menu1;
            DataGridViewRow row2 = control.Rows[1];
            using var menu2 = new ContextMenuStrip();
            row2.ContextMenuStrip = menu2;
            Assert.Same(menu1, row1.GetContextMenuStrip(0));
            Assert.Same(menu1, row1.GetContextMenuStrip(0));
            Assert.Same(menu2, row2.GetContextMenuStrip(1));
            Assert.Same(menu2, row2.GetContextMenuStrip(1));
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRow_GetContextMenuStrip_InvokeWithDataGridViewVirtualMode_CallsRowContextMenuStripNeeded()
        {
            using var menu1 = new ContextMenuStrip();
            using var menu2 = new ContextMenuStrip();
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                VirtualMode = true
            };
            DataGridViewRow row = control.Rows[0];
            row.ContextMenuStrip = menu1;

            int callCount = 0;
            DataGridViewRowContextMenuStripNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Same(menu1, e.ContextMenuStrip);
                e.ContextMenuStrip = menu2;
            };
            control.RowContextMenuStripNeeded += handler;

            Assert.Same(menu2, row.GetContextMenuStrip(0));
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.RowContextMenuStripNeeded -= handler;
            Assert.Same(menu1, row.GetContextMenuStrip(0));
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_GetContextMenuStrip_InvokeWithDataGridViewDataSource_CallsRowContextMenuStripNeeded()
        {
            using var menu1 = new ContextMenuStrip();
            using var menu2 = new ContextMenuStrip();
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                DataSource = new[] { new { Name = "Name" } }
            };
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);
            DataGridViewRow row = control.Rows[0];
            row.ContextMenuStrip = menu1;

            int callCount = 0;
            DataGridViewRowContextMenuStripNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Same(menu1, e.ContextMenuStrip);
                e.ContextMenuStrip = menu2;
            };
            control.RowContextMenuStripNeeded += handler;

            Assert.Same(menu2, row.GetContextMenuStrip(0));
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.RowContextMenuStripNeeded -= handler;
            Assert.Same(menu1, row.GetContextMenuStrip(0));
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewRow_GetContextMenuStrip_InvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => row.GetContextMenuStrip(rowIndex));
        }

        [WinFormsFact]
        public void DataGridViewRow_GetContextMenuStrip_SharedRow_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Throws<InvalidOperationException>(() => row.GetContextMenuStrip(-1));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        public void DataGridViewRow_GetErrorText_Invoke_ReturnsExpected(int rowIndex)
        {
            using var row = new DataGridViewRow();
            Assert.Empty(row.GetErrorText(rowIndex));

            // Set context menu.
            row.ErrorText = "ErrorText";
            Assert.Equal("ErrorText", row.GetErrorText(rowIndex));
        }

        [WinFormsFact]
        public void DataGridViewRow_GetErrorText_InvokeWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 2
            };
            DataGridViewRow row1 = control.Rows[0];
            row1.ErrorText = "ErrorText1";
            DataGridViewRow row2 = control.Rows[1];
            row2.ErrorText = "ErrorText2";
            Assert.Same("ErrorText1", row1.GetErrorText(0));
            Assert.Same("ErrorText1", row1.GetErrorText(0));
            Assert.Same("ErrorText2", row2.GetErrorText(1));
            Assert.Same("ErrorText2", row2.GetErrorText(1));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRow_GetErrorText_InvokeWithDataGridViewVirtualMode_CallsRowErrorTextNeeded()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                VirtualMode = true
            };
            DataGridViewRow row = control.Rows[0];
            row.ErrorText = "ErrorText1";

            int callCount = 0;
            DataGridViewRowErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Same("ErrorText1", e.ErrorText);
                e.ErrorText = "ErrorText2";
            };
            control.RowErrorTextNeeded += handler;
            Assert.Same("ErrorText2", row.GetErrorText(0));
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.RowErrorTextNeeded -= handler;
            Assert.Same("ErrorText1", row.GetErrorText(0));
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_GetErrorText_InvokeWithDataGridViewDataSource_CallsRowErrorTextNeeded()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                DataSource = new[] { new { Name = "Name" } }
            };
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);
            DataGridViewRow row = control.Rows[0];
            row.ErrorText = "ErrorText1";

            int callCount = 0;
            DataGridViewRowErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Same("ErrorText1", e.ErrorText);
                e.ErrorText = "ErrorText2";
            };
            control.RowErrorTextNeeded += handler;
            Assert.Equal("ErrorText2", row.GetErrorText(0));
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.RowErrorTextNeeded -= handler;
            Assert.Equal("ErrorText1", row.GetErrorText(0));
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewRow_GetErrorText_InvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => row.GetErrorText(rowIndex));
        }

        [WinFormsFact]
        public void DataGridViewRow_GetErrorText_SharedRow_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Throws<InvalidOperationException>(() => row.GetErrorText(-1));
        }

        [WinFormsFact]
        public void DataGridViewRow_GetState_Invoke_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            Assert.Equal(DataGridViewElementStates.Visible, row.GetState(-1));
        }

        [WinFormsFact]
        public void DataGridViewRow_GetState_InvokeWithDataGridView_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 2
            };
            DataGridViewRow row = control.Rows[0];
            row.ReadOnly = true;
            Assert.Equal(DataGridViewElementStates.Visible | DataGridViewElementStates.ReadOnly, row.GetState(0));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewRow_GetState_InvokeShared_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 2
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Equal(DataGridViewElementStates.Visible, row.GetState(0));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewRow_GetState_NoDataGridViewInvalidRowIndex_ThrowsArgumentException(int rowIndex)
        {
            using var row = new DataGridViewRow();
            Assert.Throws<ArgumentException>("rowIndex", () => row.GetState(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewRow_GetState_InvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => row.GetState(rowIndex));
        }

        public static IEnumerable<object[]> Paint_TestData()
        {
            yield return new object[] { Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true };
            yield return new object[] { new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false };
            yield return new object[] { new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, false, true };
            yield return new object[] { new Rectangle(1000, 2000, 100, 100), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false };
        }

        [WinFormsTheory]
        [MemberData(nameof(Paint_TestData))]
        public void DataGridViewRow_Paint_Invoke_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.Paint(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow);
        }

        [WinFormsTheory]
        [MemberData(nameof(Paint_TestData))]
        public void DataGridViewRow_Paint_InvokeHeadersInvisible_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RowHeadersVisible = false
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.Paint(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow);
        }

        [WinFormsTheory]
        [MemberData(nameof(Paint_TestData))]
        public void DataGridViewRow_Paint_InvokeFrozen_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RowHeadersVisible = false
            };
            control.Columns[0].Frozen = true;
            control.Columns[0].Visible = true;
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.Paint(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow);
        }

        [WinFormsTheory]
        [MemberData(nameof(Paint_TestData))]
        public void DataGridViewRow_Paint_InvokeRightToLeft_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RightToLeft = RightToLeft.Yes
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.Paint(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow);
        }

        [WinFormsTheory]
        [MemberData(nameof(Paint_TestData))]
        public void DataGridViewRow_Paint_InvokeSingleVerticalBorderAddedDataGridView_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RowHeadersVisible = false
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.Paint(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow);
        }

        [WinFormsTheory]
        [MemberData(nameof(Paint_TestData))]
        public void DataGridViewRow_Paint_InvokeSingleHorizontalBorderAddedDataGridView_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                ColumnHeadersVisible = false
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.Paint(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow);
        }

        [WinFormsTheory]
        [MemberData(nameof(Paint_TestData))]
        public void DataGridViewRow_Paint_InvokeFirstDisplayedScrollingColumnIndex_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                FirstDisplayedScrollingColumnIndex = 0
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.Paint(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow);
        }

        [WinFormsTheory]
        [MemberData(nameof(Paint_TestData))]
        public void DataGridViewRow_Paint_InvokeFirstDisplayedScrollingColumnIndexRightToLeft_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                FirstDisplayedScrollingColumnIndex = 0,
                RightToLeft = RightToLeft.Yes
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.Paint(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow);
        }

        [WinFormsTheory]
        [MemberData(nameof(Paint_TestData))]
        public void DataGridViewRow_Paint_InvokeSingleHorizontalBorderAddedDisplayedDataGridView_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                ColumnHeadersVisible = false,
                FirstDisplayedScrollingColumnIndex = 0
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.Paint(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow);
        }

        [WinFormsFact]
        public void DataGridViewRow_Paint_Invoke_CallsRowPrePaint()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[1];

            int callCount = 0;
            DataGridViewRowPrePaintEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
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
            control.RowPrePaint += handler;
            row.Paint(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false);
            Assert.Equal(1, callCount);

            // Call again, handled.
            row.Paint(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.RowPrePaint -= handler;
            row.Paint(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_Paint_InvokeNullGraphics_DoesNotCallRowPrePaint()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[1];

            int callCount = 0;
            DataGridViewRowPrePaintEventHandler handler = (sender, e) => callCount++;
            control.RowPrePaint += handler;
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);

            // Call again, handled.
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);

            // Remove handler.
            control.RowPrePaint -= handler;
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_Paint_InvokeWithGraphics_CallsRowPostPaint()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[1];

            int callCount = 0;
            DataGridViewRowPostPaintEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Same(graphics, e.Graphics);
                Assert.Equal(new Rectangle(1, 2, 3, 4), e.ClipBounds);
                Assert.Equal(new Rectangle(2, 3, 4, 5), e.RowBounds);
                Assert.Equal(1, e.RowIndex);
                Assert.Equal(DataGridViewElementStates.Frozen, e.State);
                Assert.Empty(e.ErrorText);
                Assert.True(e.IsFirstDisplayedRow);
                Assert.False(e.IsLastVisibleRow);
            };
            control.RowPostPaint += handler;
            row.Paint(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false);
            Assert.Equal(1, callCount);

            // Call again.
            row.Paint(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.RowPostPaint -= handler;
            row.Paint(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_Paint_InvokeNullGraphics_DoesNotCallRowPostPaint()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[1];

            int callCount = 0;
            DataGridViewRowPostPaintEventHandler handler = (sender, e) => callCount++;
            control.RowPostPaint += handler;
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);

            // Call again.
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, new Rectangle(1, 2, 3, 4), new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);

            // Call with nothing to do.
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, Rectangle.Empty, new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);

            // Remove handler.
            control.RowPostPaint -= handler;
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, Rectangle.Empty, new Rectangle(2, 3, 4, 5), 1, DataGridViewElementStates.Frozen, true, false));
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_Paint_NullGraphicsEmptyRectangle_ThrowsArgumentNullException()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, Rectangle.Empty, Rectangle.Empty, 1, DataGridViewElementStates.None, true, true));
        }

        [WinFormsFact]
        public void DataGridViewRow_Paint_NullGraphicsNonEmptyRectangle_ThrowsArgumentNullException()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, Rectangle.Empty, new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.None, true, true));
        }

        [WinFormsFact]
        public void DataGridViewRow_Paint_NullGraphicsNonEmptyClipRectangle_ThrowsArgumentNullException()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.Paint(null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.None, true, true));
        }

        [WinFormsFact]
        public void DataGridViewRow_Paint_NoDataGridView_ThrowsInvalidOperationException()
        {
            using var row = new SubDataGridViewRow();
            Assert.Throws<InvalidOperationException>(() => row.Paint(null, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(2)]
        public void DataGridViewRow_Paint_InvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[1];
            Assert.Throws<ArgumentOutOfRangeException>("index", () => row.Paint(graphics, new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), rowIndex, DataGridViewElementStates.None, true, true));
        }

        public static IEnumerable<object[]> PaintCells_TestData()
        {
            yield return new object[] { Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All };
            yield return new object[] { new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
            yield return new object[] { new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), 0, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, false, true, DataGridViewPaintParts.None };
            yield return new object[] { new Rectangle(1000, 2000, 100, 100), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.Displayed | DataGridViewElementStates.Displayed, true, false, DataGridViewPaintParts.All };
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintCells_Invoke_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintCells(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintCells_InvokeHeadersInvisible_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RowHeadersVisible = false
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintCells(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintCells_InvokeFrozen_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RowHeadersVisible = false
            };
            control.Columns[0].Frozen = true;
            control.Columns[0].Visible = true;
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintCells(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintCells_InvokeRightToLeft_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RightToLeft = RightToLeft.Yes
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintCells(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintCells_InvokeSingleVerticalBorderAddedDataGridView_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RowHeadersVisible = false
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintCells(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintCells_InvokeSingleHorizontalBorderAddedDataGridView_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                ColumnHeadersVisible = false
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintCells(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintCells_InvokeFirstDisplayedScrollingColumnIndex_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                FirstDisplayedScrollingColumnIndex = 0
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintCells(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintCells_InvokeFirstDisplayedScrollingColumnIndexRightToLeft_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                FirstDisplayedScrollingColumnIndex = 0,
                RightToLeft = RightToLeft.Yes
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintCells(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintCells_InvokeSingleHorizontalBorderAddedDisplayedDataGridView_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                ColumnHeadersVisible = false,
                FirstDisplayedScrollingColumnIndex = 0
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintCells(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsFact]
        public void DataGridViewRow_PaintCells_NullGraphicsEmptyRectangle_Nop()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintCells(null, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [WinFormsFact]
        public void DataGridViewRow_PaintCells_NullGraphicsNonEmptyRectangle_ThrowsArgumentNullException()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintCells(null, Rectangle.Empty, new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [WinFormsFact]
        public void DataGridViewRow_PaintCells_NullGraphicsNoVisibleColumns_ThrowsArgumentNullException()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintCells(null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 100, 100), -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [WinFormsFact]
        public void DataGridViewRow_PaintCells_NullGraphicsNonEmptyClipRectangle_ThrowsArgumentNullException()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintCells(null, new Rectangle(1, 2, 1000, 10000), new Rectangle(1, 2, 1000, 1000), 1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [WinFormsFact]
        public void DataGridViewRow_PaintCells_NoDataGridView_ThrowsInvalidOperationException()
        {
            using var row = new SubDataGridViewRow();
            Assert.Throws<InvalidOperationException>(() => row.PaintCells(null, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(2)]
        public void DataGridViewRow_PaintCells_InvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            control.Columns[0].Frozen = true;
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[1];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => row.PaintCells(graphics, new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), rowIndex, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [WinFormsTheory]
        [InlineData((DataGridViewPaintParts)(DataGridViewPaintParts.None - 1))]
        [InlineData((DataGridViewPaintParts)(DataGridViewPaintParts.All + 1))]
        public void DataGridViewRow_PaintCells_InvalidPaintParts_ThrowsArgumentException(DataGridViewPaintParts paintParts)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];
            Assert.Throws<ArgumentException>(null, () => row.PaintCells(graphics, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, paintParts));
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintHeader_Invoke_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintHeader(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintHeader_InvokeHeadersInvisible_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RowHeadersVisible = false
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintHeader(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintHeader_InvokeFrozen_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RowHeadersVisible = false
            };
            control.Columns[0].Frozen = true;
            control.Columns[0].Visible = true;
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintHeader(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintHeader_InvokeRightToLeft_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RightToLeft = RightToLeft.Yes
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintHeader(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintHeader_InvokeSingleVerticalBorderAddedDataGridView_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RowHeadersVisible = false
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintHeader(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintHeader_InvokeSingleHorizontalBorderAddedDataGridView_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                ColumnHeadersVisible = false
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintHeader(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintHeader_InvokeFirstDisplayedScrollingColumnIndex_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                FirstDisplayedScrollingColumnIndex = 0
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintHeader(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintHeader_InvokeFirstDisplayedScrollingColumnIndexRightToLeft_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                FirstDisplayedScrollingColumnIndex = 0,
                RightToLeft = RightToLeft.Yes
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintHeader(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsTheory]
        [MemberData(nameof(PaintCells_TestData))]
        public void DataGridViewRow_PaintHeader_InvokeSingleHorizontalBorderAddedDisplayedDataGridView_Success(Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
        {
            if (rowIndex == -1)
            {
                return;
            }

            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                ColumnHeadersVisible = false,
                FirstDisplayedScrollingColumnIndex = 0
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];

            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            row.PaintHeader(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
        }

        [WinFormsFact]
        public void DataGridViewRow_PaintHeader_NullGraphicsEmptyRectangle_ThrowsArgumentNullException()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RowHeadersVisible = false
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintHeader(null, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [WinFormsFact]
        public void DataGridViewRow_PaintHeader_NullGraphicsNonEmptyRectangle_ThrowsArgumentNullException()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RowHeadersVisible = false
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintHeader(null, Rectangle.Empty, new Rectangle(1, 2, 100, 100), -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [WinFormsFact]
        public void DataGridViewRow_PaintHeader_NullGraphicsNonEmptyClipRectangle_ThrowsArgumentNullException()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RowHeadersVisible = false
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintHeader(null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 100, 100), 1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [WinFormsFact]
        public void DataGridViewRow_PaintHeader_NullGraphicsRowHeadersNotVisible_ThrowsArgumentNullException()
        {
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2,
                RowHeadersVisible = false
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[1];
            Assert.Throws<ArgumentNullException>("graphics", () => row.PaintHeader(null, new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [WinFormsFact]
        public void DataGridViewRow_PaintHeader_NoDataGridView_ThrowsInvalidOperationException()
        {
            using var row = new SubDataGridViewRow();
            Assert.Throws<InvalidOperationException>(() => row.PaintHeader(null, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(2)]
        public void DataGridViewRow_PaintHeader_InvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => row.PaintHeader(graphics, new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), rowIndex, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [WinFormsFact]
        public void DataGridViewRow_PaintHeader_SharedRowIndex_ThrowsInvalidOperationException()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];
            Assert.Throws<InvalidOperationException>(() => row.PaintHeader(graphics, new Rectangle(1, 2, 100, 100), new Rectangle(1, 2, 100, 100), -1, DataGridViewElementStates.None, true, true, DataGridViewPaintParts.All));
        }

        [WinFormsTheory]
        [InlineData((DataGridViewPaintParts)(DataGridViewPaintParts.None - 1))]
        [InlineData((DataGridViewPaintParts)(DataGridViewPaintParts.All + 1))]
        public void DataGridViewRow_PaintHeader_InvalidPaintParts_ThrowsInvalidEnumArgumentException(DataGridViewPaintParts paintParts)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            using var rowTemplate = new SubDataGridViewRow();
            using var control = new DataGridView
            {
                RowTemplate = rowTemplate,
                ColumnCount = 1,
                RowCount = 2
            };
            SubDataGridViewRow row = (SubDataGridViewRow)control.Rows[0];
            Assert.Throws<InvalidEnumArgumentException>("paintParts", () => row.PaintHeader(graphics, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, true, true, paintParts));
        }

        [WinFormsTheory]
        [InlineData(new object[0], true, null)]
        [InlineData(new object[] { 1 }, true, 1)]
        [InlineData(new object[] { 1, 2 }, false, 1)]
        public void DataGridViewRow_SetValues_HasCellsWithoutDataGridView_Success(object[] values, bool expectedResult, object expectedValue)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(expectedResult, row.SetValues(values));
            Assert.Equal(expectedValue, Assert.Single(row.Cells.Cast<DataGridViewCell>()).Value);
        }

        [WinFormsTheory]
        [InlineData(new object[0], true, null)]
        [InlineData(new object[] { 1 }, true, 1)]
        [InlineData(new object[] { 1, 2 }, false, 1)]
        public void DataGridViewRow_SetValues_HasCellsWithDataGridView_Success(object[] values, bool expectedResult, object expectedValue)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Equal(expectedResult, row.SetValues(values));
            Assert.Equal(expectedValue, Assert.Single(row.Cells.Cast<DataGridViewCell>()).Value);
        }

        [WinFormsFact]
        public void DataGridViewRow_SetValues_CantSet_ReturnsFalse()
        {
            using var row = new DataGridViewRow();
            row.Cells.Add(new CantSetDataGridViewCell());
            Assert.False(row.SetValues(new object[] { 1 }));
            Assert.Null(Assert.Single(row.Cells.Cast<DataGridViewCell>()).Value);
        }

        private class CantSetDataGridViewCell : DataGridViewCell
        {
            protected override bool SetValue(int rowIndex, object value) => false;
        }

        [WinFormsFact]
        public void DataGridViewRow_SetValues_NullValue_ThrowsArgumentNullException()
        {
            using var row = new DataGridViewRow();
            Assert.Throws<ArgumentNullException>("values", () => row.SetValues(null));
        }

        [WinFormsFact]
        public void DataGridViewRow_SetValues_VirtualDataGridView_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                VirtualMode = true
            };
            DataGridViewRow row = control.Rows[0];
            Assert.Throws<InvalidOperationException>(() => row.SetValues(Array.Empty<object>()));
        }

        [WinFormsFact]
        public void DataGridViewRow_SetValues_Shared_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1
            };
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.Throws<InvalidOperationException>(() => row.SetValues(Array.Empty<object>()));
        }

        private class CustomStateDataGridViewRow : DataGridViewRow
        {
            public DataGridViewElementStates StateResult { get; set; }

            public override DataGridViewElementStates State => StateResult;
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

            public new void Paint(Graphics graphics, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow)
            {
                base.Paint(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow);
            }

            public new void PaintCells(Graphics graphics, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
            {
                base.PaintCells(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
            }

            public new void PaintHeader(Graphics graphics, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
            {
                base.PaintHeader(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts);
            }

#pragma warning restore xUnit1013

            public new bool IsRow => base.IsRow;

            public new DataGridViewHeaderCell HeaderCellCore
            {
                get => base.HeaderCellCore;
                set => base.HeaderCellCore = value;
            }

            [WinFormsFact]
            public void DataGridViewRow_AccessibilityObject_Get_ReturnsSameInstance()
            {
                Assert.Same(AccessibilityObject, AccessibilityObject);
                DataGridViewRowAccessibleObject accessibilityObject = Assert.IsType<DataGridViewRowAccessibleObject>(AccessibilityObject);
                Assert.Equal(this, accessibilityObject.Owner);
            }

            [WinFormsFact]
            public void DataGridViewRow_CreateAccessibilityInstance_Invoke_ReturnsExpected()
            {
                DataGridViewRowAccessibleObject accessibilityObject = Assert.IsType<DataGridViewRowAccessibleObject>(CreateAccessibilityInstance());
                Assert.Equal(this, accessibilityObject.Owner);
            }
        }
    }
}
