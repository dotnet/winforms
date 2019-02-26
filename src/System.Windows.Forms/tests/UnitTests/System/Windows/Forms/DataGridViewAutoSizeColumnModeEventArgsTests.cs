// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewAutoSizeColumnModeEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_DataGridViewColumn_DataGridViewAutoSizeColumn_TestData()
        {
            yield return new object[] { null, (DataGridViewAutoSizeColumnMode)(DataGridViewAutoSizeColumnMode.NotSet - 1) };
            yield return new object[] { new DataGridViewColumn(), DataGridViewAutoSizeColumnMode.AllCells };
        }

        [Theory]
        [MemberData(nameof(Ctor_DataGridViewColumn_DataGridViewAutoSizeColumn_TestData))]
        public void Ctor_DataGridViewColumn_DataGridViewAutoSizeColumn(DataGridViewColumn dataGridViewColumn, DataGridViewAutoSizeColumnMode previousMode)
        {
            var e = new DataGridViewAutoSizeColumnModeEventArgs(dataGridViewColumn, previousMode);
            Assert.Equal(dataGridViewColumn, e.Column);
            Assert.Equal(previousMode, e.PreviousMode);
        }
    }
}
