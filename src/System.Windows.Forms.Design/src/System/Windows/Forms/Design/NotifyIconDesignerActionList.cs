// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    internal class NotifyIconActionList : DesignerActionList
    {
        private readonly NotifyIconDesigner _designer;
        public NotifyIconActionList(NotifyIconDesigner designer) : base(designer.Component)
        {
            _designer = designer;
        }

        public void ChooseIcon()
        {
            EditorServiceContext.EditValue(_designer, Component!, "Icon");
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items =
            [
                new DesignerActionMethodItem(this,
                    memberName: nameof(ChooseIcon),
                    displayName: SR.ChooseIconDisplayName,
                    includeAsDesignerVerb: true)
            ];
            return items;
        }
    }
}
