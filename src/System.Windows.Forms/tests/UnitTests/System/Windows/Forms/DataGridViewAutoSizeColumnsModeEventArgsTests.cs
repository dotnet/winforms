// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewAutoSizeColumnsModeEventArgsTests
{
    public static IEnumerable<object[]> Ctor_DataGridViewAutoSizeColumnModeArray_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { Array.Empty<DataGridViewAutoSizeColumnMode>() };
        yield return new object[] { new DataGridViewAutoSizeColumnMode[] { DataGridViewAutoSizeColumnMode.AllCells, DataGridViewAutoSizeColumnMode.None - 1 } };
    }

    [Theory]
    [MemberData(nameof(Ctor_DataGridViewAutoSizeColumnModeArray_TestData))]
    public void Ctor_DataGridViewAutoSizeColumnModeArray(DataGridViewAutoSizeColumnMode[] previousModes)
    {
        DataGridViewAutoSizeColumnsModeEventArgs e = new(previousModes);
        Assert.Equal(previousModes, e.PreviousModes);
    }
}
