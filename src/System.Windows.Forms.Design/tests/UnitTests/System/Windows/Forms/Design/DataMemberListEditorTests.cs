// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.Drawing.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class DataMemberListEditorTests
{
    [Fact]
    public void DataMemberListEditor_GetEditStyle()
    {
        new DataMemberListEditor().GetEditStyle().Should().Be(UITypeEditorEditStyle.DropDown);
    }

    [Fact]
    public void DataMemberListEditor_IsDropDownResizable()
    {
        new DataMemberListEditor().IsDropDownResizable.Should().Be(true);
    }

    [Fact]
    public void DataMemberListEditor_EditValue()
    {
        DataMemberListEditor dataMemberListEditor = new();
        object value = "123";
        dataMemberListEditor.EditValue(null, null, value).Should().Be(value);

        Mock<ITypeDescriptorContext> mockTypeDescriptorContext = new(MockBehavior.Strict);
        dataMemberListEditor.EditValue(mockTypeDescriptorContext.Object, null, value).Should().Be(value);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        dataMemberListEditor.EditValue(null, mockServiceProvider.Object, value).Should().Be(value);

        mockTypeDescriptorContext.Setup(x => x.Instance).Returns(null);
        dataMemberListEditor.EditValue(null, mockServiceProvider.Object, value).Should().Be(value);
    }
}
