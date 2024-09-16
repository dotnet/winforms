// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Drawing.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;
public class MaskPropertyEditorTests
{
    private readonly MaskedTextBox _maskedTextBox;
    private readonly MaskPropertyEditor _editor;

    public MaskPropertyEditorTests()
    {
        _maskedTextBox = new();
        _editor = new();
    }

    [WinFormsFact]
    public void EditValue_WhenContextOrProviderAreNull_ShouldReturnOriginalValue()
    {
        var context = new Mock<ITypeDescriptorContext>().Object;
        Mock<IServiceProvider> provider = new();

        object? result;
        if (context.Instance is MaskedTextBox maskedTextBox)
        {
            result = _editor.EditValue(context, provider.Object, maskedTextBox.Mask);
        }
        else
        {
            result = _maskedTextBox.Mask;
        }

        result.Should().Be(_maskedTextBox.Mask);
    }

    [Fact]
    public void GetPaintValueSupported_ShouldReturnFalse()
    {
        bool result = _editor.GetPaintValueSupported(null);
        result.Should().BeFalse();
    }

    [Fact]
    public void GetEditStyle_ShouldReturnModal()
    {
        var result = _editor.GetEditStyle(null);
        result.Should().Be(UITypeEditorEditStyle.Modal);
    }
}
