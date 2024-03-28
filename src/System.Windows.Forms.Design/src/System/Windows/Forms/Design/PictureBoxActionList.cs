// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.ComponentModel;

namespace System.Windows.Forms.Design;

internal class PictureBoxActionList : DesignerActionList
{
    private readonly PictureBoxDesigner _designer;
    private readonly PictureBox _pictureBox;

    public PictureBoxActionList(PictureBoxDesigner designer)
        : base(designer.Component)
    {
        _designer = designer;
        _pictureBox = (PictureBox)designer.Component;
    }

    public PictureBoxSizeMode SizeMode
    {
        get
        {
            return _pictureBox.SizeMode;
        }
        set
        {
            TypeDescriptor.GetProperties(_pictureBox)["SizeMode"]!.SetValue(Component, value);
        }
    }

    public void ChooseImage()
    {
        EditorServiceContext.EditValue(_designer, Component!, "Image");
    }

    public override DesignerActionItemCollection GetSortedActionItems()
    {
        DesignerActionItemCollection items =
        [
            new DesignerActionMethodItem(this, "ChooseImage", SR.ChooseImageDisplayName, SR.PropertiesCategoryName, SR.ChooseImageDescription, true),
            new DesignerActionPropertyItem("SizeMode", SR.SizeModeDisplayName, SR.PropertiesCategoryName, SR.SizeModeDescription),
        ];
        return items;
    }
}
