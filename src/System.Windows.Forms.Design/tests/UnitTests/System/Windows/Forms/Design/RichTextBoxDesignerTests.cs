// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Tests;

public sealed class RichTextBoxDesignerTests
{
    [Fact]
    public void ActionLists_WithDefaultRichTextBox_ShouldReturnExpectedCount()
    {
        using RichTextBoxDesigner richTextBoxDesigner = new();
        using RichTextBox richTextBox = new();
        richTextBoxDesigner.Initialize(richTextBox);

        richTextBoxDesigner.ActionLists.Count.Should().Be(1);
    }

    [Fact]
    public void SnapLines_WithDefaultRichTextBox_ShouldReturnExpectedCount()
    {
        using RichTextBoxDesigner richTextBoxDesigner = new();
        using RichTextBox richTextBox = new();
        richTextBoxDesigner.Initialize(richTextBox);

        richTextBoxDesigner.SnapLines.Count.Should().Be(9);
    }
}
