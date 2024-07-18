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
        bool autoResizeHandles = buttonBaseDesigner.AutoResizeHandles;

        buttonBaseDesigner.Should().NotBeNull();
        autoResizeHandles.Should().Be(true);
    }

    public static IEnumerable<object[]> IDictionary_TestData()
    {
        yield return new object[] { null! };
        yield return new object[] { new Dictionary<string, object>() };
    }

    [Theory]
    [MemberData(nameof(IDictionary_TestData))]
    public void ButtonBaseDesigner_InitializeNewComponent_WithDefaultButton(IDictionary defaultValues)
    {
        using ButtonBaseDesigner buttonBaseDesigner = new();
        using Button button = new();
        buttonBaseDesigner.Initialize(button);

        buttonBaseDesigner.InitializeNewComponent(defaultValues);
    }

    [Theory]
    [MemberData(nameof(IDictionary_TestData))]
    public void ButtonBaseDesigner_InitializeNewComponent_NotInitialized(IDictionary defaultValues)
    {
        using ButtonBaseDesigner buttonBaseDesigner = new();
        Action action = () => buttonBaseDesigner.InitializeNewComponent(defaultValues);
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
