// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewRowHeightInfoNeededEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [InlineData(-1, 3)]
        [InlineData(3, 3)]
        [InlineData(6, 6)]
        [InlineData(65536, 65536)]
        public void DataGridViewRowHeightInfoNeededEventArgs_Height_Set_GetReturnsExpected(int value, int expected)
        {
            using var dataGridView = new DataGridView
            {
                ColumnCount = 1,
                VirtualMode = true
            };
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowHeightInfoNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                e.Height = value;
                Assert.Equal(expected, e.Height);
            };
            dataGridView.RowHeightInfoNeeded += handler;

            Assert.Equal(expected, row.Height);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [InlineData(65537)]
        public void Height_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            using var dataGridView = new DataGridView
            {
                ColumnCount = 1,
                VirtualMode = true
            };
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowHeightInfoNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Throws<ArgumentOutOfRangeException>("value", () => e.Height = value);
            };
            dataGridView.RowHeightInfoNeeded += handler;

            Assert.Equal(Control.DefaultFont.Height + 9, row.Height);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [InlineData(2, 10)]
        [InlineData(3, 10)]
        [InlineData(10, 10)]
        [InlineData(11, 11)]
        [InlineData(65536, 65536)]
        [InlineData(65537, 65537)]
        public void MinimumHeight_Set_GetReturnsExpected(int value, int expectedHeight)
        {
            using var dataGridView = new DataGridView
            {
                ColumnCount = 1,
                VirtualMode = true
            };
            DataGridViewRow row = dataGridView.Rows[0];
            row.Height = 10;

            int callCount = 0;
            DataGridViewRowHeightInfoNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                e.MinimumHeight = value;
                Assert.Equal(value, e.MinimumHeight);
                Assert.Equal(expectedHeight, e.Height);
            };
            dataGridView.RowHeightInfoNeeded += handler;

            Assert.Equal(value, row.MinimumHeight);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void MinimumHeight_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            using var dataGridView = new DataGridView
            {
                ColumnCount = 1,
                VirtualMode = true
            };
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowHeightInfoNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Throws<ArgumentOutOfRangeException>("value", () => e.MinimumHeight = value);
            };
            dataGridView.RowHeightInfoNeeded += handler;

            Assert.Equal(3, row.MinimumHeight);
            Assert.Equal(1, callCount);
        }
    }
}
