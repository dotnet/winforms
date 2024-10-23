// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design.Behavior;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class UpDownBaseDesignerTests
{
    [Fact]
    public void Constructor_SetsAutoResizeHandlesToTrue_And_SelectionRules_ShouldNotIncludeTopOrBottomSizeable()
    {
        using UpDownBaseDesigner designer = new();

        designer.AutoResizeHandles.Should().BeTrue();

        using NumericUpDown numericUpDown = new();
        Mock<ISite> site = new();
        site.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(new Mock<IDesignerHost>().Object);
        numericUpDown.Site = site.Object;

        designer.Initialize(numericUpDown);
        SelectionRules rules = designer.SelectionRules;

        rules.Should().NotHaveFlag(SelectionRules.TopSizeable | SelectionRules.BottomSizeable);
    }

    [WinFormsTheory]
    [InlineData(BorderStyle.None, -1)]
    [InlineData(BorderStyle.Fixed3D, 2)]
    public void SnapLines_ReturnsCorrectSnapLine(BorderStyle borderStyle, int expectedBaselineOffset)
    {
        using UpDownBaseDesigner designer = new();
        using NumericUpDown numericUpDown = new();
        numericUpDown.BorderStyle = borderStyle;
        Mock<ISite> site = new();
        site.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(new Mock<IDesignerHost>().Object);
        numericUpDown.Site = site.Object;

        designer.Initialize(numericUpDown);

        List<SnapLine> snapLines = (List<SnapLine>)designer.SnapLines;

        snapLines.Should().NotBeNull();
        SnapLine? baselineSnapLine = snapLines.Cast<SnapLine>().FirstOrDefault(sl => sl.SnapLineType == SnapLineType.Baseline);
        baselineSnapLine.Should().NotBeNull();
        baselineSnapLine.Should().BeOfType<SnapLine>().Which.Priority.Should().Be(SnapLinePriority.Medium);

        int expectedBaseline = DesignerUtils.GetTextBaseline(numericUpDown, Drawing.ContentAlignment.TopLeft) + expectedBaselineOffset;
        baselineSnapLine.Should().BeOfType<SnapLine>().Which.Offset.Should().Be(expectedBaseline);
    }
}
