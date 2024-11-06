// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;

public class CollectionEditVerbManagerTests
{
    public static TheoryData<string?, string> ValuesTheoryData() => new()
    {
        { null, SR.ToolStripItemCollectionEditorVerb },
        { "TestText", "TestText" }
    };

    [Theory]
    [MemberData(nameof(ValuesTheoryData))]
    public void CollectionEditVerbManager_EditItemsVerb_ReturnExpectedValue(string? text, string expected)
    {
        using ToolStripButton toolStripButton = new();
        using ToolStripItemDesigner toolStripItemDesigner = new();
        toolStripItemDesigner.Initialize(toolStripButton);

        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(ToolStripButton));
        PropertyDescriptor? propertyDescriptor = properties[nameof(ToolStripButton.Text)];

        CollectionEditVerbManager editor = new(text: text, designer: toolStripItemDesigner, prop: propertyDescriptor, addToDesignerVerbs: true);
        editor.EditItemsVerb.Text.Should().Be(expected);
        editor.EditItemsVerb.Enabled.Should().BeTrue();
        editor.EditItemsVerb.Supported.Should().BeTrue();
        editor.EditItemsVerb.Visible.Should().BeTrue();
        editor.EditItemsVerb.CommandID.Should().Be(new CommandID(new Guid("74d21313-2aee-11d1-8bfb-00a0c90f26f7"), 8192));
    }
}
