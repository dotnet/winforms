// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Tests;

public sealed class MaskedTextBoxDesignerTests
{
    [Fact]
    public void SnapLines_WithDefaultMaskedTextBox_ShouldReturnExpectedCount()
    {
        using MaskedTextBoxDesigner maskedTextBoxDesigner = new();
        using MaskedTextBox maskedTextBox = new();
        maskedTextBoxDesigner.Initialize(maskedTextBox);

        maskedTextBoxDesigner.SnapLines.Count.Should().Be(9);
    }

    [Fact]
    public void SelectionRules_WithDefaultMaskedTextBox_ShouldReturnExpectedValue()
    {
        using MaskedTextBoxDesigner maskedTextBoxDesigner = new();
        using MaskedTextBox maskedTextBox = new();
        maskedTextBoxDesigner.Initialize(maskedTextBox);

        SelectionRules selectionRules;
        using (new NoAssertContext())
        {
            selectionRules = maskedTextBoxDesigner.SelectionRules;
        }

        selectionRules.Should().Be(SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable | SelectionRules.Visible);
    }

    [Fact]
    public void Verbs_WithDefaultMaskedTextBox_ShouldReturnExpectedCount()
    {
        using MaskedTextBoxDesigner maskedTextBoxDesigner = new();
        using MaskedTextBox maskedTextBox = new();
        maskedTextBoxDesigner.Initialize(maskedTextBox);

        maskedTextBoxDesigner.Verbs.Count.Should().Be(1);
    }
}
