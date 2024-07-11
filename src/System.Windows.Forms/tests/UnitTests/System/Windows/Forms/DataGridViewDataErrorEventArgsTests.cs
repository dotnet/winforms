// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewDataErrorEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Exception_Int_Int_DataGridViewDataErrorContexts_TestData()
    {
        yield return new object[] { null, -1, -1, (DataGridViewDataErrorContexts)3 };
        yield return new object[] { new InvalidOperationException(), 0, 0, DataGridViewDataErrorContexts.Formatting };
        yield return new object[] { new InvalidOperationException(), 1, 2, DataGridViewDataErrorContexts.Formatting };
    }

    [Theory]
    [MemberData(nameof(Ctor_Exception_Int_Int_DataGridViewDataErrorContexts_TestData))]
    public void Ctor_Exception_Int_Int_DataGridViewDataErrorContexts(Exception exception, int columnIndex, int rowIndex, DataGridViewDataErrorContexts context)
    {
        DataGridViewDataErrorEventArgs e = new(exception, columnIndex, rowIndex, context);
        Assert.Equal(exception, e.Exception);
        Assert.Equal(columnIndex, e.ColumnIndex);
        Assert.Equal(rowIndex, e.RowIndex);
        Assert.Equal(context, e.Context);
        Assert.False(e.Cancel);
        Assert.False(e.ThrowException);
    }

    [Fact]
    public void Ctor_NegativeColumnIndex_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>("columnIndex", () => new DataGridViewDataErrorEventArgs(null, -2, 0, DataGridViewDataErrorContexts.Formatting));
    }

    [Fact]
    public void Ctor_NegativeRowIndex_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => new DataGridViewDataErrorEventArgs(null, 0, -2, DataGridViewDataErrorContexts.Formatting));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ThrowException_SetFalseWithException_GetReturnsExpected(bool value)
    {
        DataGridViewDataErrorEventArgs e = new(new InvalidOperationException(), 1, 2, DataGridViewDataErrorContexts.Formatting) { ThrowException = value };
        Assert.Equal(value, e.ThrowException);
    }

    [Fact]
    public void ThrowException_SetFalseWithoutException_GetReturnsExpected()
    {
        DataGridViewDataErrorEventArgs e = new(null, 1, 2, DataGridViewDataErrorContexts.Formatting) { ThrowException = false };
        Assert.False(e.ThrowException);
    }

    [Fact]
    public void ThrowException_SetTrueWithoutException_ThrowsArgumentException()
    {
        DataGridViewDataErrorEventArgs e = new(null, 1, 2, DataGridViewDataErrorContexts.Formatting);
        Assert.Throws<ArgumentException>(() => e.ThrowException = true);
    }
}
