// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel;
using static System.Windows.Forms.Design.LinkAreaEditor;

namespace System.Windows.Forms.Design.Tests;

public sealed class LinkAreaEditorTests
{
    [Fact]
    public void EditValue_ShouldBehaveCorrectly_BasedOnEditorServiceAvailability()
    {
        LinkAreaEditor editor = new();
        Mock<ITypeDescriptorContext> context = new();
        Mock<IServiceProvider> provider = new();
        Mock<IWindowsFormsEditorService> editorService = new();
        LinkArea value = new(0, 5);
        using Label instance = new() { Text = "Initial Text" };
        var property = TypeDescriptor.GetProperties(instance)["Text"];

        provider.Setup(p => p.GetService(typeof(IWindowsFormsEditorService))).Returns((object?)null);
        editor.EditValue(context.Object, provider.Object, value).Should().Be(value);

        provider.Setup(p => p.GetService(typeof(IWindowsFormsEditorService))).Returns(editorService.Object);
        editorService.Setup(es => es.ShowDialog(It.IsAny<Form>())).Returns(DialogResult.OK);
        var result = editor.EditValue(context.Object, provider.Object, value);
        result.Should().BeOfType<LinkArea>();
        if (result is LinkArea linkAreaResult)
        {
            linkAreaResult.Length.Should().Be(value.Length);
        }

        property?.SetValue(instance, "Updated Text");
        context.Setup(c => c.Instance).Returns(instance);
        editor.EditValue(context.Object, provider.Object, value).Should().NotBeNull();
        property?.GetValue(instance).Should().Be("Updated Text");

        editorService.Setup(es => es.ShowDialog(It.IsAny<Form>())).Callback<Form>(form =>
        {
            if (form is LinkAreaUI linkAreaUI)
            {
                linkAreaUI.SampleText = "Updated Text2";
            }
        }).Returns(DialogResult.OK);
        editor.EditValue(context.Object, provider.Object, value).Should().NotBeNull();
        property?.GetValue(instance).Should().Be("Updated Text2");
    }
}
