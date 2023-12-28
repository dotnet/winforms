// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewColumnEventArgsTests
{
    [Fact]
    public void Ctor_DataGridViewColumn()
    {
        using DataGridViewColumn dataGridViewColumn = new();
        DataGridViewColumnEventArgs e = new(dataGridViewColumn);
        Assert.Equal(dataGridViewColumn, e.Column);
    }

    [Fact]
    public void Ctor_NullDataGridViewColumn_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("dataGridViewColumn", () => new DataGridViewColumnEventArgs(null));
    }
}
