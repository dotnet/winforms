// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewRowEventArgsTests
    {
        [Fact]
        public void Ctor_DataGridViewRow()
        {
            var dataGridViewRow = new DataGridViewRow();
            var e = new DataGridViewRowEventArgs(dataGridViewRow);
            Assert.Equal(dataGridViewRow, e.Row);
        }

        [Fact]
        public void Ctor_NullDataGridViewRow_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("dataGridViewRow", () => new DataGridViewRowEventArgs(null));
        }
    }
}
