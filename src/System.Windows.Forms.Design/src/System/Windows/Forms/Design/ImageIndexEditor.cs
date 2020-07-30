// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    /// Provides an editor for visually picking an image index.
    /// </summary>
    internal class ImageIndexEditor : UITypeEditor
    {
        protected ImageList currentImageList;
        protected WeakReference currentImageListPropRef;
        protected object currentInstance;
        protected UITypeEditor imageEditor;
        protected string parentImageListProperty = "Parent";
        // disable csharp compiler warning #0414: field assigned unused value
        protected string imageListPropertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.Design.ImageIndexEditor'/> class.
        /// </summary>
        public ImageIndexEditor()
        {
            // Get the type editor for images.  We use the properties on
            // this to determine if we support value painting, etc.
            imageEditor = (UITypeEditor)TypeDescriptor.GetEditor(typeof(Image), typeof(UITypeEditor));
        }

        internal UITypeEditor ImageEditor
        {
            get => imageEditor;
        }

        internal string ParentImageListProperty
        {
            get => parentImageListProperty;
        }

        /// <summary>
        /// Retrieves an image for the current context at current index.
        /// </summary>
        protected virtual Image GetImage(ITypeDescriptorContext context, int index, string key, bool useIntIndex)
        {
            Image image = null;
            object instance = context.Instance;

            // we would not know what to do in this case anyway (i.e. multiple selection of objects)
            if (instance is object[])
            {
                return null;
            }

            // If the instances are different, then we need to re-acquire our image list.
            if ((index >= 0) || (key != null))
            {
                PropertyDescriptor currentImageListProp = null;
                if (currentImageListPropRef != null)
                {
                    currentImageListProp = currentImageListPropRef.Target as PropertyDescriptor;
                }
                if (currentImageList is null ||
                    instance != currentInstance ||
                    (currentImageListProp != null && (ImageList)currentImageListProp.GetValue(currentInstance) != currentImageList))
                {
                    currentInstance = instance;
                    // first look for an attribute
                    PropertyDescriptor imageListProp = GetImageListProperty(context.PropertyDescriptor, ref instance);

                    // not found as an attribute, do the old behavior
                    while (instance != null && imageListProp is null)
                    {
                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(instance);

                        foreach (PropertyDescriptor prop in props)
                        {
                            if (typeof(ImageList).IsAssignableFrom(prop.PropertyType))
                            {
                                imageListProp = prop;
                                break;
                            }
                        }

                        if (imageListProp is null)
                        {
                            // We didn't find the image list in this component.  See if the
                            // component has a "parent" property.  If so, walk the tree...
                            PropertyDescriptor parentProp = props[ParentImageListProperty];
                            if (parentProp != null)
                            {
                                instance = parentProp.GetValue(instance);
                            }
                            else
                            {
                                // Stick a fork in us, we're done.
                                instance = null;
                            }
                        }
                    }

                    if (imageListProp != null)
                    {
                        currentImageList = (ImageList)imageListProp.GetValue(instance);
                        currentImageListPropRef = new WeakReference(imageListProp);
                        currentInstance = instance;
                    }
                }

                if (currentImageList != null)
                {
                    if (useIntIndex)
                    {
                        if (currentImageList != null && index < currentImageList.Images.Count)
                        {
                            index = (index > 0) ? index : 0;
                            image = currentImageList.Images[index];
                        }
                    }
                    else
                    {
                        image = currentImageList.Images[key];
                    }
                }
                else
                {
                    // no image list, no image
                    image = null;
                }
            }

            return image;
        }

        /// <summary>
        /// Gets a value indicating whether this editor supports the painting of a representation of an object's value.
        /// </summary>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
            => imageEditor != null ? imageEditor.GetPaintValueSupported(context) : false;

        /// <summary>
        /// Paints a representative value of the given object to the provided canvas. Painting should be done within the boundaries of the provided rectangle.
        /// </summary>
        public override void PaintValue(PaintValueEventArgs e)
        {
            if (ImageEditor != null)
            {
                Image image = null;

                if (e.Value is int)
                {
                    image = GetImage(e.Context, (int)e.Value, null, true);
                }
                else if (e.Value is string)
                {
                    image = GetImage(e.Context, -1, (string)e.Value, false);
                }

                if (image != null)
                {
                    ImageEditor.PaintValue(new PaintValueEventArgs(e.Context, image, e.Graphics, e.Bounds));
                }
            }
        }

        internal static PropertyDescriptor GetImageListProperty(PropertyDescriptor currentComponent, ref object instance)
        {
            //multiple selection is not supported by this class
            if (instance is object[])
            {
                return null;
            }

            PropertyDescriptor imageListProp = null;
            object parentInstance = instance;

            RelatedImageListAttribute relILAttr = currentComponent.Attributes[typeof(RelatedImageListAttribute)] as RelatedImageListAttribute;
            if (relILAttr != null)
            {
                var pathInfo = relILAttr.RelatedImageList.Split('.');
                for (int i = 0; i < pathInfo.Length; i++)
                {
                    if (parentInstance is null)
                    {
                        Debug.Fail("A property specified in the path is null or not yet instanciated at this time");
                        break; // path is wrong
                    }
                    var prop = TypeDescriptor.GetProperties(parentInstance)[pathInfo[i]];
                    if (prop is null)
                    {
                        Debug.Fail("The path specified to the property is wrong");
                        break; // path is wrong
                    }
                    if (i == pathInfo.Length - 1)
                    {
                        // we're on the last one, look if that's our guy
                        if (typeof(ImageList).IsAssignableFrom(prop.PropertyType))
                        {
                            instance = parentInstance;
                            imageListProp = prop;
                            break;
                        }
                    }
                    else
                    {
                        parentInstance = prop.GetValue(parentInstance);
                    }
                }
            }

            return imageListProp;
        }
    }
}
