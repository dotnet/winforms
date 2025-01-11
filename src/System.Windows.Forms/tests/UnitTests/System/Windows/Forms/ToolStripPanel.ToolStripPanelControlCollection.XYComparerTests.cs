// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripPanel_ToolStripPanelControlCollection_XYComparerTests : IDisposable
{
    private readonly ToolStripPanel.ToolStripPanelControlCollection.XYComparer _comparer = new();
    private readonly Control _control1 = new() { Bounds = new Rectangle(10, 20, 100, 100) };
    private readonly Control _control2 = new() { Bounds = new Rectangle(10, 10, 100, 100) };

    public void Dispose()
    {
        _control1.Dispose();
        _control2.Dispose();
    }

    [WinFormsFact]
    public void Compare_FirstControlNull_ReturnsNegativeOne() =>
        _comparer.Compare(null, _control1).Should().Be(-1);

    [WinFormsFact]
    public void Compare_SecondControlNull_ReturnsOne() =>
        _comparer.Compare(_control1, null).Should().Be(1);

    [WinFormsFact]
    public void Compare_BothControlsNull_ReturnsZero() =>
        _comparer.Compare(null, null).Should().Be(0);

    [WinFormsTheory]
    [InlineData(10, 10, 20, 10, -1)]
    [InlineData(10, 10, 10, 20, -1)]
    [InlineData(10, 20, 10, 10, 1)]
    [InlineData(20, 10, 10, 10, 1)]
    public void Compare_VariousBounds_ReturnsExpected(int x1, int y1, int x2, int y2, int expected)
    {
        _control1.Bounds = new Rectangle(x1, y1, 100, 100);
        _control2.Bounds = new Rectangle(x2, y2, 100, 100);

        _comparer.Compare(_control1, _control2).Should().Be(expected);
    }
}
