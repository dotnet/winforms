// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Tests;

public sealed class PictureBoxDesignerTests
{
    [Fact]
    public void AutoResizeHandles_WithCtor_ShouldBeTrue()
    {
        using PictureBoxDesigner pictureBoxDesigner = new();

        pictureBoxDesigner.AutoResizeHandles.Should().BeTrue();
    }

    [Fact]
    public void SelectionRules_WithDefaultPictureBox_ShouldThrowNullReferenceException()
    {
        using PictureBoxDesigner pictureBoxDesigner = new();
        using PictureBox pictureBox = new();
        pictureBoxDesigner.Initialize(pictureBox);

        Action action = () => _ = pictureBoxDesigner.SelectionRules;

        action.Should().ThrowExactly<NullReferenceException>();
    }

    [Fact]
    public void ActionLists_WithDefaultPictureBox_ShouldReturnExpectedCount()
    {
        using PictureBoxDesigner pictureBoxDesigner = new();
        using PictureBox pictureBox = new();
        pictureBoxDesigner.Initialize(pictureBox);

        pictureBoxDesigner.ActionLists.Count.Should().Be(1);
    }
}
