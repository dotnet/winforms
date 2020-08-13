// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewCellErrorTextNeededEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewCellErrorTextNeededEventArgs_ErrorText_Set_GetReturnsExpected(string value)
        {
            var dataGridView = new DataGridView
            {
                ColumnCount = 1,
                VirtualMode = true
            };
            DataGridViewCell cell = dataGridView.Rows[0].Cells[0];

            int callCount = 0;
            DataGridViewCellErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                e.ErrorText = value;
                Assert.Equal(value, e.ErrorText);
            };
            dataGridView.CellErrorTextNeeded += handler;

            Assert.Same(value, cell.GetErrorText(0));
            Assert.Equal(1, callCount);
        }
    }
}
