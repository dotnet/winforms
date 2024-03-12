// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design;

internal class ImageListActionList : DesignerActionList
{
    private readonly ImageListDesigner _designer;

    public ImageListActionList(ImageListDesigner designer)
        : base(designer.Component)
    {
        _designer = designer;
    }

    public void ChooseImages() => EditorServiceContext.EditValue(_designer, Component!, "Images");

    public ColorDepth ColorDepth
    {
        get => (Component as ImageList)!.ColorDepth;
        set
        {
            if (Component is not null)
            {
                TypeDescriptor.GetProperties(Component)[nameof(ColorDepth)]?.SetValue(Component, value);
            }
        }
    }

    public Size ImageSize
    {
        get => (Component as ImageList)!.ImageSize;
        set
        {
            if (Component is not null)
            {
                TypeDescriptor.GetProperties(Component)[nameof(ImageSize)]?.SetValue(Component, value);
            }
        }
    }

    public override DesignerActionItemCollection GetSortedActionItems()
    {
        DesignerActionItemCollection items =
        [
            new DesignerActionPropertyItem(nameof(ImageSize), SR.ImageListActionList_ImageSizeDisplayName, SR.PropertiesCategoryName, SR.ImageListActionList_ImageSizeDescription),
            new DesignerActionPropertyItem(nameof(ColorDepth), SR.ImageListActionList_ColorDepthDisplayName, SR.PropertiesCategoryName, SR.ImageListActionList_ColorDepthDescription),
            new DesignerActionMethodItem(this, nameof(ChooseImages), SR.ImageListActionList_ChooseImagesDisplayName, SR.LinksCategoryName, SR.ImageListActionList_ChooseImagesDescription, true)
        ];
        return items;
    }
}
