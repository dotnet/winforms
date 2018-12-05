// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



 
namespace System.Windows.Forms {

    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Collections.Specialized;
    
    /// <include file='doc\TreeViewImageIndexConverter.uex' path='docs/doc[@for="TreeViewImageIndexConverter"]/*' />
    /// <devdoc>
    ///      TreeViewImageIndexConverter is a class that can be used to convert
    ///      image index values one data type to another.
    /// </devdoc>
    public class TreeViewImageIndexConverter : ImageIndexConverter {

        /// <include file='doc\TreeViewImageIndexConverter.uex' path='docs/doc[@for="TreeViewImageIndexConverter.IncludeNoneAsStandardValue"]/*' />
        protected override bool IncludeNoneAsStandardValue {
            get {
                return false;
            }
        }

        /// <include file='doc\TreeViewImageIndexConverter.uex' path='docs/doc[@for="TreeViewImageIndexConverter.ConvertFrom"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Converts the given value object to a 32-bit signed integer object.
        ///    </para>
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {

            string strValue = value as string;
            if (strValue != null) {
               
               if (String.Compare(strValue, SR.toStringDefault, true, culture) == 0) {
                   return -1;
               } 
               else if (String.Compare(strValue, SR.toStringNone, true, culture) == 0) {
                   return -2;
               }
            }
	    return base.ConvertFrom(context, culture, value);
        }


        /// <include file='doc\TreeViewImageIndexConverter.uex' path='docs/doc[@for="TreeViewImageIndexConverter.ConvertTo"]/*' />
        /// <devdoc>
        ///      Converts the given object to another type.  The most common types to convert
        ///      are to and from a string object.  The default implementation will make a call
        ///      to ToString on the object if the object is valid and if the destination
        ///      type is string.  If this cannot convert to the desitnation type, this will
        ///      throw a NotSupportedException.
        /// </devdoc>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == null) {
                throw new ArgumentNullException(nameof(destinationType));
            }

            
            if (destinationType == typeof(string) && value is int) {
                int intValue = (int)value;
                if (intValue == -1) {
                    return SR.toStringDefault;
                }
                else if (intValue == -2) {
                    return SR.toStringNone;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <include file='doc\ImageIndexConverter.uex' path='docs/doc[@for="ImageIndexConverter.GetStandardValues"]/*' />
        /// <devdoc>
        ///      Retrieves a collection containing a set of standard values
        ///      for the data type this validator is designed for.  This
        ///      will return null if the data type does not support a
        ///      standard set of values.
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
                        int nImages = imageList.Images.Count+2;
                        values = new object[nImages];
			values[nImages-2] = -1;
			values[nImages-1] = -2;
                        
                        // Fill in the array
                        //
                        for (int i = 0; i < nImages-2; i++) {
                            values[i] = i;
                        }
                        return new StandardValuesCollection(values);
                    }
                }
            }

            return new StandardValuesCollection(new object[] { -1, -2 });
        }
    }
} // Namespace system.windows.forms

