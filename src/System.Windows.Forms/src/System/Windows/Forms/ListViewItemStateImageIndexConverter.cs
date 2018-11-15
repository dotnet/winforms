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

    /// <include file='doc\ListViewItemStateImageIndexConverter.uex' path='docs/doc[@for="ListViewItemStateImageIndexConverter"]/*' />
    /// <devdoc>
    ///      ListViewItemStateImageIndexConverter is a class that can be used to convert
    ///      image index values one data type to another.
    /// </devdoc>
    public class ListViewItemStateImageIndexConverter : ImageIndexConverter {

        /// <include file='doc\ListViewItemStateImageIndexConverter.uex' path='docs/doc[@for="ListViewItemStateImageIndexConverter.IncludeNoneAsStandardValue"]/*' />
        protected override bool IncludeNoneAsStandardValue {
            get {
                return false;
            }
        }                                


        /// <include file='doc\ListViewItemStateImageIndexConverter.uex' path='docs/doc[@for="ListViewItemStateImageIndexConverter.GetStandardValues"]/*' />
        /// <devdoc>
        ///      Retrieves a collection containing a set of standard values
        ///      for the data type this validator is designed for.  This
        ///      will return null if the data type does not support a
        ///      standard set of values.
        /// </devdoc>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            if (context != null && context.Instance != null) {
                object instance = context.Instance;

                ImageList imageList = null;

                PropertyDescriptorCollection listViewItemProps = TypeDescriptor.GetProperties(instance);  
                PropertyDescriptor listViewProp = listViewItemProps["ListView"];

                 if (listViewProp != null) {
                        // Grab the ListView property off of the TreeNode.
                        object listViewInstance = listViewProp.GetValue(instance);
                        
                        if (listViewInstance != null) {
                            // Get the ImageList property from the ListView and set it to be the currentImageList.
                            PropertyDescriptorCollection listViewProps = TypeDescriptor.GetProperties(listViewInstance);
                            PropertyDescriptor listViewImageListProperty = listViewProps["StateImageList"];
                            if (listViewImageListProperty != null) {
                                imageList = (ImageList)listViewImageListProperty.GetValue(listViewInstance);
                            }
                        }
                 }    
                
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
            if (IncludeNoneAsStandardValue) {
                return new StandardValuesCollection(new object[] {-1});
            }
            else {
                return new StandardValuesCollection(new object[0]);
            }
            
        }

        
    }
}