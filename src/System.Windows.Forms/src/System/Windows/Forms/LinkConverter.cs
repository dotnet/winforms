// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  A TypeConverter for LinkLabel.Link objects. Access this class through the TypeDescriptor.
    /// </summary>
    public class LinkConverter : TypeConverter
    {
        /// <summary>
        ///  Determines if this converter can convert an object in the given source
        ///  type to the native type of the converter.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///  Gets a value indicating whether this converter can convert an object to the given
        ///  destination type using the context.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string))
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
            if (value is string valueStr)
            {
                string text = valueStr.Trim();
                if (text.Length == 0)
                {
                    return null;
                }

                // Parse 2 integer values.
                if (culture == null)
                {
                    culture = CultureInfo.CurrentCulture;
                }

                char sep = culture.TextInfo.ListSeparator[0];
                string[] tokens = text.Split(new char[] { sep });
                int[] values = new int[tokens.Length];
                TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = (int)intConverter.ConvertFromString(context, culture, tokens[i]);
                }

                if (values.Length != 2)
                {
                    throw new ArgumentException(string.Format(SR.TextParseFailedFormat,
                                                                text,
                                                                "Start, Length"));
                }

                return new LinkLabel.Link(values[0], values[1]);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        ///  Converts the given object to another type. The most common types to convert
        ///  are to and from a string object. The default implementation will make a call
        ///  to ToString on the object if the object is valid and if the destination
        ///  type is string. If this cannot convert to the desitnation type, this will
        ///  throw a NotSupportedException.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is LinkLabel.Link link)
            {
                if (destinationType == typeof(string))
                {
                    if (culture == null)
                    {
                        culture = CultureInfo.CurrentCulture;
                    }

                    string sep = culture.TextInfo.ListSeparator + " ";
                    TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                    string[] args = new string[]
                    {
                        intConverter.ConvertToString(context, culture, link.Start),
                        intConverter.ConvertToString(context, culture, link.Length)
                    };
                    return string.Join(sep, args);
                }

                if (destinationType == typeof(InstanceDescriptor))
                {
                    if (link.LinkData == null)
                    {
                        return new InstanceDescriptor(
                            typeof(LinkLabel.Link).GetConstructor(new Type[] { typeof(int), typeof(int) }),
                            new object[] { link.Start, link.Length },
                            true
                        );
                    }

                    return new InstanceDescriptor(
                        typeof(LinkLabel.Link).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(object) }),
                        new object[] { link.Start, link.Length, link.LinkData },
                        true
                    );
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

