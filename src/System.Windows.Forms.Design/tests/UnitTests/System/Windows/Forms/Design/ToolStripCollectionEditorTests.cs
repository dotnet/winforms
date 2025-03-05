// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripCollectionEditorTests
{
    private readonly ToolStripCollectionEditor _editor;

    public ToolStripCollectionEditorTests()
    {
        _editor = new();
    }

    [Fact]
    public void ToolStripCollectionEditor_CreateCollectionForm_DoesNotThrowException()
    {
        Action act = () => _editor.TestAccessor().Dynamic.CreateCollectionForm();
        act.Should().NotThrow();
    }

    [Fact]
    public void ToolStripCollectionEditor_HelpTopic_ReturnsExpectedValue()
    {
        string helpTopic = _editor.TestAccessor().Dynamic.HelpTopic;
        helpTopic.Should().Be("net.ComponentModel.ToolStripCollectionEditor");
    }

    [Fact]
    public void ToolStripCollectionEditor_EditValue_NullProvider_ReturnsNull()
    {
        object? result = _editor.EditValue(context: null, provider: null!, value: new object());

        result.Should().BeNull();
    }

    [Fact]
    public void ToolStripCollectionEditor_EditValue_WithProvider_ReturnsExpected()
    {
        Mock<ITypeDescriptorContext> mockTypeDescriptorContext = new();
        Mock<IServiceProvider> mockServiceProvider = new();
        object? result = _editor.EditValue(mockTypeDescriptorContext.Object, mockServiceProvider.Object, new object());

        result.Should().NotBeNull();
    }
}
