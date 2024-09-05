// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an editor for visually picking an image index.
/// </summary>
internal class ImageIndexEditor : UITypeEditor
{
    protected ImageList? _currentImageList;
    protected WeakReference<PropertyDescriptor>? _currentImageListPropertyReference;
    protected object? _currentInstance;
    protected string _parentImageListProperty = "Parent";
    protected string? _imageListPropertyName;

    /// <summary>
    ///  Initializes a new instance of the <see cref="ImageIndexEditor"/> class.
    /// </summary>
    public ImageIndexEditor()
    {
        // Get the type editor for images. We use the properties on this to determine if we support value painting, etc.
        ImageEditor = TypeDescriptorHelper.GetEditor<UITypeEditor>(typeof(Image));
    }

    internal UITypeEditor? ImageEditor { get; }

    internal string ParentImageListProperty => _parentImageListProperty;

    /// <summary>
    ///  Retrieves an image for the current context at current index.
    /// </summary>
    protected virtual Image? GetImage(ITypeDescriptorContext context, int index, string? key, bool useIntIndex)
    {
        object? instance = context.Instance;

        // We would not know what to do in this case anyway (i.e. multiple selection of objects)
        if (instance is object[] || (index < 0 && key is null))
        {
            return null;
        }

        // If the instances are different, then we need to re-acquire our image list.

        if (_currentImageList is null
            || instance != _currentInstance
            || (_currentImageListPropertyReference is not null &&
                _currentImageListPropertyReference.TryGetTarget(out PropertyDescriptor? currentProperty) &&
                (ImageList?)currentProperty.GetValue(_currentInstance) != _currentImageList))
        {
            _currentInstance = instance;

            // First look for an attribute.
            PropertyDescriptor? imageListProperty = GetImageListProperty(context.PropertyDescriptor!, ref instance);

            // Not found as an attribute, do the old behavior.
            while (instance is not null && imageListProperty is null)
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);

                foreach (PropertyDescriptor property in properties)
                {
                    if (typeof(ImageList).IsAssignableFrom(property.PropertyType))
                    {
                        imageListProperty = property;
                        break;
                    }
                }

                if (imageListProperty is null)
                {
                    // We didn't find the image list in this component. See if the
                    // component has a "parent" property. If so, walk the tree.
                    instance = properties[ParentImageListProperty]?.GetValue(instance);
                }
            }

            if (imageListProperty is not null)
            {
                _currentImageList = (ImageList?)imageListProperty.GetValue(instance);
                _currentImageListPropertyReference = new WeakReference<PropertyDescriptor>(imageListProperty);
                _currentInstance = instance;
            }
        }

        if (_currentImageList is not null)
        {
            if (useIntIndex)
            {
                if (index < _currentImageList.Images.Count)
                {
                    return _currentImageList.Images[index];
                }
            }
            else
            {
                return _currentImageList.Images[key!];
            }
        }

        return null;
    }

    /// <inheritdoc />
    public override bool GetPaintValueSupported(ITypeDescriptorContext? context)
        => ImageEditor?.GetPaintValueSupported(context) ?? false;

    /// <inheritdoc />
    public override void PaintValue(PaintValueEventArgs e)
    {
        if (ImageEditor is null)
        {
            return;
        }

        Image? image = null;

        if (e.Value is int integer)
        {
            image = GetImage(e.Context!, integer, null, true);
        }
        else if (e.Value is string text)
        {
            image = GetImage(e.Context!, -1, text, false);
        }

        if (image is not null)
        {
            ImageEditor.PaintValue(new PaintValueEventArgs(e.Context, image, e.Graphics, e.Bounds));
        }
    }

    internal static PropertyDescriptor? GetImageListProperty(PropertyDescriptor currentComponent, ref object? instance)
    {
        // Multiple selection is not supported by this class.
        if (instance is object[]
            || !currentComponent.TryGetAttribute(out RelatedImageListAttribute? imageListAttribute))
        {
            return null;
        }

        object? parentInstance = instance;

        if (imageListAttribute.RelatedImageList is null)
        {
            return null;
        }

        string[] pathInfo = imageListAttribute.RelatedImageList.Split('.');
        for (int i = 0; i < pathInfo.Length; i++)
        {
            if (parentInstance is null)
            {
                Debug.Fail("A property specified in the path is null or not yet instantiated at this time.");
                break;
            }

            PropertyDescriptor? property = TypeDescriptor.GetProperties(parentInstance)[pathInfo[i]];
            if (property is null)
            {
                Debug.Fail("The path specified to the property is wrong.");
                break;
            }

            if (i == pathInfo.Length - 1)
            {
                // We're on the last one, look if that's our match.
                if (typeof(ImageList).IsAssignableFrom(property.PropertyType))
                {
                    instance = parentInstance;
                    return property;
                }
            }
            else
            {
                parentInstance = property.GetValue(parentInstance);
            }
        }

        return null;
    }
}
