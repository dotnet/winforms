// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ListViewGroupCollectionEditorTests
{
    private readonly Mock<ListViewGroupCollectionEditor> _mockEditor;

    public ListViewGroupCollectionEditorTests() =>
      _mockEditor = new(typeof(ListViewGroup)) { CallBase = true };

    [Fact]
    public void Constructor_InitializesCollectionType()
    {
        Type expectedType = typeof(ListViewGroup);

        ListViewGroupCollectionEditor editor = new(expectedType);

        Type actualType = editor.TestAccessor().Dynamic.CollectionType;

        actualType.Should().Be(expectedType);
    }

    [WinFormsFact]
    public void EditValue_SetsAndResetsEditValue()
    {
        Mock<ITypeDescriptorContext> mockContext = new();
        Mock<IServiceProvider> mockProvider = new();
        object inputValue = new();
        object expectedResult = new();

        _mockEditor
            .Setup(e => e.EditValue(mockContext.Object, mockProvider.Object, inputValue))
            .Returns(expectedResult);

        object result = _mockEditor.Object.EditValue(mockContext.Object, mockProvider.Object, inputValue);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void CreateInstance_CreatesListViewGroupWithUniqueName()
    {
        Mock<ListViewGroupCollection> mockCollection = new(new ListView());
        _mockEditor.Object.TestAccessor().Dynamic._editValue = mockCollection.Object;

        ListViewGroup? result = _mockEditor.Object.TestAccessor().Dynamic.CreateInstance(typeof(ListViewGroup)) as ListViewGroup;

        result?.Name.Should().BeOfType<string>();
        result?.Name.Should().StartWith("ListViewGroup");
        result?.GetType().Should().Be(typeof(ListViewGroup));
    }

    [Fact]
    public void CreateInstance_ThrowsException_WhenEditValueIsNull()
    {
        _mockEditor.Object.TestAccessor().Dynamic._editValue = null;

        Action action = () => _mockEditor.Object.TestAccessor().Dynamic.CreateInstance(typeof(ListViewGroup));

        action.Should().Throw<TargetInvocationException>();
    }
}
