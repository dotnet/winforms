﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.TestUtilities;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewRowErrorTextNeededEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewRowErrorTextNeededEventArgs_ErrorText_Set_GetReturnsExpected(string value)
        {
            using var dataGridView = new DataGridView
            {
                ColumnCount = 1,
                VirtualMode = true
            };
            DataGridViewRow row = dataGridView.Rows[0];

            int callCount = 0;
            DataGridViewRowErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                e.ErrorText = value;
                Assert.Equal(value, e.ErrorText);
            };
            dataGridView.RowErrorTextNeeded += handler;

            Assert.Same(value, row.GetErrorText(0));
            Assert.Equal(1, callCount);
        }
    }
}
