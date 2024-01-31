// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Tests;

public sealed class LabelDesignerTests
{
    [Fact]
    public void AutoResizeHandles_WithCtor_ShouldBeTrue()
    {
        using LabelDesigner labelDesigner = new();

        labelDesigner.AutoResizeHandles.Should().BeTrue();
    }

    [Fact]
    public void SnapLines_WithDefaultLabel_ShouldReturnExpectedCount()
    {
        using LabelDesigner labelDesigner = new();
        using Label label = new();
        labelDesigner.Initialize(label);

        labelDesigner.SnapLines.Count.Should().Be(9);
    }

    [Fact]
    public void SelectionRules_WithDefaultLabel_ShouldThrowNullReferenceException()
    {
        using LabelDesigner labelDesigner = new();
        using Label label = new();
        labelDesigner.Initialize(label);

        Action action = () => _ = labelDesigner.SelectionRules;

        action.Should().ThrowExactly<NullReferenceException>();
    }
}
