// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ToolStripItemClickedEventArgsTests
{
    public static IEnumerable<object[]> Ctor_ToolStripItem_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ToolStripButton() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_ToolStripItem_TestData))]
    public void Ctor_ToolStripItem(ToolStripItem clickedItem)
    {
        ToolStripItemClickedEventArgs e = new(clickedItem);
        Assert.Equal(clickedItem, e.ClickedItem);
    }
}
