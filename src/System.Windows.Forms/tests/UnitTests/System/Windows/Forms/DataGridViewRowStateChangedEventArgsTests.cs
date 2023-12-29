// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewRowStateChangedEventArgsTests
{
    public static IEnumerable<object[]> Ctor_DataGridViewRow_DataGridViewElementStates_TestData()
    {
        yield return new object[] { new DataGridViewRow(), (DataGridViewElementStates)7 };
        yield return new object[] { new DataGridViewRow(), DataGridViewElementStates.Displayed };
    }

    [Theory]
    [MemberData(nameof(Ctor_DataGridViewRow_DataGridViewElementStates_TestData))]
    public void Ctor_DataGridViewRow_DataGridViewElementStates(DataGridViewRow dataGridViewRow, DataGridViewElementStates stateChanged)
    {
        DataGridViewRowStateChangedEventArgs e = new(dataGridViewRow, stateChanged);
        Assert.Equal(dataGridViewRow, e.Row);
        Assert.Equal(stateChanged, e.StateChanged);
    }
}
