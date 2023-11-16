// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyDescriptorGridEntry
{
    /// <summary>
    ///  The exception converter is a type converter that displays an exception to the user.
    /// </summary>
    private class ExceptionConverter : TypeConverter
    {
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is Exception ex)
                {
                    if (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }

                    return ex.Message;
                }

                return null;
            }

            throw GetConvertToException(value, destinationType);
        }
    }
}
