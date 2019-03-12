// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Windows.Forms
{
    public class PaddingConverter : TypeConverter
    {
        /// <devdoc>
        /// Determines if this converter can convert an object in the given source type to
        /// the native type of the converter.
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <devdoc>
        /// Converts the given object to the converter's native type.
        /// </devdoc>
        [SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes", Justification = "ConvertFromString returns an object")]
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string valueStr)
            {
                valueStr = valueStr.Trim();
                if (valueStr.Length == 0)
                {
                    return null;
                }

                // Parse 4 integer values.
                if (culture == null)
                {
                    culture = CultureInfo.CurrentCulture;
                }

                char sep = culture.TextInfo.ListSeparator[0];
                string[] tokens = valueStr.Split(new char[] { sep });
                int[] values = new int[tokens.Length];
                TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                for (int i = 0; i < values.Length; i++)
                {
                    // Note: ConvertFromString will raise exception if value cannot be converted.
                    values[i] = (int)intConverter.ConvertFromString(context, culture, tokens[i]);
                }

                if (values.Length != 4)
                {
                    throw new ArgumentException(string.Format(SR.TextParseFailedFormat,
                                            nameof(value),
                                            valueStr,
                                            "left, top, right, bottom"));
                }

                return new Padding(values[0], values[1], values[2], values[3]);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is Padding padding)
            {
                if (destinationType == typeof(string))
                {
                    if (culture == null)
                    {
                        culture = CultureInfo.CurrentCulture;
                    }

                    string sep = culture.TextInfo.ListSeparator + " ";
                    TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                    // Note: ConvertToString will raise exception if value cannot be converted.
                    string[] args = new string[]
                    {
                        intConverter.ConvertToString(context, culture, padding.Left),
                        intConverter.ConvertToString(context, culture, padding.Top),
                        intConverter.ConvertToString(context, culture, padding.Right),
                        intConverter.ConvertToString(context, culture, padding.Bottom)
                    };
                    return string.Join(sep, args);
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    if (padding.ShouldSerializeAll())
                    {
                        return new InstanceDescriptor(
                            typeof(Padding).GetConstructor(new Type[] { typeof(int) }),
                            new object[] { padding.All }
                        );
                    }
                    else
                    {
                        return new InstanceDescriptor(
                            typeof(Padding).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
                            new object[] { padding.Left, padding.Top, padding.Right, padding.Bottom }
                        );
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (propertyValues == null)
            {
                throw new ArgumentNullException(nameof(propertyValues));
            }

            Padding original = (Padding)context.PropertyDescriptor.GetValue(context.Instance);

            int all = (int)propertyValues["All"];
            if (original.All != all)
            {
                return new Padding(all);
            }
            else
            {
                return new Padding(
                    (int)propertyValues["Left"],
                    (int)propertyValues["Top"],
                    (int)propertyValues["Right"],
                    (int)propertyValues["Bottom"]
                );
            }
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) => true;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(Padding), attributes);
            return props.Sort(new string[] { "All", "Left", "Top", "Right", "Bottom" });
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;
    }
}
