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
    public void SelectionRules_WithDefaultPictureBox_ShouldReturnExpectedValue()
    {
        using PictureBoxDesigner pictureBoxDesigner = new();
        using PictureBox pictureBox = new();
        pictureBoxDesigner.Initialize(pictureBox);

        SelectionRules selectionRules;
        using (new NoAssertContext())
        {
            selectionRules = pictureBoxDesigner.SelectionRules;
        }

        selectionRules.Should().Be(SelectionRules.AllSizeable | SelectionRules.Moveable | SelectionRules.Visible);
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
