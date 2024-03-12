// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class ListControlUnboundActionList : DesignerActionList
{
    private readonly ComponentDesigner _designer;

    public ListControlUnboundActionList(ComponentDesigner designer)
        : base(designer.Component)
    {
        _designer = designer;
    }

    public void InvokeItemsDialog()
    {
        EditorServiceContext.EditValue(_designer, Component!, "Items");
    }

    public override DesignerActionItemCollection GetSortedActionItems()
    {
        DesignerActionItemCollection returnItems =
        [
            new DesignerActionMethodItem(this, "InvokeItemsDialog",
                SR.ListControlUnboundActionListEditItemsDisplayName,
                SR.ItemsCategoryName,
                SR.ListControlUnboundActionListEditItemsDescription, true),
        ];
        return returnItems;
    }
}
