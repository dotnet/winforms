// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using Microsoft.Win32;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    ///      ImageIndexConverter is a class that can be used to convert
    ///      image index values one data type to another.
    /// </summary>
    public class ImageIndexConverter : Int32Converter {

        private string parentImageListProperty  = "Parent";
        
        protected virtual bool IncludeNoneAsStandardValue {
            get {
                return true;
            }
        }                                

        /// <summary> 
        /// this is the property to look at when there is no ImageList property
        /// on the current object.  For example, in ToolBarButton - the ImageList is 
        /// on the ToolBarButton.Parent property.  In ToolStripItem, the ImageList is on 
        /// the ToolStripItem.Owner property.
        /// </summary>
        internal string ParentImageListProperty {
            get {
                return parentImageListProperty;
            }
            set {
                parentImageListProperty = value;
            }
        }
        
        /// <summary>
        ///    <para>
        ///       Converts the given value object to a 32-bit signed integer object.
        ///    </para>
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            string stringValue = value as string;
            if (stringValue != null && string.Compare(stringValue, SR.toStringNone, true, culture) == 0) {
                return -1;
            }

            return base.ConvertFrom(context, culture, value);
        }


        /// <summary>
        ///      Converts the given object to another type.  The most common types to convert
        ///      are to and from a string object.  The default implementation will make a call
        ///      to ToString on the object if the object is valid and if the destination
        ///      type is string.  If this cannot convert to the desitnation type, this will
        ///      throw a NotSupportedException.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == null) {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(string) && value is int && ((int)value) == -1) {
                return SR.toStringNone;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        ///      Retrieves a collection containing a set of standard values
        ///      for the data type this validator is designed for.  This
        ///      will return null if the data type does not support a
        ///      standard set of values.
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            if (context != null && context.Instance != null) {
                object instance = context.Instance;
                
                PropertyDescriptor imageListProp = ImageListUtils.GetImageListProperty(context.PropertyDescriptor, ref instance);

                while (instance != null && imageListProp == null) {
                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(instance);

                    foreach (PropertyDescriptor prop in props) {
                        if (typeof(ImageList).IsAssignableFrom(prop.PropertyType)) {
                            imageListProp = prop;
                            break;
                        }
                    }

                    if (imageListProp == null) {

                        // We didn't find the image list in this component.  See if the 
                        // component has a "parent" property.  If so, walk the tree...
                        //
                        PropertyDescriptor parentProp = props[ParentImageListProperty];
                        if (parentProp != null) {
                            instance = parentProp.GetValue(instance);
                        }
                        else {
                            // Stick a fork in us, we're done.
                            //
                            instance = null;
                        }
                    }
                }

                if (imageListProp != null) {
                    ImageList imageList = (ImageList)imageListProp.GetValue(instance);

                    if (imageList != null) {
                        
                        // Create array to contain standard values
                        //
                        object[] values;
                        int nImages = imageList.Images.Count;
                        if (IncludeNoneAsStandardValue) {
                            values = new object[nImages + 1];
                            values[nImages] = -1;
                        }
                        else {
                            values = new object[nImages];
                        }
                        
                        
                        // Fill in the array
                        //
                        for (int i = 0; i < nImages; i++) {
                            values[i] = i;
                        }
                        
                        return new StandardValuesCollection(values);
                    }
                }
            }

            if (IncludeNoneAsStandardValue) {
                return new StandardValuesCollection(new object[] {-1});
            }
            else {
                return new StandardValuesCollection(new object[0]);
            }
        }

        /// <summary>
        ///      Determines if the list of standard values returned from
        ///      GetStandardValues is an exclusive list.  If the list
        ///      is exclusive, then no other values are valid, such as
        ///      in an enum data type.  If the list is not exclusive,
        ///      then there are other valid values besides the list of
        ///      standard values GetStandardValues provides.
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
            return false;
        }

        /// <summary>
        ///      Determines if this object supports a standard set of values
        ///      that can be picked from a list.
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }
    }
}

