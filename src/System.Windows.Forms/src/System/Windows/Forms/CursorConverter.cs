// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace System.Windows.Forms
{
    /// <summary>
    ///  CursorConverter is a class that can be used to convert
    ///  colors from one data type to another.  Access this
    ///  class through the TypeDescriptor.
    /// </summary>
    public class CursorConverter : TypeConverter
    {
        private StandardValuesCollection values;

        /// <summary>
        ///  Determines if this converter can convert an object in the given source
        ///  type to the native type of the converter.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(byte[]))
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
            if (destinationType == typeof(InstanceDescriptor) || destinationType == typeof(byte[]))
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
            if (value is string)
            {
                string text = ((string)value).Trim();

                PropertyInfo[] props = GetProperties();
                for (int i = 0; i < props.Length; i++)
                {
                    PropertyInfo prop = props[i];
                    if (string.Equals(prop.Name, text, StringComparison.OrdinalIgnoreCase))
                    {
                        object[] tempIndex = null;
                        return prop.GetValue(null, tempIndex);
                    }
                }
            }

            if (value is byte[])
            {
                MemoryStream ms = new MemoryStream((byte[])value);
                return new Cursor(ms);
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
            if (value is Cursor cursor)
            {
                if (destinationType == typeof(string))
                {
                    PropertyInfo[] props = GetProperties();
                    int bestMatch = -1;

                    for (int i = 0; i < props.Length; i++)
                    {
                        PropertyInfo prop = props[i];
                        object[] tempIndex = null;
                        Cursor c = (Cursor)prop.GetValue(null, tempIndex);
                        if (c == cursor)
                        {
                            if (ReferenceEquals(c, value))
                            {
                                return prop.Name;
                            }
                            else
                            {
                                bestMatch = i;
                            }
                        }
                    }

                    if (bestMatch != -1)
                    {
                        return props[bestMatch].Name;
                    }

                    // We throw here because we cannot meaningfully convert a custom
                    // cursor into a string. In fact, the ResXResourceWriter will use
                    // this exception to indicate to itself that this object should
                    // be serialized through ISeriazable instead of a string.
                    //
                    throw new FormatException(SR.CursorCannotCovertToString);
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    PropertyInfo[] props = GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.GetValue(null, null) == value)
                        {
                            return new InstanceDescriptor(prop, null);
                        }
                    }
                }
                else if (destinationType == typeof(byte[]))
                {
                    return cursor.GetData();
                }
            }
            else if (destinationType == typeof(byte[]) && value == null)
            {
                return Array.Empty<byte>();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        ///  Retrieves the properties for the available cursors.
        /// </summary>
        private PropertyInfo[] GetProperties()
        {
            return typeof(Cursors).GetProperties(BindingFlags.Static | BindingFlags.Public);
        }

        /// <summary>
        ///  Retrieves a collection containing a set of standard values
        ///  for the data type this validator is designed for.  This
        ///  will return null if the data type does not support a
        ///  standard set of values.
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (values == null)
            {
                ArrayList list = new ArrayList();
                PropertyInfo[] props = GetProperties();
                for (int i = 0; i < props.Length; i++)
                {
                    PropertyInfo prop = props[i];
                    object[] tempIndex = null;
                    Debug.Assert(prop.GetValue(null, tempIndex) != null, "Property " + prop.Name + " returned NULL");
                    list.Add(prop.GetValue(null, tempIndex));
                }

                values = new StandardValuesCollection(list.ToArray());
            }

            return values;
        }

        /// <summary>
        ///  Determines if this object supports a standard set of values
        ///  that can be picked from a list.
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}

