// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms;

/// <summary>
///  SelectionRangeConverter is a class that can be used to convert
///  SelectionRange objects from one data type to another. Access this
///  class through the TypeDescriptor.
/// </summary>
public class SelectionRangeConverter : TypeConverter
{
    /// <summary>
    ///  Determines if this converter can convert an object in the given source
    ///  type to the native type of the converter.
    /// </summary>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(string) || sourceType == typeof(DateTime))
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
        if (destinationType == typeof(InstanceDescriptor) || destinationType == typeof(DateTime))
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
        if (value is string valueAsString)
        {
            ReadOnlySpan<char> text = valueAsString.AsSpan().Trim();
            if (text.IsEmpty)
            {
                return new SelectionRange(DateTime.Now.Date, DateTime.Now.Date);
            }

            // Separate the string into the two dates, and parse each one
            culture ??= CultureInfo.CurrentCulture;
            Span<DateTime> values = stackalloc DateTime[2];

            char separator = culture.TextInfo.ListSeparator[0];

            if (TypeConverterHelper.TryParseAsSpan(context, culture, text, values))
            {
                return new SelectionRange(values[0], values[1]);
            }
            else
            {
                throw new ArgumentException(string.Format(SR.TextParseFailedFormat,
                                                          valueAsString,
                                                          $"Start{separator} End"));
            }
        }
        else if (value is DateTime dt)
        {
            return new SelectionRange(dt, dt);
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
        ArgumentNullException.ThrowIfNull(destinationType);

        if (value is SelectionRange range)
        {
            if (destinationType == typeof(string))
            {
                culture ??= CultureInfo.CurrentCulture;

                string sep = culture.TextInfo.ListSeparator + " ";
                PropertyDescriptorCollection props = GetProperties(value)!;
                string?[] args = new string[props.Count];

                for (int i = 0; i < props.Count; i++)
                {
                    object propValue = props[i].GetValue(value)!;
                    args[i] = TypeDescriptor.GetConverter(propValue).ConvertToString(context, culture, propValue);
                }

                return string.Join(sep, args);
            }

            if (destinationType == typeof(DateTime))
            {
                return range.Start;
            }

            if (destinationType == typeof(InstanceDescriptor))
            {
                ConstructorInfo? ctor = typeof(SelectionRange).GetConstructor(
                [
                    typeof(DateTime), typeof(DateTime)
                ]);
                if (ctor is not null)
                {
                    return new InstanceDescriptor(ctor, new object[] { range.Start, range.End });
                }
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    ///  Creates an instance of this type given a set of property values
    ///  for the object. This is useful for objects that are immutable, but still
    ///  want to provide changeable properties.
    /// </summary>
    public override object CreateInstance(ITypeDescriptorContext? context, IDictionary propertyValues)
    {
        try
        {
            return new SelectionRange((DateTime)propertyValues["Start"]!,
                                      (DateTime)propertyValues["End"]!);
        }
        catch (InvalidCastException invalidCast)
        {
            throw new ArgumentException(SR.PropertyValueInvalidEntry, invalidCast);
        }
        catch (NullReferenceException nullRef)
        {
            throw new ArgumentException(SR.PropertyValueInvalidEntry, nullRef);
        }
    }

    /// <summary>
    ///  Determines if changing a value on this object should require a call to
    ///  CreateInstance to create a new value.
    /// </summary>
    public override bool GetCreateInstanceSupported(ITypeDescriptorContext? context)
    {
        return true;
    }

    /// <summary>
    ///  Retrieves the set of properties for this type. By default, a type has
    ///  does not return any properties. An easy implementation of this method
    ///  can just call TypeDescriptor.GetProperties for the correct data type.
    /// </summary>
    [RequiresUnreferencedCode(TrimmingConstants.TypeConverterGetPropertiesMessage)]
    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
    {
        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(SelectionRange), attributes);
        return props.Sort(["Start", "End"]);
    }

    /// <summary>
    ///  Determines if this object supports properties. By default, this
    ///  is false.
    /// </summary>
    public override bool GetPropertiesSupported(ITypeDescriptorContext? context)
    {
        return true;
    }
}
