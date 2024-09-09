// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;

namespace System.Windows.Forms.Design.Tests;

public class CollectionEditVerbManagerTests
{
    public static TheoryData<string?, string> ValuesTheoryData()
        => new()
        {
            { null, SR.ToolStripItemCollectionEditorVerb },
            { "TestText", "TestText" }
        };

    [Theory]
    [MemberData(nameof(ValuesTheoryData))]
    public void CollectionEditVerbManager_EditItemsVerb_ReturnExpectedValue(string? text, string expectedValue)
    {
        using ToolStripButton toolStripButton = new();
        ToolStripItemDesigner toolStripItemDesigner = new();
        toolStripItemDesigner.Initialize(toolStripButton);

        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(ToolStripButton));
        PropertyDescriptor? propertyDescriptor = properties["Text"];

        CollectionEditVerbManager editor = new(text, toolStripItemDesigner, propertyDescriptor, true);
        editor.EditItemsVerb.Text.Should().Be(expectedValue);
        editor.EditItemsVerb.Enabled.Should().BeTrue();
        editor.EditItemsVerb.Supported.Should().BeTrue();
        editor.EditItemsVerb.Visible.Should().BeTrue();
        editor.EditItemsVerb.CommandID!.ID.Should().Be(8192);
    }
}

