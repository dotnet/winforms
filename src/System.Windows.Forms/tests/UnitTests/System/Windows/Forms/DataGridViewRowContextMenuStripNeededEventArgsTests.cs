// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewRowContextMenuStripNeededEventArgsTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void Ctor_Int(int rowIndex)
    {
        DataGridViewRowContextMenuStripNeededEventArgs e = new(rowIndex);
        Assert.Equal(rowIndex, e.RowIndex);
        Assert.Null(e.ContextMenuStrip);
    }

    [Fact]
    public void Ctor_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => new DataGridViewRowContextMenuStripNeededEventArgs(-2));
    }

    public static IEnumerable<object[]> ContextMenuStrip_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ContextMenuStrip() };
    }

    [WinFormsTheory]
    [MemberData(nameof(ContextMenuStrip_TestData))]
    public void ContextMenuStrip_Set_GetReturnsExpected(ContextMenuStrip value)
    {
        DataGridViewRowContextMenuStripNeededEventArgs e = new(1)
        {
            ContextMenuStrip = value
        };
        Assert.Equal(value, e.ContextMenuStrip);
    }
}
