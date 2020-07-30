// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        /// <summary>
        ///  StateConverter is a class that can be used to convert State from one data type to another.
        ///  Access this class through the TypeDescriptor.
        /// </summary>
        public class StateConverter : TypeConverter
        {
            /// <summary>
            ///  Gets a value indicating whether this converter can
            ///  convert an object in the given source type to the native type of the converter
            ///  using the context.
            /// </summary>
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(byte[]))
                {
                    return true;
                }

                return base.CanConvertFrom(context, sourceType);
            }

            /// <summary>
            ///  Gets a value indicating whether this converter can
            ///  convert an object to the given destination type using the context.
            /// </summary>
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(byte[]))
                {
                    return true;
                }

                return base.CanConvertTo(context, destinationType);
            }

            /// <summary>
            ///  Converts the given object to the converter's native type.
            /// </summary>
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is byte[])
                {
                    MemoryStream ms = new MemoryStream((byte[])value);
                    return new State(ms);
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

                if (destinationType == typeof(byte[]))
                {
                    if (value != null)
                    {
                        MemoryStream ms = new MemoryStream();
                        State state = (State)value;
                        state.Save(ms);
                        ms.Close();
                        return ms.ToArray();
                    }
                    else
                    {
                        return Array.Empty<byte>();
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
