// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

// Miscellaneous utilities
internal static class ImageListUtils
{
    public static PropertyDescriptor? GetImageListProperty(PropertyDescriptor? currentComponent, ref object instance)
    {
        // Multiple selection is not supported.
        if (instance is object[])
        {
            return null;
        }

        if (currentComponent is null
            || !currentComponent.TryGetAttribute(out RelatedImageListAttribute? relatedAttribute)
            || relatedAttribute.RelatedImageList is null)
        {
            return null;
        }

        PropertyDescriptor? imageListProperty = null;
        object? parentInstance = instance;

        string[] pathInfo = relatedAttribute.RelatedImageList.Split('.');
        for (int i = 0; i < pathInfo.Length; i++)
        {
            if (parentInstance is null)
            {
                Debug.Fail("A property specified in the path is null or not yet instantiated at this time");
                break;
            }

            PropertyDescriptor? property = TypeDescriptor.GetProperties(parentInstance)[pathInfo[i]];
            if (property is null)
            {
                Debug.Fail("The path specified to the property is wrong");
                break;
            }

            if (i == pathInfo.Length - 1)
            {
                // We're on the last one, look if that's our guy
                if (typeof(ImageList).IsAssignableFrom(property.PropertyType))
                {
                    instance = parentInstance;
                    imageListProperty = property;
                    break;
                }
            }
            else
            {
                parentInstance = property.GetValue(parentInstance);
            }
        }

        return imageListProperty;
    }
}
