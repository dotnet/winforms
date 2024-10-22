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
    public void Constructor_SetsAutoResizeHandlesToTrue_And_SelectionRules_ShouldNotIncludeTopOrBottomSizeable_SnapLines_ReturnsCorrectSnapLines()
    {
        using UpDownBaseDesigner designer = new();

        designer.AutoResizeHandles.Should().BeTrue();

        using NumericUpDown numericUpDown = new();
        Mock<ISite> site = new();
        site.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(new Mock<IDesignerHost>().Object);
        numericUpDown.Site = site.Object;

        designer.Initialize(numericUpDown);
        SelectionRules rules = designer.SelectionRules;

        rules.Should().NotHaveFlag(SelectionRules.TopSizeable);
        rules.Should().NotHaveFlag(SelectionRules.BottomSizeable);

        List<SnapLine> snapLines = (List<SnapLine>)designer.SnapLines;

        snapLines.Should().NotBeNull();
        SnapLine? baselineSnapLine = snapLines.Cast<SnapLine>().FirstOrDefault(sl => sl.SnapLineType == SnapLineType.Baseline);
        baselineSnapLine.Should().NotBeNull();
        baselineSnapLine.Should().BeOfType<SnapLine>().Which.Priority.Should().Be(SnapLinePriority.Medium);
    }
}
