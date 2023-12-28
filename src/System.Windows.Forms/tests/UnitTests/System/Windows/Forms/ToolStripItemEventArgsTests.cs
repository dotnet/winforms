// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ToolStripItemEventArgsTests
{
    public static IEnumerable<object[]> Ctor_ToolStripItem_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ToolStripButton() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_ToolStripItem_TestData))]
    public void Ctor_ToolStripItem(ToolStripItem item)
    {
        ToolStripItemEventArgs e = new(item);
        Assert.Equal(item, e.Item);
    }
}
