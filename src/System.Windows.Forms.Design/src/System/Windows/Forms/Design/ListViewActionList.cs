// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.ComponentModel;

namespace System.Windows.Forms.Design
{
    internal class ListViewActionList : DesignerActionList
    {
        private readonly ComponentDesigner _designer;
        public ListViewActionList(ComponentDesigner designer) : base(designer.Component)
        {
            _designer = designer;
        }

        public void InvokeItemsDialog()
        {
            EditorServiceContext.EditValue(_designer, Component, "Items");
        }

        public void InvokeColumnsDialog()
        {
            EditorServiceContext.EditValue(_designer, Component, "Columns");
        }

        public void InvokeGroupsDialog()
        {
            EditorServiceContext.EditValue(_designer, Component, "Groups");
        }

        public View View
        {
            get
            {
                return ((ListView)Component).View;
            }
            set
            {
                TypeDescriptor.GetProperties(Component)["View"].SetValue(Component, value);
            }
        }

        public ImageList LargeImageList
        {
            get
            {
                return ((ListView)Component).LargeImageList;
            }
            set
            {
                TypeDescriptor.GetProperties(Component)["LargeImageList"].SetValue(Component, value);
            }
        }

        public ImageList SmallImageList
        {
            get
            {
                return ((ListView)Component).SmallImageList;
            }
            set
            {
                TypeDescriptor.GetProperties(Component)["SmallImageList"].SetValue(Component, value);
            }
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            items.Add(new DesignerActionMethodItem(this, "InvokeItemsDialog",
                SR.ListViewActionListEditItemsDisplayName,
                SR.PropertiesCategoryName,
                SR.ListViewActionListEditItemsDescription, true));
            items.Add(new DesignerActionMethodItem(this, "InvokeColumnsDialog",
                SR.ListViewActionListEditColumnsDisplayName,
                SR.PropertiesCategoryName,
                SR.ListViewActionListEditColumnsDescription, true));
            items.Add(new DesignerActionMethodItem(this, "InvokeGroupsDialog",
                SR.ListViewActionListEditGroupsDisplayName,
                SR.PropertiesCategoryName,
                SR.ListViewActionListEditGroupsDescription, true));
            items.Add(new DesignerActionPropertyItem("View",
                SR.ListViewActionListViewDisplayName,
                SR.PropertiesCategoryName,
                SR.ListViewActionListViewDescription));
            items.Add(new DesignerActionPropertyItem("SmallImageList",
                SR.ListViewActionListSmallImagesDisplayName,
                SR.PropertiesCategoryName,
                SR.ListViewActionListSmallImagesDescription));
            items.Add(new DesignerActionPropertyItem("LargeImageList",
                SR.ListViewActionListLargeImagesDisplayName,
                SR.PropertiesCategoryName,
                SR.ListViewActionListLargeImagesDescription));
            return items;
        }
    }
}

