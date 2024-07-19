// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;
public class ToolStripCustomIComparerTests
{
    private readonly ToolStripCustomIComparer _comparer = new();

    [WinFormsTheory]
    [InlineData(null, null, 0)]
    [InlineData(null, typeof(ToolStrip), -1)]
    [InlineData(typeof(ToolStrip), null, 1)]
    [InlineData(typeof(ToolStrip), typeof(ToolStrip), 0)]
    [InlineData(typeof(ToolStrip), typeof(ToolStripDropDown), 1)]
    [InlineData(typeof(ToolStripDropDown), typeof(ToolStrip), -1)]
    public void ToolStripCustomIComparer_Compare_Tests(Type type1, Type type2, int expected)
    {
        object obj1 = type1 is null ? null : Activator.CreateInstance(type1);
        object obj2 = type2 is null ? null : Activator.CreateInstance(type2);
        int result = _comparer.Compare((ToolStrip)obj1, (ToolStrip)obj2);

        result.Should().Be(expected);
    }
}
