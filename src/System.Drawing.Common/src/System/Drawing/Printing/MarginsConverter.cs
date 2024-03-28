// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Drawing.Printing;

/// <summary>
///  Provides a type converter to convert <see cref='Margins'/> to and from various other representations, such as a string.
/// </summary>
public class MarginsConverter : ExpandableObjectConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
        => destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is not string strValue)
        {
            return base.ConvertFrom(context, culture, value);
        }

        string text = strValue.Trim();

        if (text.Length == 0)
        {
            return null;
        }
        else
        {
            // Parse 4 integer values.
            culture ??= CultureInfo.CurrentCulture;
            char sep = culture.TextInfo.ListSeparator[0];
            string[] tokens = text.Split(sep);
            int[] values = new int[tokens.Length];
            TypeConverter intConverter = GetIntConverter();

            for (int i = 0; i < values.Length; i++)
            {
                // Note: ConvertFromString will raise exception if value cannot be converted.
                values[i] = (int)intConverter.ConvertFromString(context, culture, tokens[i])!;
            }

            if (values.Length != 4)
            {
                throw new ArgumentException(SR.Format(SR.TextParseFailedFormat, text, "left, right, top, bottom"));
            }

            return new Margins(values[0], values[1], values[2], values[3]);
        }
    }

    [UnconditionalSuppressMessage(
        "ReflectionAnalysis",
        "IL2026:RequiresUnreferencedCode",
        Justification = "TypeDescriptor.GetConverter is safe for primitive types.")]
    private static TypeConverter GetIntConverter() => TypeDescriptor.GetConverter(typeof(int));

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(destinationType);

        if (value is Margins margins)
        {
            if (destinationType == typeof(string))
            {
                culture ??= CultureInfo.CurrentCulture;
                string sep = culture.TextInfo.ListSeparator + " ";
                TypeConverter intConverter = GetIntConverter();
                string?[] args = new string[4];
                int nArg = 0;

                // Note: ConvertToString will raise exception if value cannot be converted.
                args[nArg++] = intConverter.ConvertToString(context, culture, margins.Left);
                args[nArg++] = intConverter.ConvertToString(context, culture, margins.Right);
                args[nArg++] = intConverter.ConvertToString(context, culture, margins.Top);
                args[nArg++] = intConverter.ConvertToString(context, culture, margins.Bottom);

                return string.Join(sep, args);
            }

            if (destinationType == typeof(InstanceDescriptor))
            {
                if (typeof(Margins).GetConstructor([typeof(int), typeof(int), typeof(int), typeof(int)]) is { } constructor)
                {
                    return new InstanceDescriptor(
                        constructor,
                        new object[] { margins.Left, margins.Right, margins.Top, margins.Bottom });
                }
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool GetCreateInstanceSupported(ITypeDescriptorContext? context) => true;

    public override object CreateInstance(ITypeDescriptorContext? context, IDictionary propertyValues)
    {
        ArgumentNullException.ThrowIfNull(propertyValues);

        object? left = propertyValues["Left"];
        object? right = propertyValues["Right"];
        object? top = propertyValues["Top"];
        object? bottom = propertyValues["Bottom"];

        return left is not int || right is not int || bottom is not int || top is not int
            ? throw new ArgumentException(SR.PropertyValueInvalidEntry)
            : (object)new Margins((int)left, (int)right, (int)top, (int)bottom);
    }
}
