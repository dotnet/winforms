// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  ImageIndexConverter is a class that can be used to convert
    ///  image index values one data type to another.
    /// </summary>
    public class ImageKeyConverter : StringConverter
    {
        private string parentImageListProperty = "Parent";

        protected virtual bool IncludeNoneAsStandardValue
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///  this is the property to look at when there is no ImageList property
        ///  on the current object.  For example, in ToolBarButton - the ImageList is
        ///  on the ToolBarButton.Parent property.  In ToolStripItem, the ImageList is on
        ///  the ToolStripItem.Owner property.
        /// </summary>
        internal string ParentImageListProperty
        {
            get
            {
                return parentImageListProperty;
            }
            set
            {
                parentImageListProperty = value;
            }
        }
        /// <summary>
        ///  Gets a value indicating whether this converter can convert an object in the
        ///  given source type to a string using the specified context.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///  Converts the specified value object to a string object.
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return (string)value;
            }
            if (value is null)
            {
                return "";
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        ///  Converts the given object to another type.  The most common types to convert
        ///  are to and from a string object.  The default implementation will make a call
        ///  to ToString on the object if the object is valid and if the destination
        ///  type is string.  If this cannot convert to the desitnation type, this will
        ///  throw a NotSupportedException.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType is null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(string) && value != null && value is string && ((string)value).Length == 0)
            {
                return SR.toStringNone;
            }
            else if (destinationType == typeof(string) && (value is null))
            {
                return SR.toStringNone;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        ///  Retrieves a collection containing a set of standard values
        ///  for the data type this validator is designed for.  This
        ///  will return null if the data type does not support a
        ///  standard set of values.
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                object instance = context.Instance;
                PropertyDescriptor imageListProp = ImageListUtils.GetImageListProperty(context.PropertyDescriptor, ref instance);

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
                        //
                        PropertyDescriptor parentProp = props[ParentImageListProperty];
                        if (parentProp != null)
                        {
                            instance = parentProp.GetValue(instance);
                        }
                        else
                        {
                            // Stick a fork in us, we're done.
                            //
                            instance = null;
                        }
                    }
                }

                if (imageListProp != null)
                {
                    ImageList imageList = (ImageList)imageListProp.GetValue(instance);

                    if (imageList != null)
                    {
                        // Create array to contain standard values
                        //
                        object[] values;
                        int nImages = imageList.Images.Count;
                        if (IncludeNoneAsStandardValue)
                        {
                            values = new object[nImages + 1];
                            values[nImages] = string.Empty;
                        }
                        else
                        {
                            values = new object[nImages];
                        }

                        // Fill in the array
                        //
                        StringCollection imageKeys = imageList.Images.Keys;
                        for (int i = 0; i < imageKeys.Count; i++)
                        {
                            if ((imageKeys[i] != null) && (imageKeys[i].Length != 0))
                            {
                                values[i] = imageKeys[i];
                            }
                        }

                        return new StandardValuesCollection(values);
                    }
                }
            }

            if (IncludeNoneAsStandardValue)
            {
                return new StandardValuesCollection(new object[] { "" });
            }
            else
            {
                return new StandardValuesCollection(Array.Empty<object>());
            }
        }

        /// <summary>
        ///  Determines if the list of standard values returned from
        ///  GetStandardValues is an exclusive list.  If the list
        ///  is exclusive, then no other values are valid, such as
        ///  in an enum data type.  If the list is not exclusive,
        ///  then there are other valid values besides the list of
        ///  standard values GetStandardValues provides.
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///  Determines if this object supports a standard set of values
        ///  that can be picked from a list.
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
