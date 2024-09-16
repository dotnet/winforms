// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Editors.Tests;

public class TreeNodeCollectionEditorTests
{
    [WinFormsFact]
    public void TreeNodeCollectionEditor_Constructor_InitializesProperties()
    {
        Type type = typeof(TreeNode);
        TreeNodeCollectionEditor editor = new(type);
        editor.Should().NotBeNull();
    }

    [WinFormsFact]
    public void TreeNodeCollectionEditor_Property_HelpTopic()
    {
        Type type = typeof(TreeNode);
        TreeNodeCollectionEditor collectionEditor = new(type);
        string helpTopic = collectionEditor.TestAccessor().Dynamic.HelpTopic;
        helpTopic.Should().Be("net.ComponentModel.TreeNodeCollectionEditor");
    }

    [WinFormsFact]
    public void TreeNodeCollectionEditor_CreateCollectionForm_returnExpectedValue()
    {
        Type type = typeof(TreeNode);
        TreeNodeCollectionEditor collectionEditor = new(type);
        Form colletionForm;
        using (new NoAssertContext())
        {
            colletionForm = collectionEditor.TestAccessor().Dynamic.CreateCollectionForm();
        }

        colletionForm.Should().NotBeNull();
    }
}
