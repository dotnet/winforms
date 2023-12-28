// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewColumnStateChangedEventArgsTests
{
    [Fact]
    public void Ctor_DataGridViewColumn_DataGridViewElementStates()
    {
        using DataGridViewColumn dataGridViewColumn = new();
        DataGridViewColumnStateChangedEventArgs e = new(dataGridViewColumn, DataGridViewElementStates.Displayed);
        Assert.Equal(dataGridViewColumn, e.Column);
        Assert.Equal(DataGridViewElementStates.Displayed, e.StateChanged);
    }

    [Fact]
    public void DataGridViewColumnStateChangedEventArgs_Ctor_NullDataGridViewColumn_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DataGridViewColumnStateChangedEventArgs(null, DataGridViewElementStates.Displayed));
    }
}
