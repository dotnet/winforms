// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    internal class TreeViewActionList : DesignerActionList
    {
        private readonly TreeViewDesigner _designer;
        public TreeViewActionList(TreeViewDesigner designer) : base(designer.Component)
        {
            _designer = designer;
        }

        public void InvokeNodesDialog()
        {
            EditorServiceContext.EditValue(_designer, Component, "Nodes");
        }

        public ImageList ImageList
        {
            get
            {
                return ((TreeView)Component).ImageList;
            }
            set
            {
                TypeDescriptor.GetProperties(Component)["ImageList"].SetValue(Component, value);
            }
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            items.Add(new DesignerActionMethodItem(this, "InvokeNodesDialog", SR.InvokeNodesDialogDisplayName, SR.PropertiesCategoryName, SR.InvokeNodesDialogDescription, true));
            items.Add(new DesignerActionPropertyItem("ImageList", SR.ImageListDisplayName, SR.PropertiesCategoryName, SR.ImageListDescription));
            return items;
        }
    }
}
