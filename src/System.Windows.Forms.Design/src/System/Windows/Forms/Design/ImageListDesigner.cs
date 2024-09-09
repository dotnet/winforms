// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design;

/// <summary>
/// Provides design-time functionality for ImageList.
/// </summary>
internal partial class ImageListDesigner : ComponentDesigner
{
    // The designer keeps a backup copy of all the images in the image list. Unlike the image list,
    // we don't lose any information about size and color depth.
    private OriginalImageCollection? _originalImageCollection;
    private DesignerActionListCollection? _actionLists;

    /// <summary>
    /// Accessor method for the ColorDepth property on ImageList.
    /// We shadow this property at design time.
    /// </summary>
    private ColorDepth ColorDepth
    {
        get => ImageList.ColorDepth;
        set
        {
            ImageList.Images.Clear();
            ImageList.ColorDepth = value;
            Images.PopulateHandle();
        }
    }

    private bool ShouldSerializeColorDepth() => Images.Count == 0;

    /// <summary>
    /// Accessor method for the Images property on ImageList.
    /// We shadow this property at design time.
    /// </summary>
    private OriginalImageCollection Images
    {
        get
        {
            _originalImageCollection ??= new OriginalImageCollection(this);
            return _originalImageCollection;
        }
    }

    internal ImageList ImageList => (ImageList)Component;

    /// <summary>
    /// Accessor method for the ImageSize property on ImageList.
    /// We shadow this property at design time.
    /// </summary>
    private Size ImageSize
    {
        get => ImageList.ImageSize;
        set
        {
            ImageList.Images.Clear();
            ImageList.ImageSize = value;
            Images.PopulateHandle();
        }
    }

    private bool ShouldSerializeImageSize() => Images.Count == 0;

    private Color TransparentColor
    {
        get => ImageList.TransparentColor;
        set
        {
            ImageList.Images.Clear();
            ImageList.TransparentColor = value;
            Images.PopulateHandle();
        }
    }

    private bool ShouldSerializeTransparentColor() => !TransparentColor.Equals(Color.LightGray);

    /// <summary>
    /// Accessor method for the ImageStream property on ImageList.
    /// We shadow this property at design time.
    /// </summary>
    private ImageListStreamer? ImageStream
    {
        get => ImageList.ImageStream;
        set
        {
            ImageList.ImageStream = value;
            Images.ReloadFromImageList();
        }
    }

    /// <summary>
    /// Provides an opportunity for the designer to filter the properties.
    /// </summary>
    /// <param name="properties"></param>
    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);

        // Handle shadowed properties
        string[] shadowProps =
        [
            nameof(ColorDepth),
            nameof(ImageSize),
            nameof(ImageStream),
            nameof(TransparentColor)
        ];

        for (int i = 0; i < shadowProps.Length; i++)
        {
            if (properties[shadowProps[i]] is PropertyDescriptor prop)
            {
                properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(ImageListDesigner), prop, []);
            }
        }

        // replace this one seperately because it is of a different type (OriginalImageCollection) than
        // the original property (ImageCollection)
        PropertyDescriptor? imageProp = (PropertyDescriptor?)properties["Images"];
        if (imageProp is not null)
        {
            Attribute[] attrs = new Attribute[imageProp.Attributes.Count];
            imageProp.Attributes.CopyTo(attrs, 0);
            properties["Images"] = TypeDescriptor.CreateProperty(typeof(ImageListDesigner), "Images", typeof(OriginalImageCollection), attrs);
        }
    }

    public override DesignerActionListCollection ActionLists
    {
        get
        {
            _actionLists ??= new DesignerActionListCollection
            {
                new ImageListActionList(this)
            };

            return _actionLists;
        }
    }
}
