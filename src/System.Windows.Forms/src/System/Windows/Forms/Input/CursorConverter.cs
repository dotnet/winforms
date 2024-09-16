// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms;

/// <summary>
///  CursorConverter is a class that can be used to convert
///  colors from one data type to another. Access this
///  class through the TypeDescriptor.
/// </summary>
public class CursorConverter : TypeConverter
{
    private StandardValuesCollection? _values;

    /// <summary>
    ///  Determines if this converter can convert an object in the given source
    ///  type to the native type of the converter.
    /// </summary>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
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
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
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
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string s)
        {
            string text = s.Trim();

            PropertyInfo[] props = GetProperties();
            foreach (var prop in props)
            {
                if (string.Equals(prop.Name, text, StringComparison.OrdinalIgnoreCase))
                {
                    return prop.GetValue(null, null);
                }
            }
        }

        if (value is byte[] bytes)
        {
            using MemoryStream ms = new(bytes);
            return new Cursor(ms);
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
        if (value is Cursor cursor)
        {
            if (destinationType == typeof(string))
            {
                if (cursor.CursorsProperty is string propertyName)
                {
                    return propertyName;
                }
                else if (cursor.Handle == Cursors.Arrow.Handle)
                {
                    // Arrow and Default cursors share the same HCURSOR.
                    // Always return "Default" in this case.
                    return nameof(Cursors.Default);
                }

                // We have a cursor that only has handle information. This can happen when the cursor was read via PInvoke.
                // Try to find an exact instance match to a known cursor from Cursors properties using HCURSOR equality (==).
                PropertyInfo[] props = GetProperties();
                for (int i = 0; i < props.Length; i++)
                {
                    PropertyInfo prop = props[i];
                    Cursor? knownCursor = (Cursor?)prop.GetValue(obj: null, index: null);
                    if (knownCursor == cursor)
                    {
                        return prop.Name;
                    }
                }

                // We throw here because we cannot meaningfully convert a custom
                // cursor into a string. In fact, the ResXResourceWriter will use
                // this exception to indicate to itself that this object should
                // be serialized through ISerializable instead of a string.

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
        else if (destinationType == typeof(byte[]) && value is null)
        {
            return Array.Empty<byte>();
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    ///  Retrieves the properties for the available cursors.
    /// </summary>
    private static PropertyInfo[] GetProperties()
    {
        return typeof(Cursors).GetProperties(BindingFlags.Static | BindingFlags.Public);
    }

    /// <summary>
    ///  Retrieves a collection containing a set of standard values
    ///  for the data type this validator is designed for. This
    ///  will return null if the data type does not support a
    ///  standard set of values.
    /// </summary>
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
    {
        if (_values is null)
        {
            List<object> list = [];
            PropertyInfo[] props = GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo prop = props[i];
                object[]? tempIndex = null;
                Debug.Assert(prop.GetValue(null, tempIndex) is not null, $"Property {prop.Name} returned NULL");
                if (prop.GetValue(null, tempIndex) is object item)
                {
                    list.Add(item);
                }
            }

            _values = new StandardValuesCollection(list);
        }

        return _values;
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
