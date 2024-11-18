// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design.Tests;

public class TextBoxBaseDesignerTests : IDisposable
{
    private readonly TextBoxBaseDesigner _designer;
    private readonly TextBox _textbox;

    public TextBoxBaseDesignerTests()
    {
        _designer = new();
        _textbox = new();
        Mock<ISite> site = new();
        site.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(new Mock<IDesignerHost>().Object);
        _textbox.Site = site.Object;
    }

    public void Dispose()
    {
        _designer.Dispose();
        _textbox.Dispose();
    }

    [Fact]
    public void Constructor_SetsAutoResizeHandlesToTrue_AND_SelectionRules_ReturnsCorrectRules()
    {
        _designer.AutoResizeHandles.Should().BeTrue();
        _designer.Initialize(_textbox);

        SelectionRules rules = _designer.SelectionRules;

        rules.Should().NotHaveFlag(SelectionRules.TopSizeable | SelectionRules.BottomSizeable);
    }

    [WinFormsTheory]
    [InlineData(BorderStyle.None, 0)]
    [InlineData(BorderStyle.FixedSingle, 2)]
    [InlineData(BorderStyle.Fixed3D, 3)]
    public void SnapLines_ReturnsCorrectSnapLines(BorderStyle borderStyle, int expectedBaselineOffset)
    {
        _textbox.BorderStyle = borderStyle;
        _designer.Initialize(_textbox);

        List<SnapLine> snapLines = (List<SnapLine>)_designer.SnapLines;

        snapLines.Should().NotBeNull();

        SnapLine? baselineSnapLine = snapLines.Cast<SnapLine>().FirstOrDefault(sl => sl.SnapLineType == SnapLineType.Baseline);

        baselineSnapLine.Should().NotBeNull();
        baselineSnapLine!.Priority.Should().Be(SnapLinePriority.Medium);
        int expectedBaseline = DesignerUtils.GetTextBaseline(_textbox, Drawing.ContentAlignment.TopLeft) + expectedBaselineOffset;

        baselineSnapLine.Offset.Should().Be(expectedBaseline);
    }

    [WinFormsTheory]
    [InlineData(false, true, SelectionRules.LeftSizeable | SelectionRules.RightSizeable)]
    [InlineData(true, true, SelectionRules.AllSizeable)]
    [InlineData(false, false, SelectionRules.AllSizeable)]
    public void SelectionRules_ReturnsCorrectRules(bool multiline, bool autoSize, SelectionRules expectedRules)
    {
        _textbox.Multiline = multiline;
        _textbox.AutoSize = autoSize;
        _designer.Initialize(_textbox);

        _designer.SelectionRules.Should().HaveFlag(expectedRules);
    }

    [Fact]
    public void InitializeNewComponent_ClearsTextProperty()
    {
        _textbox.Text.Should().BeEmpty();

        _textbox.Text = "Test Text";
        _designer.Initialize(_textbox);

        _textbox.Text.Should().Be("Test Text");

        _designer.InitializeNewComponent(null);

        _textbox.Text.Should().BeEmpty();
    }
}
