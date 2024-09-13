// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;

public class TextBoxDesignerTests
{
    [Fact]
    public void ActionLists_ShouldInitializeCorrectly()
    {
        using TextBoxDesigner designer = new();
        designer.Initialize(new TextBox());

        DesignerActionListCollection actionLists = designer.ActionLists;

        actionLists.Should().NotBeNull();
        actionLists.Count.Should().Be(1);
        actionLists.Should().BeAssignableTo<ICollection>();
        actionLists[0].Should().BeOfType<TextBoxActionList>();
    }

    [Fact]
    public void ActionLists_ShouldReturnSameInstance()
    {
        using TextBoxDesigner designer = new();
        designer.Initialize(new TextBox());

        DesignerActionListCollection actionLists1 = designer.ActionLists;
        DesignerActionListCollection actionLists2 = designer.ActionLists;

        actionLists1.Should().NotBeNull();
        actionLists2.Should().NotBeNull();
        actionLists1.Should().BeSameAs(actionLists2);
    }
}
