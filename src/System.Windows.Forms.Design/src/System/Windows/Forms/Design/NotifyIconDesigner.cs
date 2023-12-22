// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    internal class NotifyIconDesigner : ComponentDesigner
    {
        private DesignerActionListCollection? _actionLists;

        public override void InitializeNewComponent(IDictionary? defaultValues)
        {
            base.InitializeNewComponent(defaultValues);
            NotifyIcon icon = (NotifyIcon)Component;
            icon.Visible = true;
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                _actionLists ??= new DesignerActionListCollection
                {
                    new NotifyIconActionList(this)
                };

                return _actionLists;
            }
        }
    }
}
