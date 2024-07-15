// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Editors.Tests;

public class TreeNodeCollectionEditorTests
{
    [WinFormsFact]
    public void TreeNodeCollectionForm_Constructor_InitializesProperties()
    {
        Type type = typeof(TreeNode);
        TreeNodeCollectionEditor editor = new(type);
        Assert.NotNull(editor);
    }

    [WinFormsFact]
    public void TreeNodeCollectionForm_Property_HelpTopic()
    {
        Type type = typeof(TreeNode);
        SubTreeNodeCollectionEditor subCollectionEditor = new(type);
        Assert.Equal("net.ComponentModel.TreeNodeCollectionEditor", subCollectionEditor.HelpTopic);
    }

    internal class SubTreeNodeCollectionEditor : TreeNodeCollectionEditor
    {
        public SubTreeNodeCollectionEditor(Type type) : base(type)
        {
        }

        public new string HelpTopic => base.HelpTopic;
    }
}
