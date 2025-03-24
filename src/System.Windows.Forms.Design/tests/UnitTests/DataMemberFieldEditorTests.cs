// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using Moq;

namespace System.Windows.Forms.Design.Editors.Tests;

public class DataMemberFieldEditorTests
{
    private readonly DataMemberFieldEditor _editor = new();

    [Fact]
    public void Ctor_HasDefaultProperties() => _editor.IsDropDownResizable.Should().BeTrue();

    [Fact]
    public void GetEditStyle_ContextIsNull_ReturnsDropDown() => _editor.GetEditStyle(null).Should().Be(UITypeEditorEditStyle.DropDown);

    public static IEnumerable<object[]> EditValueCases()
    {
        string text = "Edited Text";

        Mock<ITypeDescriptorContext>? contextMock = new();
        contextMock.Setup(c => c.Instance).Returns(new ComboBox());

        Mock<IServiceProvider> providerMock = new();
        providerMock.Setup(p => p.GetService(typeof(IWindowsFormsEditorService))).Returns(new Mock<IWindowsFormsEditorService>().Object);

        yield return new object[] { null!, null!, null! };
        yield return new object[] { null!, null!, text };

        yield return new object[]
        {
            contextMock.Object,
            providerMock.Object,
            text
        };
    }

    [Theory]
    [MemberData(nameof(EditValueCases))]
    public void EditValue_WithValidInput_ReturnsValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        object? result = _editor.EditValue(context, provider, value);

        result.Should().Be(value);
    }
}
