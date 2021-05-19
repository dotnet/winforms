﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    internal class RichTextBoxActionList : DesignerActionList
    {
        private readonly RichTextBoxDesigner _designer;

        public RichTextBoxActionList(RichTextBoxDesigner designer) : base(designer.Component)
        {
            _designer = designer;
        }

        public void EditLines()
        {
            EditorServiceContext.EditValue(_designer, Component, "Lines");
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            items.Add(new DesignerActionMethodItem(this, "EditLines", SR.EditLinesDisplayName, SR.LinksCategoryName, SR.EditLinesDescription, true));
            return items;
        }
    }
}

