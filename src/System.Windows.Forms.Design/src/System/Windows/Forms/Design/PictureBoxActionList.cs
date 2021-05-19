﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.ComponentModel;

namespace System.Windows.Forms.Design
{
    internal class PictureBoxActionList : DesignerActionList
    {
        private readonly PictureBoxDesigner _designer;
        public PictureBoxActionList(PictureBoxDesigner designer) : base(designer.Component)
        {
            _designer = designer;
        }

        public PictureBoxSizeMode SizeMode
        {
            get
            {
                return ((PictureBox)Component).SizeMode;
            }
            set
            {
                TypeDescriptor.GetProperties(Component)["SizeMode"].SetValue(Component, value);
            }
        }

        public void ChooseImage()
        {
            EditorServiceContext.EditValue(_designer, Component, "Image");
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            items.Add(new DesignerActionMethodItem(this, "ChooseImage", SR.ChooseImageDisplayName, SR.PropertiesCategoryName, SR.ChooseImageDescription, true));
            items.Add(new DesignerActionPropertyItem("SizeMode", SR.SizeModeDisplayName, SR.PropertiesCategoryName, SR.SizeModeDescription));
            return items;
        }
    }
}

