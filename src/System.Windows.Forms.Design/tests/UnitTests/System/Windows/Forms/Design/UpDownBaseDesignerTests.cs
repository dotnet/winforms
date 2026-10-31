// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
    }

    private void InitializeDesigner(BorderStyle borderStyle)
        => InitializeDesigner(_designer, _numericUpDown, borderStyle);

    private static void InitializeDesigner(
        UpDownBaseDesigner designer,
        UpDownBase upDownBase,
        BorderStyle borderStyle)
    {
        Mock<ISite> site = new();
        site.Setup(s => s.DesignMode).Returns(true);
        site.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(new Mock<IDesignerHost>().Object);
        upDownBase.Site = site.Object;
        upDownBase.BorderStyle = borderStyle;
        designer.Initialize(upDownBase);
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
    [InlineData(VisualStylesMode.Net11)]
    [InlineData(VisualStylesMode.Latest)]
    public void Height_SetWithModernVisualStylesMode_ReturnsPreferredHeight(VisualStylesMode visualStylesMode)
    {
        InitializeDesigner(BorderStyle.Fixed3D);
        using DomainUpDown domainUpDown = new();
        using UpDownBaseDesigner domainUpDownDesigner = new();
        InitializeDesigner(domainUpDownDesigner, domainUpDown, BorderStyle.Fixed3D);

        foreach (UpDownBase upDownBase in new UpDownBase[] { _numericUpDown, domainUpDown })
        {
            upDownBase.AutoSize = false;
            upDownBase.VisualStylesMode = visualStylesMode;
            upDownBase.Height = upDownBase.PreferredHeight + 10;

            upDownBase.Height.Should().Be(upDownBase.PreferredHeight);
        }
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
