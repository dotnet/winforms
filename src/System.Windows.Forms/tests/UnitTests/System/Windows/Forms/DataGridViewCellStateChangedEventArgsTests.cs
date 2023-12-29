// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewCellStateChangedEventArgsTests
{
    [Theory]
    [InlineData(DataGridViewElementStates.Displayed)]
    [InlineData((DataGridViewElementStates)7)]
    public void Ctor_DataGridViewCell_DataGridViewElementStates(DataGridViewElementStates stateChanged)
    {
        SubDataGridViewCell cell = new();
        DataGridViewCellStateChangedEventArgs e = new(cell, stateChanged);
        Assert.Equal(cell, e.Cell);
        Assert.Equal(stateChanged, e.StateChanged);
    }

    [Fact]
    public void Ctor_NullDataGridViewCell_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("dataGridViewCell", () => new DataGridViewCellStateChangedEventArgs(null, DataGridViewElementStates.None));
    }

    private class SubDataGridViewCell : DataGridViewCell
    {
        public SubDataGridViewCell() : base() { }
    }
}
