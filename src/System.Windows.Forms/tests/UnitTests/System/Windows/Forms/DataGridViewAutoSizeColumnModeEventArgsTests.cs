// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewAutoSizeColumnModeEventArgsTests
{
    public static IEnumerable<object[]> Ctor_DataGridViewColumn_DataGridViewAutoSizeColumn_TestData()
    {
        yield return new object[] { null, DataGridViewAutoSizeColumnMode.NotSet - 1 };
        yield return new object[] { new DataGridViewColumn(), DataGridViewAutoSizeColumnMode.AllCells };
    }

    [Theory]
    [MemberData(nameof(Ctor_DataGridViewColumn_DataGridViewAutoSizeColumn_TestData))]
    public void Ctor_DataGridViewColumn_DataGridViewAutoSizeColumn(DataGridViewColumn dataGridViewColumn, DataGridViewAutoSizeColumnMode previousMode)
    {
        DataGridViewAutoSizeColumnModeEventArgs e = new(dataGridViewColumn, previousMode);
        Assert.Equal(dataGridViewColumn, e.Column);
        Assert.Equal(previousMode, e.PreviousMode);
    }
}
