// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design.Behavior;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class UpDownBaseDesignerTests : IDisposable
{
    private readonly UpDownBaseDesigner _designer;
    private readonly NumericUpDown _numericUpDown;

    public UpDownBaseDesignerTests()
    {
        _numericUpDown = new();
        _designer = new();
        Mock<ISite> site = new();
        site.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(new Mock<IDesignerHost>().Object);
        _numericUpDown.Site = site.Object;
    }

    private void InitializeDesigner(BorderStyle borderStyle)
    {
        _numericUpDown.BorderStyle = borderStyle;
        _designer.Initialize(_numericUpDown);
    }

    public void Dispose()
    {
        _designer.Dispose();
        _numericUpDown.Dispose();
    }

    [Fact]
    public void Constructor_SetsAutoResizeHandlesToTrue_And_SelectionRules_ShouldNotIncludeTopOrBottomSizeable()
    {
        InitializeDesigner(BorderStyle.None);
        SelectionRules rules = _designer.SelectionRules;
        _designer.AutoResizeHandles.Should().BeTrue();
        rules.Should().NotHaveFlag(SelectionRules.TopSizeable | SelectionRules.BottomSizeable);
    }

    [WinFormsTheory]
    [InlineData(BorderStyle.None, -1)]
    [InlineData(BorderStyle.Fixed3D, 2)]
    public void SnapLines_ReturnsCorrectSnapLine(BorderStyle borderStyle, int expectedBaselineOffset)
    {
        InitializeDesigner(borderStyle);
        List<SnapLine> snapLines = (List<SnapLine>)_designer.SnapLines;

        snapLines.Should().NotBeNull();
        SnapLine? baselineSnapLine = snapLines.FirstOrDefault(sl => sl.SnapLineType == SnapLineType.Baseline);
        baselineSnapLine.Should().BeOfType<SnapLine>().Which.Priority.Should().Be(SnapLinePriority.Medium);

        int expectedBaseline = DesignerUtils.GetTextBaseline(_numericUpDown, Drawing.ContentAlignment.TopLeft) + expectedBaselineOffset;
        baselineSnapLine.Should().BeOfType<SnapLine>().Which.Offset.Should().Be(expectedBaseline);
    }
}
