// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;

namespace System.Windows.Forms.Design.Tests;

public sealed class ButtonBaseDesignerTests
{
    [Fact]
    public void ButtonBaseDesigner_Constructor_Initialize_AutoResizeHandles()
    {
        using ButtonBaseDesigner buttonBaseDesigner = new();

        buttonBaseDesigner.AutoResizeHandles.Should().Be(true);
    }

    public static TheoryData<IDictionary<string, object>?> IDictionary_TheoryData => new()
    {
        null,
        new Dictionary<string, object>()
    };

    [Theory]
    [MemberData(nameof(IDictionary_TheoryData))]
    public void ButtonBaseDesigner_InitializeNewComponent_WithDefaultButton(IDictionary<string, object>? defaultValues)
    {
        using ButtonBaseDesigner buttonBaseDesigner = new();
        using Button button = new();
        buttonBaseDesigner.Initialize(button);

        buttonBaseDesigner.InitializeNewComponent((IDictionary?)defaultValues);
        Assert.False(button.IsHandleCreated);
    }

    [Theory]
    [MemberData(nameof(IDictionary_TheoryData))]
    public void ButtonBaseDesigner_InitializeNewComponent_NotInitialized(IDictionary<string, object>? defaultValues)
    {
        using ButtonBaseDesigner buttonBaseDesigner = new();
        Action action = () => buttonBaseDesigner.InitializeNewComponent((IDictionary?)defaultValues);
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ButtonBaseDesigner_SnapLinesWithDefaultButton_ShouldReturnExpectedCount()
    {
        using ButtonBaseDesigner buttonBaseDesigner = new();
        using Button button = new();
        buttonBaseDesigner.Initialize(button);

        buttonBaseDesigner.SnapLines.Count.Should().Be(9);
    }
}
