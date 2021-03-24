// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyDescriptorGridEntry
    {
        /// <summary>
        ///  The exception converter is a type converter that displays an exception to the user.
        /// </summary>
        private class ExceptionConverter : TypeConverter
        {
            /// <summary>
            ///  Converts the given object to another type.  The most common types to convert
            ///  are to and from a string object.  The default implementation will make a call
            ///  to ToString on the object if the object is valid and if the destination
            ///  type is string.  If this cannot convert to the destination type, this will
            ///  throw a NotSupportedException.
            /// </summary>
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
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
}
