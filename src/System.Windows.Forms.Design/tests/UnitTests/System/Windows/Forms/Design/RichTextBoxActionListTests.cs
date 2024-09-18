// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public sealed class RichTextBoxActionListTests
{
    [Fact]
    public void Ctor_WithNull_ThrowsException()
    {
        Action action = () => new RichTextBoxActionList(null!);
        action.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void GetSortedActions_WithDesigner_GetsCorrectItemsCount()
    {
        using RichTextBoxDesigner designer = new();
        using RichTextBox richTextBox = new();
        designer.Initialize(richTextBox);
        RichTextBoxActionList actionList = new(designer);

        actionList.GetSortedActionItems().Count.Should().Be(1);
    }
}
