// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Windows.Forms
{
    [TypeConverter(typeof(LinkAreaConverter))]
    [Serializable] // This type is participating in resx serialization scenarios.
    public struct LinkArea
    {
#pragma warning disable IDE1006
        private int start; // Do NOT rename (binary serialization).
        private int length; // Do NOT rename (binary serialization).
#pragma warning restore IDE1006

        public LinkArea(int start, int length)
        {
            this.start = start;
            this.length = length;
        }
        public int Start
        {
            get => start;
            set => start = value;
        }

        public int Length
        {
            get => length;
            set => length = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEmpty => length == start && start == 0;

        public override bool Equals(object o)
        {
            if (!(o is LinkArea a))
            {
                return false;
            }

            return this == a;
        }

        public override string ToString() => $"{{Start={Start}, Length={Length}}}";

        public static bool operator ==(LinkArea linkArea1, LinkArea linkArea2)
        {
            return linkArea1.start == linkArea2.start && linkArea1.length == linkArea2.length;
        }

        public static bool operator !=(LinkArea linkArea1, LinkArea linkArea2)
        {
            return !(linkArea1 == linkArea2);
        }

        public override int GetHashCode() => HashCode.Combine(start, length);

        /// <summary>
        ///  LinkAreaConverter is a class that can be used to convert LinkArea from one data type
        ///  to another. Access this class through the TypeDescriptor.
        /// </summary>
        public class LinkAreaConverter : TypeConverter
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
            ///  Gets a value indicating whether this converter can convert an object to the
            ///  given destination type using the context.
            /// </summary>
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(InstanceDescriptor))
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
                    if (culture is null)
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
                                                                    "start, length"));
                    }

                    return new LinkArea(values[0], values[1]);
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
                if (value is LinkArea pt)
                {
                    if (destinationType == typeof(string))
                    {
                        if (culture is null)
                        {
                            culture = CultureInfo.CurrentCulture;
                        }

                        string sep = culture.TextInfo.ListSeparator + " ";
                        TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                        string[] args = new string[]
                        {
                            intConverter.ConvertToString(context, culture, pt.Start),
                            intConverter.ConvertToString(context, culture, pt.Length)
                        };
                        return string.Join(sep, args);
                    }
                    else if (destinationType == typeof(InstanceDescriptor))
                    {
                        return new InstanceDescriptor(
                            typeof(LinkArea).GetConstructor(new Type[] { typeof(int), typeof(int) }),
                            new object[] { pt.Start, pt.Length }
                        );
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            /// <summary>
            ///  Creates an instance of this type given a set of property values
            ///  for the object. This is useful for objects that are immutable, but still
            ///  want to provide changable properties.
            /// </summary>
            public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
            {
                if (propertyValues is null)
                {
                    throw new ArgumentNullException(nameof(propertyValues));
                }

                try
                {
                    return new LinkArea((int)propertyValues[nameof(LinkArea.Start)],
                                    (int)propertyValues[nameof(LinkArea.Length)]);
                }
                catch (InvalidCastException invalidCast)
                {
                    throw new ArgumentException(SR.PropertyValueInvalidEntry, nameof(propertyValues), invalidCast);
                }
                catch (NullReferenceException nullRef)
                {
                    throw new ArgumentException(SR.PropertyValueInvalidEntry, nameof(propertyValues), nullRef);
                }
            }

            /// <summary>
            ///  Determines if changing a value on this object should require a call to
            ///  CreateInstance to create a new value.
            /// </summary>
            public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) => true;

            /// <summary>
            ///  Retrieves the set of properties for this type. By default, a type has
            ///  does not return any properties. An easy implementation of this method
            ///  can just call TypeDescriptor.GetProperties for the correct data type.
            /// </summary>
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(LinkArea), attributes);
                return props.Sort(new string[] { nameof(LinkArea.Start), nameof(LinkArea.Length) });
            }

            /// <summary>
            ///  Determines if this object supports properties. By default, this is false.
            /// </summary>
            public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;
        }
    }
}
