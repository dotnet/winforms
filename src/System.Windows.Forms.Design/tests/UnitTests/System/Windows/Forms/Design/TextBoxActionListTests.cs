// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;

public sealed class TextBoxActionListTests
{
    [Fact]
    public void Multiline_GetSet_ReturnsExpected()
    {
        using TextBox textBox = new();
        using TextBoxDesigner designer = new();
        designer.Initialize(textBox);
        TextBoxActionList actionList = new(designer)
        {
            Multiline = true
        };
        textBox.Multiline.Should().BeTrue();

        actionList.Multiline = false;
        textBox.Multiline.Should().BeFalse();
    }

    [Fact]
    public void GetSortedActionItems_ReturnsExpected()
    {
        using TextBox textBox = new();
        using TextBoxDesigner designer = new();
        designer.Initialize(textBox);
        TextBoxActionList actionList = new(designer);

        var items = actionList.GetSortedActionItems();
        items.Count.Should().Be(1);
        items[0].Should().BeOfType<DesignerActionPropertyItem>();
        ((DesignerActionPropertyItem)items[0]).MemberName.Should().Be("Multiline");
    }
}
