// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

/// <summary>
///  ImageIndexConverter is a class that can be used to convert
///  image index values one data type to another.
/// </summary>
public class TreeViewImageKeyConverter : ImageKeyConverter
{
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

        if (destinationType == typeof(string) && (value is null))
        {
            return SR.toStringDefault;
        }
        else
        {
            if (value is string strValue && (strValue.Length == 0))
            {
                return SR.toStringDefault;
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
