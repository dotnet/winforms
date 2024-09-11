// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public sealed class RichTextBoxDesignerTests : IDisposable
{
    private readonly RichTextBoxDesigner _designer = new();

    private readonly RichTextBox _richTextBox = new();

    public RichTextBoxDesignerTests()
    {
        _designer.Initialize(_richTextBox);
    }

    public void Dispose()
    {
        _richTextBox.Dispose();
        _designer.Dispose();
    }

    [Fact]
    public void RichTextBoxDesigner_InitializeNewComponent()
    {
        _designer.Control.IsHandleCreated.Should().Be(false);
        _designer.InitializeNewComponent(new Dictionary<string, string>());
        _designer.Control.IsHandleCreated.Should().Be(true);
    }

    [Fact]
    public void ActionLists_WithDefaultRichTextBox_ShouldReturnExpectedCount()
    {
        _designer.ActionLists.Count.Should().Be(1);
    }

    [Fact]
    public void SnapLines_WithDefaultRichTextBox_ShouldReturnExpectedCount()
    {
        _designer.SnapLines.Count.Should().Be(9);
    }
}
