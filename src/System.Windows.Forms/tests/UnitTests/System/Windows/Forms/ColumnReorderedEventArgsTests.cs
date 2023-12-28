// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ColumnReorderedEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Int_Int_ColumnHeader_TestData()
    {
        yield return new object[] { -1, -1, null };
        yield return new object[] { 0, 0, null };
        yield return new object[] { 1, 2, new ColumnHeader() };
    }

    [Theory]
    [MemberData(nameof(Ctor_Int_Int_ColumnHeader_TestData))]
    public void Ctor_Int_Int_ColumnHeader(int oldDisplayIndex, int newDisplayIndex, ColumnHeader header)
    {
        ColumnReorderedEventArgs e = new(oldDisplayIndex, newDisplayIndex, header);
        Assert.Equal(oldDisplayIndex, e.OldDisplayIndex);
        Assert.Equal(newDisplayIndex, e.NewDisplayIndex);
        Assert.Equal(header, e.Header);
        Assert.False(e.Cancel);
    }
}
