// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class TreeViewActionList : DesignerActionList
{
    private readonly TreeViewDesigner _designer;
    private readonly TreeView _treeView;

    public TreeViewActionList(TreeViewDesigner designer)
        : base(designer.Component)
    {
        _designer = designer;
        _treeView = (TreeView)designer.Component;
    }

    public void InvokeNodesDialog()
    {
        EditorServiceContext.EditValue(_designer, Component!, "Nodes");
    }

    public ImageList? ImageList
    {
        get
        {
            return _treeView.ImageList;
        }
        set
        {
            TypeDescriptor.GetProperties(_treeView)["ImageList"]!.SetValue(Component, value);
        }
    }

    public override DesignerActionItemCollection GetSortedActionItems()
    {
        DesignerActionItemCollection items =
        [
            new DesignerActionMethodItem(this, "InvokeNodesDialog", SR.InvokeNodesDialogDisplayName, SR.PropertiesCategoryName, SR.InvokeNodesDialogDescription, true),
            new DesignerActionPropertyItem("ImageList", SR.ImageListDisplayName, SR.PropertiesCategoryName, SR.ImageListDescription),
        ];
        return items;
    }
}
