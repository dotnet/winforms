// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;

public class LinkAreaEditorLinkAreaUITests : IDisposable
{
    private readonly LinkAreaEditor.LinkAreaUI _linkAreaUI;
    private readonly Mock<IHelpService> _mockHelpService;

    public LinkAreaEditorLinkAreaUITests()
    {
        _mockHelpService = new();
        _linkAreaUI = new(_mockHelpService.Object);
    }

    public void Dispose()
    {
        _linkAreaUI.Dispose();
    }

    [Fact]
    public void LinkAreaUI_Constructor_InitializesCorrectly()
    {
        _linkAreaUI.Should().NotBeNull();
        _linkAreaUI.Should().BeOfType<LinkAreaEditor.LinkAreaUI>();

        IHelpService? helpServiceField = _linkAreaUI.TestAccessor().Dynamic._helpService;

        helpServiceField.Should().Be(_mockHelpService.Object);
    }

    [Fact]
    public void SampleText_SetGet_WorksCorrectly()
    {
        string sampleText = "Test Sample Text";
        _linkAreaUI.SampleText = sampleText;

        _linkAreaUI.SampleText.Should().Be(sampleText);
    }

    [Fact]
    public void SampleText_Set_UpdatesSelection()
    {
        string testText = "Sample text for testing";
        LinkArea linkArea = new(7, 4);
        _linkAreaUI.Start(linkArea);

        _linkAreaUI.SampleText = testText;

        _linkAreaUI.SampleText.Should().Be(testText);

        dynamic testAccessor = _linkAreaUI.TestAccessor().Dynamic;
        int selectionStart = (int)testAccessor._sampleEdit.SelectionStart;
        int selectionLength = (int)testAccessor._sampleEdit.SelectionLength;

        selectionStart.Should().Be(linkArea.Start);
        selectionLength.Should().Be(linkArea.Length);
    }

    [Fact]
    public void Value_SetGet_WorksCorrectly()
    {
        _linkAreaUI.Start(new object());
        object? initialValue = _linkAreaUI.Value;

        _linkAreaUI.End();
        object? finalValue = _linkAreaUI.Value;

        initialValue.Should().NotBeNull();
        finalValue.Should().BeNull();
    }

    [Fact]
    public void End_SetsValueToNull()
    {
        _linkAreaUI.Start(new LinkArea(1, 2));

        _linkAreaUI.End();

        _linkAreaUI.Value.Should().BeNull();
    }

    [Fact]
    public void Start_SetsValueAndUpdatesSelection()
    {
        object? testValue = new LinkArea(3, 4);
        _linkAreaUI.Start(testValue);

        dynamic dynamicAccessor = _linkAreaUI.TestAccessor().Dynamic;
        int selectionStart = (int)dynamicAccessor._sampleEdit.SelectionStart;
        int selectionLength = (int)dynamicAccessor._sampleEdit.SelectionLength;

        _linkAreaUI.Value.Should().Be(testValue);

        string sampleText = _linkAreaUI.SampleText;
        sampleText.Length.Should().BeGreaterThanOrEqualTo(selectionStart + selectionLength);

        selectionStart.Should().Be(3);
        selectionLength.Should().Be(4);
    }

    [Fact]
    public void Start_WithNullValue_DoesNotThrow()
    {
        Action action = () => _linkAreaUI.Start(null);

        action.Should().NotThrow();
        _linkAreaUI.Value.Should().BeNull();
    }
}
