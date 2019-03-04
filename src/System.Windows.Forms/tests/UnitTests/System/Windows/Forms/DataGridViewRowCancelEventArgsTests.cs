// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewRowCancelEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_DataGridViewRow_TestData()
        {
            yield return new object[] { null };

            var dataGridViewRow = new DataGridViewRow();
            yield return new object[] { dataGridViewRow };
        }

        [Theory]
        [MemberData(nameof(Ctor_DataGridViewRow_TestData))]
        public void Ctor_DataGridViewRow(DataGridViewRow dataGridViewRow)
        {
            var e = new DataGridViewRowCancelEventArgs(dataGridViewRow);
            Assert.Equal(dataGridViewRow, e.Row);
            Assert.False(e.Cancel);
        }
    }
}
