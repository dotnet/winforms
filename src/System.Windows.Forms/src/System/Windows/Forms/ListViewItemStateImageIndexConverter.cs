// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;


namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides a type converter to convert state image index values from one data type to another.
    /// </summary>
    public class ListViewItemStateImageIndexConverter : ImageIndexConverter
    {
        /// <summary>
        ///  Gets a value that indicates whether a <see cref="none" /> or <see langword="null" /> value
        ///  is valid in the <see cref="TypeConverter.StandardValuesCollection" /> collection.
        /// </summary>
        /// <value>
        ///  Always returns <see langword="false" /> to indicate that a <see cref="none" /> or
        ///  <see langword="null" /> value isn't valid in the standard values collection.
        /// </value>
        protected override bool IncludeNoneAsStandardValue
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///  Retrieves a collection containing a set of standard values for the data type this validator is designed for.
        /// </summary>
        /// <param name="context">
        ///  An object that provides a format context, which can be used to extract additional
        ///  information about the environment this type converter is being invoked from. 
        ///  This parameter or its properties can be <see langword="null" />.
        /// </param>
        /// <returns>
        ///  A collection that holds a standard set of valid index values.If no image list is found,
        ///  this collection contains a single object with a value of -1. This method returns<see langword="null" />
        ///  if the data type doesn't support a standard set of values.
        /// </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                object instance = context.Instance;

                ImageList imageList = null;

                PropertyDescriptorCollection listViewItemProps = TypeDescriptor.GetProperties(instance);
                PropertyDescriptor listViewProp = listViewItemProps["ListView"];

                if (listViewProp != null)
                {
                    // Grab the ListView property off of the TreeNode.
                    object listViewInstance = listViewProp.GetValue(instance);

                    if (listViewInstance != null)
                    {
                        // Get the ImageList property from the ListView and set it to be the currentImageList.
                        PropertyDescriptorCollection listViewProps = TypeDescriptor.GetProperties(listViewInstance);
                        PropertyDescriptor listViewImageListProperty = listViewProps["StateImageList"];
                        if (listViewImageListProperty != null)
                        {
                            imageList = (ImageList)listViewImageListProperty.GetValue(listViewInstance);
                        }
                    }
                }

                if (imageList != null)
                {

                    // Create array to contain standard values
                    //
                    object[] values;
                    int nImages = imageList.Images.Count;
                    if (IncludeNoneAsStandardValue)
                    {
                        values = new object[nImages + 1];
                        values[nImages] = -1;
                    }
                    else
                    {
                        values = new object[nImages];
                    }

                    // Fill in the array
                    //
                    for (int i = 0; i < nImages; i++)
                    {
                        values[i] = i;
                    }

                    return new StandardValuesCollection(values);
                }

            }
            if (IncludeNoneAsStandardValue)
            {
                return new StandardValuesCollection(new object[] { -1 });
            }
            else
            {
                return new StandardValuesCollection(Array.Empty<object>());
            }

        }

    }
}
