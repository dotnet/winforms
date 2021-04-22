// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    internal class TextBoxActionList : DesignerActionList
    {
        public TextBoxActionList(TextBoxDesigner designer) : base(designer.Component)
        {
        }

        public bool Multiline
        {
            get
            {
                return ((TextBox)Component).Multiline;
            }
            set
            {
                TypeDescriptor.GetProperties(Component)["Multiline"].SetValue(Component, value);
            }
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            items.Add(new DesignerActionPropertyItem("Multiline", string.Format(SR.MultiLineDisplayName, SR.PropertiesCategoryName, SR.MultiLineDescription)));
            return items;
        }
    }
}
