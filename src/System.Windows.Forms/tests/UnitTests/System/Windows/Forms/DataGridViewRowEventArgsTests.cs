// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewRowEventArgsTests
{
    [Fact]
    public void Ctor_DataGridViewRow()
    {
        using DataGridViewRow dataGridViewRow = new();
        DataGridViewRowEventArgs e = new(dataGridViewRow);
        Assert.Equal(dataGridViewRow, e.Row);
    }

    [Fact]
    public void Ctor_NullDataGridViewRow_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("dataGridViewRow", () => new DataGridViewRowEventArgs(null));
    }
}
