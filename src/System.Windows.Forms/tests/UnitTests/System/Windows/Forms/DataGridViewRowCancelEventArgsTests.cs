// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewRowCancelEventArgsTests
{
    public static IEnumerable<object[]> Ctor_DataGridViewRow_TestData()
    {
        yield return new object[] { null };

        DataGridViewRow dataGridViewRow = new();
        yield return new object[] { dataGridViewRow };
    }

    [Theory]
    [MemberData(nameof(Ctor_DataGridViewRow_TestData))]
    public void Ctor_DataGridViewRow(DataGridViewRow dataGridViewRow)
    {
        DataGridViewRowCancelEventArgs e = new(dataGridViewRow);
        Assert.Equal(dataGridViewRow, e.Row);
        Assert.False(e.Cancel);
    }
}
