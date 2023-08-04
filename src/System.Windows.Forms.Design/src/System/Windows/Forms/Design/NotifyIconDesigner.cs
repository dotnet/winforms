// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    internal class NotifyIconDesigner : ComponentDesigner
    {
        private DesignerActionListCollection? _actionLists;

        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            base.InitializeNewComponent(defaultValues);
            NotifyIcon icon = (NotifyIcon)Component;
            icon.Visible = true;
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists is null)
                {
                    _actionLists = new DesignerActionListCollection();
                    _actionLists.Add(new NotifyIconActionList(this));
                }

                return _actionLists;
            }
        }
    }

    internal class NotifyIconActionList : DesignerActionList
    {
        private NotifyIconDesigner _designer;
        public NotifyIconActionList(NotifyIconDesigner designer) : base(designer.Component)
        {
            _designer = designer;
        }

        public void ChooseIcon()
        {
            EditorServiceContext.EditValue(_designer, Component, "Icon");
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            items.Add(new DesignerActionMethodItem(this, "ChooseIcon", SR.ChooseIconDisplayName, true));
            return items;
        }
    }
}
