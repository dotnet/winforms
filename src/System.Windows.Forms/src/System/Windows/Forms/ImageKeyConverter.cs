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
    using System.Collections.Specialized;

    /// <include file='doc\ImageKeyConverter.uex' path='docs/doc[@for="ImageKeyConverter"]/*' />
    /// <devdoc>
    /// ImageIndexConverter is a class that can be used to convert
    /// image index values one data type to another.
    /// </devdoc>
    public class ImageKeyConverter : StringConverter {

        private string parentImageListProperty  = "Parent";
   
        /// <include file='doc\ImageKeyConverter.uex' path='docs/doc[@for="ImageKeyConverter.IncludeNoneAsStandardValue"]/*' />
        protected virtual bool IncludeNoneAsStandardValue {
            get {
                return true;
            }
        }     

        /// <devdoc> 
        /// this is the property to look at when there is no ImageList property
        /// on the current object.  For example, in ToolBarButton - the ImageList is 
        /// on the ToolBarButton.Parent property.  In WinBarItem, the ImageList is on 
        /// the WinBarItem.Owner property.
        /// </devdoc>
        internal string ParentImageListProperty {
            get {
                return parentImageListProperty;
            }
            set {
                parentImageListProperty = value;
            }
        }
        /// <include file='doc\ImageKeyConverter.uex' path='docs/doc[@for="ImageKeyConverter.CanConvertFrom"]/*' />
        /// <devdoc>
        /// <para>Gets a value indicating whether this converter can convert an object in the
        /// given source type to a string using the specified context.</para>
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof(string)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <include file='doc\ImageKeyConverter.uex' path='docs/doc[@for="ImageKeyConverter.ConvertFrom"]/*' />
        /// <devdoc>
        /// <para>Converts the specified value object to a string object.</para>
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value is string) {
                return (string)value;
            }
            if (value == null) {
                return "";
            }
            return base.ConvertFrom(context, culture, value);
        }


        
        /// <include file='doc\ImageKeyConverter.uex' path='docs/doc[@for="ImageKeyConverter.ConvertTo"]/*' />
        /// <devdoc>
        /// Converts the given object to another type.  The most common types to convert
        /// are to and from a string object.  The default implementation will make a call
        /// to ToString on the object if the object is valid and if the destination
        /// type is string.  If this cannot convert to the desitnation type, this will
        /// throw a NotSupportedException.
        /// </devdoc>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == null) {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(string) && value != null && value is string && ((string)value).Length == 0) {
                return SR.toStringNone;
            }
            else if (destinationType == typeof(string) && (value == null)) {
                return SR.toStringNone;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

     
        /// <include file='doc\ImageKeyConverter.uex' path='docs/doc[@for="ImageKeyConverter.GetStandardValues"]/*' />
        /// <devdoc>
        /// Retrieves a collection containing a set of standard values
        /// for the data type this validator is designed for.  This
        /// will return null if the data type does not support a
        /// standard set of values.
        /// </devdoc>
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
                            values[nImages] = "";
                        }
                        else {
                            values = new object[nImages];
                        }
                        
                        
                        // Fill in the array
                        //
                        StringCollection imageKeys = imageList.Images.Keys;
                        for (int i = 0; i < imageKeys.Count; i++) {
                            if ((imageKeys[i] != null) && (imageKeys[i].Length != 0))
                                values[i] = imageKeys[i];
                        }
                        
                        return new StandardValuesCollection(values);
                    }
                }
            }

            if (IncludeNoneAsStandardValue) {
                return new StandardValuesCollection(new object[] {""});
            }
            else {
                return new StandardValuesCollection(new object[0]);
            }
        }
    
        /// <include file='doc\ImageKeyConverter.uex' path='docs/doc[@for="ImageKeyConverter.GetStandardValuesExclusive"]/*' />
        /// <devdoc>
        /// Determines if the list of standard values returned from
        /// GetStandardValues is an exclusive list.  If the list
        /// is exclusive, then no other values are valid, such as
        /// in an enum data type.  If the list is not exclusive,
        /// then there are other valid values besides the list of
        /// standard values GetStandardValues provides.
        /// </devdoc>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
            return true;
        }

        /// <include file='doc\ImageKeyConverter.uex' path='docs/doc[@for="ImageKeyConverter.GetStandardValuesSupported"]/*' />
        /// <devdoc>
        /// Determines if this object supports a standard set of values
        /// that can be picked from a list.
        /// </devdoc>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
             return true;
        }
    }
}

