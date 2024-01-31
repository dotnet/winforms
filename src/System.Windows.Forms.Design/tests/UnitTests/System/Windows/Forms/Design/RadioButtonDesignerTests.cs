// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Tests;

public sealed class RadioButtonDesignerTests
{
    [Fact]
    public void ActionLists_WithDefaultRadioButton_ShouldReturnExpectedCount()
    {
        using RadioButtonDesigner radioButtonDesigner = new();
        using RadioButton radioButton = new();
        radioButtonDesigner.Initialize(radioButton);

        radioButtonDesigner.ActionLists.Count.Should().Be(0);
    }

    [Fact]
    public void SnapLines_WithDefaultRadioButton_ShouldReturnExpectedCount()
    {
        using RadioButtonDesigner radioButtonDesigner = new();
        using RadioButton radioButton = new();
        radioButtonDesigner.Initialize(radioButton);

        radioButtonDesigner.SnapLines.Count.Should().Be(9);
    }

    [Fact]
    public void SelectionRules_WithDefaultRadioButton_ShouldThrowNullReferenceException()
    {
        using RadioButtonDesigner radioButtonDesigner = new();
        using RadioButton radioButton = new();
        radioButtonDesigner.Initialize(radioButton);

        Action action = () => _ = radioButtonDesigner.SelectionRules;

        action.Should().ThrowExactly<NullReferenceException>();
    }
}
