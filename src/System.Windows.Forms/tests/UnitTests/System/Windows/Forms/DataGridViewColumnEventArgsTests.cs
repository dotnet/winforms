// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class DataGridViewColumnEventArgsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void Ctor_DataGridViewColumn()
        {
            using var dataGridViewColumn = new DataGridViewColumn();
            var e = new DataGridViewColumnEventArgs(dataGridViewColumn);
            Assert.Equal(dataGridViewColumn, e.Column);
        }

        [Fact]
        public void Ctor_NullDataGridViewColumn_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("dataGridViewColumn", () => new DataGridViewColumnEventArgs(null));
        }
    }
}
