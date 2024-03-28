// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    /// <summary>
    ///  StateConverter is a class that can be used to convert State from one data type to another.
    ///  Access this class through the TypeDescriptor.
    /// </summary>
    public class StateConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(byte[]) || base.CanConvertFrom(context, sourceType);

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
            => destinationType == typeof(byte[]) || base.CanConvertTo(context, destinationType);

        /// <inheritdoc/>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is byte[] valueAsBytes)
            {
                using MemoryStream ms = new(valueAsBytes);
                return new State(ms);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc/>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            ArgumentNullException.ThrowIfNull(destinationType);

            if (destinationType == typeof(byte[]))
            {
                if (value is null)
                {
                    return Array.Empty<byte>();
                }

                if (value is not State state)
                {
                    throw GetConvertToException(value, destinationType);
                }

                using MemoryStream memoryStream = new();
                state.Save(memoryStream);
                memoryStream.Close();
                return memoryStream.ToArray();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
