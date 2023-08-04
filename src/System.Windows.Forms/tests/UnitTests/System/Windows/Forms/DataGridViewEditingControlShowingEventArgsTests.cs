// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewEditingControlShowingEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Control_DataGridViewCellStyle_TestData()
    {
        yield return new object[] { null, new DataGridViewCellStyle() };
        yield return new object[] { new Button(), null };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Control_DataGridViewCellStyle_TestData))]
    public void DataGridViewEditingControlShowingEventArgs_NullArg_ThrowsArgumentNullException(Control control, DataGridViewCellStyle cellStyle)
    {
        Assert.Throws<ArgumentNullException>(() => new DataGridViewEditingControlShowingEventArgs(control, cellStyle));
    }

    [Fact]
    public void Ctor_Control_DataGridViewCellStyle()
    {
        using var button = new Button();
        var cellStyle = new DataGridViewCellStyle();
        var e = new DataGridViewEditingControlShowingEventArgs(button, cellStyle);
        Assert.Equal(button, e.Control);
        Assert.Equal(cellStyle, e.CellStyle);
    }

    [Fact]
    public void CellStyle_SetNull_ThrowsArgumentNullException()
    {
        using var button = new Button();
        var e = new DataGridViewEditingControlShowingEventArgs(button, new DataGridViewCellStyle());
        Assert.Throws<ArgumentNullException>("value", () => e.CellStyle = null);
    }
}
