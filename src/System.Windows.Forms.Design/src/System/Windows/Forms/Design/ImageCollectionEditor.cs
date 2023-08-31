// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an editor for an image collection.
/// </summary>
internal class ImageCollectionEditor : CollectionEditor
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="ImageCollectionEditor"/> class.
    /// </summary>
    public ImageCollectionEditor(Type type)
        : base(type)
    {
    }

    /// <inheritdoc />
    protected override string GetDisplayText(object value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        string text;

        PropertyDescriptor property = TypeDescriptor.GetProperties(value)["Name"];
        if (property is not null)
        {
            text = (string)property.GetValue(value);
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
        }

        // If we want to show any type information - pretend we're an image.
        if (value is ImageListImage image)
        {
            value = image.Image;
        }

        text = TypeDescriptor.GetConverter(value).ConvertToString(value);
        if (string.IsNullOrEmpty(text))
        {
            text = value.GetType().Name;
        }

        return text;
    }

    /// <inheritdoc />
    protected override object CreateInstance(Type type)
    {
        UITypeEditor editor = (UITypeEditor)TypeDescriptor.GetEditor(typeof(ImageListImage), typeof(UITypeEditor));
        return editor.EditValue(Context, null);
    }

    /// <inheritdoc />
    protected override CollectionForm CreateCollectionForm()
    {
        CollectionForm form = base.CreateCollectionForm();

        // We want to switch the title to ImageCollection Editor instead of ImageListImage Editor. The collection
        // editor is actually using ImageListImages, while the collection we're actually editing is the Image
        // collection.
        form.Text = SR.ImageCollectionEditorFormText;
        return form;
    }

    /// <summary>
    ///  Gets images from the given array.
    /// </summary>
    /// <param name="array">The input is an <see cref="ArrayList"/> as an object.</param>
    /// <returns>An <see cref="ArrayList"/> which contains individual images that need to be created.</returns>
    protected override IList GetObjectsFromInstance(object array) => array as ArrayList;

    protected override object[] GetItems(object editValue)
    {
        if (editValue is not ImageList.ImageCollection source)
        {
            return base.GetItems(editValue);
        }

        var imageListImages = new ImageListImage[source.Count];
        for (int i = 0; i < source.Count; i++)
        {
            imageListImages[i] = new ImageListImage(source[i]) { Name = source.Keys[i] };
        }

        return imageListImages;
    }

    protected override object SetItems(object editValue, object[] value)
    {
        if (editValue is not ImageList.ImageCollection source)
        {
            return base.SetItems(editValue, value);
        }

        source.Clear();
        if (value is null || value.Length == 0)
        {
            return source;
        }

        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] is Image image)
            {
                source.Add(image);
            }
            else if (value[i] is ImageListImage imageListImage)
            {
                source.Add(imageListImage.Name, imageListImage.Image);
            }
        }

        return source;
    }
}
