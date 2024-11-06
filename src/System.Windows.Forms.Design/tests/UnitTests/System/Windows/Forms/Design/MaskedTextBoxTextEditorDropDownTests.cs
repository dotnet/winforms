// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class MaskedTextBoxTextEditorDropDownTests
{
    [Fact]
    public void Value_ReturnsNull_WhenCancelled()
    {
        using MaskedTextBox maskedTextBox = new();
        using MaskedTextBoxTextEditorDropDown dropDown = new(maskedTextBox);
        dropDown.TestAccessor().Dynamic._cancel = true;

        dropDown.Value.Should().BeNull();
    }

    [Fact]
    public void Value_ReturnsText_WhenNotCancelled()
    {
        using MaskedTextBox maskedTextBox = new("00000");
        maskedTextBox.Text = "12345";
        using MaskedTextBoxTextEditorDropDown dropDown = new(maskedTextBox);
        dropDown.TestAccessor().Dynamic._cancel = false;

        dropDown.Value.Should().Be("12345");
    }

    [Fact]
    public void ProcessDialogKey_SetsCancel_WhenEscapePressed()
    {
        using MaskedTextBox maskedTextBox = new();
        using MaskedTextBoxTextEditorDropDown dropDown = new(maskedTextBox);
        bool processDialogKey = dropDown.TestAccessor().Dynamic.ProcessDialogKey(Keys.Escape);
        bool cancel = dropDown.TestAccessor().Dynamic._cancel;

        cancel.Should().BeTrue();
        processDialogKey.Should().BeFalse();
    }

    [Fact]
    public void MaskInputRejected_SetsError_WhenInputRejected()
    {
        using MaskedTextBox maskedTextBox = new("00：00");
        using MaskedTextBoxTextEditorDropDown dropDown = new(maskedTextBox);
        using ErrorProvider errorProvider = dropDown.TestAccessor().Dynamic._errorProvider;

        // No error when setting a correct format value.
        using MaskedTextBox dropDownMaskedTextBox = dropDown.TestAccessor().Dynamic._cloneMtb;
        dropDownMaskedTextBox.Text = "12：20";
        errorProvider.GetError(dropDownMaskedTextBox).Should().Be(string.Empty);

        // Incorrectly formatted values will result in the error "Error at position 0: expected number".
        dropDownMaskedTextBox.Text = "invalid";
        errorProvider.GetError(dropDownMaskedTextBox).Should().Contain(SR.MaskedTextBoxHintDigitExpected);
    }
}
