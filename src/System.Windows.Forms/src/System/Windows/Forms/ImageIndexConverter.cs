// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

/// <summary>
///  ImageIndexConverter is a class that can be used to convert
///  image index values one data type to another.
/// </summary>
public class ImageIndexConverter : Int32Converter
{
    // Feature switch, when set to false, ImageIndexConverter is not supported in trimmed applications.
    [FeatureSwitchDefinition("System.Windows.Forms.ImageIndexConverter.IsSupported")]
#pragma warning disable IDE0075 // Simplify conditional expression - the simpler expression is hard to read
    private static bool IsSupported { get; } =
        AppContext.TryGetSwitch("System.Windows.Forms.ImageIndexConverter.IsSupported", out bool isSupported)
            ? isSupported
            : true;
#pragma warning restore IDE0075

    /// <summary>
    ///  Gets a value that indicates whether a <see langword="null" /> value is valid in
    ///  the <see cref="TypeConverter.StandardValuesCollection" /> collection.
    /// </summary>
    /// <value>
    ///  Always returns <see langword="true" /> to indicate that a <see langword="null" /> value
    ///  isn't valid in the standard values collection.
    /// </value>
    /// <remarks>
    ///  <para>
    ///   <c>none</c> is the display name that is used when standard values are presented
    ///   in the control UI and corresponds to a <see langword="null"/> value.
    ///  </para>
    /// </remarks>
    protected virtual bool IncludeNoneAsStandardValue => true;

    /// <summary>
    ///  this is the property to look at when there is no ImageList property
    ///  on the current object. For example, in ToolBarButton - the ImageList is
    ///  on the ToolBarButton.Parent property. In ToolStripItem, the ImageList is on
    ///  the ToolStripItem.Owner property.
    /// </summary>
    internal string ParentImageListProperty { get; set; } = "Parent";

    /// <summary>
    ///  Converts the given value object to a 32-bit signed integer object.
    /// </summary>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue && string.Compare(stringValue, SR.toStringNone, true, culture) == 0)
        {
            return ImageList.Indexer.DefaultIndex;
        }

        return base.ConvertFrom(context, culture, value);
    }

    /// <summary>
    ///  Converts the given object to another type. The most common types to convert
    ///  are to and from a string object. The default implementation will make a call
    ///  to ToString on the object if the object is valid and if the destination
    ///  type is string. If this cannot convert to the destination type, this will
    ///  throw a NotSupportedException.
    /// </summary>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (destinationType == typeof(string) && value is int index && index == ImageList.Indexer.DefaultIndex)
        {
            return SR.toStringNone;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    ///  Retrieves a collection containing a set of standard values for the data type this validator is designed for.
    /// </summary>
    /// <param name="context">
    ///  An <see cref="ITypeDescriptorContext" /> that provides a format context, which can be used to extract
    ///  additional information about the environment this type converter is being invoked from.
    ///  This parameter or properties of this parameter can be <see langword="null" />.
    /// </param>
    /// <returns>
    ///  A collection that holds a standard set of valid index values.
    ///  If no image list is found, this collection will contain a single object with a value of -1.
    ///  This returns <see langword="null" /> if the data type doesn't support a standard set of values.
    /// </returns>
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
    {
        if (!IsSupported)
        {
            throw new NotSupportedException(string.Format(SR.ControlNotSupportedInTrimming, nameof(ImageIndexConverter)));
        }

        if (context is not null && context.Instance is not null)
        {
            object? instance = context.Instance;

            PropertyDescriptor? imageListProp = ImageListUtils.GetImageListProperty(context.PropertyDescriptor, ref instance);

            while (instance is not null && imageListProp is null)
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
                    // We didn't find the image list in this component. See if the
                    // component has a "parent" property. If so, walk the tree...
                    PropertyDescriptor? parentProp = props[ParentImageListProperty];
                    if (parentProp is not null)
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

            if (imageListProp is not null)
            {
                ImageList? imageList = (ImageList?)imageListProp.GetValue(instance);

                if (imageList is not null)
                {
                    // Create array to contain standard values
                    object[] values;
                    int nImages = imageList.Images.Count;
                    if (IncludeNoneAsStandardValue)
                    {
                        values = new object[nImages + 1];
                        values[nImages] = ImageList.Indexer.DefaultIndex;
                    }
                    else
                    {
                        values = new object[nImages];
                    }

                    // Fill in the array
                    for (int i = 0; i < nImages; i++)
                    {
                        values[i] = i;
                    }

                    return new StandardValuesCollection(values);
                }
            }
        }

        if (IncludeNoneAsStandardValue)
        {
            return new StandardValuesCollection(new object[] { ImageList.Indexer.DefaultIndex });
        }
        else
        {
            return new StandardValuesCollection(Array.Empty<object>());
        }
    }

    /// <summary>
    ///  Determines if the list of standard values returned from
    ///  GetStandardValues is an exclusive list. If the list
    ///  is exclusive, then no other values are valid, such as
    ///  in an enum data type. If the list is not exclusive,
    ///  then there are other valid values besides the list of
    ///  standard values GetStandardValues provides.
    /// </summary>
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context)
    {
        return false;
    }

    /// <summary>
    ///  Determines if this object supports a standard set of values
    ///  that can be picked from a list.
    /// </summary>
    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
    {
        return true;
    }
}
